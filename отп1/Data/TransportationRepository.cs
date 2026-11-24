
using отп1.Models;

namespace отп1.Data;

public class TransportationRepository : ITransportationRepository
{
    private readonly List<Transportation> _transportations = new();
    private readonly ICarRepository _carRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IFuelRepository _fuelRepository;

    public TransportationRepository(
        ICarRepository carRepository,
        IDriverRepository driverRepository,
        IFuelRepository fuelRepository)
    {
        _carRepository = carRepository;
        _driverRepository = driverRepository;
        _fuelRepository = fuelRepository;
    }

    public void Add(Transportation transportation)
    {
        var car = _carRepository.GetById(transportation.CarId);
        var driver = _driverRepository.GetById(transportation.DriverId);
        var fuel = _fuelRepository.GetById(transportation.FuelId);

        if (car == null)
            throw new InvalidOperationException("Автомобиль не найден.");

        if (driver == null)
            throw new InvalidOperationException("Водитель не найден.");

        if (fuel == null)
            throw new InvalidOperationException("Топливо не найдено.");

        if (driver.CarId != car.Id)
            throw new InvalidOperationException(
                "Водитель не закреплён за выбранным автомобилем.");

        _transportations.Add(transportation);
    }

    public List<Transportation> GetAll()
    {
        return new List<Transportation>(_transportations);
    }

    public Transportation? GetById(int id)
    {
        return _transportations.FirstOrDefault(t => t.Id == id);
    }
}
