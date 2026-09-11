using System.ComponentModel.DataAnnotations;

namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє ініціалізацію БД початковими даними через <c>HasData</c> (3.13).
/// Первинний ключ тут не число, а рядок — тому його треба задати явно ([Key]).
/// </summary>
public class AppSetting
{
    [Key]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}
