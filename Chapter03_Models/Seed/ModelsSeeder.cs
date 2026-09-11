using EfCoreExamples.Chapter03_Models.Models;

namespace EfCoreExamples.Chapter03_Models.Seed;

/// <summary>
/// Наповнює сутності, які НЕ зручно задавати через <c>HasData</c>
/// (наприклад, <see cref="Product"/> має обчислюваний стовпець).
/// <para>
/// <see cref="AppSetting"/> і <see cref="Category"/> сюди не входять — їхні
/// початкові дані описані декларативно через <c>HasData</c>
/// (див. <c>ModelsContext.OnModelCreating</c> та <c>CategoryConfiguration</c>).
/// </para>
/// </summary>
public static class ModelsSeeder
{
    public static void Seed(ModelsContext db)
    {
        if (db.Products.Any())
            return;

        db.Products.AddRange(
            new Product { Name = "Keyboard", Price = 1200m, Sku = "KB-001" },
            new Product { Name = "Mouse", Price = 750m, Sku = "MS-002" },
            new Product { Name = "Monitor", Price = 8900m, Sku = "MN-003" });

        db.Customers.AddRange(
            new Customer { FullName = "Tom Sawyer", Email = "tom@example.com", Phone = "+380501112233" },
            new Customer { FullName = "Alice Liddell", Email = "alice@example.com", Phone = null },
            new Customer { FullName = "Bob Ross", Email = "bob@example.com", Phone = null });

        db.OrderLines.AddRange(
            new OrderLine { OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrderLine { OrderId = 1, ProductId = 2, Quantity = 1 });

        var article = new Article(0, "Вступ до EF Core");
        article.RegisterView();
        article.RegisterView();
        db.Articles.Add(article);

        db.SaveChanges();
    }
}
