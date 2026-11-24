
using отп1.Models;

namespace отп1.Data;

public interface ITransportationRepository
{
    void Add(Transportation transportation);
    List<Transportation> GetAll();
    Transportation? GetById(int id);
}
