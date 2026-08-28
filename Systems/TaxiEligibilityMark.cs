// <copyright file="TaxiEligibilityMark.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

namespace TaxiTraffic
{
    using Unity.Entities;

    // File: Systems/TaxiEligibilityMark.cs
    // Marker components used by TaxiTrafficSystem taxi eligibility batching.




    internal struct IgnoreTaxiMark : IComponentData
    {
    }

    internal struct TaxiAllowedMark : IComponentData
    {
    }

    // Temporary exemption while a Resident is linked to a travel group.
    // Kept separate so leaving the group sends the Resident back through normal eligibility.
    internal struct GroupTaxiAllowedMark : IComponentData
    {
    }

    // Temporary block while a cim is still at an outside-connection taxi pickup.
    internal struct OutsideTaxiBlockMark : IComponentData
    {
    }

    // We turned on IgnoreTaxi for the temporary OC block, so we may clear it later.
    internal struct OutsideTaxiOwnsIgnoreMark : IComponentData
    {
    }
}
