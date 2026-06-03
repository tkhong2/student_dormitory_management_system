using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class EfBillRepository : IBillRepository
    {
        private readonly AppDbContext _context;
        public EfBillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Bill bill)
        {
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills.AsNoTracking().ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(Guid id)
        {
            return await _context.Bills
                .Include(b => b.Payments)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }
    }
}
