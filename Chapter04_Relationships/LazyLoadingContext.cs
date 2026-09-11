using EfCoreExamples.Chapter04_Relationships.LazyLoading;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships;

/// <summary>
/// Окремий контекст для уроку 4.6 "Lazy loading".
/// Ліниве завантаження вмикається в <c>Program.cs</c> викликом
/// <c>UseLazyLoadingProxies()</c>. Ця опція діє на весь контекст і вимагає,
/// щоб усі навігаційні властивості були <c>virtual</c> — тому вона винесена окремо.
/// </summary>
public class LazyLoadingContext : DbContext
{
    public LazyLoadingContext(DbContextOptions<LazyLoadingContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
}
