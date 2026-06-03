# Backend - Dormitory Management System

Hệ thống quản lý ký túc xá với kiến trúc microservices.

## 🏗️ Kiến trúc

```
┌─────────────────┐
│   API Gateway   │ ──> Port 5000
│    (Ocelot)     │
└────────┬────────┘
         │
         ├──> RoomBuildingService (Port 5001)
         │    └─> RoomBuildingDb
         │
         ├──> BillingMaintenanceService (Port 5002)
         │    └─> BillingMaintenanceDb
         │
         └──> ContractStudentService (Port 5003)
              └─> ContractStudentDb
```

## 📦 Microservices

### 1. **RoomBuildingService** - Port 5001
Quản lý phòng, tòa nhà và file uploads
- **Controllers**: Buildings, Rooms, Files
- **Database**: RoomBuildingDb
- **Features**: 
  - CRUD Buildings (tòa nhà)
  - CRUD Rooms (phòng) với trạng thái
  - Upload/download files

### 2. **BillingMaintenanceService** - Port 5002
Quản lý thanh toán và bảo trì
- **Controllers**: Users, Invoices, Payments, MaintenanceRequests
- **Database**: BillingMaintenanceDb
- **Features**:
  - User management với BCrypt authentication
  - Invoice & Payment tracking
  - Maintenance request workflow
  - Notification system

### 3. **ContractStudentService** - Port 5003
Quản lý sinh viên và hợp đồng
- **Controllers**: Students, Contracts, RoomApplications, ContractExtensions, RoomTransfers, StudentDocuments
- **Database**: ContractStudentDb
- **Features**:
  - Student profiles với documents
  - Room applications & approval workflow
  - Contract management
  - Contract extensions
  - Room transfer requests

### 4. **API Gateway** - Port 5000
Ocelot API Gateway - Single entry point
- Routes requests to appropriate microservices
- Swagger aggregation

## 🚀 Quick Start

### Cách 1: Docker Compose (Recommended)

```bash
# Build và chạy tất cả services
docker-compose up --build

# Chạy ở background
docker-compose up -d

# Xem logs
docker-compose logs -f

# Stop tất cả
docker-compose down
```

Services sẽ chạy tại:
- API Gateway: http://localhost:5000
- RoomBuildingService: http://localhost:5001
- BillingMaintenanceService: http://localhost:5002
- ContractStudentService: http://localhost:5003
- SQL Server: localhost:1433

### Cách 2: Chạy từng service local

#### Yêu cầu:
- .NET 9.0 SDK
- SQL Server (hoặc SQL Server Express)

#### Bước 1: Cập nhật connection strings

Sửa `appsettings.Development.json` trong mỗi service:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DbName;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

#### Bước 2: Chạy migrations

```bash
# RoomBuildingService
cd services/RoomBuildingService
dotnet ef database update --project RoomBuildingService.Infrastructure --startup-project RoomBuildingService.API

# ContractStudentService
cd ../ContractStudentService
dotnet ef database update --project ContractStudentService.Infrastructure --startup-project ContractStudentService.API

# BillingMaintenanceService
cd ../BillingMaintenanceService
dotnet ef database update --project BillingMaintenanceService.Infrastructure --startup-project BillingMaintenanceService.API
```

#### Bước 3: Chạy services

```bash
# Terminal 1 - RoomBuildingService
cd services/RoomBuildingService
dotnet run --project RoomBuildingService.API

# Terminal 2 - ContractStudentService
cd services/ContractStudentService
dotnet run --project ContractStudentService.API

# Terminal 3 - BillingMaintenanceService
cd services/BillingMaintenanceService
dotnet run --project BillingMaintenanceService.API

# Terminal 4 - API Gateway
cd APIGateway
dotnet run
```

## 📊 Database Schema

### RoomBuildingDb
- Buildings: Tòa nhà
- Rooms: Phòng ở

### ContractStudentDb
- Students: Sinh viên
- StudentDocuments: Giấy tờ sinh viên
- RoomApplications: Đơn đăng ký phòng
- Contracts: Hợp đồng thuê phòng
- ContractExtensions: Gia hạn hợp đồng
- RoomTransfers: Chuyển phòng

### BillingMaintenanceDb
- Users: Tài khoản hệ thống
- Invoices & InvoiceItems: Phiếu thu tiền
- Payments: Thanh toán
- MaintenanceRequests: Yêu cầu sửa chữa
- Notifications: Thông báo
- SystemSettings: Cài đặt hệ thống
- AuditLogs: Nhật ký hoạt động

## 🔌 API Endpoints (via Gateway)

Tất cả requests đi qua Gateway tại `http://localhost:5000`

### RoomBuildingService
- `GET /api/buildings` - Danh sách tòa nhà
- `GET /api/rooms` - Danh sách phòng
- `POST /api/files` - Upload file

### ContractStudentService
- `GET /api/students` - Danh sách sinh viên
- `GET /api/contracts` - Danh sách hợp đồng
- `GET /api/roomapplications` - Đơn đăng ký phòng
- `POST /api/contractextensions` - Gia hạn hợp đồng
- `POST /api/roomtransfers` - Chuyển phòng

### BillingMaintenanceService
- `GET /api/users` - Danh sách users
- `GET /api/invoices` - Danh sách phiếu thu
- `POST /api/payments` - Thanh toán
- `GET /api/maintenancerequests` - Yêu cầu sửa chữa

### Swagger UI
- Gateway: http://localhost:5000/swagger
- RoomBuildingService: http://localhost:5001/swagger
- ContractStudentService: http://localhost:5003/swagger
- BillingMaintenanceService: http://localhost:5002/swagger

## 🧪 Testing

Mỗi service có file `.http` để test APIs:
- `RoomBuildingService.API/RoomBuildingService.http`
- `ContractStudentService.API/ContractStudentService.http`
- `BillingMaintenanceService.API/BillingMaintenanceService.http`

## 📝 Technology Stack

- **.NET 9.0** - Framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **Ocelot** - API Gateway
- **Swagger** - API Documentation
- **BCrypt.Net** - Password hashing
- **Docker** - Containerization

## 🔒 Security Notes

- Passwords được hash với BCrypt
- CORS enabled cho development (cần restrict cho production)
- JWT tokens cho authentication (ready to implement)
- Soft delete cho tất cả entities

## 📂 Project Structure

```
backend/
├── APIGateway/              # Ocelot Gateway
│   ├── ocelot.json
│   └── Program.cs
│
├── services/
│   ├── RoomBuildingService/
│   │   ├── RoomBuildingService.API/
│   │   ├── RoomBuildingService.Application/
│   │   ├── RoomBuildingService.Domain/
│   │   └── RoomBuildingService.Infrastructure/
│   │
│   ├── ContractStudentService/
│   │   ├── ContractStudentService.API/
│   │   ├── ContractStudentService.Application/
│   │   ├── ContractStudentService.Domain/
│   │   └── ContractStudentService.Infrastructure/
│   │
│   └── BillingMaintenanceService/
│       ├── BillingMaintenanceService.API/
│       ├── BillingMaintenanceService.Application/
│       ├── BillingMaintenanceService.Domain/
│       └── BillingMaintenanceService.Infrastructure/
│
├── backend.sln
└── docker-compose.yml
```

## 🐛 Troubleshooting

### Database connection issues
```bash
# Kiểm tra SQL Server đang chạy
docker ps | grep sqlserver

# Xem logs của service
docker logs billing-maintenance-service
```

### Migration issues
```bash
# Remove migration
dotnet ef migrations remove --project Infrastructure --startup-project API

# Tạo lại migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
```

### Port conflicts
Nếu port đã bị sử dụng, sửa trong `docker-compose.yml` hoặc `launchSettings.json`

## 📚 Further Development

- [ ] Implement JWT authentication middleware
- [ ] Add Redis for caching
- [ ] Add RabbitMQ for inter-service communication
- [ ] Add health checks endpoint
- [ ] Add API rate limiting
- [ ] Add logging với Serilog
- [ ] Add unit tests
- [ ] Add integration tests
