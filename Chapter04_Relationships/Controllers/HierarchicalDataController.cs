using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.12 "Ієрархічні дані" — <see cref="Employee"/> посилається сам на себе
/// (<c>ManagerId</c> → <c>Manager</c>, зворотна навігація <c>Reports</c>).
/// </summary>
[ApiController]
[Route("api/ch04/hierarchical")]
[Tags("Глава 4 — Відношення між моделями")]
public class HierarchicalDataController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public HierarchicalDataController(RelationshipsContext db) => _db = db;

    /// <summary>Верхівки ієрархії — працівники без керівника.</summary>
    [HttpGet("roots")]
    public async Task<ActionResult<object>> Roots()
    {
        var roots = await _db.Employees
            .Where(e => e.ManagerId == null)
            .Select(e => new { e.Id, e.Name, e.Position })
            .ToListAsync();

        return Ok(roots);
    }

    /// <summary>
    /// Повне дерево. EF не вміє завантажити ієрархію довільної глибини одним
    /// <c>Include</c>, тому: тягнемо ВСІХ працівників одним запитом, а дерево
    /// збираємо вже в пам'яті за <c>ManagerId</c>.
    /// </summary>
    [HttpGet("tree")]
    public async Task<ActionResult<object>> Tree()
    {
        var all = await _db.Employees.AsNoTracking().ToListAsync();
        var byManager = all.ToLookup(e => e.ManagerId);

        object Build(Employee e) => new
        {
            e.Id,
            e.Name,
            e.Position,
            reports = byManager[e.Id].Select(Build)
        };

        var tree = byManager[null].Select(Build);
        return Ok(tree);
    }

    /// <summary>Ланцюжок керівників угору від конкретного працівника.</summary>
    [HttpGet("employees/{id:int}/chain")]
    public async Task<ActionResult<object>> Chain(int id)
    {
        var chain = new List<string>();
        var current = await _db.Employees.FindAsync(id);
        if (current is null)
            return NotFound();

        while (current is not null)
        {
            chain.Add($"{current.Name} ({current.Position})");

            // Кожна ітерація — окремий запит по ManagerId. Для глибоких ієрархій
            // краще завантажити всіх одразу (див. /tree).
            current = current.ManagerId is null
                ? null
                : await _db.Employees.FindAsync(current.ManagerId);
        }

        return Ok(chain);
    }
}
