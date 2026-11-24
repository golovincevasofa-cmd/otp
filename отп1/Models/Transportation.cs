
namespace отп1.Models
{
    public class Transportation
    {
        public int Id { get; private set; }
        public int CarId { get; private set; }
        public int DriverId { get; private set; }
        public int FuelId { get; private set; }
        public decimal Quantity { get; private set; }
        public DateTime TransportationDate { get; private set; }

        private Transportation(int id, int carId, int driverId,
            int fuelId, decimal quantity, DateTime transportationDate)
        {
            Id = id;
            CarId = carId;
            DriverId = driverId;
            FuelId = fuelId;
            Quantity = quantity;
            TransportationDate = transportationDate.Date;
        }

        public static Transportation Create(int id, int carId,
            int driverId, int fuelId, decimal quantity,
            DateTime transportationDate)
        {
            if (id < 0)
                throw new ArgumentException("Некорректный ID.");

            if (carId <= 0)
                throw new ArgumentException("Выберите автомобиль.");

            if (driverId <= 0)
                throw new ArgumentException("Выберите водителя.");

            if (fuelId <= 0)
                throw new ArgumentException("Выберите топливо.");

            if (quantity <= 0)
                throw new ArgumentException("Количество топлива должно быть больше нуля.");

            if (transportationDate == default)
                throw new ArgumentException("Укажите дату перевозки.");

            return new Transportation(id, carId, driverId,
                fuelId, quantity, transportationDate);
        }
    }
}
