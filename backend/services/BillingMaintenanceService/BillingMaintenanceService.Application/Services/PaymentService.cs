using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using System.Linq;

namespace BillingMaintenanceService.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentService(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<PaymentDto> ProcessPaymentAsync(int invoiceId, decimal amount, string method, string? note, int receivedByUserId, string receivedByName)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new Exception("Invoice not found.");
            }

            if (amount <= 0)
            {
                throw new Exception("Payment amount must be greater than zero.");
            }

            if (amount > invoice.DebtAmount)
            {
                throw new Exception("Payment amount cannot exceed the remaining debt.");
            }

            // Create Payment record
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                Method = string.IsNullOrWhiteSpace(method) ? "Cash" : method,
                PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
                PaidAt = DateTime.UtcNow,
                ReceivedByUserId = receivedByUserId,
                ReceivedByName = receivedByName,
                Note = note
            };

            await _paymentRepository.AddAsync(payment);

            // Update Invoice Status and Debt
            invoice.PaidAmount += amount;
            invoice.DebtAmount -= amount;

            if (invoice.DebtAmount <= 0)
            {
                invoice.Status = "Paid";
                invoice.DebtAmount = 0;
            }
            else
            {
                invoice.Status = "PartiallyPaid";
            }
            
            invoice.UpdatedAt = DateTime.UtcNow;
            await _invoiceRepository.UpdateAsync(invoice);

            return MapToDto(payment);
        }

        public async Task<decimal> GetTotalDebtByStudentIdAsync(int studentId)
        {
            var invoices = await _invoiceRepository.GetByStudentIdAsync(studentId);
            // Sum all remaining debt from unpaid or partially paid invoices
            return invoices.Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                           .Sum(i => i.DebtAmount);
        }

        public async Task<decimal> GetTotalDebtByContractIdAsync(int contractId)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            return invoices.Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                           .Sum(i => i.DebtAmount);
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                Method = payment.Method,
                TransactionCode = payment.TransactionCode,
                BankName = payment.BankName,
                BankAccountNumber = payment.BankAccountNumber,
                PaymentDate = payment.PaymentDate,
                PaidAt = payment.PaidAt,
                ReceivedByUserId = payment.ReceivedByUserId,
                ReceivedByName = payment.ReceivedByName,
                ReceiptImageUrl = payment.ReceiptImageUrl,
                Note = payment.Note
            };
        }
    }
}
