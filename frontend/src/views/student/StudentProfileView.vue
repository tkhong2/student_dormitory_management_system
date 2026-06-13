<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Hồ sơ cá nhân</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Xem và cập nhật thông tin của bạn</p>

    <v-row>
      <!-- Avatar & basic -->
      <v-col cols="12" md="4">
        <v-card class="pa-6 text-center" style="border:1px solid #e5e7eb">
          <!-- Avatar with upload -->
          <div style="position: relative; display: inline-block;" class="mb-4">
            <v-avatar size="120" class="elevation-4" style="border: 4px solid #fff; cursor: pointer;" @click="triggerAvatarUpload">
              <v-img :src="userAvatarUrl" />
              <v-overlay
                activator="parent"
                class="align-center justify-center"
                opacity="0.7"
              >
                <v-icon size="32" color="white">mdi-camera</v-icon>
              </v-overlay>
            </v-avatar>
            
            <!-- Hidden file input -->
            <input
              ref="avatarInput"
              type="file"
              accept="image/*"
              style="display: none"
              @change="handleAvatarChange"
            />
            
            <!-- Upload button overlay -->
            <v-menu location="bottom">
              <template v-slot:activator="{ props }">
                <v-btn
                  v-bind="props"
                  icon
                  size="small"
                  color="primary"
                  style="position: absolute; bottom: 0; right: 0;"
                  elevation="2"
                >
                  <v-icon>mdi-camera</v-icon>
                </v-btn>
              </template>
              
              <v-list density="compact" class="py-2">
                <v-list-item @click="triggerAvatarUpload">
                  <template v-slot:prepend>
                    <v-icon size="small">mdi-upload</v-icon>
                  </template>
                  <v-list-item-title>Tải ảnh lên</v-list-item-title>
                </v-list-item>
                
                <v-list-item @click="viewAvatar">
                  <template v-slot:prepend>
                    <v-icon size="small">mdi-eye</v-icon>
                  </template>
                  <v-list-item-title>Xem ảnh</v-list-item-title>
                </v-list-item>
                
                <v-list-item v-if="currentUser.avatarUrl" @click="removeAvatar">
                  <template v-slot:prepend>
                    <v-icon size="small" color="error">mdi-delete</v-icon>
                  </template>
                  <v-list-item-title class="text-error">Xóa ảnh</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </div>
          
          <h3 class="text-h6 font-weight-bold mb-1">{{ currentUser.fullName }}</h3>
          <div class="text-body-2 text-medium-emphasis mb-3">
            {{ currentUser.studentCode }}
            <span v-if="currentUser.classCode"> · {{ currentUser.classCode }}</span>
          </div>
          <v-chip color="success" variant="tonal" class="mb-4">Đang ở KTX</v-chip>
          <v-divider class="my-4" />
          <div class="text-left">
            <div class="d-flex align-center ga-2 mb-3">
              <v-icon size="18" color="primary">mdi-email-outline</v-icon>
              <span class="text-body-2">{{ currentUser.email }}</span>
            </div>
            <div class="d-flex align-center ga-2 mb-3">
              <v-icon size="18" color="primary">mdi-phone-outline</v-icon>
              <span class="text-body-2">{{ currentUser.phone }}</span>
            </div>
            <div v-if="currentUser.faculty" class="d-flex align-center ga-2 mb-3">
              <v-icon size="18" color="primary">mdi-school-outline</v-icon>
              <span class="text-body-2">{{ currentUser.faculty }}</span>
            </div>
            <div class="d-flex align-center ga-2">
              <v-icon size="18" color="primary">mdi-map-marker-outline</v-icon>
              <span class="text-body-2">{{ currentUser.address || 'Chưa cập nhật' }}</span>
            </div>
          </div>
        </v-card>
      </v-col>

      <!-- Edit form -->
      <v-col cols="12" md="8">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-5">Chỉnh sửa thông tin</h3>
          <v-row>
            <v-col cols="6"><v-text-field label="Mã sinh viên" model-value="SV001" disabled /></v-col>
            <v-col cols="6"><v-text-field label="Họ và tên" model-value="Nguyễn Văn An" /></v-col>
            <v-col cols="6"><v-text-field label="Email" model-value="vanan@gmail.com" type="email" /></v-col>
            <v-col cols="6"><v-text-field label="Số điện thoại" model-value="0901234567" /></v-col>
            <v-col cols="6"><v-text-field label="Ngày sinh" model-value="2003-05-15" type="date" /></v-col>
            <v-col cols="6"><v-select label="Giới tính" :items="['Nam','Nữ']" model-value="Nam" /></v-col>
            <v-col cols="6"><v-text-field label="Lớp" model-value="K65-CNTT" disabled /></v-col>
            <v-col cols="6"><v-text-field label="Khoa" model-value="Công nghệ thông tin" disabled /></v-col>
            <v-col cols="12"><v-textarea label="Địa chỉ thường trú" model-value="Số 10, Ngõ 5, Phường X, Quận Y, Hà Nội" rows="2" /></v-col>
          </v-row>
          <div class="d-flex justify-end ga-3 mt-4">
            <v-btn variant="text">Hủy thay đổi</v-btn>
            <v-btn color="primary">Lưu thông tin</v-btn>
          </div>
        </v-card>

        <v-card class="pa-6 mt-4" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-5">Đổi mật khẩu</h3>
          <v-row>
            <v-col cols="12" sm="4"><v-text-field label="Mật khẩu hiện tại" type="password" /></v-col>
            <v-col cols="12" sm="4"><v-text-field label="Mật khẩu mới" type="password" /></v-col>
            <v-col cols="12" sm="4"><v-text-field label="Xác nhận mật khẩu" type="password" /></v-col>
          </v-row>
          <div class="d-flex justify-end mt-2">
            <v-btn color="warning" variant="tonal" style="color: white;">Đổi mật khẩu</v-btn>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { message } from 'ant-design-vue'
import axios from 'axios'

const authStore = useAuthStore()
const avatarInput = ref(null)
const uploading = ref(false)

// Get current user
const currentUser = computed(() => authStore.user || {
  fullName: 'Sinh viên',
  studentCode: 'SV???',
  email: 'N/A',
  phone: 'N/A',
  address: 'Chưa cập nhật',
  avatarUrl: null,
  faculty: null,
  classCode: null
})

// Computed avatar URL
const userAvatarUrl = computed(() => {
  if (currentUser.value.avatarUrl) {
    if (currentUser.value.avatarUrl.startsWith('http')) {
      return currentUser.value.avatarUrl
    }
    return `http://localhost:5003${currentUser.value.avatarUrl}`
  }
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(currentUser.value.fullName || 'SV')}&background=6366f1&color=fff&size=256`
})

const triggerAvatarUpload = () => {
  avatarInput.value?.click()
}

const handleAvatarChange = async (event) => {
  const file = event.target.files?.[0]
  if (!file) return

  // Validate file type
  if (!file.type.startsWith('image/')) {
    message.error('Vui lòng chọn file ảnh!')
    return
  }

  // Validate file size (max 5MB)
  if (file.size > 5 * 1024 * 1024) {
    message.error('Kích thước ảnh không được vượt quá 5MB!')
    return
  }

  uploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await axios.post('http://localhost:5003/api/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    if (response.data?.url) {
      // Update user avatar
      await updateUserAvatar(response.data.url)
      message.success('Cập nhật ảnh đại diện thành công!')
    }
  } catch (error) {
    console.error('Error uploading avatar:', error)
    message.error('Lỗi tải ảnh lên!')
  } finally {
    uploading.value = false
    // Reset input
    if (avatarInput.value) {
      avatarInput.value.value = ''
    }
  }
}

const updateUserAvatar = async (avatarUrl) => {
  try {
    // TODO: Call API to update user avatar
    // For now, just update the store
    authStore.user.avatarUrl = avatarUrl
    localStorage.setItem('user', JSON.stringify(authStore.user))
  } catch (error) {
    console.error('Error updating avatar:', error)
    throw error
  }
}

const viewAvatar = () => {
  window.open(userAvatarUrl.value, '_blank')
}

const removeAvatar = async () => {
  try {
    // TODO: Call API to remove avatar
    authStore.user.avatarUrl = null
    localStorage.setItem('user', JSON.stringify(authStore.user))
    message.success('Đã xóa ảnh đại diện!')
  } catch (error) {
    console.error('Error removing avatar:', error)
    message.error('Lỗi xóa ảnh đại diện!')
  }
}
</script>
