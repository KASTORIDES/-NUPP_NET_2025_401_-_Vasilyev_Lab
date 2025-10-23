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

        public override string ToString()
        {
            return $"{Title} ({Credits} credits)";
        }
    }
}

