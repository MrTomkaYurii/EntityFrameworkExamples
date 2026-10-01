namespace EfCoreExamples.Chapter05_Inheritance.Models;

/// <summary>
/// Базова сутність для стратегії TPT (Table Per Type).
/// Зберігається в окремій таблиці [BillingAccounts_TPT].
/// </summary>
public class BillingAccountTpt
{
    public int Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}

/// <summary>
/// Похідна сутність для TPT.
/// Зберігається у власній таблиці [CreditAccounts_TPT], де PK є одночасно FK на [BillingAccounts_TPT].
/// </summary>
public class CreditAccountTpt : BillingAccountTpt
{
    public decimal CreditLimit { get; set; }
}

/// <summary>
/// Інша похідна сутність для TPT.
/// Зберігається у власній таблиці [DepositAccounts_TPT].
/// </summary>
public class DepositAccountTpt : BillingAccountTpt
{
    public decimal InterestRate { get; set; }
}
