namespace ErtamIK.WebPortal.Services;

public sealed class DatabaseInitializer(
    IServiceProvider services,
    ILogger<DatabaseInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<PortalRepository>()
                .InitializeAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Veritabanı başlangıç işlemleri tamamlanamadı.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
