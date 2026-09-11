namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Залежна сторона зв'язку з <see cref="Blog"/>.
/// Демонструє зовнішній ключ і навігаційні властивості (4.1).
/// </summary>
public class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Views { get; set; }

    // ── зв'язок із блогом ────────────────────────────────────────────────────

    /// <summary>Зовнішній ключ. Тип <c>int</c> (не <c>int?</c>) → зв'язок обов'язковий.</summary>
    public int BlogId { get; set; }

    /// <summary>Навігаційна властивість-посилання на батьківський блог.</summary>
    public Blog? Blog { get; set; }
}
