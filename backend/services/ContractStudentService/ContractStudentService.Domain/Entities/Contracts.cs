using ContractStudentService.Domain.Enums;

namespace ContractStudentService.Domain.Entities
{
    public class Registration
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid RoomId { get; set; } // Reference ID from RoomBuildingService
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        public Student? Student { get; set; }
    }

    public class Contract
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public Guid RoomId { get; set; } // Reference ID from RoomBuildingService
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Student? Student { get; set; }
    }
}
