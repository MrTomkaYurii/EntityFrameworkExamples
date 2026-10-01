# Глава 8. Додаткові статті

Відповідає розділу [metanit — Дополнительные статьи](https://metanit.com/sharp/efcore/8.1.php).

Контекст: `AdvancedContext` → база `EfCoreExamples_Ch08_Advanced` (`EnsureCreated` + створення VIEW при старті).

| Урок metanit | Демонструє | Моделі / API | Ендпоінти |
|---|---|---|---|
| 2.11 Параллелизм | Оптимістичний паралелізм через `[Timestamp]` / `RowVersion`, перехоплення `DbUpdateConcurrencyException`, стратегії вирішення | `BankAccount`, `IsRowVersion()` | `GET /api/ch08/concurrency/accounts`, `POST /api/ch08/concurrency/simulate-conflict` |
| 8.1 Скомпилированные запросы | `EF.CompileQuery`, `EF.CompileAsyncQuery`, заміри швидкодії | `BankAccount`, делегати | `GET /api/ch08/compiled-queries/by-id/{id}`, `GET .../compiled-queries/benchmark` |
| 8.2 Проекция на представления | Робота з об'єктами SQL Server VIEW через `ToView()` та `HasNoKey()` | `AccountSummaryView` | `GET /api/ch08/views/summary`, `GET .../views/high-balance` |
| 8.3 Хранение истории изменений | Вбудоване версіонування таблиць SQL Server (Temporal Tables): `IsTemporal()`, `TemporalAll()`, `TemporalAsOf()` | `Document` | `GET /api/ch08/temporal/documents`, `POST .../documents/{id}/update`, `GET .../documents/{id}/history`, `GET .../documents/{id}/as-of` |

## На що звернути увагу

- **Оптимістичний паралелізм**: у SQL Server тип `rowversion` автоматично збільшується при кожному оновленні рядка. Якщо два запити спробують одночасно змінити рядок із тим самим початковим значенням токена, другий отримає `DbUpdateConcurrencyException`.
- **Скомпільовані запити**: корисні на високонавантажених ділянках (high-throughput API), де парсинг дерева LINQ створює відчутне навантаження на процесор.
- **Темпоральні таблиці**: SQL Server сам зберігає тіньову історію з датами початку й завершення дії кожної версії (`PeriodStart`, `PeriodEnd`). Метод `TemporalAsOf(dateTime)` дозволяє "повернутися в часі" й отримати стан сутності на будь-яку секунду в минулому.
