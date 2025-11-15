using System;

namespace StudentSystem.Common
{
    public abstract class Person
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }

        // Статичне поле
        public static int TotalPeople;

        // Статичний конструктор
        static Person()
        {
            TotalPeople = 0;
        }

        // Конструктор
        protected Person(string fullName, int age)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            Age = age;
            TotalPeople++;
        }

        // Метод
        public virtual void ShowInfo()
        {
            Console.WriteLine($"{FullName}, {Age} years old");
        }
    }
}
