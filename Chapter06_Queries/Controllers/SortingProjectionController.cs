using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.3 "Сортування та проєкція з бази даних".
/// </summary>
[ApiController]
[Route("api/ch06/sorting-projection")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class SortingProjectionController : ControllerBase
{
    private readonly QueriesContext _db;

    public SortingProjectionController(QueriesContext db) => _db = db;

    /// <summary><c>OrderBy</c> + <c>ThenBy</c> → SQL <c>ORDER BY a, b</c>.</summary>
    [HttpGet("order-by")]
    public ActionResult<object> OrderBy()
    {
        var query = _db.Users
            .OrderBy(u => u.Position)
            .ThenByDescending(u => u.Salary)
            .Select(u => new { u.Position, u.Name, u.Salary });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Проєкція через <c>Select</c>: у SQL потрапляють лише потрібні стовпці,
    /// а не <c>SELECT *</c>. Тут — в анонімний тип.
    /// </summary>
    [HttpGet("projection-anonymous")]
    public ActionResult<object> ProjectionAnonymous()
    {
        var query = _db.Users.Select(u => new { u.Name, YearsToRetirement = 60 - u.Age });
        return Ok(query.ToSqlAndData());
    }

    /// <summary>Проєкція у власний record (DTO).</summary>
    [HttpGet("projection-dto")]
    public ActionResult<object> ProjectionDto()
    {
        var query = _db.Users.Select(u => new UserBrief(u.Name, u.Company!.Name, u.Salary));
        return Ok(query.ToSqlAndData());
    }

    /// <summary><c>SelectMany</c> — "розгортання" вкладеної колекції у плаский список.</summary>
    [HttpGet("select-many")]
    public ActionResult<object> SelectMany()
    {
        var query = _db.Companies
            .SelectMany(c => c.Users, (c, u) => new { company = c.Name, employee = u.Name });

        return Ok(query.ToSqlAndData());
    }
}

public record UserBrief(string Name, string Company, decimal Salary);
