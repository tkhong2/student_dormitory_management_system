using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using ContractStudentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContractStudentService.Infrastructure.Repositories
{
    public class EfContractRepository : IContractRepository
    {
        private readonly AppDbContext _context;
        public EfContractRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Contract contract)
        {
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts.AsNoTracking().ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(Guid id)
        {
            return await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }
    }
}
