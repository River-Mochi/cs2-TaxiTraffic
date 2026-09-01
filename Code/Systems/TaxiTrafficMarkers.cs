// <copyright file="TaxiTrafficMarkers.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficMarkers.cs
// Purpose: marks residents whose IgnoreTaxi flag is owned by Taxi Traffic.

using Colossal.Serialization.Entities;
using Unity.Entities;

namespace TaxiTraffic
{
    // Taxi Traffic turned on ResidentFlags.IgnoreTaxi for this resident.
    // Persist ownership across saves so Taxi Traffic can distinguish its
    // own flag from vanilla IgnoreTaxi after reloading the city.
    internal struct IgnoreTaxiMark :
        IComponentData,
        IQueryTypeParameter,
        IEmptySerializable
    {
    }
}
