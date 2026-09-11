using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.9 "Відношення багато до багатьох" — <see cref="Student"/> ↔ <see cref="Course"/>
/// через явну проміжну сутність <see cref="Enrollment"/>.
/// </summary>
[ApiController]
[Route("api/ch04/many-to-many")]
[Tags("Глава 4 — Відношення між моделями")]
public class ManyToManyController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public ManyToManyController(RelationshipsContext db) => _db = db;

    /// <summary>Студенти з їхніми курсами (навігація "напряму", повз проміжну сутність).</summary>
    [HttpGet("students")]
    public async Task<ActionResult<object>> Students()
    {
        var students = await _db.Students
            .Select(s => new { s.Id, s.Name, courses = s.Courses.Select(c => c.Title) })
            .ToListAsync();

        return Ok(students);
    }

    /// <summary>Курси з їхніми студентами.</summary>
    [HttpGet("courses")]
    public async Task<ActionResult<object>> Courses()
    {
        var courses = await _db.Courses
            .Select(c => new { c.Id, c.Title, students = c.Students.Select(s => s.Name) })
            .ToListAsync();

        return Ok(courses);
    }

    /// <summary>Вміст проміжної таблиці — тут видно додаткову властивість <c>EnrolledOn</c>.</summary>
    [HttpGet("enrollments")]
    public async Task<ActionResult<object>> Enrollments()
    {
        var rows = await _db.Set<Enrollment>()
            .Select(e => new { e.StudentId, student = e.Student!.Name, e.CourseId, course = e.Course!.Title, e.EnrolledOn })
            .ToListAsync();

        return Ok(rows);
    }

    /// <summary>Записати студента на курс — просто додаємо курс у навігацію <c>Student.Courses</c>.</summary>
    [HttpPost("enroll")]
    public async Task<ActionResult<object>> Enroll(EnrollInput input)
    {
        var student = await _db.Students
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Id == input.StudentId);
        var course = await _db.Courses.FindAsync(input.CourseId);

        if (student is null || course is null)
            return NotFound();

        if (student.Courses.Any(c => c.Id == course.Id))
            return Ok(new { message = "Студент уже записаний на цей курс." });

        student.Courses.Add(course);   // EF сам вставить рядок у проміжну таблицю
        await _db.SaveChangesAsync();

        return Ok(new { message = $"{student.Name} записаний на курс \"{course.Title}\"." });
    }
}

public record EnrollInput(int StudentId, int CourseId);
