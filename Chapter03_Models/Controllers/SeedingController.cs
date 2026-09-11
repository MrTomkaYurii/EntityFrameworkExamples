using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;   // GetService<T>()
using Microsoft.EntityFrameworkCore.Metadata;         // IDesignTimeModel

namespace EfCoreExamples.Chapter03_Models.Controllers;

/// <summary>
/// Уроки 3.12 "Конфігурація моделей" (IEntityTypeConfiguration)
/// та 3.13 "Ініціалізація бази даних початковими даними" (HasData).
/// </summary>
[ApiController]
[Route("api/ch03/seeding")]
[Tags("Глава 3 — Створення моделей")]
public class SeedingController : ControllerBase
{
    private readonly ModelsContext _db;

    public SeedingController(ModelsContext db) => _db = db;

    /// <summary>
    /// 3.13 — налаштування, задані через <c>HasData</c> у <c>ModelsContext.OnModelCreating</c>.
    /// Ці рядки EF вставляє при створенні БД (або окремою міграцією).
    /// </summary>
    [HttpGet("settings")]
    public async Task<ActionResult<List<AppSetting>>> Settings() =>
        await _db.Settings.OrderBy(s => s.Key).ToListAsync();

    /// <summary>
    /// 3.12 + 3.13 — категорії. І налаштування, і початкові дані для них винесені
    /// в окремий клас <c>CategoryConfiguration</c>.
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<Category>>> Categories() =>
        await _db.Categories.OrderBy(c => c.Id).ToListAsync();

    /// <summary>
    /// Показує, які саме рядки EF вважає "початковими даними" (seed data)
    /// для кожної сутності — це і є вміст усіх викликів <c>HasData</c>.
    /// </summary>
    [HttpGet("seed-metadata")]
    public ActionResult<object> SeedMetadata()
    {
        // Початкові дані зберігаються лише в "design-time" моделі, а не в тій
        // оптимізованій для рантайму, що доступна через _db.Model.
        var model = _db.GetService<IDesignTimeModel>().Model;

        var result = model.GetEntityTypes()
            .Select(e => new
            {
                entity = e.ClrType.Name,
                seedRows = e.GetSeedData().ToList()
            })
            .Where(x => x.seedRows.Count > 0);

        return Ok(result);
    }
}
