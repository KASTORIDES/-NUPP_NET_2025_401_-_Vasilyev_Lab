using System;

namespace StudentSystem.Common
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public Course(string title, int credits)
        {
            Id = Guid.NewGuid();
            Title = title;
            Credits = credits;
        }

        public override string ToString() => $"{Title} ({Credits} cr)";

        public static Course CreateNew()
        {
            var rnd = RandomProvider.GetThreadRandom();
            var titles = new[] { "Programming", "Algorithms", "Databases", "Operating Systems", "Networks" };
            string title = titles[rnd.Next(titles.Length)];
            int credits = rnd.Next(2, 7);
            return new Course(title, credits);
        }
    }
}
