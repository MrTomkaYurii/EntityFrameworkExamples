using EfCoreExamples.Chapter04_Relationships.LazyLoading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.6 "Lazy loading" — <see cref="LazyLoadingContext"/> з увімкненим
/// <c>UseLazyLoadingProxies()</c> (див. <c>Program.cs</c>).
/// </summary>
[ApiController]
[Route("api/ch04/lazy-loading")]
[Tags("Глава 4 — Відношення між моделями")]
public class LazyLoadingController : ControllerBase
{
    private readonly LazyLoadingContext _db;

    public LazyLoadingController(LazyLoadingContext db) => _db = db;

    /// <summary>
    /// Завантажуємо лише команду. У момент звернення до <c>team.Players</c>
    /// EF непомітно виконує ОКРЕМИЙ запит у БД, щоб підтягнути гравців.
    /// Тобто на список із N команд буде 1 + N запитів (проблема "N+1").
    /// </summary>
    [HttpGet("teams")]
    public async Task<ActionResult<object>> Teams()
    {
        // Один запит — лише команди.
        var teams = await _db.Teams.ToListAsync();

        // А тут для КОЖНОЇ команди піде ще по одному запиту (лінива підвантажка).
        var result = teams.Select(t => new { t.Id, t.Name, playerCount = t.Players.Count });

        return Ok(new
        {
            note = "У логах SQL буде видно 1 запит на команди + окремий запит на гравців кожної команди.",
            teams = result
        });
    }

    /// <summary>
    /// Той самий результат, але через <c>Include</c> — один запит замість N+1.
    /// Порівняй кількість SQL-запитів у логах із попереднім ендпоінтом.
    /// </summary>
    [HttpGet("teams-eager")]
    public async Task<ActionResult<object>> TeamsEager()
    {
        var teams = await _db.Teams
            .Include(t => t.Players)
            .Select(t => new { t.Id, t.Name, playerCount = t.Players.Count })
            .ToListAsync();

        return Ok(new { note = "Один запит із JOIN.", teams });
    }
}
