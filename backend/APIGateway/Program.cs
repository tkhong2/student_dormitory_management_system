using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add Ocelot configuration - use different config based on environment
var environment = builder.Environment.EnvironmentName;
var ocelotConfigFile = environment == "Production" ? "ocelot.Docker.json" : "ocelot.json";
builder.Configuration.AddJsonFile(ocelotConfigFile, optional: false, reloadOnChange: true);
Console.WriteLine($"🔧 Loading Ocelot config: {ocelotConfigFile}");

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOcelot();

var app = builder.Build();

// Log incoming requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("🌐 API Gateway: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.UseCors("AllowAll");

await app.UseOcelot();

app.Run();
