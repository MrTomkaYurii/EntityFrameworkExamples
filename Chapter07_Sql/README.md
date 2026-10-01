# Глава 7. SQL в Entity Framework Core

Відповідає розділу [metanit — SQL в Entity Framework Core](https://metanit.com/sharp/efcore/6.1.php).

Контекст: `SqlContext` → база `EfCoreExamples_Ch07_Sql` (`EnsureCreated` + створення UDF/SP при старті).

| Урок metanit | Демонструє | Технологія EF Core | Ендпоінти |
|---|---|---|---|
| 6.1 Виконання SQL-запитів | Параметризований `FromSql`, `FromSqlRaw`, компонування з LINQ, `Database.SqlQuery<T>`, неключові сутності, `ExecuteSql` | `FromSql`, `SqlQuery`, `CategorySummary` (HasNoKey) | `GET /api/ch07/raw-sql/from-sql-interpolated`, `GET .../raw-sql/compose-linq`, `GET .../raw-sql/scalar-query`, `POST .../raw-sql/execute-sql` |
| 6.2 Збережені функції | Скалярні функції в LINQ-виразах (`HasDbFunction`), табличні функції (TVF) | `[DbFunction]`, `FromExpression` | `GET /api/ch07/stored-functions/scalar-function`, `GET .../stored-functions/table-valued-function` |
| 6.3 Збережені процедури | Виклик збереженої процедури з вибіркою сутностей, вихідні параметри (`OUTPUT`), модифікація даних | `FromSqlRaw`, `SqlParameter.Direction = Output`, `ExecuteSqlRaw` | `GET /api/ch07/stored-procedures/by-category`, `GET .../stored-procedures/output-parameters`, `POST .../stored-procedures/increase-prices` |

## На що звернути увагу

- **Захист від SQL Injection**: `FromSql($"SELECT ... WHERE Price > {minPrice}")` автоматично формує безпечний параметризований запит `@p0`, а не підставляє текст напряму.
- **Компонування запитів**: `FromSql` повертає `IQueryable<T>`. EF Core дозволяє додавати методи `.Where(...)`, `.OrderBy(...)`, `.Take(...)`, які будуть трансльовані у SQL-підзапит (`FROM (SELECT ...) AS [t] WHERE ...`).
- **Скалярні функції**: C#-метод слугує лише дескриптором для побудови дерева виразів — під час виконання EF Core замінює його на ім'я функції SQL Server (`dbo.fn_CalculateDiscount`).
