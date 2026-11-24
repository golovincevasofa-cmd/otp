
using отп1.Models;

namespace отп1.Data;

public interface ICarRepository
{
    void Add(Car car);
    List<Car> GetAll();
    Car? GetById(int id);
    void Update(Car car);
    void Delete(int id);
}
