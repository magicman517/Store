using Common;

namespace Catalog.Core.Entities;

public class Category : BaseEntity
{
    public required string Slug { get; set; }

    public required string Name { get; set; }
}