using Microsoft.EntityFrameworkCore;  // Import Entity Framework Core to enable database operations.
using TaskManagerAPI.Models.Domain;  // Import domain models (TaskItem and User) for use with EF Core.

namespace TaskManagerAPI.Data  // Namespace for the data access layer.
{
    public class TaskItemDbContext : DbContext  // Define a DbContext class to interact with the database.
    {
        // Constructor to pass DbContextOptions to the base DbContext class.
        public TaskItemDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            // The base constructor initializes the DbContext with the provided options.
        }

        // DbSet property for TaskItems table in the database.
        public DbSet<TaskItem> TaskItems { get; set; }

        // DbSet property for Users table in the database.
        public DbSet<User> Users { get; set; }
    }
}
