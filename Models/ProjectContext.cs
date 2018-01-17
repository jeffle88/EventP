using Microsoft.EntityFrameworkCore;
 
namespace Project.Models
{
    public class ProjectContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }
        public DbSet<User> user { get; set; } // User table
        public DbSet<Activity> activity { get; set; } // Activity table
        public DbSet<UserActivity> useractivity { get; set; } // UserActivity 3rd table
        public DbSet<Chat> chat { get; set; } // Chat Table
        public DbSet<MessageBoard> messageboard { get; set; } // Messageboard table
    }
}