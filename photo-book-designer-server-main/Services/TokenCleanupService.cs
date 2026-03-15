using Microsoft.EntityFrameworkCore;
using photo_book_designer_server_main.Data;

namespace photo_book_designer_server_main.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(
            IServiceProvider serviceProvider,
            ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token cleanup service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddDays(1).AddHours(3);
                    var delay = nextRun - now;

                    _logger.LogInformation("Next cleanup scheduled at {NextRun:yyyy-MM-dd HH:mm}", nextRun);
                    await Task.Delay(delay, stoppingToken);
                    await PerformCleanupAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Token cleanup service stopped");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during token cleanup");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private async Task PerformCleanupAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PhotoBookDBContext>();

            _logger.LogInformation("Starting cleanup of expired tokens at {Time}", DateTime.Now);

            var cutoff = DateTime.UtcNow.AddDays(-30);
            var expiredTokens = await dbContext.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow || (rt.RevokedAt != null && rt.RevokedAt < cutoff))
                .ToListAsync(stoppingToken);

            if (expiredTokens.Any())
            {
                dbContext.RefreshTokens.RemoveRange(expiredTokens);
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Removed {Count} expired/revoked tokens", expiredTokens.Count);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token cleanup service stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}
