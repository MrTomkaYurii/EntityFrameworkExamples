namespace EfCoreExamples.Chapter06_Queries.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Position { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public int CompanyId { get; set; }

    public Company? Company { get; set; }

    /// <summary>
    /// Прапорець "м'якого видалення". Глобальний фільтр запитів (6.10)
    /// автоматично відкидає рядки, де він <c>true</c>.
    /// </summary>
    public bool IsDeleted { get; set; }
}
