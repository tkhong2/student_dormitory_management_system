using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class EfMaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly AppDbContext _context;
        public EfMaintenanceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequests.AsNoTracking().ToListAsync();
        }

        public async Task<MaintenanceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.MaintenanceRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(Guid roomId)
        {
            return await _context.MaintenanceRequests.AsNoTracking().Where(r => r.RoomId == roomId).ToListAsync();
        }

        public async Task UpdateAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
