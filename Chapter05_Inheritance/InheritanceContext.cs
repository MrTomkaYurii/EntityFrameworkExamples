using EfCoreExamples.Chapter05_Inheritance.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter05_Inheritance;

/// <summary>
/// Контекст глави 5 "Успадкування".
/// Демонструє всі три стратегії зіставлення ієрархій класів на реляційну БД:
/// 1. TPH (Table Per Hierarchy) — одна таблиця на всю ієрархію зі стовпцем-дискримінатором (за замовчуванням).
/// 2. TPT (Table Per Type) — окрема таблиця для кожного типу (FK на таблицю предка).
/// 3. TPC (Table Per Class) — окрема таблиця для кожного конкретного нащадка (EF Core 7+).
/// </summary>
public class InheritanceContext : DbContext
{
    public InheritanceContext(DbContextOptions<InheritanceContext> options) : base(options)
    {
    }

    // ── TPH (Table Per Hierarchy) ─────────────────────────────────────────────
    public DbSet<UserTph> UsersTph => Set<UserTph>();
    public DbSet<EmployeeTph> EmployeesTph => Set<EmployeeTph>();
    public DbSet<ManagerTph> ManagersTph => Set<ManagerTph>();

    // ── TPT (Table Per Type) ──────────────────────────────────────────────────
    public DbSet<BillingAccountTpt> AccountsTpt => Set<BillingAccountTpt>();
    public DbSet<CreditAccountTpt> CreditAccountsTpt => Set<CreditAccountTpt>();
    public DbSet<DepositAccountTpt> DepositAccountsTpt => Set<DepositAccountTpt>();

    // ── TPC (Table Per Class) ─────────────────────────────────────────────────
    public DbSet<DeviceTpc> DevicesTpc => Set<DeviceTpc>();
    public DbSet<SmartphoneTpc> SmartphonesTpc => Set<SmartphoneTpc>();
    public DbSet<LaptopTpc> LaptopsTpc => Set<LaptopTpc>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── 5.1 TPH — Table Per Hierarchy ────────────────────────────────────
        // За замовчуванням EF Core обирає TPH. Усі класи (User, Employee, Manager)
        // зберігаються в одній таблиці Users_TPH. Стовпець "UserType" виступає дискримінатором.
        modelBuilder.Entity<UserTph>(user =>
        {
            user.ToTable("Users_TPH");
            user.HasDiscriminator<string>("UserType")
                .HasValue<UserTph>("User")
                .HasValue<EmployeeTph>("Employee")
                .HasValue<ManagerTph>("Manager");
        });

        modelBuilder.Entity<EmployeeTph>()
            .Property(e => e.Salary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ManagerTph>()
            .Property(m => m.AnnualBonus)
            .HasPrecision(18, 2);

        // ── 5.2 TPT — Table Per Type ─────────────────────────────────────────
        // Кожен клас має власну фізичну таблицю. Первинний ключ таблиць-нащадків
        // є водночас зовнішнім ключем (FK) на базову таблицю.
        modelBuilder.Entity<BillingAccountTpt>(account =>
        {
            account.ToTable("BillingAccounts_TPT");
            account.Property(a => a.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<CreditAccountTpt>(credit =>
        {
            credit.ToTable("CreditAccounts_TPT");
            credit.Property(c => c.CreditLimit).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DepositAccountTpt>(deposit =>
        {
            deposit.ToTable("DepositAccounts_TPT");
            deposit.Property(d => d.InterestRate).HasPrecision(5, 2);
        });

        // ── 5.3 TPC — Table Per Class ─────────────────────────────────────────
        // Додано в EF Core 7. Базовий клас DeviceTpc не має власної таблиці.
        // Кожен конкретний нащадок (Smartphone, Laptop) отримує незалежну таблицю
        // з повним набором стовпців. Для генерації ключів використовується спільна послідовність (HiLo/Sequence).
        modelBuilder.Entity<DeviceTpc>(device =>
        {
            device.UseTpcMappingStrategy();
            device.Property(d => d.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SmartphoneTpc>().ToTable("Smartphones_TPC");
        modelBuilder.Entity<LaptopTpc>().ToTable("Laptops_TPC");
    }
}
