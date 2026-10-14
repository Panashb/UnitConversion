using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Core.Abstractions;
using UnitConversion.Core.Services;

namespace UnitConversion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUnitConversion(this IServiceCollection services)
    {
        services.AddSingleton<IUnitRepository, InMemoryUnitRepository>();
        services.AddSingleton<IConversionService, ConversionService>();
        return services;
    }
}
