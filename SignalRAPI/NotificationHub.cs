using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.ReceiveNotification($"Welcome to the Notification Hub, {Context.ConnectionId}");
            Console.WriteLine($"Client connected with connection ID: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected with connection ID: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterClient(string clientType)
        {
            // Register the client with their connection ID
            ServerTimeNotifier.RegisterClient(Context.ConnectionId, clientType);

            string responseMessage = clientType.ToLower() switch
            {
                "desktop" => $"Time for desktop: {DateTimeOffset.Now}",
                "web" => $"Time for web: {DateTimeOffset.Now}",
                _ => "Unknown client type."
            };

            // Send the response back to the caller
            await Clients.Caller.ReceiveNotification(responseMessage);
            Console.WriteLine($"Response sent to {clientType} client: {responseMessage}");
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }
}