using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// The liveness check
app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    // This liveness check filters out all health checks by returning false,
    // so no health checks would run for this endpoint
    Predicate = _ => false
});

app.Run();