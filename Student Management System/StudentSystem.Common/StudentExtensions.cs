using System.Collections.Generic;
using System.Linq;

namespace StudentSystem.Common
{
    // Метод розширення
    public static class StudentExtensions
    {
        public static double AverageGrade(this IEnumerable<Student> students)
        {
            return students.Any() ? students.Average(s => s.AverageGrade) : 0;
        }
    }
}
