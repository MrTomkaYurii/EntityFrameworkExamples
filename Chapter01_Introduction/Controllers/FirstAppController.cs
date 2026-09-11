using EfCoreExamples.Chapter01_Introduction.Models;
using EfCoreExamples.Chapter01_Introduction.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExamples.Chapter01_Introduction.Controllers;

/// <summary>
/// Уроки 1.2 "Перше застосування" та 1.5 "Основні операції з даними. CRUD".
/// Показує повний цикл роботи з однією сутністю: Create, Read, Update, Delete.
/// </summary>
[ApiController]
[Route("api/ch01/first-app")]
[Tags("Глава 1 — Вступ до EF Core")]
public class FirstAppController : ControllerBase
{
    private readonly IntroductionContext _db;

    // DbContext приходить через конструктор — його реєстрація у Program.cs (AddDbContext).
    // Час життя — на один HTTP-запит (scoped).
    public FirstAppController(IntroductionContext db) => _db = db;

    // ─── READ ────────────────────────────────────────────────────────────────

    /// <summary>Усі користувачі. Найпростіший запит: SELECT * FROM Users.</summary>
    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        // ToListAsync() виконує запит і матеріалізує результат у список об'єктів.
        return await _db.Users.ToListAsync();
    }

    /// <summary>Один користувач за первинним ключем.</summary>
    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        // FindAsync спершу шукає сутність серед уже відстежуваних у пам'яті,
        // і лише потім (якщо не знайшов) звертається до БД за первинним ключем.
        var user = await _db.Users.FindAsync(id);

        return user is null ? NotFound() : user;
    }

    // ─── CREATE ──────────────────────────────────────────────────────────────

    /// <summary>Додати користувача. Id генерує база даних (IDENTITY).</summary>
    [HttpPost("users")]
    public async Task<ActionResult<User>> Create(UserInput input)
    {
        var user = new User { Name = input.Name, Age = input.Age };

        // Add лише позначає об'єкт як "потрібно вставити" (стан Added) у трекері змін.
        _db.Users.Add(user);

        // SaveChanges формує та виконує INSERT, а потім записує згенерований Id назад у user.
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // ─── UPDATE ──────────────────────────────────────────────────────────────

    /// <summary>Оновити користувача.</summary>
    [HttpPut("users/{id:int}")]
    public async Task<ActionResult<User>> Update(int id, UserInput input)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        // Просто змінюємо властивості відстежуваного об'єкта.
        // Трекер змін сам побачить різницю і при SaveChanges згенерує UPDATE
        // лише для змінених стовпців.
        user.Name = input.Name;
        user.Age = input.Age;

        await _db.SaveChangesAsync();
        return user;
    }

    // ─── DELETE ──────────────────────────────────────────────────────────────

    /// <summary>Видалити користувача.</summary>
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        _db.Users.Remove(user);            // стан Deleted
        await _db.SaveChangesAsync();      // DELETE FROM Users WHERE Id = @id
        return NoContent();
    }

    // ─── допоміжне для відтворюваності прикладів ──────────────────────────────

    /// <summary>Повернути БД глави до початкового стану (видалити все й засіяти заново).</summary>
    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        IntroductionSeeder.Seed(_db);
        return Ok(new { message = "Дані глави 1 скинуто до початкових." });
    }
}

/// <summary>Вхідні дані для створення/оновлення користувача (окремо від моделі БД).</summary>
public record UserInput(string Name, int Age);
