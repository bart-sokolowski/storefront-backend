namespace StorefrontApi.Data;

using StorefrontApi.Models;

public static class DataSeeder
{
    public static IEnumerable<Product> GetProducts() =>
    [
        new Product
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Name = "Wireless Headphones",
            Description = "Over-ear noise-cancelling headphones with 30-hour battery life.",
            Price = 89.99m,
            Stock = 50,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        },
        new Product
        {
            Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
            Name = "Mechanical Keyboard",
            Description = "Compact TKL layout with Cherry MX Brown switches.",
            Price = 129.99m,
            Stock = 30,
            CreatedAt = DateTime.UtcNow.AddDays(-25)
        },
        new Product
        {
            Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
            Name = "USB-C Hub",
            Description = "7-in-1 hub with HDMI, USB-A, SD card reader, and 100W PD.",
            Price = 49.99m,
            Stock = 100,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        },
        new Product
        {
            Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
            Name = "Webcam 1080p",
            Description = "Full HD webcam with built-in microphone and auto-focus.",
            Price = 74.99m,
            Stock = 40,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        },
        new Product
        {
            Id = Guid.Parse("e5f6a7b8-c9d0-1234-efab-345678901234"),
            Name = "Desk Lamp",
            Description = "LED desk lamp with adjustable colour temperature and USB charging port.",
            Price = 34.99m,
            Stock = 75,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        }
    ];
}
