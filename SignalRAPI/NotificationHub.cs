using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
    public class NotificationHub: Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
           await Clients.Client(Context.ConnectionId).ReceiveNotification($"Welcome to the Notification Hub, {Context.User?.Identity?.Name}");
            await base.OnConnectedAsync();
        }
    }

    public interface INotificationClient {
        Task ReceiveNotification(string message);

    }

}
