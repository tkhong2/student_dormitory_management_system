<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Quản lý thông báo</h1>
        <p class="text-body-2 text-medium-emphasis">
          Gửi thông báo đến sinh viên trong hệ thống
        </p>
      </div>
      <v-btn color="primary" @click="openCreateDialog">
        <v-icon start>mdi-plus</v-icon>
        Tạo thông báo
      </v-btn>
    </div>

    <!-- Notifications List -->
    <v-card class="rounded-lg">
      <v-data-table
        :headers="headers"
        :items="notifications"
        :loading="loading"
        items-per-page="10"
      >
        <template v-slot:item.title="{ item }">
          <div class="font-weight-bold">{{ item.title }}</div>
          <div class="text-caption text-medium-emphasis">{{ item.body }}</div>
        </template>

        <template v-slot:item.user="{ item }">
          <div class="d-flex align-center ga-2">
            <v-avatar size="32" color="primary" variant="tonal">
              <v-icon size="16">mdi-account</v-icon>
            </v-avatar>
            <div>
              <div class="text-body-2">{{ item.userName }}</div>
              <div class="text-caption text-medium-emphasis">ID: {{ item.userId }}</div>
            </div>
          </div>
        </template>

        <template v-slot:item.type="{ item }">
          <v-chip size="small" :color="getTypeColor(item.type)" variant="tonal">
            {{ formatType(item.type) }}
          </v-chip>
        </template>

        <template v-slot:item.isRead="{ item }">
          <v-chip 
            size="small" 
            :color="item.isRead ? 'success' : 'warning'" 
            variant="flat"
          >
            {{ item.isRead ? 'Đã đọc' : 'Chưa đọc' }}
          </v-chip>
        </template>

        <template v-slot:item.createdAt="{ item }">
          {{ formatDate(item.createdAt) }}
        </template>

        <template v-slot:item.actions="{ item }">
          <v-btn 
            icon="mdi-delete" 
            variant="text" 
            size="small" 
            color="error"
            @click="deleteNotification(item.id)"
          />
        </template>
      </v-data-table>
    </v-card>

    <!-- Create Notification Dialog -->
    <v-dialog v-model="dialog" max-width="600px">
      <v-card>
        <v-card-title class="text-h5 font-weight-bold pa-6">
          {{ editMode ? 'Sửa thông báo' : 'Tạo thông báo mới' }}
        </v-card-title>

        <v-divider />

        <v-card-text class="pa-6">
          <v-form ref="formRef">
            <!-- Send to options -->
            <v-radio-group v-model="form.sendType" class="mb-4">
              <template v-slot:label>
                <div class="text-body-2 font-weight-bold mb-2">Gửi đến</div>
              </template>
              <v-radio label="Một sinh viên cụ thể" value="single" />
              <v-radio label="Nhiều sinh viên" value="multiple" />
            </v-radio-group>

            <!-- Single student select -->
            <v-autocomplete
              v-if="form.sendType === 'single'"
              v-model="form.userId"
              :items="students"
              item-title="fullName"
              item-value="id"
              label="Chọn sinh viên"
              prepend-inner-icon="mdi-account-search"
              :rules="[v => !!v || 'Vui lòng chọn sinh viên']"
              class="mb-4"
            >
              <template v-slot:item="{ props, item }">
                <v-list-item v-bind="props">
                  <template v-slot:prepend>
                    <v-avatar color="primary" variant="tonal">
                      <v-icon size="20">mdi-account</v-icon>
                    </v-avatar>
                  </template>
                  <template v-slot:subtitle>
                    {{ item.raw.studentCode }} · {{ item.raw.email }}
                  </template>
                </v-list-item>
              </template>
            </v-autocomplete>

            <!-- Multiple students select -->
            <v-autocomplete
              v-if="form.sendType === 'multiple'"
              v-model="form.userIds"
              :items="students"
              item-title="fullName"
              item-value="id"
              label="Chọn nhiều sinh viên"
              prepend-inner-icon="mdi-account-multiple"
              multiple
              chips
              closable-chips
              :rules="[v => v && v.length > 0 || 'Vui lòng chọn ít nhất một sinh viên']"
              class="mb-4"
            />

            <v-text-field
              v-model="form.title"
              label="Tiêu đề"
              prepend-inner-icon="mdi-format-title"
              :rules="[v => !!v || 'Vui lòng nhập tiêu đề']"
              class="mb-4"
            />

            <v-textarea
              v-model="form.body"
              label="Nội dung"
              prepend-inner-icon="mdi-text"
              rows="3"
              :rules="[v => !!v || 'Vui lòng nhập nội dung']"
              class="mb-4"
            />

            <v-select
              v-model="form.type"
              :items="notificationTypes"
              item-title="label"
              item-value="value"
              label="Loại thông báo"
              prepend-inner-icon="mdi-tag"
              class="mb-4"
            />

            <v-select
              v-model="form.iconType"
              :items="iconTypes"
              item-title="label"
              item-value="value"
              label="Biểu tượng"
              prepend-inner-icon="mdi-emoticon"
              class="mb-4"
            />

            <v-text-field
              v-model="form.actionUrl"
              label="Đường dẫn (tùy chọn)"
              prepend-inner-icon="mdi-link"
              placeholder="/student/my-contract"
              hint="Đường dẫn trang khi sinh viên click vào thông báo"
              persistent-hint
            />
          </v-form>
        </v-card-text>

        <v-divider />

        <v-card-actions class="pa-6">
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="submitForm" :loading="submitting">
            {{ editMode ? 'Cập nhật' : 'Gửi thông báo' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const loading = ref(false)
const submitting = ref(false)
const dialog = ref(false)
const editMode = ref(false)
const formRef = ref(null)

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

const headers = [
  { title: 'Thông báo', key: 'title', sortable: false },
  { title: 'Người nhận', key: 'user', sortable: false },
  { title: 'Loại', key: 'type' },
  { title: 'Trạng thái', key: 'isRead' },
  { title: 'Ngày tạo', key: 'createdAt' },
  { title: '', key: 'actions', sortable: false, align: 'end' }
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

// Fetch all notifications
const fetchNotifications = async () => {
  loading.value = true
  try {
    const response = await axios.get('http://localhost:5002/api/notifications', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    notifications.value = response.data
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
  } catch (error) {
    console.error('Error fetching students:', error)
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
  const { valid } = await formRef.value.validate()
  if (!valid) return

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
    }

    dialog.value = false
    await fetchNotifications()
  } catch (error) {
    console.error('Error creating notification:', error)
    alert('Lỗi khi gửi thông báo')
  } finally {
    submitting.value = false
  }
}

// Delete notification
const deleteNotification = async (id) => {
  if (!confirm('Bạn có chắc muốn xóa thông báo này?')) return

  try {
    await axios.delete(`http://localhost:5002/api/notifications/${id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    await fetchNotifications()
  } catch (error) {
    console.error('Error deleting notification:', error)
    alert('Lỗi khi xóa thông báo')
  }
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
    'System': 'primary',
    'ApplicationApproved': 'success',
    'InvoiceCreated': 'warning',
    'MaintenanceDone': 'info',
    'ContractExpiring': 'error',
    'PaymentReceived': 'success',
    'RoomAssigned': 'success'
  }
  return colorMap[type] || 'grey'
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
