// <copyright file="TaxiTrafficSystem.Core.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Core.cs
// Taxi eligibility, outside taxi-supply control, and waiting cleanup.

namespace TaxiTraffic
{
    using Game;                 // GameSystemBase
    using Game.Citizens;        // Citizen, HouseholdMember, CommuterHousehold, TouristHousehold
    using Game.Common;          // Deleted
    using Game.Creatures;       // ResidentFlags, HumanCurrentLane, CreatureLaneFlags, RideNeeder, GroupMember, GroupCreature
    using Game.Objects;         // TripSource
    using Game.Pathfind;        // PathOwner, PathFlags
    using Game.Routes;          // TaxiStand, BoardingVehicle, Connected
    using Game.Tools;           // Temp
    using Game.Vehicles;        // Taxi
    using Unity.Collections;    // NativeList, Allocator
    using Unity.Entities;       // Entity, RefRW, RefRO

    public partial class TaxiTrafficSystem : GameSystemBase
    {
        private const int kMarkBatchPerUpdate = 2000;
        private const int kUpdateIntervalFrames = 16;
        private const float kUnstickIntervalSeconds = 1.0f;
        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        private double m_LastUnstickRealtime;

        private int m_LastResidentsAllowedToUseTaxis = int.MinValue;
        private bool m_LastBlockCommuters;
        private bool m_LastBlockTourists;
        private bool m_TaxiEligibilityResetInProgress;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return kUpdateIntervalFrames;
        }

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

            m_LastUnstickRealtime = UnityEngine.Time.realtimeSinceStartupAsDouble;

            m_LastResidentsAllowedToUseTaxis = int.MinValue;
            m_LastBlockCommuters = false;
            m_LastBlockTourists = false;
            m_TaxiEligibilityResetInProgress = false;

            ResetDebugOnCityLoaded();
            ResetStatusOnCityLoaded();

            Enabled = true;

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(Mod.s_Log, () => $"{Mod.ModTag} TaxiTrafficSystem enabled (city load complete).");
#endif
        }

        protected override void OnUpdate()
        {
            TaxiSettings? setting = Mod.Setting;
            if (setting is null)
            {
                Enabled = false;
                return;
            }

            int appliedIgnoreTaxi = 0;
            int skippedCommuters = 0;
            int skippedTourists = 0;
            int skippedGroupTravelers = 0;

            int clearedTaxiLaneWaiting = 0;
            int clearedTaxiStandWaiting = 0;
            int removedRideNeeders = 0;

            // Keep outside control independent from the local-rider settings/reset cycle.
            SuppressOutsideTaxiSupplyRequests(setting);

            // Do not sweep all marked residents every update; too expensive in large cities.
            int clearedGroupTravelers = 0;

            bool changed = DetectTaxiEligibilitySettingChange(setting);
            if (changed)
                m_TaxiEligibilityResetInProgress = true;

            // Setting changes clear old buckets before applying the new stable bucket.
            if (m_TaxiEligibilityResetInProgress)
            {
                int resetCount = ResetTaxiEligibilityMarkersBatch();

                RecordLastUpdateCounters(
                    appliedIgnoreTaxi,
                    skippedCommuters,
                    skippedTourists,
                    skippedGroupTravelers,
                    clearedGroupTravelers,
                    clearedTaxiLaneWaiting,
                    clearedTaxiStandWaiting,
                    removedRideNeeders);

                if (resetCount > 0)
                {
                    if (setting.EnableDebugLogging)
                        TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                    return;
                }

                m_TaxiEligibilityResetInProgress = false;
            }

            bool vanillaResidents = setting.ResidentsAllowedToUseTaxis >= TaxiSettings.kTaxiAllowedPercentMax;
            bool vanillaGroups = !setting.BlockCommuters && !setting.BlockTourists;

            if (vanillaResidents && vanillaGroups)
            {
                UnmarkIgnoreTaxiBatch(out _);

                RecordLastUpdateCounters(
                    appliedIgnoreTaxi,
                    skippedCommuters,
                    skippedTourists,
                    skippedGroupTravelers,
                    clearedGroupTravelers,
                    clearedTaxiLaneWaiting,
                    clearedTaxiStandWaiting,
                    removedRideNeeders);

                if (setting.EnableDebugLogging)
                    TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                return;
            }

            ApplyTaxiEligibilityBatch(
                setting,
                out appliedIgnoreTaxi,
                out skippedCommuters,
                out skippedTourists,
                out skippedGroupTravelers);

            // Vanilla clears IgnoreTaxi at arrival; reused creatures keep our blocked marker.
            RepairStaleIgnoreTaxiOnTripStart();

            // RideNeeder is already a narrow archetype, so stop invalid taxi requests every system update.
            UnstickTaxiLaneWaiters(setting, out clearedTaxiLaneWaiting, out removedRideNeeders);

            // The broader waiting-transport scan stays throttled.
            double now = UnityEngine.Time.realtimeSinceStartupAsDouble;
            if (now - m_LastUnstickRealtime >= kUnstickIntervalSeconds)
            {
                m_LastUnstickRealtime = now;
                UnstickTaxiQueues(setting, out clearedTaxiStandWaiting);
            }

            RecordLastUpdateCounters(
                appliedIgnoreTaxi,
                skippedCommuters,
                skippedTourists,
                skippedGroupTravelers,
                clearedGroupTravelers,
                clearedTaxiLaneWaiting,
                clearedTaxiStandWaiting,
                removedRideNeeders);

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);
        }

        private static void RecordLastUpdateCounters(
            int appliedIgnoreTaxi,
            int skippedCommuters,
            int skippedTourists,
            int skippedGroupTravelers,
            int clearedGroupTravelers,
            int clearedTaxiLaneWaiting,
            int clearedTaxiStandWaiting,
            int removedRideNeeders)
        {
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastSkippedCommuters = skippedCommuters;
            s_StatusLastSkippedTourists = skippedTourists;
            s_StatusLastSkippedGroupTravelers = skippedGroupTravelers;
            s_StatusLastClearedGroupTravelers = clearedGroupTravelers;
            s_StatusLastClearedTaxiLaneWaiting = clearedTaxiLaneWaiting;
            s_StatusLastClearedTaxiStandWaiting = clearedTaxiStandWaiting;
            s_StatusLastRemovedRideNeeder = removedRideNeeders;
        }

        // -----------------------
        // Outside taxi supply
        // -----------------------

        private void SuppressOutsideTaxiSupplyRequests(TaxiSettings setting)
        {
            if (!setting.BlockOutsideTaxis)
                return;

            using NativeList<Entity> toDestroy = new(Allocator.Temp);

            foreach ((RefRO<Game.Simulation.TaxiRequest> requestRef,
                      RefRO<Game.Simulation.ServiceRequest> serviceRef,
                      Entity requestEntity) in SystemAPI
                         .Query<RefRO<Game.Simulation.TaxiRequest>, RefRO<Game.Simulation.ServiceRequest>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                Game.Simulation.TaxiRequest request = requestRef.ValueRO;
                if (request.m_Type != Game.Simulation.TaxiRequestType.Outside)
                    continue;

                // Reversed Outside requests advertise taxi supply; rider requests are not reversed.
                if ((serviceRef.ValueRO.m_Flags & Game.Simulation.ServiceRequestFlags.Reversed) == 0)
                    continue;

                toDestroy.Add(requestEntity);
            }

            if (toDestroy.Length == 0)
                return;

            EntityManager.DestroyEntity(toDestroy.AsArray());
            s_StatusOutsideSupplySuppressedTotal += toDestroy.Length;
        }

        // -----------------------
        // Marker / eligibility
        // -----------------------

        private bool DetectTaxiEligibilitySettingChange(TaxiSettings setting)
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

            using NativeList<Entity> blockedMarks = new(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
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
                foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                             .Query<RefRO<Resident>>()
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

        private int ClearGroupLinkedTaxiMarksBatch()
        {
            int cleared = 0;

            using NativeList<Entity> blockedMarks = new(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithAll<IgnoreTaxiMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                if (!IsGroupLinkedTraveler(entity))
                    continue;

                resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                blockedMarks.Add(entity);

                if (SystemAPI.HasComponent<TaxiAllowedMark>(entity))
                    allowedMarks.Add(entity);

                cleared++;
                if (cleared >= kMarkBatchPerUpdate)
                    break;
            }

            if (blockedMarks.Length > 0)
                EntityManager.RemoveComponent<IgnoreTaxiMark>(blockedMarks.AsArray());

            if (allowedMarks.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(allowedMarks.AsArray());

            return cleared;
        }

        private void UnmarkIgnoreTaxiBatch(out int unmarkedCount)
        {
            unmarkedCount = 0;

            using NativeList<Entity> toUnmark = new(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new(Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
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
                foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                             .Query<RefRO<Resident>>()
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
            TaxiSettings setting,
            out int applied,
            out int skippedCommuters,
            out int skippedTourists,
            out int skippedGroupTravelers)
        {
            applied = 0;
            skippedCommuters = 0;
            skippedTourists = 0;
            skippedGroupTravelers = 0;

            using NativeList<Entity> toBlock = new(Allocator.Temp);
            using NativeList<Entity> toAllow = new(Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithNone<IgnoreTaxiMark, TaxiAllowedMark, Deleted>()
                         .WithNone<Temp>()
                         .WithEntityAccess())
            {
                processed++;

                if (IsGroupLinkedTraveler(entity))
                {
                    skippedGroupTravelers++;

                    // Mark as checked so the batch keeps moving through the active population.
                    toAllow.Add(entity);

                    if (processed >= kMarkBatchPerUpdate)
                        break;

                    continue;
                }

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

                    // TripNeeded can copy a taxi-capable Citizen path before we mark the Resident.
                    if (SystemAPI.HasComponent<TripSource>(entity) && SystemAPI.HasComponent<PathOwner>(entity))
                    {
                        RefRW<PathOwner> pathOwner = SystemAPI.GetComponentRW<PathOwner>(entity);
                        pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                        pathOwner.ValueRW.m_State |= PathFlags.Obsolete;
                    }
                }
                else
                {
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

        private void RepairStaleIgnoreTaxiOnTripStart()
        {
#if DEBUG
            int repaired = 0;
#endif

            foreach ((RefRW<Resident> resident, RefRW<PathOwner> pathOwner) in SystemAPI
                         .Query<RefRW<Resident>, RefRW<PathOwner>>()
                         .WithAll<IgnoreTaxiMark, TripSource>()
                         .WithNone<GroupMember, Deleted, Temp>()
                         .WithNone<GroupCreature>())
            {
                if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) != 0)
                    continue;

                resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                // ResetTrip can copy a taxi-capable path onto the reused creature.
                pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

#if DEBUG
                repaired++;
#endif
            }

#if DEBUG
            DebugRecordTripSourceRepairs(repaired);
#endif
        }

        private bool ShouldResidentIgnoreTaxiBySettings(
            TaxiSettings setting,
            Resident resident,
            out bool skippedCommuter,
            out bool skippedTourist)
        {
            skippedCommuter = false;
            skippedTourist = false;

            Entity citizenEntity = resident.m_Citizen;

            if (citizenEntity != Entity.Null && SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
            {
                Entity household =
                    SystemAPI.GetComponentRO<HouseholdMember>(citizenEntity).ValueRO.m_Household;

                if (household != Entity.Null)
                {
                    if (SystemAPI.HasComponent<CommuterHousehold>(household))
                    {
                        if (setting.BlockCommuters)
                            return true;

                        skippedCommuter = true;
                        return false;
                    }

                    if (SystemAPI.HasComponent<TouristHousehold>(household))
                    {
                        if (setting.BlockTourists)
                            return true;

                        skippedTourist = true;
                        return false;
                    }
                }
            }

            int allowedPercent = setting.ResidentsAllowedToUseTaxis;

            if (allowedPercent >= TaxiSettings.kTaxiAllowedPercentMax)
                return false;

            if (allowedPercent <= TaxiSettings.kTaxiAllowedPercentMin)
                return true;

            if (citizenEntity == Entity.Null || !SystemAPI.HasComponent<Citizen>(citizenEntity))
            {
                // No Citizen component means no stable saved bucket; keep strong-block behavior.
                return true;
            }

            Citizen citizen = SystemAPI.GetComponentRO<Citizen>(citizenEntity).ValueRO;
            uint roll = GetStableTaxiEligibilityRoll(citizen);

            return roll >= (uint)allowedPercent;
        }

        private static uint GetStableTaxiEligibilityRoll(Citizen citizen)
        {
            uint seed = ((uint)citizen.m_PseudoRandom << 16) | citizen.m_PseudoRandom;
            uint hash = seed ^ kTaxiEligibilityHashSalt;

            hash ^= hash >> 16;
            hash *= 0x7feb352du;
            hash ^= hash >> 15;
            hash *= 0x846ca68bu;
            hash ^= hash >> 16;

            return hash % 100u;
        }

        private bool IsGroupLinkedTraveler(Entity entity)
        {
            return SystemAPI.HasComponent<GroupMember>(entity) ||
                   SystemAPI.HasBuffer<GroupCreature>(entity);
        }

        // -----------------------
        // Taxi waiting cleanup
        // -----------------------

        private void UnstickTaxiLaneWaiters(
            TaxiSettings setting,
            out int clearedTaxiLaneWaiting,
            out int removedRideNeeders)
        {
            clearedTaxiLaneWaiting = 0;
            removedRideNeeders = 0;

            using NativeList<Entity> toBlockMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveAllowedMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveRideNeeder = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident,
                      RefRW<HumanCurrentLane> lane,
                      RefRW<PathOwner> pathOwner,
                      Entity entity) in SystemAPI
                         .Query<RefRW<Resident>, RefRW<HumanCurrentLane>, RefRW<PathOwner>>()
                         .WithAll<RideNeeder>()
                         .WithNone<GroupMember, Deleted, Temp>()
                         .WithNone<GroupCreature>()
                         .WithEntityAccess())
            {
                CreatureLaneFlags taxiWaitMask = CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi;

                if ((lane.ValueRO.m_Flags & taxiWaitMask) != taxiWaitMask)
                    continue;

                if (!ShouldResidentIgnoreTaxiBySettings(setting, resident.ValueRO, out _, out _))
                    continue;

                if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) == 0)
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                if (!SystemAPI.HasComponent<IgnoreTaxiMark>(entity))
                    toBlockMark.Add(entity);

                if (SystemAPI.HasComponent<TaxiAllowedMark>(entity))
                    toRemoveAllowedMark.Add(entity);

                lane.ValueRW.m_Flags &= ~taxiWaitMask;
                lane.ValueRW.m_QueueEntity = Entity.Null;

                pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

                // Removing RideNeeder makes TaxiDispatch invalidate the old request instead of relinking it.
                toRemoveRideNeeder.Add(entity);
                clearedTaxiLaneWaiting++;
            }

            if (toRemoveAllowedMark.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(toRemoveAllowedMark.AsArray());

            if (toBlockMark.Length > 0)
                EntityManager.AddComponent<IgnoreTaxiMark>(toBlockMark.AsArray());

            if (toRemoveRideNeeder.Length > 0)
            {
                EntityManager.RemoveComponent<RideNeeder>(toRemoveRideNeeder.AsArray());
                removedRideNeeders = toRemoveRideNeeder.Length;
            }
        }

        private void UnstickTaxiQueues(TaxiSettings setting, out int clearedTaxiStandWaiting)
        {
#if DEBUG
            long debugStartTicks = DebugGetTimestamp();
            int debugScanned = 0;
            int debugWaitingTransport = 0;
            int debugTaxiQueue = 0;
#endif

            clearedTaxiStandWaiting = 0;

            using NativeList<Entity> toBlockMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveAllowedMark = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident,
                      RefRW<HumanCurrentLane> lane,
                      RefRW<PathOwner> pathOwner,
                      Entity entity) in SystemAPI
                         .Query<RefRW<Resident>, RefRW<HumanCurrentLane>, RefRW<PathOwner>>()
                         .WithNone<GroupMember, Deleted, Temp>()
                         .WithNone<GroupCreature>()
                         .WithEntityAccess())
            {
#if DEBUG
                debugScanned++;
#endif

                if ((resident.ValueRO.m_Flags & ResidentFlags.WaitingTransport) == 0)
                    continue;

#if DEBUG
                debugWaitingTransport++;
#endif

                Entity queueEntity = lane.ValueRO.m_QueueEntity;
                if (queueEntity == Entity.Null)
                    continue;

                if (!IsTaxiQueueEntity(queueEntity))
                    continue;

#if DEBUG
                debugTaxiQueue++;
#endif

                if (!ShouldResidentIgnoreTaxiBySettings(setting, resident.ValueRO, out _, out _))
                    continue;

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

#if DEBUG
            DebugRecordUnstickTaxiQueues(
                debugStartTicks,
                debugScanned,
                debugWaitingTransport,
                debugTaxiQueue,
                clearedTaxiStandWaiting);
#endif
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
