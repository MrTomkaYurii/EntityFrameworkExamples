using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Урок 1.6 "Конфігурація підключення".
/// Показує, звідки застосунок бере налаштування БД і що саме в них закладено.
/// </summary>
[ApiController]
[Route("api/ch01/connection")]
[Tags("Глава 1 — Вступ до EF Core")]
public class ConnectionController : ControllerBase
{
    private readonly IntroductionContext _db;
    private readonly IConfiguration _configuration;

    public ConnectionController(IntroductionContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    /// <summary>Зведена інформація про поточне підключення.</summary>
    [HttpGet("info")]
    public async Task<ActionResult<object>> Info()
    {
        var connection = _db.Database.GetDbConnection();

        // ServerVersion доступний лише на відкритому з'єднанні — відкриваємо на мить.
        string? serverVersion = null;
        await connection.OpenAsync();
        try
        {
            serverVersion = connection.ServerVersion;
        }
        finally
        {
            await connection.CloseAsync();
        }

        return Ok(new
        {
            // Провайдер задається у Program.cs методом UseSqlServer(...).
            provider = _db.Database.ProviderName,

            // Рядок підключення живе в appsettings.json → ConnectionStrings:Ch01.
            connectionStringSource = "appsettings.json → ConnectionStrings:Ch01",
            connectionString = Mask(_configuration.GetConnectionString("Ch01")),

            server = connection.DataSource,
            database = connection.Database,
            serverVersion,

            // Тайм-аут команди можна змінити в UseSqlServer(o => o.CommandTimeout(...)).
            commandTimeoutSeconds = _db.Database.GetCommandTimeout()
        });
    }

    // Прибираємо потенційно чутливі значення (пароль тощо) перед показом рядка підключення.
    private static string? Mask(string? connectionString) =>
        connectionString is null
            ? null
            : Regex.Replace(connectionString, "(?i)(password|pwd)=[^;]*", "$1=***");
}
