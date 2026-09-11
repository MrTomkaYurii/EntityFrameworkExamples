namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє конструктори сутностей (3.4) та поля як стан сутності (3.5).
/// <para>
/// EF Core вміє створювати сутність через конструктор із параметрами, імена
/// яких збігаються з властивостями (регістр не важливий). Значення для
/// навігацій та згенерованих ключів EF підставляє після виклику конструктора.
/// </para>
/// </summary>
public class Article
{
    // Приватне поле-лічильник. Окремої властивості з сетером назовні немає —
    // збільшити його можна лише методом RegisterView(). EF читає/пише його
    // безпосередньо (backing field) завдяки read-only властивості ViewCount.
    private int _viewCount;

    public Article(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id { get; private set; }

    public string Title { get; private set; }

    /// <summary>Лише для читання назовні. У БД — звичайний стовпець "ViewCount".</summary>
    public int ViewCount => _viewCount;

    public void RegisterView() => _viewCount++;
}
