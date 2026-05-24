using System;
using System.Collections.Generic;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class MockBillingRepository
    {
        public List<Bill> Bills { get; }
        public MockBillingRepository()
        {
            Bills = new List<Bill>
            {
                // 12 hóa đơn chưa thanh toán (tổng 15.2M)
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000101"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000102"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000103"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000104"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000105"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000106"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000107"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000108"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000109"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000110"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000111"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000112"), Amount = 1200000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Unpaid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },

                // 3 hóa đơn quá hạn (tổng 2.5M)
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000101"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000201"), Amount = 1000000, DueDate = new DateTime(2026, 5, 15), Status = BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000102"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000202"), Amount = 800000, DueDate = new DateTime(2026, 5, 10), Status = BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000103"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000203"), Amount = 700000, DueDate = new DateTime(2026, 5, 5), Status = BillStatus.Overdue, Description = "Tiền phòng tháng 5/2026 (quá hạn)", CreatedAt = new DateTime(2026, 5, 1) },

                // Các hóa đơn đã thanh toán để tổng thu tháng này là 125.4M
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000201"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000301"), Amount = 10000000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000202"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000302"), Amount = 20000000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000203"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000303"), Amount = 30000000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) },
                new Bill { Id = Guid.Parse("10000000-0000-0000-0000-000000000204"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000304"), Amount = 65400000, DueDate = new DateTime(2026, 5, 31), Status = BillStatus.Paid, Description = "Tiền phòng tháng 5/2026", CreatedAt = new DateTime(2026, 5, 1) }
            };
        }
    }
}
