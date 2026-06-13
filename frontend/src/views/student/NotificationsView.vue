<template>
  <div>
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 24px;">
      <div>
        <h1 style="font-size: 28px; font-weight: bold; margin-bottom: 4px;">Thông báo</h1>
        <p style="font-size: 14px; color: rgba(0,0,0,0.45);">
          Xem các thông báo mới nhất từ hệ thống
        </p>
      </div>
      <a-button 
        v-if="unreadCount > 0" 
        type="primary"
        @click="markAllAsRead"
        :loading="markingAllRead"
      >
        <template #icon><check-outlined /></template>
        Đánh dấu tất cả đã đọc
      </a-button>
    </div>

    <!-- Loading -->
    <div v-if="loading" style="text-align: center; padding: 48px;">
      <a-spin size="large" />
    </div>

    <!-- Notifications List -->
    <a-card v-else-if="notifications.length > 0" :bordered="true" style="border-radius: 8px;">
      <a-list :data-source="notifications" item-layout="horizontal">
        <template #renderItem="{ item: notification, index }">
          <a-list-item 
            :style="{ 
              backgroundColor: !notification.isRead ? '#e6f4ff' : 'transparent',
              cursor: 'pointer',
              padding: '16px'
            }"
            @click="handleNotificationClick(notification)"
          >
            <template #actions>
              <a-dropdown>
                <a-button type="text" size="small">
                  <template #icon><more-outlined /></template>
                </a-button>
                <template #overlay>
                  <a-menu>
                    <a-menu-item 
                      v-if="!notification.isRead"
                      @click="markAsRead(notification.id)"
                    >
                      <check-outlined style="margin-right: 8px;" />
                      Đánh dấu đã đọc
                    </a-menu-item>
                    <a-menu-item 
                      danger
                      @click="deleteNotification(notification.id)"
                    >
                      <delete-outlined style="margin-right: 8px;" />
                      Xóa
                    </a-menu-item>
                  </a-menu>
                </template>
              </a-dropdown>
            </template>

            <a-list-item-meta>
              <template #avatar>
                <a-avatar :size="48" :style="{ backgroundColor: getIconColor(notification.iconType) }">
                  <template #icon>
                    <component :is="getIconComponent(notification.type)" />
                  </template>
                </a-avatar>
              </template>

              <template #title>
                <span style="font-weight: bold;">
                  {{ notification.title }}
                  <a-tag 
                    v-if="!notification.isRead" 
                    color="blue"
                    style="margin-left: 8px;"
                  >
                    Mới
                  </a-tag>
                </span>
              </template>

              <template #description>
                <div style="font-size: 14px; margin-bottom: 4px;">
                  {{ notification.body }}
                </div>
                <div style="font-size: 12px; color: rgba(0,0,0,0.45);">
                  <clock-circle-outlined style="margin-right: 4px;" />
                  {{ formatTime(notification.createdAt) }}
                </div>
              </template>
            </a-list-item-meta>
          </a-list-item>
        </template>
      </a-list>
    </a-card>

    <!-- Empty State -->
    <a-card v-else :bordered="true" style="border-radius: 8px;">
      <a-empty description="Không có thông báo">
        <template #image>
          <bell-outlined style="font-size: 80px; color: #d9d9d9;" />
        </template>
        <template #description>
          <h3 style="font-size: 18px; font-weight: bold; margin-bottom: 8px;">Không có thông báo</h3>
          <p style="font-size: 14px; color: rgba(0,0,0,0.45);">
            Bạn chưa có thông báo nào. Các thông báo mới sẽ xuất hiện tại đây.
          </p>
        </template>
      </a-empty>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, h } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { 
  CheckOutlined,
  MoreOutlined,
  DeleteOutlined,
  ClockCircleOutlined,
  BellOutlined,
  InfoCircleOutlined,
  CheckCircleOutlined,
  FileSearchOutlined,
  ToolOutlined,
  CalendarOutlined,
  DollarOutlined,
  HomeOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'

const authStore = useAuthStore()
const router = useRouter()

const loading = ref(false)
const markingAllRead = ref(false)
const notifications = ref([])

const currentUser = computed(() => authStore.user)
const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

// Fetch notifications
const fetchNotifications = async () => {
  loading.value = true
  try {
    const userId = currentUser.value.id
    console.log('=== FETCH NOTIFICATIONS (Full Page) ===')
    console.log('User ID:', userId)
    
    if (!userId) return

    // Get student ID from contract (since notifications use studentId, not userId)
    const contractResponse = await axios.get(`http://localhost:5001/api/contracts/user/${userId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    console.log('Contract response:', contractResponse.data)

    const activeContract = contractResponse.data.find(c => c.status === 'Active')
    console.log('Active contract:', activeContract)
    
    if (!activeContract) {
      console.log('No active contract found')
      return
    }

    const studentId = activeContract.studentId
    console.log('Fetching notifications for studentId:', studentId)

    const response = await axios.get(`http://localhost:5002/api/notifications/user/${studentId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    console.log('Notifications response:', response.data)
    notifications.value = response.data
  } catch (error) {
    console.error('Error fetching notifications:', error)
  } finally {
    loading.value = false
  }
}

// Mark as read
const markAsRead = async (notificationId) => {
  try {
    await axios.put(`http://localhost:5002/api/notifications/${notificationId}/read`, {}, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Update local state
    const notification = notifications.value.find(n => n.id === notificationId)
    if (notification) {
      notification.isRead = true
      notification.readAt = new Date().toISOString()
    }
  } catch (error) {
    console.error('Error marking as read:', error)
  }
}

// Mark all as read
const markAllAsRead = async () => {
  markingAllRead.value = true
  try {
    // Get student ID from contract
    const userId = currentUser.value.id
    const contractResponse = await axios.get(`http://localhost:5001/api/contracts/user/${userId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    const activeContract = contractResponse.data.find(c => c.status === 'Active')
    if (!activeContract) return

    const studentId = activeContract.studentId

    await axios.put(`http://localhost:5002/api/notifications/user/${studentId}/read-all`, {}, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Update local state
    notifications.value.forEach(n => {
      n.isRead = true
      n.readAt = new Date().toISOString()
    })

    message.success('Đã đánh dấu tất cả thông báo là đã đọc')
  } catch (error) {
    console.error('Error marking all as read:', error)
    message.error('Lỗi khi đánh dấu thông báo')
  } finally {
    markingAllRead.value = false
  }
}

// Delete notification
const deleteNotification = (notificationId) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: 'Bạn có chắc muốn xóa thông báo này?',
    okText: 'Xóa',
    cancelText: 'Hủy',
    okType: 'danger',
    async onOk() {
      try {
        await axios.delete(`http://localhost:5002/api/notifications/${notificationId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })

        // Remove from local state
        notifications.value = notifications.value.filter(n => n.id !== notificationId)
        message.success('Đã xóa thông báo')
      } catch (error) {
        console.error('Error deleting notification:', error)
        message.error('Lỗi khi xóa thông báo')
      }
    }
  })
}

// Handle notification click
const handleNotificationClick = async (notification) => {
  // Mark as read
  if (!notification.isRead) {
    await markAsRead(notification.id)
  }

  // Navigate to action URL if exists
  if (notification.actionUrl) {
    router.push(notification.actionUrl)
  }
}

// Get icon component based on type
const getIconComponent = (type) => {
  const iconMap = {
    'System': InfoCircleOutlined,
    'ApplicationApproved': CheckCircleOutlined,
    'InvoiceCreated': FileSearchOutlined,
    'MaintenanceDone': ToolOutlined,
    'ContractExpiring': CalendarOutlined,
    'PaymentReceived': DollarOutlined,
    'RoomAssigned': HomeOutlined
  }
  return iconMap[type] || BellOutlined
}

// Get icon color based on icon type
const getIconColor = (iconType) => {
  const colorMap = {
    'success': '#52c41a',
    'warning': '#faad14',
    'error': '#ff4d4f',
    'info': '#1890ff'
  }
  return colorMap[iconType] || '#1890ff'
}

// Format time
const formatTime = (dateString) => {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now - date
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMins < 1) return 'Vừa xong'
  if (diffMins < 60) return `${diffMins} phút trước`
  if (diffHours < 24) return `${diffHours} giờ trước`
  if (diffDays === 1) return 'Hôm qua'
  if (diffDays < 7) return `${diffDays} ngày trước`
  
  return date.toLocaleString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  fetchNotifications()
})
</script>
