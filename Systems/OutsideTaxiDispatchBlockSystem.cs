// <copyright file="OutsideTaxiDispatchBlockSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/OutsideTaxiDispatchBlockSystem.cs
// Stops taxi jobs that vanilla assigns directly to OC-owned taxi sources.

namespace TaxiTraffic
{
    using Game;            // GameSystemBase, GameMode
    using Unity.Entities;  // Entity, RefRO, DynamicBuffer

    internal partial class OutsideTaxiDispatchBlockSystem : GameSystemBase
    {
        private const int kUpdateIntervalFrames = 16;

        // Match TaxiDispatch so OC assignments are removed in the same simulation tick.
        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return kUpdateIntervalFrames;
        }

        public override int GetUpdateOffset(SystemUpdatePhase phase)
        {
            return 0;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            Enabled =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                 purpose == Colossal.Serialization.Entities.Purpose.LoadGame);
        }

        protected override void OnUpdate()
        {
            TaxiSettings? setting = Mod.Setting;
            if (setting is null || !setting.BlockOutsideTaxis)
                return;

            RemoveOcDepotTaxiDispatches();
            RemoveOcOwnedTaxiDispatches();
        }

        private void RemoveOcDepotTaxiDispatches()
        {
            foreach ((RefRO<Game.Buildings.TransportDepot> _, Entity source) in SystemAPI
                         .Query<RefRO<Game.Buildings.TransportDepot>>()
                         .WithAll<Game.Objects.OutsideConnection>()
                         .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                         .WithEntityAccess())
            {
                RemovePendingTaxiDispatches(source);
            }
        }

        private void RemoveOcOwnedTaxiDispatches()
        {
            foreach ((RefRO<Game.Vehicles.Taxi> _,
                      RefRO<Game.Common.Owner> ownerRef,
                      Entity source) in SystemAPI
                         .Query<RefRO<Game.Vehicles.Taxi>, RefRO<Game.Common.Owner>>()
                         .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                         .WithEntityAccess())
            {
                Entity owner = ownerRef.ValueRO.m_Owner;
                if (owner == Entity.Null ||
                    !SystemAPI.Exists(owner) ||
                    !SystemAPI.HasComponent<Game.Objects.OutsideConnection>(owner))
                {
                    continue;
                }

                RemovePendingTaxiDispatches(source);
            }
        }

        private void RemovePendingTaxiDispatches(Entity source)
        {
            if (!SystemAPI.HasBuffer<Game.Simulation.ServiceDispatch>(source))
                return;

            DynamicBuffer<Game.Simulation.ServiceDispatch> dispatches =
                SystemAPI.GetBuffer<Game.Simulation.ServiceDispatch>(source);

            for (int i = dispatches.Length - 1; i >= 0; i--)
            {
                Entity request = dispatches[i].m_Request;
                if (request == Entity.Null ||
                    !SystemAPI.Exists(request) ||
                    !SystemAPI.HasComponent<Game.Simulation.TaxiRequest>(request))
                {
                    continue;
                }

                // Let vanilla reset the unhandled request and try another source.
                dispatches.RemoveAt(i);
            }
        }
    }
}

