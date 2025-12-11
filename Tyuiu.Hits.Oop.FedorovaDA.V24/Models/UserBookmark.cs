namespace Tyuiu.Hits.Oop.FedorovaDA.V24.Models
{
    public class UserBookmark
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ArticleId { get; set; } 
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
