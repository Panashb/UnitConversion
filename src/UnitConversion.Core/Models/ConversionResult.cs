namespace UnitConversion.Core.Models;

/// <summary>The outcome of a successful conversion.</summary>
public sealed record ConversionResult(
    UnitCategory Category,
    MeasurementUnit From,
    MeasurementUnit To,
    decimal InputValue,
    decimal Result);
