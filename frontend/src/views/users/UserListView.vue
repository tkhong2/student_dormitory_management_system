<template>
  <div>
    <!-- Page Header -->
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
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col v-for="r in roleStats" :key="r.role" :xs="24" :sm="8">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb;">
          <div style="display: flex; align-items: center; gap: 16px;">
            <a-avatar :size="48" :style="{ backgroundColor: r.bg }">
              <template #icon>
                <component :is="r.iconComponent" :style="{ fontSize: '24px', color: r.color }" />
              </template>
            </a-avatar>
            <div>
              <div style="font-size: 24px; font-weight: 700;">{{ r.count }}</div>
              <div style="font-size: 14px; color: #8c8c8c;">{{ r.role }}</div>
            </div>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px;" :bordered="false">
      <a-row :gutter="[16, 16]" align="middle">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm người dùng..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="16" style="text-align: right;">
          <a-radio-group v-model:value="filterRole" button-style="solid">
            <a-radio-button value="all">Tất cả</a-radio-button>
            <a-radio-button value="Admin">Admin</a-radio-button>
            <a-radio-button value="Staff">Nhân viên</a-radio-button>
            <a-radio-button value="Student">Sinh viên</a-radio-button>
          </a-radio-group>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false">
      <a-table
        :columns="antColumns"
        :data-source="filteredUsers"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} người dùng` }"
        :scroll="{ x: 1000 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'fullName'">
            <div style="display: flex; align-items: center; gap: 12px; padding: 8px 0;">
              <a-avatar :size="36" :style="{ backgroundColor: getAvatarBg(record.role) }">
                <template #icon>
                  <component :is="getRoleIconComponent(record.role)" />
                </template>
              </a-avatar>
              <div>
                <div style="font-weight: 600; font-size: 14px;">{{ record.fullName }}</div>
                <div style="font-size: 12px; color: #8c8c8c;">{{ record.email }}</div>
              </div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'role'">
            <a-tag :color="getAntRoleColor(record.role)">{{ roleDisplay(record.role) }}</a-tag>
          </template>
          
          <template v-else-if="column.key === 'status'">
            <a-badge :status="record.active ? 'success' : 'default'" :text="record.active ? 'Hoạt động' : 'Vô hiệu'" />
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Sửa">
                <a-button type="text" size="small" @click="openEditDialog(record)">
                  <template #icon><EditOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Reset mật khẩu">
                <a-button type="text" size="small" @click="resetPassword(record)">
                  <template #icon><KeyOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xóa">
                <a-button type="text" danger size="small" @click="confirmDelete(record)">
                  <template #icon><DeleteOutlined /></template>
                </a-button>
              </a-tooltip>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editMode ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới'"
      width="600px"
      @cancel="closeDialog"
    >
      <a-form layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item 
              label="Tên đăng nhập" 
              :validate-status="formErrors.username ? 'error' : ''"
              :help="formErrors.username"
            >
              <a-input 
                v-model:value="form.username" 
                placeholder="Nhập tên đăng nhập"
                :disabled="editMode"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Họ và tên"
              :validate-status="formErrors.fullName ? 'error' : ''"
              :help="formErrors.fullName"
            >
              <a-input 
                v-model:value="form.fullName" 
                placeholder="Nhập họ và tên"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Email"
              :validate-status="formErrors.email ? 'error' : ''"
              :help="formErrors.email"
            >
              <a-input 
                v-model:value="form.email" 
                placeholder="Nhập email"
                type="email"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Số điện thoại"
              :validate-status="formErrors.phone ? 'error' : ''"
              :help="formErrors.phone"
            >
              <a-input 
                v-model:value="form.phone" 
                placeholder="Nhập số điện thoại"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Vai trò"
              :validate-status="formErrors.role ? 'error' : ''"
              :help="formErrors.role"
            >
              <a-select 
                v-model:value="form.role" 
                placeholder="Chọn vai trò"
              >
                <a-select-option v-for="opt in roleOptions" :key="opt.value" :value="opt.value">
                  {{ opt.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12" v-if="form.role === 'Student'">
            <a-form-item label="Mã sinh viên">
              <a-input 
                v-model:value="form.studentCode" 
                placeholder="Nhập mã sinh viên"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12" v-if="!editMode">
            <a-form-item 
              label="Mật khẩu"
              :validate-status="formErrors.password ? 'error' : ''"
              :help="formErrors.password"
            >
              <a-input-password 
                v-model:value="form.password" 
                placeholder="Nhập mật khẩu"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12" v-if="!editMode">
            <a-form-item 
              label="Xác nhận mật khẩu"
              :validate-status="formErrors.confirmPassword ? 'error' : ''"
              :help="formErrors.confirmPassword"
            >
              <a-input-password 
                v-model:value="form.confirmPassword" 
                placeholder="Nhập lại mật khẩu"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
      <template #footer>
        <a-button @click="closeDialog" :disabled="saving">Hủy</a-button>
        <a-button type="primary" @click="saveUser" :loading="saving">
          {{ editMode ? 'Cập nhật' : 'Tạo tài khoản' }}
        </a-button>
      </template>
    </a-modal>

    <!-- Delete Confirmation Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      @ok="deleteUser"
      @cancel="deleteDialog = false"
      okText="Xóa"
      okType="danger"
      cancelText="Hủy"
      :confirmLoading="saving"
    >
      <p>Bạn có chắc muốn xóa người dùng <strong>{{ deleteTarget?.fullName }}</strong>?</p>
    </a-modal>

    <!-- Reset Password Modal -->
    <a-modal
      v-model:open="resetPasswordDialog"
      title="Reset mật khẩu"
      @ok="doResetPassword"
      @cancel="closeResetPasswordDialog"
      okText="Reset"
      cancelText="Hủy"
      :confirmLoading="saving"
    >
      <p style="margin-bottom: 16px;">Reset mật khẩu cho người dùng <strong>{{ resetTarget?.fullName }}</strong>?</p>
      <a-form-item 
        label="Mật khẩu mới"
        :validate-status="passwordError ? 'error' : ''"
        :help="passwordError"
      >
        <a-input-password 
          v-model:value="newPassword" 
          placeholder="Nhập mật khẩu mới"
        />
      </a-form-item>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { 
  PlusOutlined, 
  SearchOutlined, 
  EditOutlined, 
  DeleteOutlined,
  KeyOutlined,
  CrownOutlined,
  SafetyOutlined,
  UserOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
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
    { role:'Quản trị viên', count: adminCount, iconComponent: CrownOutlined, color:'#ef4444', bg:'#fef2f2' },
    { role:'Nhân viên', count: staffCount, iconComponent: SafetyOutlined, color:'#3b82f6', bg:'#e0f2fe' },
    { role:'Sinh viên', count: studentCount, iconComponent: UserOutlined, color:'#16a34a', bg:'#dcfce7' },
  ]
})

const antColumns = [
  { title: 'Người dùng', key: 'fullName', width: 280 },
  { title: 'Username', dataIndex: 'username', key: 'username', width: 150, align: 'center' },
  { title: 'Vai trò', key: 'role', width: 120, align: 'center' },
  { title: 'Trạng thái', key: 'status', width: 130, align: 'center' },
  { title: 'Ngày tạo', dataIndex: 'created', key: 'created', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 150, align: 'center', fixed: 'right' }
]

const users = ref([])

const getAntRoleColor = (role) => {
  return { 'Admin': 'red', 'Staff': 'blue', 'Student': 'green' }[role] || 'default'
}

const getAvatarBg = (role) => {
  return { 'Admin': '#fef2f2', 'Staff': '#e0f2fe', 'Student': '#dcfce7' }[role] || '#f5f5f5'
}

const getRoleIconComponent = (role) => {
  return { 'Admin': CrownOutlined, 'Staff': SafetyOutlined, 'Student': UserOutlined }[role] || UserOutlined
}

async function loadUsers() {
  loading.value = true
  try {
    const response = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    users.value = response.data.map(user => ({
      id: user.id,
      username: user.username,
      fullName: user.fullName,
      email: user.email,
      phone: user.phone,
      role: user.role,
      active: user.isActive,
      created: user.createdAt ? new Date(user.createdAt).toLocaleDateString('vi-VN') : 'N/A'
    }))
  } catch (error) {
    console.error('Error loading users:', error)
    message.error(error.response?.data?.message || 'Không thể tải danh sách người dùng')
  } finally {
    loading.value = false
  }
}

const filteredUsers = computed(() => {
  let result = users.value
  if (filterRole.value !== 'all') {
    result = result.filter(u => u.role === filterRole.value)
  }
  if (search.value) {
    const keyword = search.value.toLowerCase()
    result = result.filter(u => 
      u.fullName?.toLowerCase().includes(keyword) ||
      u.username?.toLowerCase().includes(keyword) ||
      u.email?.toLowerCase().includes(keyword)
    )
  }
  return result
})

const roleDisplay = r => ({'Admin':'Admin','Staff':'Nhân viên','Student':'Sinh viên'}[r]||r)

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
      message.success('Cập nhật người dùng thành công')
    } else {
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
      message.success('Thêm người dùng thành công')
    }
    
    closeDialog()
    await loadUsers()
  } catch (error) {
    console.error('Error saving user:', error)
    const errorMsg = error.response?.data?.message || error.response?.data || 'Có lỗi xảy ra'
    message.error(errorMsg)
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
    message.success('Xóa người dùng thành công')
    deleteDialog.value = false
    await loadUsers()
  } catch (error) {
    console.error('Error deleting user:', error)
    message.error(error.response?.data?.message || 'Không thể xóa người dùng')
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
    message.success('Reset mật khẩu thành công')
    closeResetPasswordDialog()
  } catch (error) {
    console.error('Error resetting password:', error)
    message.error(error.response?.data?.message || 'Không thể reset mật khẩu')
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
