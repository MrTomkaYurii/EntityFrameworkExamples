namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Демонструє комплексні типи (complex types, 4.11) — з'явилися в EF Core 8.
/// <para>
/// На відміну від власних типів, комплексний тип — це справжній value object:
/// у нього немає прихованого ключа й "власної" ідентичності, він завжди зберігається
/// в тій самій таблиці й не може існувати окремо. Один екземпляр можна навіть
/// присвоїти двом сутностям.
/// </para>
/// </summary>
public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Комплексний тип. Стовпці Address_City, Address_Street, Address_Zip у таблиці Customers.</summary>
    public Address Address { get; set; } = new();
}

/// <summary>Комплексний тип (value object). Налаштовується через <c>ComplexProperty</c>.</summary>
public class Address
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}
