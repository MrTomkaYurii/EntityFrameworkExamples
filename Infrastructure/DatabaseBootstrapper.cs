using EfCoreExamples.Chapter01_Introduction;
using EfCoreExamples.Chapter01_Introduction.Seed;
using EfCoreExamples.Chapter03_Models;
using EfCoreExamples.Chapter03_Models.Seed;
using EfCoreExamples.Chapter04_Relationships;
using EfCoreExamples.Chapter04_Relationships.Seed;
using EfCoreExamples.Chapter05_Inheritance;
using EfCoreExamples.Chapter05_Inheritance.Seed;
using EfCoreExamples.Chapter06_Queries;
using EfCoreExamples.Chapter06_Queries.Seed;
using EfCoreExamples.Chapter07_Sql;
using EfCoreExamples.Chapter07_Sql.Seed;
using EfCoreExamples.Chapter08_Advanced;
using EfCoreExamples.Chapter08_Advanced.Seed;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Infrastructure;

/// <summary>
/// Створює та наповнює бази даних усіх глав під час старту застосунку.
/// Це НЕ бізнес-логіка — лише підготовка навчального середовища, щоб кожен
/// приклад одразу мав з чим працювати.
/// </summary>
/// <remarks>
/// Більшість контекстів створюються через <c>Database.EnsureCreated()</c> —
/// найпростіший спосіб отримати БД за моделлю без міграцій.
/// Виняток — <see cref="MigrationsDemoContext"/>: він навмисно застосовує
/// справжні міграції (урок 1.8).
/// </remarks>
public static class DatabaseBootstrapper
{
    public static void Run(WebApplication app)
    {
        // DbContext зареєстровано як scoped-сервіс, а на старті ще немає HTTP-запиту
        // (і його області). Тому створюємо власну область вручну.
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseBootstrapper");

        try
        {
            // ── Глава 1. Вступ ────────────────────────────────────────────────
            var introduction = services.GetRequiredService<IntroductionContext>();
            introduction.Database.EnsureCreated();          // створити БД за моделлю, якщо її ще немає
            IntroductionSeeder.Seed(introduction);

            // Пісочниця уроку 1.4 — просто створюємо порожню БД (її можна вільно видаляти).
            services.GetRequiredService<SandboxContext>().Database.EnsureCreated();

            // ── Глава 1.8. Міграції ───────────────────────────────────────────
            // Тут саме Migrate(): застосовує всі міграції зі збірки й створює
            // таблицю __EFMigrationsHistory. EnsureCreated() і Migrate() —
            // взаємовиключні способи створення схеми.
            var migrationsDemo = services.GetRequiredService<MigrationsDemoContext>();
            migrationsDemo.Database.Migrate();
            MigrationsDemoSeeder.Seed(migrationsDemo);

            // ── Глава 3. Створення моделей ────────────────────────────────────
            var models = services.GetRequiredService<ModelsContext>();
            models.Database.EnsureCreated();
            ModelsSeeder.Seed(models);   // AppSetting/Category сіються через HasData самим EnsureCreated

            // ── Глава 4. Відношення між моделями ──────────────────────────────
            var relationships = services.GetRequiredService<RelationshipsContext>();
            relationships.Database.EnsureCreated();
            RelationshipsSeeder.Seed(relationships);

            var lazyLoading = services.GetRequiredService<LazyLoadingContext>();
            lazyLoading.Database.EnsureCreated();
            LazyLoadingSeeder.Seed(lazyLoading);

            // ── Глава 5. Успадкування ─────────────────────────────────────────
            var inheritance = services.GetRequiredService<InheritanceContext>();
            inheritance.Database.EnsureCreated();
            InheritanceSeeder.Seed(inheritance);

            // ── Глава 6. Запити та LINQ to Entities ───────────────────────────
            var queries = services.GetRequiredService<QueriesContext>();
            queries.Database.EnsureCreated();
            QueriesSeeder.Seed(queries);

            // ── Глава 7. SQL в EF Core ────────────────────────────────────────
            var sql = services.GetRequiredService<SqlContext>();
            sql.Database.EnsureCreated();
            SqlSeeder.Seed(sql);

            // ── Глава 8. Додаткові можливості ─────────────────────────────────
            var advanced = services.GetRequiredService<AdvancedContext>();
            advanced.Database.EnsureCreated();
            AdvancedSeeder.Seed(advanced);

            logger.LogInformation("Навчальні бази даних готові.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Не вдалося підготувати навчальні бази даних. Переконайтесь, що встановлено " +
                "SQL Server LocalDB (перевірка: `sqllocaldb info`).");
        }
    }
}
