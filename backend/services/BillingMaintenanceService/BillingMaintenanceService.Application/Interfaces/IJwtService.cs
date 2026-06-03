using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Application.DTOs;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IJwtService
    {
        JwtTokenResultDto GenerateToken(User user);
    }
}
