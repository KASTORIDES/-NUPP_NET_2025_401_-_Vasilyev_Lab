class Program
{
    static void Main(string[] args)
    {
        StudentManager manager = new StudentManager();

        while (true)
        {
            Console.WriteLine("\n--- Student Management System ---");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Remove Student");
            Console.WriteLine("3. Find Student");
            Console.WriteLine("4. Show All Students");
            Console.WriteLine("5. Exit");
            Console.Write("Choose option: ");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Enter name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter group: ");
                    string group = Console.ReadLine();
                    Console.Write("Enter average grade: ");
                    double grade = double.Parse(Console.ReadLine());

                    manager.AddStudent(new Student(name, group, grade));
                    Console.WriteLine("Student added!");
                    break;

                case 2:
                    Console.Write("Enter name to remove: ");
                    manager.RemoveStudent(Console.ReadLine());
                    Console.WriteLine("Student removed!");
                    break;

                case 3:
                    Console.Write("Enter name to find: ");
                    var student = manager.FindStudent(Console.ReadLine());
                    Console.WriteLine(student != null ? student.ToString() : "Not found.");
                    break;

                case 4:
                    manager.ShowAllStudents();
                    break;

                case 5:
                    return;

                default:
                    Console.WriteLine("Invalid option!");
                    break;
            }
        }
    }
}
