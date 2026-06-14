# Check-in/Check-out Implementation Summary

## ✅ Hoàn thành

### Backend

1. **Entities Created** (`ContractStudentService.Domain/Entities/CheckInCheckOut.cs`)
   - `CheckIn`: Entity lưu thông tin nhận phòng
   - `CheckOut`: Entity lưu thông tin trả phòng
   - Bao gồm: thông tin sinh viên, phòng, hình ảnh, tiền cọc, tình trạng phòng, v.v.

2. **DTOs Created** (`ContractStudentService.Application/DTOs/DTOs.cs`)
   - `CheckInDto`, `CreateCheckInDto`
   - `CheckOutDto`, `CreateCheckOutDto`
   - `PendingCheckInDto`, `PendingCheckOutDto`

3. **Repositories** (`ContractStudentService.Infrastructure/Repositories/Repositories.cs`)
   - `ICheckInRepository` và implementation
   - `ICheckOutRepository` và implementation
   - Đã đăng ký trong `Program.cs`

4. **Controller** (`ContractStudentService.API/Controllers/CheckInCheckOutController.cs`)
   - `GET /api/CheckInCheckOut/pending-checkins` - Lấy danh sách hợp đồng cần check-in
   - `GET /api/CheckInCheckOut/pending-checkouts` - Lấy danh sách hợp đồng cần check-out
   - `GET /api/CheckInCheckOut/checkins` - Lấy tất cả check-in đã thực hiện
   - `GET /api/CheckInCheckOut/checkouts` - Lấy tất cả check-out đã thực hiện
   - `GET /api/CheckInCheckOut/checkin/contract/{contractId}` - Lấy check-in theo contract
   - `GET /api/CheckInCheckOut/checkout/contract/{contractId}` - Lấy check-out theo contract
   - `POST /api/CheckInCheckOut/checkin` - Tạo check-in mới
   - `POST /api/CheckInCheckOut/checkout` - Tạo check-out mới

5. **Database Configuration**
   - Entity Framework mapping đã được cấu hình trong `AppDbContext.cs`
   - DbSet cho CheckIns và CheckOuts
   - Migration đã được tạo: `AddCheckInCheckOutTables`

6. **API Gateway Routing** (`APIGateway/ocelot.json`)
   - Route: `/api/checkin-checkout/{everything}` → ContractStudentService (port 5001)

### Frontend

1. **Service Layer** (`frontend/src/services/checkInCheckOutService.js`)
   - `getPendingCheckIns()` - Lấy danh sách cần check-in
   - `getPendingCheckOuts()` - Lấy danh sách cần check-out
   - `getAllCheckIns()` - Lấy tất cả check-in
   - `getAllCheckOuts()` - Lấy tất cả check-out
   - `getCheckInByContractId(contractId)`
   - `getCheckOutByContractId(contractId)`
   - `createCheckIn(data)`
   - `createCheckOut(data)`

2. **View Component** (`frontend/src/views/staff/StaffCheckinCheckoutView.vue`)
   - UI đã được thiết kế với 3-step wizard cho check-in và check-out
   - Tích hợp với API thật
   - Upload hình ảnh qua fileService
   - Validation và error handling

3. **Other Updates**
   - `LoginView.vue`: Đã thay thế `alert()` bằng Ant Design `notification`

## 🔧 Cách chạy

### 1. Apply Migration

```powershell
cd backend
dotnet ef database update -p services\ContractStudentService\ContractStudentService.Infrastructure -s services\ContractStudentService\ContractStudentService.API
```

### 2. Start Services

```powershell
cd backend
.\run.ps1
```

Hoặc thủ công:
```powershell
# Terminal 1 - API Gateway
cd backend\APIGateway
dotnet run

# Terminal 2 - Contract Student Service
cd backend\services\ContractStudentService\ContractStudentService.API
dotnet run

# Terminal 3 - Room Building Service
cd backend\services\RoomBuildingService\RoomBuildingService.API
dotnet run

# Terminal 4 - Billing Maintenance Service
cd backend\services\BillingMaintenanceService\BillingMaintenanceService.API
dotnet run

# Terminal 5 - Frontend
cd frontend
npm run dev
```

### 3. Access

- Frontend: http://localhost:5173
- API Gateway: http://localhost:5000
- Check-in/Check-out: `/staff/checkin-checkout`

## 📋 API Endpoints

### Check-in Flow

1. **Lấy danh sách cần check-in**
   ```
   GET /api/checkin-checkout/pending-checkins
   ```
   Response: Danh sách các hợp đồng Active chưa check-in (trong vòng 30 ngày tới)

2. **Thực hiện check-in**
   ```
   POST /api/checkin-checkout/checkin
   Content-Type: application/json
   
   {
     "contractId": 1,
     "studentId": 1,
     "roomId": 1,
     "roomNumber": "101",
     "buildingName": "Tòa A",
     "checkInDate": "2026-06-14T10:00:00Z",
     "checkedInByUserId": 1,
     "checkedInByName": "Admin",
     "idCardImageUrls": "[\"url1\", \"url2\"]",
     "isDepositPaid": true,
     "depositAmount": 1000000,
     "depositPaidAt": "2026-06-14T10:00:00Z",
     "roomConditionChecklist": "[\"walls\", \"doors\", \"electric\"]",
     "roomImageUrls": "[\"url1\", \"url2\"]",
     "roomCondition": "Good",
     "notes": "Phòng sạch sẽ",
     "keysProvided": "Key + Access Card",
     "keyCount": 2
   }
   ```

### Check-out Flow

1. **Lấy danh sách cần check-out**
   ```
   GET /api/checkin-checkout/pending-checkouts
   ```
   Response: Danh sách các hợp đồng Active sắp hết hạn (trong vòng 60 ngày)

2. **Thực hiện check-out**
   ```
   POST /api/checkin-checkout/checkout
   Content-Type: application/json
   
   {
     "contractId": 1,
     "studentId": 1,
     "roomId": 1,
     "roomNumber": "101",
     "buildingName": "Tòa A",
     "checkOutDate": "2026-06-14T10:00:00Z",
     "checkedOutByUserId": 1,
     "checkedOutByName": "Admin",
     "currentRoomImageUrls": "[\"url1\", \"url2\"]",
     "roomCondition": "Good",
     "damageDescription": null,
     "depositAmount": 1000000,
     "compensationCost": 0,
     "refundAmount": 1000000,
     "compensationDetails": null,
     "isKeyReturned": true,
     "isDepositRefunded": true,
     "depositRefundedAt": "2026-06-14T10:00:00Z",
     "refundMethod": "Cash",
     "notes": null
   }
   ```

## 🎯 Features

### Check-in Features
- ✅ Danh sách sinh viên cần nhận phòng
- ✅ 3-step wizard: Xác minh → Kiểm tra phòng → Hoàn tất
- ✅ Upload ảnh CMND/CCCD
- ✅ Upload ảnh phòng ban đầu (4-6 ảnh)
- ✅ Checklist kiểm tra phòng (tường, cửa, điện, nước, nội thất, vệ sinh)
- ✅ Xác nhận thu tiền cọc
- ✅ Ghi chú tình trạng phòng
- ✅ Tự động cập nhật trạng thái hợp đồng

### Check-out Features
- ✅ Danh sách sinh viên cần trả phòng
- ✅ 3-step wizard: Kiểm tra → Tính toán → Hoàn tất
- ✅ Đánh giá tình trạng phòng (Tốt / Hư hỏng nhẹ / Hư hỏng nặng)
- ✅ Upload ảnh phòng hiện tại
- ✅ Mô tả hư hỏng (nếu có)
- ✅ Tính toán chi phí bồi thường tự động
- ✅ Tính toán tiền hoàn trả = Tiền cọc - Chi phí bồi thường
- ✅ Xác nhận thu hồi chìa khóa/thẻ từ
- ✅ Xác nhận hoàn tiền cọc
- ✅ Tự động cập nhật trạng thái hợp đồng thành "Terminated"
- ✅ Lưu thông tin hoàn cọc vào Contract

## 🗄️ Database Schema

### CheckIns Table
- Id, ContractId, StudentId, RoomId
- RoomNumber, BuildingName (snapshot)
- CheckInDate, CheckedInByUserId, CheckedInByName
- IdCardImageUrls (JSON array)
- IsDepositPaid, DepositAmount, DepositPaidAt
- RoomConditionChecklist (JSON array)
- RoomImageUrls (JSON array)
- RoomCondition, Notes
- InitialElectricityReading, InitialWaterReading
- KeysProvided, KeyCount
- Status, CreatedAt

### CheckOuts Table
- Id, ContractId, StudentId, RoomId
- RoomNumber, BuildingName (snapshot)
- CheckOutDate, CheckedOutByUserId, CheckedOutByName
- CurrentRoomImageUrls (JSON array)
- RoomCondition, DamageDescription
- DepositAmount, CompensationCost, RefundAmount, CompensationDetails
- FinalElectricityReading, FinalWaterReading
- IsKeyReturned, IsDepositRefunded, DepositRefundedAt
- RefundMethod, RefundReference, Notes
- Status, CreatedAt

## 📝 Business Logic

### Check-in Logic
1. Lấy danh sách các Active contracts chưa có CheckIn record
2. Chỉ hiển thị contracts bắt đầu trong vòng 30 ngày hoặc đã bắt đầu
3. Khi check-in, cập nhật Contract.IsDepositPaid nếu đã thu tiền
4. Lưu ảnh và thông tin kiểm tra phòng ban đầu
5. Ghi lại chỉ số điện nước ban đầu (nếu có)

### Check-out Logic
1. Lấy danh sách các Active contracts chưa có CheckOut record
2. Chỉ hiển thị contracts kết thúc trong vòng 60 ngày hoặc đã hết hạn
3. So sánh tình trạng phòng hiện tại với lúc check-in
4. Tính toán chi phí bồi thường dựa trên mức độ hư hỏng
5. Tính toán tiền hoàn trả = Tiền cọc - Chi phí bồi thường
6. Cập nhật Contract status thành "Terminated"
7. Lưu thông tin hoàn cọc vào Contract (DepositReturnedAmount, DepositReturnedAt, DepositDeductionReason)
8. Ghi lại chỉ số điện nước cuối (nếu có)

## 🐛 Known Issues / TODO

- [ ] Validation cho upload ảnh (size, format)
- [ ] Tích hợp với room occupancy update
- [ ] Export báo cáo check-in/check-out
- [ ] Email notification cho sinh viên
- [ ] Print biên bản bàn giao
- [ ] History log các lần check-in/check-out

## 📚 References

- Entity definitions: `ContractStudentService.Domain/Entities/CheckInCheckOut.cs`
- Controller: `ContractStudentService.API/Controllers/CheckInCheckOutController.cs`
- Frontend view: `frontend/src/views/staff/StaffCheckinCheckoutView.vue`
- Service: `frontend/src/services/checkInCheckOutService.js`
