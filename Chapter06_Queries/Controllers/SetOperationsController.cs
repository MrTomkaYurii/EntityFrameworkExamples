using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.5 "Операції з множинами: об'єднання, перетин, різниця".
/// Порівнюємо множини посад у двох компаніях.
/// </summary>
[ApiController]
[Route("api/ch06/set-operations")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class SetOperationsController : ControllerBase
{
    private readonly QueriesContext _db;

    public SetOperationsController(QueriesContext db) => _db = db;

    private IQueryable<string> PositionsIn(string company) =>
        _db.Users.Where(u => u.Company!.Name == company).Select(u => u.Position);

    /// <summary><c>Union</c> — об'єднання без дублікатів (SQL <c>UNION</c>).</summary>
    [HttpGet("union")]
    public ActionResult<object> Union() =>
        Ok(PositionsIn("Microsoft").Union(PositionsIn("Google")).ToSqlAndData());

    /// <summary><c>Concat</c> — об'єднання із дублікатами (SQL <c>UNION ALL</c>).</summary>
    [HttpGet("concat")]
    public ActionResult<object> Concat() =>
        Ok(PositionsIn("Microsoft").Concat(PositionsIn("Google")).ToSqlAndData());

    /// <summary><c>Intersect</c> — посади, що є в обох компаніях.</summary>
    [HttpGet("intersect")]
    public ActionResult<object> Intersect() =>
        Ok(PositionsIn("Microsoft").Intersect(PositionsIn("Google")).ToSqlAndData());

    /// <summary><c>Except</c> — посади, що є в першій компанії, але не в другій.</summary>
    [HttpGet("except")]
    public ActionResult<object> Except() =>
        Ok(PositionsIn("Spotify").Except(PositionsIn("Google")).ToSqlAndData());

    /// <summary><c>Distinct</c> — усі унікальні посади.</summary>
    [HttpGet("distinct")]
    public ActionResult<object> Distinct() =>
        Ok(_db.Users.Select(u => u.Position).Distinct().ToSqlAndData());
}
