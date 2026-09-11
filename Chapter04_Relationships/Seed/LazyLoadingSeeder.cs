using EfCoreExamples.Chapter04_Relationships.LazyLoading;

namespace EfCoreExamples.Chapter04_Relationships.Seed;

public static class LazyLoadingSeeder
{
    public static void Seed(LazyLoadingContext db)
    {
        if (db.Teams.Any())
            return;

        db.Teams.AddRange(
            new Team
            {
                Name = "Falcons",
                Players = { new Player { Name = "Alex" }, new Player { Name = "Ben" }, new Player { Name = "Chris" } }
            },
            new Team
            {
                Name = "Sharks",
                Players = { new Player { Name = "Dan" }, new Player { Name = "Ed" } }
            });

        db.SaveChanges();
    }
}
