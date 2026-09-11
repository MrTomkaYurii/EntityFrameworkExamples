using EfCoreExamples.Chapter04_Relationships.Models;
using EfCoreExamples.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Уроки 4.4 "Завантаження пов'язаних даних. Include" та 4.5 "Explicit loading".
/// Кожен приклад повертає ще й згенерований SQL — видно, скільки запитів іде в БД.
/// </summary>
[ApiController]
[Route("api/ch04/loading")]
[Tags("Глава 4 — Відношення між моделями")]
public class LoadingRelatedDataController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public LoadingRelatedDataController(RelationshipsContext db) => _db = db;

    /// <summary>4.4 — <c>Include</c>: підвантажити дописи разом із блогами одним запитом (JOIN).</summary>
    [HttpGet("include")]
    public ActionResult<object> Include()
    {
        var query = _db.Blogs
            .Include(b => b.Posts)
            .Select(b => new { b.Name, posts = b.Posts.Select(p => p.Title) });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>4.4 — <c>ThenInclude</c>: спуститись глибше по ланцюжку навігацій.</summary>
    [HttpGet("then-include")]
    public async Task<ActionResult<object>> ThenInclude()
    {
        var students = await _db.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .Select(s => new
            {
                s.Name,
                enrollments = s.Enrollments.Select(e => new { e.Course!.Title, e.EnrolledOn })
            })
            .ToListAsync();

        return Ok(students);
    }

    /// <summary>4.4 — фільтрований <c>Include</c>: підвантажити лише частину пов'язаних записів.</summary>
    [HttpGet("filtered-include")]
    public ActionResult<object> FilteredInclude()
    {
        var query = _db.Blogs
            .Include(b => b.Posts.Where(p => p.Views > 500).OrderByDescending(p => p.Views))
            .Select(b => new { b.Name, popularPosts = b.Posts.Select(p => new { p.Title, p.Views }) });

        return Ok(query.ToSqlAndData());
    }

    /// <summary>
    /// 4.5 — explicit loading: спочатку завантажуємо блог БЕЗ дописів,
    /// а потім довантажуємо колекцію окремим запитом через <c>Entry(...).Collection(...).Load()</c>.
    /// </summary>
    [HttpGet("explicit/{blogId:int}")]
    public async Task<ActionResult<object>> Explicit(int blogId)
    {
        // Запит №1 — тільки блог.
        var blog = await _db.Blogs.FirstOrDefaultAsync(b => b.Id == blogId);
        if (blog is null)
            return NotFound();

        var postsBeforeLoad = blog.Posts.Count;   // 0 — ще не завантажено

        // Запит №2 — підвантажити пов'язану колекцію.
        await _db.Entry(blog).Collection(b => b.Posts).LoadAsync();

        return Ok(new
        {
            blog.Name,
            postsBeforeLoad,
            postsAfterLoad = blog.Posts.Count,
            posts = blog.Posts.Select(p => p.Title)
        });
    }
}
