using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.2 "Вибірка та фільтрація". Кожен ендпоінт повертає ще й згенерований SQL.
/// </summary>
[ApiController]
[Route("api/ch06/filtering")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class FilteringController : ControllerBase
{
    private readonly QueriesContext _db;

    public FilteringController(QueriesContext db) => _db = db;

    /// <summary>Усі користувачі (глобальний фільтр уже прибрав "видаленого").</summary>
    [HttpGet("all")]
    public ActionResult<object> All() =>
        Ok(_db.Users.Select(u => new { u.Id, u.Name, u.Age, u.Position }).ToSqlAndData());

    /// <summary><c>Where</c> — переклад умови у SQL <c>WHERE</c>.</summary>
    [HttpGet("where")]
    public ActionResult<object> Where([FromQuery] int minAge = 30, [FromQuery] string? position = null)
    {
        var query = _db.Users.AsQueryable();

        query = query.Where(u => u.Age >= minAge);
        if (!string.IsNullOrEmpty(position))
            query = query.Where(u => u.Position == position);

        return Ok(query.Select(u => new { u.Name, u.Age, u.Position }).ToSqlAndData());
    }

    /// <summary>
    /// <c>Find</c> проти <c>FirstOrDefault</c>: <c>Find</c> спершу шукає в пам'яті серед
    /// відстежуваних сутностей і лише потім іде в БД.
    /// </summary>
    [HttpGet("by-id/{id:int}")]
    public async Task<ActionResult<object>> ById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        return user is null ? NotFound() : Ok(new { user.Id, user.Name, user.Age });
    }

    /// <summary><c>First</c> / <c>Single</c>: <c>Single</c> додатково перевіряє, що рядок рівно один.</summary>
    [HttpGet("first-vs-single")]
    public async Task<ActionResult<object>> FirstVsSingle()
    {
        var first = await _db.Users.OrderBy(u => u.Id).FirstAsync(u => u.Position == "Manager");

        string singleResult;
        try
        {
            _ = await _db.Users.SingleAsync(u => u.Position == "Manager");
            singleResult = "OK — менеджер рівно один";
        }
        catch (InvalidOperationException)
        {
            singleResult = "виняток — менеджерів більше ніж один (Single цього не пробачає)";
        }

        return Ok(new { first = new { first.Name, first.Position }, singleResult });
    }

    /// <summary><c>Skip</c> / <c>Take</c> — посторінкова вибірка (SQL <c>OFFSET ... FETCH</c>).</summary>
    [HttpGet("paging")]
    public ActionResult<object> Paging([FromQuery] int page = 1, [FromQuery] int pageSize = 3)
    {
        var query = _db.Users
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new { u.Id, u.Name });

        return Ok(query.ToSqlAndData());
    }
}
