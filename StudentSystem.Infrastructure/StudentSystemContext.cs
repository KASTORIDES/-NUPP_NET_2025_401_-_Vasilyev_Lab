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
        public DbSet<TeacherModel> Teachers { get; set; } = null!;
        public DbSet<StudentModel> Students { get; set; } = null!;
        public DbSet<CourseModel> Courses { get; set; } = null!;
        public DbSet<StudentDetailModel> StudentDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentModel>().HasKey(s => s.Id);
            modelBuilder.Entity<CourseModel>().HasKey(c => c.Id);
            modelBuilder.Entity<StudentDetailModel>().HasKey(d => d.Id);

            modelBuilder.Entity<StudentModel>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Student)
                .HasForeignKey(c => c.StudentModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentModel>()
                .HasOne(s => s.StudentDetail)
                .WithOne(d => d.Student)
                .HasForeignKey<StudentDetailModel>(d => d.Id);

            modelBuilder.Entity<CourseModel>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherModelId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
