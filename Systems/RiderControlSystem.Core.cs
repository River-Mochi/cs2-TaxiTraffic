// File: Systems/RiderControlSystem.Core.cs
// Purpose: Demand-side taxi control (SAFE variant):
// - Uses a stable per-resident taxi permission slider
// - Applies ResidentFlags.IgnoreTaxi only to residents outside the allowed taxi bucket
// - Unwinds blocked residents from taxi waiting states so cims don't freeze
// Notes:
// - Never touch Deleted/Temp entities.
// - Never use SystemAPI from static methods (Entities source-gen limitation).

namespace RiderControl
{
    using CS2Shared.RiverMochi; // LogUtils
    using Game;
    using Game.Citizens;        // Citizen, HouseholdMember, CommuterHousehold, TouristHousehold
    using Game.Common;          // Deleted
    using Game.Creatures;       // ResidentFlags, HumanCurrentLane, CreatureLaneFlags, RideNeeder
    using Game.Pathfind;        // PathOwner, PathFlags
    using Game.Routes;          // TaxiStand, BoardingVehicle, Connected
    using Game.Tools;           // Temp
    using Game.Vehicles;        // Taxi
    using Unity.Collections;
    using Unity.Entities;
    using CreatureResident = Game.Creatures.Resident;
    using UTime = UnityEngine.Time;

    public partial class RiderControlSystem : GameSystemBase
    {
        // -----------------------
        // Knobs (perf + behavior)
        // -----------------------

        // Batch size for applying/removing eligibility marks each update (limits hitching in huge cities).
        private const int kMarkBatchPerUpdate = 2000;

        // Unstick taxi waiting states on an interval (not every frame).
        private const float kUnstickIntervalSeconds = 1.0f;

        // Verbose TaxiSummary log interval.
        // Increase this if log is too noisy and to prevent huge log files.
        private const float kDebugSummaryIntervalSeconds = 120.0f;

        // Mod-local salt for stable taxi eligibility buckets.
        // This keeps the slider independent from vanilla's own pseudo-random "reasons".
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        // -----------------------
        // Timers / setting cache
        // -----------------------

        private float m_UnstickTimer;

        private int m_LastResidentsAllowedToUseTaxis = int.MinValue;
        private bool m_LastBlockCommuters;
        private bool m_LastBlockTourists;
        private bool m_TaxiEligibilityResetInProgress;

        protected override void OnCreate()
        {
            base.OnCreate();

            InitStatusSystemsOnCreate();

            // Only run after a real city is loaded.
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            bool isRealGame =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                 purpose == Colossal.Serialization.Entities.Purpose.LoadGame);

            if (!isRealGame)
                return;

            m_UnstickTimer = 0f;

            m_LastResidentsAllowedToUseTaxis = int.MinValue;
            m_LastBlockCommuters = false;
            m_LastBlockTourists = false;
            m_TaxiEligibilityResetInProgress = false;

            ResetDebugOnCityLoaded();
            ResetStatusOnCityLoaded();

            Enabled = true;

#if DEBUG
            LogUtils.Info(Mod.s_Log, () => $"{Mod.ModTag} RiderControlSystem enabled (city load complete).");
#endif
        }

        protected override void OnUpdate()
        {
            Setting? setting = Mod.Setting;
            if (setting is null)
            {
                Enabled = false;
                return;
            }

            int appliedIgnoreTaxi = 0;
            int skippedCommuters = 0;
            int skippedTourists = 0;

            int clearedTaxiLaneWaiting = 0;
            int clearedTaxiStandWaiting = 0;
            int clearedRideNeederLinks = 0;

            bool changed = DetectTaxiEligibilitySettingChange(setting);
            if (changed)
            {
                m_TaxiEligibilityResetInProgress = true;
            }

            // Setting changes must clear old buckets before applying the new stable bucket.
            if (m_TaxiEligibilityResetInProgress)
            {
                int resetCount = ResetTaxiEligibilityMarkersBatch();
                if (resetCount > 0)
                {
                    TickStatusSnapshot();

                    if (setting.EnableDebugLogging)
                        TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                    return;
                }

                m_TaxiEligibilityResetInProgress = false;
            }

            // 100% is the clean OFF/vanilla-style state for this mod's taxi restriction.
            if (setting.ResidentsAllowedToUseTaxis >= Setting.TaxiAllowedPercentMax)
            {
                UnmarkIgnoreTaxiBatch(out _);

                TickStatusSnapshot();

                if (setting.EnableDebugLogging)
                    TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                return;
            }

            ApplyTaxiEligibilityBatch(setting, out appliedIgnoreTaxi, out skippedCommuters, out skippedTourists);

            // Unstick pass (interval-based).
            m_UnstickTimer += UTime.unscaledDeltaTime;
            if (m_UnstickTimer >= kUnstickIntervalSeconds)
            {
                m_UnstickTimer = 0f;

                UnstickTaxiLaneWaiters(setting, out clearedTaxiLaneWaiting, out clearedRideNeederLinks);
                UnstickTaxiQueues(setting, out clearedTaxiStandWaiting);
            }

            // Status fields (defined in Status partial).
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastSkippedCommuters = skippedCommuters;
            s_StatusLastSkippedTourists = skippedTourists;
            s_StatusLastClearedTaxiLaneWaiting = clearedTaxiLaneWaiting;
            s_StatusLastClearedTaxiStandWaiting = clearedTaxiStandWaiting;
            s_StatusLastRemovedRideNeeder = clearedRideNeederLinks;

            TickStatusSnapshot();

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);
        }

        // -----------------------
        // Marker / eligibility
        // -----------------------

        private bool DetectTaxiEligibilitySettingChange(Setting setting)
        {
            int allowed = setting.ResidentsAllowedToUseTaxis;

            bool changed =
                m_LastResidentsAllowedToUseTaxis != allowed ||
                m_LastBlockCommuters != setting.BlockCommuters ||
                m_LastBlockTourists != setting.BlockTourists;

            if (!changed)
                return false;

            m_LastResidentsAllowedToUseTaxis = allowed;
            m_LastBlockCommuters = setting.BlockCommuters;
            m_LastBlockTourists = setting.BlockTourists;

            return true;
        }

        private int ResetTaxiEligibilityMarkersBatch()
        {
            int resetCount = 0;

            using NativeList<Entity> blockedMarks = new NativeList<Entity>(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new NativeList<Entity>(Allocator.Temp);

            foreach ((RefRW<CreatureResident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<CreatureResident>>()
                         .WithAll<IgnoreTaxiMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                blockedMarks.Add(entity);

                resetCount++;
                if (resetCount >= kMarkBatchPerUpdate)
                    break;
            }

            if (resetCount < kMarkBatchPerUpdate)
            {
                foreach ((RefRO<CreatureResident> _, Entity entity) in SystemAPI
                             .Query<RefRO<CreatureResident>>()
                             .WithAll<TaxiAllowedMark>()
                             .WithNone<Deleted, Temp>()
                             .WithEntityAccess())
                {
                    allowedMarks.Add(entity);

                    resetCount++;
                    if (resetCount >= kMarkBatchPerUpdate)
                        break;
                }
            }

            if (blockedMarks.Length > 0)
                EntityManager.RemoveComponent<IgnoreTaxiMark>(blockedMarks.AsArray());

            if (allowedMarks.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(allowedMarks.AsArray());

            return resetCount;
        }

        private void UnmarkIgnoreTaxiBatch(out int unmarkedCount)
        {
            unmarkedCount = 0;

            using NativeList<Entity> toUnmark = new NativeList<Entity>(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new NativeList<Entity>(Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<CreatureResident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<CreatureResident>>()
                         .WithAll<IgnoreTaxiMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                toUnmark.Add(entity);

                processed++;
                if (processed >= kMarkBatchPerUpdate)
                    break;
            }

            if (processed < kMarkBatchPerUpdate)
            {
                foreach ((RefRO<CreatureResident> _, Entity entity) in SystemAPI
                             .Query<RefRO<CreatureResident>>()
                             .WithAll<TaxiAllowedMark>()
                             .WithNone<Deleted, Temp>()
                             .WithEntityAccess())
                {
                    allowedMarks.Add(entity);

                    processed++;
                    if (processed >= kMarkBatchPerUpdate)
                        break;
                }
            }

            if (toUnmark.Length > 0)
            {
                EntityManager.RemoveComponent<IgnoreTaxiMark>(toUnmark.AsArray());
                unmarkedCount = toUnmark.Length;
            }

            if (allowedMarks.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(allowedMarks.AsArray());
        }

        private void ApplyTaxiEligibilityBatch(
            Setting setting,
            out int applied,
            out int skippedCommuters,
            out int skippedTourists)
        {
            applied = 0;
            skippedCommuters = 0;
            skippedTourists = 0;

            using NativeList<Entity> toBlock = new NativeList<Entity>(Allocator.Temp);
            using NativeList<Entity> toAllow = new NativeList<Entity>(Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<CreatureResident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<CreatureResident>>()
                         .WithNone<IgnoreTaxiMark, TaxiAllowedMark, Deleted>()
                         .WithNone<Temp>()
                         .WithEntityAccess())
            {
                processed++;

                bool shouldBlock = ShouldResidentIgnoreTaxiBySettings(
                    setting,
                    resident.ValueRO,
                    out bool skippedCommuter,
                    out bool skippedTourist);

                if (skippedCommuter)
                    skippedCommuters++;

                if (skippedTourist)
                    skippedTourists++;

                if (shouldBlock)
                {
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;
                    toBlock.Add(entity);
                    applied++;
                }
                else
                {
                    // Mark as checked so the batch can progress through very large populations.
                    toAllow.Add(entity);
                }

                if (processed >= kMarkBatchPerUpdate)
                    break;
            }

            if (toBlock.Length > 0)
                EntityManager.AddComponent<IgnoreTaxiMark>(toBlock.AsArray());

            if (toAllow.Length > 0)
                EntityManager.AddComponent<TaxiAllowedMark>(toAllow.AsArray());
        }

        private bool ShouldResidentIgnoreTaxiBySettings(
            Setting setting,
            CreatureResident resident,
            out bool skippedCommuter,
            out bool skippedTourist)
        {
            skippedCommuter = false;
            skippedTourist = false;

            int allowedPercent = setting.ResidentsAllowedToUseTaxis;

            if (allowedPercent >= Setting.TaxiAllowedPercentMax)
                return false;

            Entity citizenEntity = resident.m_Citizen;

            if (citizenEntity != Entity.Null && SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
            {
                Entity household =
                    SystemAPI.GetComponentRO<HouseholdMember>(citizenEntity).ValueRO.m_Household;

                if (household != Entity.Null)
                {
                    if (!setting.BlockCommuters && SystemAPI.HasComponent<CommuterHousehold>(household))
                    {
                        skippedCommuter = true;
                        return false;
                    }

                    if (!setting.BlockTourists && SystemAPI.HasComponent<TouristHousehold>(household))
                    {
                        skippedTourist = true;
                        return false;
                    }
                }
            }

            if (allowedPercent <= Setting.TaxiAllowedPercentMin)
                return true;

            if (citizenEntity == Entity.Null || !SystemAPI.HasComponent<Citizen>(citizenEntity))
            {
                // No citizen component means no stable citizen random bucket; keep strong-block behavior.
                return true;
            }

            Citizen citizen = SystemAPI.GetComponentRO<Citizen>(citizenEntity).ValueRO;

            // Stable bucket: same citizen should stay in the same taxi-allowed bucket after reload.
            // Use a mod-local hash instead of a vanilla pseudo-random "reason" so this choice stays
            // independent from unrelated game systems such as car-keeping behavior.
            uint roll = GetStableTaxiEligibilityRoll(citizen);

            return roll >= (uint)allowedPercent;
        }

        private static uint GetStableTaxiEligibilityRoll(Citizen citizen)
        {
            // Citizen.m_PseudoRandom is saved by the game, so this bucket survives save/load.
            uint seed = ((uint)citizen.m_PseudoRandom << 16) | citizen.m_PseudoRandom;
            uint hash = seed ^ kTaxiEligibilityHashSalt;

            // Small 32-bit avalanche mix. Deterministic, fast, and independent from vanilla helpers.
            hash ^= hash >> 16;
            hash *= 0x7feb352du;
            hash ^= hash >> 15;
            hash *= 0x846ca68bu;
            hash ^= hash >> 16;

            return hash % 100u;
        }

        // -----------------------
        // Taxi waiting cleanup
        // -----------------------

        private void UnstickTaxiLaneWaiters(
            Setting setting,
            out int clearedTaxiLaneWaiting,
            out int clearedRideNeederLinks)
        {
            clearedTaxiLaneWaiting = 0;
            clearedRideNeederLinks = 0;

            using NativeList<Entity> toBlockMark = new NativeList<Entity>(Allocator.Temp);
            using NativeList<Entity> toRemoveAllowedMark = new NativeList<Entity>(Allocator.Temp);

            foreach ((RefRW<RideNeeder> rn,
                      RefRW<HumanCurrentLane> lane,
                      RefRW<PathOwner> pathOwner,
                      Entity entity) in SystemAPI
                         .Query<RefRW<RideNeeder>, RefRW<HumanCurrentLane>, RefRW<PathOwner>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                var taxiWaitMask = CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi;

                if ((lane.ValueRO.m_Flags & taxiWaitMask) != taxiWaitMask)
                    continue;

                if (!SystemAPI.HasComponent<CreatureResident>(entity))
                    continue;

                RefRW<CreatureResident> resident = SystemAPI.GetComponentRW<CreatureResident>(entity);

                if (!ShouldResidentIgnoreTaxiBySettings(setting, resident.ValueRO, out _, out _))
                    continue;

                // Enforce IgnoreTaxi only for the blocked resident being unstuck.
                if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) == 0)
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                if (!SystemAPI.HasComponent<IgnoreTaxiMark>(entity))
                    toBlockMark.Add(entity);

                if (SystemAPI.HasComponent<TaxiAllowedMark>(entity))
                    toRemoveAllowedMark.Add(entity);

                lane.ValueRW.m_Flags &= ~taxiWaitMask;
                lane.ValueRW.m_QueueEntity = Entity.Null;

                if (rn.ValueRO.m_RideRequest != Entity.Null)
                {
                    rn.ValueRW.m_RideRequest = Entity.Null;
                    clearedRideNeederLinks++;
                }

                pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

                clearedTaxiLaneWaiting++;
            }

            if (toRemoveAllowedMark.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(toRemoveAllowedMark.AsArray());

            if (toBlockMark.Length > 0)
                EntityManager.AddComponent<IgnoreTaxiMark>(toBlockMark.AsArray());
        }

        private void UnstickTaxiQueues(Setting setting, out int clearedTaxiStandWaiting)
        {
            clearedTaxiStandWaiting = 0;

            using NativeList<Entity> toBlockMark = new NativeList<Entity>(Allocator.Temp);
            using NativeList<Entity> toRemoveAllowedMark = new NativeList<Entity>(Allocator.Temp);

            foreach ((RefRW<CreatureResident> resident,
                      RefRW<HumanCurrentLane> lane,
                      RefRW<PathOwner> pathOwner,
                      Entity entity) in SystemAPI
                         .Query<RefRW<CreatureResident>, RefRW<HumanCurrentLane>, RefRW<PathOwner>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                if ((resident.ValueRO.m_Flags & ResidentFlags.WaitingTransport) == 0)
                    continue;

                Entity queueEntity = lane.ValueRO.m_QueueEntity;
                if (queueEntity == Entity.Null)
                    continue;

                if (!IsTaxiQueueEntity(queueEntity))
                    continue;

                if (!ShouldResidentIgnoreTaxiBySettings(setting, resident.ValueRO, out _, out _))
                    continue;

                // Enforce IgnoreTaxi for the resident being unstuck (targeted, no citywide sweep).
                if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) == 0)
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                if (!SystemAPI.HasComponent<IgnoreTaxiMark>(entity))
                    toBlockMark.Add(entity);

                if (SystemAPI.HasComponent<TaxiAllowedMark>(entity))
                    toRemoveAllowedMark.Add(entity);

                resident.ValueRW.m_Flags &= ~ResidentFlags.WaitingTransport;
                lane.ValueRW.m_QueueEntity = Entity.Null;
                lane.ValueRW.m_Flags &= ~(CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi);

                pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

                clearedTaxiStandWaiting++;
            }

            if (toRemoveAllowedMark.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(toRemoveAllowedMark.AsArray());

            if (toBlockMark.Length > 0)
                EntityManager.AddComponent<IgnoreTaxiMark>(toBlockMark.AsArray());
        }

        private bool IsTaxiQueueEntity(Entity queueEntity)
        {
            if (queueEntity == Entity.Null)
                return false;

            if (IsDirectTaxiQueueEntity(queueEntity))
                return true;

            // Vanilla route waiting can point at a Connected waypoint first.
            for (int i = 0; i < 3; i++)
            {
                if (!SystemAPI.HasComponent<Connected>(queueEntity))
                    return false;

                Entity connected = SystemAPI.GetComponentRO<Connected>(queueEntity).ValueRO.m_Connected;
                if (connected == Entity.Null || connected == queueEntity)
                    return false;

                if (IsDirectTaxiQueueEntity(connected))
                    return true;

                queueEntity = connected;
            }

            return false;
        }

        private bool IsDirectTaxiQueueEntity(Entity queueEntity)
        {
            if (SystemAPI.HasComponent<TaxiStand>(queueEntity))
                return true;

            if (!SystemAPI.HasComponent<BoardingVehicle>(queueEntity))
                return false;

            BoardingVehicle boardingVehicle = SystemAPI.GetComponentRO<BoardingVehicle>(queueEntity).ValueRO;
            return boardingVehicle.m_Vehicle != Entity.Null &&
                   SystemAPI.HasComponent<Taxi>(boardingVehicle.m_Vehicle);
        }
    }
}
