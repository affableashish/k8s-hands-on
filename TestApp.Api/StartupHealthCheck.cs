using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestApp.Api;

public class StartupHealthCheck : IHealthCheck
{
    // The volatile keyword indicates that a field might be modified by multiple threads that are executing at the same time.
    private volatile bool _isReady;

    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (StartupCompleted)
        {
            return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("The startup task is still running."));
    }
}