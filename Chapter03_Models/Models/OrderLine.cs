namespace EfCoreExamples.Chapter03_Models.Models;

/// <summary>
/// Демонструє складений (composite) первинний ключ (3.8):
/// <c>HasKey(x =&gt; new { x.OrderId, x.ProductId })</c>.
/// Такий ключ неможливо задати анотацією [Key] — лише через Fluent API.
/// </summary>
public class OrderLine
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
