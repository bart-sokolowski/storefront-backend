namespace StorefrontApi.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ProductStatus
{
    Active,
    Archived
}
