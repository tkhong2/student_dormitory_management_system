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
                new Bill
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    ContractId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    StudentId = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    RoomId = Guid.Parse("40000000-0000-0000-0000-000000000101"),
                    Amount = 800000,
                    DueDate = new DateTime(2026, 5, 31),
                    Status = Domain.Enums.BillStatus.Unpaid,
                    Description = "Tiền phòng tháng 5/2026",
                    CreatedAt = new DateTime(2026, 5, 1)
                },
                new Bill
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    ContractId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    StudentId = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    RoomId = Guid.Parse("40000000-0000-0000-0000-000000000102"),
                    Amount = 1500000,
                    DueDate = new DateTime(2026, 5, 31),
                    Status = Domain.Enums.BillStatus.Unpaid,
                    Description = "Tiền phòng tháng 5/2026",
                    CreatedAt = new DateTime(2026, 5, 1)
                },
                new Bill
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    ContractId = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                    StudentId = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                    RoomId = Guid.Parse("40000000-0000-0000-0000-000000000201"),
                    Amount = 800000,
                    DueDate = new DateTime(2026, 5, 31),
                    Status = Domain.Enums.BillStatus.Overdue,
                    Description = "Tiền phòng tháng 5/2026",
                    CreatedAt = new DateTime(2026, 5, 1)
                },
                new Bill
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    ContractId = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                    StudentId = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                    RoomId = Guid.Parse("40000000-0000-0000-0000-000000000305"),
                    Amount = 2500000,
                    DueDate = new DateTime(2026, 5, 31),
                    Status = Domain.Enums.BillStatus.Paid,
                    Description = "Tiền phòng tháng 5/2026",
                    CreatedAt = new DateTime(2026, 5, 1)
                }
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