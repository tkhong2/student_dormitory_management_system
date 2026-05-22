using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
