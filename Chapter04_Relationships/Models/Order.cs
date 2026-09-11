namespace EfCoreExamples.Chapter04_Relationships.Models;

/// <summary>
/// Демонструє власні типи (owned types, 4.10).
/// <para>
/// <see cref="ShippingAddress"/> і <see cref="Lines"/> не мають власних <c>DbSet</c>
/// і власного ключа — вони "належать" замовленню. За замовчуванням EF складає їх
/// у ту саму таблицю (owned reference) або в окрему таблицю (owned collection).
/// </para>
/// </summary>
public class Order
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    /// <summary>Власний тип-посилання: стовпці ShippingAddress_City, ShippingAddress_Street... в тій самій таблиці Orders.</summary>
    public OrderAddress ShippingAddress { get; set; } = new();

    /// <summary>Власна колекція: окрема таблиця з прихованим зовнішнім ключем на Orders.</summary>
    public List<OrderItem> Lines { get; set; } = [];
}

/// <summary>Власний тип. Не має ключа — існує лише як частина <see cref="Order"/>.</summary>
public class OrderAddress
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}

/// <summary>Елемент власної колекції замовлення.</summary>
public class OrderItem
{
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
