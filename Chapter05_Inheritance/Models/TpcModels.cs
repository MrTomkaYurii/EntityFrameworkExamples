namespace EfCoreExamples.Chapter05_Inheritance.Models;

/// <summary>
/// Базова сутність для стратегії TPC (Table Per Class - EF Core 7+).
/// Зазвичай є абстрактною. У базі даних для неї НЕ створюється таблиця!
/// </summary>
public abstract class DeviceTpc
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>
/// Конкретний нащадок у TPC. Зберігається у самостійній таблиці [Smartphones_TPC],
/// яка містить УСІ стовпці (як успадковані Model, Price, так і власні).
/// </summary>
public class SmartphoneTpc : DeviceTpc
{
    public string OperatingSystem { get; set; } = string.Empty;
}

/// <summary>
/// Інший конкретний нащадок у TPC. Зберігається у самостійній таблиці [Laptops_TPC].
/// Не має спільних таблиць зі SmartphoneTpc.
/// </summary>
public class LaptopTpc : DeviceTpc
{
    public int RamGigabytes { get; set; }
}
