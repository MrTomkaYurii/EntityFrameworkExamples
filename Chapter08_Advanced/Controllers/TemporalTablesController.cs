using EfCoreExamples.Chapter08_Advanced.Models;
using EfCoreExamples.Chapter08_Advanced.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced.Controllers;

/// <summary>
/// Урок 8.3 "Зберігання історії змін та темпоральні таблиці (Temporal Tables)".
/// Демонструє системне версіонування в SQL Server через IsTemporal(),
/// відстеження ревізій та запити TemporalAll / TemporalAsOf.
/// </summary>
[ApiController]
[Route("api/ch08/temporal")]
[Tags("Глава 8 — Додаткові можливості")]
public class TemporalTablesController : ControllerBase
{
    private readonly AdvancedContext _db;

    public TemporalTablesController(AdvancedContext db) => _db = db;

    /// <summary>
    /// Отримання поточних актуальних версій документів.
    /// </summary>
    [HttpGet("documents")]
    public async Task<ActionResult<object>> GetDocuments()
    {
        var docs = await _db.Documents.AsNoTracking().ToListAsync();
        return Ok(docs);
    }

    /// <summary>
    /// Оновлення документа. SQL Server автоматично копіює попередню версію
    /// в історичну таблицю з точними часовими мітками початку й кінця дії версії.
    /// </summary>
    [HttpPost("documents/{id:int}/update")]
    public async Task<ActionResult<object>> UpdateDocument(int id, [FromBody] UpdateDocumentRequest request)
    {
        var doc = await _db.Documents.FindAsync(id);
        if (doc is null) return NotFound();

        doc.Title = request.Title;
        doc.Content = request.Content;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Документ оновлено. Попередня версія автоматично заархівована в історії змін.",
            current = doc
        });
    }

    /// <summary>
    /// Отримання повної історії всіх ревізій документа через TemporalAll().
    /// Дозволяє побачити, як змінювався документ із часом, разом із періодом валідності кожного стану.
    /// </summary>
    [HttpGet("documents/{id:int}/history")]
    public async Task<ActionResult<object>> GetDocumentHistory(int id)
    {
        var history = await _db.Documents
            .TemporalAll()
            .Where(d => d.Id == id)
            .OrderByDescending(d => EF.Property<DateTime>(d, "PeriodEnd"))
            .Select(d => new
            {
                d.Id,
                d.Title,
                d.Content,
                d.Author,
                validFromUtc = EF.Property<DateTime>(d, "PeriodStart"),
                validToUtc = EF.Property<DateTime>(d, "PeriodEnd")
            })
            .ToListAsync();

        return Ok(history);
    }

    /// <summary>
    /// Отримання стану документа у точний момент часу в минулому (Point-In-Time) через TemporalAsOf.
    /// </summary>
    [HttpGet("documents/{id:int}/as-of")]
    public async Task<ActionResult<object>> GetDocumentAsOf(int id, [FromQuery] DateTime utcTimestamp)
    {
        var doc = await _db.Documents
            .TemporalAsOf(utcTimestamp)
            .FirstOrDefaultAsync(d => d.Id == id);

        return doc is null
            ? NotFound(new { message = $"На момент часу {utcTimestamp:u} документ ще не існував." })
            : Ok(new { timestamp = utcTimestamp, document = doc });
    }

    /// <summary>Скидання даних глави 8 до початкового стану.</summary>
    [HttpPost("reset")]
    public ActionResult<object> Reset()
    {
        AdvancedSeeder.Reset(_db);
        return Ok(new { message = "Дані глави 8 повернуто до початкового стану." });
    }
}

public record UpdateDocumentRequest(string Title, string Content);
