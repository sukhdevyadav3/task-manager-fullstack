using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models.Domain;

namespace TaskManagerAPI.Data
{
    public class TaskItemDbContext: DbContext
    {
        public TaskItemDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
        {
                
        }

        public DbSet<TaskItem> TaskItems{ get; set; }

        public DbSet<User> Users{ get; set; }
    }
}
