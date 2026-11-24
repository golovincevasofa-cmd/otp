
using отп1.Data;
using отп1.Models;

namespace отп1.Data;

public class CarRepository : ICarRepository
{
    private readonly List<Car> _cars = new();

    public void Add(Car car)
    {
        _cars.Add(car);
    }

    public List<Car> GetAll()
    {
        return new List<Car>(_cars);
    }

    public Car? GetById(int id)
    {
        return _cars.FirstOrDefault(c => c.Id == id);
    }

    public void Update(Car car)
    {
        int index = _cars.FindIndex(c => c.Id == car.Id);

        if (index < 0)
            throw new InvalidOperationException("Автомобиль не найден");

        _cars[index] = car;
    }

    public void Delete(int id)
    {
        _cars.RemoveAll(c => c.Id == id);
    }
}
