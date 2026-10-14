using System.ComponentModel.DataAnnotations;
using UnitConversion.Core.Models;

namespace UnitConversion.Api.Contracts;

//Query-string parameters for a conversion.
public sealed class ConvertRequest
{
    //Measurement category: Length, Temperature, Mass, Volume or Speed.
    [Required, EnumDataType(typeof(UnitCategory))]
    public UnitCategory? Category { get; init; }

    //Source unit name or symbol, e.g. "meter" or "m".
    [Required, MinLength(1)]
    public string? From { get; init; }

    //Target unit name or symbol, e.g. "foot" or "ft".
    [Required, MinLength(1)]
    public string? To { get; init; }

    //The number to convert.
    [Required]
    public decimal? Value { get; init; }
}
