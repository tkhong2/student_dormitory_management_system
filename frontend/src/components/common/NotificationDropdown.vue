<template>
  <a-dropdown placement="bottomRight" :trigger="['click']">
    <a-badge :count="unreadCount" :offset="[-6, 4]">
      <a-button type="text" shape="circle" @click="loadNotifications">
        <template #icon><BellOutlined /></template>
      </a-button>
    </a-badge>
    
    <template #overlay>
      <div class="notification-dropdown">
        <div class="notification-header">
          <h4>Thông báo</h4>
          <a-button type="link" size="small" @click="markAllAsRead">
            Đánh dấu đã đọc
          </a-button>
        </div>
        
        <a-spin :spinning="loading">
          <div class="notification-list">
            <template v-if="notifications.length === 0">
              <a-empty description="Không có thông báo" :image-style="{ height: '60px' }" />
            </template>
            
            <template v-else>
              <div 
                v-for="notif in notifications" 
                :key="notif.id"
                class="notification-item"
                :class="{ unread: notif.unread, [`priority-${notif.priority}`]: true }"
                @click="handleNotificationClick(notif)"
              >
                <div class="notification-icon">
                  <FileTextOutlined v-if="notif.type === 'room_application'" style="color: #1890ff" />
                  <ToolOutlined v-else-if="notif.type === 'maintenance_request'" style="color: #fa8c16" />
                  <CheckCircleOutlined v-else-if="notif.type === 'contract_approved'" style="color: #52c41a" />
                  <ClockCircleOutlined v-else-if="notif.type === 'contract_expiring'" style="color: #ff4d4f" />
                  <FileTextOutlined v-else-if="notif.type === 'contract_extension'" style="color: #722ed1" />
                  <SwapOutlined v-else-if="notif.type === 'room_transfer'" style="color: #13c2c2" />
                  <BellOutlined v-else style="color: #595959" />
                </div>
                <div class="notification-content">
                  <div class="notification-title">{{ notif.title }}</div>
                  <div class="notification-description">{{ notif.description }}</div>
                  <div class="notification-time">{{ formatTime(notif.time) }}</div>
                </div>
                <div v-if="notif.unread" class="unread-dot"></div>
              </div>
            </template>
          </div>
        </a-spin>
        
        <div class="notification-footer">
          <a-button type="link" block @click="viewAll">
            Xem tất cả
          </a-button>
        </div>
      </div>
    </template>
  </a-dropdown>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  BellOutlined, 
  FileTextOutlined, 
  ToolOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
  SwapOutlined
} from '@ant-design/icons-vue'
import notificationService from '@/services/notificationService'
import { message } from 'ant-design-vue'

const router = useRouter()
const notifications = ref([])
const unreadCount = ref(0)
const loading = ref(false)

const loadNotifications = async () => {
  loading.value = true
  try {
    notifications.value = await notificationService.getAllNotifications()
    unreadCount.value = notifications.value.filter(n => n.unread).length
  } catch (error) {
    console.error('Error loading notifications:', error)
  } finally {
    loading.value = false
  }
}

const formatTime = (time) => {
  const now = new Date()
  const diff = now - time
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)
  
  if (minutes < 1) return 'Vừa xong'
  if (minutes < 60) return `${minutes} phút trước`
  if (hours < 24) return `${hours} giờ trước`
  if (days < 7) return `${days} ngày trước`
  
  return time.toLocaleDateString('vi-VN')
}

const handleNotificationClick = (notif) => {
  if (notif.link) {
    router.push(notif.link)
  }
}

const markAllAsRead = () => {
  notifications.value.forEach(n => n.unread = false)
  unreadCount.value = 0
  message.success('Đã đánh dấu tất cả là đã đọc')
}

const viewAll = () => {
  router.push('/admin/notifications')
}

onMounted(() => {
  loadNotifications()
  // Refresh notifications every 2 minutes
  setInterval(loadNotifications, 120000)
})
</script>

<style scoped>
.notification-dropdown {
  width: 380px;
  max-height: 480px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  overflow: hidden;
}

.notification-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid #f0f0f0;
}

.notification-header h4 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.notification-list {
  max-height: 400px;
  overflow-y: auto;
}

.notification-item {
  display: flex;
  gap: 12px;
  padding: 12px 16px;
  cursor: pointer;
  transition: background-color 0.2s;
  border-bottom: 1px solid #f5f5f5;
  position: relative;
}

.notification-item:hover {
  background-color: #fafafa;
}

.notification-item.unread {
  background-color: #f0f9ff;
}

.notification-item.priority-high {
  border-left: 3px solid #ff4d4f;
}

.notification-icon {
  font-size: 20px;
  padding-top: 2px;
}

.notification-content {
  flex: 1;
  min-width: 0;
}

.notification-title {
  font-weight: 600;
  font-size: 14px;
  color: #262626;
  margin-bottom: 4px;
}

.notification-description {
  font-size: 13px;
  color: #595959;
  margin-bottom: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.notification-time {
  font-size: 12px;
  color: #8c8c8c;
}

.unread-dot {
  width: 8px;
  height: 8px;
  background-color: #1890ff;
  border-radius: 50%;
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
}

.notification-footer {
  border-top: 1px solid #f0f0f0;
  padding: 8px;
}

.notification-list::-webkit-scrollbar {
  width: 6px;
}

.notification-list::-webkit-scrollbar-track {
  background: #f5f5f5;
}

.notification-list::-webkit-scrollbar-thumb {
  background: #d9d9d9;
  border-radius: 3px;
}

.notification-list::-webkit-scrollbar-thumb:hover {
  background: #bfbfbf;
}
</style>
