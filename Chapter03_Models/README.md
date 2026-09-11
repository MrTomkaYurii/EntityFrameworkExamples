# Глава 3. Створення моделей

Відповідає розділу [metanit — Создание моделей в Entity Framework Core](https://metanit.com/sharp/efcore/3.1.php).

Контекст: `ModelsContext` → база `EfCoreExamples_Ch03_Models` (`EnsureCreated` при старті).
Багато ендпоінтів повертають **метадані моделі** — так видно, що саме зробили анотації та Fluent API.

| Урок metanit | Демонструє | Де в коді | Ендпоінти |
|---|---|---|---|
| 3.1 Fluent API та анотації | два стилі налаштування поряд | `Models/Product.cs` (анотації) + `ModelsContext.OnModelCreating` (Fluent) | `GET /api/ch03/model-mapping/product` |
| 3.2 Визначення моделей | що EF вважає сутністю | `Controllers/ModelDefinitionController.cs` | `GET /api/ch03/model-definition/entities` |
| 3.3 Властивості сутності | `[NotMapped]`, які властивості зберігаються | `Models/Product.cs` (`Display`) | `GET /api/ch03/model-definition/not-mapped` |
| 3.4 Конструктори сутностей | створення через конструктор з параметрами | `Models/Article.cs` | `GET /api/ch03/model-definition/articles` |
| 3.5 Поля сутності | backing field, властивість лише для читання | `Models/Article.cs`, `HasField("_viewCount")` | `POST /api/ch03/model-definition/articles/{id}/views` |
| 3.6 Зіставлення таблиць і стовпців | `ToTable(schema)`, `[Column]`, типи стовпців | `ModelsContext`, `Models/Product.cs` | `GET /api/ch03/model-mapping/product` |
| 3.7 Обов'язкові / необов'язкові | `NOT NULL` за типом (`string` vs `string?`) | `Models/Customer.cs` | `GET /api/ch03/model-mapping/required-optional` |
| 3.8 Налаштування ключів | складений PK, альтернативний ключ | `Models/OrderLine.cs`, `Models/Customer.cs` | `GET /api/ch03/keys-and-indexes/keys`, `POST /api/ch03/keys-and-indexes/{customers,order-lines}` |
| 3.9 Налаштування індексів | `HasIndex`, `IsUnique`, `HasFilter` | `ModelsContext`, `CategoryConfiguration` | `GET /api/ch03/keys-and-indexes/indexes` |
| 3.10 Генерація значень | IDENTITY, `HasDefaultValueSql`, `HasComputedColumnSql` | `ModelsContext` (Product) | `POST /api/ch03/value-generation/products`, `GET /api/ch03/value-generation/metadata` |
| 3.11 Обмеження властивостей | `HasPrecision`, `HasMaxLength`, `HasCheckConstraint` | `ModelsContext` (Product) | `POST /api/ch03/value-generation/products/invalid-price` |
| 3.12 Конфігурація моделей | `IEntityTypeConfiguration<T>`, `ApplyConfigurationsFromAssembly` | `Configurations/CategoryConfiguration.cs` | `GET /api/ch03/seeding/categories` |
| 3.13 Початкові дані | `HasData` (у контексті та в конфігурації) | `ModelsContext` (AppSetting), `CategoryConfiguration` | `GET /api/ch03/seeding/{settings,categories,seed-metadata}` |

## Підказки

- `GET /api/ch03/model-mapping/create-script` — повний DDL-скрипт схеми глави.
- Порівняй відповідь `GET .../value-generation/metadata` з тим, що після `POST .../value-generation/products`
  повертається в полі `generatedByDatabase`.
- `seed-metadata` читає дані з design-time моделі (`IDesignTimeModel`) — у рантайм-моделі їх немає.
