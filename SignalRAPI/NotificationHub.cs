using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            // Send a welcome message to the connected client
            await Clients.Client(Context.ConnectionId).ReceiveNotification($"Welcome to the Notification Hub, {Context.User?.Identity?.Name}");
            Console.WriteLine($"Client connected with connection ID: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Unregister the user when they disconnect
            ServerTimeNotifier.UnregisterUser(Context.ConnectionId);
            Console.WriteLine($"Client disconnected with connection ID: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        // Register the user ID with their connection ID
        public Task RegisterUserId(string userId)
        {
            ServerTimeNotifier.RegisterUser(userId, Context.ConnectionId);
            return Task.CompletedTask;
        }
    }

    // Client interface for strongly-typed hubs
    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }

}
