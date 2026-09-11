# Глава 1. Вступ до EF Core

Відповідає розділу [metanit — Введение в Entity Framework Core](https://metanit.com/sharp/efcore/1.1.php).

| Урок metanit | Демонструє | Де в коді | Ендпоінти |
|---|---|---|---|
| 1.2 Перше застосування | POCO-клас, `DbContext`, `DbSet<T>` | `IntroductionContext`, `Models/User.cs` | `GET /api/ch01/first-app/users` |
| 1.3 Підключення до наявної БД | Database First, `dotnet ef dbcontext scaffold`, дві моделі над однією таблицею | `ScaffoldedLike/ExistingDbContext.cs` | `GET /api/ch01/existing-database/users` |
| 1.4 Управління базою даних | `CanConnect`, `EnsureCreated`, `EnsureDeleted`, `GenerateCreateScript` | `SandboxContext`, `Controllers/DatabaseManagementController.cs` | `GET /api/ch01/database/can-connect`, `POST /api/ch01/database/ensure-created`, `POST /api/ch01/database/ensure-deleted`, `GET /api/ch01/database/create-script` |
| 1.5 CRUD | `Add` / `Find` / зміна властивостей / `Remove` + `SaveChanges` | `Controllers/FirstAppController.cs` | `POST`/`GET`/`PUT`/`DELETE /api/ch01/first-app/users/{id}` |
| 1.6 Конфігурація підключення | звідки береться рядок підключення, провайдер, тайм-аут | `Controllers/ConnectionController.cs` | `GET /api/ch01/connection/info` |
| 1.7 Логування операцій | `LogTo`, `EnableSensitiveDataLogging` | `Controllers/LoggingController.cs` | `GET /api/ch01/logging/capture`, `GET /api/ch01/logging/capture-sensitive` |
| 1.8 Міграції | `GetMigrations`, `GetPendingMigrations`, `Migrate`, відкат до цільової міграції, SQL-скрипт | `MigrationsDemoContext`, `Migrations/`, `Controllers/MigrationsController.cs` | `GET /api/ch01/migrations/{all,applied,pending,script}`, `POST /api/ch01/migrations/{migrate,revert-to-initial}` |

## Бази даних глави

| Контекст | База даних | Як створюється |
|---|---|---|
| `IntroductionContext` | `EfCoreExamples_Ch01_Introduction` | `EnsureCreated` + seed при старті |
| `ExistingDbContext` | `EfCoreExamples_Ch01_Introduction` (та сама) | — (лише читає) |
| `SandboxContext` | `EfCoreExamples_Ch01_Sandbox` | `EnsureCreated` при старті; далі керується вручну |
| `MigrationsDemoContext` | `EfCoreExamples_Ch01_Migrations` | `Database.Migrate()` при старті |

## Порядок для уроку 1.8 (повний цикл міграцій)

1. `GET /api/ch01/migrations/applied` → обидві міграції вже застосовані (їх наклав `DatabaseBootstrapper`).
2. `POST /api/ch01/migrations/revert-to-initial` → БД відкотилася до `Initial`.
3. `GET /api/ch01/migrations/pending` → показує `AddProductCreatedAt`.
4. `POST /api/ch01/migrations/migrate` → застосовує її знову.

## Команди `dotnet ef`, використані в главі

```bash
dotnet ef migrations add Initial --context MigrationsDemoContext -o Chapter01_Introduction/Migrations
dotnet ef migrations add AddProductCreatedAt --context MigrationsDemoContext -o Chapter01_Introduction/Migrations
dotnet ef database update --context MigrationsDemoContext
```
