using EfCoreExamples.Chapter08_Advanced.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced.Controllers;

/// <summary>
/// Урок 8.2 "Проєкція запитів на подання (Database Views)".
/// Демонструє читання даних зі створеного в СУБД подання через ToView() та HasNoKey().
/// </summary>
[ApiController]
[Route("api/ch08/views")]
[Tags("Глава 8 — Додаткові можливості")]
public class ViewsController : ControllerBase
{
    private readonly AdvancedContext _db;

    public ViewsController(AdvancedContext db) => _db = db;

    /// <summary>
    /// Читання всіх записів із подання dbo.V_AccountSummary.
    /// Погляньте на SQL: EF Core звертається безпосередньо до об'єкта [V_AccountSummary].
    /// </summary>
    [HttpGet("summary")]
    public ActionResult<object> GetSummary()
    {
        var query = _db.AccountSummaries;
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Фільтрація та сортування даних із подання за допомогою LINQ.
    /// EF Core автоматично додає WHERE та ORDER BY до запиту SELECT FROM [V_AccountSummary].
    /// </summary>
    [HttpGet("high-balance")]
    public ActionResult<object> GetHighBalance([FromQuery] decimal minBalance = 3000)
    {
        var query = _db.AccountSummaries
            .Where(s => s.TotalBalance >= minBalance)
            .OrderByDescending(s => s.TotalBalance);

        return Ok(query.ToSqlAndData());
    }
}
