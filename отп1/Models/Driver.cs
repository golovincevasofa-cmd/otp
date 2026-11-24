
namespace отп1.Models
{
    public class Driver
    {
        public int Id { get; private set; }
        public string FullName { get; private set; }
        public string LicenseNumber { get; private set; }
        public int CarId { get; private set; }

        private Driver(int id, string fullName,
            string licenseNumber, int carId)
        {
            Id = id;
            FullName = fullName;
            LicenseNumber = licenseNumber;
            CarId = carId;
        }

        public static Driver Create(int id, string fullName,
            string licenseNumber, int carId)
        {
            if (id < 0)
                throw new ArgumentException("Некорректный ID.");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Введите ФИО водителя.");

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("Введите номер водительского удостоверения.");

            if (carId <= 0)
                throw new ArgumentException("Выберите автомобиль.");

            return new Driver(id, fullName.Trim(),
                licenseNumber.Trim(), carId);
        }

        public void Update(string fullName,
            string licenseNumber, int carId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Введите ФИО водителя.");

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("Введите номер водительского удостоверения.");

            if (carId <= 0)
                throw new ArgumentException("Выберите автомобиль.");

            FullName = fullName.Trim();
            LicenseNumber = licenseNumber.Trim();
            CarId = carId;
        }
    }
}

