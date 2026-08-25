// <copyright file="TaxiEligibilityMark.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiEligibilityMarks.cs
// Marker components used by TaxiTrafficSystem taxi eligibility batching.

namespace TaxiTraffic
{
    using Unity.Entities;

    internal struct IgnoreTaxiMark : IComponentData
    {
    }

    internal struct TaxiAllowedMark : IComponentData
    {
    }
}
