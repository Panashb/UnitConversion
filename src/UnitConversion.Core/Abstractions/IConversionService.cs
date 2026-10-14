using UnitConversion.Core.Models;

namespace UnitConversion.Core.Abstractions;

public interface IConversionService
{
    /// <summary>Converts <paramref name="value"/> between two units of the same category.</summary>
    /// <exception cref="Exceptions.UnknownUnitException">A unit is not defined in the category.</exception>
    ConversionResult Convert(UnitCategory category, string from, string to, decimal value);
}
