using EfCoreExamples.Chapter08_Advanced.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced.Controllers;

/// <summary>
/// Урок 2.11 "Паралелізм (Concurrency)".
/// Демонструє роботу токена паралелізму (RowVersion / Timestamp),
/// виникнення конфлікту при одночасному редагуванні та способи його вирішення.
/// </summary>
[ApiController]
[Route("api/ch08/concurrency")]
[Tags("Глава 8 — Додаткові можливості")]
public class ConcurrencyController : ControllerBase
{
    private readonly AdvancedContext _db;
    private readonly IServiceProvider _serviceProvider;

    public ConcurrencyController(AdvancedContext db, IServiceProvider serviceProvider)
    {
        _db = db;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Отримання списку банківських рахунків разом зі значенням токена RowVersion у форматі HEX.
    /// </summary>
    [HttpGet("accounts")]
    public async Task<ActionResult<object>> GetAccounts()
    {
        var accounts = await _db.BankAccounts.AsNoTracking().ToListAsync();

        return Ok(accounts.Select(a => new
        {
            a.Id,
            a.AccountNumber,
            a.OwnerName,
            a.Balance,
            rowVersionHex = Convert.ToHexString(a.RowVersion)
        }));
    }

    /// <summary>
    /// Симуляція одночасного редагування одного рахунку двома користувачами (гонка даних).
    /// Перший користувач успішно зберігає зміни. Другий стикається з DbUpdateConcurrencyException.
    /// </summary>
    /// <param name="resolutionStrategy">Стратегія вирішення: "None" (показати помилку), "DatabaseWins" (залишити зміни першого), "ClientWins" (перетерти даними другого).</param>
    [HttpPost("simulate-conflict")]
    public async Task<ActionResult<object>> SimulateConflict([FromQuery] string resolutionStrategy = "None")
    {
        using var scope1 = _serviceProvider.CreateScope();
        using var scope2 = _serviceProvider.CreateScope();

        var db1 = scope1.ServiceProvider.GetRequiredService<AdvancedContext>();
        var db2 = scope2.ServiceProvider.GetRequiredService<AdvancedContext>();

        // Обидва користувачі одночасно зчитують один і той самий рахунок
        var accountUser1 = await db1.BankAccounts.FirstAsync(a => a.Id == 1);
        var accountUser2 = await db2.BankAccounts.FirstAsync(a => a.Id == 1);

        decimal initialBalance = accountUser1.Balance;
        string initialVersion = Convert.ToHexString(accountUser1.RowVersion);

        // 1. Користувач 1 поповнює баланс на 500 і першим зберігає зміни
        accountUser1.Balance += 500m;
        await db1.SaveChangesAsync();
        string user1Version = Convert.ToHexString(accountUser1.RowVersion);

        // 2. Користувач 2 (який зчитав рахунок ще до поповнення) намагається зняти 200
        accountUser2.Balance -= 200m;

        try
        {
            await db2.SaveChangesAsync();
            return Ok(new { message = "Конфлікту не сталося (несподівано)." });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Отримуємо запис, на якому стався конфлікт
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();

            var currentClientValue = (decimal)entry.CurrentValues["Balance"]!;
            var originalValue = (decimal)entry.OriginalValues["Balance"]!;
            var databaseValue = (decimal)databaseValues!["Balance"]!;

            string actionTaken;

            switch (resolutionStrategy.ToLowerInvariant())
            {
                case "databasewins":
                    // Стратегія "База виграє": скидаємо зміни другого користувача, приймаємо актуальні дані з БД
                    entry.OriginalValues.SetValues(databaseValues);
                    actionTaken = "DatabaseWins: зміни другого користувача відхилено, залишено баланс із бази.";
                    break;

                case "clientwins":
                    // Стратегія "Клієнт виграє": оновлюємо оригінальний RowVersion на новий із бази і повторно зберігаємо
                    entry.OriginalValues.SetValues(databaseValues);
                    await db2.SaveChangesAsync();
                    actionTaken = "ClientWins: дані першого користувача перетерто змінами другого користувача.";
                    break;

                default:
                    actionTaken = "Помилка зафіксована: операцію другого користувача перервано через DbUpdateConcurrencyException.";
                    break;
            }

            return Conflict(new
            {
                title = "Конфлікт оптимістичного паралелізму виявлено!",
                initialBalance,
                initialVersion,
                user1Action = "Користувач 1 додав +500",
                user1NewVersion = user1Version,
                user2Action = "Користувач 2 спробував зняти -200 зі старим RowVersion",
                conflictDetails = new
                {
                    clientValueProposed = currentClientValue,
                    databaseValueActual = databaseValue,
                    originalValueWhenRead = originalValue
                },
                resolutionStrategyApplied = resolutionStrategy,
                actionTaken
            });
        }
    }
}
