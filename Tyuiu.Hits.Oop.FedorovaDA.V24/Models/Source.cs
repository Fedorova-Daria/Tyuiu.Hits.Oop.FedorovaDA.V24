namespace NewsAggregator.Models
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RssUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}
