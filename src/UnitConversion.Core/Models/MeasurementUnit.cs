namespace UnitConversion.Core.Models;

/// <summary>
/// A unit of measurement, expressed relative to its category's base unit:
/// <c>baseValue = value * FactorToBase + OffsetToBase</c>.
/// Most units only need a factor; temperature scales also need an offset.
/// </summary>
/// <param name="Name">Full name, e.g. "meter".</param>
/// <param name="Symbol">Short symbol, e.g. "m".</param>
/// <param name="Category">The category this unit belongs to.</param>
/// <param name="FactorToBase">Multiplier that converts this unit to the base unit.</param>
/// <param name="OffsetToBase">Amount added after multiplying (non-zero for Celsius/Fahrenheit-style scales).</param>
public sealed record MeasurementUnit(
    string Name,
    string Symbol,
    UnitCategory Category,
    decimal FactorToBase,
    decimal OffsetToBase = 0m);
