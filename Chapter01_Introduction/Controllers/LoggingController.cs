using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Урок 1.7 "Логування операцій".
/// Показує, як побачити SQL, який EF Core відправляє в базу даних.
/// <para>
/// Налаштування логування задається при створенні контексту. Щоб не змінювати
/// глобальну конфігурацію, у цих прикладах ми будуємо власний тимчасовий
/// <see cref="IntroductionContext"/> прямо в екшені.
/// </para>
/// </summary>
[ApiController]
[Route("api/ch01/logging")]
[Tags("Глава 1 — Вступ до EF Core")]
public class LoggingController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public LoggingController(IConfiguration configuration) => _configuration = configuration;

    /// <summary>
    /// <c>LogTo</c> перенаправляє лог EF Core у будь-який приймач (тут — у рядок).
    /// Параметри запиту в логах замінені на <c>?</c>.
    /// </summary>
    [HttpGet("capture")]
    public ActionResult<object> Capture()
    {
        var log = new StringBuilder();

        var options = new DbContextOptionsBuilder<IntroductionContext>()
            .UseSqlServer(_configuration.GetConnectionString("Ch01"))
            // Кожен рядок логу рівня Information і вище додаємо в StringBuilder.
            .LogTo(line => log.AppendLine(line), LogLevel.Information)
            .Options;

        using var db = new IntroductionContext(options);
        var users = db.Users.Where(u => u.Age > 30).OrderBy(u => u.Name).ToList();

        return Ok(new { log = log.ToString(), users });
    }

    /// <summary>
    /// Те саме, але з <c>EnableSensitiveDataLogging()</c> — тепер у логах видно
    /// РЕАЛЬНІ значення параметрів (<c>@__age_0='30'</c>).
    /// <b>Увага:</b> вмикати таке можна лише локально для дебагу, ніколи не на проді —
    /// у лог можуть потрапити персональні дані.
    /// </summary>
    [HttpGet("capture-sensitive")]
    public ActionResult<object> CaptureSensitive()
    {
        var log = new StringBuilder();

        var options = new DbContextOptionsBuilder<IntroductionContext>()
            .UseSqlServer(_configuration.GetConnectionString("Ch01"))
            .LogTo(line => log.AppendLine(line), LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        using var db = new IntroductionContext(options);
        var age = 30;
        var users = db.Users.Where(u => u.Age > age).OrderBy(u => u.Name).ToList();

        return Ok(new
        {
            note = "Порівняй значення параметрів у полі log із виводом ендпоінта /capture.",
            log = log.ToString(),
            users
        });
    }
}
