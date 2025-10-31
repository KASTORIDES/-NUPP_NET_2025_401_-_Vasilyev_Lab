using Microsoft.EntityFrameworkCore;
using StudentSystem.Infrastructure.Models;

namespace StudentSystem.Infrastructure
{
        public class StudentSystemContext : DbContext
    {
        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
            : base(options)
        {
        }

        public DbSet<StudentModel> Students { get; set; } = null!;
        public DbSet<CourseModel> Courses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentModel>().HasKey(s => s.Id);
            modelBuilder.Entity<CourseModel>().HasKey(c => c.Id);

            modelBuilder.Entity<StudentModel>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Student)
                .HasForeignKey(c => c.StudentModelId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
