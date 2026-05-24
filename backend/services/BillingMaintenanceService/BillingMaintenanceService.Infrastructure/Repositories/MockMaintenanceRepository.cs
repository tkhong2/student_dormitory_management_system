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
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000101"), Description = "Sửa bóng đèn phòng 101", Status = MaintenanceStatus.Pending, CreatedAt = new DateTime(2026, 5, 10), Note = "Chưa xử lý" },
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000102"), Description = "Sửa điều hòa phòng 102", Status = MaintenanceStatus.InProgress, CreatedAt = new DateTime(2026, 5, 12), Note = "Đang sửa" },
                new MaintenanceRequest { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), RoomId = Guid.Parse("40000000-0000-0000-0000-000000000103"), Description = "Sửa cửa phòng 103", Status = MaintenanceStatus.Completed, CreatedAt = new DateTime(2026, 5, 5), Note = "Đã hoàn thành" }
            };
        }
    }
}
