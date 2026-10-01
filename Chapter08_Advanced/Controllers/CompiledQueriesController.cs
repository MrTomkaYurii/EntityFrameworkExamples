using System.Diagnostics;
using EfCoreExamples.Chapter08_Advanced.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced.Controllers;

/// <summary>
/// Урок 8.1 "Скомпільовані запити (Compiled Queries)".
/// Демонструє використання EF.CompileQuery та EF.CompileAsyncQuery для уникнення
/// накладних витрат на регулярний аналіз дерева виразів LINQ.
/// </summary>
[ApiController]
[Route("api/ch08/compiled-queries")]
[Tags("Глава 8 — Додаткові можливості")]
public class CompiledQueriesController : ControllerBase
{
    private readonly AdvancedContext _db;

    // Скомпільований запит кешує результат трансляції виразу у делегат.
    // Виконується один раз при ініціалізації та перевикористовується для всіх наступних викликів.
    private static readonly Func<AdvancedContext, int, BankAccount?> GetAccountByIdCompiled =
        EF.CompileQuery((AdvancedContext db, int id) =>
            db.BankAccounts.FirstOrDefault(a => a.Id == id));

    private static readonly Func<AdvancedContext, decimal, IAsyncEnumerable<BankAccount>> GetHighBalanceAccountsCompiled =
        EF.CompileAsyncQuery((AdvancedContext db, decimal minBalance) =>
            db.BankAccounts.Where(a => a.Balance >= minBalance));

    public CompiledQueriesController(AdvancedContext db) => _db = db;

    /// <summary>
    /// Отримання банківського рахунку за ідентифікатором за допомогою попередньо скомпільованого запиту.
    /// </summary>
    [HttpGet("by-id/{id:int}")]
    public ActionResult<object> GetById(int id)
    {
        var account = GetAccountByIdCompiled(_db, id);
        return account is null ? NotFound() : Ok(account);
    }

    /// <summary>
    /// Асинхронний скомпільований запит із фільтром за балансом.
    /// </summary>
    [HttpGet("high-balance")]
    public async Task<ActionResult<object>> GetHighBalance([FromQuery] decimal minBalance = 2000)
    {
        var list = new List<BankAccount>();
        await foreach (var acc in GetHighBalanceAccountsCompiled(_db, minBalance))
        {
            list.Add(acc);
        }

        return Ok(list);
    }

    /// <summary>
    /// Порівняння продуктивності: стандартний LINQ-запит проти скомпільованого запиту (100 ітерацій).
    /// </summary>
    [HttpGet("benchmark")]
    public ActionResult<object> Benchmark([FromQuery] int iterations = 100)
    {
        // 1. Стандартний LINQ (щоразу аналізується Expression Tree)
        var swStandard = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            int targetId = (i % 4) + 1;
            _ = _db.BankAccounts.AsNoTracking().FirstOrDefault(a => a.Id == targetId);
        }
        swStandard.Stop();

        // 2. Скомпільований запит (делегат уже готовий)
        var swCompiled = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            int targetId = (i % 4) + 1;
            _ = GetAccountByIdCompiled(_db, targetId);
        }
        swCompiled.Stop();

        return Ok(new
        {
            iterations,
            standardLinqElapsedMs = swStandard.Elapsed.TotalMilliseconds,
            compiledQueryElapsedMs = swCompiled.Elapsed.TotalMilliseconds,
            explanation = "Скомпільований запит пропускає етап парсингу та оптимізації LINQ-дерева виразів при кожному виклику, що помітно знижує навантаження на CPU при високому RPS."
        });
    }
}
