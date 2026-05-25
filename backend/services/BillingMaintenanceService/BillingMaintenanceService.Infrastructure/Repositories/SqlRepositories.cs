using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly AppDbContext _context;

        public BillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills.AsNoTracking().ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(Guid id)
        {
            return await _context.Bills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Bill bill)
        {
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
        }
    }

    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
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
            return await _context.Payments.AsNoTracking()
                .Where(p => p.BillId == billId)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }

    public class MaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly AppDbContext _context;

        public MaintenanceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequests.AsNoTracking().ToListAsync();
        }

        public async Task<MaintenanceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.MaintenanceRequests.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(Guid roomId)
        {
            return await _context.MaintenanceRequests.AsNoTracking()
                .Where(r => r.RoomId == roomId)
                .ToListAsync();
        }

        public async Task AddAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
}
