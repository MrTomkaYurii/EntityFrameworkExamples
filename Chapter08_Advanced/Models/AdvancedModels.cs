namespace EfCoreExamples.Chapter08_Advanced.Models;

/// <summary>
/// Сутність банківського рахунку з підтримкою оптимістичного паралелізму (Concurrency).
/// Стовпець RowVersion автоматично оновлюється SQL Server при кожному INSERT / UPDATE.
/// </summary>
public class BankAccount
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }

    /// <summary>
    /// Токен паралелізму (concurrency token). У SQL Server мапиться на тип [rowversion] (або timestamp).
    /// </summary>
    public byte[] RowVersion { get; set; } = [];
}

/// <summary>
/// Неключова сутність, зіставлена з поданням (Database View) [V_AccountSummary].
/// </summary>
public class AccountSummaryView
{
    public string OwnerName { get; set; } = string.Empty;
    public int AccountCount { get; set; }
    public decimal TotalBalance { get; set; }
}

/// <summary>
/// Сутність документа з підтримкою системного версіонування (Temporal Table).
/// SQL Server автоматично веде повну історію всіх змін у прихованій таблиці історії.
/// </summary>
public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}
