using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EfCoreExamples.Chapter01_Introduction;

/// <summary>
/// Фабрика контексту для DESIGN-TIME — її використовує інструмент <c>dotnet ef</c>,
/// коли створює чи застосовує міграції (застосунок при цьому не запущений).
/// <para>
/// Без цієї фабрики <c>dotnet ef</c> намагався б підняти весь веб-хост
/// (<c>Program.cs</c>), що для навчального прикладу зайве. Рядок підключення
/// тут заданий напряму — це окремий, ізольований від рантайму шлях.
/// </para>
/// </summary>
public class MigrationsDemoContextFactory : IDesignTimeDbContextFactory<MigrationsDemoContext>
{
    public MigrationsDemoContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MigrationsDemoContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EfCoreExamples_Ch01_Migrations;" +
                          "Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new MigrationsDemoContext(options);
    }
}
