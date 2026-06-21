using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Infrastructure.Auth;
using BillingMaintenanceService.Infrastructure.Data;
using BillingMaintenanceService.Infrastructure.Persistence;
using BillingMaintenanceService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            })
           .ConfigureWarnings(warnings =>
               warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Register services
builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply EF migrations at startup (with retry loop)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var retries = 10;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("✅ BillingMaintenanceDb migration applied successfully.");
            
            // Seed initial data
            await DataSeeder.SeedAsync(db);
            
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"⏳ SQL Server not ready, retrying... {retries} attempts left. Error: {ex.Message}");
            Thread.Sleep(5000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
