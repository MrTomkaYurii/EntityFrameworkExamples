namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє винесення налаштувань у окремий клас <c>IEntityTypeConfiguration&lt;Category&gt;</c> (3.12).
/// Початкові дані для цієї сутності теж задаються там (<c>HasData</c>).
/// </summary>
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
