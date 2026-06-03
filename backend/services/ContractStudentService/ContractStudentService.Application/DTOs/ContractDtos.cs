namespace ContractStudentService.Application.DTOs
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateContractDto
    {
        public string Code { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
