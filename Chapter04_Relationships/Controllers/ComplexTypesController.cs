using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.11 "Комплексні типи" — <see cref="Customer"/> з властивістю-value object
/// <see cref="Address"/>, налаштованою через <c>ComplexProperty</c>.
/// </summary>
[ApiController]
[Route("api/ch04/complex-types")]
[Tags("Глава 4 — Відношення між моделями")]
public class ComplexTypesController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public ComplexTypesController(RelationshipsContext db) => _db = db;

    [HttpGet("customers")]
    public async Task<ActionResult<object>> Customers()
    {
        var customers = await _db.Customers
            .Select(c => new { c.Id, c.Name, c.Address })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpPost("customers")]
    public async Task<ActionResult<object>> Create(CustomerInput input)
    {
        var customer = new Customer
        {
            Name = input.Name,
            Address = new Address { City = input.City, Street = input.Street, Zip = input.Zip }
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return Ok(new { customer.Id, customer.Name });
    }

    /// <summary>
    /// Чим комплексний тип відрізняється від власного:
    /// <list type="bullet">
    ///   <item>немає прихованого ключа й окремої ідентичності — це чистий value object;</item>
    ///   <item>завжди в тій самій таблиці, не може бути в колекції як окрема таблиця;</item>
    ///   <item>один екземпляр можна присвоїти кільком сутностям.</item>
    /// </list>
    /// </summary>
    [HttpGet("complex-vs-owned")]
    public ActionResult<object> ComplexVsOwned()
    {
        var customer = _db.Model.FindEntityType(typeof(Customer))!;
        var complexProps = customer.GetComplexProperties()
            .Select(cp => new
            {
                name = cp.Name,
                type = cp.ComplexType.ClrType.Name,
                columns = cp.ComplexType.GetProperties().Select(p => p.GetColumnName())
            });

        return Ok(new
        {
            table = customer.GetTableName(),
            complexProperties = complexProps
        });
    }
}

public record CustomerInput(string Name, string City, string Street, string Zip);
