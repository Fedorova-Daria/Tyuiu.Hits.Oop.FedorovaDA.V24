namespace Tyuiu.Hits.Oop.FedorovaDA.V24.Models
{
    public class UserReadHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime ReadDate { get; set; } = DateTime.UtcNow;
    }
}
