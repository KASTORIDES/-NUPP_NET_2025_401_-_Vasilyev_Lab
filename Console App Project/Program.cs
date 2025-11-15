using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Common;
using StudentSystem.Infrastructure;
using StudentSystem.Infrastructure.Models;
using StudentSystem.Infrastructure.Repositories;

namespace StudentSystem.ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Async thread-safe CRUD demo (SQLite) ===");

            string dbFile = Path.Combine(Environment.CurrentDirectory, "students.db");
            var options = new DbContextOptionsBuilder<StudentSystemContext>()
                .UseSqlite($"Data Source={dbFile}")
                .Options;

            using var context = new StudentSystemContext(options);
            context.Database.EnsureCreated(); // быстрая инициализация БД для демо

            var repo = new Repository<StudentModel>(context);
            var studentService = new CrudServiceAsync<StudentModel>(repo);

            AutoResetEvent doneEvent = new(false);
            int totalToCreate = 1000;
            int concurrentLimit = 50;
            int createdCount = 0;
            object counterLock = new();
            var sem = new SemaphoreSlim(concurrentLimit, concurrentLimit);
            var sw = Stopwatch.StartNew();
            var tasks = new List<Task>();

            for (int i = 0; i < totalToCreate; i++)
            {
                await sem.WaitAsync();
                var t = Task.Run(async () =>
                {
                    try
                    {
                        var s = Student.CreateNew(); // создаём domain-объект
                        var model = MapToModel(s);    // маппим в EF-модель
                        bool created = await studentService.CreateAsync(model);

                        if (created)
                        {
                            lock (counterLock) { createdCount++; }
                        }
                    }
                    finally
                    {
                        sem.Release();
                    }
                });
                tasks.Add(t);
            }

            Task.WhenAll(tasks).ContinueWith(_ => doneEvent.Set());
            Console.WriteLine("Waiting for parallel creation to finish...");
            doneEvent.WaitOne();
            sw.Stop();
            Console.WriteLine($"Created {createdCount} students in {sw.ElapsedMilliseconds} ms");

            bool saved = await studentService.SaveAsync();
            Console.WriteLine(saved ? $"Saved to {dbFile}" : "Failed to save.");

            var allStudents = (await studentService.ReadAllAsync()).ToList();

            if (allStudents.Any())
            {
                double min = allStudents.Min(s => s.AverageGrade);
                double max = allStudents.Max(s => s.AverageGrade);
                double avg = allStudents.Average(s => s.AverageGrade);
                Console.WriteLine($"Grades - Min: {min:F2}, Max: {max:F2}, Avg: {avg:F2}");

                var page2 = await studentService.ReadAllAsync(2, 10);
                Console.WriteLine($"\nPage 2 (10 items): {page2.Count()} items");
            }
            else
            {
                Console.WriteLine("No students present.");
            }

            var mre = new ManualResetEventSlim(false);
            Task.Run(() => { Thread.Sleep(200); mre.Set(); });
            mre.Wait();
            Console.WriteLine("ManualResetEventSlim signaled.");

            using (var quickSem = new SemaphoreSlim(1, 1))
            {
                await quickSem.WaitAsync();
                try { Console.WriteLine("Inside quick semaphore critical section"); }
                finally { quickSem.Release(); }
            }

            Teacher.ShowTotalPeople();
            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadKey();
        }

        static StudentModel MapToModel(Student s) => new StudentModel
        {
            FullName = s.FullName,
            Age = s.Age,
            AverageGrade = s.AverageGrade,
            Courses = new List<CourseModel>()
        };
    }
}
