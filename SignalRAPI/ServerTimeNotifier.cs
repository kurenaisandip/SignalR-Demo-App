
using Microsoft.AspNetCore.SignalR;

namespace SignalRAPI
{
    public class ServerTimeNotifier : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
        private readonly ILogger<ServerTimeNotifier> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while(!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Sending server time to all clients");
                await _hubContext.Clients.All.ReceiveNotification($"Server time: {DateTimeOffset.Now}");
            }
        }
    }
}
