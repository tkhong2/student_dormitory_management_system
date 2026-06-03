using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories;

public class MaintenanceRequestRepository : IMaintenanceRequestRepository
{
    private readonly AppDbContext _context;

    public MaintenanceRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceRequest?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceRequests
            .Include(m => m.AssignedToUser)
            .Include(m => m.StatusLogs)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
    {
        return await _context.MaintenanceRequests
            .Include(m => m.AssignedToUser)
            .Include(m => m.StatusLogs)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(int roomId)
    {
        return await _context.MaintenanceRequests
            .Include(m => m.AssignedToUser)
            .Include(m => m.StatusLogs)
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRequest>> GetByStudentIdAsync(int studentId)
    {
        return await _context.MaintenanceRequests
            .Include(m => m.AssignedToUser)
            .Include(m => m.StatusLogs)
            .Where(m => m.RequestedByStudentId == studentId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<MaintenanceRequest> AddAsync(MaintenanceRequest request)
    {
        _context.MaintenanceRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
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
