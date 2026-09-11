using EfCoreExamples.Chapter04_Relationships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter04_Relationships.Controllers;

/// <summary>
/// Урок 4.7 "Відношення один до одного" — <see cref="User"/> ↔ <see cref="UserProfile"/>.
/// Залежна сторона (<c>UserProfile</c>) визначається за зовнішнім ключем <c>UserId</c>,
/// на який EF ставить УНІКАЛЬНИЙ індекс — саме він робить зв'язок "один до одного".
/// </summary>
[ApiController]
[Route("api/ch04/one-to-one")]
[Tags("Глава 4 — Відношення між моделями")]
public class OneToOneController : ControllerBase
{
    private readonly RelationshipsContext _db;

    public OneToOneController(RelationshipsContext db) => _db = db;

    [HttpGet("users")]
    public async Task<ActionResult<object>> Users()
    {
        var users = await _db.Users
            .Select(u => new
            {
                u.Id,
                u.Name,
                profile = u.Profile == null
                    ? null
                    : new { u.Profile.Id, u.Profile.Bio, u.Profile.Website }
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Створити профіль для користувача. Повторний виклик для того самого користувача
    /// впаде на унікальному індексі <c>UserId</c> — це й доводить, що зв'язок "один до одного".
    /// </summary>
    [HttpPost("users/{id:int}/profile")]
    public async Task<ActionResult<object>> AddProfile(int id, ProfileInput input)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        _db.Profiles.Add(new UserProfile { UserId = id, Bio = input.Bio, Website = input.Website });

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Профіль для користувача {user.Name} створено." });
        }
        catch (DbUpdateException ex)
        {
            return Conflict(new
            {
                note = "У цього користувача вже є профіль (унікальний індекс на UserId).",
                error = ex.GetBaseException().Message
            });
        }
    }

    /// <summary>Навігація від профілю до користувача.</summary>
    [HttpGet("profiles/{id:int}/user")]
    public async Task<ActionResult<object>> ProfileUser(int id)
    {
        var profile = await _db.Profiles
            .Where(p => p.Id == id)
            .Select(p => new { p.Id, p.Bio, user = new { p.User!.Id, p.User.Name } })
            .FirstOrDefaultAsync();

        return profile is null ? NotFound() : Ok(profile);
    }
}

public record ProfileInput(string Bio, string? Website);
