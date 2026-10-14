using UnitConversion.Core.Models;
using UnitConversion.Infrastructure;

namespace UnitConversion.Tests;

public class InMemoryUnitRepositoryTests
{
    private readonly InMemoryUnitRepository _repository = new();

    [Fact]
    public void GetAll_WithoutCategory_ReturnsEveryCategory()
    {
        var categories = _repository.GetAll().Select(u => u.Category).Distinct();

        Assert.Equal(Enum.GetValues<UnitCategory>().Length, categories.Count());
    }

    [Fact]
    public void GetAll_WithCategory_ReturnsOnlyThatCategory()
    {
        var units = _repository.GetAll(UnitCategory.Mass);

        Assert.NotEmpty(units);
        Assert.All(units, u => Assert.Equal(UnitCategory.Mass, u.Category));
    }

    [Fact]
    public void Find_UnknownUnit_ReturnsNull()
    {
        Assert.Null(_repository.Find(UnitCategory.Length, "nope"));
    }

    [Fact]
    public void Units_HaveUniqueNamesAndSymbolsWithinCategory()
    {
        foreach (var category in Enum.GetValues<UnitCategory>())
        {
            var units = _repository.GetAll(category);

            Assert.Equal(units.Count, units.Select(u => u.Name.ToLowerInvariant()).Distinct().Count());
            Assert.Equal(units.Count, units.Select(u => u.Symbol.ToLowerInvariant()).Distinct().Count());
        }
    }
}
