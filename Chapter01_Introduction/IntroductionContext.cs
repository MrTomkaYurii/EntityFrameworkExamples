using EfCoreExamples.Chapter01_Introduction.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction;

/// <summary>
/// Контекст даних глави 1 — клас-посередник між кодом і базою даних.
/// <para>
/// Кожна властивість <see cref="DbSet{TEntity}"/> відповідає таблиці.
/// Налаштування підключення (провайдер + рядок підключення) задаєтьсяззовні,
/// у <c>Program.cs</c> через <c>AddDbContext</c> — тому конструктор приймає
/// готові <see cref="DbContextOptions"/>, а метод <c>OnConfiguring</c> тут не потрібен.
/// </para>
/// </summary>
public class IntroductionContext : DbContext
{
    public IntroductionContext(DbContextOptions<IntroductionContext> options)
        : base(options)
    {
    }

    /// <summary>Таблиця користувачів (за домовленістю — "Users").</summary>
    public DbSet<User> Users => Set<User>();
}
