<template>
  <div>
    <h1 style="font-size: 28px; font-weight: bold; margin-bottom: 4px;">Hồ sơ cá nhân</h1>
    <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 24px;">Xem và cập nhật thông tin của bạn</p>

    <a-spin :spinning="loading" tip="Đang tải...">
      <a-row :gutter="16">
        <!-- Avatar & basic -->
        <a-col :xs="24" :md="8">
          <a-card :bordered="true" style="text-align: center; padding: 24px;">
          <!-- Avatar with upload -->
          <div style="position: relative; display: inline-block; margin-bottom: 16px;">
            <a-avatar :size="120" :src="userAvatarUrl" style="border: 4px solid #fff; box-shadow: 0 4px 6px rgba(0,0,0,0.1); cursor: pointer;" @click="triggerAvatarUpload" />
            
            <!-- Hidden file input -->
            <input
              ref="avatarInput"
              type="file"
              accept="image/*"
              style="display: none"
              @change="handleAvatarChange"
            />
            
            <!-- Upload button overlay -->
            <a-dropdown>
              <a-button 
                type="primary" 
                shape="circle" 
                size="small"
                style="position: absolute; bottom: 0; right: 0;"
              >
                <template #icon><camera-outlined /></template>
              </a-button>
              <template #overlay>
                <a-menu>
                  <a-menu-item @click="triggerAvatarUpload">
                    <upload-outlined /> Tải ảnh lên
                  </a-menu-item>
                  <a-menu-item @click="viewAvatar">
                    <eye-outlined /> Xem ảnh
                  </a-menu-item>
                  <a-menu-item v-if="profileData.avatarUrl" danger @click="removeAvatar">
                    <delete-outlined /> Xóa ảnh
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </div>
          
          <h3 style="font-size: 18px; font-weight: bold; margin-bottom: 4px;">{{ profileData.fullName }}</h3>
          <div style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 12px;">
            {{ profileData.studentCode }}
            <span v-if="profileData.classCode"> · {{ profileData.classCode }}</span>
          </div>
          <a-tag color="success" style="margin-bottom: 16px;">Đang ở KTX</a-tag>
          <a-divider style="margin: 16px 0;" />
          <div style="text-align: left;">
            <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 12px;">
              <mail-outlined style="font-size: 18px; color: #1890ff;" />
              <span style="font-size: 14px;">{{ profileData.email }}</span>
            </div>
            <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 12px;">
              <phone-outlined style="font-size: 18px; color: #1890ff;" />
              <span style="font-size: 14px;">{{ profileData.phone || 'Chưa cập nhật' }}</span>
            </div>
            <div v-if="profileData.faculty" style="display: flex; align-items: center; gap: 8px; margin-bottom: 12px;">
              <bank-outlined style="font-size: 18px; color: #1890ff;" />
              <span style="font-size: 14px;">{{ profileData.faculty }}</span>
            </div>
            <div style="display: flex; align-items: center; gap: 8px;">
              <environment-outlined style="font-size: 18px; color: #1890ff;" />
              <span style="font-size: 14px;">{{ profileData.address || 'Chưa cập nhật' }}</span>
            </div>
          </div>
        </a-card>
      </a-col>

      <!-- Edit form -->
      <a-col :xs="24" :md="16">
        <a-card :bordered="true" style="padding: 24px; margin-bottom: 16px;">
          <h3 style="font-size: 16px; font-weight: bold; margin-bottom: 20px;">Chỉnh sửa thông tin</h3>
          <a-form :model="editForm" layout="vertical">
            <a-row :gutter="16">
              <a-col :span="12">
                <a-form-item label="Mã sinh viên">
                  <a-input v-model:value="profileData.studentCode" disabled />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Họ và tên">
                  <a-input v-model:value="editForm.fullName" />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Email">
                  <a-input v-model:value="editForm.email" type="email" />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Số điện thoại">
                  <a-input v-model:value="editForm.phone" />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Ngày sinh">
                  <a-input v-model:value="editForm.dateOfBirth" type="date" />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Giới tính">
                  <a-select v-model:value="editForm.gender" :options="[{label: 'Nam', value: 'Nam'}, {label: 'Nữ', value: 'Nữ'}, {label: 'Khác', value: 'Khác'}]" />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Lớp">
                  <a-input v-model:value="profileData.classCode" disabled />
                </a-form-item>
              </a-col>
              <a-col :span="12">
                <a-form-item label="Khoa">
                  <a-input v-model:value="profileData.faculty" disabled />
                </a-form-item>
              </a-col>
              <a-col :span="24">
                <a-form-item label="Địa chỉ thường trú">
                  <a-textarea v-model:value="editForm.address" :rows="2" />
                </a-form-item>
              </a-col>
            </a-row>
            <div style="display: flex; justify-content: flex-end; gap: 12px; margin-top: 16px;">
              <a-button @click="resetForm">Hủy thay đổi</a-button>
              <a-button type="primary" @click="saveProfile" :loading="saving">Lưu thông tin</a-button>
            </div>
          </a-form>
        </a-card>

        <a-card :bordered="true" style="padding: 24px;">
          <h3 style="font-size: 16px; font-weight: bold; margin-bottom: 20px;">Đổi mật khẩu</h3>
          <a-form :model="passwordForm" layout="vertical">
            <a-row :gutter="16">
              <a-col :xs="24" :sm="8">
                <a-form-item label="Mật khẩu hiện tại">
                  <a-input-password v-model:value="passwordForm.currentPassword" />
                </a-form-item>
              </a-col>
              <a-col :xs="24" :sm="8">
                <a-form-item label="Mật khẩu mới">
                  <a-input-password v-model:value="passwordForm.newPassword" />
                </a-form-item>
              </a-col>
              <a-col :xs="24" :sm="8">
                <a-form-item label="Xác nhận mật khẩu">
                  <a-input-password v-model:value="passwordForm.confirmPassword" />
                </a-form-item>
              </a-col>
            </a-row>
            <div style="display: flex; justify-content: flex-end; margin-top: 8px;">
              <a-button style="background-color: #faad14; border-color: #faad14; color: white;" @click="changePassword" :loading="changingPassword">Đổi mật khẩu</a-button>
            </div>
          </a-form>
        </a-card>
      </a-col>
    </a-row>
    </a-spin>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { message } from 'ant-design-vue'
import axios from 'axios'
import { 
  CameraOutlined, 
  UploadOutlined, 
  EyeOutlined, 
  DeleteOutlined,
  MailOutlined,
  PhoneOutlined,
  BankOutlined,
  EnvironmentOutlined
} from '@ant-design/icons-vue'

const authStore = useAuthStore()
const avatarInput = ref(null)
const uploading = ref(false)
const loading = ref(false)
const saving = ref(false)
const changingPassword = ref(false)

const profileData = ref({
  id: null,
  studentCode: '',
  fullName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  gender: '',
  classCode: '',
  faculty: '',
  address: '',
  avatarUrl: null,
  // Full student data for API update
  major: '',
  academicYear: 0,
  placeOfBirth: '',
  ethnicity: '',
  religion: '',
  nationality: '',
  bloodType: '',
  healthInsuranceNumber: '',
  identityCard: '',
  identityCardIssuedDate: '',
  identityCardIssuedPlace: '',
  permanentProvince: '',
  temporaryAddress: '',
  emergencyContactName: '',
  emergencyContactPhone: '',
  emergencyContactRelation: '',
  emergencyContactAddress: '',
  isActive: true,
  notes: '',
  userId: null
})

const editForm = ref({
  fullName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  gender: '',
  address: ''
})

const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Computed avatar URL
const userAvatarUrl = computed(() => {
  if (profileData.value.avatarUrl) {
    if (profileData.value.avatarUrl.startsWith('http')) {
      return profileData.value.avatarUrl
    }
    return `http://localhost:5003${profileData.value.avatarUrl}`
  }
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(profileData.value.fullName || 'SV')}&background=1890ff&color=fff&size=256`
})

// Load profile data
const loadProfile = async () => {
  loading.value = true
  try {
    const user = authStore.user
    if (!user?.id) {
      message.error('Không tìm thấy thông tin người dùng!')
      return
    }

    // Get student info from ContractStudentService
    const response = await axios.get(`http://localhost:5001/api/students/by-user/${user.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    const student = response.data
    
    // Format dates
    let formattedDateOfBirth = ''
    if (student.dateOfBirth) {
      const date = new Date(student.dateOfBirth)
      formattedDateOfBirth = date.toISOString().split('T')[0]
    }

    let formattedIdIssuedDate = ''
    if (student.identityCardIssuedDate) {
      const date = new Date(student.identityCardIssuedDate)
      formattedIdIssuedDate = date.toISOString().split('T')[0]
    }

    // Store full student data
    profileData.value = {
      id: student.id,
      studentCode: student.studentCode || '',
      fullName: student.fullName || '',
      email: student.email || '',
      phone: student.phone || '',
      dateOfBirth: formattedDateOfBirth,
      gender: student.gender || 'Nam',
      classCode: student.classCode || '',
      faculty: student.faculty || '',
      address: student.permanentAddress || '',
      avatarUrl: student.avatarUrl || null,
      // Full data for API
      major: student.major || '',
      academicYear: student.academicYear || 2024,
      placeOfBirth: student.placeOfBirth || '',
      ethnicity: student.ethnicity || '',
      religion: student.religion || '',
      nationality: student.nationality || 'Việt Nam',
      bloodType: student.bloodType || '',
      healthInsuranceNumber: student.healthInsuranceNumber || '',
      identityCard: student.identityCard || '',
      identityCardIssuedDate: formattedIdIssuedDate,
      identityCardIssuedPlace: student.identityCardIssuedPlace || '',
      permanentProvince: student.permanentProvince || '',
      temporaryAddress: student.temporaryAddress || '',
      emergencyContactName: student.emergencyContactName || '',
      emergencyContactPhone: student.emergencyContactPhone || '',
      emergencyContactRelation: student.emergencyContactRelation || '',
      emergencyContactAddress: student.emergencyContactAddress || '',
      isActive: student.isActive !== false,
      notes: student.notes || '',
      userId: student.userId
    }

    // Copy editable fields to edit form
    editForm.value = {
      fullName: profileData.value.fullName,
      email: profileData.value.email,
      phone: profileData.value.phone,
      dateOfBirth: profileData.value.dateOfBirth,
      gender: profileData.value.gender,
      address: profileData.value.address
    }

  } catch (error) {
    console.error('Error loading profile:', error)
    message.error('Lỗi tải thông tin hồ sơ!')
  } finally {
    loading.value = false
  }
}

const triggerAvatarUpload = () => {
  avatarInput.value?.click()
}

// Save profile
const saveProfile = async () => {
  saving.value = true
  try {
    // Build full CreateStudentDto with updated editable fields
    const updateData = {
      studentCode: profileData.value.studentCode,
      faculty: profileData.value.faculty,
      major: profileData.value.major,
      academicYear: profileData.value.academicYear,
      classCode: profileData.value.classCode,
      fullName: editForm.value.fullName, // from edit form
      gender: editForm.value.gender, // from edit form
      dateOfBirth: editForm.value.dateOfBirth, // from edit form
      placeOfBirth: profileData.value.placeOfBirth,
      ethnicity: profileData.value.ethnicity,
      religion: profileData.value.religion,
      nationality: profileData.value.nationality,
      bloodType: profileData.value.bloodType,
      healthInsuranceNumber: profileData.value.healthInsuranceNumber,
      phone: editForm.value.phone, // from edit form
      email: editForm.value.email, // from edit form
      identityCard: profileData.value.identityCard,
      identityCardIssuedDate: profileData.value.identityCardIssuedDate,
      identityCardIssuedPlace: profileData.value.identityCardIssuedPlace,
      permanentAddress: editForm.value.address, // from edit form
      permanentProvince: profileData.value.permanentProvince,
      temporaryAddress: profileData.value.temporaryAddress,
      emergencyContactName: profileData.value.emergencyContactName,
      emergencyContactPhone: profileData.value.emergencyContactPhone,
      emergencyContactRelation: profileData.value.emergencyContactRelation,
      emergencyContactAddress: profileData.value.emergencyContactAddress,
      avatarUrl: profileData.value.avatarUrl,
      isActive: profileData.value.isActive,
      notes: profileData.value.notes,
      userId: profileData.value.userId
    }

    await axios.put(`http://localhost:5001/api/students/${profileData.value.id}`, updateData, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Update local data
    profileData.value.fullName = editForm.value.fullName
    profileData.value.email = editForm.value.email
    profileData.value.phone = editForm.value.phone
    profileData.value.dateOfBirth = editForm.value.dateOfBirth
    profileData.value.gender = editForm.value.gender
    profileData.value.address = editForm.value.address
    
    // Update auth store
    authStore.user.fullName = editForm.value.fullName
    authStore.user.email = editForm.value.email
    authStore.user.phone = editForm.value.phone
    localStorage.setItem('user', JSON.stringify(authStore.user))

    message.success('Cập nhật thông tin thành công!')
  } catch (error) {
    console.error('Error saving profile:', error)
    message.error('Lỗi cập nhật thông tin!')
  } finally {
    saving.value = false
  }
}

// Reset form
const resetForm = () => {
  editForm.value = {
    fullName: profileData.value.fullName,
    email: profileData.value.email,
    phone: profileData.value.phone,
    dateOfBirth: profileData.value.dateOfBirth,
    gender: profileData.value.gender,
    address: profileData.value.address
  }
  message.info('Đã khôi phục thông tin ban đầu')
}

// Change password
const changePassword = async () => {
  if (!passwordForm.value.currentPassword || !passwordForm.value.newPassword || !passwordForm.value.confirmPassword) {
    message.warning('Vui lòng điền đầy đủ thông tin!')
    return
  }

  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    message.error('Mật khẩu xác nhận không khớp!')
    return
  }

  if (passwordForm.value.newPassword.length < 6) {
    message.error('Mật khẩu mới phải có ít nhất 6 ký tự!')
    return
  }

  changingPassword.value = true
  try {
    await axios.post('http://localhost:5002/api/auth/change-password', {
      userId: authStore.user.id,
      currentPassword: passwordForm.value.currentPassword,
      newPassword: passwordForm.value.newPassword
    }, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Clear password form
    passwordForm.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    }

    message.success('Đổi mật khẩu thành công!')
  } catch (error) {
    console.error('Error changing password:', error)
    if (error.response?.status === 400) {
      message.error('Mật khẩu hiện tại không đúng!')
    } else {
      message.error('Lỗi đổi mật khẩu!')
    }
  } finally {
    changingPassword.value = false
  }
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
      // Update avatarUrl in profileData and save
      profileData.value.avatarUrl = response.data.url
      
      // Build full update DTO
      const updateData = {
        studentCode: profileData.value.studentCode,
        faculty: profileData.value.faculty,
        major: profileData.value.major,
        academicYear: profileData.value.academicYear,
        classCode: profileData.value.classCode,
        fullName: profileData.value.fullName,
        gender: profileData.value.gender,
        dateOfBirth: profileData.value.dateOfBirth,
        placeOfBirth: profileData.value.placeOfBirth,
        ethnicity: profileData.value.ethnicity,
        religion: profileData.value.religion,
        nationality: profileData.value.nationality,
        bloodType: profileData.value.bloodType,
        healthInsuranceNumber: profileData.value.healthInsuranceNumber,
        phone: profileData.value.phone,
        email: profileData.value.email,
        identityCard: profileData.value.identityCard,
        identityCardIssuedDate: profileData.value.identityCardIssuedDate,
        identityCardIssuedPlace: profileData.value.identityCardIssuedPlace,
        permanentAddress: profileData.value.address,
        permanentProvince: profileData.value.permanentProvince,
        temporaryAddress: profileData.value.temporaryAddress,
        emergencyContactName: profileData.value.emergencyContactName,
        emergencyContactPhone: profileData.value.emergencyContactPhone,
        emergencyContactRelation: profileData.value.emergencyContactRelation,
        emergencyContactAddress: profileData.value.emergencyContactAddress,
        avatarUrl: response.data.url,
        isActive: profileData.value.isActive,
        notes: profileData.value.notes,
        userId: profileData.value.userId
      }
      
      await axios.put(`http://localhost:5001/api/students/${profileData.value.id}`, updateData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })

      authStore.user.avatarUrl = response.data.url
      localStorage.setItem('user', JSON.stringify(authStore.user))
      
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

const viewAvatar = () => {
  window.open(userAvatarUrl.value, '_blank')
}

const removeAvatar = async () => {
  try {
    // Build full update DTO with avatarUrl = null
    const updateData = {
      studentCode: profileData.value.studentCode,
      faculty: profileData.value.faculty,
      major: profileData.value.major,
      academicYear: profileData.value.academicYear,
      classCode: profileData.value.classCode,
      fullName: profileData.value.fullName,
      gender: profileData.value.gender,
      dateOfBirth: profileData.value.dateOfBirth,
      placeOfBirth: profileData.value.placeOfBirth,
      ethnicity: profileData.value.ethnicity,
      religion: profileData.value.religion,
      nationality: profileData.value.nationality,
      bloodType: profileData.value.bloodType,
      healthInsuranceNumber: profileData.value.healthInsuranceNumber,
      phone: profileData.value.phone,
      email: profileData.value.email,
      identityCard: profileData.value.identityCard,
      identityCardIssuedDate: profileData.value.identityCardIssuedDate,
      identityCardIssuedPlace: profileData.value.identityCardIssuedPlace,
      permanentAddress: profileData.value.address,
      permanentProvince: profileData.value.permanentProvince,
      temporaryAddress: profileData.value.temporaryAddress,
      emergencyContactName: profileData.value.emergencyContactName,
      emergencyContactPhone: profileData.value.emergencyContactPhone,
      emergencyContactRelation: profileData.value.emergencyContactRelation,
      emergencyContactAddress: profileData.value.emergencyContactAddress,
      avatarUrl: null,
      isActive: profileData.value.isActive,
      notes: profileData.value.notes,
      userId: profileData.value.userId
    }
    
    await axios.put(`http://localhost:5001/api/students/${profileData.value.id}`, updateData, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    profileData.value.avatarUrl = null
    authStore.user.avatarUrl = null
    localStorage.setItem('user', JSON.stringify(authStore.user))
    
    message.success('Đã xóa ảnh đại diện!')
  } catch (error) {
    console.error('Error removing avatar:', error)
    message.error('Lỗi xóa ảnh đại diện!')
  }
}

onMounted(() => {
  loadProfile()
})
</script>
