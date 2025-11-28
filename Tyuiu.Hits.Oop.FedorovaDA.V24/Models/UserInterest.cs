using System.ComponentModel.DataAnnotations;
namespace NewsAggregator.Models
{
    public class UserInterest
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

}
