using Common;

namespace Catalog.Core.Entities;

public class Product : BaseEntity
{
    public required string Slug { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}