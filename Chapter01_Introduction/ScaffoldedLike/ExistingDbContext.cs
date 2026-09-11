using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.ScaffoldedLike;

/// <summary>
/// Урок 1.3 "Підключення до наявної бази даних" (підхід Database First).
/// <para>
/// Замість того щоб описувати модель у коді й створювати БД з неї, тут ми
/// навпаки — підключаємось до вже наявної бази <c>EfCoreExamples_Ch01_Introduction</c>
/// (її створює <see cref="IntroductionContext"/>) і працюємо з існуючою таблицею.
/// </para>
/// <para>
/// У реальному проєкті цей клас і сутності згенерував би інструмент реверс-інжинірингу:
/// </para>
/// <code>
/// dotnet ef dbcontext scaffold ^
///   "Server=(localdb)\MSSQLLocalDB;Database=EfCoreExamples_Ch01_Introduction;Trusted_Connection=True;TrustServerCertificate=True" ^
///   Microsoft.EntityFrameworkCore.SqlServer ^
///   --context ExistingDbContext ^
///   --output-dir Chapter01_Introduction/ScaffoldedLike ^
///   --no-onconfiguring
/// </code>
/// <para>Прапорець <c>--no-onconfiguring</c> прибирає рядок підключення з коду —
/// ми передаємо його ззовні через <c>AddDbContext</c>, як і решту контекстів.</para>
/// </summary>
public class ExistingDbContext : DbContext
{
    public ExistingDbContext(DbContextOptions<ExistingDbContext> options)
        : base(options)
    {
    }

    /// <summary>Та сама фізична таблиця "Users", але через окрему модель.</summary>
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Явно закріплюємо ім'я таблиці — саме це зробив би scaffold.
        modelBuilder.Entity<AppUser>().ToTable("Users");
    }
}
