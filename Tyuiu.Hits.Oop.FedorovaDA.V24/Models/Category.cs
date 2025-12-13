using Tyuiu.Hits.Oop.FedorovaDA.V24.Models;

namespace NewsAggregator.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsInteresting { get; set; } = false; // Флаг "Интересно пользователю"
        public List<Source> Sources { get; set; } = new();
    }
}
