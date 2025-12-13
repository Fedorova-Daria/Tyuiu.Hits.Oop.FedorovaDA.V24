using Tyuiu.Hits.Oop.FedorovaDA.V24.Models;

namespace NewsAggregator.Models
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime PublishedDate { get; set; }

        public int SourceId { get; set; }
        public Source? Source { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
