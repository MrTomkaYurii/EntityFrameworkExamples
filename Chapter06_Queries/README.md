# Глава 6. Запити та LINQ to Entities

Відповідає розділу [metanit — Запросы и LINQ to Entities](https://metanit.com/sharp/efcore/6.1.php).

Контекст: `QueriesContext` → база `EfCoreExamples_Ch06_Queries`.
**Більшість ендпоінтів повертають `{ sql, count, data }`** — щоб одразу бачити, у який SQL
перетворюється LINQ-вираз (через `IQueryable.ToQueryString()`, хелпер `QueryPresentation`).

| Урок metanit | Демонструє | Ендпоінти |
|---|---|---|
| 6.2 Вибірка та фільтрація | `Where`, `Find`, `First`/`Single`, `Skip`/`Take` | `GET /api/ch06/filtering/{all,where,by-id/{id},first-vs-single,paging}` |
| 6.3 Сортування та проєкція | `OrderBy`/`ThenBy`, `Select` (анонімний тип, DTO), `SelectMany` | `GET /api/ch06/sorting-projection/{order-by,projection-anonymous,projection-dto,select-many}` |
| 6.4 З'єднання та групування | `Join`, навігаційне з'єднання, `GroupBy`, `GroupJoin` | `GET /api/ch06/join-grouping/{join,navigation-join,group-by,group-join}` |
| 6.5 Операції з множинами | `Union`, `Concat`, `Intersect`, `Except`, `Distinct` | `GET /api/ch06/set-operations/{union,concat,intersect,except,distinct}` |
| 6.6 Агрегатні операції | `Count`, `Sum`, `Min`, `Max`, `Average`, `Any`, `All` | `GET /api/ch06/aggregation/{salary,any-all,filtered}` |
| 6.7 Відстеження та AsNoTracking | трекінг проти `AsNoTracking`, `ChangeTracker.Entries`, ідентичність об'єктів | `GET /api/ch06/tracking/{tracking,no-tracking,identity-resolution}` |
| 6.8 Виконання запитів | відкладене виконання, повторне виконання одного `IQueryable` | `GET /api/ch06/query-execution/deferred` |
| 6.9 IEnumerable та IQueryable | межа "БД / пам'ять": фільтр у SQL проти фільтра в пам'яті після `AsEnumerable()` | `GET /api/ch06/query-execution/{iqueryable,ienumerable}` |
| 6.10 Фільтри рівня моделі | `HasQueryFilter`, `IgnoreQueryFilters` | `GET /api/ch06/query-filters/{default,ignored,summary}` |
| 6.11 Масове оновлення та видалення | `ExecuteUpdate`, `ExecuteDelete` (без завантаження сутностей) | `POST /api/ch06/bulk-operations/{raise-salary,delete-out-of-stock,reset}` |

## На що звернути увагу

- У кожному SQL присутній `WHERE [u].[IsDeleted] = CAST(0 AS bit)` — це глобальний фільтр (6.10).
- Порівняй `query-execution/iqueryable` та `query-execution/ienumerable`: у другому в SQL **немає**
  фільтра по `Salary` — його виконано вже в пам'яті застосунку.
- `bulk-operations` змінюють дані напряму в БД; поверни їх у вихідний стан через `POST .../reset`.
