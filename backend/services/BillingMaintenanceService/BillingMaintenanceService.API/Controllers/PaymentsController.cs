using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentsController(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var dtos = payments.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(MapToDto(payment));
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByInvoiceId(int invoiceId)
        {
            var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId);
            var dtos = payments.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
        {
            // Get invoice to update
            var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
            if (invoice == null) return NotFound("Invoice not found");

            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                Method = dto.Method,
                TransactionCode = dto.TransactionCode,
                BankName = dto.BankName,
                BankAccountNumber = dto.BankAccountNumber,
                PaymentDate = dto.PaymentDate,
                PaidAt = DateTime.UtcNow,
                ReceivedByUserId = dto.ReceivedByUserId,
                ReceivedByName = dto.ReceivedByName,
                ReceiptImageUrl = dto.ReceiptImageUrl,
                Note = dto.Note
            };

            await _paymentRepository.AddAsync(payment);

            // Update invoice status
            invoice.PaidAmount += dto.Amount;
            invoice.DebtAmount = invoice.TotalAmount - invoice.PaidAmount;
            
            if (invoice.DebtAmount <= 0)
            {
                invoice.Status = "Paid";
                invoice.DebtAmount = 0;
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.Status = "PartialPaid";
            }

            await _invoiceRepository.UpdateAsync(invoice);

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, MapToDto(payment));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] PaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();

            payment.Method = dto.Method;
            payment.TransactionCode = dto.TransactionCode;
            payment.BankName = dto.BankName;
            payment.PaymentDate = dto.PaymentDate;
            payment.ReceiptImageUrl = dto.ReceiptImageUrl;
            payment.Note = dto.Note;

            await _paymentRepository.UpdateAsync(payment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();

            // Get invoice to update
            var invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId);
            if (invoice != null)
            {
                invoice.PaidAmount -= payment.Amount;
                invoice.DebtAmount = invoice.TotalAmount - invoice.PaidAmount;
                
                if (invoice.DebtAmount > 0 && invoice.PaidAmount > 0)
                {
                    invoice.Status = "PartialPaid";
                }
                else if (invoice.PaidAmount <= 0)
                {
                    invoice.Status = "Unpaid";
                }

                await _invoiceRepository.UpdateAsync(invoice);
            }

            await _paymentRepository.DeleteAsync(payment);
            return NoContent();
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                InvoiceCode = payment.Invoice?.InvoiceCode ?? "",
                Amount = payment.Amount,
                Method = payment.Method,
                TransactionCode = payment.TransactionCode,
                BankName = payment.BankName,
                PaymentDate = payment.PaymentDate,
                PaidAt = payment.PaidAt,
                ReceivedByUserId = payment.ReceivedByUserId,
                ReceivedByName = payment.ReceivedByName,
                ReceiptImageUrl = payment.ReceiptImageUrl,
                Note = payment.Note,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}
