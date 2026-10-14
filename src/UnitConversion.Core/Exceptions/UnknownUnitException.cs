using UnitConversion.Core.Models;

namespace UnitConversion.Core.Exceptions;

/// <summary>Thrown when a requested unit does not exist in the requested category.</summary>
public sealed class UnknownUnitException(UnitCategory category, string unit)
    : Exception($"Unit '{unit}' is not supported in category '{category}'.")
{
    public UnitCategory Category { get; } = category;
    public string Unit { get; } = unit;
}
