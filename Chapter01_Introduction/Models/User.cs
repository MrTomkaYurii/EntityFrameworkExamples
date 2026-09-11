namespace EfCoreExamples.Chapter01_Introduction.Models;

/// <summary>
/// Найпростіша сутність — звичайний C#-клас (POCO, "plain old CLR object").
/// EF Core за домовленістю (convention):
/// <list type="bullet">
///   <item>вважає властивість <c>Id</c> (або <c>UserId</c>) первинним ключем;</item>
///   <item>первинний ключ типу <c>int</c> робить автоінкрементним (IDENTITY);</item>
///   <item>назву таблиці бере від імені властивості DbSet — тут це "Users".</item>
/// </list>
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }
}
