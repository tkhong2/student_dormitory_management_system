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
          <a-image :src="selectedRoom.image" :alt="`Phòng ${selectedRoom.name}`" style="border-radius: 8px; width: 100%;" />
        </a-col>
        <a-col :xs="24" :md="16">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;">
            <h3 style="font-size: 18px; font-weight: 700; margin: 0;">Phòng {{ selectedRoom.name }}</h3>
            <a-tag color="blue">{{ selectedRoom.building }}</a-tag>
          </div>
          <a-descriptions :column="2" size="small">
            <a-descriptions-item label="Loại phòng">
              <a-tag color="orange">{{ selectedRoom.roomTypeName || 'N/A' }}</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="Diện tích">{{ selectedRoom.area }} m²</a-descriptions-item>
            <a-descriptions-item label="Sức chứa">{{ selectedRoom.capacity }} người</a-descriptions-item>
            <a-descriptions-item label="Chỗ trống">{{ selectedRoom.available }} chỗ</a-descriptions-item>
            <a-descriptions-item label="Tiện nghi" :span="2">
              <div style="display: flex; flex-wrap: wrap; gap: 4px;">
                <a-tag v-for="(amenity, index) in roomAmenities" :key="index" color="green" size="small">
                  {{ amenity }}
                </a-tag>
                <span v-if="!roomAmenities || roomAmenities.length === 0" style="color: #999;">
                  Tiện nghi cơ bản
                </span>
              </div>
            </a-descriptions-item>
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
        
        <!-- Thông tin cá nhân (auto-filled, read-only) -->
        <a-divider orientation="left">Thông tin cá nhân</a-divider>
        <a-row :gutter="16">
          <a-col :xs="24" :md="12">
            <a-form-item label="Họ và tên">
              <a-input v-model:value="form.fullName" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Mã sinh viên">
              <a-input v-model:value="form.studentCode" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Email">
              <a-input v-model:value="form.email" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Số điện thoại">
              <a-input v-model:value="form.phone" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Giới tính">
              <a-input v-model:value="form.gender" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Ngày sinh">
              <a-input v-model:value="form.dateOfBirth" disabled />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item label="Địa chỉ thường trú">
              <a-textarea v-model:value="form.address" :rows="2" disabled />
            </a-form-item>
          </a-col>
        </a-row>

        <!-- Thời gian thuê -->
        <a-divider orientation="left">Thời gian thuê</a-divider>
        <a-row :gutter="16">
          <a-col :xs="24" :md="12">
            <a-form-item label="Thời hạn thuê *" :validate-status="errors.duration ? 'error' : ''" :help="errors.duration">
              <a-select v-model:value="form.duration" placeholder="Chọn thời hạn thuê">
                <a-select-option :value="3">3 tháng</a-select-option>
                <a-select-option :value="6">6 tháng</a-select-option>
                <a-select-option :value="12">1 năm</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-checkbox v-model:checked="form.isLocalStudent">Sinh viên nội tỉnh</a-checkbox>
          </a-col>
        </a-row>

        <!-- Nguyện vọng -->
        <a-divider orientation="left">Nguyện vọng</a-divider>
        <a-row :gutter="16">
          <a-col :xs="24">
            <a-form-item label="Nguyện vọng">
              <a-textarea v-model:value="form.preferences" :rows="3" placeholder="Nguyện vọng về bạn cùng phòng, vị trí phòng, tầng lầu..." />
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item label="Ghi chú">
              <a-textarea v-model:value="form.note" :rows="2" placeholder="Ghi chú thêm..." />
            </a-form-item>
          </a-col>
        </a-row>

        <!-- Liên hệ khẩn cấp -->
        <a-divider orientation="left">Người liên hệ khẩn cấp</a-divider>
        <a-row :gutter="16">
          <a-col :xs="24" :md="12">
            <a-form-item label="Họ tên *" :validate-status="errors.emergencyContactName ? 'error' : ''" :help="errors.emergencyContactName">
              <a-input v-model:value="form.emergencyContactName" placeholder="Nhập họ tên người liên hệ khẩn cấp" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Số điện thoại *" :validate-status="errors.emergencyContactPhone ? 'error' : ''" :help="errors.emergencyContactPhone">
              <a-input v-model:value="form.emergencyContactPhone" placeholder="Nhập số điện thoại" />
            </a-form-item>
          </a-col>
          <a-col :xs="24" :md="12">
            <a-form-item label="Mối quan hệ *" :validate-status="errors.emergencyContactRelationship ? 'error' : ''" :help="errors.emergencyContactRelationship">
              <a-select v-model:value="form.emergencyContactRelationship" placeholder="Chọn mối quan hệ">
                <a-select-option value="Bố">Bố</a-select-option>
                <a-select-option value="Mẹ">Mẹ</a-select-option>
                <a-select-option value="Anh">Anh trai</a-select-option>
                <a-select-option value="Chị">Chị gái</a-select-option>
                <a-select-option value="Em">Em</a-select-option>
                <a-select-option value="Ông">Ông</a-select-option>
                <a-select-option value="Bà">Bà</a-select-option>
                <a-select-option value="Khác">Khác</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <!-- Cam kết -->
        <a-divider orientation="left">Cam kết</a-divider>
        <a-row :gutter="16">
          <a-col :xs="24">
            <a-form-item :validate-status="errors.agreedToRegulations ? 'error' : ''" :help="errors.agreedToRegulations">
              <a-checkbox v-model:checked="form.agreedToRegulations">
                Tôi đã đọc và đồng ý với <a href="#" @click.prevent="showRegulations">quy định ký túc xá</a>
              </a-checkbox>
            </a-form-item>
          </a-col>
          <a-col :xs="24">
            <a-form-item :validate-status="errors.confirmedInformationAccuracy ? 'error' : ''" :help="errors.confirmedInformationAccuracy">
              <a-checkbox v-model:checked="form.confirmedInformationAccuracy">
                Tôi xác nhận các thông tin cung cấp là chính xác
              </a-checkbox>
            </a-form-item>
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { SendOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { useAuthStore } from '@/stores/auth'
import dayjs from 'dayjs'

const router = useRouter()
const authStore = useAuthStore()
const loading = ref(false)
const submitting = ref(false)
const selectedRoom = ref(null)

// Parse amenities from selectedRoom
const roomAmenities = computed(() => {
  if (!selectedRoom.value) return []
  
  // If amenities is a string, parse it
  if (typeof selectedRoom.value.amenities === 'string') {
    // Try to parse as JSON array or comma-separated
    try {
      const parsed = JSON.parse(selectedRoom.value.amenities)
      return Array.isArray(parsed) ? parsed : []
    } catch {
      // If not JSON, try comma-separated
      return selectedRoom.value.amenities.split(',').map(a => a.trim()).filter(a => a)
    }
  }
  
  // If already an array
  if (Array.isArray(selectedRoom.value.amenities)) {
    return selectedRoom.value.amenities
  }
  
  // Fallback: try facilities field
  if (selectedRoom.value.facilities) {
    if (typeof selectedRoom.value.facilities === 'string') {
      return selectedRoom.value.facilities.split(',').map(a => a.trim()).filter(a => a)
    }
    if (Array.isArray(selectedRoom.value.facilities)) {
      return selectedRoom.value.facilities
    }
  }
  
  return []
})

const form = ref({
  // Auto-filled from user data (read-only)
  fullName: '',
  studentCode: '',
  email: '',
  phone: '',
  gender: '',
  dateOfBirth: '',
  address: '',
  // User input fields
  duration: undefined,
  isLocalStudent: false,
  preferences: '',
  note: '',
  emergencyContactName: '',
  emergencyContactPhone: '',
  emergencyContactRelationship: undefined,
  agreedToRegulations: false,
  confirmedInformationAccuracy: false
})

const errors = ref({})

onMounted(async () => {
  // Lấy thông tin phòng đã chọn từ localStorage
  const roomData = localStorage.getItem('selectedRoom')
  if (roomData) {
    selectedRoom.value = JSON.parse(roomData)
  } else {
    message.warning('Vui lòng chọn phòng trước khi đăng ký')
    router.push('/rooms')
    return
  }

  // Refresh user data from API to get latest info
  await authStore.refreshUserData()

  // Auto-fill user data from auth store (User table now has gender, DOB, address)
  if (authStore.user) {
    form.value.fullName = authStore.user.fullName || ''
    form.value.email = authStore.user.email || ''
    form.value.phone = authStore.user.phone || ''
    form.value.studentCode = authStore.user.studentCode || authStore.user.username || ''
    
    // Format gender
    if (authStore.user.gender) {
      form.value.gender = authStore.user.gender === 'Male' ? 'Nam' : authStore.user.gender === 'Female' ? 'Nữ' : authStore.user.gender
    } else {
      form.value.gender = 'Chưa cập nhật'
    }
    
    // Format date of birth
    if (authStore.user.dateOfBirth) {
      const date = new Date(authStore.user.dateOfBirth)
      form.value.dateOfBirth = date.toLocaleDateString('vi-VN')
    } else {
      form.value.dateOfBirth = 'Chưa cập nhật'
    }
    
    form.value.address = authStore.user.address || 'Chưa cập nhật'
  }
})

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const showRegulations = () => {
  message.info('Tính năng xem quy định sẽ được bổ sung sau')
}

const validateForm = () => {
  errors.value = {}
  
  if (!form.value.duration) errors.value.duration = 'Vui lòng chọn thời hạn thuê'
  if (!form.value.emergencyContactName) errors.value.emergencyContactName = 'Vui lòng nhập họ tên người liên hệ khẩn cấp'
  if (!form.value.emergencyContactPhone) errors.value.emergencyContactPhone = 'Vui lòng nhập số điện thoại'
  if (!form.value.emergencyContactRelationship) errors.value.emergencyContactRelationship = 'Vui lòng chọn mối quan hệ'
  
  if (!form.value.agreedToRegulations) {
    errors.value.agreedToRegulations = 'Bạn phải đồng ý với quy định ký túc xá'
  }
  
  if (!form.value.confirmedInformationAccuracy) {
    errors.value.confirmedInformationAccuracy = 'Bạn phải xác nhận thông tin chính xác'
  }
  
  return Object.keys(errors.value).length === 0
}

const submitRegistration = async () => {
  if (!validateForm()) {
    message.error('Vui lòng điền đầy đủ thông tin và hoàn tất cam kết')
    return
  }

  if (!selectedRoom.value) {
    message.error('Không tìm thấy thông tin phòng')
    return
  }

  submitting.value = true

  try {
    const userId = authStore.user.id

    if (!userId) {
      message.error('Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.')
      submitting.value = false
      return
    }

    // Calculate start and end dates based on duration
    const startDate = dayjs()
    const endDate = startDate.add(form.value.duration, 'month')

    const applicationData = {
      studentId: userId,
      preferredBuildingId: selectedRoom.value.buildingId,
      preferredBuildingName: selectedRoom.value.building,
      preferredRoomTypeId: selectedRoom.value.roomTypeId,
      preferredRoomTypeName: selectedRoom.value.roomTypeName,
      preferredRoomPrice: selectedRoom.value.price,
      requestedStartDate: startDate.format('YYYY-MM-DD'),
      requestedEndDate: endDate.format('YYYY-MM-DD'),
      durationMonths: form.value.duration,
      preferences: form.value.preferences,
      note: form.value.note,
      isLocalStudent: form.value.isLocalStudent,
      emergencyContactName: form.value.emergencyContactName,
      emergencyContactPhone: form.value.emergencyContactPhone,
      emergencyContactRelationship: form.value.emergencyContactRelationship,
      agreedToRegulations: form.value.agreedToRegulations,
      confirmedInformationAccuracy: form.value.confirmedInformationAccuracy,
      // Assigned room info
      assignedRoomId: selectedRoom.value.id,
      assignedRoomNumber: selectedRoom.value.name,
      assignedBuildingName: selectedRoom.value.building,
      // User info for Student record creation
      userFullName: authStore.user.fullName,
      userEmail: authStore.user.email,
      userPhone: authStore.user.phone
    }

    console.log('Sending application data:', applicationData)

    await roomApplicationService.create(applicationData)

    message.success('Gửi đơn đăng ký thành công! Vui lòng đợi nhân viên duyệt.')
    
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
