namespace StorefrontApi.Services;

using System.Collections.Concurrent;
using StorefrontApi.Channels;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;

public class MockPaymentService : IPaymentService
{
    private readonly PaymentChannel _channel;
    private readonly ConcurrentDictionary<string, PaymentResult> _paymentStatuses = new();

    public MockPaymentService(PaymentChannel channel)
    {
        _channel = channel;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken ct = default)
    {
        var reference = Guid.NewGuid().ToString();
        var result = new PaymentResult { PaymentReference = reference, Status = PaymentStatus.Pending };
        _paymentStatuses[reference] = result;

        await _channel.Writer.WriteAsync(new PaymentMessage
        {
            OrderId = request.OrderId,
            IdempotencyKey = request.IdempotencyKey,
            Amount = request.Amount,
            UserEmail = request.UserEmail,
            Items = request.Items.Select(i => new PaymentMessageItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        }, ct);

        return result;
    }

    public Task<PaymentResult> GetPaymentStatusAsync(string paymentReference, CancellationToken ct = default)
    {
        if (_paymentStatuses.TryGetValue(paymentReference, out var result))
            return Task.FromResult(result);

        return Task.FromResult(new PaymentResult
        {
            PaymentReference = paymentReference,
            Status = PaymentStatus.Failed,
            FailureReason = "Payment reference not found."
        });
    }

    public Task<bool> CancelPaymentAsync(string paymentReference, CancellationToken ct = default)
    {
        if (!_paymentStatuses.TryGetValue(paymentReference, out var result))
            return Task.FromResult(false);

        if (result.Status != PaymentStatus.Pending)
            return Task.FromResult(false);

        result.Status = PaymentStatus.Cancelled;
        return Task.FromResult(true);
    }
}
