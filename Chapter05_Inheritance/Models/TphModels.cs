namespace EfCoreExamples.Chapter05_Inheritance.Models;

/// <summary>
/// Базова сутність для стратегії TPH (Table Per Hierarchy).
/// Зберігається в одній таблиці [Users_TPH] разом з усіма нащадками.
/// </summary>
public class UserTph
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Похідна сутність першого рівня в TPH.
/// Її властивості Company і Salary зберігаються у тій самій таблиці Users_TPH
/// (і для базових користувачів містять NULL).
/// </summary>
public class EmployeeTph : UserTph
{
    public string Company { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}

/// <summary>
/// Похідна сутність другого рівня в TPH.
/// Демонструє багаторівневу ієрархію в межах однієї таблиці.
/// </summary>
public class ManagerTph : EmployeeTph
{
    public string Department { get; set; } = string.Empty;
    public decimal AnnualBonus { get; set; }
}
