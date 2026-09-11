using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCoreExamples.Chapter03_Models;

/// <summary>
/// Перетворює метадані моделі EF Core (<see cref="IEntityType"/>) на зручний для JSON
/// вигляд. Потрібно лише для навчальних ендпоінтів — щоб "побачити", що саме
/// зробили анотації та Fluent API.
/// </summary>
public static class ModelInspection
{
    public static object Describe(IEntityType entityType) => new
    {
        clrType = entityType.ClrType.Name,
        table = entityType.GetTableName(),
        schema = entityType.GetSchema(),

        properties = entityType.GetProperties().Select(p => new
        {
            name = p.Name,
            column = p.GetColumnName(),
            columnType = p.GetColumnType(),
            isNullable = p.IsNullable,
            maxLength = p.GetMaxLength(),
            precision = p.GetPrecision(),
            scale = p.GetScale(),
            isPrimaryKey = p.IsPrimaryKey(),
            valueGenerated = p.ValueGenerated.ToString(),
            defaultValueSql = p.GetDefaultValueSql(),
            computedColumnSql = p.GetComputedColumnSql()
        }),

        keys = entityType.GetKeys().Select(k => new
        {
            columns = k.Properties.Select(p => p.Name),
            isPrimaryKey = k.IsPrimaryKey()
        }),

        indexes = entityType.GetIndexes().Select(i => new
        {
            columns = i.Properties.Select(p => p.Name),
            isUnique = i.IsUnique,
            filter = i.GetFilter()
        })
    };
}
