using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestApp.Api.Health;

//https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#create-health-checks
public class RandomHealthCheck : IHealthCheck
{
    private static readonly Random Rnd = new Random();
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var numberFrom0To4 = Rnd.Next(5);
        var isNumberEqualTo0 = numberFrom0To4 == 0; // When this happens, let's make it look unhealthy

        var result = !isNumberEqualTo0
            ? HealthCheckResult.Healthy()
            : new HealthCheckResult(context.Registration.FailureStatus, "Random Health Check Failed!");

        return Task.FromResult(result);
    }
}