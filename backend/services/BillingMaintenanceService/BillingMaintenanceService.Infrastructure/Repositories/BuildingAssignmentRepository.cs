using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.Infrastructure.Repositories;

public class BuildingAssignmentRepository : IBuildingAssignmentRepository
{
    private readonly AppDbContext _context;

    public BuildingAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BuildingAssignment?> GetByIdAsync(int id)
    {
        return await _context.BuildingAssignments
            .Include(ba => ba.User)
            .FirstOrDefaultAsync(ba => ba.Id == id && !ba.IsDeleted);
    }

    public async Task<IEnumerable<BuildingAssignment>> GetAllAsync()
    {
        return await _context.BuildingAssignments
            .Include(ba => ba.User)
            .Where(ba => !ba.IsDeleted)
            .OrderBy(ba => ba.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BuildingAssignment>> GetByUserIdAsync(int userId)
    {
        return await _context.BuildingAssignments
            .Where(ba => ba.UserId == userId && !ba.IsDeleted)
            .OrderBy(ba => ba.BuildingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<BuildingAssignment>> GetByBuildingIdAsync(int buildingId)
    {
        return await _context.BuildingAssignments
            .Include(ba => ba.User)
            .Where(ba => ba.BuildingId == buildingId && !ba.IsDeleted)
            .OrderBy(ba => ba.User.FullName)
            .ToListAsync();
    }

    public async Task<BuildingAssignment?> GetByUserAndBuildingAsync(int userId, int buildingId)
    {
        return await _context.BuildingAssignments
            .FirstOrDefaultAsync(ba => ba.UserId == userId && ba.BuildingId == buildingId && !ba.IsDeleted);
    }

    public async Task<BuildingAssignment> AddAsync(BuildingAssignment assignment)
    {
        _context.BuildingAssignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task UpdateAsync(BuildingAssignment assignment)
    {
        assignment.UpdatedAt = DateTime.UtcNow;
        _context.BuildingAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BuildingAssignment assignment)
    {
        assignment.IsDeleted = true;
        assignment.DeletedAt = DateTime.UtcNow;
        _context.BuildingAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }
}
