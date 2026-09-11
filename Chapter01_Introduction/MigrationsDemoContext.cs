using EfCoreExamples.Chapter01_Introduction.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction;

/// <summary>
/// Окремий контекст для уроку 1.8 "Управління схемою БД та міграції".
/// <para>
/// На відміну від решти глав, ця БД створюється НЕ через <c>EnsureCreated()</c>,
/// а через справжні міграції. Файли міграцій лежать у теці <c>Migrations/</c>
/// і згенеровані командою:
/// </para>
/// <code>
/// dotnet ef migrations add Initial --context MigrationsDemoContext -o Chapter01_Introduction/Migrations
/// dotnet ef migrations add AddProductCreatedAt --context MigrationsDemoContext -o Chapter01_Introduction/Migrations
/// </code>
/// <para>
/// Застосувати міграції можна командою <c>dotnet ef database update --context MigrationsDemoContext</c>
/// або програмно — <c>Database.Migrate()</c> (див. <c>DatabaseBootstrapper</c> та <c>MigrationsController</c>).
/// </para>
/// </summary>
public class MigrationsDemoContext : DbContext
{
    public MigrationsDemoContext(DbContextOptions<MigrationsDemoContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
