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
        private struct TaxiAvoidanceData
        {
            public int m_ResidentsAvoidTaxis;
            public bool m_BlockCommuters;
            public bool m_BlockTourists;

            [ReadOnly]
            public ComponentLookup<HouseholdMember> m_HouseholdMemberLookup;

            [ReadOnly]
            public ComponentLookup<CommuterHousehold> m_CommuterHouseholdLookup;

            [ReadOnly]
            public ComponentLookup<TouristHousehold> m_TouristHouseholdLookup;

            [ReadOnly]
            public ComponentLookup<Citizen> m_CitizenLookup;

            public bool ShouldAvoid(Resident resident)
            {
                // Fresh-install defaults block every eligible group. Avoid household
                // lookups entirely for this common and most aggressive setting.
                if (m_ResidentsAvoidTaxis >= TaxiSettings.kTaxiAvoidPercentMax &&
                    m_BlockCommuters &&
                    m_BlockTourists)
                {
                    return true;
                }

                Entity citizenEntity = resident.m_Citizen;
                Entity household = Entity.Null;

                if (citizenEntity != Entity.Null &&
                    m_HouseholdMemberLookup.HasComponent(citizenEntity))
                {
                    household =
                        m_HouseholdMemberLookup[citizenEntity].m_Household;

                    if (household != Entity.Null)
                    {
                        // Commuters and tourists use their own controls instead of
                        // the local-resident percentage.
                        if (m_CommuterHouseholdLookup.HasComponent(household))
                            return m_BlockCommuters;

                        if (m_TouristHouseholdLookup.HasComponent(household))
                            return m_BlockTourists;
                    }
                }

                if (m_ResidentsAvoidTaxis <= TaxiSettings.kTaxiAvoidPercentMin)
                    return false;

                if (m_ResidentsAvoidTaxis >= TaxiSettings.kTaxiAvoidPercentMax)
                    return true;

                if (household != Entity.Null)
                {
                    uint householdRoll =
                        GetHouseholdTaxiEligibilityRoll(household);

                    return householdRoll < (uint)m_ResidentsAvoidTaxis;
                }

                // Rare fallback when there is no usable household link.
                if (citizenEntity == Entity.Null ||
                    !m_CitizenLookup.HasComponent(citizenEntity))
                {
                    return false;
                }

                Citizen citizen = m_CitizenLookup[citizenEntity];

                return GetStableCitizenTaxiEligibilityRoll(citizen) <
                       (uint)m_ResidentsAvoidTaxis;
            }
        }

        private TaxiAvoidanceData CreateTaxiAvoidanceData(
            TaxiSettings setting)
        {
            return new TaxiAvoidanceData
            {
                m_ResidentsAvoidTaxis = setting.ResidentsAvoidTaxis,
                m_BlockCommuters = setting.BlockCommuters,
                m_BlockTourists = setting.BlockTourists,
                m_HouseholdMemberLookup =
                    SystemAPI.GetComponentLookup<HouseholdMember>(
                        isReadOnly: true),
                m_CommuterHouseholdLookup =
                    SystemAPI.GetComponentLookup<CommuterHousehold>(
                        isReadOnly: true),
                m_TouristHouseholdLookup =
                    SystemAPI.GetComponentLookup<TouristHousehold>(
                        isReadOnly: true),
                m_CitizenLookup =
                    SystemAPI.GetComponentLookup<Citizen>(
                        isReadOnly: true)
            };
        }

        private void UpdateResidentTaxiEligibility(
            TaxiAvoidanceData avoidanceData,
            out int applied,
            out int removed,
            out int reapplied)
        {
            RunResidentTaxiEligibilityJob(
                m_EligibilityFullQuery,
                avoidanceData,
                out applied,
                out removed,
                out reapplied);
        }

        private void UpdateResidentTaxiEligibilityBucket(
            TaxiAvoidanceData avoidanceData,
            uint simulationFrame,
            out int applied,
            out int removed,
            out int reapplied)
        {
            uint updateFrameIndex =
                simulationFrame % kResidentUpdateFrameCount;

            // Maximum avoidance is the common heavy setting. Residents already
            // owned by Taxi Traffic no longer need household/group classification.
            // Keep ResidentAI's 16-frame bucket, but only inspect new/unowned cims.
            if (avoidanceData.m_ResidentsAvoidTaxis >=
                    TaxiSettings.kTaxiAvoidPercentMax &&
                avoidanceData.m_BlockCommuters &&
                avoidanceData.m_BlockTourists)
            {
                m_MaxAvoidanceEligibilityBucketQuery.SetSharedComponentFilter(
                    new UpdateFrame(updateFrameIndex));

                RunMaximumAvoidanceEligibilityJob(
                    out applied,
                    out removed,
                    out reapplied);

                return;
            }

            // Match ResidentAI's current shared UpdateFrame bucket. Each resident
            // is still reevaluated once per 16 simulation frames, but the work is
            // spread evenly instead of producing one large periodic spike.
            m_EligibilityBucketQuery.SetSharedComponentFilter(
                new UpdateFrame(updateFrameIndex));

            RunResidentTaxiEligibilityJob(
                m_EligibilityBucketQuery,
                avoidanceData,
                out applied,
                out removed,
                out reapplied);
        }

        private void RunMaximumAvoidanceEligibilityJob(
            out int applied,
            out int removed,
            out int reapplied)
        {
            m_EligibilityCounters[0] = 0;
            m_EligibilityCounters[1] = 0;
            m_EligibilityCounters[2] = 0;

            using EntityCommandBuffer buffer =
                new EntityCommandBuffer(Allocator.TempJob);

            MaximumAvoidanceEligibilityJob job =
                new MaximumAvoidanceEligibilityJob
                {
                    m_EntityType =
                        SystemAPI.GetEntityTypeHandle(),
                    m_ResidentType =
                        SystemAPI.GetComponentTypeHandle<Resident>(),
                    m_AppliedCount = m_EligibilityCounters,
                    m_CommandBuffer = buffer.AsParallelWriter()
                };

            // The query already excludes Taxi Traffic-owned residents, so this
            // Burst pass only handles new/unowned cims in ResidentAI's current
            // bucket. Vanilla-owned IgnoreTaxi flags are observed but never claimed.
            job.Run(m_MaxAvoidanceEligibilityBucketQuery);

            buffer.Playback(EntityManager);

            applied = m_EligibilityCounters[0];
            removed = 0;
            reapplied = 0;
        }

        [BurstCompile]
        private struct MaximumAvoidanceEligibilityJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;

            public ComponentTypeHandle<Resident> m_ResidentType;

            public NativeArray<int> m_AppliedCount;

            public EntityCommandBuffer.ParallelWriter m_CommandBuffer;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entities =
                    chunk.GetNativeArray(m_EntityType);

                NativeArray<Resident> residents =
                    chunk.GetNativeArray(ref m_ResidentType);

                int applied = 0;

                ChunkEntityEnumerator enumerator =
                    new ChunkEntityEnumerator(
                        useEnabledMask,
                        chunkEnabledMask,
                        chunk.Count);

                while (enumerator.NextEntityIndex(out int i))
                {
                    Resident resident = residents[i];
                    ResidentFlags flags = resident.m_Flags;

                    // Keep the same safety guard as the general eligibility path.
                    if ((flags & ResidentFlags.InVehicle) != 0)
                        continue;

                    // If vanilla already owns IgnoreTaxi, leave it untouched and
                    // do not add Taxi Traffic's ownership marker.
                    if ((flags & ResidentFlags.IgnoreTaxi) != 0)
                        continue;

                    resident.m_Flags |= ResidentFlags.IgnoreTaxi;
                    residents[i] = resident;

                    m_CommandBuffer.AddComponent<IgnoreTaxiMark>(
                        unfilteredChunkIndex,
                        entities[i]);

                    applied++;
                }

                if (applied != 0)
                    m_AppliedCount[0] += applied;
            }
        }

        private void RunResidentTaxiEligibilityJob(
            EntityQuery query,
            TaxiAvoidanceData avoidanceData,
            out int applied,
            out int removed,
            out int reapplied)
        {
            m_EligibilityCounters[0] = 0;
            m_EligibilityCounters[1] = 0;
            m_EligibilityCounters[2] = 0;

            using EntityCommandBuffer buffer =
                new EntityCommandBuffer(Allocator.TempJob);

            ResidentTaxiEligibilityJob job =
                new ResidentTaxiEligibilityJob
                {
                    m_EntityType =
                        SystemAPI.GetEntityTypeHandle(),
                    m_ResidentType =
                        SystemAPI.GetComponentTypeHandle<Resident>(),
                    m_IgnoreTaxiMarkType =
                        SystemAPI.GetComponentTypeHandle<IgnoreTaxiMark>(
                            isReadOnly: true),
                    m_AvoidanceData = avoidanceData,
                    m_Counters = m_EligibilityCounters,
                    m_CommandBuffer = buffer.AsParallelWriter()
                };

            // The bucket is intentionally small. Run it immediately so Burst
            // removes managed per-entity overhead without adding schedule/Complete
            // latency before the reapply and RideNeeder safety passes.
            job.Run(query);

            buffer.Playback(EntityManager);

            applied = m_EligibilityCounters[0];
            removed = m_EligibilityCounters[1];
            reapplied = m_EligibilityCounters[2];
        }

        [BurstCompile]
        private struct ResidentTaxiEligibilityJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;

            public ComponentTypeHandle<Resident> m_ResidentType;

            [ReadOnly]
            public ComponentTypeHandle<IgnoreTaxiMark> m_IgnoreTaxiMarkType;

            [ReadOnly]
            public TaxiAvoidanceData m_AvoidanceData;

            public NativeArray<int> m_Counters;

            public EntityCommandBuffer.ParallelWriter m_CommandBuffer;

            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entities =
                    chunk.GetNativeArray(m_EntityType);

                NativeArray<Resident> residents =
                    chunk.GetNativeArray(ref m_ResidentType);

                // IgnoreTaxiMark is structural, so an archetype chunk either owns
                // the marker for every resident in the chunk or for none of them.
                bool ownsIgnoreTaxi =
                    chunk.Has(ref m_IgnoreTaxiMarkType);

                int applied = 0;
                int removed = 0;
                int reapplied = 0;

                ChunkEntityEnumerator enumerator =
                    new ChunkEntityEnumerator(
                        useEnabledMask,
                        chunkEnabledMask,
                        chunk.Count);

                while (enumerator.NextEntityIndex(out int i))
                {
                    Resident resident = residents[i];
                    ResidentFlags flags = resident.m_Flags;

                    // Do not change a cim that is already inside a vehicle.
                    // Existing taxi trips finish naturally.
                    if ((flags & ResidentFlags.InVehicle) != 0)
                        continue;

                    bool shouldAvoid =
                        m_AvoidanceData.ShouldAvoid(resident);

                    bool ignoreTaxiNow =
                        (flags & ResidentFlags.IgnoreTaxi) != 0;

                    if (shouldAvoid)
                    {
                        if (ownsIgnoreTaxi)
                        {
                            if (!ignoreTaxiNow)
                            {
                                resident.m_Flags |= ResidentFlags.IgnoreTaxi;
                                residents[i] = resident;
                                reapplied++;
                            }

                            continue;
                        }

                        // If vanilla already owns IgnoreTaxi, do not claim it.
                        if (ignoreTaxiNow)
                            continue;

                        resident.m_Flags |= ResidentFlags.IgnoreTaxi;
                        residents[i] = resident;

                        m_CommandBuffer.AddComponent<IgnoreTaxiMark>(
                            unfilteredChunkIndex,
                            entities[i]);

                        applied++;
                        continue;
                    }

                    if (!ownsIgnoreTaxi)
                        continue;

                    if (ignoreTaxiNow)
                    {
                        resident.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                        residents[i] = resident;
                    }

                    m_CommandBuffer.RemoveComponent<IgnoreTaxiMark>(
                        unfilteredChunkIndex,
                        entities[i]);

                    removed++;
                }

                if (applied != 0)
                    m_Counters[0] += applied;

                if (removed != 0)
                    m_Counters[1] += removed;

                if (reapplied != 0)
                    m_Counters[2] += reapplied;
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

            ReapplyOwnedTaxiBlocksJob job =
                new ReapplyOwnedTaxiBlocksJob
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
                    new ChunkEntityEnumerator(
                        useEnabledMask,
                        chunkEnabledMask,
                        chunk.Count);

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
                    buffer = new EntityCommandBuffer(Allocator.TempJob);
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
