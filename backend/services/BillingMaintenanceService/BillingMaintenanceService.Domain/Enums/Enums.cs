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

    public enum MaintenanceStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }
}
