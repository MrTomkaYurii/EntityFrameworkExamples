using System.Data;
using EfCoreExamples.Chapter07_Sql.Models;
using EfCoreExamples.Chapter07_Sql.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter07_Sql.Controllers;

/// <summary>
/// Урок 6.3 "Збережені процедури (Stored Procedures)".
/// Демонструє завантаження сутностей з процедури, використання вихідних параметрів (OUTPUT)
/// та виконання модифікацій через процедури.
/// </summary>
[ApiController]
[Route("api/ch07/stored-procedures")]
[Tags("Глава 7 — SQL в EF Core")]
public class StoredProceduresController : ControllerBase
{
    private readonly SqlContext _db;

    public StoredProceduresController(SqlContext db) => _db = db;

    /// <summary>
    /// Виклик збереженої процедури dbo.sp_GetProductsByCategory, що повертає набір сутностей.
    /// Результат вибірки автоматично матеріалізується в екземпляри Product.
    /// </summary>
    [HttpGet("by-category")]
    public async Task<ActionResult<object>> GetByCategory([FromQuery] string category = "Smartphones")
    {
        var categoryParam = new SqlParameter("@category", category);

        var products = await _db.Products
            .FromSqlRaw("EXEC dbo.sp_GetProductsByCategory @category", categoryParam)
            .ToListAsync();

        return Ok(new
        {
            category,
            count = products.Count,
            data = products
        });
    }

    /// <summary>
    /// Виклик збереженої процедури з вихідними параметрами (OUTPUT Parameters).
    /// Демонструє налаштування SqlParameter із Direction = ParameterDirection.Output.
    /// </summary>
    [HttpGet("output-parameters")]
    public async Task<ActionResult<object>> GetCustomerStats([FromQuery] decimal minSpent = 1000)
    {
        var minSpentParam = new SqlParameter("@minSpent", minSpent);

        var countParam = new SqlParameter
        {
            ParameterName = "@customerCount",
            SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.Output
        };

        var totalSpentParam = new SqlParameter
        {
            ParameterName = "@totalSpent",
            SqlDbType = SqlDbType.Decimal,
            Precision = 18,
            Scale = 2,
            Direction = ParameterDirection.Output
        };

        await _db.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_GetCustomerStats @minSpent, @customerCount OUTPUT, @totalSpent OUTPUT",
            minSpentParam, countParam, totalSpentParam);

        int count = countParam.Value is DBNull ? 0 : (int)countParam.Value;
        decimal total = totalSpentParam.Value is DBNull ? 0m : (decimal)totalSpentParam.Value;

        return Ok(new
        {
            minSpent,
            customerCount = count,
            totalSpent = total,
            message = $"Знайдено {count} клієнтів із витратами від {minSpent:C}, загальна сума: {total:C}."
        });
    }

    /// <summary>
    /// Виконання збереженої процедури для оновлення цін (DML).
    /// </summary>
    [HttpPost("increase-prices")]
    public async Task<ActionResult<object>> IncreaseCategoryPrices(
        [FromQuery] string category = "Laptops",
        [FromQuery] decimal percent = 5)
    {
        var categoryParam = new SqlParameter("@category", category);
        var percentParam = new SqlParameter("@percent", percent);

        int rowsAffected = await _db.Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_IncreaseCategoryPrices @category, @percent",
            categoryParam, percentParam);

        return Ok(new
        {
            category,
            percent,
            rowsAffected,
            message = $"Ціни для категорії '{category}' успішно піднято на {percent}%."
        });
    }

    /// <summary>Скидання даних глави 7 до початкового стану.</summary>
    [HttpPost("reset")]
    public ActionResult<object> Reset()
    {
        SqlSeeder.Reset(_db);
        return Ok(new { message = "Дані глави 7 повернуто до початкового стану." });
    }
}
