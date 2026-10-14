using UnitConversion.Core.Models;

namespace UnitConversion.Core.Abstractions;

/// <summary>
/// Source of unit definitions. The in-memory implementation can later be
/// replaced by a database-backed one without changing any callers.
/// </summary>
public interface IUnitRepository
{
    /// <summary>Returns the unit with the given name or symbol (case-insensitive), or null.</summary>
    MeasurementUnit? Find(UnitCategory category, string nameOrSymbol);

    /// <summary>Returns all units, optionally filtered by category.</summary>
    IReadOnlyList<MeasurementUnit> GetAll(UnitCategory? category = null);
}
