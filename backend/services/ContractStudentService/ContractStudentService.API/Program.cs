using ContractStudentService.Application.Interfaces;
using ContractStudentService.Infrastructure.Persistence;
using ContractStudentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for inter-service communication
builder.Services.AddHttpClient();

// Register repositories
builder.Services.AddScoped<IStudentRepository, ContractStudentService.Infrastructure.Repositories.StudentRepository>();
builder.Services.AddScoped<IStudentDocumentRepository, ContractStudentService.Infrastructure.Repositories.StudentDocumentRepository>();
builder.Services.AddScoped<IRoomApplicationRepository, ContractStudentService.Infrastructure.Repositories.RoomApplicationRepository>();
builder.Services.AddScoped<IContractRepository, ContractStudentService.Infrastructure.Repositories.ContractRepository>();
builder.Services.AddScoped<IContractExtensionRepository, ContractStudentService.Infrastructure.Repositories.ContractExtensionRepository>();
builder.Services.AddScoped<IRoomTransferRepository, ContractStudentService.Infrastructure.Repositories.RoomTransferRepository>();
builder.Services.AddScoped<IContractTemplateRepository, ContractStudentService.Infrastructure.Repositories.ContractTemplateRepository>();
builder.Services.AddScoped<IContractTermRepository, ContractStudentService.Infrastructure.Repositories.ContractTermRepository>();

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
            Console.WriteLine("✅ ContractStudentDb migration applied successfully.");
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

// Enable static files for uploaded documents
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

app.Run();
