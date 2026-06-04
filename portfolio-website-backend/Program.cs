namespace portfolio_website_backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        // Define CORS policy
        var allowedOrigins =
            builder.Configuration.GetValue<string>("AllowedOrigins")?.Split(';')
            ?? Array.Empty<string>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "DefaultPolicy",
                policy =>
                {
                    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseRouting();
        app.UseCors("DefaultPolicy");
        app.UseAuthorization();

        app.MapPost(
            "/api/ping",
            async (PingRequest request) =>
            {
                // Simulate processing with jitter (1.5s - 2.5s)
                var jitterDelay = 1500 + Random.Shared.Next(0, 1000);
                await Task.Delay(jitterDelay);

                return Results.Ok(new PingResponse($"Echo: {request.Message}", DateTime.UtcNow));
            }
        );

        app.Run();
    }

    public record PingRequest(string Message);

    public record PingResponse(String Response, DateTime Timestamp);
}
