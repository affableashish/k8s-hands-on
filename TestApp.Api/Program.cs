using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TestApp.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// I added this 👇
builder.Services.AddHostedService<StartupBackgroundService>();

// Notice that I'm not registering RandomHealthCheck class.
// I'm doing this only because StartupBackgroundService needs this.
// If that was not the case, this wouldn't have been necessary
builder.Services.AddSingleton<StartupHealthCheck>();

builder.Services.AddHealthChecks()
    .AddCheck<RandomHealthCheck>("Random Check", tags: new[] { "random" })
    .AddCheck<StartupHealthCheck>("Startup Check", tags: new[] { "startup" });
// I added this 👆

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

// I added this 👇
// The liveness check
app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    // This liveness check filters out all health checks by returning false,
    // so no health checks would run for this endpoint
    Predicate = _ => false
});

// The readiness check
app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    // Only health checks with tag of "startup" would run
    Predicate = healthCheck => healthCheck.Tags.Contains("startup")
});

// The random check
app.MapHealthChecks("/healthz/random", new HealthCheckOptions
{
    // Only health checks with tag of "random" would run
    Predicate = healthCheck => healthCheck.Tags.Contains("random")
});
// I added this 👆

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}