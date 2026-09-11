using EfCoreExamples.Chapter01_Introduction;
using EfCoreExamples.Chapter01_Introduction.ScaffoldedLike;
using EfCoreExamples.Chapter03_Models;
using EfCoreExamples.Chapter04_Relationships;
using EfCoreExamples.Chapter06_Queries;
using EfCoreExamples.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
//  1. Реєстрація сервісів у контейнері залежностей
// ─────────────────────────────────────────────────────────────────────────────

// Увесь API побудований на контролерах з атрибутною маршрутизацією.
builder.Services.AddControllers();

// Генерація OpenAPI-документа за адресою /openapi/v1.json (пакет Microsoft.AspNetCore.OpenApi).
builder.Services.AddOpenApi(options =>
{
    // Сортуємо теги за назвою, щоб у Scalar глави йшли по порядку (1, 3, 4, 6),
    // а не в порядку виявлення контролерів.
    options.AddDocumentTransformer((document, _, _) =>
    {
        if (document.Tags is { Count: > 1 })
        {
            var ordered = document.Tags.OrderBy(t => t.Name, StringComparer.Ordinal).ToList();
            document.Tags.Clear();
            foreach (var tag in ordered)
                document.Tags.Add(tag);
        }

        return Task.CompletedTask;
    });
});

// Кожна глава курсу має власний DbContext і власну базу даних на LocalDB.
// Так конфігурації моделей різних глав не конфліктують між собою,
// а кожен приклад лишається самодостатнім. Рядки підключення — у appsettings.json.
var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");

// Глава 1. Вступ.
builder.Services.AddDbContext<IntroductionContext>(options =>
    options.UseSqlServer(connectionStrings["Ch01"]));

// Глава 1.8. Окремий контекст, який навчається на СПРАВЖНІХ міграціях (не EnsureCreated).
builder.Services.AddDbContext<MigrationsDemoContext>(options =>
    options.UseSqlServer(connectionStrings["Ch01Migrations"]));

// Глава 1.3. "Database First": окремий контекст над тією самою БД, що й IntroductionContext.
builder.Services.AddDbContext<ExistingDbContext>(options =>
    options.UseSqlServer(connectionStrings["Ch01"]));

// Глава 1.4. "Пісочниця" для дослідів зі створенням/видаленням БД.
builder.Services.AddDbContext<SandboxContext>(options =>
    options.UseSqlServer(connectionStrings["Ch01Sandbox"]));

// Глава 3. Створення моделей.
builder.Services.AddDbContext<ModelsContext>(options =>
    options.UseSqlServer(connectionStrings["Ch03"]));

// Глава 4. Відношення між моделями.
builder.Services.AddDbContext<RelationshipsContext>(options =>
    options.UseSqlServer(connectionStrings["Ch04"]));

// Глава 4.6. Ліниве завантаження — окрема опція, що діє на весь контекст.
builder.Services.AddDbContext<LazyLoadingContext>(options => options
    .UseLazyLoadingProxies()
    .UseSqlServer(connectionStrings["Ch04Lazy"]));

// Глава 6. Запити та LINQ to Entities.
builder.Services.AddDbContext<QueriesContext>(options =>
    options.UseSqlServer(connectionStrings["Ch06"]));

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
//  2. Підготовка навчального середовища
// ─────────────────────────────────────────────────────────────────────────────

// Створюємо та наповнюємо початковими даними всі бази даних при старті,
// щоб кожен приклад одразу мав з чим працювати.
DatabaseBootstrapper.Run(app);

// ─────────────────────────────────────────────────────────────────────────────
//  3. Конвеєр обробки HTTP-запитів
//     Автентифікації та HTTPS-редиректу тут навмисно немає — це локальний
//     навчальний застосунок, зайві налаштування лише відволікали б від EF Core.
// ─────────────────────────────────────────────────────────────────────────────

// /openapi/v1.json — сирий OpenAPI-документ.
app.MapOpenApi();

// /scalar — інтерактивний UI, через який зручно надсилати запити до прикладів.
app.MapScalarApiReference(options => options
    .WithTitle("EF Core Examples — приклади за курсом metanit"));

app.MapControllers();

// Кореневий маршрут одразу веде на Scalar, щоб не шукати адресу вручну.
app.MapGet("/", () => Results.Redirect("/scalar"))
    .ExcludeFromDescription();

app.Run();
