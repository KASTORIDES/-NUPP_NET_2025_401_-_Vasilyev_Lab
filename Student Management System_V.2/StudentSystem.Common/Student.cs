using System;

namespace StudentSystem.Common
{
    public class Student : Person
    {
        public string Group { get; set; }
        public double AverageGrade { get; set; }

        // Делегат і подія
        public delegate void GradeChangedHandler(Student student);
        public event GradeChangedHandler? OnGradeChanged;

        // Статичний конструктор (приклад)
        static Student()
        {
            // тут можна ініціалізувати щось глобальне для Student-ів
        }

        public Student(string fullName, int age, string group, double averageGrade)
            : base(fullName, age)
        {
            Group = group;
            AverageGrade = averageGrade;
        }

        public void UpdateGrade(double newGrade)
        {
            AverageGrade = newGrade;
            OnGradeChanged?.Invoke(this);
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"{FullName}, Group: {Group}, Avg: {AverageGrade:F2}");
        }

        // Статичний метод — створює випадкового студента
        public static Student CreateNew()
        {
            var rnd = RandomProvider.GetThreadRandom();
            var names = new[] { "Alice", "Bob", "Charlie", "Diana", "Ethan", "Fiona", "George", "Helen" };
            var groups = new[] { "CS-101", "CS-102", "CS-201", "SE-101" };
            string name = names[rnd.Next(names.Length)] + " " + (char)('A' + rnd.Next(0, 26)) + ".";
            int age = rnd.Next(17, 30);
            string group = groups[rnd.Next(groups.Length)];
            double grade = Math.Round(2.0 + rnd.NextDouble() * 3.0, 2); // 2.0 - 5.0
            return new Student(name, age, group, grade);
        }
    }
}
