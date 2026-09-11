using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships;

/// <summary>
/// Контекст глави 4. Містить усі види зв'язків між сутностями.
/// Ліниве завантаження (4.6) винесене в окремий <see cref="LazyLoadingContext"/>.
/// </summary>
public class RelationshipsContext : DbContext
{
    public RelationshipsContext(DbContextOptions<RelationshipsContext> options) : base(options)
    {
    }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> Profiles => Set<UserProfile>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Blog → Post: обов'язковий зв'язок, каскадне видалення за замовчуванням (4.1, 4.3, 4.8)
        modelBuilder.Entity<Blog>()
            .HasMany(b => b.Posts)
            .WithOne(p => p.Blog)
            .HasForeignKey(p => p.BlogId)
            .OnDelete(DeleteBehavior.Cascade);   // явно, хоча це і є значення за замовчуванням

        // ── Company → User: необов'язковий зв'язок, при видаленні компанії FK обнуляється (4.3)
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // ── Department → Project: видалення заборонене, поки є пов'язані проєкти (4.3)
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Projects)
            .WithOne(p => p.Department)
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── User ↔ UserProfile: один до одного (4.7)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId);   // залежна сторона — UserProfile

        // ── Student ↔ Course: багато до багатьох через явну сутність Enrollment (4.9)
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Courses)
            .WithMany(c => c.Students)
            .UsingEntity<Enrollment>(
                right => right.HasOne(e => e.Course).WithMany(c => c.Enrollments),
                left => left.HasOne(e => e.Student).WithMany(s => s.Enrollments),
                join => join.HasKey(e => new { e.StudentId, e.CourseId }));

        // ── Order: власні типи (4.10)
        modelBuilder.Entity<Order>().OwnsOne(o => o.ShippingAddress);   // у ту саму таблицю Orders
        modelBuilder.Entity<Order>().OwnsMany(o => o.Lines);            // в окрему таблицю Order_Lines

        // ── Customer: комплексний тип (4.11)
        modelBuilder.Entity<Customer>().ComplexProperty(c => c.Address);

        // ── Employee → Employee: ієрархія (4.12).
        // На SQL Server самопосилання не може бути каскадним — інакше "цикл каскадів".
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Reports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
