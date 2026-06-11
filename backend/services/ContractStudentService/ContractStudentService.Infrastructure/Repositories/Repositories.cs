using Microsoft.EntityFrameworkCore;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using ContractStudentService.Infrastructure.Persistence;

namespace ContractStudentService.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        public StudentRepository(AppDbContext context) => _context = context;

        public async Task<Student?> GetByIdAsync(int id) => 
            await _context.Students
                .Include(s => s.Documents)
                .Include(s => s.Applications)
                .Include(s => s.Contracts)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Student>> GetAllAsync() => 
            await _context.Students
                .Include(s => s.Documents)
                .ToListAsync();

        public async Task<Student?> GetByStudentCodeAsync(string studentCode) => 
            await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);

        public async Task<Student?> GetByUserIdAsync(int userId) => 
            await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

        public async Task AddAsync(Student student) 
        { 
            await _context.Students.AddAsync(student); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(Student student) 
        { 
            _context.Students.Update(student); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(Student student) 
        { 
            _context.Students.Remove(student); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class StudentDocumentRepository : IStudentDocumentRepository
    {
        private readonly AppDbContext _context;
        public StudentDocumentRepository(AppDbContext context) => _context = context;

        public async Task<StudentDocument?> GetByIdAsync(int id) => 
            await _context.StudentDocuments.FindAsync(id);

        public async Task<IEnumerable<StudentDocument>> GetAllAsync() => 
            await _context.StudentDocuments.ToListAsync();

        public async Task<IEnumerable<StudentDocument>> GetByStudentIdAsync(int studentId) => 
            await _context.StudentDocuments.Where(d => d.StudentId == studentId).ToListAsync();

        public async Task AddAsync(StudentDocument document) 
        { 
            await _context.StudentDocuments.AddAsync(document); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(StudentDocument document) 
        { 
            _context.StudentDocuments.Update(document); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(StudentDocument document) 
        { 
            _context.StudentDocuments.Remove(document); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class RoomApplicationRepository : IRoomApplicationRepository
    {
        private readonly AppDbContext _context;
        public RoomApplicationRepository(AppDbContext context) => _context = context;

        public async Task<RoomApplication?> GetByIdAsync(int id) => 
            await _context.RoomApplications
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<RoomApplication>> GetAllAsync() => 
            await _context.RoomApplications
                .Include(a => a.Student)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetByStudentIdAsync(int studentId) => 
            await _context.RoomApplications
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetByUserIdAsync(int userId) =>
            await _context.RoomApplications
                .Include(a => a.Student)
                .Where(a => a.Student.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetByStatusAsync(string status) => 
            await _context.RoomApplications
                .Include(a => a.Student)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetActiveApplicationsByStudentAsync(int studentId) =>
            await _context.RoomApplications
                .Where(a => a.StudentId == studentId && (a.Status == "Pending" || a.Status == "Approved"))
                .ToListAsync();

        public async Task<IEnumerable<RoomApplication>> GetActiveApplicationsByUserIdAsync(int userId) =>
            await _context.RoomApplications
                .Include(a => a.Student)
                .Where(a => a.Student.UserId == userId && (a.Status == "Pending" || a.Status == "Approved"))
                .ToListAsync();

        public async Task AddAsync(RoomApplication application) 
        { 
            await _context.RoomApplications.AddAsync(application); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(RoomApplication application) 
        { 
            _context.RoomApplications.Update(application); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(RoomApplication application) 
        { 
            _context.RoomApplications.Remove(application); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class ContractRepository : IContractRepository
    {
        private readonly AppDbContext _context;
        public ContractRepository(AppDbContext context) => _context = context;

        public async Task<Contract?> GetByIdAsync(int id) => 
            await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.Application)
                .Include(c => c.Extensions)
                .Include(c => c.RoomTransfers)
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Contract>> GetAllAsync() => 
            await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Contract>> GetByStudentIdAsync(int studentId) => 
            await _context.Contracts
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Contract>> GetByUserIdAsync(int userId) =>
            await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .Where(c => c.Student.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<Contract?> GetByContractCodeAsync(string contractCode) => 
            await _context.Contracts
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .FirstOrDefaultAsync(c => c.ContractCode == contractCode);

        public async Task<IEnumerable<Contract>> GetByStatusAsync(string status) => 
            await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Contract>> GetActiveContractsByStudentAsync(int studentId) =>
            await _context.Contracts
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .Where(c => c.StudentId == studentId && c.Status == "Active")
                .ToListAsync();

        public async Task<IEnumerable<Contract>> GetActiveContractsByUserIdAsync(int userId) =>
            await _context.Contracts
                .Include(c => c.Student)
                .Include(c => c.ContractTemplate)
                    .ThenInclude(t => t.Terms)
                .Where(c => c.Student.UserId == userId && c.Status == "Active")
                .ToListAsync();

        public async Task<int> GetNextSequenceForYearAsync(int year)
        {
            var lastContract = await _context.Contracts
                .Where(c => c.ContractCode.StartsWith($"HD{year}"))
                .OrderByDescending(c => c.ContractCode)
                .FirstOrDefaultAsync();

            if (lastContract == null)
                return 1;

            // Extract sequence number from HD2024001 -> 001 -> 1
            var sequenceStr = lastContract.ContractCode.Substring(6);
            if (int.TryParse(sequenceStr, out int sequence))
                return sequence + 1;

            return 1;
        }

        public async Task AddAsync(Contract contract) 
        { 
            await _context.Contracts.AddAsync(contract); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(Contract contract) 
        { 
            _context.Contracts.Update(contract); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(Contract contract) 
        { 
            _context.Contracts.Remove(contract); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class ContractExtensionRepository : IContractExtensionRepository
    {
        private readonly AppDbContext _context;
        public ContractExtensionRepository(AppDbContext context) => _context = context;

        public async Task<ContractExtension?> GetByIdAsync(int id) => 
            await _context.ContractExtensions.FindAsync(id);

        public async Task<IEnumerable<ContractExtension>> GetAllAsync() => 
            await _context.ContractExtensions.ToListAsync();

        public async Task<IEnumerable<ContractExtension>> GetByContractIdAsync(int contractId) => 
            await _context.ContractExtensions
                .Where(e => e.ContractId == contractId)
                .OrderByDescending(e => e.ApprovedAt)
                .ToListAsync();

        public async Task AddAsync(ContractExtension extension) 
        { 
            await _context.ContractExtensions.AddAsync(extension); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(ContractExtension extension) 
        { 
            _context.ContractExtensions.Update(extension); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(ContractExtension extension) 
        { 
            _context.ContractExtensions.Remove(extension); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class RoomTransferRepository : IRoomTransferRepository
    {
        private readonly AppDbContext _context;
        public RoomTransferRepository(AppDbContext context) => _context = context;

        public async Task<RoomTransfer?> GetByIdAsync(int id) => 
            await _context.RoomTransfers
                .Include(t => t.Contract)
                    .ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<RoomTransfer>> GetAllAsync() => 
            await _context.RoomTransfers
                .Include(t => t.Contract)
                    .ThenInclude(c => c.Student)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomTransfer>> GetByContractIdAsync(int contractId) => 
            await _context.RoomTransfers
                .Where(t => t.ContractId == contractId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomTransfer>> GetByStudentIdAsync(int studentId) => 
            await _context.RoomTransfers
                .Include(t => t.Contract)
                .Where(t => t.Contract.StudentId == studentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<RoomTransfer>> GetByStatusAsync(string status) => 
            await _context.RoomTransfers
                .Include(t => t.Contract)
                    .ThenInclude(c => c.Student)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(RoomTransfer transfer) 
        { 
            await _context.RoomTransfers.AddAsync(transfer); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(RoomTransfer transfer) 
        { 
            _context.RoomTransfers.Update(transfer); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(RoomTransfer transfer) 
        { 
            _context.RoomTransfers.Remove(transfer); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class ContractTemplateRepository : IContractTemplateRepository
    {
        private readonly AppDbContext _context;
        public ContractTemplateRepository(AppDbContext context) => _context = context;

        public async Task<ContractTemplate?> GetByIdAsync(int id) => 
            await _context.ContractTemplates
                .Include(t => t.Terms.OrderBy(term => term.OrderIndex))
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<ContractTemplate>> GetAllAsync() => 
            await _context.ContractTemplates
                .Include(t => t.Terms.OrderBy(term => term.OrderIndex))
                .OrderBy(t => t.Name)
                .ToListAsync();

        public async Task<ContractTemplate?> GetByCodeAsync(string code) => 
            await _context.ContractTemplates
                .Include(t => t.Terms.OrderBy(term => term.OrderIndex))
                .FirstOrDefaultAsync(t => t.Code == code);

        public async Task<ContractTemplate?> GetDefaultAsync() => 
            await _context.ContractTemplates
                .Include(t => t.Terms.OrderBy(term => term.OrderIndex))
                .FirstOrDefaultAsync(t => t.IsDefault && t.IsActive);

        public async Task<IEnumerable<ContractTemplate>> GetActiveAsync() => 
            await _context.ContractTemplates
                .Include(t => t.Terms.OrderBy(term => term.OrderIndex))
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

        public async Task AddAsync(ContractTemplate template) 
        { 
            await _context.ContractTemplates.AddAsync(template); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(ContractTemplate template) 
        { 
            _context.ContractTemplates.Update(template); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(ContractTemplate template) 
        { 
            _context.ContractTemplates.Remove(template); 
            await _context.SaveChangesAsync(); 
        }
    }

    public class ContractTermRepository : IContractTermRepository
    {
        private readonly AppDbContext _context;
        public ContractTermRepository(AppDbContext context) => _context = context;

        public async Task<ContractTerm?> GetByIdAsync(int id) => 
            await _context.ContractTerms.FindAsync(id);

        public async Task<IEnumerable<ContractTerm>> GetAllAsync() => 
            await _context.ContractTerms.ToListAsync();

        public async Task<IEnumerable<ContractTerm>> GetByTemplateIdAsync(int templateId) => 
            await _context.ContractTerms
                .Where(t => t.ContractTemplateId == templateId)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();

        public async Task AddAsync(ContractTerm term) 
        { 
            await _context.ContractTerms.AddAsync(term); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(ContractTerm term) 
        { 
            _context.ContractTerms.Update(term); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(ContractTerm term) 
        { 
            _context.ContractTerms.Remove(term); 
            await _context.SaveChangesAsync(); 
        }
    }
}