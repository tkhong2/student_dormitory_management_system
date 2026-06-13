<template>
  <div>
    <!-- Welcome -->
    <a-card :bordered="false" style="background: linear-gradient(135deg, #1890ff 0%, #096dd9 100%); margin-bottom: 24px; border-radius: 12px;">
      <div style="display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 16px;">
        <div>
          <h1 style="color: white; font-size: 28px; font-weight: bold; margin-bottom: 8px;">Xin chào, {{ currentUser.fullName }}! 👋</h1>
          <p style="color: rgba(255,255,255,0.85); max-width: 500px; margin: 0;">Chào mừng bạn đến với Cổng thông tin Sinh viên KTX. Tại đây bạn có thể theo dõi phòng ở, hợp đồng, thanh toán và gửi yêu cầu hỗ trợ.</p>
        </div>
        <a-avatar :size="80" :src="userAvatarUrl" style="box-shadow: 0 4px 6px rgba(0,0,0,0.1);" />
      </div>
    </a-card>

    <!-- Quick Info Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 24px;">
      <!-- Room Card -->
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true" :loading="loadingRoom" style="padding: 20px;">
          <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
            <a-avatar :size="42" style="background-color: #e6f4ff; border-radius: 8px;">
              <template #icon><home-outlined style="color: #1890ff; font-size: 22px;" /></template>
            </a-avatar>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">Phòng của tôi</div>
          </div>
          <template v-if="roomInfo">
            <div style="font-size: 20px; font-weight: bold;">{{ roomInfo.roomNumber }}</div>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">{{ roomInfo.buildingName }}</div>
          </template>
          <template v-else>
            <div style="font-size: 14px; color: rgba(0,0,0,0.45);">Chưa xếp phòng</div>
          </template>
        </a-card>
      </a-col>

      <!-- Contract Card -->
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true" :loading="loadingContract" style="padding: 20px;">
          <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
            <a-avatar :size="42" style="background-color: #dcfce7; border-radius: 8px;">
              <template #icon><file-text-outlined style="color: #52c41a; font-size: 22px;" /></template>
            </a-avatar>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">Hợp đồng</div>
          </div>
          <template v-if="contractInfo">
            <div style="font-size: 20px; font-weight: bold;" :style="{ color: contractInfo.statusColorHex }">
              {{ contractInfo.statusText }}
            </div>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">
              Hết hạn: {{ contractInfo.endDate }}
            </div>
          </template>
          <template v-else>
            <div style="font-size: 14px; color: rgba(0,0,0,0.45);">Chưa có hợp đồng</div>
          </template>
        </a-card>
      </a-col>

      <!-- Payment Card -->
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true" :loading="loadingPayment" style="padding: 20px;">
          <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
            <a-avatar :size="42" style="background-color: #fff7ed; border-radius: 8px;">
              <template #icon><clock-circle-outlined style="color: #faad14; font-size: 22px;" /></template>
            </a-avatar>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">Thanh toán</div>
          </div>
          <template v-if="paymentInfo">
            <div style="font-size: 20px; font-weight: bold; color: #faad14;">
              {{ formatCurrency(paymentInfo.amount) }}
            </div>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">
              Hạn: {{ paymentInfo.dueDate }}
            </div>
          </template>
          <template v-else>
            <div style="font-size: 14px; color: rgba(0,0,0,0.45);">Không có hóa đơn chưa thanh toán</div>
          </template>
        </a-card>
      </a-col>

      <!-- Maintenance Card -->
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true" :loading="loadingMaintenance" style="padding: 20px;">
          <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
            <a-avatar :size="42" style="background-color: #e0f2fe; border-radius: 8px;">
              <template #icon><tool-outlined style="color: #1890ff; font-size: 22px;" /></template>
            </a-avatar>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">Yêu cầu sửa chữa</div>
          </div>
          <template v-if="maintenanceInfo && maintenanceInfo.count > 0">
            <div style="font-size: 20px; font-weight: bold;">{{ maintenanceInfo.count }}</div>
            <div style="font-size: 12px; color: rgba(0,0,0,0.45);">{{ maintenanceInfo.status }}</div>
          </template>
          <template v-else>
            <div style="font-size: 14px; color: rgba(0,0,0,0.45);">Không có yêu cầu</div>
          </template>
        </a-card>
      </a-col>
    </a-row>

    <a-row :gutter="[16, 16]">
      <!-- Roommates -->
      <a-col :xs="24" :md="12">
        <a-card :bordered="true" :loading="loadingRoommates" style="padding: 20px; height: 100%;">
          <h3 style="font-size: 16px; font-weight: bold; margin-bottom: 16px;">Bạn cùng phòng</h3>
          
          <template v-if="roommates.length > 0">
            <div v-for="mate in roommates" :key="mate.studentCode" style="display: flex; align-items: center; gap: 12px; margin-bottom: 16px;">
              <a-avatar :size="40" style="background-color: #e6f4ff;">
                <template #icon><user-outlined style="color: #1890ff;" /></template>
              </a-avatar>
              <div style="flex: 1;">
                <div style="font-size: 14px; font-weight: bold;">{{ mate.name }}</div>
                <div style="font-size: 12px; color: rgba(0,0,0,0.45);">{{ mate.info }}</div>
              </div>
              <a-tag color="success">Đang ở</a-tag>
            </div>
            
            <div v-if="roomCapacity.available > 0" style="text-align: center; padding: 8px 0;">
              <a-tag color="blue">
                Còn {{ roomCapacity.available }} giường trống
              </a-tag>
            </div>
          </template>
          
          <template v-else-if="!loadingRoommates && !roomInfo">
            <div style="text-align: center; padding: 32px 0; color: rgba(0,0,0,0.45);">
              <home-outlined style="font-size: 48px; color: #d9d9d9; margin-bottom: 8px; display: block;" />
              <div style="font-size: 14px;">Chưa có phòng</div>
            </div>
          </template>
          
          <template v-else-if="!loadingRoommates">
            <div style="text-align: center; padding: 32px 0; color: rgba(0,0,0,0.45);">
              <team-outlined style="font-size: 48px; color: #d9d9d9; margin-bottom: 8px; display: block;" />
              <div style="font-size: 14px;">Bạn ở phòng một mình</div>
            </div>
          </template>
        </a-card>
      </a-col>

      <!-- Announcements -->
      <a-col :xs="24" :md="12">
        <a-card :bordered="true" style="padding: 20px; height: 100%;">
          <h3 style="font-size: 16px; font-weight: bold; margin-bottom: 16px;">Thông báo mới</h3>
          
          <template v-if="announcements.length > 0">
            <div v-for="a in announcements" :key="a.id" style="display: flex; gap: 12px; margin-bottom: 16px;">
              <a-avatar :size="32" :style="{ backgroundColor: getAntdColor(a.color) }">
                <template #icon>
                  <component :is="getAntdIcon(a.type)" />
                </template>
              </a-avatar>
              <div style="min-width: 0; flex: 1;">
                <div style="font-size: 14px; font-weight: 500;">
                  {{ a.title }}
                  <a-tag v-if="!a.isRead" color="blue" style="margin-left: 4px;">Mới</a-tag>
                </div>
                <div style="font-size: 12px; color: rgba(0,0,0,0.45);">{{ a.time }}</div>
              </div>
            </div>
            <a-button type="link" block @click="$router.push('/student/notifications')">
              Xem tất cả
            </a-button>
          </template>
          
          <template v-else>
            <div style="text-align: center; padding: 32px 0; color: rgba(0,0,0,0.45);">
              <bell-outlined style="font-size: 48px; color: #d9d9d9; margin-bottom: 8px; display: block;" />
              <div style="font-size: 14px;">Không có thông báo mới</div>
            </div>
          </template>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { contractService } from '@/services/contractService'
import axios from 'axios'
import { 
  HomeOutlined, 
  FileTextOutlined, 
  ClockCircleOutlined, 
  ToolOutlined, 
  UserOutlined, 
  TeamOutlined, 
  BellOutlined,
  InfoCircleOutlined,
  CheckCircleOutlined,
  FileSearchOutlined,
  CalendarOutlined,
  DollarOutlined
} from '@ant-design/icons-vue'

const authStore = useAuthStore()

// Get current user from auth store
const currentUser = computed(() => authStore.user || {
  fullName: 'Sinh viên',
  avatarUrl: null,
  id: null
})

// Process avatar URL
const userAvatarUrl = computed(() => {
  if (currentUser.value.avatarUrl) {
    if (currentUser.value.avatarUrl.startsWith('http')) {
      return currentUser.value.avatarUrl
    }
    return `http://localhost:5003${currentUser.value.avatarUrl}`
  }
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(currentUser.value.fullName || 'SV')}&background=1890ff&color=fff&size=128`
})

// Loading states
const loadingRoom = ref(false)
const loadingContract = ref(false)
const loadingPayment = ref(false)
const loadingMaintenance = ref(false)

// Data
const roomInfo = ref(null)
const contractInfo = ref(null)
const paymentInfo = ref(null)
const maintenanceInfo = ref(null)
const roommates = ref([])
const announcements = ref([])

// Loading
const loadingRoommates = ref(false)

// Format currency
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('vi-VN', { 
    style: 'currency', 
    currency: 'VND' 
  }).format(amount)
}

// Format date
const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('vi-VN')
}

// Fetch room info
const fetchRoomInfo = async () => {
  loadingRoom.value = true
  try {
    const userId = currentUser.value.id
    if (!userId) return

    // Get active contract
    const contracts = await contractService.getByUserId(userId)
    const activeContract = contracts.find(c => c.status === 'Active')
    
    if (activeContract) {
      roomInfo.value = {
        roomNumber: activeContract.roomNumber,
        buildingName: activeContract.buildingName
      }
    }
  } catch (error) {
    console.error('Error fetching room info:', error)
  } finally {
    loadingRoom.value = false
  }
}

// Fetch contract info
const fetchContractInfo = async () => {
  loadingContract.value = true
  try {
    const userId = currentUser.value.id
    if (!userId) return

    const contracts = await contractService.getByUserId(userId)
    const activeContract = contracts.find(c => c.status === 'Active' || c.status === 'Pending')
    
    if (activeContract) {
      const statusMap = {
        'Active': { text: 'Hiệu lực', colorHex: '#52c41a' },
        'Pending': { text: 'Chờ duyệt', colorHex: '#faad14' },
        'Expired': { text: 'Hết hạn', colorHex: '#ff4d4f' }
      }
      
      const status = statusMap[activeContract.status] || { text: activeContract.status, colorHex: '#000' }
      
      contractInfo.value = {
        statusText: status.text,
        statusColorHex: status.colorHex,
        endDate: formatDate(activeContract.endDate)
      }
    }
  } catch (error) {
    console.error('Error fetching contract info:', error)
  } finally {
    loadingContract.value = false
  }
}

// Fetch payment info (unpaid invoices)
const fetchPaymentInfo = async () => {
  loadingPayment.value = true
  try {
    const userId = currentUser.value.id
    if (!userId) {
      console.log('No userId for payment')
      return
    }

    console.log('=== FETCH PAYMENT INFO ===')
    console.log('Fetching for userId:', userId)

    // First, get studentId from active contract
    const contracts = await contractService.getByUserId(userId)
    const activeContract = contracts.find(c => c.status === 'Active')
    
    if (!activeContract) {
      console.log('No active contract, cannot get studentId')
      return
    }

    const studentId = activeContract.studentId
    console.log('Student ID from contract:', studentId)

    // Get invoices for this user
    const response = await axios.get('http://localhost:5002/api/invoices', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    console.log('All invoices:', response.data)
    
    // Find unpaid invoices for current student using studentId
    const unpaidInvoice = response.data.find(inv => {
      console.log(`Invoice ${inv.invoiceCode}: studentId=${inv.studentId}, status=${inv.status}, debt=${inv.debtAmount}, total=${inv.totalAmount}`)
      // Only show invoices that are Unpaid or PartiallyPaid AND have remaining debt
      const isUnpaid = inv.status === 'Unpaid' || inv.status === 'PartiallyPaid'
      const hasDebt = (inv.debtAmount && inv.debtAmount > 0) || (inv.totalAmount && inv.totalAmount > inv.paidAmount)
      return inv.studentId === studentId && isUnpaid && hasDebt
    })
    
    console.log('Unpaid invoice found:', unpaidInvoice)
    
    if (unpaidInvoice) {
      paymentInfo.value = {
        amount: unpaidInvoice.debtAmount || unpaidInvoice.totalAmount,
        dueDate: formatDate(unpaidInvoice.dueDate)
      }
    } else {
      paymentInfo.value = null
    }
  } catch (error) {
    console.error('Error fetching payment info:', error)
  } finally {
    loadingPayment.value = false
  }
}

// Fetch maintenance requests
const fetchMaintenanceInfo = async () => {
  loadingMaintenance.value = true
  try {
    const userId = currentUser.value.id
    if (!userId) {
      console.log('No userId for maintenance')
      return
    }

    console.log('=== FETCH MAINTENANCE ===')
    console.log('Fetching for userId:', userId)

    const response = await axios.get(`http://localhost:5002/api/maintenancerequests`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    console.log('All maintenance requests:', response.data)
    
    // Count pending/inprogress requests for current student using userId directly
    const pendingRequests = response.data.filter(req => {
      console.log(`Request ${req.id}: requestedByStudentId=${req.requestedByStudentId}, status=${req.status}`)
      return req.requestedByStudentId === userId && 
        (req.status === 'New' || req.status === 'Assigned' || req.status === 'InProgress')
    })
    
    console.log('Pending requests for user:', pendingRequests)
    
    if (pendingRequests.length > 0) {
      maintenanceInfo.value = {
        count: pendingRequests.length,
        status: 'Đang xử lý'
      }
    } else {
      maintenanceInfo.value = null
    }
  } catch (error) {
    console.error('Error fetching maintenance info:', error)
  } finally {
    loadingMaintenance.value = false
  }
}

// Fetch roommates (students in same room)
const fetchRoommates = async () => {
  loadingRoommates.value = true
  try {
    const userId = currentUser.value.id
    console.log('=== FETCH ROOMMATES ===')
    console.log('User ID:', userId)
    
    if (!userId) return

    // Get active contract to find room
    const contracts = await contractService.getByUserId(userId)
    console.log('User contracts:', contracts)
    
    const activeContract = contracts.find(c => c.status === 'Active')
    console.log('Active contract:', activeContract)
    
    if (!activeContract) {
      return // No room assigned yet
    }

    // Get all contracts for the same room
    const allContracts = await contractService.getAll()
    console.log('Current student ID:', activeContract.studentId)
    console.log('All contracts in room:', allContracts.filter(c => c.roomId === activeContract.roomId && c.status === 'Active'))
    
    const sameRoomContracts = allContracts.filter(c => 
      c.roomId === activeContract.roomId && 
      c.status === 'Active' &&
      c.studentId !== activeContract.studentId // Exclude current user by studentId, not userId
    )
    console.log('Roommate contracts found:', sameRoomContracts.length)
    console.log('Roommate contracts:', sameRoomContracts)

    // Fetch student details for each contract
    const roommatePromises = sameRoomContracts.map(async (contract) => {
      try {
        const response = await axios.get(`http://localhost:5001/api/students/${contract.studentId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        })
        return {
          name: response.data.fullName,
          info: `${response.data.studentCode} · ${response.data.classCode || 'N/A'}`,
          studentCode: response.data.studentCode
        }
      } catch (error) {
        console.error('Error fetching roommate:', error)
        return null
      }
    })

    const roommateResults = await Promise.all(roommatePromises)
    roommates.value = roommateResults.filter(r => r !== null)

  } catch (error) {
    console.error('Error fetching roommates:', error)
  } finally {
    loadingRoommates.value = false
  }
}

// Fetch notifications from Notifications API
const fetchAnnouncements = async () => {
  try {
    const userId = currentUser.value.id
    console.log('=== FETCH ANNOUNCEMENTS ===')
    console.log('User ID:', userId)
    
    if (!userId) return

    // Get student ID from active contract (since notifications use studentId, not userId)
    const contracts = await contractService.getByUserId(userId)
    console.log('User contracts:', contracts)
    
    const activeContract = contracts.find(c => c.status === 'Active')
    console.log('Active contract:', activeContract)
    
    if (!activeContract) {
      console.log('No active contract found, cannot fetch notifications')
      return
    }

    const studentId = activeContract.studentId
    console.log('Fetching notifications for studentId:', studentId)

    // Get notifications from BillingMaintenanceService using studentId
    const response = await axios.get(`http://localhost:5002/api/notifications/user/${studentId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Take only the 4 most recent notifications
    announcements.value = response.data
      .slice(0, 4)
      .map(n => ({
        id: n.id,
        title: n.title,
        time: getRelativeTime(n.createdAt),
        type: n.type,  // Use type directly
        color: n.iconType,  // Use iconType directly
        isRead: n.isRead
      }))
  } catch (error) {
    console.error('Error fetching notifications:', error)
    // Keep empty if no notifications
  }
}

// Helper function to get relative time
const getRelativeTime = (dateString) => {
  const date = new Date(dateString)
  const now = new Date()
  const diffMs = now - date
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMins < 60) return `${diffMins} phút trước`
  if (diffHours < 24) return `${diffHours} giờ trước`
  if (diffDays === 1) return 'Hôm qua'
  if (diffDays < 7) return `${diffDays} ngày trước`
  return date.toLocaleDateString('vi-VN')
}

// Helper function to get Ant Design icon based on type
const getAntdIcon = (type) => {
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

// Helper function to get Ant Design color based on icon type
const getAntdColor = (iconType) => {
  const colorMap = {
    'success': '#52c41a',
    'warning': '#faad14',
    'error': '#ff4d4f',
    'info': '#1890ff',
    'primary': '#1890ff'
  }
  return colorMap[iconType] || '#1890ff'
}

// Keep original helper functions for backward compatibility
const getAnnouncementIcon = (type) => {
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

const getAnnouncementColor = (iconType) => {
  const colorMap = {
    'success': 'success',
    'warning': 'warning',
    'error': 'error',
    'info': 'info'
  }
  return colorMap[iconType] || 'primary'
}

// Fetch all data on mount
onMounted(() => {
  fetchRoomInfo()
  fetchContractInfo()
  fetchPaymentInfo()
  fetchMaintenanceInfo()
  fetchRoommates()
  fetchAnnouncements()
})

const roomCapacity = computed(() => {
  // Get room capacity from room info if available
  // For now, assume 4-person rooms
  const capacity = 4
  const occupied = roommates.value.length + 1 // +1 for current user
  const available = capacity - occupied
  return {
    total: capacity,
    occupied,
    available
  }
})
</script>
