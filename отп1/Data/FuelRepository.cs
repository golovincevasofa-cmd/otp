
using отп1.Models;

namespace отп1.Data;

public class FuelRepository : IFuelRepository
{
    private readonly List<Fuel> _fuels = new();

    public void Add(Fuel fuel)
    {
        _fuels.Add(fuel);
    }

    public List<Fuel> GetAll()
    {
        return new List<Fuel>(_fuels);
    }

    public Fuel? GetById(int id)
    {
        return _fuels.FirstOrDefault(f => f.Id == id);
    }

    public void Update(Fuel fuel)
    {
        int index = _fuels.FindIndex(f => f.Id == fuel.Id);

        if (index < 0)
            throw new InvalidOperationException("Топливо не найдено.");

        _fuels[index] = fuel;
    }

    public void Delete(int id)
    {
        _fuels.RemoveAll(f => f.Id == id);
    }
}
