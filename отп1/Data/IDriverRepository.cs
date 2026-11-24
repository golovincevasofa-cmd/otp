
using отп1.Models;

namespace отп1.Data;

public interface IDriverRepository
{
    void Add(Driver driver);
    List<Driver> GetAll();
    Driver? GetById(int id);
    void Update(Driver driver);
    void Delete(int id);
}
