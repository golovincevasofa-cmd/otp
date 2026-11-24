
namespace отп1.Models
{
    public class Fuel
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private Fuel(int id, string name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static Fuel Create(int id, string name,
            string? description)
        {
            if (id < 0)
                throw new ArgumentException("Некорректный ID.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Введите название топлива.");

            return new Fuel(id, name.Trim(),
                string.IsNullOrWhiteSpace(description)
                    ? null
                    : description.Trim());
        }

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Введите название топлива.");

            Name = name.Trim();
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();
        }
    }
}
