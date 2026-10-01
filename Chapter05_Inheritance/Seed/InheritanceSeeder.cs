using EfCoreExamples.Chapter05_Inheritance.Models;

namespace EfCoreExamples.Chapter05_Inheritance.Seed;

/// <summary>
/// Наповнює базу даних глави 5 початковими даними для всіх трьох стратегій успадкування.
/// </summary>
public static class InheritanceSeeder
{
    public static void Seed(InheritanceContext db)
    {
        // ── 1. TPH ────────────────────────────────────────────────────────────
        if (!db.UsersTph.Any())
        {
            db.UsersTph.AddRange(
                new UserTph
                {
                    Name = "Alice Brown",
                    Email = "alice@example.com"
                },
                new EmployeeTph
                {
                    Name = "Bob Smith",
                    Email = "bob@example.com",
                    Company = "Google",
                    Salary = 3200m
                },
                new ManagerTph
                {
                    Name = "Charlie Davis",
                    Email = "charlie@example.com",
                    Company = "Google",
                    Salary = 5500m,
                    Department = "Cloud Platform",
                    AnnualBonus = 2000m
                });
        }

        // ── 2. TPT ────────────────────────────────────────────────────────────
        if (!db.AccountsTpt.Any())
        {
            db.AccountsTpt.AddRange(
                new BillingAccountTpt
                {
                    Owner = "Dave Miller",
                    Balance = 750m
                },
                new CreditAccountTpt
                {
                    Owner = "Eva Green",
                    Balance = 2400m,
                    CreditLimit = 5000m
                },
                new DepositAccountTpt
                {
                    Owner = "Frank White",
                    Balance = 12000m,
                    InterestRate = 12.5m
                });
        }

        // ── 3. TPC ────────────────────────────────────────────────────────────
        if (!db.DevicesTpc.Any())
        {
            db.DevicesTpc.AddRange(
                new SmartphoneTpc
                {
                    Model = "Pixel 9 Pro",
                    Price = 999m,
                    OperatingSystem = "Android 15"
                },
                new SmartphoneTpc
                {
                    Model = "iPhone 16 Pro",
                    Price = 1199m,
                    OperatingSystem = "iOS 18"
                },
                new LaptopTpc
                {
                    Model = "ThinkPad X1 Carbon",
                    Price = 1850m,
                    RamGigabytes = 32
                },
                new LaptopTpc
                {
                    Model = "MacBook Pro 16",
                    Price = 2499m,
                    RamGigabytes = 36
                });
        }

        db.SaveChanges();
    }

    public static void Reset(InheritanceContext db)
    {
        db.UsersTph.RemoveRange(db.UsersTph);
        db.AccountsTpt.RemoveRange(db.AccountsTpt);
        db.DevicesTpc.RemoveRange(db.DevicesTpc);
        db.SaveChanges();

        Seed(db);
    }
}
