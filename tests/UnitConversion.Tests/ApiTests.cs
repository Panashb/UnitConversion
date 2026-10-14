using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Tests;

public class ApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Convert_ValidRequest_Returns200WithResult()
    {
        var response = await _client.GetAsync(
            "/api/v1/conversions?category=temperature&from=celsius&to=fahrenheit&value=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Temperature", body.GetProperty("category").GetString());
        Assert.Equal(212m, body.GetProperty("result").GetDecimal());
    }

    [Theory]
    [InlineData("/api/v1/conversions?category=length&from=meter&to=kilogram&value=1")] // unit not in category
    [InlineData("/api/v1/conversions?category=length&from=meter&to=foot")]             // missing value
    [InlineData("/api/v1/conversions?category=length&from=meter&to=foot&value=abc")]   // not a number
    [InlineData("/api/v1/conversions?category=bogus&from=meter&to=foot&value=1")]      // bad category
    [InlineData("/api/v1/conversions?category=length&to=foot&value=1")]                // missing from
    public async Task Convert_InvalidRequest_Returns400(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Convert_Overflow_Returns400()
    {
        var response = await _client.GetAsync(
            "/api/v1/conversions?category=length&from=kilometer&to=meter&value=79228162514264337593543950335");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Units_FilteredByCategory_ReturnsOnlyThatCategory()
    {
        var units = await _client.GetFromJsonAsync<JsonElement>("/api/v1/units?category=mass");

        Assert.True(units.GetArrayLength() > 0);
        foreach (var unit in units.EnumerateArray())
        {
            Assert.Equal("Mass", unit.GetProperty("category").GetString());
        }
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
