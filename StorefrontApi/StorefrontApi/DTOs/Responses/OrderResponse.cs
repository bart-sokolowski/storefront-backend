namespace StorefrontApi.DTOs.Responses;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OrderItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
