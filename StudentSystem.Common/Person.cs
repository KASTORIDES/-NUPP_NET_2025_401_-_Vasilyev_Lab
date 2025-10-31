using System;

namespace StudentSystem.Common
{
    public class Person
    {
        // Публичный счётчик с приватным сеттером — безопасно для чтения
        public static int TotalPeople { get; private set; } = 0;

        // Сохраняем Name для обратной совместимости, добавляем FullName
        public string Name { get; set; }
        public string FullName
        {
            get => Name;
            set => Name = value;
        }

        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
            TotalPeople++;
        }

        // Виртуальный метод для переопределения в наследниках
        public virtual void ShowInfo()
        {
            Console.WriteLine($"{FullName}, age {Age}");
        }

        public static void ShowTotalPeople()
        {
            Console.WriteLine($"Total people: {TotalPeople}");
        }
    }
}