using ContractStudentService.Domain.Entities;

namespace ContractStudentService.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByStudentCodeAsync(string studentCode);
        Task<Student?> GetByUserIdAsync(int userId);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
    }

    public interface IStudentDocumentRepository
    {
        Task<StudentDocument?> GetByIdAsync(int id);
        Task<IEnumerable<StudentDocument>> GetAllAsync();
        Task<IEnumerable<StudentDocument>> GetByStudentIdAsync(int studentId);
        Task AddAsync(StudentDocument document);
        Task UpdateAsync(StudentDocument document);
        Task DeleteAsync(StudentDocument document);
    }

    public interface IRoomApplicationRepository
    {
        Task<RoomApplication?> GetByIdAsync(int id);
        Task<IEnumerable<RoomApplication>> GetAllAsync();
        Task<IEnumerable<RoomApplication>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<RoomApplication>> GetByStatusAsync(string status);
        Task AddAsync(RoomApplication application);
        Task UpdateAsync(RoomApplication application);
        Task DeleteAsync(RoomApplication application);
    }

    public interface IContractRepository
    {
        Task<Contract?> GetByIdAsync(int id);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<IEnumerable<Contract>> GetByStudentIdAsync(int studentId);
        Task<Contract?> GetByContractCodeAsync(string contractCode);
        Task<IEnumerable<Contract>> GetByStatusAsync(string status);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(Contract contract);
    }

    public interface IContractExtensionRepository
    {
        Task<ContractExtension?> GetByIdAsync(int id);
        Task<IEnumerable<ContractExtension>> GetAllAsync();
        Task<IEnumerable<ContractExtension>> GetByContractIdAsync(int contractId);
        Task AddAsync(ContractExtension extension);
        Task UpdateAsync(ContractExtension extension);
        Task DeleteAsync(ContractExtension extension);
    }

    public interface IRoomTransferRepository
    {
        Task<RoomTransfer?> GetByIdAsync(int id);
        Task<IEnumerable<RoomTransfer>> GetAllAsync();
        Task<IEnumerable<RoomTransfer>> GetByContractIdAsync(int contractId);
        Task<IEnumerable<RoomTransfer>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<RoomTransfer>> GetByStatusAsync(string status);
        Task AddAsync(RoomTransfer transfer);
        Task UpdateAsync(RoomTransfer transfer);
        Task DeleteAsync(RoomTransfer transfer);
    }
}
