using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter03_Models.Controllers;

/// <summary>
/// Уроки 3.8 "Налаштування ключів" та 3.9 "Налаштування індексів".
/// </summary>
[ApiController]
[Route("api/ch03/keys-and-indexes")]
[Tags("Глава 3 — Створення моделей")]
public class KeysAndIndexesController : ControllerBase
{
    private readonly ModelsContext _db;

    public KeysAndIndexesController(ModelsContext db) => _db = db;

    /// <summary>Первинні та альтернативні ключі всіх сутностей глави.</summary>
    [HttpGet("keys")]
    public ActionResult<object> Keys()
    {
        var result = _db.Model.GetEntityTypes().Select(e => new
        {
            entity = e.ClrType.Name,
            keys = e.GetKeys().Select(k => new
            {
                columns = k.Properties.Select(p => p.Name),
                kind = k.IsPrimaryKey() ? "primary" : "alternate"
            })
        });

        return Ok(result);
    }

    /// <summary>Індекси всіх сутностей: стовпці, унікальність, фільтр.</summary>
    [HttpGet("indexes")]
    public ActionResult<object> Indexes()
    {
        var result = _db.Model.GetEntityTypes()
            .Where(e => e.GetIndexes().Any())
            .Select(e => new
            {
                entity = e.ClrType.Name,
                indexes = e.GetIndexes().Select(i => new
                {
                    columns = i.Properties.Select(p => p.Name),
                    isUnique = i.IsUnique,
                    filter = i.GetFilter()
                })
            });

        return Ok(result);
    }

    /// <summary>
    /// 3.8 — спроба додати другого клієнта з тим самим Email порушує альтернативний ключ.
    /// Друге звернення до цього ендпоінта поверне помилку БД.
    /// </summary>
    [HttpPost("customers")]
    public async Task<ActionResult<object>> AddCustomer(CustomerInput input)
    {
        _db.Customers.Add(new Customer
        {
            FullName = input.FullName,
            Email = input.Email,
            Phone = input.Phone
        });

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Клієнта додано." });
        }
        catch (DbUpdateException ex)
        {
            // Порушення унікальності (альтернативний ключ або унікальний індекс).
            return Conflict(new { error = ex.GetBaseException().Message });
        }
    }

    /// <summary>
    /// 3.8 — складений первинний ключ (OrderId + ProductId).
    /// Повтор тієї самої пари ключів поверне помилку.
    /// </summary>
    [HttpPost("order-lines")]
    public async Task<ActionResult<object>> AddOrderLine(OrderLineInput input)
    {
        _db.OrderLines.Add(new OrderLine
        {
            OrderId = input.OrderId,
            ProductId = input.ProductId,
            Quantity = input.Quantity
        });

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Рядок замовлення додано." });
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new { error = ex.GetBaseException().Message });
        }
    }
}

public record CustomerInput(string FullName, string Email, string? Phone);
public record OrderLineInput(int OrderId, int ProductId, int Quantity);
