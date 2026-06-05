using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IContractService _contractService;

        public InvoiceService(IInvoiceRepository invoiceRepository, IContractService contractService)
        {
            _invoiceRepository = invoiceRepository;
            _contractService = contractService;
        }

        public async Task<InvoiceDto> GenerateMonthlyInvoiceAsync(int contractId, int billingMonth, int billingYear, int createdByUserId)
        {
            var contract = await _contractService.GetContractByIdAsync(contractId);
            if (contract == null)
            {
                throw new InvalidOperationException($"Contract '{contractId}' not found.");
            }

            if (!string.Equals(contract.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Monthly invoices can only be generated for active contracts.");
            }

            if (!IsContractActiveForMonth(contract, billingMonth, billingYear))
            {
                throw new InvalidOperationException("Contract does not cover the requested billing period.");
            }

            var existingInvoice = await _invoiceRepository.GetByContractIdAndBillingPeriodAsync(contractId, billingMonth, billingYear);
            if (existingInvoice != null)
            {
                throw new InvalidOperationException("A monthly invoice for this contract and billing period already exists.");
            }

            var previousDebt = await CalculatePreviousDebtAsync(contractId, billingMonth, billingYear);
            var dueDate = CalculateDueDate(contract.PaymentDueDay, billingYear, billingMonth);
            var invoice = new Invoice
            {
                InvoiceCode = $"INV-{contract.ContractCode}-{billingYear}{billingMonth:D2}",
                InvoiceType = "Monthly",
                ContractId = contract.Id,
                StudentId = contract.StudentId,
                StudentName = contract.StudentName ?? string.Empty,
                StudentCode = contract.StudentCode ?? string.Empty,
                RoomId = contract.RoomId,
                RoomNumber = contract.RoomNumber,
                BuildingName = contract.BuildingName,
                BillingMonth = billingMonth,
                BillingYear = billingYear,
                RentAmount = contract.MonthlyRent,
                ElectricityAmount = 0m,
                WaterAmount = 0m,
                ServiceAmount = 0m,
                PreviousDebt = previousDebt,
                Discount = 0m,
                DueDate = dueDate,
                CreatedByUserId = createdByUserId,
                Notes = $"Auto-generated monthly invoice for contract {contract.ContractCode}.",
                Status = "Unpaid"
            };

            invoice.TotalAmount = invoice.RentAmount + invoice.ElectricityAmount + invoice.WaterAmount + invoice.ServiceAmount + invoice.PreviousDebt - invoice.Discount;
            invoice.DebtAmount = invoice.TotalAmount;

            invoice.Items.Add(new InvoiceItem
            {
                ItemName = "Phí thuê phòng",
                ItemDescription = $"Phí thuê nhà tháng {billingMonth}/{billingYear}",
                Quantity = 1,
                Unit = "Tháng",
                UnitPrice = contract.MonthlyRent,
                Amount = contract.MonthlyRent,
                SortOrder = 1
            });

            if (previousDebt > 0)
            {
                invoice.Items.Add(new InvoiceItem
                {
                    ItemName = "Nợ kỳ trước",
                    ItemDescription = "Số tiền chưa thanh toán từ các kỳ trước",
                    Quantity = 1,
                    Unit = "Lần",
                    UnitPrice = previousDebt,
                    Amount = previousDebt,
                    SortOrder = 2
                });
            }

            await _invoiceRepository.AddAsync(invoice);
            return MapToDto(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GenerateMonthlyInvoicesForMonthAsync(int billingMonth, int billingYear, int createdByUserId)
        {
            var contracts = await _contractService.GetContractsByStatusAsync("Active");
            var invoices = new List<InvoiceDto>();

            foreach (var contract in contracts)
            {
                if (!IsContractActiveForMonth(contract, billingMonth, billingYear))
                {
                    continue;
                }

                var existingInvoice = await _invoiceRepository.GetByContractIdAndBillingPeriodAsync(contract.Id, billingMonth, billingYear);
                if (existingInvoice != null)
                {
                    continue;
                }

                try
                {
                    invoices.Add(await GenerateMonthlyInvoiceAsync(contract.Id, billingMonth, billingYear, createdByUserId));
                }
                catch
                {
                    // Skip contracts that cannot be invoiced, but continue processing the rest.
                }
            }

            return invoices;
        }

        public async Task<int> ProcessOverdueInvoicesAsync(int reminderIntervalDays)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var overdueInvoices = await _invoiceRepository.GetPendingOverdueInvoicesAsync(today);
            var updatedCount = 0;

            foreach (var invoice in overdueInvoices)
            {
                var daysOverdue = (DateTime.UtcNow.Date - invoice.DueDate.ToDateTime(new TimeOnly(0, 0))).Days;
                invoice.OverdueDays = Math.Max(daysOverdue, 0);
                invoice.Status = "Overdue";

                if (invoice.LastReminderSentAt == null || (DateTime.UtcNow - invoice.LastReminderSentAt.Value).TotalDays >= reminderIntervalDays)
                {
                    invoice.ReminderCount += 1;
                    invoice.LastReminderSentAt = DateTime.UtcNow;
                }

                invoice.UpdatedAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(invoice);
                updatedCount++;
            }

            return updatedCount;
        }

        private async Task<decimal> CalculatePreviousDebtAsync(int contractId, int billingMonth, int billingYear)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            return invoices
                .Where(i => i.BillingYear < billingYear || (i.BillingYear == billingYear && i.BillingMonth < billingMonth))
                .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                .Sum(i => i.DebtAmount);
        }

        private static DateOnly CalculateDueDate(int paymentDueDay, int billingYear, int billingMonth)
        {
            var lastDay = DateTime.DaysInMonth(billingYear, billingMonth);
            var dueDay = Math.Clamp(paymentDueDay, 1, lastDay);
            return DateOnly.FromDateTime(new DateTime(billingYear, billingMonth, dueDay));
        }

        private static bool IsContractActiveForMonth(ContractDto contract, int billingMonth, int billingYear)
        {
            var periodStart = new DateOnly(billingYear, billingMonth, 1);
            var periodEnd = new DateOnly(billingYear, billingMonth, DateTime.DaysInMonth(billingYear, billingMonth));
            return contract.StartDate <= periodEnd && contract.EndDate >= periodStart;
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceType = invoice.InvoiceType,
                ContractId = invoice.ContractId,
                StudentId = invoice.StudentId,
                StudentName = invoice.StudentName,
                StudentCode = invoice.StudentCode,
                RoomId = invoice.RoomId,
                RoomNumber = invoice.RoomNumber,
                BuildingName = invoice.BuildingName,
                BillingMonth = invoice.BillingMonth,
                BillingYear = invoice.BillingYear,
                RentAmount = invoice.RentAmount,
                ElectricityAmount = invoice.ElectricityAmount,
                WaterAmount = invoice.WaterAmount,
                ServiceAmount = invoice.ServiceAmount,
                PreviousDebt = invoice.PreviousDebt,
                Discount = invoice.Discount,
                PenaltyAmount = invoice.PenaltyAmount,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                DebtAmount = invoice.DebtAmount,
                Status = invoice.Status,
                DueDate = invoice.DueDate,
                OverdueDays = invoice.OverdueDays,
                Notes = invoice.Notes,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    Id = i.Id,
                    InvoiceId = i.InvoiceId,
                    ItemName = i.ItemName,
                    ItemDescription = i.ItemDescription,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    UnitPrice = i.UnitPrice,
                    Amount = i.Amount,
                    SortOrder = i.SortOrder
                }).ToList()
            };
        }
    }
}
