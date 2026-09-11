namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Залежна сторона зв'язку "один до одного" з <see cref="User"/> (4.7).
/// EF визначає залежну сторону за наявністю зовнішнього ключа <c>UserId</c>.
/// </summary>
public class UserProfile
{
    public int Id { get; set; }

    public string Bio { get; set; } = string.Empty;

    public string? Website { get; set; }

    // ── зв'язок із користувачем ──────────────────────────────────────────────

    /// <summary>Зовнішній ключ. Унікальний індекс на нього робить зв'язок саме "один до одного".</summary>
    public int UserId { get; set; }

    public User? User { get; set; }
}
