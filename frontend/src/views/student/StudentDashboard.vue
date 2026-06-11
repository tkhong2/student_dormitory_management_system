<template>
  <div>
    <!-- Welcome -->
    <v-card class="pa-6 mb-6 gradient-secondary rounded-xl">
      <div class="d-flex align-center justify-space-between flex-wrap ga-4">
        <div>
          <h1 class="text-h4 font-weight-bold mb-2">Xin chào, {{ currentUser.fullName }}! 👋</h1>
          <p style="opacity:.85;max-width:500px">Chào mừng bạn đến với Cổng thông tin Sinh viên KTX. Tại đây bạn có thể theo dõi phòng ở, hợp đồng, thanh toán và gửi yêu cầu hỗ trợ.</p>
        </div>
        <v-avatar size="80" class="elevation-4">
          <v-img :src="userAvatarUrl" />
        </v-avatar>
      </div>
    </v-card>

    <!-- Quick Info Cards -->
    <v-row class="mb-6">
      <!-- Room Card -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb" :loading="loadingRoom">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#ede9fe" size="42" rounded="lg">
              <v-icon color="primary" size="22">mdi-door-closed</v-icon>
            </v-avatar>
            <div class="text-caption text-medium-emphasis">Phòng của tôi</div>
          </div>
          <template v-if="roomInfo">
            <div class="text-h5 font-weight-bold">{{ roomInfo.roomNumber }}</div>
            <div class="text-caption text-medium-emphasis">{{ roomInfo.buildingName }}</div>
          </template>
          <template v-else>
            <div class="text-body-2 text-medium-emphasis">Chưa xếp phòng</div>
          </template>
        </v-card>
      </v-col>

      <!-- Contract Card -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb" :loading="loadingContract">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#dcfce7" size="42" rounded="lg">
              <v-icon color="success" size="22">mdi-file-check</v-icon>
            </v-avatar>
            <div class="text-caption text-medium-emphasis">Hợp đồng</div>
          </div>
          <template v-if="contractInfo">
            <div class="text-h5 font-weight-bold" :class="contractInfo.statusColor">
              {{ contractInfo.statusText }}
            </div>
            <div class="text-caption text-medium-emphasis">
              Hết hạn: {{ contractInfo.endDate }}
            </div>
          </template>
          <template v-else>
            <div class="text-body-2 text-medium-emphasis">Chưa có hợp đồng</div>
          </template>
        </v-card>
      </v-col>

      <!-- Payment Card -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb" :loading="loadingPayment">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#fff7ed" size="42" rounded="lg">
              <v-icon color="warning" size="22">mdi-cash-clock</v-icon>
            </v-avatar>
            <div class="text-caption text-medium-emphasis">Thanh toán</div>
          </div>
          <template v-if="paymentInfo">
            <div class="text-h5 font-weight-bold text-warning">
              {{ formatCurrency(paymentInfo.amount) }}
            </div>
            <div class="text-caption text-medium-emphasis">
              Hạn: {{ paymentInfo.dueDate }}
            </div>
          </template>
          <template v-else>
            <div class="text-body-2 text-medium-emphasis">Không có hóa đơn chưa thanh toán</div>
          </template>
        </v-card>
      </v-col>

      <!-- Maintenance Card -->
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb" :loading="loadingMaintenance">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#e0f2fe" size="42" rounded="lg">
              <v-icon color="info" size="22">mdi-wrench</v-icon>
            </v-avatar>
            <div class="text-caption text-medium-emphasis">Yêu cầu sửa chữa</div>
          </div>
          <template v-if="maintenanceInfo && maintenanceInfo.count > 0">
            <div class="text-h5 font-weight-bold">{{ maintenanceInfo.count }}</div>
            <div class="text-caption text-medium-emphasis">{{ maintenanceInfo.status }}</div>
          </template>
          <template v-else>
            <div class="text-body-2 text-medium-emphasis">Không có yêu cầu</div>
          </template>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <!-- Roommates -->
      <v-col cols="12" md="6">
        <v-card class="pa-5 h-100" style="border:1px solid #e5e7eb" :loading="loadingRoommates">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Bạn cùng phòng</h3>
          
          <template v-if="roommates.length > 0">
            <div v-for="mate in roommates" :key="mate.studentCode" class="d-flex align-center ga-3 mb-4">
              <v-avatar size="40" color="primary" variant="tonal">
                <v-icon size="20">mdi-account</v-icon>
              </v-avatar>
              <div class="flex-grow-1">
                <div class="text-body-2 font-weight-bold">{{ mate.name }}</div>
                <div class="text-caption text-medium-emphasis">{{ mate.info }}</div>
              </div>
              <v-chip size="x-small" variant="tonal" color="success">Đang ở</v-chip>
            </div>
            
            <div v-if="roomCapacity.available > 0" class="text-center py-2">
              <v-chip variant="outlined" size="small" color="info">
                Còn {{ roomCapacity.available }} giường trống
              </v-chip>
            </div>
          </template>
          
          <template v-else-if="!loadingRoommates && !roomInfo">
            <div class="text-center py-8 text-medium-emphasis">
              <v-icon size="48" color="grey-lighten-1" class="mb-2">mdi-home-off</v-icon>
              <div class="text-body-2">Chưa có phòng</div>
            </div>
          </template>
          
          <template v-else-if="!loadingRoommates">
            <div class="text-center py-8 text-medium-emphasis">
              <v-icon size="48" color="grey-lighten-1" class="mb-2">mdi-account-multiple</v-icon>
              <div class="text-body-2">Bạn ở phòng một mình</div>
            </div>
          </template>
        </v-card>
      </v-col>

      <!-- Announcements -->
      <v-col cols="12" md="6">
        <v-card class="pa-5 h-100" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Thông báo mới</h3>
          
          <template v-if="announcements.length > 0">
            <div v-for="a in announcements" :key="a.id" class="d-flex ga-3 mb-4">
              <v-avatar :color="a.color" size="32" variant="tonal">
                <v-icon size="16">{{ a.icon }}</v-icon>
              </v-avatar>
              <div style="min-width:0">
                <div class="text-body-2 font-weight-medium">{{ a.title }}</div>
                <div class="text-caption text-medium-emphasis">{{ a.time }}</div>
              </div>
            </div>
          </template>
          
          <template v-else>
            <div class="text-center py-8 text-medium-emphasis">
              <v-icon size="48" color="grey-lighten-1" class="mb-2">mdi-bell-off</v-icon>
              <div class="text-body-2">Không có thông báo mới</div>
            </div>
          </template>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { contractService } from '@/services/contractService'
import axios from 'axios'

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
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(currentUser.value.fullName || 'SV')}&background=6366f1&color=fff&size=128`
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
        'Active': { text: 'Hiệu lực', color: 'text-success' },
        'Pending': { text: 'Chờ duyệt', color: 'text-warning' },
        'Expired': { text: 'Hết hạn', color: 'text-error' }
      }
      
      const status = statusMap[activeContract.status] || { text: activeContract.status, color: '' }
      
      contractInfo.value = {
        statusText: status.text,
        statusColor: status.color,
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
    if (!userId) return

    // Get invoices for this user
    const response = await axios.get('http://localhost:5002/api/invoices', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    // Find unpaid invoices for current student
    const unpaidInvoice = response.data.find(inv => 
      inv.studentId === userId && 
      (inv.status === 'Unpaid' || inv.status === 'PartiallyPaid')
    )
    
    if (unpaidInvoice) {
      paymentInfo.value = {
        amount: unpaidInvoice.debtAmount || unpaidInvoice.totalAmount,
        dueDate: formatDate(unpaidInvoice.dueDate)
      }
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
    if (!userId) return

    const response = await axios.get('http://localhost:5002/api/maintenancerequests', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    
    // Count pending/inprogress requests for current student
    const pendingRequests = response.data.filter(req => 
      req.requestedByStudentId === userId && 
      (req.status === 'Pending' || req.status === 'Assigned' || req.status === 'InProgress')
    )
    
    if (pendingRequests.length > 0) {
      maintenanceInfo.value = {
        count: pendingRequests.length,
        status: 'Đang xử lý'
      }
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
    if (!userId) return

    // Get active contract to find room
    const contracts = await contractService.getByUserId(userId)
    const activeContract = contracts.find(c => c.status === 'Active')
    
    if (!activeContract) {
      return // No room assigned yet
    }

    // Get all contracts for the same room
    const allContracts = await contractService.getAll()
    const sameRoomContracts = allContracts.filter(c => 
      c.roomId === activeContract.roomId && 
      c.status === 'Active' &&
      c.studentId !== userId // Exclude current user
    )

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

// Fetch announcements from BuildingAnnouncements API
const fetchAnnouncements = async () => {
  try {
    // Get announcements from RoomBuildingService
    const response = await axios.get('http://localhost:5003/api/buildingannouncements', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Take only the 4 most recent active announcements
    announcements.value = response.data
      .filter(a => a.isActive !== false)
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 4)
      .map(a => ({
        id: a.id,
        title: a.title,
        time: getRelativeTime(a.createdAt),
        icon: getAnnouncementIcon(a.type),
        color: getAnnouncementColor(a.priority)
      }))
  } catch (error) {
    console.error('Error fetching announcements:', error)
    // Keep empty if no announcements
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

// Helper function to get icon based on type
const getAnnouncementIcon = (type) => {
  const iconMap = {
    'Payment': 'mdi-cash-clock',
    'Inspection': 'mdi-calendar-check',
    'Rule': 'mdi-book-open',
    'Maintenance': 'mdi-wrench',
    'Event': 'mdi-calendar-star',
    'Emergency': 'mdi-alert'
  }
  return iconMap[type] || 'mdi-information'
}

// Helper function to get color based on priority
const getAnnouncementColor = (priority) => {
  const colorMap = {
    'High': 'error',
    'Medium': 'warning',
    'Low': 'info'
  }
  return colorMap[priority] || 'primary'
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
