namespace NewsAggregator.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsInteresting { get; set; } = false; // Флаг "Интересно пользователю"
        public List<Source> Sources { get; set; } = new();
    }
}
