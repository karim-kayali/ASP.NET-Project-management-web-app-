using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AdvancedFinalProject.Models;

namespace AdvancedFinalProject
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<TaskItem> tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-Many relationship between Project and Creator (User)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany() // A User (Creator) can have many Projects
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many relationship between TaskItem and Creator (User)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Creator)
                .WithMany() // A User (Creator) can have many TaskItems
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many relationship between TaskItem and Assignee (User)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany() // A User (Assignee) can be assigned many TaskItems
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many relationship between Project and User (Members)
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithMany(u => u.Projects);  
        }


     


    }
}
