namespace StorefrontApi.Models;

public class Order
{
    public Guid Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string PaymentReference { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Failed,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Confirmed,
    Failed,
    Cancelled
}
