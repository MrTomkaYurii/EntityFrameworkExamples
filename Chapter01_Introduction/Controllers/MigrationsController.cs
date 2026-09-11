using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Урок 1.8 "Управління схемою БД та міграції" (на прикладі <see cref="MigrationsDemoContext"/>).
/// <para>
/// У проєкті дві міграції: <c>Initial</c> і <c>AddProductCreatedAt</c>.
/// CLI-аналоги ендпоінтів:
/// <list type="bullet">
///   <item><c>dotnet ef migrations list</c> — список міграцій;</item>
///   <item><c>dotnet ef database update</c> — застосувати всі;</item>
///   <item><c>dotnet ef database update Initial</c> — відкотитися до <c>Initial</c>;</item>
///   <item><c>dotnet ef migrations script</c> — SQL-скрипт.</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/ch01/migrations")]
[Tags("Глава 1 — Вступ до EF Core")]
public class MigrationsController : ControllerBase
{
    private readonly MigrationsDemoContext _db;

    public MigrationsController(MigrationsDemoContext db) => _db = db;

    /// <summary>Усі міграції, які є у збірці (незалежно від стану БД).</summary>
    [HttpGet("all")]
    public ActionResult<IEnumerable<string>> All() =>
        Ok(_db.Database.GetMigrations());

    /// <summary>Міграції, вже застосовані до БД (рядки з таблиці __EFMigrationsHistory).</summary>
    [HttpGet("applied")]
    public async Task<ActionResult<IEnumerable<string>>> Applied() =>
        Ok(await _db.Database.GetAppliedMigrationsAsync());

    /// <summary>Міграції, які ще НЕ застосовані до БД.</summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<string>>> Pending() =>
        Ok(await _db.Database.GetPendingMigrationsAsync());

    /// <summary>Повний SQL-скрипт усіх міграцій (те саме, що <c>dotnet ef migrations script</c>).</summary>
    [HttpGet("script")]
    public ActionResult<object> Script()
    {
        // IMigrator — внутрішній сервіс EF Core, дістаємо його з контейнера контексту.
        var migrator = _db.GetService<IMigrator>();
        var sql = migrator.GenerateScript();
        return Ok(new { sql });
    }

    /// <summary>Застосувати всі невиконані міграції (аналог <c>database update</c>).</summary>
    [HttpPost("migrate")]
    public async Task<ActionResult<object>> Migrate()
    {
        var before = await _db.Database.GetAppliedMigrationsAsync();
        await _db.Database.MigrateAsync();
        var after = await _db.Database.GetAppliedMigrationsAsync();

        return Ok(new
        {
            appliedBefore = before,
            appliedAfter = after
        });
    }

    /// <summary>
    /// Відкотити БД до стану першої міграції. Після цього ендпоінт <c>pending</c>
    /// знову покаже <c>AddProductCreatedAt</c> — зручно, щоб продемонструвати повний цикл.
    /// </summary>
    [HttpPost("revert-to-initial")]
    public ActionResult<object> RevertToInitial()
    {
        var migrator = _db.GetService<IMigrator>();

        // Міграція до конкретної цільової точки: усе, що після "Initial", буде відкочено
        // через їхні методи Down().
        migrator.Migrate("Initial");

        return Ok(new
        {
            message = "БД відкочено до міграції Initial. Тепер виклич GET pending, потім POST migrate.",
            applied = _db.Database.GetAppliedMigrations()
        });
    }
}
