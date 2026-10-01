using EfCoreExamples.Chapter05_Inheritance.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter05_Inheritance.Controllers;

/// <summary>
/// Урок 4.2 "Підхід TPT — Table Per Type".
/// Кожен клас має власну фізичну таблицю. Запити до базового класу збирають дані через JOIN.
/// </summary>
[ApiController]
[Route("api/ch05/tpt")]
[Tags("Глава 5 — Успадкування")]
public class TptController : ControllerBase
{
    private readonly InheritanceContext _db;

    public TptController(InheritanceContext db) => _db = db;

    /// <summary>
    /// Поліморфний запит: повертає всі рахунки (базові, кредитні, депозитні).
    /// Погляньте на SQL: EF Core будує запит із LEFT JOIN для кожної таблиці-нащадка!
    /// </summary>
    [HttpGet("all")]
    public ActionResult<object> GetAll()
    {
        return Ok(_db.AccountsTpt.ToSqlAndData());
    }

    /// <summary>
    /// Запит лише кредитних рахунків.
    /// EF Core будує INNER JOIN між [BillingAccounts_TPT] та [CreditAccounts_TPT].
    /// </summary>
    [HttpGet("credits")]
    public ActionResult<object> GetCredits()
    {
        var query = _db.CreditAccountsTpt;
        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// Створення нового кредитного рахунку.
    /// EF Core автоматично створює транзакцію і виконує ДВА INSERT:
    /// 1. У базову таблицю [BillingAccounts_TPT] (генерується PK).
    /// 2. У похідну таблицю [CreditAccounts_TPT] (зі згенерованим PK як FK).
    /// </summary>
    [HttpPost("credit-account")]
    public async Task<ActionResult<object>> CreateCreditAccount([FromBody] CreateCreditAccountRequest request)
    {
        var account = new CreditAccountTpt
        {
            Owner = request.Owner,
            Balance = request.Balance,
            CreditLimit = request.CreditLimit
        };

        _db.CreditAccountsTpt.Add(account);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCredits), new { id = account.Id }, account);
    }
}

public record CreateCreditAccountRequest(string Owner, decimal Balance, decimal CreditLimit);
