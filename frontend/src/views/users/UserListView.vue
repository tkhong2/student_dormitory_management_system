<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">Quản lý Người dùng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Tổng số: {{ users.length }} người dùng</p>
      </div>
      <a-button type="primary" @click="openCreateDialog" style="background: #ff9800; border-color: #ff9800;">
        + Thêm người dùng
      </a-button>
    </div>

    <!-- Role stats -->
    <v-row style="margin-bottom: 16px;">
      <v-col v-for="r in roleStats" :key="r.role" cols="12" sm="4">
        <v-card class="pa-5 d-flex align-center ga-4" style="border:1px solid #e5e7eb">
          <v-avatar :color="r.bg" size="48" rounded="lg">
            <v-icon :color="r.color" size="24">{{ r.icon }}</v-icon>
          </v-avatar>
          <div>
            <div class="text-h5 font-weight-bold">{{ r.count }}</div>
            <div class="text-body-2 text-medium-emphasis">{{ r.role }}</div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-card style="border:1px solid #e5e7eb">
      <div class="pa-4 d-flex flex-wrap ga-3 align-center">
        <v-text-field v-model="search" placeholder="Tìm người dùng..." prepend-inner-icon="mdi-magnify" density="compact" hide-details style="max-width:300px" />
        <v-chip-group v-model="filterRole" class="ml-auto">
          <v-chip filter value="all">Tất cả</v-chip>
          <v-chip filter value="Admin" color="error">Admin</v-chip>
          <v-chip filter value="Staff" color="info">Nhân viên</v-chip>
          <v-chip filter value="Student" color="success">Sinh viên</v-chip>
        </v-chip-group>
      </div>

      <v-data-table :headers="headers" :items="filteredUsers" :search="search" items-per-page="10" :loading="loading">
        <template #item.fullName="{ item }">
          <div class="d-flex align-center ga-3 py-2">
            <v-avatar size="36" :color="roleColor(item.role)" variant="tonal">
              <v-icon size="18">{{ roleIcon(item.role) }}</v-icon>
            </v-avatar>
            <div>
              <div class="text-body-2 font-weight-bold">{{ item.fullName }}</div>
              <div class="text-caption text-medium-emphasis">{{ item.email }}</div>
            </div>
          </div>
        </template>
        <template #item.role="{ item }">
          <v-chip :color="roleColor(item.role)" size="x-small" variant="tonal">{{ roleDisplay(item.role) }}</v-chip>
        </template>
        <template #item.status="{ item }">
          <div class="d-flex align-center ga-1">
            <span class="dot-pulse" :style="{background:item.active?'#16a34a':'#94a3b8'}" />
            <span class="text-body-2">{{ item.active ? 'Hoạt động' : 'Vô hiệu' }}</span>
          </div>
        </template>
        <template #item.actions="{ item }">
          <v-btn icon="mdi-pencil-outline" size="small" variant="text" @click="openEditDialog(item)" />
          <v-btn icon="mdi-lock-reset" size="small" variant="text" color="warning" title="Reset mật khẩu" @click="resetPassword(item)" />
          <v-btn icon="mdi-delete-outline" size="small" variant="text" color="error" @click="confirmDelete(item)" />
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialog" max-width="560" persistent>
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">{{ editMode ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới' }}</h2>
        <v-row>
          <v-col cols="6">
            <v-text-field 
              v-model="form.username" 
              label="Tên đăng nhập" 
              :disabled="editMode"
              :error-messages="formErrors.username"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field 
              v-model="form.fullName" 
              label="Họ và tên"
              :error-messages="formErrors.fullName"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field 
              v-model="form.email" 
              label="Email" 
              type="email"
              :error-messages="formErrors.email"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field 
              v-model="form.phone" 
              label="Số điện thoại"
              :error-messages="formErrors.phone"
            />
          </v-col>
          <v-col cols="6">
            <v-select 
              v-model="form.role" 
              label="Vai trò" 
              :items="roleOptions"
              item-title="label"
              item-value="value"
              :error-messages="formErrors.role"
            />
          </v-col>
          <v-col cols="6" v-if="form.role === 'Student'">
            <v-text-field 
              v-model="form.studentCode" 
              label="Mã sinh viên"
            />
          </v-col>
          <v-col cols="6" v-if="!editMode">
            <v-text-field 
              v-model="form.password" 
              label="Mật khẩu" 
              type="password"
              :error-messages="formErrors.password"
            />
          </v-col>
          <v-col cols="6" v-if="!editMode">
            <v-text-field 
              v-model="form.confirmPassword" 
              label="Xác nhận mật khẩu" 
              type="password"
              :error-messages="formErrors.confirmPassword"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="closeDialog" :disabled="saving">Hủy</v-btn>
          <v-btn color="primary" @click="saveUser" :loading="saving">
            {{ editMode ? 'Cập nhật' : 'Tạo tài khoản' }}
          </v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-3">Xác nhận xóa</h2>
        <p>Bạn có chắc muốn xóa người dùng <strong>{{ deleteTarget?.fullName }}</strong>?</p>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" @click="deleteUser" :loading="saving">Xóa</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Reset Password Dialog -->
    <v-dialog v-model="resetPasswordDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Reset mật khẩu</h2>
        <p class="mb-4">Reset mật khẩu cho người dùng <strong>{{ resetTarget?.fullName }}</strong>?</p>
        <v-text-field 
          v-model="newPassword" 
          label="Mật khẩu mới" 
          type="password"
          :error-messages="passwordError"
        />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="closeResetPasswordDialog">Hủy</v-btn>
          <v-btn color="warning" @click="doResetPassword" :loading="saving" style="color: white;">Reset</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Snackbar for notifications -->
    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import axios from 'axios'

const search = ref('')
const filterRole = ref('all')
const dialog = ref(false)
const deleteDialog = ref(false)
const resetPasswordDialog = ref(false)
const loading = ref(false)
const saving = ref(false)
const editMode = ref(false)
const deleteTarget = ref(null)
const resetTarget = ref(null)
const newPassword = ref('')
const passwordError = ref('')

const form = ref({
  username: '',
  password: '',
  confirmPassword: '',
  fullName: '',
  email: '',
  phone: '',
  role: 'Student',
  studentCode: ''
})

const formErrors = ref({})

const snackbar = ref({
  show: false,
  message: '',
  color: 'success'
})

const roleOptions = [
  { label: 'Admin', value: 'Admin' },
  { label: 'Nhân viên', value: 'Staff' },
  { label: 'Sinh viên', value: 'Student' }
]

const roleStats = computed(() => {
  const adminCount = users.value.filter(u => u.role === 'Admin').length
  const staffCount = users.value.filter(u => u.role === 'Staff').length
  const studentCount = users.value.filter(u => u.role === 'Student').length
  
  return [
    { role:'Quản trị viên', count: adminCount, icon:'mdi-shield-crown', color:'error', bg:'#fef2f2' },
    { role:'Nhân viên', count: staffCount, icon:'mdi-account-hard-hat', color:'info', bg:'#e0f2fe' },
    { role:'Sinh viên', count: studentCount, icon:'mdi-school', color:'success', bg:'#dcfce7' },
  ]
})

const headers = [
  { title:'Người dùng', key:'fullName' },
  { title:'Username', key:'username', align:'center' },
  { title:'Vai trò', key:'role', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'Ngày tạo', key:'created', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]

const users = ref([])

// Load users from API
async function loadUsers() {
  loading.value = true
  try {
    // Call directly to BillingMaintenanceService since Gateway might not be running
    const response = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    // Map API response to component format
    users.value = response.data.map(user => ({
      id: user.id,
      username: user.username,
      fullName: user.fullName,
      email: user.email,
      role: user.role,
      active: user.isActive,
      created: user.createdAt ? new Date(user.createdAt).toLocaleDateString('vi-VN') : 'N/A'
    }))
    
    console.log('Loaded users:', users.value)
  } catch (error) {
    console.error('Error loading users:', error)
    console.error('Error details:', error.response?.data || error.message)
  } finally {
    loading.value = false
  }
}

const filteredUsers = computed(() => {
  if (filterRole.value === 'all') return users.value
  return users.value.filter(u => u.role === filterRole.value)
})

const roleColor = r => ({'Admin':'error','Staff':'info','Student':'success'}[r]||'grey')
const roleIcon = r => ({'Admin':'mdi-shield-crown','Staff':'mdi-account-hard-hat','Student':'mdi-school'}[r]||'mdi-account')

// Map role names for display
const roleDisplay = r => ({'Admin':'Admin','Staff':'Nhân viên','Student':'Sinh viên'}[r]||r)

// Dialog functions
function openCreateDialog() {
  editMode.value = false
  form.value = {
    username: '',
    password: '',
    confirmPassword: '',
    fullName: '',
    email: '',
    phone: '',
    role: 'Student',
    studentCode: ''
  }
  formErrors.value = {}
  dialog.value = true
}

function openEditDialog(user) {
  editMode.value = true
  form.value = {
    id: user.id,
    username: user.username,
    fullName: user.fullName,
    email: user.email,
    phone: user.phone || '',
    role: user.role,
    studentCode: user.studentCode || '',
    password: '',
    confirmPassword: ''
  }
  formErrors.value = {}
  dialog.value = true
}

function closeDialog() {
  dialog.value = false
  editMode.value = false
  formErrors.value = {}
}

function validateForm() {
  const errors = {}
  
  if (!form.value.username.trim()) {
    errors.username = 'Vui lòng nhập tên đăng nhập'
  }
  
  if (!form.value.fullName.trim()) {
    errors.fullName = 'Vui lòng nhập họ tên'
  }
  
  if (!form.value.email.trim()) {
    errors.email = 'Vui lòng nhập email'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
    errors.email = 'Email không hợp lệ'
  }
  
  if (!form.value.phone.trim()) {
    errors.phone = 'Vui lòng nhập số điện thoại'
  }
  
  if (!form.value.role) {
    errors.role = 'Vui lòng chọn vai trò'
  }
  
  // Password validation for create mode only
  if (!editMode.value) {
    if (!form.value.password) {
      errors.password = 'Vui lòng nhập mật khẩu'
    } else if (form.value.password.length < 6) {
      errors.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    }
    
    if (form.value.password !== form.value.confirmPassword) {
      errors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    }
  }
  
  formErrors.value = errors
  return Object.keys(errors).length === 0
}

async function saveUser() {
  if (!validateForm()) return
  
  saving.value = true
  try {
    if (editMode.value) {
      // Update user
      await axios.put(`http://localhost:5002/api/users/${form.value.id}`, {
        fullName: form.value.fullName,
        email: form.value.email,
        phone: form.value.phone,
        role: form.value.role,
        studentCode: form.value.studentCode,
        isActive: true
      }, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })
      showNotification('Cập nhật người dùng thành công', 'success')
    } else {
      // Create user
      await axios.post('http://localhost:5002/api/users', {
        username: form.value.username,
        password: form.value.password,
        fullName: form.value.fullName,
        email: form.value.email,
        phone: form.value.phone,
        role: form.value.role,
        studentCode: form.value.role === 'Student' ? form.value.studentCode : null
      }, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })
      showNotification('Thêm người dùng thành công', 'success')
    }
    
    closeDialog()
    await loadUsers()
  } catch (error) {
    console.error('Error saving user:', error)
    const errorMsg = error.response?.data?.message || error.response?.data || 'Có lỗi xảy ra'
    showNotification(errorMsg, 'error')
  } finally {
    saving.value = false
  }
}

function confirmDelete(user) {
  deleteTarget.value = user
  deleteDialog.value = true
}

async function deleteUser() {
  saving.value = true
  try {
    await axios.delete(`http://localhost:5002/api/users/${deleteTarget.value.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    showNotification('Xóa người dùng thành công', 'success')
    deleteDialog.value = false
    await loadUsers()
  } catch (error) {
    console.error('Error deleting user:', error)
    showNotification(error.response?.data?.message || 'Không thể xóa người dùng', 'error')
  } finally {
    saving.value = false
  }
}

function resetPassword(user) {
  resetTarget.value = user
  newPassword.value = ''
  passwordError.value = ''
  resetPasswordDialog.value = true
}

function closeResetPasswordDialog() {
  resetPasswordDialog.value = false
  resetTarget.value = null
  newPassword.value = ''
  passwordError.value = ''
}

async function doResetPassword() {
  if (!newPassword.value) {
    passwordError.value = 'Vui lòng nhập mật khẩu mới'
    return
  }
  
  if (newPassword.value.length < 6) {
    passwordError.value = 'Mật khẩu phải có ít nhất 6 ký tự'
    return
  }
  
  saving.value = true
  try {
    await axios.put(
      `http://localhost:5002/api/users/${resetTarget.value.id}/password`,
      `"${newPassword.value}"`,
      {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      }
    )
    showNotification('Reset mật khẩu thành công', 'success')
    closeResetPasswordDialog()
  } catch (error) {
    console.error('Error resetting password:', error)
    showNotification(error.response?.data?.message || 'Không thể reset mật khẩu', 'error')
  } finally {
    saving.value = false
  }
}

function showNotification(message, color = 'success') {
  snackbar.value = {
    show: true,
    message,
    color
  }
}

onMounted(() => {
  loadUsers()
})
</script>
