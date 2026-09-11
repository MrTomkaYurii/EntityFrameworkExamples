using EfCoreExamples.Chapter01_Introduction.Models;

namespace EfCoreExamples.Chapter01_Introduction.Seed;

/// <summary>Наповнює БД глави 1 початковими даними, якщо таблиця порожня.</summary>
public static class IntroductionSeeder
{
    public static void Seed(IntroductionContext db)
    {
        // Ідемпотентність: якщо дані вже є — нічого не робимо.
        if (db.Users.Any())
            return;

        db.Users.AddRange(
            new User { Name = "Tom", Age = 33 },
            new User { Name = "Alice", Age = 26 },
            new User { Name = "Bob", Age = 41 },
            new User { Name = "Kate", Age = 29 },
            new User { Name = "Sam", Age = 37 });

        db.SaveChanges();
    }
}
