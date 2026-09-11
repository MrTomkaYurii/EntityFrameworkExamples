namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Бере участь одразу у двох зв'язках:
/// <list type="bullet">
///   <item>"багато до одного" з <see cref="Company"/> (необов'язковий, 4.8);</item>
///   <item>"один до одного" з <see cref="UserProfile"/> (4.7).</item>
/// </list>
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    // ── зв'язок із компанією (необов'язковий) ────────────────────────────────

    /// <summary><c>int?</c> → зв'язок необов'язковий, користувач може бути без компанії.</summary>
    public int? CompanyId { get; set; }

    public Company? Company { get; set; }

    // ── зв'язок із профілем (один до одного) ─────────────────────────────────

    public UserProfile? Profile { get; set; }
}
