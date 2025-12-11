using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Models;
using Tyuiu.Hits.Oop.FedorovaDA.V24.Models;

namespace NewsAggregator.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }
        public DbSet<UserBookmark> UserBookmarks { get; set; }
        public DbSet<UserReadHistory> UserReadHistory { get; set; }
    }

}
