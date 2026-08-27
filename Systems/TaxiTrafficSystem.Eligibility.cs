// <copyright file="TaxiTrafficSystem.Eligibility.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace TaxiTraffic
{
    using Game.Citizens;     // Citizen, HouseholdMember, HouseholdCitizen, commuter/tourist households
    using Game.Common;       // Deleted
    using Game.Creatures;    // Resident, ResidentFlags, GroupMember, GroupCreature
    using Game.Objects;      // TripSource
    using Game.Pathfind;     // PathOwner, PathFlags
    using Game.Tools;        // Temp
    using Unity.Collections; // NativeList, NativeHashMap, Allocator
    using Unity.Entities;    // Entity, RefRO, RefRW, DynamicBuffer

    // File: Systems/TaxiTrafficSystem.Eligibility.cs
    // Taxi eligibility markers, group exemptions, and trip-start repair.

    public partial class TaxiTrafficSystem
    {
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
            using NativeList<Entity> groupAllowedMarks = new(Allocator.Temp);

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

            if (resetCount < kMarkBatchPerUpdate)
            {
                foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                             .Query<RefRO<Resident>>()
                             .WithAll<GroupTaxiAllowedMark>()
                             .WithNone<Deleted, Temp>()
                             .WithEntityAccess())
                {
                    groupAllowedMarks.Add(entity);

                    resetCount++;
                    if (resetCount >= kMarkBatchPerUpdate)
                        break;
                }
            }

            if (blockedMarks.Length > 0)
                EntityManager.RemoveComponent<IgnoreTaxiMark>(blockedMarks.AsArray());

            if (allowedMarks.Length > 0)
                EntityManager.RemoveComponent<TaxiAllowedMark>(allowedMarks.AsArray());

            if (groupAllowedMarks.Length > 0)
                EntityManager.RemoveComponent<GroupTaxiAllowedMark>(groupAllowedMarks.AsArray());

            return resetCount;
        }

        private int MaintainGroupTaxiExemptionsBatch()
        {
            int changed = 0;

            // A blocked solo Resident can become group-linked later. Move that Resident to a temporary group exemption.
            using (NativeList<Entity> toUnblock = new(Allocator.Temp))
            using (NativeList<Entity> toGroupAllow = new(Allocator.Temp))
            {
                foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                             .Query<RefRW<Resident>>()
                             .WithAll<IgnoreTaxiMark, GroupMember>()
                             .WithNone<GroupTaxiAllowedMark, Deleted, Temp>()
                             .WithEntityAccess())
                {
                    resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                    toUnblock.Add(entity);
                    toGroupAllow.Add(entity);

                    changed++;
                    if (changed >= kMarkBatchPerUpdate)
                        break;
                }

                if (changed < kMarkBatchPerUpdate)
                {
                    foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                                 .Query<RefRW<Resident>>()
                                 .WithAll<IgnoreTaxiMark, GroupCreature>()
                                 .WithNone<GroupMember, GroupTaxiAllowedMark, Deleted>()
                                 .WithNone<Temp>()
                                 .WithEntityAccess())
                    {
                        resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                        toUnblock.Add(entity);
                        toGroupAllow.Add(entity);

                        changed++;
                        if (changed >= kMarkBatchPerUpdate)
                            break;
                    }
                }

                if (toUnblock.Length > 0)
                    EntityManager.RemoveComponent<IgnoreTaxiMark>(toUnblock.AsArray());

                if (toGroupAllow.Length > 0)
                    EntityManager.AddComponent<GroupTaxiAllowedMark>(toGroupAllow.AsArray());
            }

            if (changed >= kMarkBatchPerUpdate)
                return changed;

            // Migrate old normal-allowed markers that are currently used only because the Resident is in a group.
            using (NativeList<Entity> oldAllowedMarks = new(Allocator.Temp))
            using (NativeList<Entity> toGroupAllow = new(Allocator.Temp))
            {
                foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                             .Query<RefRO<Resident>>()
                             .WithAll<TaxiAllowedMark, GroupMember>()
                             .WithNone<GroupTaxiAllowedMark, Deleted, Temp>()
                             .WithEntityAccess())
                {
                    oldAllowedMarks.Add(entity);
                    toGroupAllow.Add(entity);

                    changed++;
                    if (changed >= kMarkBatchPerUpdate)
                        break;
                }

                if (changed < kMarkBatchPerUpdate)
                {
                    foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                                 .Query<RefRO<Resident>>()
                                 .WithAll<TaxiAllowedMark, GroupCreature>()
                                 .WithNone<GroupMember, GroupTaxiAllowedMark, Deleted>()
                                 .WithNone<Temp>()
                                 .WithEntityAccess())
                    {
                        oldAllowedMarks.Add(entity);
                        toGroupAllow.Add(entity);

                        changed++;
                        if (changed >= kMarkBatchPerUpdate)
                            break;
                    }
                }

                if (oldAllowedMarks.Length > 0)
                    EntityManager.RemoveComponent<TaxiAllowedMark>(oldAllowedMarks.AsArray());

                if (toGroupAllow.Length > 0)
                    EntityManager.AddComponent<GroupTaxiAllowedMark>(toGroupAllow.AsArray());
            }

            if (changed >= kMarkBatchPerUpdate)
                return changed;

            // Once group links disappear, remove only the temporary exemption.
            using NativeList<Entity> staleGroupMarks = new(Allocator.Temp);

            foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                         .Query<RefRO<Resident>>()
                         .WithAll<GroupTaxiAllowedMark>()
                         .WithNone<GroupMember, GroupCreature, Deleted>()
                         .WithNone<Temp>()
                         .WithEntityAccess())
            {
                staleGroupMarks.Add(entity);

                changed++;
                if (changed >= kMarkBatchPerUpdate)
                    break;
            }

            if (staleGroupMarks.Length > 0)
                EntityManager.RemoveComponent<GroupTaxiAllowedMark>(staleGroupMarks.AsArray());

            return changed;
        }

        private void UnmarkIgnoreTaxiBatch(out int unmarkedCount)
        {
            unmarkedCount = 0;

            using NativeList<Entity> toUnmark = new(Allocator.Temp);
            using NativeList<Entity> allowedMarks = new(Allocator.Temp);
            using NativeList<Entity> groupAllowedMarks = new(Allocator.Temp);

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

            if (processed < kMarkBatchPerUpdate)
            {
                foreach ((RefRO<Resident> _, Entity entity) in SystemAPI
                             .Query<RefRO<Resident>>()
                             .WithAll<GroupTaxiAllowedMark>()
                             .WithNone<Deleted, Temp>()
                             .WithEntityAccess())
                {
                    groupAllowedMarks.Add(entity);

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

            if (groupAllowedMarks.Length > 0)
                EntityManager.RemoveComponent<GroupTaxiAllowedMark>(groupAllowedMarks.AsArray());
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
            using NativeList<Entity> toGroupAllow = new(Allocator.Temp);

            // Family members reuse one roll during this batch instead of recalculating the same household.
            using NativeHashMap<Entity, uint> householdRollCache =
                new(kMarkBatchPerUpdate, Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithNone<IgnoreTaxiMark, TaxiAllowedMark, GroupTaxiAllowedMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                processed++;

                if (IsGroupLinkedTraveler(entity))
                {
                    skippedGroupTravelers++;

                    // Group exemption is temporary; do not reuse the normal TaxiAllowedMark.
                    toGroupAllow.Add(entity);

                    if (processed >= kMarkBatchPerUpdate)
                        break;

                    continue;
                }

                bool shouldBlock = ShouldResidentIgnoreTaxiBySettings(
                    setting,
                    resident.ValueRO,
                    ref householdRollCache,
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
                    if (SystemAPI.HasComponent<TripSource>(entity) &&
                        SystemAPI.HasComponent<PathOwner>(entity))
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

            if (toGroupAllow.Length > 0)
                EntityManager.AddComponent<GroupTaxiAllowedMark>(toGroupAllow.AsArray());
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
            ref NativeHashMap<Entity, uint> householdRollCache,
            out bool skippedCommuter,
            out bool skippedTourist)
        {
            skippedCommuter = false;
            skippedTourist = false;

            Entity citizenEntity = resident.m_Citizen;
            Entity household = Entity.Null;

            if (citizenEntity != Entity.Null &&
                SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
            {
                household =
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

            if (household != Entity.Null)
            {
                if (!householdRollCache.TryGetValue(household, out uint householdRoll))
                {
                    householdRoll = GetStableHouseholdTaxiEligibilityRoll(household, citizenEntity);
                    householdRollCache.TryAdd(household, householdRoll);
                }

                return householdRoll >= (uint)allowedPercent;
            }

            if (citizenEntity == Entity.Null ||
                !SystemAPI.HasComponent<Citizen>(citizenEntity))
            {
                // No household or Citizen means no stable bucket; keep strong-block behavior.
                return true;
            }

            Citizen citizen = SystemAPI.GetComponentRO<Citizen>(citizenEntity).ValueRO;
            return GetStableCitizenTaxiEligibilityRoll(citizen) >= (uint)allowedPercent;
        }

        private uint GetStableHouseholdTaxiEligibilityRoll(
            Entity household,
            Entity fallbackCitizen)
        {
            uint sum = 0u;
            uint xor = 0u;
            uint count = 0u;

            if (SystemAPI.HasBuffer<HouseholdCitizen>(household))
            {
                DynamicBuffer<HouseholdCitizen> members =
                    SystemAPI.GetBuffer<HouseholdCitizen>(household);

                for (int i = 0; i < members.Length; i++)
                {
                    Entity member = members[i].m_Citizen;
                    if (member == Entity.Null || !SystemAPI.HasComponent<Citizen>(member))
                        continue;

                    Citizen citizen = SystemAPI.GetComponentRO<Citizen>(member).ValueRO;
                    uint seed = ((uint)citizen.m_PseudoRandom << 16) | citizen.m_PseudoRandom;
                    uint mixed = MixTaxiEligibilitySeed(seed);

                    // Order-independent so every family member gets the same household roll.
                    sum += mixed;
                    xor ^= mixed * 0x9E3779B9u;
                    count++;
                }
            }

            if (count == 0u &&
                fallbackCitizen != Entity.Null &&
                SystemAPI.HasComponent<Citizen>(fallbackCitizen))
            {
                Citizen citizen = SystemAPI.GetComponentRO<Citizen>(fallbackCitizen).ValueRO;
                return GetStableCitizenTaxiEligibilityRoll(citizen);
            }

            // Household composition changes can intentionally move the family to a new bucket.
            uint householdSeed =
                sum ^
                (xor + (count * 0x85EBCA6Bu)) ^
                kTaxiEligibilityHashSalt;

            return MixTaxiEligibilitySeed(householdSeed) % 100u;
        }

        private static uint GetStableCitizenTaxiEligibilityRoll(Citizen citizen)
        {
            uint seed = ((uint)citizen.m_PseudoRandom << 16) | citizen.m_PseudoRandom;
            return MixTaxiEligibilitySeed(seed) % 100u;
        }

        private static uint MixTaxiEligibilitySeed(uint seed)
        {
            uint hash = seed ^ kTaxiEligibilityHashSalt;

            hash ^= hash >> 16;
            hash *= 0x7feb352du;
            hash ^= hash >> 15;
            hash *= 0x846ca68bu;
            hash ^= hash >> 16;

            return hash;
        }

        private bool IsGroupLinkedTraveler(Entity entity)
        {
            return SystemAPI.HasComponent<GroupMember>(entity) ||
                   SystemAPI.HasBuffer<GroupCreature>(entity);
        }
    }
}
