# EF Core Examples

Навчальні приклади Entity Framework Core, зібрані за структурою курсу
[metanit — Entity Framework Core](https://metanit.com/sharp/efcore/).

Кожна можливість EF Core — це окремий ендпоінт Web API. Відкриваєш `/scalar`,
надсилаєш запит, дивишся результат (а часто — і згенерований SQL).

## Що потрібно

- **.NET SDK 10** (`dotnet --version` → `10.0.x`)
- **SQL Server LocalDB** — перевірка: `sqllocaldb info` має показати `MSSQLLocalDB`.
  Зазвичай ставиться разом із Visual Studio (робоче навантаження "Data storage and processing")
  або окремо: [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb).
- Для глави про міграції — інструмент `dotnet ef`:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Запуск

```bash
dotnet run
```

Застосунок при старті сам створює й наповнює всі потрібні бази даних на LocalDB
(див. `Infrastructure/DatabaseBootstrapper.cs`), після чого відкриває Scalar UI.

- Scalar UI: `http://localhost:5299/scalar`
- OpenAPI-документ: `http://localhost:5299/openapi/v1.json`
- Готові запити: `requests.http` (відкрити у Visual Studio / Rider / VS Code + REST Client)

## Принципи проєкту

- **Web API на контролерах.** Один ендпоінт = одна можливість EF Core.
- **Без бізнес-логіки.** Контролери працюють напряму з `DbContext`. Жодних сервісів,
  репозиторіїв, MediatR — лише те, що потрібно, щоб показати саме EF Core.
- **Один `DbContext` і одна база даних на главу.** Конфігурації моделей різних глав
  конфліктували б між собою; ізольований контекст лишається самодостатнім прикладом.
- **Коментарі українською, код — англійською.**
- **Скидання даних.** Глави, приклади яких змінюють дані, мають ендпоінт `POST .../reset`.

## Глави

| Глава | Тема | Стан | Деталі |
|---|---|---|---|
| 1 | Вступ до EF Core | ✅ готово | [Chapter01_Introduction/README.md](Chapter01_Introduction/README.md) |
| 2 | Провайдери баз даних | 🔜 заплановано | — |
| 3 | Створення моделей | ✅ готово | [Chapter03_Models/README.md](Chapter03_Models/README.md) |
| 4 | Відношення між моделями | ✅ готово | [Chapter04_Relationships/README.md](Chapter04_Relationships/README.md) |
| 5 | Успадкування (TPH / TPT / TPC) | 🔜 заплановано | — |
| 6 | Запити та LINQ to Entities | ✅ готово | [Chapter06_Queries/README.md](Chapter06_Queries/README.md) |
| 7 | SQL в EF Core (процедури, функції) | 🔜 заплановано | — |
| 8 | Додаткові статті (паралелізм тощо) | 🔜 заплановано | — |

У Scalar ендпоінти згруповані за главами (тег `Глава N — ...`).

## Структура

```
EfCoreExamples.csproj        один проєкт, net10.0
Program.cs                    мінімальний хост: контролери + OpenAPI/Scalar + реєстрація DbContext
appsettings.json             рядки підключення до LocalDB (по одному на главу)
requests.http                готові HTTP-запити
Infrastructure/
  DatabaseBootstrapper.cs    створення + наповнення всіх БД при старті
  QueryPresentation.cs       хелпер { sql, count, data } для навчальних запитів
Chapter0N_Xxx/
  README.md                  конспект глави: урок metanit → контролер → ендпоінт
  Models/                    сутності
  XxxContext.cs              DbContext глави
  Seed/                      початкові дані
  Controllers/               по одному контролеру на групу уроків
```

## Скидання всіх баз даних

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DECLARE @s nvarchar(max)=''; SELECT @s+='DROP DATABASE ['+name+'];' FROM sys.databases WHERE name LIKE 'EfCoreExamples[_]%'; EXEC(@s)"
```

Наступний `dotnet run` створить їх заново.
