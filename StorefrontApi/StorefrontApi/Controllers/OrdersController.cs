namespace StorefrontApi.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorefrontApi.DTOs.Requests;
using StorefrontApi.Interfaces;
using StorefrontApi.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
        var isAdmin = User.IsInRole("Admin");
        var result = await _orderService.GetOrdersAsync(userEmail, isAdmin, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        CancellationToken ct)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
        var result = await _orderService.CreateOrderAsync(userEmail, idempotencyKey, request, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _orderService.GetOrderAsync(id, ct);
        return Ok(result);
    }
}
