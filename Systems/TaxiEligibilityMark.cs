// File: Systems/TaxiEligibilityMarks.cs
// Marker components used by RiderControlSystem taxi eligibility batching.

namespace RiderControl
{
    using Unity.Entities;

    internal struct IgnoreTaxiMark : IComponentData
    {
    }

    internal struct TaxiAllowedMark : IComponentData
    {
    }
}
