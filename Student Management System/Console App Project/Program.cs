using System;
using StudentSystem.Common;
using System.Linq;

namespace StudentSystem.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var studentService = new CrudService<Student>();
            var teacherService = new CrudService<Teacher>();
            var courseService = new CrudService<Course>();

            // Створення об'єктів
            var student1 = new Student("Alice", 19, "CS-101", 4.5);
            var student2 = new Student("Bob", 20, "CS-102", 3.8);
            var teacher = new Teacher("Dr. Smith", 45, "Mathematics", 1200);
            var course = new Course("Programming", 5);

            // Підписка на подію
            student1.OnGradeChanged += s => Console.WriteLine($"{s.FullName}'s grade changed to {s.AverageGrade}");

            // Додавання до CRUD
            studentService.Create(student1);
            studentService.Create(student2);
            teacherService.Create(teacher);
            courseService.Create(course);

            // Оновлення оцінки
            student1.UpdateGrade(4.8);

            // Виведення
            Console.WriteLine("\n--- All Students ---");
            foreach (var s in studentService.ReadAll())
                s.ShowInfo();

            Console.WriteLine($"\nAverage grade: {studentService.ReadAll().AverageGrade():F2}");

            Console.WriteLine("\n--- Teachers ---");
            foreach (var t in teacherService.ReadAll())
                t.ShowInfo();

            Console.WriteLine("\n--- Courses ---");
            foreach (var c in courseService.ReadAll())
                Console.WriteLine(c);

            Teacher.ShowTotalPeople();
        }
    }
}
