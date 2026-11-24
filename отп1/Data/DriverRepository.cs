
using отп1.Models;

namespace отп1.Data;

public class DriverRepository : IDriverRepository
{
    private readonly List<Driver> _drivers = new();

    public void Add(Driver driver)
    {
        _drivers.Add(driver);
    }

    public List<Driver> GetAll()
    {
        return new List<Driver>(_drivers);
    }

    public Driver? GetById(int id)
    {
        return _drivers.FirstOrDefault(d => d.Id == id);
    }

    public void Update(Driver driver)
    {
        int index = _drivers.FindIndex(d => d.Id == driver.Id);

        if (index < 0)
            throw new InvalidOperationException("Водитель не найден.");

        _drivers[index] = driver;
    }

    public void Delete(int id)
    {
        _drivers.RemoveAll(d => d.Id == id);
    }
}
