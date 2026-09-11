using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Infrastructure;

/// <summary>
/// Презентаційний хелпер для навчальних прикладів: показує SQL, який EF Core
/// згенерує для запиту, поряд із самим результатом. Це НЕ бізнес-логіка —
/// потрібно лише для наочності, щоб бачити, у що перетворюється LINQ-вираз.
/// </summary>
public static class QueryPresentation
{
    /// <summary>
    /// Виконує <paramref name="query"/> і повертає об'єкт { sql, count, data }.
    /// <para><c>sql</c> — рядок з методу <see cref="EntityFrameworkQueryableExtensions.ToQueryString"/>:
    /// саме цей текст EF Core відправить у базу даних.</para>
    /// </summary>
    public static QueryResult<T> ToSqlAndData<T>(this IQueryable<T> query)
    {
        // ToQueryString() лише формує текст запиту — звернення до БД не відбувається.
        var sql = query.ToQueryString();

        // А ось тут запит реально виконується.
        var data = query.ToList();

        return new QueryResult<T>(sql, data.Count, data);
    }
}

/// <summary>Результат навчального запиту: згенерований SQL + отримані рядки.</summary>
/// <param name="Sql">Текст запиту, який EF Core відправляє в базу даних.</param>
/// <param name="Count">Кількість отриманих рядків.</param>
/// <param name="Data">Самі рядки.</param>
public record QueryResult<T>(string Sql, int Count, IReadOnlyList<T> Data);
