// <copyright file="TaxiTrafficSystem.Enforcement.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Enforcement.cs
// Purpose: stop new on-demand taxi requests for cims Taxi Traffic blocks.

using Game.Common;
using Game.Creatures;
using Game.Pathfind;
using Game.Simulation;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void StopBlockedRideNeeders(
            TaxiAvoidanceData avoidanceData,
            out int lateAppliedIgnoreTaxi,
            out int stoppedRideNeeders,
            out int existingTaxiRequestsStopped,
            out int repathedTaxiWaiters)
        {
            lateAppliedIgnoreTaxi = 0;
            stoppedRideNeeders = 0;
            existingTaxiRequestsStopped = 0;
            repathedTaxiWaiters = 0;

            if (m_RideNeederQuery.IsEmptyIgnoreFilter)
                return;

            m_EnforcementCounters[0] = 0;
            m_EnforcementCounters[1] = 0;
            m_EnforcementCounters[2] = 0;
            m_EnforcementCounters[3] = 0;

            using EntityCommandBuffer buffer =
                new EntityCommandBuffer(Allocator.TempJob);

            StopBlockedRideNeedersJob job =
                new StopBlockedRideNeedersJob
                {
                    m_EntityType =
                        SystemAPI.GetEntityTypeHandle(),
                    m_ResidentType =
                        SystemAPI.GetComponentTypeHandle<Resident>(),
                    m_RideNeederType =
                        SystemAPI.GetComponentTypeHandle<RideNeeder>(
                            isReadOnly: true),
                    m_HumanCurrentLaneType =
                        SystemAPI.GetComponentTypeHandle<HumanCurrentLane>(),
                    m_PathOwnerType =
                        SystemAPI.GetComponentTypeHandle<PathOwner>(),
                    m_IgnoreTaxiMarkType =
                        SystemAPI.GetComponentTypeHandle<IgnoreTaxiMark>(
                            isReadOnly: true),
                    m_DispatchedLookup =
                        SystemAPI.GetComponentLookup<Dispatched>(
                            isReadOnly: true),
                    m_TaxiRequestLookup =
                        SystemAPI.GetComponentLookup<TaxiRequest>(
                            isReadOnly: true),
                    m_AvoidanceData = avoidanceData,
                    m_Counters = m_EnforcementCounters,
                    m_CommandBuffer = buffer.AsParallelWriter()
                };

            // Keep this immediate. Taxi Traffic runs after ResidentAI, and the
            // RideNeeder must be removed before the later taxi request systems.
            // Burst cuts the managed hot-loop cost without changing that ordering.
            job.Run(m_RideNeederQuery);

            buffer.Playback(EntityManager);

            lateAppliedIgnoreTaxi = m_EnforcementCounters[0];
            stoppedRideNeeders = m_EnforcementCounters[1];
            existingTaxiRequestsStopped = m_EnforcementCounters[2];
            repathedTaxiWaiters = m_EnforcementCounters[3];
        }

        [BurstCompile]
        private struct StopBlockedRideNeedersJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;

            public ComponentTypeHandle<Resident> m_ResidentType;

            [ReadOnly]
            public ComponentTypeHandle<RideNeeder> m_RideNeederType;

            public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

            public ComponentTypeHandle<PathOwner> m_PathOwnerType;

            [ReadOnly]
            public ComponentTypeHandle<IgnoreTaxiMark> m_IgnoreTaxiMarkType;

            [ReadOnly]
            public ComponentLookup<Dispatched> m_DispatchedLookup;

            [ReadOnly]
            public ComponentLookup<TaxiRequest> m_TaxiRequestLookup;

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

                NativeArray<RideNeeder> rideNeeders =
                    chunk.GetNativeArray(ref m_RideNeederType);

                // These are optional on the query. Like vanilla RideNeederSystem,
                // an empty array means this archetype does not have the component.
                NativeArray<HumanCurrentLane> lanes =
                    chunk.GetNativeArray(ref m_HumanCurrentLaneType);

                NativeArray<PathOwner> pathOwners =
                    chunk.GetNativeArray(ref m_PathOwnerType);

                bool ownsIgnoreTaxi =
                    chunk.Has(ref m_IgnoreTaxiMarkType);

                int lateApplied = 0;
                int stopped = 0;
                int existingRequests = 0;
                int repathed = 0;

                ChunkEntityEnumerator enumerator =
                    new ChunkEntityEnumerator(
                        useEnabledMask,
                        chunkEnabledMask,
                        chunk.Count);

                while (enumerator.NextEntityIndex(out int i))
                {
                    Resident resident = residents[i];
                    ResidentFlags residentFlags = resident.m_Flags;

                    // Never interfere once boarding/transport has become an active trip.
                    if ((residentFlags &
                         (ResidentFlags.InVehicle |
                          ResidentFlags.WaitingTransport)) != 0)
                    {
                        continue;
                    }

                    if (!m_AvoidanceData.ShouldAvoid(resident))
                        continue;

                    Entity entity = entities[i];
                    Entity requestEntity = rideNeeders[i].m_RideRequest;

                    // Once vanilla has already assigned a taxi, let that trip finish.
                    // New taxi demand is stopped before dispatch instead.
                    if (requestEntity != Entity.Null &&
                        m_DispatchedLookup.HasComponent(requestEntity))
                    {
                        continue;
                    }

                    if ((residentFlags & ResidentFlags.IgnoreTaxi) == 0)
                    {
                        resident.m_Flags |= ResidentFlags.IgnoreTaxi;
                        residents[i] = resident;

                        if (!ownsIgnoreTaxi)
                        {
                            m_CommandBuffer.AddComponent<IgnoreTaxiMark>(
                                unfilteredChunkIndex,
                                entity);

                            lateApplied++;
                        }
                    }

                    TaxiRequest request;
                    if (requestEntity != Entity.Null &&
                        m_TaxiRequestLookup.TryGetComponent(
                            requestEntity,
                            out request) &&
                        request.m_Seeker == entity &&
                        request.m_Type != TaxiRequestType.Stand)
                    {
                        // Do not destroy the request ourselves.
                        // Removing RideNeeder makes vanilla TaxiDispatch reject
                        // the request and perform its own normal cleanup.
                        existingRequests++;
                    }

                    if (lanes.Length != 0 &&
                        pathOwners.Length != 0)
                    {
                        HumanCurrentLane lane = lanes[i];

                        if ((lane.m_Flags & CreatureLaneFlags.Taxi) != 0)
                        {
                            PathOwner pathOwner = pathOwners[i];

                            // This mirrors vanilla's own escape path after a failed
                            // taxi request: leave the taxi pickup lane and repath.
                            lane.m_Flags &=
                                ~(CreatureLaneFlags.ParkingSpace |
                                  CreatureLaneFlags.Taxi);

                            pathOwner.m_State &= ~PathFlags.Failed;
                            pathOwner.m_State |= PathFlags.Obsolete;

                            lanes[i] = lane;
                            pathOwners[i] = pathOwner;
                            repathed++;
                        }
                    }

                    // Immediate playback after this Burst job keeps the same safety
                    // boundary as the old managed pass.
                    m_CommandBuffer.RemoveComponent<RideNeeder>(
                        unfilteredChunkIndex,
                        entity);
                    stopped++;
                }

                if (lateApplied != 0)
                    m_Counters[0] += lateApplied;

                if (stopped != 0)
                    m_Counters[1] += stopped;

                if (existingRequests != 0)
                    m_Counters[2] += existingRequests;

                if (repathed != 0)
                    m_Counters[3] += repathed;
            }
        }
    }
}
