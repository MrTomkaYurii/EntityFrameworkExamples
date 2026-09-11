using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter03_Models.Controllers;

/// <summary>
/// Уроки 3.10 "Генерація значень властивостей і стовпців"
/// та 3.11 "Обмеження властивостей".
/// </summary>
[ApiController]
[Route("api/ch03/value-generation")]
[Tags("Глава 3 — Створення моделей")]
public class ValueGenerationController : ControllerBase
{
    private readonly ModelsContext _db;

    public ValueGenerationController(ModelsContext db) => _db = db;

    /// <summary>
    /// 3.10 — створюємо товар, задаючи лише Name, Price та Sku.
    /// БД сама заповнить: <c>Id</c> (IDENTITY), <c>CreatedAt</c> (DEFAULT SYSUTCDATETIME()),
    /// <c>PriceWithVat</c> (обчислюваний стовпець). Дивись ці значення у відповіді.
    /// </summary>
    [HttpPost("products")]
    public async Task<ActionResult<object>> Create(ProductInput input)
    {
        var product = new Product
        {
            Name = input.Name,
            Price = input.Price,
            Sku = input.Sku
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Після SaveChanges EF уже підтягнув згенеровані БД значення назад в об'єкт.
        return Ok(new
        {
            product.Id,
            product.Name,
            product.Price,
            generatedByDatabase = new { product.CreatedAt, product.PriceWithVat }
        });
    }

    [HttpGet("products/{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var p = await _db.Products.FindAsync(id);
        return p is null
            ? NotFound()
            : Ok(new { p.Id, p.Name, p.Price, p.CreatedAt, p.PriceWithVat });
    }

    /// <summary>
    /// 3.11 — CHECK-обмеження "[Price] >= 0" не пропустить від'ємну ціну.
    /// EF успішно згенерує INSERT, але БД його відхилить.
    /// </summary>
    [HttpPost("products/invalid-price")]
    public async Task<ActionResult<object>> InvalidPrice()
    {
        _db.Products.Add(new Product { Name = "Broken", Price = -10m, Sku = $"BAD-{Guid.NewGuid():N}" });

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Несподівано: БД прийняла від'ємну ціну." });
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new
            {
                note = "Спрацювало обмеження CK_Products_Price_NonNegative на рівні БД.",
                error = ex.GetBaseException().Message
            });
        }
    }

    /// <summary>Метадані генерації значень для стовпців <see cref="Product"/>.</summary>
    [HttpGet("metadata")]
    public ActionResult<object> Metadata()
    {
        var entityType = _db.Model.FindEntityType(typeof(Product))!;

        return Ok(entityType.GetProperties().Select(p => new
        {
            property = p.Name,
            valueGenerated = p.ValueGenerated.ToString(),  // Never / OnAdd / OnAddOrUpdate
            defaultValueSql = p.GetDefaultValueSql(),
            computedColumnSql = p.GetComputedColumnSql()
        }));
    }
}

public record ProductInput(string Name, decimal Price, string Sku);
