namespace EfCoreExamples.Chapter04_Relationships.LazyLoading;

/// <summary>
/// Модель для лінивого завантаження (4.6). Ключова відмінність — навігаційні
/// властивості позначені <c>virtual</c>: EF створює проксі-нащадок класу
/// й перехоплює звернення до них, щоб довантажити дані з БД за потреби.
/// </summary>
public class Team
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public virtual List<Player> Players { get; set; } = [];
}
