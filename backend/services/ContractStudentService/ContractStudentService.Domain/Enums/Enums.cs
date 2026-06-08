namespace ContractStudentService.Domain.Enums
{
    public enum RegistrationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum ContractStatus
    {
        Draft = 0,           // Hợp đồng nháp (vừa tạo sau khi duyệt đơn)
        PendingDeposit = 1,  // Chờ đóng tiền cọc
        Active = 2,          // Đã đóng cọc, đang hiệu lực
        Expired = 3,         // Hết hạn
        Terminated = 4,      // Đã chấm dứt
        Renewed = 5          // Đã gia hạn
    }
}
