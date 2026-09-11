using System.Diagnostics;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Уроки 6.8 "Виконання запитів" та 6.9 "IEnumerable та IQueryable".
/// </summary>
[ApiController]
[Route("api/ch06/query-execution")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class QueryExecutionController : ControllerBase
{
    private readonly QueriesContext _db;

    public QueryExecutionController(QueriesContext db) => _db = db;

    /// <summary>
    /// 6.8 — відкладене виконання. Побудова <c>IQueryable</c> не звертається до БД;
    /// запит виконується лише на <c>ToList</c> / <c>foreach</c> / <c>Count</c> тощо.
    /// </summary>
    [HttpGet("deferred")]
    public ActionResult<object> Deferred()
    {
        var query = _db.Users.Where(u => u.Age > 30);   // БД ще не чіпали

        var sql = query.ToQueryString();
        var firstRun = query.Count();                    // ← ось тут пішов запит
        var secondRun = query.ToList().Count;            // ← і тут ще раз

        return Ok(new
        {
            note = "Один і той самий IQueryable виконується щоразу заново.",
            sql,
            firstRun,
            secondRun
        });
    }

    /// <summary>
    /// 6.9 — доки запит типу <c>IQueryable</c>, умови транслюються у SQL і виконуються в БД.
    /// </summary>
    [HttpGet("iqueryable")]
    public ActionResult<object> Queryable()
    {
        // Where застосовується ДО матеріалізації → потрапляє у SQL WHERE.
        var query = _db.Users
            .Where(u => u.Salary > 5000)
            .Select(u => new { u.Name, u.Salary });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// 6.9 — після <c>AsEnumerable()</c> запит стає <c>IEnumerable</c>: подальші умови
    /// виконуються вже у пам'яті застосунку (client-side), не в БД.
    /// Порівняй SQL із попереднім ендпоінтом — тут у ньому немає фільтра по Salary.
    /// </summary>
    [HttpGet("ienumerable")]
    public ActionResult<object> Enumerable()
    {
        var sql = _db.Users.ToQueryString();   // SELECT усіх користувачів, без WHERE

        var stopwatch = Stopwatch.StartNew();
        var result = _db.Users
            .AsEnumerable()                     // ← межа: далі все в пам'яті
            .Where(u => u.Salary > 5000)
            .Select(u => new { u.Name, u.Salary })
            .ToList();
        stopwatch.Stop();

        return Ok(new
        {
            note = "Фільтр Salary > 5000 виконано в пам'яті — з БД приїхали ВСІ рядки.",
            sqlSentToDatabase = sql,
            result
        });
    }
}
