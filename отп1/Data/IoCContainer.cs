
using Microsoft.Extensions.DependencyInjection;
using System;

namespace отп1.Data;

public static class IoCContainer
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ICarRepository, CarRepository>();
        services.AddSingleton<IDriverRepository, DriverRepository>();
        services.AddSingleton<IFuelRepository, FuelRepository>();
        services.AddSingleton<ITransportationRepository, TransportationRepository>();

        return services.BuildServiceProvider();
    }
}
