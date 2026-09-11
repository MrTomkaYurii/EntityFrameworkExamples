using EfCoreExamples.Chapter03_Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreExamples.Chapter03_Models.Configurations;

/// <summary>
/// Урок 3.12 "Конфігурація моделей".
/// Замість того щоб складати всі налаштування в один довгий <c>OnModelCreating</c>,
/// їх можна розкласти по класах — по одному на сутність. Контекст підхоплює їх
/// автоматично через <c>ApplyConfigurationsFromAssembly</c>.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name).IsUnique();

        // Урок 3.13 (частина): початкові дані для Category.
        builder.HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Гаджети та техніка" },
            new Category { Id = 2, Name = "Books", Description = "Паперові та електронні книги" },
            new Category { Id = 3, Name = "Home", Description = "Товари для дому" });
    }
}
