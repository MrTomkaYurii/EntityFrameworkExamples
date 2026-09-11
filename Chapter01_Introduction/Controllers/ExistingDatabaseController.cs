using EfCoreExamples.Chapter01_Introduction.ScaffoldedLike;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Урок 1.3 "Підключення до наявної бази даних" (підхід Database First).
/// <para>
/// <see cref="ExistingDbContext"/> працює з тією самою фізичною БД, що й
/// <see cref="IntroductionContext"/>, але через власну модель <c>AppUser</c>,
/// написану так, ніби її згенерував <c>dotnet ef dbcontext scaffold</c>.
/// Точну команду scaffold дивись у коментарях до <see cref="ExistingDbContext"/>.
/// </para>
/// </summary>
[ApiController]
[Route("api/ch01/existing-database")]
[Tags("Глава 1 — Вступ до EF Core")]
public class ExistingDatabaseController : ControllerBase
{
    private readonly ExistingDbContext _db;

    public ExistingDatabaseController(ExistingDbContext db) => _db = db;

    /// <summary>Читаємо наявні дані через "згенерований" контекст.</summary>
    [HttpGet("users")]
    public async Task<ActionResult<object>> GetUsers()
    {
        var users = await _db.Users.AsNoTracking().ToListAsync();

        return Ok(new
        {
            note = "Ці рядки створив IntroductionContext, а читаємо ми їх через окремий " +
                   "ExistingDbContext — дві моделі над однією таблицею Users.",
            table = _db.Model.FindEntityType(typeof(AppUser))!.GetTableName(),
            users
        });
    }
}
