using System.Collections.Generic;
using System.Linq;

namespace StudentSystem.Common
{
    public static class StudentExtensions
    {
        // Метод розширення для підрахунку середнього з групи студентів
        public static double AverageGrade(this IEnumerable<Student> students)
        {
            var list = students.ToList();
            return list.Any() ? list.Average(s => s.AverageGrade) : 0.0;
        }
    }
}
