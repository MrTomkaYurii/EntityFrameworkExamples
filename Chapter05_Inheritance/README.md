# Глава 5. Успадкування (TPH / TPT / TPC)

Відповідає розділу [metanit — Наследование](https://metanit.com/sharp/efcore/4.1.php).

Контекст: `InheritanceContext` → база `EfCoreExamples_Ch05_Inheritance` (`EnsureCreated` при старті).

| Урок metanit | Демонструє | Моделі | Ендпоінти |
|---|---|---|---|
| 4.1 TPH (Table Per Hierarchy) | Одна таблиця `Users_TPH` для всієї ієрархії, стовпець-дискримінатор | `UserTph`, `EmployeeTph`, `ManagerTph` | `GET /api/ch05/tph/all`, `GET /api/ch05/tph/employees`, `POST /api/ch05/tph/managers` |
| 4.2 TPT (Table Per Type) | Окремі таблиці для кожного типу, зв'язок через FK на PK предка | `BillingAccountTpt`, `CreditAccountTpt`, `DepositAccountTpt` | `GET /api/ch05/tpt/all`, `GET /api/ch05/tpt/credits`, `POST /api/ch05/tpt/credit-account` |
| 4.3 TPC (Table Per Class) | Окремі таблиці для конкретних класів (EF Core 7+), спільна послідовність PK | `DeviceTpc`, `SmartphoneTpc`, `LaptopTpc` | `GET /api/ch05/tpc/all`, `GET /api/ch05/tpc/smartphones`, `POST /api/ch05/tpc/smartphones` |
| Порівняння стратегій | Зведена таблиця характеристик + повний DDL-скрипт усіх таблиць | Усі моделі глави | `GET /api/ch05/comparison/strategies`, `GET /api/ch05/comparison/create-script` |

## Як розрізнити згенерований SQL

- **TPH**: Поліморфний запит (`GET /api/ch05/tph/all`) читає одну таблицю `[Users_TPH]` без жодних JOIN чи UNION.
- **TPT**: Поліморфний запит (`GET /api/ch05/tpt/all`) генерує `LEFT JOIN` для кожної похідної таблиці.
- **TPC**: Поліморфний запит (`GET /api/ch05/tpc/all`) об'єднує таблиці за допомогою `UNION ALL`.
