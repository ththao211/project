using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Sau này thêm:
        // public DbSet<Project> Projects { get; set; }
        // public DbSet<Task> Tasks { get; set; }
    }
}