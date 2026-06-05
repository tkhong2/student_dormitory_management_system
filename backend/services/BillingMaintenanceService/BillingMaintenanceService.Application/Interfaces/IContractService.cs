using BillingMaintenanceService.Application.DTOs;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IContractService
    {
        Task<ContractDto?> GetContractByIdAsync(int contractId);
        Task<IEnumerable<ContractDto>> GetContractsByStatusAsync(string status);
    }
}
