using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Application.Interfaces;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class MockBillRepository : IBillRepository
    {
        private readonly List<Bill> _bills;
        public MockBillRepository()
        {
            _bills = new List<Bill>
            {
                // 12 hóa đơn chưa thanh toán, tổng 15.2M
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000001"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000001"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000101"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000002"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000002"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000102"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000003"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000003"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000103"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000004"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000004"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000104"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000005"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000005"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000105"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000006"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000006"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000106"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000007"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000007"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000107"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000008"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000008"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000108"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000009"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000009"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000109"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000010"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000010"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000110"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000011"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000011"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000111"), Amount = 1200000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000012"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000012"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000112"), Amount = 2000000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },

                // 3 hóa đơn quá hạn, tổng 2.5M
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000101"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000101"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000101"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000201"), Amount = 1000000, DueDate = new DateTime(2026,5,15), Status = Domain.Enums.BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000102"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000102"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000102"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000202"), Amount = 800000, DueDate = new DateTime(2026,5,10), Status = Domain.Enums.BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000103"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000103"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000103"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000203"), Amount = 700000, DueDate = new DateTime(2026,5,5), Status = Domain.Enums.BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026,5,1) },

                // Các hóa đơn đã thanh toán để tổng thu tháng này bằng 125.4M
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000201"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000201"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000201"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000301"), Amount = 10000000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000202"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000202"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000202"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000302"), Amount = 20000000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000203"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000203"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000203"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000303"), Amount = 30000000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000204"), ContractId = Guid.Parse("20000000-0000-0000-0000-000000000204"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000204"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000304"), Amount = 65400000, DueDate = new DateTime(2026,5,31), Status = Domain.Enums.BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026,5,1) }
            };
        }
        public Task AddAsync(Bill bill)
        {
            _bills.Add(bill);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Bill bill)
        {
            _bills.Remove(bill);
            return Task.CompletedTask;
        }
        public Task<IEnumerable<Bill>> GetAllAsync() => Task.FromResult(_bills.AsEnumerable());
        public Task<Bill?> GetByIdAsync(Guid id) => Task.FromResult(_bills.FirstOrDefault(b => b.Id == id));
        public Task UpdateAsync(Bill bill)
        {
            var idx = _bills.FindIndex(b => b.Id == bill.Id);
            if (idx >= 0) _bills[idx] = bill;
            return Task.CompletedTask;
        }
    }

    public class MockPaymentRepository : IPaymentRepository
    {
        private readonly List<Payment> _payments;
        public MockPaymentRepository()
        {
            _payments = new List<Payment>
            {
                new Payment
                {
                    Id = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
                    BillId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Amount = 1300000,
                    PaymentDate = DateTime.UtcNow.AddDays(-1),
                    PaymentMethod = "Chuyển khoản",
                    TransactionId = "TXN20260501"
                }
            };
        }
        public Task AddAsync(Payment payment)
        {
            _payments.Add(payment);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Payment payment)
        {
            _payments.Remove(payment);
            return Task.CompletedTask;
        }
        public Task<IEnumerable<Payment>> GetAllAsync() => Task.FromResult(_payments.AsEnumerable());
        public Task<Payment?> GetByIdAsync(Guid id) => Task.FromResult(_payments.FirstOrDefault(p => p.Id == id));
        public Task<IEnumerable<Payment>> GetByBillIdAsync(Guid billId) => Task.FromResult(_payments.Where(p => p.BillId == billId).AsEnumerable());
        public Task UpdateAsync(Payment payment)
        {
            var idx = _payments.FindIndex(p => p.Id == payment.Id);
            if (idx >= 0) _payments[idx] = payment;
            return Task.CompletedTask;
        }
    }

    public class MockMaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly List<MaintenanceRequest> _requests;
        public MockMaintenanceRequestRepository()
        {
            _requests = new List<MaintenanceRequest>
            {
                new MaintenanceRequest
                {
                    Id = Guid.Parse("bbbb1111-1111-1111-1111-111111111111"),
                    RoomId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    StudentId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Description = "Sửa điều hòa phòng A101",
                    Status = Domain.Enums.MaintenanceStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
        }
        public Task AddAsync(MaintenanceRequest request)
        {
            _requests.Add(request);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(MaintenanceRequest request)
        {
            _requests.Remove(request);
            return Task.CompletedTask;
        }
        public Task<IEnumerable<MaintenanceRequest>> GetAllAsync() => Task.FromResult(_requests.AsEnumerable());
        public Task<MaintenanceRequest?> GetByIdAsync(Guid id) => Task.FromResult(_requests.FirstOrDefault(r => r.Id == id));
        public Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(Guid roomId) => Task.FromResult(_requests.Where(r => r.RoomId == roomId).AsEnumerable());
        public Task UpdateAsync(MaintenanceRequest request)
        {
            var idx = _requests.FindIndex(r => r.Id == request.Id);
            if (idx >= 0) _requests[idx] = request;
            return Task.CompletedTask;
        }
    }
}