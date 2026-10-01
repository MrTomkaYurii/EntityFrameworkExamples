using EfCoreExamples.Chapter08_Advanced.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter08_Advanced.Seed;

/// <summary>
/// Наповнює базу даних глави 8 початковими записами та створює подання (View).
/// </summary>
public static class AdvancedSeeder
{
    public static void Seed(AdvancedContext db)
    {
        // ── 1. Початкові банківські рахунки ───────────────────────────────────
        if (!db.BankAccounts.Any())
        {
            db.BankAccounts.AddRange(
                new BankAccount { AccountNumber = "UA001", OwnerName = "Тарас Шевченко", Balance = 1500m },
                new BankAccount { AccountNumber = "UA002", OwnerName = "Тарас Шевченко", Balance = 3500m },
                new BankAccount { AccountNumber = "UA003", OwnerName = "Леся Українка", Balance = 4200m },
                new BankAccount { AccountNumber = "UA004", OwnerName = "Іван Франко", Balance = 2800m });
        }

        // ── 2. Початкові документи (темпоральна таблиця) ──────────────────────
        if (!db.Documents.Any())
        {
            db.Documents.AddRange(
                new Document
                {
                    Title = "Архітектурний маніфест",
                    Content = "Версія 1.0: базові принципи побудови застосунку.",
                    Author = "Lead Architect"
                },
                new Document
                {
                    Title = "Політика безпеки",
                    Content = "Версія 1.0: правила доступу до сервісів.",
                    Author = "Security Team"
                });
        }

        db.SaveChanges();

        // ── 3. Створення подання (View) для уроку 8.2 ─────────────────────────
        db.Database.ExecuteSqlRaw("""
            CREATE OR ALTER VIEW dbo.V_AccountSummary AS
            SELECT 
                OwnerName, 
                COUNT(*) AS AccountCount, 
                SUM(Balance) AS TotalBalance
            FROM BankAccounts
            GROUP BY OwnerName;
            """);
    }

    public static void Reset(AdvancedContext db)
    {
        db.BankAccounts.RemoveRange(db.BankAccounts);
        db.Documents.RemoveRange(db.Documents);
        db.SaveChanges();

        Seed(db);
    }
}
