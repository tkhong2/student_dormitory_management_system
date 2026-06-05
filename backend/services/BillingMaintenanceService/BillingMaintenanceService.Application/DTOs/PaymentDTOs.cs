namespace BillingMaintenanceService.Application.DTOs
{
    public class ProcessPaymentRequestDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public string? Note { get; set; }
    }
}
