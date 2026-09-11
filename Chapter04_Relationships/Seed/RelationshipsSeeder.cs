using EfCoreExamples.Chapter04_Relationships.Models;

namespace EfCoreExamples.Chapter04_Relationships.Seed;

/// <summary>Наповнює всі приклади глави 4 узгодженим набором пов'язаних даних.</summary>
public static class RelationshipsSeeder
{
    public static void Seed(RelationshipsContext db)
    {
        if (db.Blogs.Any())
            return;

        // ── Blog → Post ─────────────────────────────────────────────────────
        db.Blogs.AddRange(
            new Blog
            {
                Name = ".NET Blog",
                Posts =
                {
                    new Post { Title = "What's new in EF Core 10", Views = 1200 },
                    new Post { Title = "Minimal APIs tips", Views = 800 }
                }
            },
            new Blog
            {
                Name = "SQL Corner",
                Posts = { new Post { Title = "Window functions", Views = 430 } }
            });

        // ── Company → User ──────────────────────────────────────────────────
        var microsoft = new Company { Name = "Microsoft" };
        var google = new Company { Name = "Google" };
        db.Companies.AddRange(microsoft, google);
        db.SaveChanges();   // зберігаємо покроково, щоб Id у прикладах були передбачувані

        db.Users.AddRange(
            new User { Name = "Tom", Age = 33, Company = microsoft },
            new User { Name = "Bob", Age = 41, Company = microsoft },
            new User { Name = "Sam", Age = 28, Company = google },
            new User { Name = "Kate", Age = 25, Company = null },    // без компанії — дозволено
            // ── User → UserProfile (один до одного) ─────────────────────────
            new User
            {
                Name = "Alice",
                Age = 30,
                Company = google,
                Profile = new UserProfile { Bio = "Backend developer", Website = "https://alice.dev" }
            });
        db.SaveChanges();

        // ── Department → Project (Restrict) ─────────────────────────────────
        db.Departments.AddRange(
            new Department
            {
                Name = "R&D",
                Projects = { new Project { Title = "Compiler" }, new Project { Title = "Runtime" } }
            },
            new Department { Name = "Marketing" });   // без проєктів — цей відділ можна буде видалити

        // ── Student ↔ Course (багато до багатьох) ───────────────────────────
        var math = new Course { Title = "Mathematics" };
        var physics = new Course { Title = "Physics" };
        var history = new Course { Title = "History" };
        db.Courses.AddRange(math, physics, history);

        var john = new Student { Name = "John", Courses = { math, physics } };
        var mary = new Student { Name = "Mary", Courses = { physics, history } };
        db.Students.AddRange(john, mary);

        // ── Order з власними типами ─────────────────────────────────────────
        db.Orders.Add(new Order
        {
            Number = "ORD-1001",
            ShippingAddress = new OrderAddress { City = "Kyiv", Street = "Khreshchatyk 1", Zip = "01001" },
            Lines =
            {
                new OrderItem { Product = "Keyboard", Quantity = 1, Price = 1200m },
                new OrderItem { Product = "Mouse", Quantity = 2, Price = 750m }
            }
        });

        // ── Customer із комплексним типом ──────────────────────────────────
        db.Customers.Add(new Customer
        {
            Name = "Acme LLC",
            Address = new Address { City = "Lviv", Street = "Rynok 10", Zip = "79000" }
        });

        // ── Employee: ієрархія ─────────────────────────────────────────────
        var ceo = new Employee { Name = "Diana", Position = "CEO" };
        var cto = new Employee { Name = "Erik", Position = "CTO", Manager = ceo };
        var dev1 = new Employee { Name = "Frank", Position = "Developer", Manager = cto };
        var dev2 = new Employee { Name = "Grace", Position = "Developer", Manager = cto };
        db.Employees.AddRange(ceo, cto, dev1, dev2);

        db.SaveChanges();

        // Дати запису на курси — окремим кроком, коли вже є ключі.
        foreach (var enrollment in db.Set<Enrollment>())
            enrollment.EnrolledOn = new DateOnly(2026, 9, 1);
        db.SaveChanges();
    }
}
