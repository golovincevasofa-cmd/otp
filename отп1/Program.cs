
using Microsoft.Extensions.DependencyInjection;
using отп1.Data;

namespace отп1;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using ServiceProvider serviceProvider = IoCContainer.Build();

        var carRepository =
            serviceProvider.GetRequiredService<ICarRepository>();

        var driverRepository =
            serviceProvider.GetRequiredService<IDriverRepository>();

        var fuelRepository =
            serviceProvider.GetRequiredService<IFuelRepository>();

        var transportationRepository =
            serviceProvider.GetRequiredService<ITransportationRepository>();

        Application.Run(new Form1(
            carRepository,
            driverRepository,
            fuelRepository,
            transportationRepository));
    }
}
