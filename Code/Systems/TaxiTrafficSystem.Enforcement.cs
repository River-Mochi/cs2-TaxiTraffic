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
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void StopBlockedRideNeeders(
            TaxiSettings setting,
            out int lateAppliedIgnoreTaxi,
            out int stoppedRideNeeders,
            out int existingTaxiRequestsStopped,
            out int repathedTaxiWaiters)
        {
            lateAppliedIgnoreTaxi = 0;
            stoppedRideNeeders = 0;
            existingTaxiRequestsStopped = 0;
            repathedTaxiWaiters = 0;

            EntityCommandBuffer buffer = default;
            bool hasBuffer = false;

            foreach ((RefRW<Resident> resident,
                      RefRO<RideNeeder> rideNeeder,
                      Entity entity) in SystemAPI
                         .Query<RefRW<Resident>, RefRO<RideNeeder>>()
                         .WithNone<CurrentVehicle, Deleted, Temp>()
                         .WithEntityAccess())
            {
                ResidentFlags residentFlags = resident.ValueRO.m_Flags;

                // Never interfere once boarding/transport has become an active trip.
                if ((residentFlags &
                     (ResidentFlags.InVehicle | ResidentFlags.WaitingTransport)) != 0)
                {
                    continue;
                }

                if (!ShouldResidentAvoidTaxi(setting, resident.ValueRO))
                    continue;

                Entity requestEntity = rideNeeder.ValueRO.m_RideRequest;

                // Once vanilla has already assigned a taxi, let that trip finish.
                // New taxi demand is stopped before dispatch instead.
                if (requestEntity != Entity.Null &&
                    SystemAPI.Exists(requestEntity) &&
                    SystemAPI.HasComponent<Dispatched>(requestEntity))
                {
                    continue;
                }

                bool ownsIgnoreTaxi =
                    SystemAPI.HasComponent<IgnoreTaxiMark>(entity);

                if ((residentFlags & ResidentFlags.IgnoreTaxi) == 0)
                {
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                    if (!ownsIgnoreTaxi)
                    {
                        if (!hasBuffer)
                        {
                            buffer = new EntityCommandBuffer(Allocator.Temp);
                            hasBuffer = true;
                        }

                        buffer.AddComponent<IgnoreTaxiMark>(entity);
                        lateAppliedIgnoreTaxi++;
                    }
                }

                if (requestEntity != Entity.Null &&
                    SystemAPI.Exists(requestEntity) &&
                    SystemAPI.HasComponent<TaxiRequest>(requestEntity))
                {
                    TaxiRequest request =
                        SystemAPI.GetComponentRO<TaxiRequest>(
                            requestEntity).ValueRO;

                    if (request.m_Seeker == entity &&
                        request.m_Type != TaxiRequestType.Stand)
                    {
                        // Do not destroy the request ourselves.
                        // Removing RideNeeder makes vanilla TaxiDispatch reject
                        // the request and perform its own normal cleanup.
                        existingTaxiRequestsStopped++;
                    }
                }

                if (SystemAPI.HasComponent<HumanCurrentLane>(entity) &&
                    SystemAPI.HasComponent<PathOwner>(entity))
                {
                    RefRW<HumanCurrentLane> lane =
                        SystemAPI.GetComponentRW<HumanCurrentLane>(entity);

                    if ((lane.ValueRO.m_Flags & CreatureLaneFlags.Taxi) != 0)
                    {
                        RefRW<PathOwner> pathOwner =
                            SystemAPI.GetComponentRW<PathOwner>(entity);

                        // This mirrors vanilla's own escape path after a failed
                        // taxi request: leave the taxi pickup lane and repath.
                        lane.ValueRW.m_Flags &=
                            ~(CreatureLaneFlags.ParkingSpace |
                              CreatureLaneFlags.Taxi);

                        pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                        pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

                        repathedTaxiWaiters++;
                    }
                }

                // Immediate playback is important: Taxi Traffic runs after
                // ResidentAI and must remove RideNeeder before later taxi systems.
                if (!hasBuffer)
                {
                    buffer = new EntityCommandBuffer(Allocator.Temp);
                    hasBuffer = true;
                }

                buffer.RemoveComponent<RideNeeder>(entity);
                stoppedRideNeeders++;
            }

            if (hasBuffer)
            {
                buffer.Playback(EntityManager);
                buffer.Dispose();
            }
        }
    }
}
