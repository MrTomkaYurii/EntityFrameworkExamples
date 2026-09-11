namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє альтернативний ключ (3.8) та унікальний фільтрований індекс (3.9).
/// Налаштування — у <c>ModelsContext.OnModelCreating</c>.
/// </summary>
public class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    /// <summary>Альтернативний ключ: HasAlternateKey(c =&gt; c.Email). Значення має бути унікальним.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Необов'язковий (nullable). Унікальний індекс із фільтром "[Phone] IS NOT NULL" —
    /// щоб кілька клієнтів без телефону не порушували унікальність.
    /// </summary>
    public string? Phone { get; set; }
}
