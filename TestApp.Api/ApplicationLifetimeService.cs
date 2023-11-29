namespace TestApp.Api;

// IHostedService interface provides a mechanism for tasks that run in the background throughout
// the lifetime of the application
public class ApplicationLifetimeService(IHostApplicationLifetime applicationLifetime,
    ILogger<ApplicationLifetimeService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Register a callback that sleeps for 30 seconds
        applicationLifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("SIGTERM received, waiting 10 seconds.");
            Thread.Sleep(10_000);
            logger.LogInformation("Termination delay complete, continuing stopping process.");
        });
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}