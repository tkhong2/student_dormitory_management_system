# Validation: Giới hạn số phòng theo tòa nhà

## Mục đích
Ngăn không cho thêm phòng vượt quá số phòng tối đa của tòa nhà.

## Đã thực hiện

### 1. **Backend Validation** (RoomsController.cs)
✅ Kiểm tra số phòng hiện tại trước khi thêm
✅ Trả về lỗi 400 nếu tòa nhà đã đủ phòng
✅ Kiểm tra trùng số phòng trong cùng tầng

**Ví dụ response lỗi**:
```json
{
  "message": "Không thể thêm phòng. Tòa A đã đủ 96 phòng (hiện có 96 phòng)"
}
```

### 2. **Frontend Validation** (RoomListView.vue)
✅ Hiển thị số phòng còn có thể thêm khi chọn tòa
✅ Tag màu xanh: Còn có thể thêm
✅ Tag màu đỏ: Đã đủ phòng
✅ Chặn submit nếu tòa đã đủ phòng

**UI hiển thị**:
```
Tòa nhà: [Tòa A ▼]
✅ Còn có thể thêm 10 phòng (86/96)

hoặc

Tòa nhà: [Tòa B ▼]
❌ Tòa nhà đã đủ 96 phòng
```

## Test Cases

### Test 1: Thêm phòng khi còn slot
- Chọn Tòa A (giả sử có 50/96 phòng)
- Thấy: "Còn có thể thêm 46 phòng (50/96)" ✅
- Nhập thông tin → Lưu thành công ✅

### Test 2: Thêm phòng khi đã đủ
- Chọn Tòa B (giả sử có 96/96 phòng)
- Thấy: "Tòa nhà đã đủ 96 phòng" ❌
- Click Lưu → Hiển thị lỗi: "Không thể thêm phòng..." ❌

### Test 3: Backend reject
- Bypass frontend validation
- POST /api/rooms với buildingId đã đủ phòng
- Response 400: "Không thể thêm phòng. Tòa A đã đủ 96 phòng..." ❌

## Lưu ý
- Validation hoạt động **cả frontend và backend**
- Không ảnh hưởng khi **SỬA phòng** (chỉ validate khi THÊM MỚI)
- Số phòng hiện tại được tính real-time từ database
