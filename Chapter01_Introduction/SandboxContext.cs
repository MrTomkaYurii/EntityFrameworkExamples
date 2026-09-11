using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction;

/// <summary>
/// "Пісочниця" для уроку 1.4 "Управління базою даних".
/// Окрема БД, яку контролер може вільно створювати, видаляти й перестворювати,
/// не зачіпаючи інші приклади глави.
/// </summary>
public class SandboxContext : DbContext
{
    public SandboxContext(DbContextOptions<SandboxContext> options)
        : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
}

/// <summary>Проста замітка — щоб було що зберігати в пісочниці.</summary>
public class Note
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
