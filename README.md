# 🏢 Dormitory Management System

Hệ thống quản lý ký túc xá với kiến trúc Microservices + API Gateway + Vue 3 Frontend.

---

## 📋 Mục lục

- [Khởi tạo dự án từ đầu](#-khởi-tạo-dự-án-từ-đầu)
- [Quick Start - Chạy dự án có sẵn](#-quick-start---khởi-động-siêu-nhanh)
- [Cấu trúc Project](#-cấu-trúc-project)
- [Kiến trúc Microservices](#-kiến-trúc-microservices)
- [Tính năng chính](#-tính-năng-chính)
- [Tech Stack](#-tech-stack)

---

## 🎯 Khởi tạo dự án từ đầu

### Yêu cầu hệ thống:
- ✅ **.NET 9.0 SDK** - [Tải tại đây](https://dotnet.microsoft.com/download/dotnet/9.0)
- ✅ **Node.js 18+** & npm - [Tải tại đây](https://nodejs.org/)
- ✅ **SQL Server** hoặc SQL Server Express - [Tải tại đây](https://www.microsoft.com/sql-server/sql-server-downloads)
- ✅ **Visual Studio 2022** hoặc **VS Code** (khuyên dùng)
- ✅ **Git** - [Tải tại đây](https://git-scm.com/)

### Bước 1️⃣: Clone repository

```bash
git clone <repository-url>
cd repo
```

### Bước 2️⃣: Cấu hình Database

#### 2.1. Tạo 3 databases trong SQL Server:
- `BillingMaintenanceDb`
- `ContractStudentDb`
- `RoomBuildingDb`

```sql
-- Chạy trong SQL Server Management Studio (SSMS)
CREATE DATABASE BillingMaintenanceDb;
CREATE DATABASE ContractStudentDb;
CREATE DATABASE RoomBuildingDb;
GO
```

#### 2.2. Cập nhật Connection String trong các file `appsettings.Development.json`:

**BillingMaintenanceService:**
```bash
# Mở file: backend/services/BillingMaintenanceService/BillingMaintenanceService.API/appsettings.Development.json
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BillingMaintenanceDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True"
  }
}
```

**ContractStudentService:**
```bash
# Mở file: backend/services/ContractStudentService/ContractStudentService.API/appsettings.Development.json
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ContractStudentDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True"
  }
}
```

**RoomBuildingService:**
```bash
# Mở file: backend/services/RoomBuildingService/RoomBuildingService.API/appsettings.Development.json
```

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RoomBuildingDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True"
  }
}
```

> 💡 **Lưu ý:** Thay `localhost`, `sa`, và `YourPassword123` bằng thông tin SQL Server của bạn.

### Bước 3️⃣: Restore & Build Backend

```bash
cd backend

# Restore dependencies
dotnet restore

# Build toàn bộ solution
dotnet build
```

### Bước 4️⃣: Chạy Migrations để tạo database schema

#### 4.1. BillingMaintenanceService:
```bash
cd services/BillingMaintenanceService/BillingMaintenanceService.API
dotnet ef database update --project ../BillingMaintenanceService.Infrastructure --startup-project .
```

#### 4.2. ContractStudentService:
```bash
cd ../../ContractStudentService/ContractStudentService.API
dotnet ef database update --project ../ContractStudentService.Infrastructure --startup-project .
```

#### 4.3. RoomBuildingService:
```bash
cd ../../RoomBuildingService/RoomBuildingService.API
dotnet ef database update --project ../RoomBuildingService.Infrastructure --startup-project .
```

> ✅ Sau bước này, các database sẽ có đầy đủ tables và dữ liệu mẫu (seed data).

### Bước 5️⃣: Cài đặt Frontend dependencies

```bash
cd ../../../frontend
npm install
```

### Bước 6️⃣: Chạy ứng dụng

#### Cách 1: Dùng script tự động (Khuyên dùng)
```bash
# Quay về thư mục gốc
cd ..

# Windows Batch
start-dev.bat

# Hoặc PowerShell
.\start-dev.ps1
```

#### Cách 2: Chạy thủ công từng service

**Terminal 1 - BillingMaintenanceService:**
```bash
cd backend/services/BillingMaintenanceService/BillingMaintenanceService.API
dotnet run
```

**Terminal 2 - ContractStudentService:**
```bash
cd backend/services/ContractStudentService/ContractStudentService.API
dotnet run
```

**Terminal 3 - RoomBuildingService:**
```bash
cd backend/services/RoomBuildingService/RoomBuildingService.API
dotnet run
```

**Terminal 4 - API Gateway:**
```bash
cd backend/APIGateway
dotnet run
```

**Terminal 5 - Frontend:**
```bash
cd frontend
npm run dev
```

### Bước 7️⃣: Kiểm tra ứng dụng

Truy cập các URL sau:

| Service | URL | Swagger API Docs |
|---------|-----|------------------|
| **Frontend** | http://localhost:5173 | - |
| **API Gateway** | http://localhost:5000 | http://localhost:5000/swagger |
| BillingMaintenanceService | http://localhost:5002 | http://localhost:5002/swagger |
| ContractStudentService | http://localhost:5001 | http://localhost:5001/swagger |
| RoomBuildingService | http://localhost:5003 | http://localhost:5003/swagger |

### Bước 8️⃣: Đăng nhập với tài khoản mặc định

Truy cập http://localhost:5173 và đăng nhập:

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Admin |
| `staff01` | `Staff@123` | Staff |
| `student01` | `Student@123` | Student |

---

## 🚀 Quick Start - Khởi động siêu nhanh!

> 💡 **Dành cho ai đã clone dự án và đã setup database.**

### ⚡ Cách 1: Script tự động (Khuyên dùng) ⭐

#### Batch Script (Windows)
```bash
# Chạy toàn bộ ứng dụng (Backend + Frontend)
start-dev.bat

# Dừng tất cả
stop-dev.bat
```

#### PowerShell
```powershell
# Chạy toàn bộ ứng dụng (Backend + Frontend)
.\start-dev.ps1

# Dừng tất cả
.\stop-dev.ps1
```

Các script sẽ tự động:
- ✅ Khởi động 3 microservices (BillingMaintenance, ContractStudent, RoomBuilding)
- ✅ Khởi động API Gateway
- ✅ Khởi động Frontend Vue 3 với Vite
- ✅ Mở tất cả trong các terminal riêng biệt

### 🌐 Sau khi chạy, truy cập:

| Service | URL | Mô tả |
|---------|-----|-------|
| **Frontend** | http://localhost:5173 | Giao diện người dùng |
| **API Gateway** | http://localhost:5000 | Cổng API chính |
| BillingMaintenanceService | http://localhost:5002 | Thanh toán & Bảo trì |
| ContractStudentService | http://localhost:5001 | Hợp đồng & Sinh viên |
| RoomBuildingService | http://localhost:5003 | Phòng & Tòa nhà |

### 🔐 Tài khoản đăng nhập mặc định:

| Username | Password | Role | Quyền |
|----------|----------|------|-------|
| `admin` | `Admin@123` | Admin | Full quyền quản trị |
| `staff01` | `Staff@123` | Staff | Nhân viên (limited) |
| `student01` | `Student@123` | Student | Sinh viên |

---

## 🛠️ Chạy thủ công (Development)

### 1️⃣ Chạy Backend

#### Cách A: Script tự động (Nhanh)
```bash
cd backend
start-all.bat          # Windows Batch
# hoặc
.\start-all.ps1        # PowerShell
```

#### Cách B: Chạy từng service
```bash
# Terminal 1 - BillingMaintenanceService
cd backend/services/BillingMaintenanceService/BillingMaintenanceService.API
dotnet run

# Terminal 2 - ContractStudentService  
cd backend/services/ContractStudentService/ContractStudentService.API
dotnet run

# Terminal 3 - RoomBuildingService
cd backend/services/RoomBuildingService/RoomBuildingService.API
dotnet run

# Terminal 4 - API Gateway
cd backend/APIGateway
dotnet run
```

### 2️⃣ Chạy Frontend

```bash
cd frontend
npm install          # Lần đầu tiên
npm run dev         # Chạy dev server
```

Frontend sẽ chạy tại: http://localhost:5173

---

## 📁 Cấu trúc Project

```
repo/
├── 🎨 frontend/              # Vue 3 + Vite + Ant Design Vue
│   ├── src/
│   │   ├── views/           # Trang giao diện (Admin, Staff, Student)
│   │   ├── components/      # Component tái sử dụng
│   │   ├── stores/          # Pinia stores (state management)
│   │   ├── router/          # Vue Router (routing)
│   │   ├── services/        # API services
│   │   ├── utils/           # Utilities (excelExport, dateFormat, etc.)
│   │   └── assets/          # CSS, images, fonts
│   ├── package.json
│   └── vite.config.js
│
├── 🔧 backend/               # .NET 9 Microservices
│   ├── APIGateway/          # Ocelot Gateway (Port 5000)
│   │   ├── ocelot.json      # Routing configuration
│   │   └── Program.cs
│   │
│   ├── services/
│   │   ├── BillingMaintenanceService/    (Port 5002)
│   │   │   ├── BillingMaintenanceService.API/
│   │   │   ├── BillingMaintenanceService.Application/
│   │   │   ├── BillingMaintenanceService.Domain/
│   │   │   └── BillingMaintenanceService.Infrastructure/
│   │   │
│   │   ├── ContractStudentService/       (Port 5001)
│   │   │   ├── ContractStudentService.API/
│   │   │   ├── ContractStudentService.Application/
│   │   │   ├── ContractStudentService.Domain/
│   │   │   └── ContractStudentService.Infrastructure/
│   │   │
│   │   └── RoomBuildingService/          (Port 5003)
│   │       ├── RoomBuildingService.API/
│   │       ├── RoomBuildingService.Application/
│   │       ├── RoomBuildingService.Domain/
│   │       └── RoomBuildingService.Infrastructure/
│   │
│   ├── backend.sln          # Solution file
│   ├── start-all.bat        # Chạy tất cả backend services
│   ├── stop-all.bat         # Dừng tất cả backend services
│   ├── start-all.ps1        # PowerShell version
│   ├── run.ps1              # Script đa năng
│   └── docker-compose.yml   # Docker setup
│
├── start-dev.bat            # 🚀 Chạy toàn bộ app
├── stop-dev.bat             # ⛔ Dừng toàn bộ app
├── start-dev.ps1            # PowerShell version
├── stop-dev.ps1             # PowerShell version
└── README.md                # Tài liệu này
```

---

## 🏗️ Kiến trúc Microservices

```
┌─────────────────────┐
│   Frontend (Vue 3)  │  http://localhost:5173
│  Ant Design Vue UI  │
└──────────┬──────────┘
           │ HTTP Requests
           ▼
┌───────────────────────────────────────────────┐
│         API Gateway (Ocelot)                  │  http://localhost:5000
│         - Authentication (JWT)                │
│         - Request routing                     │
│         - Load balancing                      │
└─────────────────┬─────────────────────────────┘
                  │
       ┌──────────┼──────────┐
       ▼          ▼          ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│          │ │          │ │          │
│  Room &  │ │ Billing  │ │Contract  │
│ Building │ │   &      │ │   &      │
│ Service  │ │Maintainc │ │ Student  │
│          │ │  Service │ │ Service  │
│ :5003    │ │  :5002   │ │  :5001   │
└────┬─────┘ └────┬─────┘ └────┬─────┘
     │            │            │
     ▼            ▼            ▼
┌──────────┐ ┌──────────┐ ┌──────────┐
│RoomBuil  │ │Billing   │ │Contract  │
│dingDb    │ │Maintainc │ │StudentDb │
│          │ │   eDb    │ │          │
└──────────┘ └──────────┘ └──────────┘
```

### Chi tiết từng service:

#### 🏢 **RoomBuildingService** (Port 5003)
**Database:** `RoomBuildingDb`

**Chức năng:**
- Quản lý tòa nhà (Buildings)
- Quản lý tầng (Floors)
- Quản lý loại phòng (RoomTypes)
- Quản lý phòng (Rooms)
- Quản lý tiện nghi (Amenities)
- Upload/Download files (hình ảnh phòng, tài liệu)

**Entities:**
- Building, Floor, RoomType, Room, Amenity

#### 💰 **BillingMaintenanceService** (Port 5002)
**Database:** `BillingMaintenanceDb`

**Chức năng:**
- Quản lý người dùng & Authentication (JWT)
- Quản lý hóa đơn (Invoices)
- Quản lý thanh toán (Payments)
- Quản lý yêu cầu bảo trì (MaintenanceRequests)
- Quản lý thông báo (Notifications)
- Tích hợp thanh toán online (Sepay)

**Entities:**
- User, Invoice, Payment, MaintenanceRequest, Notification

#### 📝 **ContractStudentService** (Port 5001)
**Database:** `ContractStudentDb`

**Chức năng:**
- Quản lý sinh viên (Students)
- Quản lý đơn đăng ký phòng (RoomApplications)
- Quản lý hợp đồng (Contracts)
- Quản lý gia hạn hợp đồng (ContractExtensions)
- Quản lý chuyển phòng (RoomTransfers)
- Quản lý mẫu hợp đồng (ContractTemplates)
- Upload/Download documents (CMND, giấy tờ sinh viên)

**Entities:**
- Student, RoomApplication, Contract, ContractExtension, RoomTransfer, ContractTemplate

---

## 🎯 Tính năng chính

### 👨‍💼 Admin Dashboard
- ✅ Quản lý người dùng (Admin/Staff/Student)
- ✅ Quản lý tòa nhà & phòng
- ✅ Quản lý hợp đồng
- ✅ Quản lý hóa đơn & thanh toán
- ✅ Quản lý yêu cầu bảo trì
- ✅ Thống kê & báo cáo

### 👨‍🎓 Student Portal
- ✅ Đăng ký phòng
- ✅ Xem hợp đồng & hóa đơn
- ✅ Thanh toán online
- ✅ Gửi yêu cầu bảo trì
- ✅ Chuyển phòng
- ✅ Gia hạn hợp đồng

---

## 🐳 Triển khai với Docker

Docker cho phép chạy toàn bộ backend services trong containers, không cần cài .NET SDK.

### Yêu cầu:
- ✅ **Docker Desktop** - [Tải tại đây](https://www.docker.com/products/docker-desktop)
- ✅ **Docker Compose** (đi kèm Docker Desktop)
- ✅ **SQL Server** (chạy riêng hoặc dùng SQL Server Docker container)

### Bước 1️⃣: Cấu hình Database Connection String

Mở file `backend/docker-compose.yml` và thêm environment variables cho connection string:

```yaml
services:
  billing-maintenance-service:
    build:
      context: ./services/BillingMaintenanceService
      dockerfile: Dockerfile
    ports:
      - "5002:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=BillingMaintenanceDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True
    networks:
      - backend-network
```

> 💡 **Lưu ý:** 
> - Dùng `host.docker.internal` thay vì `localhost` để container truy cập SQL Server trên máy host
> - Hoặc chạy SQL Server trong Docker container riêng (xem bên dưới)

### Bước 2️⃣: Build và chạy containers

```bash
cd backend

# Build images và chạy containers
docker-compose up --build -d

# Xem logs
docker-compose logs -f

# Kiểm tra containers đang chạy
docker ps
```

### Bước 3️⃣: Chạy Migrations (lần đầu tiên)

Có 2 cách:

**Cách 1: Chạy migrations từ máy host (cần .NET SDK)**
```bash
# BillingMaintenanceService
cd services/BillingMaintenanceService/BillingMaintenanceService.API
dotnet ef database update --project ../BillingMaintenanceService.Infrastructure

# ContractStudentService
cd ../../ContractStudentService/ContractStudentService.API
dotnet ef database update --project ../ContractStudentService.Infrastructure

# RoomBuildingService
cd ../../RoomBuildingService/RoomBuildingService.API
dotnet ef database update --project ../RoomBuildingService.Infrastructure
```

**Cách 2: Chạy migrations trong container**
```bash
# Vào container
docker exec -it <container_id> bash

# Chạy migrations
dotnet ef database update --project Infrastructure
```

### Bước 4️⃣: Chạy Frontend (riêng)

Frontend vẫn chạy bình thường:
```bash
cd frontend
npm install
npm run dev
```

### Bước 5️⃣: Truy cập ứng dụng

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| API Gateway | http://localhost:5000 |
| BillingMaintenanceService | http://localhost:5002 |
| ContractStudentService | http://localhost:5001 |
| RoomBuildingService | http://localhost:5003 |

### 🛑 Dừng và xóa containers

```bash
# Dừng containers
docker-compose stop

# Dừng và xóa containers
docker-compose down

# Xóa cả volumes (database data)
docker-compose down -v

# Xóa images
docker-compose down --rmi all
```

### 📦 Chạy SQL Server trong Docker (Optional)

Nếu chưa có SQL Server, bạn có thể chạy trong Docker:

```bash
# Chạy SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password123" ^
  -p 1433:1433 --name sqlserver ^
  -d mcr.microsoft.com/mssql/server:2022-latest

# Kiểm tra container
docker ps

# Kết nối: Server=localhost,1433;User Id=sa;Password=YourStrong@Password123
```

Sau đó cập nhật connection string trong `docker-compose.yml`:
```yaml
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=BillingMaintenanceDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True
```

Và thêm SQL Server vào `docker-compose.yml`:
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
    ports:
      - "1433:1433"
    networks:
      - backend-network
    volumes:
      - sqlserver-data:/var/opt/mssql

volumes:
  sqlserver-data:
```

### 🔍 Debug Docker containers

```bash
# Xem logs của service cụ thể
docker-compose logs billing-maintenance-service

# Xem logs realtime
docker-compose logs -f contract-student-service

# Vào bên trong container
docker exec -it <container_name> bash

# Xem resource usage
docker stats

# Kiểm tra network
docker network inspect backend_backend-network
```

---

## 📚 Tech Stack

### Frontend
- **Vue 3** - Progressive framework
- **Vite** - Lightning fast build tool
- **Ant Design Vue** - UI components
- **Pinia** - State management
- **Vue Router** - Routing
- **Axios** - HTTP client
- **Chart.js** - Data visualization

### Backend
- **.NET 9.0** - Framework
- **ASP.NET Core Web API** - REST APIs
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **Ocelot** - API Gateway
- **BCrypt.Net** - Password hashing
- **Swagger/OpenAPI** - API documentation

### DevOps
- **Docker & Docker Compose** - Containerization
- **PowerShell & Batch** - Automation scripts

---

## 🔧 Cấu hình Database

Sửa `appsettings.Development.json` trong mỗi service:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DbName;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Chạy Migrations:
```bash
# Trong mỗi service
dotnet ef database update --project Infrastructure --startup-project API
```

---

## 🐛 Troubleshooting

### Backend không chạy được?
```bash
# Kiểm tra .NET SDK
dotnet --version

# Build lại
cd backend
dotnet build

# Xóa cache
dotnet clean
```

### Frontend không chạy được?
```bash
cd frontend

# Xóa node_modules và cài lại
rm -rf node_modules
npm install

# Clear cache
npm cache clean --force
```

### Port bị chiếm?
Sửa port trong:
- Backend: `launchSettings.json` của mỗi service
- Frontend: `vite.config.js`

### Không thể dừng services?
```bash
# Windows
taskkill /F /IM dotnet.exe
taskkill /F /IM node.exe

# PowerShell
Get-Process dotnet | Stop-Process -Force
Get-Process node | Stop-Process -Force
```

---

## 📖 Documentation

- [Backend README](./backend/README.md) - Chi tiết về microservices
- [Frontend README](./frontend/README.md) - Chi tiết về Vue app
- API Docs: http://localhost:5000/swagger (sau khi chạy Gateway)

---

## 🤝 Contributing

1. Fork project
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 📝 License

MIT License - xem file [LICENSE](LICENSE)

---

## 👥 Team

Developed by DNU Students

---

## 🎉 Happy Coding!

Nếu có vấn đề gì, tạo issue hoặc liên hệ team! 🚀
