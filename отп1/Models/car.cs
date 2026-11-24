
using отп1.Enums;

namespace отп1.Models
{
    public class Car
    {
        public int Id { get; private set; }
        public string LicensePlate { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public decimal Capacity { get; private set; }

        public VehicleStatus Status { get; private set; }
        public VehicleFeatures Features { get; private set; }

        private Car(
            int id,
            string licensePlate,
            string brand,
            string model,
            decimal capacity,
            VehicleStatus status,
            VehicleFeatures features)
        {
            Id = id;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            Capacity = capacity;
            Status = status;
            Features = features;
        }

        public static Car Create(
            int id,
            string licensePlate,
            string brand,
            string model,
            decimal capacity,
            VehicleStatus status = VehicleStatus.Available,
            VehicleFeatures features = VehicleFeatures.None)
        {
            if (id < 0)
                throw new ArgumentException("Некорректный ID.");

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("Введите госномер автомобиля.");

            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Введите марку автомобиля.");

            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Введите модель автомобиля.");

            if (capacity <= 0)
                throw new ArgumentException("Вместимость должна быть больше нуля.");

            return new Car(
                id,
                licensePlate.Trim(),
                brand.Trim(),
                model.Trim(),
                capacity,
                status,
                features);
        }

        public void Update(
            string licensePlate,
            string brand,
            string model,
            decimal capacity,
            VehicleStatus? status = null,
            VehicleFeatures? features = null)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("Введите госномер автомобиля.");

            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Введите марку автомобиля.");

            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Введите модель автомобиля.");

            if (capacity <= 0)
                throw new ArgumentException("Вместимость должна быть больше нуля.");

            LicensePlate = licensePlate.Trim();
            Brand = brand.Trim();
            Model = model.Trim();
            Capacity = capacity;

            if (status.HasValue)
                Status = status.Value;

            if (features.HasValue)
                Features = features.Value;
        }
    }
}
