using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using ContractStudentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContractStudentService.Infrastructure.Repositories
{
    public class EfStudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        public EfStudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.AsNoTracking().ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }
    }
}
