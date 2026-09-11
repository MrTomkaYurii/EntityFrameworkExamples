# Глава 4. Відношення між моделями

Відповідає розділу [metanit — Отношения между моделями](https://metanit.com/sharp/efcore/4.1.php).

Контексти:
- `RelationshipsContext` → база `EfCoreExamples_Ch04_Relationships`
- `LazyLoadingContext` → база `EfCoreExamples_Ch04_LazyLoading` (з `UseLazyLoadingProxies`)

| Урок metanit | Демонструє | Модель | Ендпоінти |
|---|---|---|---|
| 4.1 FK та навігації | зовнішній ключ, навігаційні властивості | `Blog` / `Post` | `GET /api/ch04/one-to-many/foreign-keys` |
| 4.2 Налаштування FK | `HasForeignKey`, обов'язковість за типом | `Blog` / `Post` | `GET /api/ch04/one-to-many/foreign-keys`, `POST /api/ch04/one-to-many/blogs/{id}/posts` |
| 4.3 Каскадне видалення | `Cascade` / `SetNull` / `Restrict` | `Blog→Post`, `Company→User`, `Department→Project` | `GET /api/ch04/cascade-delete/behaviors`, `DELETE /api/ch04/cascade-delete/{blogs,companies,departments}/{id}` |
| 4.4 Include | `Include`, `ThenInclude`, фільтрований `Include` | `Blog` / `Post`, `Student` / `Course` | `GET /api/ch04/loading/{include,then-include,filtered-include}` |
| 4.5 Explicit loading | `Entry(...).Collection(...).Load()` | `Blog` / `Post` | `GET /api/ch04/loading/explicit/{blogId}` |
| 4.6 Lazy loading | `virtual` навігації, проксі, проблема N+1 | `Team` / `Player` | `GET /api/ch04/lazy-loading/teams` проти `.../teams-eager` |
| 4.7 Один до одного | залежна сторона за FK, унікальний індекс | `User` / `UserProfile` | `GET /api/ch04/one-to-one/users`, `POST /api/ch04/one-to-one/users/{id}/profile` |
| 4.8 Один до багатьох | колекційна навігація, проєкція count | `Blog` / `Post` | `GET /api/ch04/one-to-many/blogs`, `.../blogs/{id}` |
| 4.9 Багато до багатьох | явна проміжна сутність, додаткові дані зв'язку | `Student` / `Course` / `Enrollment` | `GET /api/ch04/many-to-many/{students,courses,enrollments}`, `POST /api/ch04/many-to-many/enroll` |
| 4.10 Власні типи | `OwnsOne` (той самий рядок), `OwnsMany` (окрема таблиця) | `Order` / `OrderAddress` / `OrderItem` | `GET /api/ch04/owned-types/{orders,storage}` |
| 4.11 Комплексні типи | `ComplexProperty`, value object без ключа | `Customer` / `Address` | `GET /api/ch04/complex-types/{customers,complex-vs-owned}` |
| 4.12 Ієрархічні дані | self-reference, побудова дерева в пам'яті | `Employee` | `GET /api/ch04/hierarchical/{roots,tree}`, `.../employees/{id}/chain` |

## Порядок для уроку 4.3 (каскадне видалення)

1. `GET /api/ch04/cascade-delete/behaviors` — побачити налаштування всіх зв'язків.
2. `DELETE /api/ch04/cascade-delete/blogs/1` — разом із блогом зникають його дописи (Cascade).
3. `DELETE /api/ch04/cascade-delete/companies/1` — працівники лишаються, але без компанії (SetNull).
4. `DELETE /api/ch04/cascade-delete/departments/1` — помилка: у відділу є проєкти (Restrict).
5. `POST /api/ch04/cascade-delete/reset` — відновити дані.

## Lazy loading (4.6)

Порівняй SQL у логах:
- `GET /api/ch04/lazy-loading/teams` → 1 запит на команди + окремий запит на гравців кожної команди (**N+1**).
- `GET /api/ch04/lazy-loading/teams-eager` → 1 запит із `JOIN`.
