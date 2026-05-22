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
        Active = 1,
        Expired = 2,
        Terminated = 3,
        Renewed = 4
    }
}
