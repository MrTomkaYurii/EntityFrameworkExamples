# Глава 2. Провайдери баз даних

Відповідає розділу [metanit — Провайдеры баз данных](https://metanit.com/sharp/efcore/7.1.php).

У цьому проєкті всі робочі приклади розгорнуті на **Microsoft SQL Server (LocalDB)**,
оскільки LocalDB постачається разом із Visual Studio та .NET і не потребує зовнішніх служб чи Docker.
Водночас ця глава демонструє конфігурацію, відмінності та особливості основних провайдерів EF Core,
про які йдеться у курсі Metanit.

| Урок metanit | Демонструє | Провайдер / Пакет | Ендпоінти |
|---|---|---|---|
| 2.1 MS SQL Server | Підключення, стійкість з'єднання (`EnableRetryOnFailure`), тайм-аути | `Microsoft.EntityFrameworkCore.SqlServer` | `GET /api/ch02/providers/current`, `GET /api/ch02/providers/resilience` |
| 2.2 MySQL | Налаштування через Pomelo, визначення версії сервера | `Pomelo.EntityFrameworkCore.MySql` | `GET /api/ch02/providers/catalog` |
| 2.3 PostgreSQL | Налаштування через Npgsql, мапінг типів | `Npgsql.EntityFrameworkCore.PostgreSQL` | `GET /api/ch02/providers/catalog` |

## Як EF Core взаємодіє з провайдерами

EF Core є незалежним від конкретної СУБД ядром (`Microsoft.EntityFrameworkCore`).
Кожен провайдер реалізує адаптери для генерації діалекту SQL, мапінгу специфічних типів даних
та виконання команд над відповідною клієнтською бібліотекою (SqlClient, Npgsql, MySqlConnector).

```csharp
// MS SQL Server
options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDb;Trusted_Connection=True;");

// PostgreSQL
options.UseNpgsql("Host=localhost;Database=mydb;Username=postgres;Password=secret");

// MySQL
options.UseMySql(
    "Server=localhost;Database=mydb;User=root;Password=secret;",
    ServerVersion.AutoDetect(connectionString));
```
