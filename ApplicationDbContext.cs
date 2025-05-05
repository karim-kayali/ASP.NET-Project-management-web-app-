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

             
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany()  
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
     .HasOne(t => t.Creator)
     .WithMany(u => u.CreatedTasks)
     .HasForeignKey(t => t.CreatorId)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithMany(u => u.Projects);  
        }
     

     


    }
}
