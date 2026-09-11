namespace EfCoreExamples.Chapter01_Introduction.ScaffoldedLike;

/// <summary>
/// Клас, який приблизно так згенерував би <c>dotnet ef dbcontext scaffold</c>
/// на основі вже наявної таблиці <c>Users</c> (підхід "Database First").
/// Написаний вручну, щоб не залежати від порядку створення БД під час навчання.
/// </summary>
public class AppUser
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }
}
