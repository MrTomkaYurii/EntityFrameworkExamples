using EfCoreExamples.Chapter08_Advanced.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced;

/// <summary>
/// Контекст глави 8 "Додаткові статті та просунуті можливості".
/// Демонструє оптимістичний паралелізм (Concurrency), скомпільовані запити,
/// проєкцію на подання (Views) та темпоральні таблиці (Temporal Tables).
/// </summary>
public class AdvancedContext : DbContext
{
    public AdvancedContext(DbContextOptions<AdvancedContext> options) : base(options)
    {
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<AccountSummaryView> AccountSummaries => Set<AccountSummaryView>();
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── 8.1 Оптимістичний паралелізм (Concurrency Token / RowVersion) ─────
        // Стовпець RowVersion отримує тип даних rowversion у SQL Server.
        // При SaveChanges EF Core генерує: WHERE [Id] = @p0 AND [RowVersion] = @p1.
        // Якщо інший потік встиг змінити рядок, кількість оновлених записів = 0,
        // і EF Core викидає DbUpdateConcurrencyException.
        modelBuilder.Entity<BankAccount>(b =>
        {
            b.Property(x => x.Balance).HasPrecision(18, 2);
            b.Property(x => x.RowVersion).IsRowVersion();
        });

        // ── 8.2 Проєкція на подання (Database Views) ──────────────────────────
        // Сутність без первинного ключа (HasNoKey), зіставлена з об'єктом VIEW СУБД.
        modelBuilder.Entity<AccountSummaryView>(v =>
        {
            v.HasNoKey();
            v.ToView("V_AccountSummary");
            v.Property(x => x.TotalBalance).HasPrecision(18, 2);
        });

        // ── 8.3 Темпоральні таблиці (Temporal Tables) ──────────────────────────
        // Вмикає вбудоване системне версіонування SQL Server.
        // EF Core автоматично додає тіньові стовпці PeriodStart та PeriodEnd
        // і створює окрему таблицю історії змін.
        modelBuilder.Entity<Document>(d =>
        {
            d.ToTable("Documents", tb => tb.IsTemporal());
        });
    }
}
