using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.10 "Фільтри запитів рівня моделі".
/// У <c>QueriesContext.OnModelCreating</c> задано
/// <c>modelBuilder.Entity&lt;User&gt;().HasQueryFilter(u =&gt; !u.IsDeleted)</c>.
/// </summary>
[ApiController]
[Route("api/ch06/query-filters")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class QueryFiltersController : ControllerBase
{
    private readonly QueriesContext _db;

    public QueryFiltersController(QueriesContext db) => _db = db;

    /// <summary>Звичайний запит — глобальний фільтр додає <c>WHERE [u].[IsDeleted] = 0</c>.</summary>
    [HttpGet("default")]
    public ActionResult<object> Default() =>
        Ok(_db.Users.Select(u => new { u.Name, u.IsDeleted }).ToSqlAndData());

    /// <summary><c>IgnoreQueryFilters()</c> вимикає фільтр — видно й "видалені" рядки.</summary>
    [HttpGet("ignored")]
    public ActionResult<object> Ignored() =>
        Ok(_db.Users.IgnoreQueryFilters()
            .Select(u => new { u.Name, u.IsDeleted })
            .ToSqlAndData());

    /// <summary>Скільки рядків приховує фільтр.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<object>> Summary() => Ok(new
    {
        visible = await _db.Users.CountAsync(),
        total = await _db.Users.IgnoreQueryFilters().CountAsync()
    });
}
