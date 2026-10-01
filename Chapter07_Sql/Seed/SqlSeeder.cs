using EfCoreExamples.Chapter07_Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter07_Sql.Seed;

/// <summary>
/// Наповнює базу даних глави 7 початковими записами та створює в базі даних
/// збережені функції (UDF) і збережені процедури (Stored Procedures).
/// </summary>
public static class SqlSeeder
{
    public static void Seed(SqlContext db)
    {
        // ── 1. Створення збережених функцій СУБД ──────────────────────────────
        // Скалярна функція: обчислення ціни зі знижкою
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER FUNCTION dbo.fn_CalculateDiscount(@price decimal(18,2), @percent int)
            RETURNS decimal(18,2)
            AS
            BEGIN
                RETURN @price - (@price * @percent / 100.0)
            END
            """);

        // Таблична функція (TVF): вибірка товарів у діапазоні цін
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER FUNCTION dbo.fn_GetProductsByPriceRange(@minPrice decimal(18,2), @maxPrice decimal(18,2))
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT Id, Name, Category, Price, StockCount
                FROM Products
                WHERE Price >= @minPrice AND Price <= @maxPrice
            )
            """);

        // ── 2. Створення збережених процедур ─────────────────────────────────
        // Процедура 1: повернення сутностей за категорією
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER PROCEDURE dbo.sp_GetProductsByCategory
                @category nvarchar(100)
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT Id, Name, Category, Price, StockCount
                FROM Products
                WHERE Category = @category;
            END
            """);

        // Процедура 2: з вихідними параметрами (OUTPUT)
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER PROCEDURE dbo.sp_GetCustomerStats
                @minSpent decimal(18,2),
                @customerCount int OUTPUT,
                @totalSpent decimal(18,2) OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT 
                    @customerCount = COUNT(*),
                    @totalSpent = ISNULL(SUM(TotalSpent), 0)
                FROM Customers
                WHERE TotalSpent >= @minSpent;
            END
            """);

        // Процедура 3: оновлення даних (DML)
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER PROCEDURE dbo.sp_IncreaseCategoryPrices
                @category nvarchar(100),
                @percent decimal(5,2)
            AS
            BEGIN
                SET NOCOUNT ON;
                UPDATE Products
                SET Price = Price * (1.0 + @percent / 100.0)
                WHERE Category = @category;
            END
            """);

        // ── 3. Початкові дані ─────────────────────────────────────────────────
        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product { Name = "iPhone 16", Category = "Smartphones", Price = 999m, StockCount = 15 },
                new Product { Name = "Galaxy S24", Category = "Smartphones", Price = 849m, StockCount = 20 },
                new Product { Name = "MacBook Pro 14", Category = "Laptops", Price = 1999m, StockCount = 8 },
                new Product { Name = "ThinkPad P1", Category = "Laptops", Price = 2199m, StockCount = 5 },
                new Product { Name = "AirPods Pro", Category = "Audio", Price = 249m, StockCount = 40 },
                new Product { Name = "Sony WH-1000XM5", Category = "Audio", Price = 379m, StockCount = 12 });
        }

        if (!db.Customers.Any())
        {
            db.Customers.AddRange(
                new Customer { FullName = "Олександр Коваленко", Email = "olexandr@example.com", TotalSpent = 1250m },
                new Customer { FullName = "Марія Шевченко", Email = "maria@example.com", TotalSpent = 3400m },
                new Customer { FullName = "Іван Бондаренко", Email = "ivan@example.com", TotalSpent = 450m },
                new Customer { FullName = "Оксана Мельник", Email = "oksana@example.com", TotalSpent = 2100m });
        }

        db.SaveChanges();
    }

    public static void Reset(SqlContext db)
    {
        db.Products.RemoveRange(db.Products);
        db.Customers.RemoveRange(db.Customers);
        db.SaveChanges();

        Seed(db);
    }
}
