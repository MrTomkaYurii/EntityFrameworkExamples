using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє: зіставлення таблиць/стовпців (3.6), обмеження (3.11),
/// генерацію значень (3.10), а також комбінування анотацій даних і Fluent API (3.1).
/// <para>
/// Тут навмисно змішані два стилі налаштування:
/// <list type="bullet">
///   <item>анотації даних — прямо на властивостях цього класу;</item>
///   <item>Fluent API — у методі <c>OnModelCreating</c> контексту <c>ModelsContext</c>.</item>
/// </list>
/// Fluent API має пріоритет, якщо обидва способи налаштовують те саме.
/// </para>
/// </summary>
public class Product
{
    public int Id { get; set; }

    // Анотація: у БД стовпець зватиметься "Title", а не "Name".
    [Column("Title")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    // Точність задається через Fluent API: HasPrecision(18, 2).
    public decimal Price { get; set; }

    // Унікальний індекс налаштовується через Fluent API: HasIndex(...).IsUnique().
    public string Sku { get; set; } = string.Empty;

    // Значення підставляє БД (HasDefaultValueSql("SYSUTCDATETIME()")).
    // Тому у власному коді цю властивість не встановлюємо.
    public DateTime CreatedAt { get; set; }

    // Обчислюваний стовпець (HasComputedColumnSql). БД рахує його сама — лише читаємо.
    public decimal PriceWithVat { get; private set; }

    // [NotMapped]: властивість існує в C#, але стовпця в таблиці для неї немає (3.3).
    [NotMapped]
    public string Display => $"{Name} ({Price:C})";
}
