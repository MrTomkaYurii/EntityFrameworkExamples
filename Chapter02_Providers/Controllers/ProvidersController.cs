using System.Text.RegularExpressions;
using EfCoreExamples.Chapter01_Introduction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter02_Providers.Controllers;

/// <summary>
/// Глава 2 "Провайдери баз даних".
/// Демонструє можливості, конфігурацію та роботу з провайдерами СУБД в EF Core.
/// </summary>
[ApiController]
[Route("api/ch02/providers")]
[Tags("Глава 2 — Провайдери баз даних")]
public class ProvidersController : ControllerBase
{
    private readonly IntroductionContext _db;
    private readonly IConfiguration _configuration;

    public ProvidersController(IntroductionContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    /// <summary>
    /// Інформація про активний провайдер, використаний у цьому навчальному проєкті (SQL Server).
    /// </summary>
    [HttpGet("current")]
    public async Task<ActionResult<object>> GetCurrentProvider()
    {
        var connection = _db.Database.GetDbConnection();
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
            providerName = _db.Database.ProviderName,
            isSqlServer = _db.Database.IsSqlServer(),
            server = connection.DataSource,
            database = connection.Database,
            serverVersion,
            commandTimeout = _db.Database.GetCommandTimeout() ?? 30,
            activeConnectionState = connection.State.ToString()
        });
    }

    /// <summary>
    /// Огляд провайдерів баз даних за матеріалами Metanit: MS SQL Server, MySQL, PostgreSQL.
    /// </summary>
    [HttpGet("catalog")]
    public ActionResult<object> GetProvidersCatalog()
    {
        var catalog = new[]
        {
            new
            {
                rdbms = "Microsoft SQL Server",
                nugetPackage = "Microsoft.EntityFrameworkCore.SqlServer",
                connectionMethod = "options.UseSqlServer(connectionString)",
                sampleConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True;TrustServerCertificate=True;",
                notes = "Офіційний провайдер від Microsoft. Підтримує Temporal Tables, HierarchyId, RowVersion (Concurrency), LocalDB, Azure SQL."
            },
            new
            {
                rdbms = "PostgreSQL",
                nugetPackage = "Npgsql.EntityFrameworkCore.PostgreSQL",
                connectionMethod = "options.UseNpgsql(connectionString)",
                sampleConnectionString = "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=secret",
                notes = "Провідний open-source провайдер для PostgreSQL. Підтримує масиви, JSON/JSONB, геометричні типи, перерахування (enums)."
            },
            new
            {
                rdbms = "MySQL / MariaDB",
                nugetPackage = "Pomelo.EntityFrameworkCore.MySql",
                connectionMethod = "options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))",
                sampleConnectionString = "Server=localhost;Port=3306;Database=mydb;Uid=root;Pwd=secret;",
                notes = "Спільнотний провайдер Pomelo. Вимагає явного вказання версії сервера (ServerVersion) для оптимізації діалекту SQL."
            },
            new
            {
                rdbms = "SQLite",
                nugetPackage = "Microsoft.EntityFrameworkCore.Sqlite",
                connectionMethod = "options.UseSqlite(\"Data Source=app.db\")",
                sampleConnectionString = "Data Source=app.db",
                notes = "Вбудована файлова БД, чудово підходить для мобільних додатків, десктопу або легких тестів."
            }
        };

        return Ok(catalog);
    }

    /// <summary>
    /// Демонстрація параметрів стійкості (resilience), пулінгу та повторних спроб у SQL Server.
    /// </summary>
    [HttpGet("resilience")]
    public ActionResult<object> GetResilienceExplanation()
    {
        return Ok(new
        {
            title = "Налаштування стійкості підключення (Connection Resiliency)",
            description = "У хмарних середовищах та навантажених мережах трапляються короткочасні збої (transient faults). EF Core дозволяє налаштувати автоматичні повтори:",
            codeExample = """
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Вмикає стратегію повторних спроб (3 спроби з паузою до 5 секунд)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);

                    // Задає час очікування виконання SQL-команд
                    sqlOptions.CommandTimeout(60);

                    // Таблиця для збереження історії міграцій
                    sqlOptions.MigrationsHistoryTable("__MyMigrationsHistory", "admin");
                }));
            """,
            benefits = new[]
            {
                "Автоматичний повтор запитів у разі тимчасової недоступності мережі або СУБД",
                "Підтримка транзакційних блоків через db.Database.CreateExecutionStrategy().Execute(() => ...)",
                "Запобігання падінню сервісу при короткочасних розривах з'єднання"
            }
        });
    }
}
