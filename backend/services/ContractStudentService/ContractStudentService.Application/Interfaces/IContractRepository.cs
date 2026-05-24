using ContractStudentService.Domain.Entities;

namespace ContractStudentService.Application.Interfaces
{
    public interface IContractRepository
    {
        Task<Contract?> GetByIdAsync(Guid id);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(Contract contract);
    }
}
