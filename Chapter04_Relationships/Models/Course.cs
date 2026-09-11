namespace EfCoreExamples.Chapter04_Relationships.Models;

public class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<Student> Students { get; set; } = [];

    public List<Enrollment> Enrollments { get; set; } = [];
}
