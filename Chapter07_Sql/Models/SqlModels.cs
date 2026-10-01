namespace EfCoreExamples.Chapter07_Sql.Models;

/// <summary>
/// Сутність товару для прикладів виконання SQL-запитів, функцій та процедур.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockCount { get; set; }
}

/// <summary>
/// Сутність клієнта для демонстрації виклику процедур з вихідними параметрами (OUTPUT).
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
}

/// <summary>
/// Неключова сутність (Keyless Entity Type) для проєкції результатів агрегатних SQL-запитів
/// (GROUP BY, JOIN, вибірки без первинного ключа).
/// </summary>
public class CategorySummary
{
    public string Category { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal AveragePrice { get; set; }
}
