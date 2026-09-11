using EfCoreExamples.Chapter06_Queries.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter06_Queries;

/// <summary>
/// Контекст глави 6. Дані навмисно "багатші" (кілька компаній, десяток користувачів,
/// товари різних категорій) — щоб було на чому показувати фільтрацію, групування та агрегати.
/// </summary>
public class QueriesContext : DbContext
{
    public QueriesContext(DbContextOptions<QueriesContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId);

        modelBuilder.Entity<User>().Property(u => u.Salary).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

        // 6.10 — глобальний фільтр запитів: усі запити до User автоматично
        // отримують "WHERE IsDeleted = 0", доки явно не викликано IgnoreQueryFilters().
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}
