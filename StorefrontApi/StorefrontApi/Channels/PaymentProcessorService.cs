namespace StorefrontApi.Channels;

using Microsoft.AspNetCore.SignalR;
using StorefrontApi.Hubs;
using StorefrontApi.Interfaces;
using StorefrontApi.Models;
using StorefrontApi.Repositories;

public class PaymentProcessorService : BackgroundService
{
    private readonly PaymentChannel _channel;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly ILogger<PaymentProcessorService> _logger;

    public PaymentProcessorService(
        PaymentChannel channel,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IHubContext<NotificationHub> hub,
        ILogger<PaymentProcessorService> logger)
    {
        _channel = channel;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _hub = hub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessAsync(message, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error processing payment for order {OrderId}", message.OrderId);
            }
        }
    }

    private async Task ProcessAsync(PaymentMessage message, CancellationToken ct)
    {
        await Task.Delay(Random.Shared.Next(1500, 3500), ct);

        var succeeded = Random.Shared.NextDouble() > 0.3;

        if (succeeded)
        {
            await _orderRepository.UpdatePaymentStatusAsync(message.OrderId, PaymentStatus.Confirmed, null, ct);
        }
        else
        {
            await _orderRepository.UpdatePaymentStatusAsync(
                message.OrderId,
                PaymentStatus.Failed,
                "Payment declined by processor.",
                ct);

            foreach (var item in message.Items)
                await _productRepository.IncrementStockAsync(item.ProductId, item.Quantity, ct);
        }

        await _hub.Clients.Group(message.UserEmail).SendAsync("PaymentStatusUpdated", new
        {
            orderId = message.OrderId,
            paymentStatus = succeeded ? "Confirmed" : "Failed",
            note = succeeded ? "Your payment was confirmed." : "Your payment was declined. Stock has been restored."
        }, ct);
    }
}
