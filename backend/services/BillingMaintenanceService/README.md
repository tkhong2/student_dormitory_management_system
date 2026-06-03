# Billing & Maintenance Service

Microservice quản lý thanh toán (Invoices, Payments) và yêu cầu bảo trì (Maintenance Requests).

## 🏗️ Kiến trúc

- **Clean Architecture** với 4 layers:
  - `Domain`: Entities, Enums
  - `Application`: DTOs, Interfaces
  - `Infrastructure`: DbContext, Repositories, Auth
  - `API`: Controllers, Program.cs

## 📦 Công nghệ

- **.NET 9.0**
- **Entity Framework Core 9.0** với SQL Server
- **BCrypt.Net** cho password hashing
- **JWT** cho authentication
- **Swagger** cho API documentation

## 🚀 Chạy local

### 1. Cập nhật connection string

Sửa `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BillingMaintenanceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### 2. Chạy migrations

```bash
dotnet ef database update --project BillingMaintenanceService.Infrastructure --startup-project BillingMaintenanceService.API
```

### 3. Chạy service

```bash
dotnet run --project BillingMaintenanceService.API
```

Service sẽ chạy tại: `http://localhost:5002`

Swagger UI: `http://localhost:5002/swagger`

## 📊 Database Schema

### Users
- Quản lý tài khoản Admin, Staff, Student
- Password hashing với BCrypt
- JWT token cho authentication

### Invoices & InvoiceItems
- Phiếu thu tiền hàng tháng (tiền phòng, điện, nước, dịch vụ)
- Chi tiết từng khoản thu
- Trạng thái: Unpaid, PartialPaid, Paid, Overdue, Cancelled

### Payments
- Lịch sử thanh toán cho từng Invoice
- Hỗ trợ nhiều phương thức: Cash, BankTransfer, Momo, VNPay
- Track transaction code và receipt images

### MaintenanceRequests & MaintenanceStatusLogs
- Yêu cầu sửa chữa/bảo trì từ sinh viên
- Assign cho kỹ thuật viên
- Track timeline thay đổi trạng thái
- Đánh giá sau khi hoàn thành

### Notifications
- Thông báo in-app cho users
- Link đến entity liên quan
- Track read/unread status

### SystemSettings
- Cài đặt hệ thống dạng key-value
- Group theo chức năng

### AuditLogs
- Nhật ký hoạt động của users
- Track CREATE, UPDATE, DELETE operations

### ContactInquiries
- Câu hỏi từ trang public (chưa đăng nhập)
- Staff reply và track status

## 🔌 API Endpoints

### Users
- `GET /api/users` - Danh sách users
- `GET /api/users/{id}` - Chi tiết user
- `POST /api/users` - Tạo user mới
- `PUT /api/users/{id}` - Cập nhật user
- `PUT /api/users/{id}/password` - Đổi mật khẩu
- `DELETE /api/users/{id}` - Xóa user

### Invoices
- `GET /api/invoices` - Danh sách invoices
- `GET /api/invoices/{id}` - Chi tiết invoice
- `GET /api/invoices/student/{studentId}` - Invoices của student
- `GET /api/invoices/contract/{contractId}` - Invoices của contract
- `GET /api/invoices/code/{invoiceCode}` - Tìm theo mã phiếu thu
- `POST /api/invoices` - Tạo invoice mới
- `PUT /api/invoices/{id}` - Cập nhật invoice
- `DELETE /api/invoices/{id}` - Xóa invoice

### Payments
- `GET /api/payments` - Danh sách payments
- `GET /api/payments/{id}` - Chi tiết payment
- `GET /api/payments/invoice/{invoiceId}` - Payments của invoice
- `POST /api/payments` - Ghi nhận thanh toán
- `PUT /api/payments/{id}` - Cập nhật payment
- `DELETE /api/payments/{id}` - Xóa payment (tự động cập nhật invoice)

### MaintenanceRequests
- `GET /api/maintenancerequests` - Danh sách requests
- `GET /api/maintenancerequests/{id}` - Chi tiết request
- `GET /api/maintenancerequests/room/{roomId}` - Requests của room
- `GET /api/maintenancerequests/student/{studentId}` - Requests của student
- `POST /api/maintenancerequests` - Tạo request mới
- `PUT /api/maintenancerequests/{id}` - Cập nhật request
- `POST /api/maintenancerequests/{id}/assign` - Assign cho kỹ thuật viên
- `POST /api/maintenancerequests/{id}/start` - Bắt đầu xử lý
- `POST /api/maintenancerequests/{id}/resolve` - Hoàn thành
- `POST /api/maintenancerequests/{id}/reject` - Từ chối
- `POST /api/maintenancerequests/{id}/rate` - Đánh giá
- `DELETE /api/maintenancerequests/{id}` - Xóa request

## 🐳 Docker

Build image:

```bash
docker build -t billing-maintenance-service .
```

Run container:

```bash
docker run -p 8080:8080 billing-maintenance-service
```

## 📝 Notes

- Service này **không có FK cứng** sang các service khác (RoomBuildingService, ContractStudentService)
- Sử dụng **snapshot data** (StudentName, RoomNumber, BuildingName) để tránh phụ thuộc
- Implement **soft delete** với `IsDeleted` flag
- Tự động track `CreatedAt`, `UpdatedAt`, `DeletedAt`
- CORS enabled cho tất cả origins (chỉ development)
