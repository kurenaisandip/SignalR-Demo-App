
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
       public class ServerTimeNotifier : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5); // Send notifications every 5 seconds
        private readonly ILogger<ServerTimeNotifier> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private static readonly ConcurrentDictionary<string, string> ConnectionUsers = new(); // Maps connectionId to userId

        public ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Send a notification to all registered connections
                foreach (var connectionId in ConnectionUsers.Keys)
                {
                    _logger.LogInformation($"Sending server time to connection: {connectionId}");

                    // Send a message to the specific client using their connectionId
                    await _hubContext.Clients.Client(connectionId).ReceiveNotification($"Server time: {DateTimeOffset.Now}");
                    Console.WriteLine($"Server time sent to connection {connectionId}: {DateTimeOffset.Now}");
                }
            }
        }

        // Register a user with their connectionId
        public static void RegisterUser(string userId, string connectionId)
        {
            ConnectionUsers[connectionId] = userId;
            Console.WriteLine($"User {userId} registered with connection ID {connectionId}");
        }

        // Unregister a user when they disconnect
        public static void UnregisterUser(string connectionId)
        {
            if (ConnectionUsers.TryRemove(connectionId, out var userId))
            {
                Console.WriteLine($"User {userId} with connection ID {connectionId} unregistered.");
            }
        }
    }
}
