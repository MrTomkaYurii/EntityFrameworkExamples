namespace EfCoreExamples.Chapter04_Relationships.LazyLoading;

public class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TeamId { get; set; }

    public virtual Team? Team { get; set; }
}
