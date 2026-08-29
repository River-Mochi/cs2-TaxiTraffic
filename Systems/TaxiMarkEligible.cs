// <copyright file="TaxiMarkEligible.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiMarkEligible.cs
// Purpose: Taxi Traffic ownership markers.

using Unity.Entities;

namespace TaxiTraffic
{
    // Active marker: Taxi Traffic turned on ResidentFlags.IgnoreTaxi.
    // This lets us clear only our own flag when settings later allow taxis.
    internal struct IgnoreTaxiMark : IComponentData
    {
    }

    // Legacy inert marker types kept temporarily so test saves/builds that knew
    // these component types remain compatible. New code never adds or queries them.
    internal struct TaxiAllowedMark : IComponentData
    {
    }

    internal struct GroupTaxiAllowedMark : IComponentData
    {
    }

    // Outside-connection feature markers. This feature is isolated from normal
    // resident eligibility and only runs when Block outside taxis is enabled.
    internal struct OutsideTaxiBlockMark : IComponentData
    {
    }

    internal struct OutsideTaxiOwnsIgnoreMark : IComponentData
    {
    }
}
