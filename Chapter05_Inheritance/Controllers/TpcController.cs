using EfCoreExamples.Chapter05_Inheritance.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter05_Inheritance.Controllers;

/// <summary>
/// Урок 4.3 "Підхід TPC — Table Per Class" (EF Core 7+).
/// Кожен конкретний клас зберігається у своїй окремій таблиці без спільних таблиць.
/// Поліморфні запити виконуються через UNION ALL.
/// </summary>
[ApiController]
[Route("api/ch05/tpc")]
[Tags("Глава 5 — Успадкування")]
public class TpcController : ControllerBase
{
    private readonly InheritanceContext _db;

    public TpcController(InheritanceContext db) => _db = db;

    /// <summary>
    /// Поліморфний запит до базового абстрактного класу DeviceTpc.
    /// Зверніть увагу на SQL: для базового класу немає таблиці, тому EF Core об'єднує
    /// таблиці всіх конкретних класів за допомогою UNION ALL!
    /// </summary>
    [HttpGet("all")]
    public ActionResult<object> GetAll()
    {
        return Ok(_db.DevicesTpc.ToSqlAndData());
    }

    /// <summary>
    /// Запит конкретного типу SmartphoneTpc.
    /// Працює максимально швидко: читає лише таблицю [Smartphones_TPC] без будь-яких JOIN чи UNION!
    /// </summary>
    [HttpGet("smartphones")]
    public ActionResult<object> GetSmartphones()
    {
        var query = _db.SmartphonesTpc;
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Створення нового смартфона.
    /// Виконується один чистий INSERT у таблицю [Smartphones_TPC].
    /// Значення Id береться зі спільної послідовності SQL Server (SEQUENCE),
    /// що гарантує унікальність ключів між усіма таблицями ієрархії.
    /// </summary>
    [HttpPost("smartphones")]
    public async Task<ActionResult<object>> CreateSmartphone([FromBody] CreateSmartphoneRequest request)
    {
        var phone = new SmartphoneTpc
        {
            Model = request.Model,
            Price = request.Price,
            OperatingSystem = request.OperatingSystem
        };

        _db.SmartphonesTpc.Add(phone);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSmartphones), new { id = phone.Id }, phone);
    }
}

public record CreateSmartphoneRequest(string Model, decimal Price, string OperatingSystem);
