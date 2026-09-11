namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Одна зі сторін зв'язку "багато до багатьох" з <see cref="Course"/> (4.9).
/// Проміжну таблицю описує явна сутність <see cref="Enrollment"/>.
/// </summary>
public class Student
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Зручна навігація "напряму" до курсів (EF наповнює її через Enrollment).</summary>
    public List<Course> Courses { get; set; } = [];

    /// <summary>Навігація до рядків проміжної таблиці — потрібна, щоб дістатися до <c>EnrolledOn</c>.</summary>
    public List<Enrollment> Enrollments { get; set; } = [];
}
