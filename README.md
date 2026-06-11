# 🏢 Dormitory Management System

Hệ thống quản lý ký túc xá với kiến trúc Microservices + API Gateway + Vue 3 Frontend.

## 🚀 Quick Start - Khởi động siêu nhanh!

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

## 📁 Cấu trúc Project

```
repo/
├── 🎨 frontend/              # Vue 3 + Vite + Ant Design Vue
│   ├── src/
│   │   ├── views/           # Trang giao diện
│   │   ├── stores/          # Pinia stores
│   │   ├── router/          # Vue Router
│   │   └── assets/          # CSS, images
│   └── package.json
│
├── 🔧 backend/               # .NET 9 Microservices
│   ├── APIGateway/          # Ocelot Gateway (Port 5000)
│   ├── services/
│   │   ├── BillingMaintenanceService/    (Port 5002)
│   │   ├── ContractStudentService/       (Port 5001)
│   │   └── RoomBuildingService/          (Port 5003)
│   │
│   ├── start-all.bat        # Chạy tất cả backend services
│   ├── stop-all.bat         # Dừng tất cả backend services
│   ├── start-all.ps1        # PowerShell version
│   ├── run.ps1              # Script đa năng
│   └── docker-compose.yml   # Docker setup
│
├── start-dev.bat            # 🚀 Chạy toàn bộ app
├── stop-dev.bat             # ⛔ Dừng toàn bộ app
├── start-dev.ps1            # PowerShell version
└── stop-dev.ps1             # PowerShell version
```

---

## 🛠️ Chạy thủ công (Development)

### Yêu cầu:
- ✅ .NET 9.0 SDK
- ✅ Node.js 18+ & npm
- ✅ SQL Server (hoặc SQL Server Express)

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

## 🏗️ Kiến trúc Microservices

```
┌─────────────┐
│  Frontend   │
│  (Vue 3)    │
└──────┬──────┘
       │
       ▼
┌──────────────────┐
│   API Gateway    │ Port 5000
│    (Ocelot)      │
└────────┬─────────┘
         │
         ├──> 🏢 RoomBuildingService (Port 5003)
         │    └─> RoomBuildingDb
         │         • Buildings, Rooms
         │         • File uploads
         │
         ├──> 💰 BillingMaintenanceService (Port 5002)
         │    └─> BillingMaintenanceDb
         │         • Users, Auth (JWT)
         │         • Invoices, Payments
         │         • Maintenance Requests
         │         • Notifications
         │
         └──> 📝 ContractStudentService (Port 5001)
              └─> ContractStudentDb
                   • Students, Documents
                   • Room Applications
                   • Contracts
                   • Contract Extensions
                   • Room Transfers
```

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

## 🐳 Docker (Production)

```bash
# Backend services
cd backend
docker-compose up --build -d

# Kiểm tra logs
docker-compose logs -f

# Dừng tất cả
docker-compose down
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
