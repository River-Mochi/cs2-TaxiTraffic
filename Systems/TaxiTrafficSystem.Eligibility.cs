// <copyright file="TaxiTrafficSystem.Eligibility.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace TaxiTraffic
{
    using Game.Citizens;     // Citizen, HouseholdMember, commuter/tourist households
    using Game.Common;       // Deleted
    using Game.Creatures;    // Resident, ResidentFlags, CurrentVehicle, RideNeeder, GroupMember, GroupCreature
    using Game.Objects;      // TripSource
    using Game.Pathfind;     // prevent gs errors partial files
    using Game.Tools;        // Temp
    using Unity.Collections; // NativeList, Allocator
    using Unity.Entities;    // Entity, RefRO, RefRW


    // File: Systems/TaxiTrafficSystem.Eligibility.cs
    // Household-consistent taxi eligibility with soft enforcement.

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
                // This only restores TaxiTraffic's own IgnoreTaxi bit.
                // It does not repath, alter taxi queues, or cancel an active ride.
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

            // Legacy cleanup from the old temporary travel-group exemption.
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
            // Travel groups are no longer exempt from household taxi eligibility.
            // Keep the hook for Core/status compatibility while old group markers migrate out.
            return 0;
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
                // Restore vanilla taxi eligibility only for residents TaxiTraffic marked.
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

            // Kept for existing status/report plumbing; travel groups are no longer exempt.
            skippedGroupTravelers = 0;

            using NativeList<Entity> toBlock = new(Allocator.Temp);
            using NativeList<Entity> toAllow = new(Allocator.Temp);

            int processed = 0;
            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithNone<IgnoreTaxiMark, TaxiAllowedMark, GroupTaxiAllowedMark>()
                         .WithNone<CurrentVehicle, RideNeeder, TripSource>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                // Vanilla can set InVehicle before CurrentVehicle is added at EndFrame.
                // Do not modify a resident during that valid boarding transition.
                if ((resident.ValueRO.m_Flags & ResidentFlags.InVehicle) != 0)
                    continue;

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
                    // Soft enforcement only: affect future taxi route selection.
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;
                    toBlock.Add(entity);
                    applied++;
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

        private bool ShouldResidentIgnoreTaxiBySettings(
            TaxiSettings setting,
            Resident resident,
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
                uint householdRoll = GetHouseholdTaxiEligibilityRoll(household);
                return householdRoll >= (uint)allowedPercent;
            }

            // Rare fallback for a Resident whose Citizen has no usable household link.
            if (citizenEntity == Entity.Null ||
                !SystemAPI.HasComponent<Game.Citizens.Citizen>(citizenEntity))
            {
                return true;
            }

            Game.Citizens.Citizen citizen =
                SystemAPI.GetComponentRO<Game.Citizens.Citizen>(citizenEntity).ValueRO;

            return GetStableCitizenTaxiEligibilityRoll(citizen) >= (uint)allowedPercent;
        }

        private static uint GetHouseholdTaxiEligibilityRoll(Entity household)
        {
            // All members point to the same household Entity, so they share one bucket.
            // No HouseholdCitizen scan or per-frame cache is needed.
            uint index = unchecked((uint)household.Index);
            uint version = unchecked((uint)household.Version);
            uint seed = index ^ (version * 0x9E3779B9u);

            return MixTaxiEligibilitySeed(seed) % 100u;
        }

        private static uint GetStableCitizenTaxiEligibilityRoll(Game.Citizens.Citizen citizen)
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

        // Diagnostic only. Travel-group membership no longer changes eligibility.
        private bool IsGroupLinkedTraveler(Entity entity)
        {
            return SystemAPI.HasComponent<GroupMember>(entity) ||
                   SystemAPI.HasBuffer<GroupCreature>(entity);
        }
    }
}
