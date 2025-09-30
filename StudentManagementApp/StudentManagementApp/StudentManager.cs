using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StudentManager
{
    private List<Student> students = new List<Student>();

    public void AddStudent(Student student)
    {
        students.Add(student);
    }

    public void RemoveStudent(string name)
    {
        students.RemoveAll(s => s.Name == name);
    }

    public Student FindStudent(string name)
    {
        return students.FirstOrDefault(s => s.Name == name);
    }

    public void ShowAllStudents()
    {
        if (students.Count == 0)
            Console.WriteLine("No students in the list.");
        else
            students.ForEach(s => Console.WriteLine(s));
    }
}

