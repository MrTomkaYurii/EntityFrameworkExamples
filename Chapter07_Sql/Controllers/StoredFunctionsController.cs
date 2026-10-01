using EfCoreExamples.Chapter07_Sql.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter07_Sql.Controllers;

/// <summary>
/// Урок 6.2 "Збережені функції (Stored / User-Defined Functions)".
/// Демонструє використання скалярних функцій всередині LINQ та табличних функцій (TVF).
/// </summary>
[ApiController]
[Route("api/ch07/stored-functions")]
[Tags("Глава 7 — SQL в EF Core")]
public class StoredFunctionsController : ControllerBase
{
    private readonly SqlContext _db;

    public StoredFunctionsController(SqlContext db) => _db = db;

    /// <summary>
    /// Виклик скалярної функції dbo.fn_CalculateDiscount всередині LINQ-запиту.
    /// EF Core підставляє виклик функції СУБД прямо у вираз SELECT або WHERE.
    /// </summary>
    [HttpGet("scalar-function")]
    public ActionResult<object> ScalarFunction([FromQuery] int discountPercent = 15)
    {
        var query = _db.Products
            .Select(p => new
            {
                p.Name,
                OriginalPrice = p.Price,
                DiscountPercent = discountPercent,
                DiscountedPrice = SqlContext.CalculateDiscount(p.Price, discountPercent)
            });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Виклик табличної функції (Table-Valued Function, TVF) dbo.fn_GetProductsByPriceRange.
    /// Функція повертає набір сутностей у вигляді IQueryable, на який можна накладати
    /// подальші умови Where, OrderBy тощо.
    /// </summary>
    [HttpGet("table-valued-function")]
    public ActionResult<object> TableValuedFunction(
        [FromQuery] decimal minPrice = 300,
        [FromQuery] decimal maxPrice = 1500)
    {
        var query = _db.GetProductsByPriceRange(minPrice, maxPrice)
            .Where(p => p.StockCount > 0)
            .OrderBy(p => p.Price);

        return Ok(query.ToSqlAndData());
    }
}
