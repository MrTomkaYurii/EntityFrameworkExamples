using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter03_Models;

/// <summary>
/// Контекст глави 3. Показує обидва способи налаштування моделі:
/// анотації даних (на класах у теці <c>Models</c>) і Fluent API (у методі нижче).
/// </summary>
public class ModelsContext : DbContext
{
    public ModelsContext(DbContextOptions<ModelsContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<AppSetting> Settings => Set<AppSetting>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Product: зіставлення, обмеження, генерація значень ────────────────
        modelBuilder.Entity<Product>(product =>
        {
            // 3.6 — таблиця у власній схемі БД.
            product.ToTable("Products", schema: "catalog");

            // 3.11 — точність decimal: 18 цифр усього, 2 після коми.
            product.Property(p => p.Price).HasPrecision(18, 2);

            // 3.9 — унікальний індекс по артикулу.
            product.HasIndex(p => p.Sku).IsUnique();

            // 3.10 — значення за замовчуванням рахує СУБД під час INSERT.
            product.Property(p => p.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // 3.10 — обчислюваний стовпець. stored: true — значення зберігається фізично.
            product.Property(p => p.PriceWithVat).HasComputedColumnSql("[Price] * 1.20", stored: true);

            // 3.11 — перевірка на рівні БД (CHECK constraint).
            product.ToTable(t => t.HasCheckConstraint("CK_Products_Price_NonNegative", "[Price] >= 0"));
        });

        // ── Customer: альтернативний ключ та фільтрований унікальний індекс ───
        modelBuilder.Entity<Customer>(customer =>
        {
            // 3.8 — альтернативний ключ (не первинний, але теж унікальний і не null).
            customer.HasAlternateKey(c => c.Email);

            customer.Property(c => c.FullName).IsRequired().HasMaxLength(150);

            // 3.9 — унікальний індекс лише для рядків, де телефон заданий.
            customer.HasIndex(c => c.Phone)
                .IsUnique()
                .HasFilter("[Phone] IS NOT NULL");
        });

        // ── OrderLine: складений первинний ключ ──────────────────────────────
        modelBuilder.Entity<OrderLine>()
            .HasKey(line => new { line.OrderId, line.ProductId });   // 3.8

        // ── Article: поле як стан сутності ──────────────────────────────────
        modelBuilder.Entity<Article>()
            .Property(a => a.ViewCount)
            .HasField("_viewCount");                                 // 3.5

        // ── AppSetting: початкові дані через HasData ─────────────────────────
        modelBuilder.Entity<AppSetting>().HasData(                   // 3.13
            new AppSetting { Key = "site.title", Value = "EF Core Examples" },
            new AppSetting { Key = "site.pageSize", Value = "20" },
            new AppSetting { Key = "feature.darkMode", Value = "true" });

        // ── Category: налаштування винесене в окремий клас ───────────────────
        // ApplyConfigurationsFromAssembly знаходить усі IEntityTypeConfiguration<T>
        // у збірці й застосовує їх. Предикат обмежує пошук лише текою цієї глави,
        // щоб не зачепити конфігурації інших глав у тому самому проєкті.
        modelBuilder.ApplyConfigurationsFromAssembly(                 // 3.12
            typeof(ModelsContext).Assembly,
            predicate: t => t.Namespace == typeof(Configurations.CategoryConfiguration).Namespace);
    }
}
