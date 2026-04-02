namespace StorefrontApi.Repositories;

using System.Collections.Concurrent;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();
    private readonly ConcurrentDictionary<string, Guid> _idempotencyIndex = new();

    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_orders.Values.OrderByDescending(o => o.CreatedAt).AsEnumerable());
    }

    public Task<IEnumerable<Order>> GetByUserEmailAsync(string email, CancellationToken ct = default)
    {
        var orders = _orders.Values
            .Where(o => o.UserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.CreatedAt);

        return Task.FromResult(orders.AsEnumerable());
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default)
    {
        if (_idempotencyIndex.TryGetValue(key, out var orderId) && _orders.TryGetValue(orderId, out var order))
            return Task.FromResult<Order?>(order);

        return Task.FromResult<Order?>(null);
    }

    public Task<Order> AddAsync(Order order, string idempotencyKey, CancellationToken ct = default)
    {
        _orders[order.Id] = order;
        _idempotencyIndex[idempotencyKey] = order.Id;
        return Task.FromResult(order);
    }

    public Task UpdatePaymentStatusAsync(Guid id, PaymentStatus status, string? failureReason = null, CancellationToken ct = default)
    {
        if (!_orders.TryGetValue(id, out var order))
            return Task.CompletedTask;

        order.PaymentStatus = status;
        order.Status = status == PaymentStatus.Confirmed ? OrderStatus.Confirmed : OrderStatus.Failed;
        order.FailureReason = failureReason;
        order.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}
