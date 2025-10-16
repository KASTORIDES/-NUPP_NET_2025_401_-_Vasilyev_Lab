using System;
using System.Threading;

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Group { get; set; }
    public double AverageGrade { get; set; }

    public Student() { }

    public Student(string name, string group, double averageGrade)
    {
        Id = Guid.NewGuid();
        Name = name;
        Group = group;
        AverageGrade = averageGrade;
    }

    public override string ToString()
    {
        return $"{Name}, Group: {Group}, Average Grade: {AverageGrade:F2} (Id: {Id})";
    }

    private static readonly ThreadLocal<Random> _rnd = new(() =>
        new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));

    public static Student CreateNew()
    {
        var r = _rnd.Value;
        string[] groups = { "A1", "B2", "C3", "D4", "E5" };
        var name = $"Student_{r.Next(1000, 9999)}";
        var group = groups[r.Next(groups.Length)];
        var grade = Math.Round(r.NextDouble() * 5.0, 2);
        return new Student(name, group, grade);
    }
}