using EfCoreExamples.Chapter07_Sql.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter07_Sql.Controllers;

/// <summary>
/// Урок 6.1 "Виконання SQL-запитів".
/// Показує використання FromSql, FromSqlRaw, комбінування SQL з LINQ,
/// скалярні SqlQuery та пряме виконання команд ExecuteSql.
/// </summary>
[ApiController]
[Route("api/ch07/raw-sql")]
[Tags("Глава 7 — SQL в EF Core")]
public class RawSqlController : ControllerBase
{
    private readonly SqlContext _db;

    public RawSqlController(SqlContext db) => _db = db;

    /// <summary>
    /// Безпечний параметризований SQL через інтерполяцію рядків (FromSql).
    /// EF Core автоматично перетворює інтерпольовані змінні на SQL-параметри (@p0, @p1),
    /// захищаючи від атак SQL Injection.
    /// </summary>
    [HttpGet("from-sql-interpolated")]
    public ActionResult<object> FromSqlInterpolated([FromQuery] decimal minPrice = 500)
    {
        var query = _db.Products.FromSql($"SELECT * FROM Products WHERE Price >= {minPrice}");
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Явний FromSqlRaw з параметрами за позицією.
    /// Використовується, коли текст запиту формується динамічно або з конфігурації.
    /// </summary>
    [HttpGet("from-sql-raw")]
    public ActionResult<object> FromSqlRaw([FromQuery] int maxStock = 15)
    {
        var query = _db.Products.FromSqlRaw("SELECT * FROM Products WHERE StockCount <= {0}", maxStock);
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Компонування LINQ поверх FromSql:
    /// EF Core бере сирий SELECT, загортає його в підзапит (subquery) і додає Where, OrderBy, Take!
    /// </summary>
    [HttpGet("compose-linq")]
    public ActionResult<object> ComposeLinq([FromQuery] string category = "Smartphones")
    {
        var query = _db.Products
            .FromSql($"SELECT * FROM Products")
            .Where(p => p.Category == category)
            .OrderByDescending(p => p.Price);

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Скалярні запити через Database.SqlQuery (EF Core 8+):
    /// Дозволяє вибирати окремі скалярні значення (назви, числа, дати) без потреби в повноцінній сутності.
    /// </summary>
    [HttpGet("scalar-query")]
    public async Task<ActionResult<object>> ScalarQuery([FromQuery] decimal minPrice = 800)
    {
        var names = await _db.Database
            .SqlQuery<string>($"SELECT Name FROM Products WHERE Price >= {minPrice}")
            .ToListAsync();

        return Ok(new { minPrice, productNames = names });
    }

    /// <summary>
    /// Проєкція агрегатних результатів у неключову сутність (Keyless Entity Type) CategorySummary.
    /// Дозволяє читати результати складних SQL-запитів із GROUP BY та JOIN.
    /// </summary>
    [HttpGet("keyless-group-by")]
    public ActionResult<object> KeylessGroupBy()
    {
        var query = _db.CategorySummaries.FromSql($"""
            SELECT Category, COUNT(*) AS ProductCount, AVG(Price) AS AveragePrice
            FROM Products
            GROUP BY Category
            """);

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Пряме виконання DML (UPDATE, DELETE, INSERT) без завантаження сутностей.
    /// Повертає кількість змінених рядків.
    /// </summary>
    [HttpPost("execute-sql")]
    public async Task<ActionResult<object>> ExecuteSql([FromQuery] string category = "Smartphones", [FromQuery] int bonusStock = 5)
    {
        int rowsAffected = await _db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Products SET StockCount = StockCount + {bonusStock} WHERE Category = {category}");

        return Ok(new { category, bonusStock, rowsAffected, message = $"Оновлено {rowsAffected} записів." });
    }
}
