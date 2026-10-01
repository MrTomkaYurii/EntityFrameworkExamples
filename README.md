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

## 📚 Навчальні матеріали та Архітектурні посібники

Для швидкого занурення в теорію та архітектурні патерни підготовлено інтерактивні матеріали:

- 🎓 **[Інтерактивна презентація архітектури EF Core](docs/Architecture-Presentation.md)** — майстер-клас на 25 слайдів з клікабельною навігацією, життєвими аналогіями, порівняльними таблицями, Mermaid-діаграмами та прямими посиланнями на код проєкту.
- 📘 **[Глибокий технічний довідник та інженерна шпаргалка](docs/EFCore-Architecture-Guide.md)** — розбір внутрішніх механік `DbContext` та `ChangeTracker`, конвеєра збереження даних (Save Pipeline), матриця CLI-команд `dotnet ef`, стратегії оптимізації продуктивності та обробка конкурентності.

## Глави курсу Metanit

| Глава | Тема | Стан | Деталі |
|---|---|---|---|
| 1 | Вступ до EF Core | ✅ готово | [Chapter01_Introduction/README.md](Chapter01_Introduction/README.md) |
| 2 | Провайдери баз даних | ✅ готово | [Chapter02_Providers/README.md](Chapter02_Providers/README.md) |
| 3 | Створення моделей | ✅ готово | [Chapter03_Models/README.md](Chapter03_Models/README.md) |
| 4 | Відношення між моделями | ✅ готово | [Chapter04_Relationships/README.md](Chapter04_Relationships/README.md) |
| 5 | Успадкування (TPH / TPT / TPC) | ✅ готово | [Chapter05_Inheritance/README.md](Chapter05_Inheritance/README.md) |
| 6 | Запити та LINQ to Entities | ✅ готово | [Chapter06_Queries/README.md](Chapter06_Queries/README.md) |
| 7 | SQL в EF Core (процедури, функції) | ✅ готово | [Chapter07_Sql/README.md](Chapter07_Sql/README.md) |
| 8 | Додаткові статті (паралелізм тощо) | ✅ готово | [Chapter08_Advanced/README.md](Chapter08_Advanced/README.md) |

У Scalar ендпоінти згруповані за главами (тег `Глава N — ...`).

## Структура

```
EfCoreExamples.csproj        один проєкт, net10.0 (OpenAPI, Scalar, SqlServer)
Program.cs                    мінімальний хост: контролери + OpenAPI/Scalar + реєстрація DbContext
appsettings.json             рядки підключення до LocalDB (по одному на главу)
requests.http                готові HTTP-запити для тестування ендпоінтів
docs/
  Architecture-Presentation.md  інтерактивна презентація майстер-класу (25 слайдів з Mermaid)
  EFCore-Architecture-Guide.md  глибокий технічний довідник та інженерна шпаргалка
Infrastructure/
  DatabaseBootstrapper.cs    автоматичне створення + наповнення всіх БД при старті
  QueryPresentation.cs       хелпер { sql, count, data } для навчальних запитів
Chapter01_Introduction/      Вступ, CRUD, конфігурація, логування, міграції
Chapter02_Providers/         Провайдери СУБД, стійкість зв'язку (EnableRetryOnFailure)
Chapter03_Models/            Моделі, Fluent API, анотації, ключі, індекси, backing fields
Chapter04_Relationships/     1:1, 1:N, N:M, Owned Types, Complex Types, Lazy Loading
Chapter05_Inheritance/       Успадкування: TPH, TPT, TPC стратегії зіставлення
Chapter06_Queries/           LINQ, AsNoTracking, фільтри моделі, ExecuteUpdate/Delete
Chapter07_Sql/               Сирий SQL (FromSql), збережені функції (UDF), процедури
Chapter08_Advanced/          Оптимістичний паралелізм (RowVersion), Views, Temporal Tables
```

## Скидання всіх баз даних

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DECLARE @s nvarchar(max)=''; SELECT @s+='DROP DATABASE ['+name+'];' FROM sys.databases WHERE name LIKE 'EfCoreExamples[_]%'; EXEC(@s)"
```

Наступний `dotnet run` створить їх заново.
