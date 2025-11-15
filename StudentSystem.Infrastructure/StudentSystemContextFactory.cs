using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentSystem.Infrastructure
{
	public class StudentSystemContextFactory : IDesignTimeDbContextFactory<StudentSystemContext>
	{
		public StudentSystemContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<StudentSystemContext>();

			// “акой же SQLite, как в консольном приложении
			string dbFile = Path.Combine(Environment.CurrentDirectory, "students.db");
			optionsBuilder.UseSqlite($"Data Source={dbFile}");

			return new StudentSystemContext(optionsBuilder.Options);
		}
	}
}