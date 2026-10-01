using EfCoreExamples.Chapter05_Inheritance.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter05_Inheritance.Controllers;

/// <summary>
/// Урок 4.1 "Підхід TPH — Table Per Hierarchy".
/// Усі класи ієрархії зберігаються в одній таблиці, а тип запису визначається дискримінатором.
/// </summary>
[ApiController]
[Route("api/ch05/tph")]
[Tags("Глава 5 — Успадкування")]
public class TphController : ControllerBase
{
    private readonly InheritanceContext _db;

    public TphController(InheritanceContext db) => _db = db;

    /// <summary>
    /// Поліморфний запит: повертає всіх користувачів (базових User, Employee, Manager).
    /// EF Core вибирає все з однієї таблиці [Users_TPH] за один простий SELECT.
    /// </summary>
    [HttpGet("all")]
    public ActionResult<object> GetAll()
    {
        return Ok(_db.UsersTph.ToSqlAndData());
    }

    /// <summary>
    /// Фільтрація за типом через OfType&lt;EmployeeTph&gt;().
    /// Повертає працівників та їхніх нащадків (Manager).
    /// Зверніть увагу на SQL: EF Core автоматично генерує WHERE [UserType] IN ('Employee', 'Manager').
    /// </summary>
    [HttpGet("employees")]
    public ActionResult<object> GetEmployees()
    {
        var query = _db.UsersTph.OfType<EmployeeTph>();
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Фільтрація лише менеджерів.
    /// У SQL: WHERE [UserType] = 'Manager'.
    /// </summary>
    [HttpGet("managers")]
    public ActionResult<object> GetManagers()
    {
        var query = _db.UsersTph.OfType<ManagerTph>();
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Створення нового менеджера. EF Core вставляє запис у таблицю [Users_TPH]
    /// і автоматично проставляє дискримінатор 'Manager'.
    /// </summary>
    [HttpPost("managers")]
    public async Task<ActionResult<object>> CreateManager([FromBody] CreateManagerRequest request)
    {
        var manager = new ManagerTph
        {
            Name = request.Name,
            Email = request.Email,
            Company = request.Company,
            Salary = request.Salary,
            Department = request.Department,
            AnnualBonus = request.AnnualBonus
        };

        _db.UsersTph.Add(manager);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetManagers), new { id = manager.Id }, manager);
    }
}

public record CreateManagerRequest(
    string Name,
    string Email,
    string Company,
    decimal Salary,
    string Department,
    decimal AnnualBonus);
