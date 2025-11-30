using System;

namespace StudentSystem.Common
{
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

        public override void ShowInfo()
        {
            Console.WriteLine($"{FullName} teaches {Subject}, salary: {Salary:F2}");
        }

        public static Teacher CreateNew()
        {
            var rnd = RandomProvider.GetThreadRandom();
            var names = new[] { "Dr. Smith", "Prof. Brown", "Dr. Green", "Prof. Black" };
            var subs = new[] { "Mathematics", "Programming", "Databases", "OS" };
            string name = names[rnd.Next(names.Length)];
            int age = rnd.Next(30, 70);
            string subject = subs[rnd.Next(subs.Length)];
            double salary = Math.Round(800 + rnd.NextDouble() * 2000, 2);
            return new Teacher(name, age, subject, salary);
        }

        public static void ShowTotalPeople()
        {
            Person.ShowTotalPeople();
        }
    }
}
