using EfCoreExamples.Chapter07_Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter07_Sql;

/// <summary>
/// Контекст глави 7 "SQL в Entity Framework Core".
/// Демонструє виконання сирих SQL-запитів, роботу зі збереженими функціями (скалярними та табличними)
/// та збереженими процедурами СУБД.
/// </summary>
public class SqlContext : DbContext
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CategorySummary> CategorySummaries => Set<CategorySummary>();

    /// <summary>
    /// C# прототип для скалярної SQL-функції <c>dbo.fn_CalculateDiscount</c>.
    /// Метод не має реалізації на C#, оскільки EF Core перехоплює виклик у LINQ-дереві
    /// та транслює його безпосередньо у виклик функції СУБД.
    /// </summary>
    public static decimal CalculateDiscount(decimal price, int percent) =>
        throw new NotSupportedException("Цей метод призначений для трансляції в SQL-запит і не може виконуватися в пам'яті.");

    /// <summary>
    /// C# метод для табличної SQL-функції (Table-Valued Function, TVF) <c>dbo.fn_GetProductsByPriceRange</c>.
    /// Повертає IQueryable, на який можна накладати додаткові LINQ-оператори (Where, OrderBy тощо).
    /// </summary>
    public IQueryable<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice) =>
        FromExpression(() => GetProductsByPriceRange(minPrice, maxPrice));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(p =>
        {
            p.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Customer>(c =>
        {
            c.Property(x => x.TotalSpent).HasPrecision(18, 2);
        });

        // ── 7.1 Keyless Entity Type для проєкції результатів сирого SQL ──────────
        // Якщо SQL-запит повертає довільну структуру (наприклад, результат GROUP BY),
        // налаштовуємо HasNoKey(). EF Core не відстежує такі сутності в ChangeTracker.
        modelBuilder.Entity<CategorySummary>(s =>
        {
            s.HasNoKey();
            s.Property(x => x.AveragePrice).HasPrecision(18, 2);
        });

        // ── 7.2 Мапінг користувацьких SQL-функцій ──────────────────────────────
        // 1. Скалярна функція: приймає скалярні аргументи, повертає значення (decimal).
        modelBuilder.HasDbFunction(typeof(SqlContext).GetMethod(nameof(CalculateDiscount))!)
            .HasName("fn_CalculateDiscount")
            .HasSchema("dbo");

        // 2. Таблична функція (TVF): повертає набір рядків, сумісний із сутністю Product.
        modelBuilder.HasDbFunction(typeof(SqlContext).GetMethod(nameof(GetProductsByPriceRange))!)
            .HasName("fn_GetProductsByPriceRange")
            .HasSchema("dbo");
    }
}
