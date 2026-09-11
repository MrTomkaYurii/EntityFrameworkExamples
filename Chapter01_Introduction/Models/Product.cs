namespace EfCoreExamples.Chapter01_Introduction.Models;

/// <summary>
/// Сутність для демонстрації МІГРАЦІЙ (урок 1.8).
/// Історія змін схеми:
/// <list type="number">
///   <item>міграція <c>Initial</c> — таблиця з <c>Id</c>, <c>Name</c>, <c>Price</c>;</item>
///   <item>міграція <c>AddProductCreatedAt</c> — додано стовпець <c>CreatedAt</c>.</item>
/// </list>
/// Подивитись обидві міграції та згенерований ними SQL можна через
/// контролер <c>MigrationsController</c>.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>Додано другою міграцією — щоб побачити зміну схеми в дії.</summary>
    public DateTime CreatedAt { get; set; }
}
