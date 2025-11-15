namespace StudentSystem.REST.Models;

public record StudentDto(
	Guid Guid,
	string FullName,
	int Age,
	double AverageGrade
);

public record StudentCreateDto(
	string FullName,
	int Age,
	double AverageGrade
);

public record StudentUpdateDto(
	string FullName,
	int Age,
	double AverageGrade
);