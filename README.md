# 🏢 Hệ thống Quản lý Ký túc xá

<div align="center">

![Vue.js](https://img.shields.io/badge/Vue.js-3.x-4FC08D?style=for-the-badge&logo=vue.js&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)

Hệ thống quản lý ký túc xá toàn diện với kiến trúc Microservices

[Tài liệu](#-tài-liệu) •
[Cài đặt](#-cài-đặt-nhanh) •
[Demo](#-demo) •
[API Docs](#-api-documentation)

</div>

---

## ✨ Tính năng chính

### 🎯 Quản trị viên (Admin)
- 📊 **Dashboard** - Tổng quan thống kê realtime
- 🏢 **Quản lý Tòa nhà** - CRUD tòa nhà, tầng, sơ đồ mặt bằng
- 🚪 **Quản lý Phòng** - Loại phòng, phòng trống, đang ở, bảo trì
- 🛋️ **Quản lý Tiện nghi** - Điều hòa, tủ lạnh, giường, bàn...
- 📋 **Biên bản kiểm tra** - Check-in/out, định kỳ, sự cố
- 📢 **Thông báo** - Thông báo tòa nhà hoặc toàn KTX
- 🎓 **Quản lý Sinh viên** - Hồ sơ, phòng ở, lịch sử
- 📝 **Quản lý Hợp đồng** - Tạo, gia hạn, chấm dứt
- 💰 **Hóa đơn & Thanh toán** - Tự động tính phí, tracking
- 🔧 **Bảo trì** - Tiếp nhận, xử lý yêu cầu sửa chữa

### 👨‍🎓 Sinh viên (Student Portal)
- 🏠 **Phòng của tôi** - Thông tin phòng, bạn cùng phòng
- 📋 **Hợp đồng** - Xem chi tiết hợp đồng đang ở
- 💳 **Thanh toán** - Xem hóa đơn, thanh toán online
- 🔧 **Yêu cầu sửa chữa** - Báo hỏng hóc ngay trên app
- 📢 **Thông báo** - Nhận thông báo từ BQL
- 👤 **Hồ sơ** - Cập nhật thông tin cá nhân

### 🌐 Trang công khai
- 🏡 **Trang chủ** - Giới thiệu KTX, tiện ích
- 📋 **Đăng ký phòng** - Form đăng ký online
- 📰 **Tin tức** - Thông báo, sự kiện
- 📖 **Nội quy** - Quy định KTX
- 📞 **Liên hệ** - Thông tin liên hệ BQL

---

## 🏗️ Kiến trúc hệ thống

```
┌─────────────────────────────────────────────────────────────┐
│                        Frontend (Vue 3)                      │
│                     Port: 5173 (Vite Dev)                    │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    API Gateway (Ocelot)                      │
│                         Port: 5000                           │
└──────────────┬─────────────┬────────────────┬───────────────┘
               │             │                │
       ┌───────▼───────┐ ┌───▼───────┐  ┌────▼──────────┐
       │ RoomBuilding  │ │  Billing  │  │   Contract    │
       │   Service     │ │   Service │  │   Service     │
       │  Port: 5001   │ │Port: 5002 │  │  Port: 5059   │
       └───────┬───────┘ └─────┬─────┘  └────┬──────────┘
               │               │              │
       ┌───────▼───────────────▼──────────────▼───────────┐
       │            SQL Server (3 Databases)              │
       │    - RoomBuildingDB                              │
       │    - BillingMaintenanceDB                        │
       │    - ContractStudentDB                           │
       └──────────────────────────────────────────────────┘
```

---

## 🚀 Cài đặt nhanh

### Yêu cầu hệ thống
- .NET 9 SDK
- Node.js 18+
- SQL Server 2022
- Docker Desktop (khuyến nghị)

### Chạy với Docker Compose

```bash
# Clone repository
git clone <repo-url>
cd repo

# Chạy toàn bộ hệ thống
docker-compose up -d

# Xem logs
docker-compose logs -f

# Truy cập
# - Frontend: http://localhost:5173
# - API Gateway: http://localhost:5000
# - Swagger RoomBuilding: http://localhost:5001/swagger
```

### Chạy Manual

```bash
# Backend
cd backend/services/RoomBuildingService/RoomBuildingService.API
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

📖 **Xem hướng dẫn chi tiết**: [SETUP_GUIDE.md](SETUP_GUIDE.md)

---

## 📂 Cấu trúc thư mục

```
repo/
├── backend/
│   ├── APIGateway/                    # Ocelot API Gateway
│   └── services/
│       ├── RoomBuildingService/       # Service quản lý tòa nhà, phòng
│       │   ├── RoomBuildingService.API
│       │   ├── RoomBuildingService.Application
│       │   ├── RoomBuildingService.Domain
│       │   └── RoomBuildingService.Infrastructure
│       ├── BillingMaintenanceService/ # Service hóa đơn, bảo trì
│       └── ContractStudentService/    # Service sinh viên, hợp đồng
├── frontend/                          # Vue 3 + Ant Design Vue
│   ├── src/
│   │   ├── components/               # Reusable components
│   │   ├── views/                    # Page views
│   │   ├── layouts/                  # Layout wrappers
│   │   ├── router/                   # Vue Router config
│   │   ├── services/                 # API services
│   │   └── stores/                   # Pinia stores
│   └── package.json
├── docker-compose.yml                 # Docker Compose config
├── README.md                          # File này
└── SETUP_GUIDE.md                     # Hướng dẫn chi tiết
```

---

## 🎨 Screenshots

### Dashboard Admin
Dashboard tổng quan với biểu đồ thống kê realtime

![Dashboard](docs/screenshots/dashboard.png)

### Quản lý Tòa nhà
Danh sách tòa nhà với filter và search

![Buildings](docs/screenshots/buildings.png)

### Quản lý Phòng
Xem và quản lý phòng theo tòa nhà, tầng

![Rooms](docs/screenshots/rooms.png)

### Student Portal
Giao diện sinh viên xem phòng và thanh toán

![Student](docs/screenshots/student.png)

---

## 🔌 API Endpoints

### RoomBuildingService (Port 5001)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/buildings` | Lấy danh sách tòa nhà |
| POST | `/api/buildings` | Tạo tòa nhà mới |
| GET | `/api/roomtypes` | Lấy danh sách loại phòng |
| GET | `/api/rooms` | Lấy danh sách phòng |
| GET | `/api/amenities` | Lấy danh sách tiện nghi |
| GET | `/api/roominspections` | Lấy biên bản kiểm tra |
| GET | `/api/buildingannouncemets` | Lấy thông báo |

### BillingMaintenanceService (Port 5002)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/bills` | Lấy danh sách hóa đơn |
| POST | `/api/payments` | Tạo thanh toán mới |
| GET | `/api/maintenancerequests` | Lấy yêu cầu bảo trì |

### ContractStudentService (Port 5059)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/students` | Lấy danh sách sinh viên |
| GET | `/api/contracts` | Lấy danh sách hợp đồng |
| POST | `/api/contracts` | Tạo hợp đồng mới |

📝 **Xem API docs đầy đủ**: Swagger UI tại `/swagger` của từng service

---

## 🛠️ Tech Stack

### Backend
- **Framework**: .NET 9
- **Database**: SQL Server 2022
- **ORM**: Entity Framework Core 9
- **API Gateway**: Ocelot
- **Architecture**: Clean Architecture + Microservices

### Frontend
- **Framework**: Vue 3 (Composition API)
- **UI Library**: Ant Design Vue 4
- **State Management**: Pinia
- **Routing**: Vue Router 4
- **HTTP Client**: Axios
- **Build Tool**: Vite
- **Charts**: Chart.js + vue-chartjs

### DevOps
- **Container**: Docker + Docker Compose
- **CI/CD**: GitHub Actions (coming soon)

---

## 📊 Database Schema

### RoomBuildingDB
- Building (Tòa nhà)
- Floor (Tầng)
- RoomType (Loại phòng)
- Room (Phòng)
- Amenity (Tiện nghi)
- RoomTypeAmenity (Liên kết)
- BuildingAnnouncement (Thông báo)
- RoomInspection (Biên bản kiểm tra)
- RoomReservation (Đặt chỗ tạm)
- RoomStatusLog (Log trạng thái)
- RoomImage (Ảnh phòng)

### BillingMaintenanceDB
- Bill (Hóa đơn)
- Payment (Thanh toán)
- MaintenanceRequest (Yêu cầu bảo trì)

### ContractStudentDB
- Student (Sinh viên)
- RoomApplication (Đơn đăng ký)
- RoomContract (Hợp đồng)

---

## 🧪 Testing

```bash
# Run backend tests
cd backend/services/RoomBuildingService
dotnet test

# Run frontend tests
cd frontend
npm run test
```

---

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guide](CONTRIBUTING.md) first.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 Changelog

### Version 1.0.0 (2026-06-03)
- ✨ Initial release
- 🏢 Building, Floor, Room management
- 🚪 RoomType and Amenity management
- 📋 Room inspection workflow
- 📢 Building announcements
- 🎓 Student and contract management
- 💰 Billing and payment tracking
- 🔧 Maintenance request system
- 📱 Responsive frontend with Ant Design Vue

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

- **Backend Team** - Clean Architecture + Microservices
- **Frontend Team** - Vue 3 + Ant Design Vue
- **DevOps Team** - Docker + CI/CD

---

## 📞 Support

Nếu bạn gặp vấn đề hoặc có câu hỏi:

- 📧 Email: support@dormitory.edu.vn
- 💬 GitHub Issues: [Create an issue](https://github.com/your-repo/issues)
- 📖 Documentation: [Wiki](https://github.com/your-repo/wiki)

---

<div align="center">

Made with ❤️ by Dormitory Management Team

⭐ Star us on GitHub if you find this project helpful!

</div>
