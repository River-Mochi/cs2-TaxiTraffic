// <copyright file="TaxiTrafficSystem.Waiting.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Waiting.cs
// Taxi waiting-lane and taxi-queue cleanup.

namespace TaxiTraffic
{
    using Game.Common;       // Deleted
    using Game.Creatures;    // Resident, HumanCurrentLane, RideNeeder
    using Game.Pathfind;     // PathOwner, PathFlags
    using Game.Routes;       // TaxiStand, BoardingVehicle, Connected
    using Game.Tools;        // Temp
    using Game.Vehicles;     // Taxi
    using Unity.Collections; // NativeList, Allocator
    using Unity.Entities;    // Entity, RefRO, RefRW

    public partial class TaxiTrafficSystem
    {
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
