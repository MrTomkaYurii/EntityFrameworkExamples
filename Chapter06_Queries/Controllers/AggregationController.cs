using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.6 "Агрегатні операції".
/// Усі ці методи виконуються на боці БД і повертають одне значення.
/// </summary>
[ApiController]
[Route("api/ch06/aggregation")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class AggregationController : ControllerBase
{
    private readonly QueriesContext _db;

    public AggregationController(QueriesContext db) => _db = db;

    /// <summary>Count / Sum / Min / Max / Average по зарплатах.</summary>
    [HttpGet("salary")]
    public async Task<ActionResult<object>> Salary() => Ok(new
    {
        count = await _db.Users.CountAsync(),
        total = await _db.Users.SumAsync(u => u.Salary),
        min = await _db.Users.MinAsync(u => u.Salary),
        max = await _db.Users.MaxAsync(u => u.Salary),
        average = await _db.Users.AverageAsync(u => u.Salary)
    });

    /// <summary><c>Any</c> / <c>All</c> — булеві агрегати (SQL <c>EXISTS</c> / <c>NOT EXISTS</c>).</summary>
    [HttpGet("any-all")]
    public async Task<ActionResult<object>> AnyAll() => Ok(new
    {
        anyManagerOver40 = await _db.Users.AnyAsync(u => u.Position == "Manager" && u.Age > 40),
        allSalariesPositive = await _db.Users.AllAsync(u => u.Salary > 0),
        anyProductOutOfStock = await _db.Products.AnyAsync(p => !p.InStock)
    });

    /// <summary>Агрегати з фільтром: середня зарплата розробників.</summary>
    [HttpGet("filtered")]
    public async Task<ActionResult<object>> Filtered()
    {
        var developers = _db.Users.Where(u => u.Position == "Developer");

        return Ok(new
        {
            developerCount = await developers.CountAsync(),
            developerAverageSalary = await developers.AverageAsync(u => u.Salary)
        });
    }
}
