namespace UnitConversion.Api.Contracts;

public sealed record ConversionResponse(
    string Category,
    string From,
    string To,
    decimal InputValue,
    decimal Result);
