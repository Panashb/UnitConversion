using UnitConversion.Core.Abstractions;
using UnitConversion.Core.Exceptions;
using UnitConversion.Core.Models;

namespace UnitConversion.Core.Services;

public sealed class ConversionService(IUnitRepository repository) : IConversionService
{
    /// <summary>Results are rounded to this many significant digits to hide decimal arithmetic noise.</summary>
    public const int SignificantDigits = 15;

    public ConversionResult Convert(UnitCategory category, string from, string to, decimal value)
    {
        var source = repository.Find(category, from) ?? throw new UnknownUnitException(category, from);
        var target = repository.Find(category, to) ?? throw new UnknownUnitException(category, to);

        // value -> base unit -> target unit
        var baseValue = value * source.FactorToBase + source.OffsetToBase;
        var raw = (baseValue - target.OffsetToBase) / target.FactorToBase;

        return new ConversionResult(category, source, target, value, Normalize(raw));
    }

    // Round to significant digits (so tiny results keep precision), then strip trailing zeros
    // (212.000000000000 -> 212).
    private static decimal Normalize(decimal value)
    {
        if (value == 0m)
        {
            return 0m;
        }

        var magnitude = (int)Math.Floor(Math.Log10((double)Math.Abs(value))) + 1;
        var decimals = Math.Clamp(SignificantDigits - magnitude, 0, 28);
        return Math.Round(value, decimals, MidpointRounding.ToEven) / 1.0000000000000000000000000000m;
    }
}
