namespace BillingMaintenanceService.Application.DTOs
{
    public class JwtTokenResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class BillDto
    {
        public Guid Id { get; set; }
        public string BillCode { get; set; } = string.Empty;
        public Guid ContractId { get; set; }
        public Guid StudentId { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid StudentId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public string? Note { get; set; }
    }

    public class MaintenanceRequestDto
    {
        public Guid Id { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public Guid RoomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest : LoginRequestDto
    {
    }

    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Optional. In Development, the first registered account may set this to Admin/Staff/Student.
        // Otherwise it will default to Student.
        public string? Role { get; set; }
        public Guid? ReferenceId { get; set; }
    }

    public class RegisterRequest : RegisterRequestDto
    {
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserResponse? User { get; set; }
    }

    public class LoginResponse : AuthResponseDto
    {
    }

    public class CreateUserRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SetActiveRequestDto
    {
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangeMyPasswordRequestDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserResponse : UserDto
    {
    }
}
