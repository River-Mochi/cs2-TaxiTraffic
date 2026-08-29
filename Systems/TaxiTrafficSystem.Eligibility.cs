// <copyright file="TaxiTrafficSystem.Eligibility.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Eligibility.cs
// Purpose: simple household-consistent taxi eligibility with soft enforcement.

using Game.Citizens;  // HouseholdMember, CommuterHousehold, TouristHousehold, Citizen
using Game.Common;    // Deleted
using Game.Creatures; // Resident, ResidentFlags, CurrentVehicle, RideNeeder
using Game.Objects;   // TripSource
using Game.Tools;     // Temp
using Unity.Entities; // Entity, EntityCommandBuffer, RefRW

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void UpdateResidentTaxiEligibility(
            TaxiSettings setting,
            out int applied,
            out int removed)
        {
            applied = 0;
            removed = 0;

            // Structural marker changes play back at the normal game end-frame
            // barrier. ResidentFlags itself is not structural and is written now.
            EntityCommandBuffer buffer = m_EndFrameBarrier.CreateCommandBuffer();

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithNone<CurrentVehicle, RideNeeder, TripSource>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                ResidentFlags flags = resident.ValueRO.m_Flags;

                // Do not touch residents during active vehicle/transport transitions.
                // Existing taxi trips and waits are allowed to finish naturally.
                if ((flags & (ResidentFlags.InVehicle | ResidentFlags.WaitingTransport)) != 0)
                    continue;

                bool shouldAvoid = ShouldResidentAvoidTaxi(
                    setting,
                    resident.ValueRO);

                bool ownsIgnoreTaxi =
                    SystemAPI.HasComponent<IgnoreTaxiMark>(entity);

                bool ignoreTaxiNow =
                    (flags & ResidentFlags.IgnoreTaxi) != 0;

                if (shouldAvoid)
                {
                    if (!ignoreTaxiNow)
                    {
                        resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                        // Only claim ownership when Taxi Traffic actually turns
                        // IgnoreTaxi on. Never claim a flag vanilla already owned.
                        if (!ownsIgnoreTaxi)
                            buffer.AddComponent<IgnoreTaxiMark>(entity);

                        applied++;
                    }

                    continue;
                }

                if (!ownsIgnoreTaxi)
                    continue;

                // Taxi Traffic owns this IgnoreTaxi flag and the resident is now
                // allowed again. Do not repath, clear queues, or cancel a ride.
                if (ignoreTaxiNow)
                    resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;

                buffer.RemoveComponent<IgnoreTaxiMark>(entity);
                removed++;
            }
        }

        private int ClearOwnedResidentTaxiBlocks()
        {
            int removed = 0;

            EntityCommandBuffer buffer = m_EndFrameBarrier.CreateCommandBuffer();

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithAll<IgnoreTaxiMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                // Game-default mode clears only the flag Taxi Traffic owns.
                // No path invalidation or trip cancellation is performed.
                resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                buffer.RemoveComponent<IgnoreTaxiMark>(entity);
                removed++;
            }

            return removed;
        }

        private bool ShouldResidentAvoidTaxi(
            TaxiSettings setting,
            Resident resident)
        {
            Entity citizenEntity = resident.m_Citizen;
            Entity household = Entity.Null;

            if (citizenEntity != Entity.Null &&
                SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
            {
                household =
                    SystemAPI.GetComponentRO<HouseholdMember>(citizenEntity).ValueRO.m_Household;

                if (household != Entity.Null)
                {
                    // Commuters and tourists are separate controls. When their
                    // toggle is OFF, they remain vanilla regardless of the local
                    // resident percentage.
                    if (SystemAPI.HasComponent<CommuterHousehold>(household))
                        return setting.BlockCommuters;

                    if (SystemAPI.HasComponent<TouristHousehold>(household))
                        return setting.BlockTourists;
                }
            }

            int avoidPercent = setting.ResidentsAvoidTaxis;

            if (avoidPercent <= TaxiSettings.kTaxiAvoidPercentMin)
                return false;

            if (avoidPercent >= TaxiSettings.kTaxiAvoidPercentMax)
                return true;

            if (household != Entity.Null)
            {
                uint householdRoll = GetHouseholdTaxiEligibilityRoll(household);
                return householdRoll < (uint)avoidPercent;
            }

            // Rare fallback when a Resident has a Citizen but no usable household
            // link. If even the Citizen link is invalid, leave vanilla behavior.
            if (citizenEntity == Entity.Null ||
                !SystemAPI.HasComponent<Citizen>(citizenEntity))
            {
                return false;
            }

            Citizen citizen =
                SystemAPI.GetComponentRO<Citizen>(citizenEntity).ValueRO;

            return GetStableCitizenTaxiEligibilityRoll(citizen) <
                   (uint)avoidPercent;
        }

        private static uint GetHouseholdTaxiEligibilityRoll(Entity household)
        {
            // Every member points to the same household Entity, so all members
            // naturally receive the same long-term percentage bucket.
            uint index = unchecked((uint)household.Index);
            uint version = unchecked((uint)household.Version);
            uint seed = index ^ (version * 0x9E3779B9u);

            return MixTaxiEligibilitySeed(seed) % 100u;
        }

        private static uint GetStableCitizenTaxiEligibilityRoll(Citizen citizen)
        {
            uint seed =
                ((uint)citizen.m_PseudoRandom << 16) |
                citizen.m_PseudoRandom;

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
    }
}
