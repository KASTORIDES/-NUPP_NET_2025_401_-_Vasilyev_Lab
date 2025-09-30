using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Student
{
    public string Name { get; set; }
    public string Group { get; set; }
    public double AverageGrade { get; set; }

    public Student(string name, string group, double averageGrade)
    {
        Name = name;
        Group = group;
        AverageGrade = averageGrade;
    }

    public override string ToString()
    {
        return $"{Name}, Group: {Group}, Average Grade: {AverageGrade}";
    }
}


