using EfCoreExamples.Chapter05_Inheritance.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter05_Inheritance.Controllers;

/// <summary>
/// Порівняння стратегій TPH, TPT та TPC, генерація DDL-скрипта та скидання даних.
/// </summary>
[ApiController]
[Route("api/ch05/comparison")]
[Tags("Глава 5 — Успадкування")]
public class InheritanceComparisonController : ControllerBase
{
    private readonly InheritanceContext _db;

    public InheritanceComparisonController(InheritanceContext db) => _db = db;

    /// <summary>
    /// Порівняльна таблиця та рекомендації щодо вибору стратегії успадкування.
    /// </summary>
    [HttpGet("strategies")]
    public ActionResult<object> GetComparison()
    {
        return Ok(new
        {
            title = "Порівняння стратегій зіставлення успадкування в EF Core",
            strategies = new[]
            {
                new
                {
                    strategy = "TPH (Table Per Hierarchy)",
                    metanitLesson = "4.1",
                    tableCount = "Одна таблиця на всю ієрархію",
                    discriminator = "Потрібен стовпець-дискримінатор (за замовчуванням 'Discriminator')",
                    pros = "Найпростіша схема, максимальна швидкість поліморфних запитів (без JOIN і UNION), легко додавати нові класи",
                    cons = "Стовпці класів-нащадків повинні допускати NULL (nullable), навіть якщо логічно вони обов'язкові",
                    bestFor = "Більшості стандартних сценаріїв, де класи-нащадки мають небагато специфічних полів"
                },
                new
                {
                    strategy = "TPT (Table Per Type)",
                    metanitLesson = "4.2",
                    tableCount = "Окрема таблиця для кожного типу (базового і похідних)",
                    discriminator = "Не потрібен (тип визначається наявністю рядка у відповідній таблиці)",
                    pros = "Нормалізована схема БД, стовпці нащадків можуть бути NOT NULL",
                    cons = "Поліморфні запити вимагають кількох LEFT JOIN (деградація продуктивності при великій глибині ієрархії)",
                    bestFor = "Коли нащадки мають багато різних обов'язкових стовпців, або коли потрібна строга нормалізація БД"
                },
                new
                {
                    strategy = "TPC (Table Per Class)",
                    metanitLesson = "4.3",
                    tableCount = "Окрема таблиця для кожного конкретного типу (для абстрактного предка таблиці немає)",
                    discriminator = "Не потрібен",
                    pros = "Запити до конкретного нащадка відбуваються без JOIN, стовпці NOT NULL",
                    cons = "Поліморфний запит до базового типу генерує UNION ALL; дублювання визначень стовпців у таблицях",
                    bestFor = "Коли поліморфні запити рідкісні, а робота переважно ведеться з конкретними класами"
                }
            }
        });
    }

    /// <summary>
    /// Генерація повного DDL SQL-скрипта схеми бази даних глави 5.
    /// Дозволяє наочно побачити фізичну структуру таблиць для всіх трьох стратегій.
    /// </summary>
    [HttpGet("create-script")]
    public ActionResult<object> GetCreateScript()
    {
        var script = _db.Database.GenerateCreateScript();
        return Content(script, "text/plain");
    }

    /// <summary>Скидання даних глави 5 до початкового стану.</summary>
    [HttpPost("reset")]
    public ActionResult<object> Reset()
    {
        InheritanceSeeder.Reset(_db);
        return Ok(new { message = "Дані глави 5 повернуто до початкового стану." });
    }
}
