using System;
using System.Collections.Generic;

namespace StudentSystem.Infrastructure.Models
{
    public class StudentModel
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public int Age { get; set; }
        public double AverageGrade { get; set; }

        // One-to-many: студент має багато предметів
        public List<CourseModel> Courses { get; set; } = new();
    }

    public class CourseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key
        public int StudentModelId { get; set; }
        public StudentModel Student { get; set; }
    }
}
