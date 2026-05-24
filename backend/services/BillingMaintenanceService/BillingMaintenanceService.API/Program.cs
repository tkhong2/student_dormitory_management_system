using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register fake repositories for Billing/Maintenance
builder.Services.AddSingleton<IBillRepository, MockBillRepository>();
builder.Services.AddSingleton<IPaymentRepository, MockPaymentRepository>();
builder.Services.AddSingleton<IMaintenanceRequestRepository, MockMaintenanceRequestRepository>();

// Enable CORS for local frontend development
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

// Configure the HTTP request pipeline.
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
