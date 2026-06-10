using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SimpleServe.Core;

public sealed class CleanupWorker(
        ServerRegistry registry,
        ILogger<CleanupWorker> logger,
        IConfiguration configuration) : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(
        Math.Max(1, int.TryParse(configuration["Cleanup:IntervalSeconds"], out var seconds) ? seconds : 1));
    private readonly TimeSpan _heartbeatTimeout = TimeSpan.FromSeconds(
        Math.Max(1, int.TryParse(configuration["Cleanup:HeartbeatTimeoutSeconds"], out var timeoutSeconds) ? timeoutSeconds : 10));


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            foreach (var (id, server) in registry.Servers)
            {
                if (now - server.LastHeartbeat > _heartbeatTimeout)
                {
                    registry.Servers.TryRemove(id, out _);

                    logger.LogInformation(
                        "Server {ServerId} removed due to heartbeat timeout",
                        id);
                }
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }
}