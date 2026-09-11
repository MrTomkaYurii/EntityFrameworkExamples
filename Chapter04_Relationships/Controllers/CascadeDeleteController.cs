using EfCoreExamples.Chapter04_Relationships.Models;
using EfCoreExamples.Chapter04_Relationships.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.3 "Каскадне видалення". Три сценарії:
/// <list type="bullet">
///   <item><b>Cascade</b> — <see cref="Blog"/> → <see cref="Post"/> (обов'язковий зв'язок);</item>
///   <item><b>SetNull</b> — <see cref="Company"/> → <see cref="User"/> (необов'язковий зв'язок);</item>
///   <item><b>Restrict</b> — <see cref="Department"/> → <see cref="Project"/> (видалення заборонене).</item>
/// </list>
/// Після дослідів виклич <c>POST /api/ch04/cascade-delete/reset</c>.
/// </summary>
[ApiController]
[Route("api/ch04/cascade-delete")]
[Tags("Глава 4 — Відношення між моделями")]
public class CascadeDeleteController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public CascadeDeleteController(RelationshipsContext db) => _db = db;

    /// <summary>Поведінка при видаленні для кожного зв'язку в моделі.</summary>
    [HttpGet("behaviors")]
    public ActionResult<object> Behaviors()
    {
        var result = _db.Model.GetEntityTypes()
            .Where(e => !e.IsOwned())   // власні типи мають технічний каскад до власника — пропускаємо
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => !fk.DeclaringEntityType.IsOwned())
            .Select(fk => new
            {
                relationship = $"{fk.PrincipalEntityType.ClrType.Name} → {fk.DeclaringEntityType.ClrType.Name}",
                required = fk.IsRequired,
                onDelete = fk.DeleteBehavior.ToString()
            });

        return Ok(result);
    }

    /// <summary>Cascade: видаляємо блог — БД сама видаляє його дописи.</summary>
    [HttpDelete("blogs/{id:int}")]
    public async Task<ActionResult<object>> DeleteBlog(int id)
    {
        var blog = await _db.Blogs.Include(b => b.Posts).FirstOrDefaultAsync(b => b.Id == id);
        if (blog is null)
            return NotFound();

        var postCount = blog.Posts.Count;
        _db.Blogs.Remove(blog);
        await _db.SaveChangesAsync();

        return Ok(new { deletedBlog = blog.Name, alsoDeletedPosts = postCount });
    }

    /// <summary>SetNull: видаляємо компанію — у її працівників <c>CompanyId</c> стає <c>null</c>.</summary>
    [HttpDelete("companies/{id:int}")]
    public async Task<ActionResult<object>> DeleteCompany(int id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company is null)
            return NotFound();

        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();

        var orphaned = await _db.Users
            .Where(u => u.CompanyId == null)
            .Select(u => u.Name)
            .ToListAsync();

        return Ok(new { deletedCompany = company.Name, usersNowWithoutCompany = orphaned });
    }

    /// <summary>Restrict: видалити відділ із проєктами не вийде — БД поверне помилку.</summary>
    [HttpDelete("departments/{id:int}")]
    public async Task<ActionResult<object>> DeleteDepartment(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department is null)
            return NotFound();

        _db.Departments.Remove(department);

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { deletedDepartment = department.Name });
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new
            {
                note = "Відділ має проєкти, а зв'язок налаштований як Restrict.",
                error = ex.GetBaseException().Message
            });
        }
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        RelationshipsSeeder.Seed(_db);
        return Ok(new { message = "Дані глави 4 відновлено." });
    }
}
