
using отп1.Models;

namespace отп1.Data;

public interface IFuelRepository
{
    void Add(Fuel fuel);
    List<Fuel> GetAll();
    Fuel? GetById(int id);
    void Update(Fuel fuel);
    void Delete(int id);
}
