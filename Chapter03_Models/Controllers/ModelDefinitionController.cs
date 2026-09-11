using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter03_Models.Controllers;

/// <summary>
/// Уроки 3.2–3.5: що робить клас сутністю, властивості (3.3),
/// конструктори (3.4) та поля як стан сутності (3.5).
/// </summary>
[ApiController]
[Route("api/ch03/model-definition")]
[Tags("Глава 3 — Створення моделей")]
public class ModelDefinitionController : ControllerBase
{
    private readonly ModelsContext _db;

    public ModelDefinitionController(ModelsContext db) => _db = db;

    /// <summary>
    /// Усі сутності, які EF Core "бачить" у цьому контексті, та їхні таблиці.
    /// Сюди потрапляє те, що оголошене як <c>DbSet&lt;T&gt;</c> або знайдене через навігації.
    /// </summary>
    [HttpGet("entities")]
    public ActionResult<object> Entities()
    {
        var entities = _db.Model.GetEntityTypes()
            .Select(e => new
            {
                clrType = e.ClrType.Name,
                table = e.GetSchema() is { } s ? $"{s}.{e.GetTableName()}" : e.GetTableName()
            });

        return Ok(entities);
    }

    /// <summary>
    /// 3.3 — властивості, які EF <b>не</b> зберігає: тут це <c>Product.Display</c> ([NotMapped]).
    /// </summary>
    [HttpGet("not-mapped")]
    public ActionResult<object> NotMapped()
    {
        var product = _db.Model.FindEntityType(typeof(Product))!;
        var mapped = product.GetProperties().Select(p => p.Name).ToHashSet();

        var allPublic = typeof(Product)
            .GetProperties()
            .Select(p => p.Name);

        return Ok(new
        {
            mapped,
            notMapped = allPublic.Where(name => !mapped.Contains(name))
        });
    }

    /// <summary>3.4 — сутність <see cref="Article"/> не має конструктора без параметрів.
    /// EF створює її через <c>Article(int id, string title)</c>, зіставляючи параметри з властивостями.</summary>
    [HttpGet("articles")]
    public async Task<ActionResult<object>> Articles()
    {
        // Кожен об'єкт у цьому списку EF створив, викликавши конструктор із параметрами.
        var articles = await _db.Articles
            .Select(a => new { a.Id, a.Title, a.ViewCount })
            .ToListAsync();

        return Ok(articles);
    }

    /// <summary>
    /// 3.5 — <c>Article.ViewCount</c> не має публічного сетера. Значення змінюється лише
    /// методом <c>RegisterView()</c>, а EF читає й пише його через приватне поле <c>_viewCount</c>.
    /// Виклич цей ендпоінт кілька разів — лічильник зберігається між запитами.
    /// </summary>
    [HttpPost("articles/{id:int}/views")]
    public async Task<ActionResult<object>> RegisterView(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article is null)
            return NotFound();

        article.RegisterView();
        await _db.SaveChangesAsync();   // EF згенерує UPDATE стовпця ViewCount

        return Ok(new { article.Id, article.Title, article.ViewCount });
    }
}
