using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateToken(int userId, string username, string role);
        string GenerateRefreshToken();
    }
}
