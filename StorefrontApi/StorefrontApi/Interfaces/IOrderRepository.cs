namespace StorefrontApi.Interfaces;

using StorefrontApi.Models;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByUserEmailAsync(string email, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default);
    Task<Order> AddAsync(Order order, string idempotencyKey, CancellationToken ct = default);
    Task UpdatePaymentStatusAsync(Guid id, PaymentStatus status, string? failureReason = null, CancellationToken ct = default);
}
