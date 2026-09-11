namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Явна проміжна сутність для зв'язку "багато до багатьох" <see cref="Student"/>—<see cref="Course"/>.
/// <para>
/// Якби додаткових даних (як <c>EnrolledOn</c>) не було, цю сутність можна було б
/// не створювати — EF згенерував би приховану проміжну таблицю сам.
/// </para>
/// </summary>
public class Enrollment
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    /// <summary>Додаткова властивість зв'язку — заради неї й потрібна явна сутність.</summary>
    public DateOnly EnrolledOn { get; set; }
}
