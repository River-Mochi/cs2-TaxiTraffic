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
    // Outside taxi-supply control and move-in evidence.

    using Game.Citizens;     // Household, HouseholdMember, household flags
    using Game.Common;       // Deleted
    using Game.Creatures;    // Resident
    using Game.Objects;      // TripSource
    using Game.Tools;        // Temp
    using Game.Vehicles;     // Taxi, Passenger
    using Unity.Collections; // NativeList, Allocator
    using Unity.Entities;    // Entity, RefRO, DynamicBuffer

    public partial class TaxiTrafficSystem
    {
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

                // Reversed OC requests advertise taxi supply; rider requests are not reversed.
                if ((serviceRef.ValueRO.m_Flags & Game.Simulation.ServiceRequestFlags.Reversed) == 0)
                    continue;

                toDestroy.Add(requestEntity);
            }

            if (toDestroy.Length == 0)
                return;

            EntityManager.DestroyEntity(toDestroy.AsArray());
            s_StatusOutsideSupplySuppressedTotal += toDestroy.Length;
        }

        private void ObserveOutsideTaxiMoveInEvidence()
        {
            using NativeList<Entity> seen = new(Allocator.Temp);

            foreach ((RefRO<Taxi> taxiRef, Entity taxiEntity) in SystemAPI
                         .Query<RefRO<Taxi>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                if ((taxiRef.ValueRO.m_State & Game.Vehicles.TaxiFlags.FromOutside) == 0 ||
                    !SystemAPI.HasBuffer<Game.Vehicles.Passenger>(taxiEntity))
                {
                    continue;
                }

                DynamicBuffer<Passenger> passengers =
                    SystemAPI.GetBuffer<Passenger>(taxiEntity);

                for (int i = 0; i < passengers.Length; i++)
                {
                    Entity passenger = passengers[i].m_Passenger;
                    if (passenger == Entity.Null ||
                        !SystemAPI.Exists(passenger) ||
                        SystemAPI.HasComponent<OutsideTaxiMoveInSeenMark>(passenger) ||
                        !SystemAPI.HasComponent<Resident>(passenger))
                    {
                        continue;
                    }

                    Resident resident = SystemAPI.GetComponentRO<Resident>(passenger).ValueRO;
                    if (!IsLocalMoveInFromOutsideConnection(passenger, resident))
                        continue;

                    seen.Add(passenger);
                }
            }

            if (seen.Length == 0)
                return;

            EntityManager.AddComponent<OutsideTaxiMoveInSeenMark>(seen.AsArray());
            s_StatusOutsideTaxiMoveInFromOcSeenTotal += seen.Length;
        }

        private bool IsLocalMoveInFromOutsideConnection(Entity residentEntity, Resident resident)
        {
            Entity citizen = resident.m_Citizen;
            if (citizen == Entity.Null ||
                !SystemAPI.Exists(citizen) ||
                !SystemAPI.HasComponent<HouseholdMember>(citizen))
            {
                return false;
            }

            Entity household = SystemAPI.GetComponentRO<HouseholdMember>(citizen).ValueRO.m_Household;
            if (household == Entity.Null ||
                !SystemAPI.Exists(household) ||
                !SystemAPI.HasComponent<Household>(household) ||
                SystemAPI.HasComponent<TouristHousehold>(household) ||
                SystemAPI.HasComponent<CommuterHousehold>(household))
            {
                return false;
            }

            Household householdData = SystemAPI.GetComponentRO<Household>(household).ValueRO;
            if ((householdData.m_Flags & HouseholdFlags.MovedIn) != 0 ||
                !SystemAPI.HasComponent<TripSource>(residentEntity))
            {
                return false;
            }

            Entity source = SystemAPI.GetComponentRO<TripSource>(residentEntity).ValueRO.m_Source;
            return source != Entity.Null &&
                   SystemAPI.Exists(source) &&
                   SystemAPI.HasComponent<Game.Objects.OutsideConnection>(source);
        }
    }
}
