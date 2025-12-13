using Tyuiu.Hits.Oop.FedorovaDA.V24.Models;

namespace NewsAggregator.Models
{
    public class Source : BaseEntity
    {
        public string Name { get; set; }
        public string RssUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}
