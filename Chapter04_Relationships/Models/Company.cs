namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Головна сторона НЕобов'язкового зв'язку "один до багатьох" із <see cref="User"/>:
/// у користувача <c>CompanyId</c> — nullable. При видаленні компанії зовнішній ключ
/// користувачів обнуляється (<c>DeleteBehavior.SetNull</c>, урок 4.3).
/// </summary>
public class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<User> Users { get; set; } = [];
}
