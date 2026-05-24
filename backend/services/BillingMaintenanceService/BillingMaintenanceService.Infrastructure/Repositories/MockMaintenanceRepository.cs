using System;
using System.Collections.Generic;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Infrastructure.Repositories
{
    public class MockMaintenanceRepository
    {
        public List<MaintenanceRequest> Requests { get; }
        public MockMaintenanceRepository()
        {
            Requests = new List<MaintenanceRequest>
            {
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000101"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000001"), Description = "Sửa bóng đèn phòng A101", Status = MaintenanceStatus.Pending, CreatedAt = new DateTime(2026, 5, 10), Note = "Đèn bị hỏng, cần thay ngay" },
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000102"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000002"), Description = "Sửa điều hòa phòng A102", Status = MaintenanceStatus.InProgress, CreatedAt = new DateTime(2026, 5, 12), Note = "Đang chờ kỹ thuật đến kiểm tra" },
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000103"), StudentId = Guid.Parse("30000000-0000-0000-0000-000000000003"), Description = "Sửa cửa phòng A103", Status = MaintenanceStatus.Completed, CreatedAt = new DateTime(2026, 5, 5), Note = "Đã hoàn thành và kiểm tra lại" }
            };
        }
    }
}
