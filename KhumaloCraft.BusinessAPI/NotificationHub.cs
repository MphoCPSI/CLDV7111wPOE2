using Microsoft.AspNetCore.SignalR;

namespace KhumaloCraft.BusinessAPI;

public class NotificationHub : Hub
{
  public override async Task OnConnectedAsync()
  {
    var userId = Context.UserIdentifier;
    if (!string.IsNullOrEmpty(userId))
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
    await base.OnConnectedAsync();
  }
}