<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Đăng ký phòng</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Hoàn tất đăng ký phòng ký túc xá</p>
    </div>

    <!-- Selected Room Info -->
    <a-card v-if="selectedRoom" style="margin-bottom: 16px;" :bordered="false">
      <h3 style="font-size: 16px; font-weight: 600; margin-bottom: 16px;">Phòng đã chọn</h3>
      <a-row :gutter="16">
        <a-col :xs="24" :md="8">
          <a-image :src="selectedRoom.image" :alt="selectedRoom.name" style="border-radius: 8px; width: 100%;" />
        </a-col>
        <a-col :xs="24" :md="16">
          <div style="display: flex; justify-content: space-between; margin-bottom: 12px;">
            <h3 style="font-size: 18px; font-weight: 700; margin: 0;">{{ selectedRoom.name }}</h3>
            <a-tag color="blue">{{ selectedRoom.building }}</a-tag>
          </div>
          <a-descriptions :column="2" size="small">
            <a-descriptions-item label="Sức chứa">{{ selectedRoom.capacity }} người</a-descriptions-item>
            <a-descriptions-item label="Diện tích">{{ selectedRoom.area }} m²</a-descriptions-item>
            <a-descriptions-item label="Tiện nghi">{{ selectedRoom.facilities }}</a-descriptions-item>
            <a-descriptions-item label="Chỗ trống">{{ selectedRoom.available }} chỗ</a-descriptions-item>
            <a-descriptions-item label="Giá thuê/tháng" :span="2">
              <span style="font-size: 18px; font-weight: 700; color: #ff6b00;">{{ formatPrice(selectedRoom.price) }}</span>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
    </a-card>

    <!-- Registration Form -->
    <a-card :bordered="false" :loading="loading">
      <h3 style="font-size: 16px; font-weight: 600; margin-bottom: 16px;">Thông tin đăng ký</h3>
      <a-form layout="vertical" :model="form">
        <a-row :gutter="16">
          <a-col :xs="24" :md="12">
            <a-form-item label="Họ và tên *" :validate-status="errors.fullName ? 'error' : ''" :help="errors.fullName">
              <a-input v-model:value="form.fullName" placeholder="Nhập họ và tên" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Mã sinh viên *" :validate-status="errors.studentCode ? 'error' : ''" :help="errors.studentCode">
              <a-input v-model:value="form.studentCode" placeholder="Nhập mã sinh viên" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Email *" :validate-status="errors.email ? 'error' : ''" :help="errors.email">
              <a-input v-model:value="form.email" type="email" placeholder="Nhập email" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Số điện thoại *" :validate-status="errors.phone ? 'error' : ''" :help="errors.phone">
              <a-input v-model:value="form.phone" placeholder="Nhập số điện thoại" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Giới tính *" :validate-status="errors.gender ? 'error' : ''" :help="errors.gender">
              <a-select v-model:value="form.gender" placeholder="Chọn giới tính">
                <a-select-option value="Nam">Nam</a-select-option>
                <a-select-option value="Nữ">Nữ</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Ngày sinh *" :validate-status="errors.dateOfBirth ? 'error' : ''" :help="errors.dateOfBirth">
              <a-date-picker 
                v-model:value="form.dateOfBirth" 
                placeholder="Chọn ngày sinh" 
                style="width: 100%;"
                format="DD/MM/YYYY"
              />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Ngày bắt đầu *" :validate-status="errors.startDate ? 'error' : ''" :help="errors.startDate">
              <a-date-picker 
                v-model:value="form.startDate" 
                placeholder="Chọn ngày bắt đầu ở" 
                style="width: 100%;"
                format="DD/MM/YYYY"
              />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Ngày kết thúc *" :validate-status="errors.endDate ? 'error' : ''" :help="errors.endDate">
              <a-date-picker 
                v-model:value="form.endDate" 
                placeholder="Chọn ngày kết thúc" 
                style="width: 100%;"
                format="DD/MM/YYYY"
              />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item label="Địa chỉ thường trú">
              <a-textarea v-model:value="form.address" :rows="2" placeholder="Nhập địa chỉ thường trú" />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item label="Yêu cầu đặc biệt">
              <a-textarea v-model:value="form.specialRequirements" :rows="2" placeholder="Yêu cầu về bạn cùng phòng, vị trí phòng..." />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item label="Ghi chú">
              <a-textarea v-model:value="form.note" :rows="2" placeholder="Ghi chú thêm..." />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-checkbox v-model:checked="form.isLocalStudent">Sinh viên nội tỉnh</a-checkbox>
          </a-col>
        </a-row>

        <a-alert type="info" show-icon style="margin-top: 24px; margin-bottom: 24px;">
          <template #message>
            <strong>Lưu ý quan trọng:</strong>
          </template>
          <template #description>
            <ul style="margin: 8px 0 0 0; padding-left: 20px;">
              <li>Sau khi gửi đơn, bạn sẽ nhận được email xác nhận trong vòng 24-48h</li>
              <li>Vui lòng chuẩn bị đầy đủ giấy tờ: CMND/CCCD, Giấy xác nhận sinh viên</li>
              <li>Đơn đăng ký sẽ được xét duyệt theo thứ tự và mức độ ưu tiên</li>
              <li>Sau khi được duyệt, hợp đồng sẽ được tạo tự động</li>
            </ul>
          </template>
        </a-alert>

        <div style="display: flex; justify-content: flex-end; gap: 12px;">
          <a-button size="large" @click="$router.push('/rooms')">
            Hủy
          </a-button>
          <a-button 
            type="primary" 
            size="large" 
            @click="submitRegistration"
            :loading="submitting"
            style="background: #ff9800; border-color: #ff9800;"
          >
            <template #icon><SendOutlined /></template>
            Gửi đơn đăng ký
          </a-button>
        </div>
      </a-form>
    </a-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { SendOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import dayjs from 'dayjs'

const router = useRouter()
const loading = ref(false)
const submitting = ref(false)
const selectedRoom = ref(null)

const form = ref({
  fullName: '',
  studentCode: '',
  email: '',
  phone: '',
  gender: undefined,
  dateOfBirth: null,
  startDate: null,
  endDate: null,
  address: '',
  specialRequirements: '',
  note: '',
  isLocalStudent: false
})

const errors = ref({})

onMounted(() => {
  // Lấy thông tin phòng đã chọn từ localStorage
  const roomData = localStorage.getItem('selectedRoom')
  if (roomData) {
    selectedRoom.value = JSON.parse(roomData)
  } else {
    message.warning('Vui lòng chọn phòng trước khi đăng ký')
    router.push('/rooms')
  }

  // Lấy thông tin user từ localStorage để tự động điền form
  const user = JSON.parse(localStorage.getItem('user') || '{}')
  if (user) {
    form.value.fullName = user.fullName || ''
    form.value.email = user.email || ''
    form.value.phone = user.phone || ''
  }
})

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const validateForm = () => {
  errors.value = {}
  
  if (!form.value.fullName) errors.value.fullName = 'Vui lòng nhập họ tên'
  if (!form.value.studentCode) errors.value.studentCode = 'Vui lòng nhập mã sinh viên'
  if (!form.value.email) errors.value.email = 'Vui lòng nhập email'
  if (!form.value.phone) errors.value.phone = 'Vui lòng nhập số điện thoại'
  if (!form.value.gender) errors.value.gender = 'Vui lòng chọn giới tính'
  if (!form.value.dateOfBirth) errors.value.dateOfBirth = 'Vui lòng chọn ngày sinh'
  if (!form.value.startDate) errors.value.startDate = 'Vui lòng chọn ngày bắt đầu'
  if (!form.value.endDate) errors.value.endDate = 'Vui lòng chọn ngày kết thúc'
  
  return Object.keys(errors.value).length === 0
}

const submitRegistration = async () => {
  if (!validateForm()) {
    message.error('Vui lòng điền đầy đủ thông tin')
    return
  }

  if (!selectedRoom.value) {
    message.error('Không tìm thấy thông tin phòng')
    return
  }

  submitting.value = true

  try {
    // Lấy thông tin user từ localStorage (user đã login)
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    if (!userId) {
      message.error('Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.')
      submitting.value = false
      return
    }

    // Sử dụng userId làm studentId (vì chưa có bảng Students riêng)
    const studentId = userId

    const applicationData = {
      studentId: studentId,
      preferredBuildingId: selectedRoom.value.buildingId,
      preferredBuildingName: selectedRoom.value.building,
      preferredRoomTypeId: selectedRoom.value.roomTypeId,
      preferredRoomTypeName: selectedRoom.value.roomTypeName,
      preferredRoomPrice: selectedRoom.value.price,
      requestedStartDate: dayjs(form.value.startDate).format('YYYY-MM-DD'),
      requestedEndDate: dayjs(form.value.endDate).format('YYYY-MM-DD'),
      specialRequirements: form.value.specialRequirements,
      note: form.value.note,
      isLocalStudent: form.value.isLocalStudent,
      // *** Thông tin phòng đã chọn (KEY CHANGE) ***
      assignedRoomId: selectedRoom.value.id,
      assignedRoomNumber: selectedRoom.value.name,
      assignedBuildingName: selectedRoom.value.building,
      // *** Thông tin user để tạo Student record ***
      userFullName: user.fullName,
      userEmail: user.email,
      userPhone: user.phone
    }

    console.log('Sending application data:', applicationData)

    await roomApplicationService.create(applicationData)

    message.success('Gửi đơn đăng ký thành công! Vui lòng đợi admin duyệt.')
    
    // Xóa selectedRoom khỏi localStorage
    localStorage.removeItem('selectedRoom')
    
    // Redirect về dashboard của sinh viên
    setTimeout(() => {
      router.push('/student')
    }, 1500)
  } catch (error) {
    console.error('Error submitting registration:', error)
    console.error('Error response:', error.response)
    message.error(error.response?.data?.message || error.message || 'Có lỗi xảy ra khi gửi đơn đăng ký')
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 600;
}
</style>
