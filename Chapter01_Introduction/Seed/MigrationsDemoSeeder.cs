using EfCoreExamples.Chapter01_Introduction.Models;

namespace EfCoreExamples.Chapter01_Introduction.Seed;

/// <summary>Наповнює БД уроку про міграції кількома товарами.</summary>
public static class MigrationsDemoSeeder
{
    public static void Seed(MigrationsDemoContext db)
    {
        if (db.Products.Any())
            return;

        db.Products.AddRange(
            new Product { Name = "Laptop",     Price = 45_000m, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Smartphone", Price = 28_000m, CreatedAt = DateTime.UtcNow },
            new Product { Name = "Headphones", Price =  3_500m, CreatedAt = DateTime.UtcNow });

        db.SaveChanges();
    }
}
