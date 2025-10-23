using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentSystem.Common;
using System.Collections.Generic;

namespace StudentSystem.ConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("=== Async thread-safe CRUD demo ===");

            string storageFile = Path.Combine(Environment.CurrentDirectory, "students.json");
            var studentService = new CrudServiceAsync<Student>(storageFile);

            // Приклад використання AutoResetEvent для сигналізації завершення
            AutoResetEvent doneEvent = new(false);

            int totalToCreate = 1000;
            int concurrentLimit = 50; // для SemaphoreSlim прикладу

            // Лічильники з Lock
            int createdCount = 0;
            object counterLock = new();

            // SemaphoreSlim для обмеження паралельності при створенні
            var sem = new SemaphoreSlim(concurrentLimit, concurrentLimit);

            var sw = Stopwatch.StartNew();

            // Запуск паралельного створення об'єктів (Task-орієнтований)
            var tasks = new List<Task>();

            for (int i = 0; i < totalToCreate; i++)
            {
                await sem.WaitAsync(); // обмежуємо кількість одночасних тасків
                var t = Task.Run(async () =>
                {
                    try
                    {
                        var s = Student.CreateNew();
                        bool created = await studentService.CreateAsync(s);

                        if (created)
                        {
                            // lock приклад — збільшуємо лічильник без гонки
                            lock (counterLock)
                            {
                                createdCount++;
                            }
                        }
                    }
                    finally
                    {
                        sem.Release();
                    }
                });
                tasks.Add(t);
            }

            // Коли всі таски завершаться — сигналізуємо
            Task.WhenAll(tasks).ContinueWith(_ => doneEvent.Set());

            // Чекаємо сигналу (демонстрація AutoResetEvent)
            Console.WriteLine("Waiting for parallel creation to finish...");
            doneEvent.WaitOne(); // Заблокує потік Main до завершення створення

            sw.Stop();
            Console.WriteLine($"Created {createdCount} students in {sw.ElapsedMilliseconds} ms");

            // Демонстрація: збережемо колекцію у файл асинхронно
            bool saved = await studentService.SaveAsync();
            Console.WriteLine(saved ? $"Saved to {storageFile}" : "Failed to save.");

            // Тепер завантажимо з пам'яті та обчислимо min/max/avg для AverageGrade
            var allStudents = (await studentService.ReadAllAsync()).Cast<Student>().ToList();

            if (allStudents.Any())
            {
                double min = allStudents.Min(s => s.AverageGrade);
                double max = allStudents.Max(s => s.AverageGrade);
                double avg = allStudents.Average(s => s.AverageGrade);
                Console.WriteLine($"Grades - Min: {min:F2}, Max: {max:F2}, Avg: {avg:F2}");

                // Приклад пагінації: сторінка 2, по 10 елементів
                var page2 = await studentService.ReadAllAsync(2, 10);
                Console.WriteLine($"\nPage 2 (10 items): {page2.Count()} items");
            }
            else
            {
                Console.WriteLine("No students present.");
            }

            // Демонстрація інших синхронізаційних примітивів:
            // ManualResetEventSlim — на прикладі короткої імітації
            var mre = new ManualResetEventSlim(false);
            Task.Run(() =>
            {
                Thread.Sleep(200);
                mre.Set(); // сигналізує
            });
            mre.Wait(); // чекаємо сигналу
            Console.WriteLine("ManualResetEventSlim signaled.");

            // SemaphoreSlim використано вище; тут коротко ще раз
            using (var quickSem = new SemaphoreSlim(1, 1))
            {
                await quickSem.WaitAsync();
                try
                {
                    // критична секція
                    Console.WriteLine("Inside quick semaphore critical section");
                }
                finally
                {
                    quickSem.Release();
                }
            }

            // Приклад lock показано при збільшенні createdCount
            // Показати загальну кількість людей (статичне поле)
            Teacher.ShowTotalPeople(); // Person.TotalPeople відслідковує створені Person-об'єкти у поточному запуску

            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
