using EfCoreExamples.Chapter06_Queries.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.11 "Масове оновлення та видалення. ExecuteUpdate та ExecuteDelete".
/// <para>
/// На відміну від <c>SaveChanges()</c>, ці методи:
/// <list type="bullet">
///   <item>виконуються ОДРАЗУ, без завантаження сутностей у пам'ять;</item>
///   <item>генерують один <c>UPDATE</c> / <c>DELETE ... WHERE</c> для всіх рядків;</item>
///   <item>НЕ оновлюють трекер змін — уже завантажені об'єкти можуть "застаріти".</item>
/// </list>
/// Після дослідів виклич <c>POST /api/ch06/bulk-operations/reset</c>.
/// </para>
/// </summary>
[ApiController]
[Route("api/ch06/bulk-operations")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class BulkOperationsController : ControllerBase
{
    private readonly QueriesContext _db;

    public BulkOperationsController(QueriesContext db) => _db = db;

    /// <summary>Підняти зарплату всім на заданій посаді одним запитом.</summary>
    [HttpPost("raise-salary")]
    public async Task<ActionResult<object>> RaiseSalary([FromQuery] string position = "Developer", [FromQuery] decimal amount = 500)
    {
        var affected = await _db.Users
            .Where(u => u.Position == position)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.Salary, u => u.Salary + amount));

        return Ok(new { position, amount, rowsAffected = affected });
    }

    /// <summary>Видалити всі товари, яких немає в наявності.</summary>
    [HttpPost("delete-out-of-stock")]
    public async Task<ActionResult<object>> DeleteOutOfStock()
    {
        var affected = await _db.Products
            .Where(p => !p.InStock)
            .ExecuteDeleteAsync();

        return Ok(new { rowsAffected = affected });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        QueriesSeeder.Seed(_db);
        return Ok(new { message = "Дані глави 6 відновлено." });
    }
}
