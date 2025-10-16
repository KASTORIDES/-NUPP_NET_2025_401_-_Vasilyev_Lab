using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static readonly object _consoleLock = new();
    private static readonly SemaphoreSlim _parallelSemaphore = new(20);
    private static readonly AutoResetEvent _saveCompleted = new(false);

    static async Task Main(string[] args)
    {
        var service = new StudentService("students.json");

        while (true)
        {
            Console.WriteLine("\n--- Student Management System (Lab2) ---");
            Console.WriteLine("1. Add Student (interactive)");
            Console.WriteLine("2. Remove Student by name");
            Console.WriteLine("3. Find Student by name");
            Console.WriteLine("4. Show All Students (first 50)");
            Console.WriteLine("5. Save to file now");
            Console.WriteLine("6. Run Lab2 automated scenario (create 1000 in parallel, compute stats, save)");
            Console.WriteLine("7. Read page (pagination example)");
            Console.WriteLine("8. Exit");
            Console.Write("Choose option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            switch (choice)
            {
                case 1:
                    await AddInteractive(service);
                    break;
                case 2:
                    await RemoveByName(service);
                    break;
                case 3:
                    await FindByName(service);
                    break;
                case 4:
                    await ShowAll(service);
                    break;
                case 5:
                    _ = Task.Run(async () =>
                    {
                        var ok = await service.SaveAsync();
                        lock (_consoleLock) Console.WriteLine(ok ? "Save succeeded." : "Save failed.");
                        _saveCompleted.Set();
                    });
                    
                    if (!_saveCompleted.WaitOne(10000))
                    {
                        Console.WriteLine("Save did not complete in 10 seconds.");
                    }
                    break;
                case 6:
                    await RunAutomatedScenario(service);
                    break;
                case 7:
                    await PaginationExample(service);
                    break;
                case 8:
                    Console.WriteLine("Exiting...");
                    service.Dispose();
                    return;
                default:
                    Console.WriteLine("Invalid option!");
                    break;
            }
        }
    }

    private static async Task AddInteractive(StudentService service)
    {
        Console.Write("Enter name: ");
        var name = Console.ReadLine();
        Console.Write("Enter group: ");
        var group = Console.ReadLine();
        Console.Write("Enter average grade: ");
        if (!double.TryParse(Console.ReadLine(), out double grade))
        {
            Console.WriteLine("Invalid grade.");
            return;
        }
        var s = new Student(name, group, grade);
        var added = await service.CreateAsync(s);
        Console.WriteLine(added ? "Student added." : "Add failed (maybe duplicate Id).");
    }

    private static async Task RemoveByName(StudentService service)
    {
        Console.Write("Enter name to remove: ");
        var name = Console.ReadLine();
        var all = (await service.ReadAllAsync()).ToList();
        var matches = all.Where(st => st.Name == name).ToList();
        if (!matches.Any())
        {
            Console.WriteLine("No students with that name.");
            return;
        }
        foreach (var m in matches)
        {
            await service.RemoveAsync(m);
        }
        Console.WriteLine($"Removed {matches.Count} student(s) named {name}.");
        
        var ok = await service.SaveAsync();
        Console.WriteLine(ok ? "Saved after removal." : "Save failed.");
    }

    private static async Task FindByName(StudentService service)
    {
        Console.Write("Enter name to find: ");
        var name = Console.ReadLine();
        var all = (await service.ReadAllAsync()).ToList();
        var found = all.FirstOrDefault(s => s.Name == name);
        Console.WriteLine(found != null ? found.ToString() : "Not found.");
    }

    private static async Task ShowAll(StudentService service)
    {
        var all = (await service.ReadAllAsync()).ToList();
        if (!all.Any())
        {
            Console.WriteLine("No students in the list.");
            return;
        }
        foreach (var s in all.Take(50)) 
        {
            Console.WriteLine(s);
        }
        if (all.Count > 50) Console.WriteLine($"... ({all.Count - 50} more)");
    }

    private static async Task RunAutomatedScenario(StudentService service)
    {
        const int itemsToCreate = 1000;
        Console.WriteLine($"Creating {itemsToCreate} students in parallel...");

        
        Parallel.For(0, itemsToCreate, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
        {
            
            _parallelSemaphore.Wait();
            try
            {
                var student = Student.CreateNew();
                service.CreateAsync(student).GetAwaiter().GetResult();

                if ((i + 1) % 100 == 0)
                {
                    lock (_consoleLock)
                    {
                        Console.WriteLine($"Created {i + 1} students...");
                    }
                }
            }
            finally
            {
                _parallelSemaphore.Release();
            }
        });

        Console.WriteLine("Creation finished. Computing statistics...");

        var all = (await service.ReadAllAsync()).ToList();

        if (!all.Any())
        {
            Console.WriteLine("No students found after creation.");
            return;
        }

        var min = all.Min(s => s.AverageGrade);
        var max = all.Max(s => s.AverageGrade);
        var avg = all.Average(s => s.AverageGrade);

        lock (_consoleLock)
        {
            Console.WriteLine("=== AverageGrade Stats ===");
            Console.WriteLine($"Min: {min:F2}");
            Console.WriteLine($"Max: {max:F2}");
            Console.WriteLine($"Avg: {avg:F2}");
        }

        var saveTask = service.SaveAsync();
        var saved = await saveTask;
        Console.WriteLine(saved ? $"Saved {all.Count} students to file." : "Save failed.");
    }

    private static async Task PaginationExample(StudentService service)
    {
        Console.Write("Enter page number (1..): ");
        if (!int.TryParse(Console.ReadLine(), out int page)) page = 1;
        Console.Write("Enter amount per page: ");
        if (!int.TryParse(Console.ReadLine(), out int amount)) amount = 10;

        var pageItems = (await service.ReadAllAsync(page, amount)).ToList();
        if (!pageItems.Any())
        {
            Console.WriteLine("No items on this page.");
            return;
        }
        Console.WriteLine($"Page {page}, {pageItems.Count} items:");
        foreach (var s in pageItems) Console.WriteLine(s);
    }
}
