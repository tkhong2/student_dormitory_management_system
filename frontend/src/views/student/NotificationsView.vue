<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Thông báo</h1>
        <p class="text-body-2 text-medium-emphasis">
          Xem các thông báo mới nhất từ hệ thống
        </p>
      </div>
      <v-btn 
        v-if="unreadCount > 0" 
        color="primary" 
        variant="tonal"
        @click="markAllAsRead"
        :loading="markingAllRead"
      >
        <v-icon start>mdi-check-all</v-icon>
        Đánh dấu tất cả đã đọc
      </v-btn>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular indeterminate color="primary" />
    </div>

    <!-- Notifications List -->
    <v-card v-else-if="notifications.length > 0" class="rounded-lg">
      <v-list lines="three">
        <template v-for="(notification, index) in notifications" :key="notification.id">
          <v-list-item 
            :class="{ 'bg-blue-50': !notification.isRead }"
            @click="handleNotificationClick(notification)"
            class="notification-item"
          >
            <template v-slot:prepend>
              <v-avatar :color="getIconColor(notification.iconType)" variant="tonal" size="48">
                <v-icon :color="getIconColor(notification.iconType)">{{ getIcon(notification.type) }}</v-icon>
              </v-avatar>
            </template>

            <v-list-item-title class="font-weight-bold mb-1">
              {{ notification.title }}
              <v-chip 
                v-if="!notification.isRead" 
                size="x-small" 
                color="primary" 
                variant="flat"
                class="ml-2"
              >
                Mới
              </v-chip>
            </v-list-item-title>

            <v-list-item-subtitle class="text-body-2 mb-1">
              {{ notification.body }}
            </v-list-item-subtitle>

            <v-list-item-subtitle class="text-caption text-medium-emphasis">
              <v-icon size="14" class="mr-1">mdi-clock-outline</v-icon>
              {{ formatTime(notification.createdAt) }}
            </v-list-item-subtitle>

            <template v-slot:append>
              <v-menu>
                <template v-slot:activator="{ props }">
                  <v-btn icon="mdi-dots-vertical" variant="text" v-bind="props" size="small" />
                </template>
                <v-list density="compact">
                  <v-list-item 
                    v-if="!notification.isRead"
                    @click="markAsRead(notification.id)"
                  >
                    <template v-slot:prepend>
                      <v-icon size="18">mdi-check</v-icon>
                    </template>
                    <v-list-item-title>Đánh dấu đã đọc</v-list-item-title>
                  </v-list-item>
                  <v-list-item @click="deleteNotification(notification.id)">
                    <template v-slot:prepend>
                      <v-icon size="18" color="error">mdi-delete</v-icon>
                    </template>
                    <v-list-item-title class="text-error">Xóa</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>
            </template>
          </v-list-item>

          <v-divider v-if="index < notifications.length - 1" />
        </template>
      </v-list>
    </v-card>

    <!-- Empty State -->
    <v-card v-else class="rounded-lg">
      <div class="text-center py-12">
        <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-bell-off-outline</v-icon>
        <h3 class="text-h6 font-weight-bold mb-2">Không có thông báo</h3>
        <p class="text-body-2 text-medium-emphasis">
          Bạn chưa có thông báo nào. Các thông báo mới sẽ xuất hiện tại đây.
        </p>
      </div>
    </v-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import axios from 'axios'

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
  } catch (error) {
    console.error('Error marking all as read:', error)
  } finally {
    markingAllRead.value = false
  }
}

// Delete notification
const deleteNotification = async (notificationId) => {
  if (!confirm('Bạn có chắc muốn xóa thông báo này?')) return

  try {
    await axios.delete(`http://localhost:5002/api/notifications/${notificationId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Remove from local state
    notifications.value = notifications.value.filter(n => n.id !== notificationId)
  } catch (error) {
    console.error('Error deleting notification:', error)
  }
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

// Get icon based on type
const getIcon = (type) => {
  const iconMap = {
    'System': 'mdi-information',
    'ApplicationApproved': 'mdi-check-circle',
    'InvoiceCreated': 'mdi-receipt-text',
    'MaintenanceDone': 'mdi-tools',
    'ContractExpiring': 'mdi-calendar-alert',
    'PaymentReceived': 'mdi-cash-check',
    'RoomAssigned': 'mdi-door-open'
  }
  return iconMap[type] || 'mdi-bell'
}

// Get icon color based on icon type
const getIconColor = (iconType) => {
  const colorMap = {
    'success': 'success',
    'warning': 'warning',
    'error': 'error',
    'info': 'info'
  }
  return colorMap[iconType] || 'primary'
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

<style scoped>
.notification-item {
  cursor: pointer;
  transition: background-color 0.2s;
}

.notification-item:hover {
  background-color: rgba(0, 0, 0, 0.02);
}
</style>
