using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries.Controllers;

/// <summary>
/// Урок 6.7 "Відстеження об'єктів та AsNoTracking".
/// </summary>
[ApiController]
[Route("api/ch06/tracking")]
[Tags("Глава 6 — Запити та LINQ to Entities")]
public class TrackingController : ControllerBase
{
    private readonly QueriesContext _db;

    public TrackingController(QueriesContext db) => _db = db;

    /// <summary>
    /// Звичайний запит ВІДСТЕЖУЄ сутності: контекст запам'ятовує їх і бачить зміни.
    /// Тому редагування + <c>SaveChanges()</c> згенерує <c>UPDATE</c>.
    /// </summary>
    [HttpGet("tracking")]
    public async Task<ActionResult<object>> Tracking()
    {
        var user = await _db.Users.FirstAsync();
        user.Salary += 1;   // зміна в пам'яті

        var stateBeforeSave = _db.Entry(user).State.ToString();  // Modified
        _db.ChangeTracker.Clear();                               // відкотити, нічого не зберігаємо

        return Ok(new
        {
            trackedEntitiesCount = 0,
            note = "Запит без AsNoTracking відстежує сутності — зміни підхопить SaveChanges().",
            stateAfterEditWas = stateBeforeSave
        });
    }

    /// <summary>
    /// <c>AsNoTracking</c> — сутності НЕ потрапляють у трекер. Швидше й економніше
    /// для запитів "тільки читання", але зміни в таких об'єктах EF не побачить.
    /// </summary>
    [HttpGet("no-tracking")]
    public async Task<ActionResult<object>> NoTracking()
    {
        var users = await _db.Users.AsNoTracking().ToListAsync();

        return Ok(new
        {
            trackedEntitiesCount = _db.ChangeTracker.Entries().Count(),  // 0
            loaded = users.Count
        });
    }

    /// <summary>
    /// Ідентичність об'єктів: у відстежуваному запиті один рядок БД = один об'єкт у пам'яті.
    /// </summary>
    [HttpGet("identity-resolution")]
    public async Task<ActionResult<object>> IdentityResolution()
    {
        var a = await _db.Users.FirstAsync(u => u.Id == 1);
        var b = await _db.Users.FirstAsync(u => u.Id == 1);

        return Ok(new
        {
            sameInstanceWhenTracking = ReferenceEquals(a, b),   // true
            trackedEntitiesCount = _db.ChangeTracker.Entries().Count()
        });
    }
}
