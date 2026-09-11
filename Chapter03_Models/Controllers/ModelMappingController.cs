using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter03_Models.Controllers;

/// <summary>
/// Уроки 3.1 (Fluent API vs анотації), 3.6 (зіставлення таблиць і стовпців),
/// 3.7 (обов'язкові та необов'язкові властивості).
/// </summary>
[ApiController]
[Route("api/ch03/model-mapping")]
[Tags("Глава 3 — Створення моделей")]
public class ModelMappingController : ControllerBase
{
    private readonly ModelsContext _db;

    public ModelMappingController(ModelsContext db) => _db = db;

    /// <summary>
    /// 3.6 — як клас <see cref="Product"/> ліг на таблицю: схема, ім'я таблиці,
    /// імена та типи стовпців. Порівняй з анотаціями в <c>Product.cs</c> і
    /// з Fluent API в <c>ModelsContext.OnModelCreating</c>.
    /// </summary>
    [HttpGet("product")]
    public ActionResult<object> Product()
    {
        var entityType = _db.Model.FindEntityType(typeof(Product))!;
        return Ok(ModelInspection.Describe(entityType));
    }

    /// <summary>
    /// 3.7 — які властивості обов'язкові (<c>NOT NULL</c>), а які ні.
    /// EF визначає це насамперед за типом: <c>string</c> → обов'язковий,
    /// <c>string?</c> → необов'язковий (за увімкнених nullable reference types).
    /// </summary>
    [HttpGet("required-optional")]
    public ActionResult<object> RequiredOptional()
    {
        var result = new[] { typeof(Product), typeof(Customer) }
            .Select(clr => _db.Model.FindEntityType(clr)!)
            .Select(entityType => new
            {
                entity = entityType.ClrType.Name,
                properties = entityType.GetProperties().Select(p => new
                {
                    name = p.Name,
                    required = !p.IsNullable,
                    clrType = p.ClrType.Name
                })
            });

        return Ok(result);
    }

    /// <summary>SQL-скрипт створення всієї схеми глави 3.</summary>
    [HttpGet("create-script")]
    public ActionResult<object> CreateScript() =>
        Ok(new { sql = _db.Database.GenerateCreateScript() });
}
