
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
    public class ServerTimeNotifier : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
        private readonly ILogger<ServerTimeNotifier> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private static readonly ConcurrentDictionary<string, string> ClientTypes = new();

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
                Console.WriteLine("Sending periodic messages to clients...");

                if (ClientTypes.IsEmpty)
                {
                    Console.WriteLine("No clients registered.");
                    continue;
                }

                foreach (var (connectionId, clientType) in ClientTypes)
                {
                    string message = clientType.ToLower() switch
                    {
                        "desktop" => $"Time for desktop: {DateTimeOffset.Now}",
                        "web" => $"Time for web: {DateTimeOffset.Now}",
                        _ => "Unknown client type."
                    };

                    await _hubContext.Clients.Client(connectionId).ReceiveNotification(message);
                    Console.WriteLine($"Message sent to {clientType} client ({connectionId}): {message}");
                }
            }
        }

        public static void RegisterClient(string connectionId, string clientType)
        {
            ClientTypes[connectionId] = clientType;
            Console.WriteLine($"Client {connectionId} registered as {clientType}");
        }
        
        public static void UnregisterClient(string connectionId)
        {
            if (ClientTypes.TryRemove(connectionId, out var clientType))
            {
                Console.WriteLine($"Client {connectionId} ({clientType}) unregistered.");
            }
        }
    }
}
