// <copyright file="TaxiTrafficSystem.Eligibility.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Eligibility.cs
// Purpose: household-consistent taxi eligibility and IgnoreTaxi ownership.

using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void UpdateResidentTaxiEligibility(
            TaxiSettings setting,
            out int applied,
            out int removed,
            out int reapplied)
        {
            applied = 0;
            removed = 0;
            reapplied = 0;

            EntityCommandBuffer buffer = default;
            bool hasBuffer = false;

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithNone<CurrentVehicle, Deleted, Temp>()
                         .WithEntityAccess())
            {
                ResidentFlags flags = resident.ValueRO.m_Flags;

                // Do not change a cim that is already inside a vehicle.
                // Existing taxi trips finish naturally.
                if ((flags & ResidentFlags.InVehicle) != 0)
                    continue;

                bool shouldAvoid =
                    ShouldResidentAvoidTaxi(setting, resident.ValueRO);

                bool ownsIgnoreTaxi =
                    SystemAPI.HasComponent<IgnoreTaxiMark>(entity);

                bool ignoreTaxiNow =
                    (flags & ResidentFlags.IgnoreTaxi) != 0;

                if (shouldAvoid)
                {
                    if (ownsIgnoreTaxi)
                    {
                        // The small UpdateFrame pass normally repairs this sooner.
                        // The full scan also catches any unusual missed resident.
                        if (!ignoreTaxiNow)
                        {
                            resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;
                            reapplied++;
                        }

                        continue;
                    }

                    // If vanilla already owns IgnoreTaxi, do not claim it.
                    if (ignoreTaxiNow)
                        continue;

                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                    if (!hasBuffer)
                    {
                        buffer = new EntityCommandBuffer(Allocator.Temp);
                        hasBuffer = true;
                    }

                    buffer.AddComponent<IgnoreTaxiMark>(entity);
                    applied++;
                    continue;
                }

                if (!ownsIgnoreTaxi)
                    continue;

                if (ignoreTaxiNow)
                    resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;

                if (!hasBuffer)
                {
                    buffer = new EntityCommandBuffer(Allocator.Temp);
                    hasBuffer = true;
                }

                buffer.RemoveComponent<IgnoreTaxiMark>(entity);
                removed++;
            }

            if (hasBuffer)
            {
                buffer.Playback(EntityManager);
                buffer.Dispose();
            }
        }

        private void ReapplyOwnedTaxiBlocks(
            uint simulationFrame,
            out int reapplied)
        {
            m_ReapplyCounter[0] = 0;

            // Match the exact UpdateFrame bucket ResidentAI just processed.
            // This cuts the steady reapply scan to about 1/16 of owned cims.
            m_ReapplyBlockQuery.SetSharedComponentFilter(
                new UpdateFrame(
                    simulationFrame % kResidentUpdateFrameCount));

            ReapplyOwnedTaxiBlocksJob job = new()
            {
                m_ResidentType =
                    SystemAPI.GetComponentTypeHandle<Resident>(),
                m_ReappliedCount = m_ReapplyCounter
            };

            // This pass is small after UpdateFrame filtering. Running it immediately
            // preserves ordering before the RideNeeder protection pass and avoids
            // worker-job scheduling/Complete overhead for only a few thousand cims.
            job.Run(m_ReapplyBlockQuery);

            reapplied = m_ReapplyCounter[0];
        }

        [BurstCompile]
        private struct ReapplyOwnedTaxiBlocksJob : IJobChunk
        {
            public ComponentTypeHandle<Resident> m_ResidentType;

            public NativeArray<int> m_ReappliedCount;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Resident> residents =
                    chunk.GetNativeArray(ref m_ResidentType);

                int reapplied = 0;
                ChunkEntityEnumerator enumerator =
                    new(useEnabledMask, chunkEnabledMask, chunk.Count);

                while (enumerator.NextEntityIndex(out int i))
                {
                    Resident resident = residents[i];
                    ResidentFlags flags = resident.m_Flags;

                    // Preserve the same safety guard as the old managed pass.
                    if ((flags & ResidentFlags.InVehicle) != 0)
                        continue;

                    if ((flags & ResidentFlags.IgnoreTaxi) != 0)
                        continue;

                    resident.m_Flags |= ResidentFlags.IgnoreTaxi;
                    residents[i] = resident;
                    reapplied++;
                }

                if (reapplied != 0)
                    m_ReappliedCount[0] += reapplied;
            }
        }

        private int ClearOwnedResidentTaxiBlocks()
        {
            int removed = 0;

            EntityCommandBuffer buffer = default;
            bool hasBuffer = false;

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithAll<IgnoreTaxiMark>()
                         .WithNone<CurrentVehicle, Deleted, Temp>()
                         .WithEntityAccess())
            {
                ResidentFlags flags = resident.ValueRO.m_Flags;

                if ((flags & ResidentFlags.InVehicle) != 0)
                    continue;

                resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;

                if (!hasBuffer)
                {
                    buffer = new EntityCommandBuffer(Allocator.Temp);
                    hasBuffer = true;
                }

                buffer.RemoveComponent<IgnoreTaxiMark>(entity);
                removed++;
            }

            if (hasBuffer)
            {
                buffer.Playback(EntityManager);
                buffer.Dispose();
            }

            return removed;
        }

        private bool ShouldResidentAvoidTaxi(
            TaxiSettings setting,
            Resident resident)
        {
            // Fresh-install defaults block every eligible group. Avoid household
            // lookups entirely for this common and most aggressive setting.
            if (setting.ResidentsAvoidTaxis >=
                    TaxiSettings.kTaxiAvoidPercentMax &&
                setting.BlockCommuters &&
                setting.BlockTourists)
            {
                return true;
            }

            Entity citizenEntity = resident.m_Citizen;
            Entity household = Entity.Null;

            if (citizenEntity != Entity.Null &&
                SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
            {
                household =
                    SystemAPI.GetComponentRO<HouseholdMember>(
                        citizenEntity).ValueRO.m_Household;

                if (household != Entity.Null)
                {
                    // Commuters and tourists use their own controls instead of
                    // the local-resident percentage.
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
                uint householdRoll =
                    GetHouseholdTaxiEligibilityRoll(household);

                return householdRoll < (uint)avoidPercent;
            }

            // Rare fallback when there is no usable household link.
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
            // Everyone in a household gets the same stable percentage bucket.
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
