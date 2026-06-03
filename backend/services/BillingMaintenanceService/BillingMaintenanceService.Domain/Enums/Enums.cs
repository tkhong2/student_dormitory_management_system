namespace BillingMaintenanceService.Domain.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Staff = 2,
        Student = 3
    }

    public enum BillStatus
    {
        Unpaid = 1,
        Paid = 2,
        Overdue = 3,
        Cancelled = 4
    }

    public enum PaymentMethod
    {
        Cash = 1,
        BankTransfer = 2,
        Online = 3,
        Other = 4
    }

    public enum MaintenanceStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum DebtStatus
    {
        Open = 1,
        PartiallyPaid = 2,
        Paid = 3,
        Overdue = 4,
        Cancelled = 5
    }
}
