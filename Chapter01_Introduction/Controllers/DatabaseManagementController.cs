using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Урок 1.4 "Управління базою даних".
/// Методи <c>context.Database</c>: перевірка з'єднання, створення та видалення БД
/// без міграцій, перегляд SQL-скрипту схеми.
/// Усі досліди — над окремою "пісочницею" (<see cref="SandboxContext"/>),
/// тож інші приклади глави не постраждають.
/// </summary>
[ApiController]
[Route("api/ch01/database")]
[Tags("Глава 1 — Вступ до EF Core")]
public class DatabaseManagementController : ControllerBase
{
    private readonly SandboxContext _db;

    public DatabaseManagementController(SandboxContext db) => _db = db;

    /// <summary>Чи можемо ми фізично під'єднатися до БД?</summary>
    [HttpGet("can-connect")]
    public async Task<ActionResult<object>> CanConnect()
    {
        // CanConnect() не кидає виняток, а повертає true/false — зручно для healthcheck.
        var canConnect = await _db.Database.CanConnectAsync();
        return Ok(new { canConnect });
    }

    /// <summary>
    /// <c>EnsureCreated()</c> — створює БД і всю схему за моделлю, якщо БД ще немає.
    /// Повертає <c>true</c>, якщо БД було створено цим викликом; <c>false</c>, якщо вже існувала.
    /// Міграції при цьому НЕ застосовуються.
    /// </summary>
    [HttpPost("ensure-created")]
    public async Task<ActionResult<object>> EnsureCreated()
    {
        var created = await _db.Database.EnsureCreatedAsync();
        return Ok(new { created });
    }

    /// <summary>
    /// <c>EnsureDeleted()</c> — повністю видаляє БД. Повертає <c>true</c>, якщо було що видаляти.
    /// Після цього виклику пісочниця порожня — відновити її можна через <c>ensure-created</c>.
    /// </summary>
    [HttpPost("ensure-deleted")]
    public async Task<ActionResult<object>> EnsureDeleted()
    {
        var deleted = await _db.Database.EnsureDeletedAsync();
        return Ok(new { deleted, hint = "Виклич POST ensure-created, щоб створити БД знову." });
    }

    /// <summary>
    /// SQL-скрипт створення схеми, який EF згенерував би для поточної моделі.
    /// Корисно, щоб побачити, у які саме DDL-команди перетворюються класи.
    /// </summary>
    [HttpGet("create-script")]
    public ActionResult<object> CreateScript()
    {
        var sql = _db.Database.GenerateCreateScript();
        return Ok(new { sql });
    }

    /// <summary>Додати замітку — просто щоб переконатися, що створена БД працює.</summary>
    [HttpPost("notes")]
    public async Task<ActionResult<Note>> AddNote(NoteInput input)
    {
        var note = new Note { Text = input.Text };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        return note;
    }

    /// <summary>Список заміток у пісочниці.</summary>
    [HttpGet("notes")]
    public async Task<ActionResult<List<Note>>> GetNotes() =>
        await _db.Notes.OrderBy(n => n.Id).ToListAsync();
}

public record NoteInput(string Text);
