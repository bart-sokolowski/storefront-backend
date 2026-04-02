namespace StorefrontApi.Interfaces;

using StorefrontApi.Models;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> GetPaymentStatusAsync(string paymentReference, CancellationToken ct = default);
    Task<bool> CancelPaymentAsync(string paymentReference, CancellationToken ct = default);
}

public class PaymentRequest
{
    public Guid OrderId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
    public string UserEmail { get; set; } = string.Empty;
    public List<PaymentItemRequest> Items { get; set; } = [];
}

public class PaymentItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class PaymentResult
{
    public string PaymentReference { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string? FailureReason { get; set; }
}
