// <copyright file="TaxiTrafficSystem.Outside.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace TaxiTraffic
{
    // File: Systems/TaxiTrafficSystem.Outside.cs
    // Outside-connection taxi blocking and legacy status helper.

    using Game.Citizens;     // Household, HouseholdMember, household flags
    using Game.Common;       // Deleted
    using Game.Creatures;    // Resident, HumanCurrentLane, RideNeeder
    using Game.Objects;      // TripSource
    using Game.Pathfind;     // PathOwner, PathFlags
    using Game.Tools;        // Temp
    using Unity.Collections; // NativeList, Allocator
    using Unity.Entities;    // Entity, RefRO, RefRW

    public partial class TaxiTrafficSystem
    {
        // Stops OC taxi attempts before RideNeederSystem can create another OC taxi request.
        private void UpdateOutsideTaxiBlocking(
            TaxiSettings setting,
            out int clearedTaxiLaneWaiting,
            out int removedRideNeeders)
        {
            clearedTaxiLaneWaiting = 0;
            removedRideNeeders = 0;

            MaintainOutsideTaxiBlockMarks(setting);

            if (!setting.BlockOutsideTaxis)
                return;

            using NativeList<Entity> toAddBlockMark = new(Allocator.Temp);
            using NativeList<Entity> toAddOwnsIgnoreMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveRideNeeder = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident,
                      RefRW<HumanCurrentLane> lane,
                      RefRW<PathOwner> pathOwner,
                      Entity entity) in SystemAPI
                         .Query<RefRW<Resident>, RefRW<HumanCurrentLane>, RefRW<PathOwner>>()
                         .WithAll<RideNeeder>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                CreatureLaneFlags taxiWaitMask =
                    CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Taxi;

                if ((lane.ValueRO.m_Flags & taxiWaitMask) != taxiWaitMask)
                    continue;

                if (!IsOutsideTaxiPickupLane(lane.ValueRO.m_Lane))
                    continue;

                if (!SystemAPI.HasComponent<OutsideTaxiBlockMark>(entity))
                    toAddBlockMark.Add(entity);

                if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) == 0)
                {
                    resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                    if (!SystemAPI.HasComponent<OutsideTaxiOwnsIgnoreMark>(entity))
                        toAddOwnsIgnoreMark.Add(entity);
                }

                lane.ValueRW.m_Flags &= ~taxiWaitMask;
                lane.ValueRW.m_QueueEntity = Entity.Null;

                pathOwner.ValueRW.m_State &= ~PathFlags.Failed;
                pathOwner.ValueRW.m_State |= PathFlags.Obsolete;

                // No RideNeeder means vanilla cannot create or keep this OC taxi request.
                toRemoveRideNeeder.Add(entity);
                clearedTaxiLaneWaiting++;
            }

            if (toAddBlockMark.Length > 0)
                EntityManager.AddComponent<OutsideTaxiBlockMark>(toAddBlockMark.AsArray());

            if (toAddOwnsIgnoreMark.Length > 0)
                EntityManager.AddComponent<OutsideTaxiOwnsIgnoreMark>(toAddOwnsIgnoreMark.AsArray());

            if (toRemoveRideNeeder.Length > 0)
            {
                EntityManager.RemoveComponent<RideNeeder>(toRemoveRideNeeder.AsArray());
                removedRideNeeders = toRemoveRideNeeder.Length;
            }
        }

        // Keep the temporary block only while the cim is still physically at the OC.
        private void MaintainOutsideTaxiBlockMarks(TaxiSettings setting)
        {
            using NativeList<Entity> toAddOwnsIgnoreMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveBlockMark = new(Allocator.Temp);
            using NativeList<Entity> toRemoveOwnsIgnoreMark = new(Allocator.Temp);

            foreach ((RefRW<Resident> resident, Entity entity) in SystemAPI
                         .Query<RefRW<Resident>>()
                         .WithAll<OutsideTaxiBlockMark>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                bool stillAtOutsidePickup = false;

                if (setting.BlockOutsideTaxis &&
                    SystemAPI.HasComponent<HumanCurrentLane>(entity))
                {
                    HumanCurrentLane lane =
                        SystemAPI.GetComponentRO<HumanCurrentLane>(entity).ValueRO;

                    stillAtOutsidePickup = IsOutsideTaxiPickupLane(lane.m_Lane);
                }

                if (stillAtOutsidePickup)
                {
                    if ((resident.ValueRO.m_Flags & ResidentFlags.IgnoreTaxi) == 0)
                    {
                        resident.ValueRW.m_Flags |= ResidentFlags.IgnoreTaxi;

                        if (!SystemAPI.HasComponent<OutsideTaxiOwnsIgnoreMark>(entity))
                            toAddOwnsIgnoreMark.Add(entity);
                    }

                    continue;
                }

                // Clear only the temporary IgnoreTaxi flag that this OC block turned on.
                if (SystemAPI.HasComponent<OutsideTaxiOwnsIgnoreMark>(entity) &&
                    !SystemAPI.HasComponent<IgnoreTaxiMark>(entity))
                {
                    resident.ValueRW.m_Flags &= ~ResidentFlags.IgnoreTaxi;
                }

                toRemoveBlockMark.Add(entity);

                if (SystemAPI.HasComponent<OutsideTaxiOwnsIgnoreMark>(entity))
                    toRemoveOwnsIgnoreMark.Add(entity);
            }

            if (toAddOwnsIgnoreMark.Length > 0)
                EntityManager.AddComponent<OutsideTaxiOwnsIgnoreMark>(toAddOwnsIgnoreMark.AsArray());

            if (toRemoveOwnsIgnoreMark.Length > 0)
                EntityManager.RemoveComponent<OutsideTaxiOwnsIgnoreMark>(toRemoveOwnsIgnoreMark.AsArray());

            if (toRemoveBlockMark.Length > 0)
                EntityManager.RemoveComponent<OutsideTaxiBlockMark>(toRemoveBlockMark.AsArray());
        }

        private bool IsOutsideTaxiPickupLane(Entity lane)
        {
            if (lane == Entity.Null ||
                !SystemAPI.Exists(lane) ||
                !SystemAPI.HasComponent<Game.Net.ConnectionLane>(lane))
            {
                return false;
            }

            Game.Net.ConnectionLane connection =
                SystemAPI.GetComponentRO<Game.Net.ConnectionLane>(lane).ValueRO;

            return (connection.m_Flags & Game.Net.ConnectionLaneFlags.Outside) != 0;
        }

        // Kept only for the current Status page; remove with the old move-in research rows later.
        private bool IsLocalMoveInFromOutsideConnection(Entity residentEntity, Resident resident)
        {
            Entity citizen = resident.m_Citizen;
            if (citizen == Entity.Null ||
                !SystemAPI.Exists(citizen) ||
                !SystemAPI.HasComponent<HouseholdMember>(citizen))
            {
                return false;
            }

            Entity household =
                SystemAPI.GetComponentRO<HouseholdMember>(citizen).ValueRO.m_Household;

            if (household == Entity.Null ||
                !SystemAPI.Exists(household) ||
                !SystemAPI.HasComponent<Household>(household) ||
                SystemAPI.HasComponent<TouristHousehold>(household) ||
                SystemAPI.HasComponent<CommuterHousehold>(household))
            {
                return false;
            }

            Household householdData =
                SystemAPI.GetComponentRO<Household>(household).ValueRO;

            if ((householdData.m_Flags & HouseholdFlags.MovedIn) != 0 ||
                !SystemAPI.HasComponent<TripSource>(residentEntity))
            {
                return false;
            }

            Entity source =
                SystemAPI.GetComponentRO<TripSource>(residentEntity).ValueRO.m_Source;

            return source != Entity.Null &&
                   SystemAPI.Exists(source) &&
                   SystemAPI.HasComponent<Game.Objects.OutsideConnection>(source);
        }
    }
}
