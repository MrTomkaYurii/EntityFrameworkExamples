using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Уроки 4.1 "Зовнішні ключі та навігаційні властивості", 4.2 "Налаштування FK"
/// та 4.8 "Відношення один до багатьох" — на прикладі <see cref="Blog"/> → <see cref="Post"/>.
/// </summary>
[ApiController]
[Route("api/ch04/one-to-many")]
[Tags("Глава 4 — Відношення між моделями")]
public class OneToManyController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public OneToManyController(RelationshipsContext db) => _db = db;

    /// <summary>
    /// 4.1–4.2 — як EF бачить зв'язок Post → Blog: зовнішній ключ, навігації,
    /// обов'язковість, поведінка при видаленні.
    /// </summary>
    [HttpGet("foreign-keys")]
    public ActionResult<object> ForeignKeys()
    {
        var post = _db.Model.FindEntityType(typeof(Post))!;

        return Ok(post.GetForeignKeys().Select(fk => new
        {
            dependent = fk.DeclaringEntityType.ClrType.Name,          // Post
            principal = fk.PrincipalEntityType.ClrType.Name,          // Blog
            foreignKeyProperties = fk.Properties.Select(p => p.Name), // [ "BlogId" ]
            isRequired = fk.IsRequired,
            dependentToPrincipalNav = fk.DependentToPrincipal?.Name,  // Post.Blog
            principalToDependentNav = fk.PrincipalToDependent?.Name,  // Blog.Posts
            deleteBehavior = fk.DeleteBehavior.ToString()
        }));
    }

    /// <summary>Список блогів із кількістю дописів (проєкція, без завантаження самих дописів).</summary>
    [HttpGet("blogs")]
    public async Task<ActionResult<object>> Blogs()
    {
        var blogs = await _db.Blogs
            .Select(b => new { b.Id, b.Name, postCount = b.Posts.Count })
            .ToListAsync();

        return Ok(blogs);
    }

    /// <summary>Один блог разом із дописами.</summary>
    [HttpGet("blogs/{id:int}")]
    public async Task<ActionResult<object>> BlogWithPosts(int id)
    {
        var blog = await _db.Blogs
            .Where(b => b.Id == id)
            .Select(b => new
            {
                b.Id,
                b.Name,
                posts = b.Posts.Select(p => new { p.Id, p.Title, p.Views })
            })
            .FirstOrDefaultAsync();

        return blog is null ? NotFound() : Ok(blog);
    }

    /// <summary>
    /// Додати допис до блогу. Досить встановити навігацію <c>Blog</c> (або <c>BlogId</c>) —
    /// EF сам проставить зовнішній ключ.
    /// </summary>
    [HttpPost("blogs/{id:int}/posts")]
    public async Task<ActionResult<object>> AddPost(int id, PostInput input)
    {
        var blog = await _db.Blogs.FindAsync(id);
        if (blog is null)
            return NotFound();

        var post = new Post { Title = input.Title, Blog = blog };
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return Ok(new { post.Id, post.Title, post.BlogId });
    }
}

public record PostInput(string Title);
