using System;

namespace StudentSystem.Common
{
    // Клас Student наслідує Person
    public class Student : Person
    {
        public string Group { get; set; }
        public double AverageGrade { get; set; }

        // Делегат і подія
        public delegate void GradeChangedHandler(Student student);
        public event GradeChangedHandler? OnGradeChanged;

        // Конструктор
        public Student(string fullName, int age, string group, double averageGrade)
            : base(fullName, age)
        {
            Group = group;
            AverageGrade = averageGrade;
        }

        // Метод
        public void UpdateGrade(double newGrade)
        {
            AverageGrade = newGrade;
            OnGradeChanged?.Invoke(this);
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"{FullName}, Group: {Group}, Avg: {AverageGrade}");
        }
    }
}

