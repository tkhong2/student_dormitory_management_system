using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class EfPaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public EfPaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.AsNoTracking().ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByBillIdAsync(Guid billId)
        {
            return await _context.Payments.AsNoTracking().Where(p => p.BillId == billId).ToListAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
