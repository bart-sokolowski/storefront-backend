namespace StorefrontApi.Services;

using StorefrontApi.Common;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.DTOs.Responses;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;
using StorefrontApi.Repositories;
using StorefrontApi.Validation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentService _paymentService;
    private readonly IValidationService _validationService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IPaymentService paymentService,
        IValidationService validationService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _paymentService = paymentService;
        _validationService = validationService;
    }

    public async Task<ApiResult<IEnumerable<OrderResponse>>> GetOrdersAsync(string userEmail, bool isAdmin, CancellationToken ct = default)
    {
        var orders = isAdmin
            ? await _orderRepository.GetAllAsync(ct)
            : await _orderRepository.GetByUserEmailAsync(userEmail, ct);

        return ApiResult<IEnumerable<OrderResponse>>.Ok(orders.Select(MapToResponse));
    }

    public async Task<ApiResult<OrderResponse>> GetOrderAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order is null)
            return ApiResult<OrderResponse>.Fail("Order not found.");

        return ApiResult<OrderResponse>.Ok(MapToResponse(order));
    }

    public async Task<ApiResult<OrderResponse>> CreateOrderAsync(
        string userEmail,
        string idempotencyKey,
        CreateOrderRequest request,
        CancellationToken ct = default)
    {
        var existing = await _orderRepository.GetByIdempotencyKeyAsync(idempotencyKey, ct);
        if (existing is not null)
            return ApiResult<OrderResponse>.Ok(MapToResponse(existing), "Existing order returned.");

        var validation = await _validationService.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ApiResult<OrderResponse>.Fail("Validation failed.", validation.Errors);

        var items = new List<OrderItem>();
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, ct);

            if (product is null || product.Status == ProductStatus.Archived)
                return ApiResult<OrderResponse>.Fail($"Product '{itemRequest.ProductId}' is not available.");

            if (product.Stock < itemRequest.Quantity)
                return ApiResult<OrderResponse>.Fail($"Insufficient stock for '{product.Name}'. Available: {product.Stock}.");

            items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = itemRequest.Quantity
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserEmail = userEmail,
            Items = items,
            TotalAmount = items.Sum(i => i.LineTotal),
            PaymentStatus = PaymentStatus.Pending,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order, idempotencyKey, ct);

        foreach (var itemRequest in request.Items)
            await _productRepository.DecrementStockAsync(itemRequest.ProductId, itemRequest.Quantity, ct);

        var paymentResult = await _paymentService.ProcessPaymentAsync(new PaymentRequest
        {
            OrderId = order.Id,
            IdempotencyKey = idempotencyKey,
            Amount = order.TotalAmount,
            UserEmail = userEmail,
            Items = request.Items.Select(i => new PaymentItemRequest
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        }, ct);

        order.PaymentReference = paymentResult.PaymentReference;

        return ApiResult<OrderResponse>.Ok(MapToResponse(order), "Order created. Payment is processing.");
    }

    private static OrderResponse MapToResponse(Order order) => new()
    {
        Id = order.Id,
        UserEmail = order.UserEmail,
        Items = order.Items.Select(i => new OrderItemResponse
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            LineTotal = i.LineTotal
        }).ToList(),
        TotalAmount = order.TotalAmount,
        Status = order.Status.ToString(),
        PaymentStatus = order.PaymentStatus.ToString(),
        FailureReason = order.FailureReason,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
