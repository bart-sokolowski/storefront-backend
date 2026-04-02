namespace StorefrontApi.Interfaces;

using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;

public interface IOrderService
{
    Task<ApiResult<IEnumerable<OrderResponse>>> GetOrdersAsync(string userEmail, bool isAdmin, CancellationToken ct = default);
    Task<ApiResult<OrderResponse>> GetOrderAsync(Guid id, CancellationToken ct = default);
    Task<ApiResult<OrderResponse>> CreateOrderAsync(string userEmail, string idempotencyKey, CreateOrderRequest request, CancellationToken ct = default);
}
