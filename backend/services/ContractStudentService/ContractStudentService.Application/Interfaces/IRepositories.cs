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
        Task<IEnumerable<RoomApplication>> GetByUserIdAsync(int userId);
        Task<IEnumerable<RoomApplication>> GetByStatusAsync(string status);
        Task<IEnumerable<RoomApplication>> GetActiveApplicationsByStudentAsync(int studentId);
        Task<IEnumerable<RoomApplication>> GetActiveApplicationsByUserIdAsync(int userId);
        Task AddAsync(RoomApplication application);
        Task UpdateAsync(RoomApplication application);
        Task DeleteAsync(RoomApplication application);
    }

    public interface IContractRepository
    {
        Task<Contract?> GetByIdAsync(int id);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<IEnumerable<Contract>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Contract>> GetByUserIdAsync(int userId);
        Task<Contract?> GetByContractCodeAsync(string contractCode);
        Task<IEnumerable<Contract>> GetByStatusAsync(string status);
        Task<IEnumerable<Contract>> GetActiveContractsByStudentAsync(int studentId);
        Task<IEnumerable<Contract>> GetActiveContractsByUserIdAsync(int userId);
        Task<int> GetNextSequenceForYearAsync(int year);
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

    public interface IContractTemplateRepository
    {
        Task<ContractTemplate?> GetByIdAsync(int id);
        Task<IEnumerable<ContractTemplate>> GetAllAsync();
        Task<ContractTemplate?> GetByCodeAsync(string code);
        Task<ContractTemplate?> GetDefaultAsync();
        Task<IEnumerable<ContractTemplate>> GetActiveAsync();
        Task AddAsync(ContractTemplate template);
        Task UpdateAsync(ContractTemplate template);
        Task DeleteAsync(ContractTemplate template);
    }

    public interface IContractTermRepository
    {
        Task<ContractTerm?> GetByIdAsync(int id);
        Task<IEnumerable<ContractTerm>> GetAllAsync();
        Task<IEnumerable<ContractTerm>> GetByTemplateIdAsync(int templateId);
        Task AddAsync(ContractTerm term);
        Task UpdateAsync(ContractTerm term);
        Task DeleteAsync(ContractTerm term);
    }

    public interface ICheckInRepository
    {
        Task<CheckIn?> GetByIdAsync(int id);
        Task<IEnumerable<CheckIn>> GetAllAsync();
        Task<CheckIn?> GetByContractIdAsync(int contractId);
        Task<IEnumerable<CheckIn>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<CheckIn>> GetByRoomIdAsync(int roomId);
        Task AddAsync(CheckIn checkIn);
        Task UpdateAsync(CheckIn checkIn);
        Task DeleteAsync(CheckIn checkIn);
    }

    public interface ICheckOutRepository
    {
        Task<CheckOut?> GetByIdAsync(int id);
        Task<IEnumerable<CheckOut>> GetAllAsync();
        Task<CheckOut?> GetByContractIdAsync(int contractId);
        Task<IEnumerable<CheckOut>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<CheckOut>> GetByRoomIdAsync(int roomId);
        Task AddAsync(CheckOut checkOut);
        Task UpdateAsync(CheckOut checkOut);
        Task DeleteAsync(CheckOut checkOut);
    }
}
