using UnitConversion.Core.Abstractions;
using UnitConversion.Core.Models;

namespace UnitConversion.Infrastructure;

/// <summary>
/// Hardcoded unit definitions for this version of the project.
/// Base units: meter (length), kilogram (mass), Celsius (temperature),
/// liter (volume), meter/second (speed).
/// </summary>
public sealed class InMemoryUnitRepository : IUnitRepository
{
    private static readonly MeasurementUnit[] Units =
    [
        // Length (base: meter)
        new("meter", "m", UnitCategory.Length, 1m),
        new("kilometer", "km", UnitCategory.Length, 1000m),
        new("centimeter", "cm", UnitCategory.Length, 0.01m),
        new("millimeter", "mm", UnitCategory.Length, 0.001m),
        new("mile", "mi", UnitCategory.Length, 1609.344m),
        new("yard", "yd", UnitCategory.Length, 0.9144m),
        new("foot", "ft", UnitCategory.Length, 0.3048m),
        new("inch", "in", UnitCategory.Length, 0.0254m),

        // Mass (base: kilogram)
        new("kilogram", "kg", UnitCategory.Mass, 1m),
        new("gram", "g", UnitCategory.Mass, 0.001m),
        new("milligram", "mg", UnitCategory.Mass, 0.000001m),
        new("pound", "lb", UnitCategory.Mass, 0.45359237m),
        new("ounce", "oz", UnitCategory.Mass, 0.028349523125m),
        new("stone", "st", UnitCategory.Mass, 6.35029318m),

        // Temperature (base: Celsius). base = value * factor + offset
        new("celsius", "C", UnitCategory.Temperature, 1m),
        new("fahrenheit", "F", UnitCategory.Temperature, 5m / 9m, -160m / 9m),
        new("kelvin", "K", UnitCategory.Temperature, 1m, -273.15m),

        // Volume (base: liter)
        new("liter", "l", UnitCategory.Volume, 1m),
        new("milliliter", "ml", UnitCategory.Volume, 0.001m),
        new("gallon", "gal", UnitCategory.Volume, 3.785411784m),
        new("quart", "qt", UnitCategory.Volume, 0.946352946m),
        new("cup", "cup", UnitCategory.Volume, 0.2365882365m),

        // Speed (base: meter/second)
        new("meter per second", "m/s", UnitCategory.Speed, 1m),
        new("kilometer per hour", "km/h", UnitCategory.Speed, 1000m / 3600m),
        new("mile per hour", "mph", UnitCategory.Speed, 0.44704m),
        new("knot", "kn", UnitCategory.Speed, 1852m / 3600m),
    ];

    public MeasurementUnit? Find(UnitCategory category, string nameOrSymbol) =>
        Units.FirstOrDefault(u => u.Category == category && Matches(u, nameOrSymbol));

    public IReadOnlyList<MeasurementUnit> GetAll(UnitCategory? category = null) =>
        category is null ? Units : Units.Where(u => u.Category == category).ToArray();

    private static bool Matches(MeasurementUnit unit, string input) =>
        string.Equals(unit.Name, input, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(unit.Symbol, input, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(unit.Name + "s", input, StringComparison.OrdinalIgnoreCase); // simple plural: "meters"
}
