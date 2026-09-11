using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.4 "З'єднання та групування таблиць".
/// </summary>
[ApiController]
[Route("api/ch06/join-grouping")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class JoinGroupingController : ControllerBase
{
    private readonly QueriesContext _db;

    public JoinGroupingController(QueriesContext db) => _db = db;

    /// <summary>
    /// Явний <c>Join</c> за ключами. Часто його можна замінити навігацією
    /// (<c>u.Company.Name</c>) — див. ендпоінт <c>/navigation-join</c>.
    /// </summary>
    [HttpGet("join")]
    public ActionResult<object> Join()
    {
        var query = _db.Users.Join(
            _db.Companies,
            user => user.CompanyId,
            company => company.Id,
            (user, company) => new { user.Name, company = company.Name, company.Country });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>Те саме через навігаційну властивість — коротше й читабельніше.</summary>
    [HttpGet("navigation-join")]
    public ActionResult<object> NavigationJoin()
    {
        var query = _db.Users.Select(u => new { u.Name, company = u.Company!.Name });
        return Ok(query.ToSqlAndData());
    }

    /// <summary><c>GroupBy</c> — групування з агрегатом (SQL <c>GROUP BY</c>).</summary>
    [HttpGet("group-by")]
    public ActionResult<object> GroupBy()
    {
        var query = _db.Users
            .GroupBy(u => u.Position)
            .Select(g => new
            {
                position = g.Key,
                count = g.Count(),
                averageSalary = g.Average(u => u.Salary)
            });

        return Ok(query.ToSqlAndData());
    }

    /// <summary><c>GroupJoin</c> — компанія + список її працівників у вигляді підколекції.</summary>
    [HttpGet("group-join")]
    public ActionResult<object> GroupJoin()
    {
        var query = _db.Companies.GroupJoin(
            _db.Users,
            company => company.Id,
            user => user.CompanyId,
            (company, users) => new { company = company.Name, employees = users.Select(u => u.Name) });

        return Ok(query.ToSqlAndData());
    }
}
