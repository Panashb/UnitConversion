using UnitConversion.Core.Exceptions;
using UnitConversion.Core.Models;
using UnitConversion.Core.Services;
using UnitConversion.Infrastructure;

namespace UnitConversion.Tests;

public class ConversionServiceTests
{
    private readonly InMemoryUnitRepository _repository = new();
    private readonly ConversionService _service;

    public ConversionServiceTests() => _service = new ConversionService(_repository);

    [Theory]
    [InlineData(UnitCategory.Length, "meter", "kilometer", 1000, 1)]
    [InlineData(UnitCategory.Length, "mile", "kilometer", 1, 1.609344)]
    [InlineData(UnitCategory.Length, "inch", "centimeter", 1, 2.54)]
    [InlineData(UnitCategory.Length, "foot", "inch", 1, 12)]
    [InlineData(UnitCategory.Mass, "kilogram", "gram", 2, 2000)]
    [InlineData(UnitCategory.Mass, "pound", "kilogram", 1, 0.45359237)]
    [InlineData(UnitCategory.Mass, "pound", "ounce", 1, 16)]
    [InlineData(UnitCategory.Temperature, "celsius", "fahrenheit", 100, 212)]
    [InlineData(UnitCategory.Temperature, "celsius", "fahrenheit", 0, 32)]
    [InlineData(UnitCategory.Temperature, "fahrenheit", "celsius", -40, -40)]
    [InlineData(UnitCategory.Temperature, "kelvin", "celsius", 0, -273.15)]
    [InlineData(UnitCategory.Temperature, "celsius", "kelvin", -273.15, 0)]
    [InlineData(UnitCategory.Volume, "gallon", "liter", 1, 3.785411784)]
    [InlineData(UnitCategory.Volume, "gallon", "quart", 1, 4)]
    [InlineData(UnitCategory.Speed, "mph", "km/h", 60, 96.56064)]
    public void Convert_ReturnsExpectedValue(
        UnitCategory category, string from, string to, double value, double expected)
    {
        var result = _service.Convert(category, from, to, (decimal)value);

        Assert.Equal((decimal)expected, result.Result, precision: 6);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsInput()
    {
        var result = _service.Convert(UnitCategory.Length, "meter", "meter", 42.5m);

        Assert.Equal(42.5m, result.Result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-15.5)]
    public void Convert_HandlesZeroAndNegativeValues(double value)
    {
        var result = _service.Convert(UnitCategory.Length, "meter", "foot", (decimal)value);

        Assert.Equal(Math.Sign(value), Math.Sign(result.Result));
    }

    [Theory]
    [InlineData("METER", "Foot")]
    [InlineData("m", "ft")]
    [InlineData("Meter", "FT")]
    public void Convert_MatchesNamesAndSymbolsCaseInsensitively(string from, string to)
    {
        var result = _service.Convert(UnitCategory.Length, from, to, 1m);

        Assert.Equal(3.280839895m, result.Result, precision: 6);
    }

    [Fact]
    public void Convert_AcceptsSimplePlurals()
    {
        var result = _service.Convert(UnitCategory.Length, "meters", "kilometers", 500m);

        Assert.Equal(0.5m, result.Result);
    }

    [Fact]
    public void Convert_UnknownUnit_Throws()
    {
        var ex = Assert.Throws<UnknownUnitException>(
            () => _service.Convert(UnitCategory.Length, "meter", "parsec", 1m));

        Assert.Equal("parsec", ex.Unit);
    }

    [Fact]
    public void Convert_UnitFromAnotherCategory_Throws()
    {
        Assert.Throws<UnknownUnitException>(
            () => _service.Convert(UnitCategory.Length, "meter", "kilogram", 1m));
    }

    [Fact]
    public void Convert_RoundTrip_ReturnsOriginalValueForEveryUnitPair()
    {
        const decimal original = 123.456m;

        foreach (var category in Enum.GetValues<UnitCategory>())
        {
            var units = _repository.GetAll(category);
            foreach (var a in units)
            {
                foreach (var b in units)
                {
                    var there = _service.Convert(category, a.Name, b.Name, original).Result;
                    var back = _service.Convert(category, b.Name, a.Name, there).Result;

                    Assert.Equal(original, back, precision: 8);
                }
            }
        }
    }
}
