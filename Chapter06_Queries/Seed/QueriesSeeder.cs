using EfCoreExamples.Chapter06_Queries.Models;

namespace EfCoreExamples.Chapter06_Queries.Seed;

public static class QueriesSeeder
{
    public static void Seed(QueriesContext db)
    {
        if (db.Companies.Any())
            return;

        var microsoft = new Company { Name = "Microsoft", Country = "USA" };
        var google = new Company { Name = "Google", Country = "USA" };
        var spotify = new Company { Name = "Spotify", Country = "Sweden" };
        db.Companies.AddRange(microsoft, google, spotify);
        db.SaveChanges();

        db.Users.AddRange(
            new User { Name = "Tom", Age = 33, Position = "Developer", Salary = 5000, Company = microsoft },
            new User { Name = "Bob", Age = 41, Position = "Manager", Salary = 6200, Company = microsoft },
            new User { Name = "Sam", Age = 28, Position = "Developer", Salary = 4300, Company = microsoft },
            new User { Name = "Kate", Age = 25, Position = "Designer", Salary = 3900, Company = google },
            new User { Name = "Alice", Age = 30, Position = "Developer", Salary = 5100, Company = google },
            new User { Name = "Mike", Age = 45, Position = "Manager", Salary = 7000, Company = google },
            new User { Name = "Emma", Age = 38, Position = "Developer", Salary = 5600, Company = spotify },
            new User { Name = "Liam", Age = 27, Position = "Designer", Salary = 4100, Company = spotify },
            new User { Name = "Olivia", Age = 34, Position = "Manager", Salary = 6800, Company = spotify },
            // "видалений" користувач — глобальний фільтр приховає його від звичайних запитів
            new User { Name = "Ghost", Age = 99, Position = "Developer", Salary = 0, Company = microsoft, IsDeleted = true });

        db.Products.AddRange(
            new Product { Title = "Keyboard", Category = "Peripherals", Price = 1200, InStock = true },
            new Product { Title = "Mouse", Category = "Peripherals", Price = 750, InStock = true },
            new Product { Title = "Monitor 27\"", Category = "Displays", Price = 8900, InStock = true },
            new Product { Title = "Monitor 32\"", Category = "Displays", Price = 13500, InStock = false },
            new Product { Title = "USB-C Cable", Category = "Accessories", Price = 250, InStock = true },
            new Product { Title = "Laptop Stand", Category = "Accessories", Price = 900, InStock = false });

        db.SaveChanges();
    }
}
