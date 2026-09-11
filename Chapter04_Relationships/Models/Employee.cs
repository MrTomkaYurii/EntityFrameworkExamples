namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Ієрархічні дані через зв'язок сутності самої на себе (self-reference, 4.12).
/// Кожен працівник може мати керівника (теж <see cref="Employee"/>) і підлеглих.
/// </summary>
public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    // ── зв'язок сам на себе ──────────────────────────────────────────────────

    /// <summary>Зовнішній ключ на керівника. <c>null</c> — це вершина ієрархії (напр. CEO).</summary>
    public int? ManagerId { get; set; }

    public Employee? Manager { get; set; }

    public List<Employee> Reports { get; set; } = [];
}
