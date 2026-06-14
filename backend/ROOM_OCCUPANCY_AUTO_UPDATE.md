# Tự động cập nhật số người và trạng thái phòng

## Vấn đề cũ
- Trạng thái phòng (Available/Occupied/Full) bị **hardcode**
- Khi sinh viên đăng ký phòng, số người không được cập nhật
- Admin phải **thủ công** sửa CurrentOccupants và Status

## Giải pháp

### 1. **API mới: Update Room Occupancy** (RoomBuildingService)

**Endpoint**: `PUT /api/rooms/{id}/occupancy`

**Request Body**:
```json
{
  "increment": 1  // +1: thêm người, -1: bớt người
}
```

**Logic tự động**:
- Cập nhật `CurrentOccupants`
- Tự động set `Status`:
  - `CurrentOccupants = 0` → Status = "Available"
  - `CurrentOccupants < MaxOccupants` → Status = "Occupied"
  - `CurrentOccupants >= MaxOccupants` → Status = "Full"

**Response**:
```json
{
  "message": "Cập nhật thành công",
  "currentOccupants": 3,
  "maxOccupants": 4,
  "status": "Occupied"
}
```

### 2. **RoomServiceClient** (ContractStudentService)

Service helper để gọi RoomBuildingService từ ContractStudentService:

```csharp
await _roomServiceClient.UpdateRoomOccupancyAsync(roomId, increment: 1);
```

### 3. **Tích hợp với workflow**

#### **Khi DUYỆT đơn đăng ký** (RoomApplicationsController.Approve):
```csharp
// Duyệt đơn → Tạo hợp đồng
await _contractRepository.AddAsync(contract);

// ✅ Tự động +1 người vào phòng
await _roomServiceClient.UpdateRoomOccupancyAsync(
    application.AssignedRoomId.Value, 
    increment: 1
);
```

#### **Khi CHẤM DỨT hợp đồng** (ContractsController.Terminate):
```csharp
// Chấm dứt hợp đồng
contract.Status = "Terminated";
await _contractRepository.UpdateAsync(contract);

// ✅ Tự động -1 người khỏi phòng
await _roomServiceClient.UpdateRoomOccupancyAsync(
    contract.RoomId, 
    increment: -1
);
```

## Kết quả

### **Trước**:
- Admin duyệt đơn → Phòng vẫn "Available" ❌
- Phải vào "Quản lý phòng" → Sửa thủ công

### **Sau**:
| Hành động | CurrentOccupants | Status |
|-----------|------------------|---------|
| Phòng trống | 0 | Available |
| Duyệt đơn SV1 | 0 → 1 | Available → Occupied ✅ |
| Duyệt đơn SV2 | 1 → 2 | Occupied |
| Duyệt đơn SV3 | 2 → 3 | Occupied |
| Duyệt đơn SV4 | 3 → 4 | Occupied → Full ✅ |
| Chấm dứt HĐ SV1 | 4 → 3 | Full → Occupied ✅ |

## Tính năng bổ sung

### **Validation**:
- Không cho `CurrentOccupants < 0`
- Không cho `CurrentOccupants > MaxOccupants`
- Trả lỗi 400 nếu vi phạm

### **Fault tolerance**:
- Nếu RoomServiceClient fail → Log warning nhưng không làm fail transaction chính
- Admin vẫn có thể sửa thủ công nếu cần

## Kiểm tra

### Test 1: Duyệt đơn
1. Tạo đơn đăng ký cho phòng 101 (capacity: 4, current: 0)
2. Admin duyệt đơn
3. ✅ Kiểm tra: CurrentOccupants = 1, Status = "Occupied"

### Test 2: Phòng đầy
1. Phòng 102: capacity = 4, current = 3
2. Admin duyệt đơn thứ 4
3. ✅ Kiểm tra: CurrentOccupants = 4, Status = "Full"
4. ⛔ Duyệt đơn thứ 5 → Lỗi "Phòng đã đầy"

### Test 3: Chấm dứt hợp đồng
1. Phòng 103: capacity = 4, current = 4, status = "Full"
2. Admin chấm dứt 1 hợp đồng
3. ✅ Kiểm tra: CurrentOccupants = 3, Status = "Occupied"

## Files đã sửa

### RoomBuildingService:
- ✅ `RoomsController.cs` - Thêm endpoint UpdateOccupancy

### ContractStudentService:
- ✅ `RoomServiceClient.cs` - Service helper mới
- ✅ `Program.cs` - Đăng ký RoomServiceClient
- ✅ `RoomApplicationsController.cs` - Gọi UpdateOccupancy khi duyệt
- ✅ `ContractsController.cs` - Gọi UpdateOccupancy khi terminate
- ✅ `appsettings.json` - Config RoomBuildingService URL

## Cấu hình

```json
{
  "Services": {
    "BillingService": "http://localhost:5002",
    "RoomBuildingService": "http://localhost:5003"
  }
}
```
