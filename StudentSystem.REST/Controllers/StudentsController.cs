using Microsoft.AspNetCore.Mvc;
using StudentSystem.Common;
using StudentSystem.Infrastructure.Models;
using StudentSystem.REST.Models;

namespace StudentSystem.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
	private readonly ICrudServiceAsync<StudentModel> _students;

	public StudentsController(ICrudServiceAsync<StudentModel> students)
	{
		_students = students;
	}

	// GET: api/students
	[HttpGet]
	public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
	{
		var all = await _students.ReadAllAsync();
		var result = all.Select(MapToDto);
		return Ok(result);
	}

	// GET: api/students/{guid}
	[HttpGet("{id:guid}")]
	public async Task<ActionResult<StudentDto>> GetById(Guid id)
	{
		var student = await _students.ReadAsync(id);
		if (student == null)
			return NotFound();

		return Ok(MapToDto(student));
	}

	// POST: api/students
	[HttpPost]
	public async Task<ActionResult<StudentDto>> Create(StudentCreateDto dto)
	{
		var model = new StudentModel
		{
			Guid = Guid.NewGuid(),
			FullName = dto.FullName,
			Age = dto.Age,
			AverageGrade = dto.AverageGrade
		};

		var created = await _students.CreateAsync(model);
		if (!created)
			return BadRequest();

		await _students.SaveAsync();

		var result = MapToDto(model);
		return CreatedAtAction(nameof(GetById), new { id = model.Guid }, result);
	}

	// PUT: api/students/{guid}
	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(Guid id, StudentUpdateDto dto)
	{
		var existing = await _students.ReadAsync(id);
		if (existing == null)
			return NotFound();

		existing.FullName = dto.FullName;
		existing.Age = dto.Age;
		existing.AverageGrade = dto.AverageGrade;

		var updated = await _students.UpdateAsync(existing);
		if (!updated)
			return BadRequest();

		await _students.SaveAsync();
		return NoContent();
	}

	// DELETE: api/students/{guid}
	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var existing = await _students.ReadAsync(id);
		if (existing == null)
			return NotFound();

		var removed = await _students.RemoveAsync(existing);
		if (!removed)
			return BadRequest();

		await _students.SaveAsync();
		return NoContent();
	}

	private static StudentDto MapToDto(StudentModel s) =>
		new(s.Guid, s.FullName, s.Age, s.AverageGrade);
}