using System;

namespace StudentSystem.Common
{
    // Клас Teacher також наслідує Person
    public class Teacher : Person
    {
        public string Subject { get; set; }
        public double Salary { get; set; }

        public Teacher(string fullName, int age, string subject, double salary)
            : base(fullName, age)
        {
            Subject = subject;
            Salary = salary;
        }

        // Статичний метод
        public static void ShowTotalPeople()
        {
            Console.WriteLine($"Total people in system: {Person.TotalPeople}");
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"{FullName} teaches {Subject}, salary: {Salary}");
        }
    }
}

