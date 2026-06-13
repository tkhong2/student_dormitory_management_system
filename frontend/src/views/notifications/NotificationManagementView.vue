<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Quản lý thông báo</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Gửi thông báo đến sinh viên trong hệ thống
        </p>
      </div>
      <a-button type="primary" @click="openCreateDialog" style="background: #ff9800; border-color: #ff9800;">
        <template #icon><PlusOutlined /></template>
        Tạo thông báo
      </a-button>
    </div>

    <!-- Notifications Table -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="notifications"
        :loading="loading"
        :pagination="{ pageSize: 10 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'title'">
            <div>
              <div style="font-weight: 600;">{{ record.title }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.body }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'user'">
            <div style="display: flex; align-items: center; gap: 8px;">
              <a-avatar size="small" style="background: #1890ff;">
                <template #icon><UserOutlined /></template>
              </a-avatar>
              <div>
                <div style="font-size: 13px;">{{ record.userName || 'Không xác định' }}</div>
                <div style="font-size: 12px; color: #8c8c8c;">
                  {{ record.studentCode ? `${record.studentCode} · ` : '' }}{{ record.email || `ID: ${record.userId}` }}
                </div>
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'type'">
            <a-tag :color="getTypeColor(record.type)">
              {{ formatType(record.type) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'isRead'">
            <a-tag :color="record.isRead ? 'success' : 'warning'">
              {{ record.isRead ? 'Đã đọc' : 'Chưa đọc' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'createdAt'">
            {{ formatDate(record.createdAt) }}
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-button 
              type="text" 
              danger 
              size="small"
              @click="deleteNotification(record.id)"
            >
              <template #icon><DeleteOutlined /></template>
            </a-button>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create Notification Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editMode ? 'Sửa thông báo' : 'Tạo thông báo mới'"
      width="600px"
      @cancel="dialog = false"
    >
      <a-form layout="vertical">
        <!-- Send to options -->
        <a-form-item label="Gửi đến" required>
          <a-radio-group v-model:value="form.sendType">
            <a-radio value="single">Một sinh viên cụ thể</a-radio>
            <a-radio value="multiple">Nhiều sinh viên</a-radio>
          </a-radio-group>
        </a-form-item>

        <!-- Single student select -->
        <a-form-item 
          v-if="form.sendType === 'single'"
          label="Chọn sinh viên"
          required
        >
          <a-select
            v-model:value="form.userId"
            show-search
            placeholder="Tìm và chọn sinh viên"
            :filter-option="filterStudentOption"
            :options="studentOptions"
          >
            <template #option="{ label, value, studentCode, email }">
              <div style="display: flex; align-items: center; gap: 8px;">
                <a-avatar size="small" style="background: #1890ff;">
                  <template #icon><UserOutlined /></template>
                </a-avatar>
                <div>
                  <div>{{ label }}</div>
                  <div style="font-size: 12px; color: #8c8c8c;">{{ studentCode }} · {{ email }}</div>
                </div>
              </div>
            </template>
          </a-select>
        </a-form-item>

        <!-- Multiple students select -->
        <a-form-item 
          v-if="form.sendType === 'multiple'"
          label="Chọn nhiều sinh viên"
          required
        >
          <a-select
            v-model:value="form.userIds"
            mode="multiple"
            show-search
            placeholder="Chọn nhiều sinh viên"
            :filter-option="filterStudentOption"
            :options="studentOptions"
          />
        </a-form-item>

        <a-form-item label="Tiêu đề" required>
          <a-input
            v-model:value="form.title"
            placeholder="Nhập tiêu đề thông báo"
          />
        </a-form-item>

        <a-form-item label="Nội dung" required>
          <a-textarea
            v-model:value="form.body"
            :rows="3"
            placeholder="Nhập nội dung thông báo"
          />
        </a-form-item>

        <a-form-item label="Loại thông báo">
          <a-select
            v-model:value="form.type"
            :options="notificationTypes"
          />
        </a-form-item>

        <a-form-item label="Biểu tượng">
          <a-select
            v-model:value="form.iconType"
            :options="iconTypes"
          />
        </a-form-item>

        <a-form-item label="Đường dẫn (tùy chọn)">
          <a-input
            v-model:value="form.actionUrl"
            placeholder="/student/my-contract"
          />
          <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
            Đường dẫn trang khi sinh viên click vào thông báo
          </div>
        </a-form-item>
      </a-form>

      <template #footer>
        <a-button @click="dialog = false">Hủy</a-button>
        <a-button type="primary" @click="submitForm" :loading="submitting">
          {{ editMode ? 'Cập nhật' : 'Gửi thông báo' }}
        </a-button>
      </template>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { 
  PlusOutlined, 
  UserOutlined, 
  DeleteOutlined 
} from '@ant-design/icons-vue'
import axios from 'axios'

const loading = ref(false)
const submitting = ref(false)
const dialog = ref(false)
const editMode = ref(false)

const notifications = ref([])
const students = ref([])

const form = ref({
  sendType: 'single',
  userId: null,
  userIds: [],
  title: '',
  body: '',
  type: 'System',
  iconType: 'info',
  actionUrl: ''
})

const columns = [
  { title: 'Thông báo', key: 'title', width: 300 },
  { title: 'Người nhận', key: 'user', width: 250 },
  { title: 'Loại', key: 'type', align: 'center', width: 150 },
  { title: 'Trạng thái', key: 'isRead', align: 'center', width: 120 },
  { title: 'Ngày tạo', key: 'createdAt', align: 'center', width: 150 },
  { title: '', key: 'actions', align: 'center', width: 80 }
]

const notificationTypes = [
  { label: 'Hệ thống', value: 'System' },
  { label: 'Đơn đăng ký được duyệt', value: 'ApplicationApproved' },
  { label: 'Hóa đơn mới', value: 'InvoiceCreated' },
  { label: 'Bảo trì hoàn tất', value: 'MaintenanceDone' },
  { label: 'Hợp đồng sắp hết hạn', value: 'ContractExpiring' },
  { label: 'Thanh toán thành công', value: 'PaymentReceived' },
  { label: 'Xếp phòng', value: 'RoomAssigned' }
]

const iconTypes = [
  { label: 'Thông tin', value: 'info' },
  { label: 'Thành công', value: 'success' },
  { label: 'Cảnh báo', value: 'warning' },
  { label: 'Lỗi', value: 'error' }
]

const studentOptions = computed(() => 
  students.value.map(s => ({
    label: s.fullName,
    value: s.id,
    studentCode: s.studentCode,
    email: s.email
  }))
)

const filterStudentOption = (input, option) => {
  return option.label.toLowerCase().includes(input.toLowerCase()) ||
         option.studentCode?.toLowerCase().includes(input.toLowerCase()) ||
         option.email?.toLowerCase().includes(input.toLowerCase())
}

// Fetch all notifications
const fetchNotifications = async () => {
  loading.value = true
  try {
    const response = await axios.get('http://localhost:5002/api/notifications', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    const notificationsData = response.data
    console.log('Notifications raw data:', notificationsData)
    
    // Load student information from ContractStudentService for each notification
    const notificationsWithStudentInfo = await Promise.all(
      notificationsData.map(async (notification) => {
        try {
          // Try to get student info from ContractStudentService
          const studentResponse = await axios.get(`http://localhost:5001/api/students/${notification.userId}`, {
            headers: {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
          })
          return {
            ...notification,
            userName: studentResponse.data.fullName,
            studentCode: studentResponse.data.studentCode,
            email: studentResponse.data.email
          }
        } catch (error) {
          // If student not found in ContractStudentService, try BillingMaintenanceService
          try {
            const userResponse = await axios.get(`http://localhost:5002/api/users/${notification.userId}`, {
              headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
              }
            })
            return {
              ...notification,
              userName: userResponse.data.fullName,
              email: userResponse.data.email
            }
          } catch {
            // If not found in either service, use fallback
            return {
              ...notification,
              userName: notification.userName || 'Không xác định',
              email: '-'
            }
          }
        }
      })
    )
    
    notifications.value = notificationsWithStudentInfo
    console.log('Notifications with student info:', notificationsWithStudentInfo)
  } catch (error) {
    console.error('Error fetching notifications:', error)
  } finally {
    loading.value = false
  }
}

// Fetch all students
const fetchStudents = async () => {
  try {
    const response = await axios.get('http://localhost:5001/api/students', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    students.value = response.data
    console.log('Students loaded:', students.value.length)
  } catch (error) {
    console.error('Error fetching students:', error)
    message.error('Không thể tải danh sách sinh viên')
  }
}

// Open create dialog
const openCreateDialog = () => {
  editMode.value = false
  form.value = {
    sendType: 'single',
    userId: null,
    userIds: [],
    title: '',
    body: '',
    type: 'System',
    iconType: 'info',
    actionUrl: ''
  }
  dialog.value = true
}

// Submit form
const submitForm = async () => {
  // Validation
  if (form.value.sendType === 'single' && !form.value.userId) {
    message.warning('Vui lòng chọn sinh viên')
    return
  }
  if (form.value.sendType === 'multiple' && (!form.value.userIds || form.value.userIds.length === 0)) {
    message.warning('Vui lòng chọn ít nhất một sinh viên')
    return
  }
  if (!form.value.title || !form.value.body) {
    message.warning('Vui lòng nhập đầy đủ tiêu đề và nội dung')
    return
  }

  submitting.value = true
  try {
    if (form.value.sendType === 'single') {
      // Send to single user
      await axios.post('http://localhost:5002/api/notifications', {
        userId: form.value.userId,
        title: form.value.title,
        body: form.value.body,
        type: form.value.type,
        iconType: form.value.iconType,
        actionUrl: form.value.actionUrl || null
      }, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })
      message.success('Đã gửi thông báo')
    } else {
      // Broadcast to multiple users
      await axios.post('http://localhost:5002/api/notifications/broadcast', {
        userIds: form.value.userIds,
        title: form.value.title,
        body: form.value.body,
        type: form.value.type,
        iconType: form.value.iconType,
        actionUrl: form.value.actionUrl || null
      }, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })
      message.success(`Đã gửi thông báo đến ${form.value.userIds.length} sinh viên`)
    }

    dialog.value = false
    await fetchNotifications()
  } catch (error) {
    console.error('Error creating notification:', error)
    message.error('Lỗi khi gửi thông báo')
  } finally {
    submitting.value = false
  }
}

// Delete notification
const deleteNotification = async (id) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: 'Bạn có chắc muốn xóa thông báo này?',
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await axios.delete(`http://localhost:5002/api/notifications/${id}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })
        message.success('Đã xóa thông báo')
        await fetchNotifications()
      } catch (error) {
        console.error('Error deleting notification:', error)
        message.error('Lỗi khi xóa thông báo')
      }
    }
  })
}

// Format type
const formatType = (type) => {
  const typeMap = {
    'System': 'Hệ thống',
    'ApplicationApproved': 'Đơn được duyệt',
    'InvoiceCreated': 'Hóa đơn mới',
    'MaintenanceDone': 'Bảo trì xong',
    'ContractExpiring': 'HĐ hết hạn',
    'PaymentReceived': 'Thanh toán',
    'RoomAssigned': 'Xếp phòng'
  }
  return typeMap[type] || type
}

// Get type color
const getTypeColor = (type) => {
  const colorMap = {
    'System': 'blue',
    'ApplicationApproved': 'green',
    'InvoiceCreated': 'orange',
    'MaintenanceDone': 'cyan',
    'ContractExpiring': 'red',
    'PaymentReceived': 'green',
    'RoomAssigned': 'green'
  }
  return colorMap[type] || 'default'
}

// Format date
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  fetchNotifications()
  fetchStudents()
})
</script>
