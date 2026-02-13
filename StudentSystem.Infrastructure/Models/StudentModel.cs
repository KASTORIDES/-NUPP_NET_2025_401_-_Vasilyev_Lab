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

        // One-to-one: подробиці про студента
        public StudentDetailModel StudentDetail { get; set; }
    }

    public class CourseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key to Student
        public int StudentModelId { get; set; }
        public StudentModel Student { get; set; }

        // Foreign key to Teacher (преподаватель ведёт этот курс)
        public int? TeacherModelId { get; set; }
        public TeacherModel? Teacher { get; set; }
    }

    public class TeacherModel
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Subject { get; set; }
        public double Salary { get; set; }

        // One-to-many: преподаватель ведёт много курсов
        public List<CourseModel> Courses { get; set; } = new();
    }

    public class StudentDetailModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public StudentModel Student { get; set; }
    }
}
