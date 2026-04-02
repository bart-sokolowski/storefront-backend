namespace StorefrontApi.Hubs;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
            await Groups.AddToGroupAsync(Context.ConnectionId, email);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, email);

        await base.OnDisconnectedAsync(exception);
    }
}
