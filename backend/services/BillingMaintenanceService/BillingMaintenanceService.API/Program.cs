using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;
using BillingMaintenanceService.Infrastructure.Auth;
using BillingMaintenanceService.Infrastructure.Repositories;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AppDbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BillingMaintenanceDatabase"),
        sqlOptions => sqlOptions.EnableRetryOnFailure());
});

// Configure JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");
var jwtExpires = jwtSection.GetValue<int?>("ExpiresMinutes") ?? 60;
if (!string.IsNullOrEmpty(jwtKey))
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true
        };
    });
    builder.Services.AddAuthorization();
}

// Register EF repositories for Billing/Maintenance
builder.Services.AddScoped<IBillRepository, EfBillRepository>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();
builder.Services.AddScoped<IMaintenanceRequestRepository, EfMaintenanceRequestRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedDefaultUsers(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void SeedDefaultUsers(AppDbContext db)
{
    SeedUser(db, "admin", "Admin@123", "System Admin", "admin@dorm.local", UserRole.Admin);
    SeedUser(db, "staff", "Staff@123", "Default Staff", "staff@dorm.local", UserRole.Staff);
    SeedUser(db, "student", "Student@123", "Default Student", "student@dorm.local", UserRole.Student);
    db.SaveChanges();
}

static void SeedUser(AppDbContext db, string username, string password, string fullName, string email, UserRole role)
{
    if (db.Users.Any(u => u.Username == username))
        return;

    db.Users.Add(new User
    {
        Id = Guid.NewGuid(),
        Username = username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        FullName = fullName,
        Email = email,
        Role = role,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    });
}
