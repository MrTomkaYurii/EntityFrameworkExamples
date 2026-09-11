namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Головна сторона ОБОВ'ЯЗКОВОГО зв'язку з <see cref="Project"/>, для якого
/// каскадне видалення вимкнене (<c>DeleteBehavior.Restrict</c>, урок 4.3):
/// відділ не можна видалити, поки в нього є проєкти.
/// </summary>
public class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Project> Projects { get; set; } = [];
}
