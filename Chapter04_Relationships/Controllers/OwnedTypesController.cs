using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.10 "Власні типи" (owned types) — на прикладі <see cref="Order"/>,
/// який володіє <see cref="OrderAddress"/> (посилання) та колекцією <see cref="OrderItem"/>.
/// </summary>
[ApiController]
[Route("api/ch04/owned-types")]
[Tags("Глава 4 — Відношення між моделями")]
public class OwnedTypesController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public OwnedTypesController(RelationshipsContext db) => _db = db;

    /// <summary>
    /// Власні типи EF підвантажує автоматично (без <c>Include</c>) —
    /// вони частина сутності-власника.
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<object>> Orders()
    {
        // Власні типи (ShippingAddress, Lines) підтягуються самі — окремий Include не потрібен.
        // AsNoTracking() тут обов'язковий: EF не дає відстежувати власний тип
        // у проєкції без сутності-власника.
        var orders = await _db.Orders.AsNoTracking().ToListAsync();

        return Ok(orders);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<object>> Create(OrderInput input)
    {
        var order = new Order
        {
            Number = input.Number,
            ShippingAddress = new OrderAddress { City = input.City, Street = input.Street, Zip = input.Zip },
            Lines = input.Lines
                .Select(l => new OrderItem { Product = l.Product, Quantity = l.Quantity, Price = l.Price })
                .ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(new { order.Id, order.Number });
    }

    /// <summary>
    /// Де фізично зберігаються власні типи:
    /// посилання (<c>ShippingAddress</c>) — стовпцями в таблиці власника;
    /// колекція (<c>Lines</c>) — в окремій таблиці з прихованим зовнішнім ключем.
    /// </summary>
    [HttpGet("storage")]
    public ActionResult<object> Storage()
    {
        var order = _db.Model.FindEntityType(typeof(Order))!;

        var owned = order.GetNavigations()
            .Where(n => n.TargetEntityType.IsOwned())
            .Select(n => new
            {
                navigation = n.Name,
                targetType = n.TargetEntityType.ClrType.Name,
                table = n.TargetEntityType.GetTableName(),
                isCollection = n.IsCollection
            });

        return Ok(new
        {
            ownerTable = order.GetTableName(),
            owned
        });
    }
}

public record OrderLineInput(string Product, int Quantity, decimal Price);
public record OrderInput(string Number, string City, string Street, string Zip, List<OrderLineInput> Lines);
