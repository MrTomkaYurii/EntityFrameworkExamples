namespace EfCoreExamples.Chapter06_Queries.Models;

public class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public List<User> Users { get; set; } = [];
}
