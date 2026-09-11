namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Головна сторона зв'язку "один до багатьох" (4.8) з <see cref="Post"/>.
/// Зв'язок обов'язковий (у Post є не-nullable <c>BlogId</c>), тому за замовчуванням
/// діє каскадне видалення: видаляємо блог — зникають і його дописи (4.3).
/// </summary>
public class Blog
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Навігаційна властивість-колекція: "дописи цього блогу".</summary>
    public List<Post> Posts { get; set; } = [];
}
