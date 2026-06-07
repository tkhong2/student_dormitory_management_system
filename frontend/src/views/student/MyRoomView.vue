<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Phòng của tôi</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Thông tin chi tiết về phòng bạn đang ở</p>

    <!-- Loading State -->
    <v-card v-if="loading" class="pa-12 text-center" style="border:1px solid #e5e7eb">
      <v-progress-circular indeterminate color="primary" size="64" />
      <p class="text-body-1 mt-4">Đang tải thông tin phòng...</p>
    </v-card>

    <!-- No Room State -->
    <v-card v-else-if="!hasRoom" class="pa-12 text-center" style="border:1px solid #e5e7eb">
      <v-icon size="80" color="grey-lighten-2">mdi-home-search-outline</v-icon>
      <h3 class="text-h6 font-weight-bold mt-4 mb-2">Chưa có phòng</h3>
      <p class="text-body-2 text-medium-emphasis mb-6">
        Bạn chưa được phân phòng. Vui lòng đăng ký phòng hoặc liên hệ ban quản lý.
      </p>
      <v-btn color="primary" size="large" to="/student/rooms" prepend-icon="mdi-plus">
        Đăng ký phòng ngay
      </v-btn>
    </v-card>

    <!-- Room Details -->
    <v-row v-else>
      <v-col cols="12" md="8">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-4 mb-6">
            <v-avatar color="#ede9fe" size="56" rounded="lg">
              <v-icon color="primary" size="28">mdi-door-closed</v-icon>
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">Phòng {{ roomInfo.roomNumber }} — {{ roomInfo.buildingName }}</div>
              <div class="text-body-2 text-medium-emphasis">
                Tầng {{ roomInfo.floorNumber }} · {{ roomInfo.roomTypeName }}
              </div>
            </div>
            <v-spacer />
            <v-chip 
              :color="getStatusColor(roomInfo.applicationStatus)" 
              variant="tonal" 
              size="small"
            >
              {{ getStatusText(roomInfo.applicationStatus) }}
            </v-chip>
          </div>

          <v-divider class="mb-5" />

          <v-row>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Loại phòng</div>
              <div class="text-body-2 font-weight-bold">{{ roomInfo.roomTypeName }}</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Sức chứa</div>
              <div class="text-body-2 font-weight-bold">{{ roomInfo.currentOccupants }}/{{ roomInfo.maxOccupants }} sinh viên</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Giá thuê</div>
              <div class="text-body-2 font-weight-bold text-primary">{{ formatPrice(roomInfo.pricePerMonth) }}/tháng</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Diện tích</div>
              <div class="text-body-2 font-weight-bold">{{ roomInfo.area }}m²</div>
            </v-col>
          </v-row>

          <v-divider class="my-5" />

          <h4 class="text-subtitle-2 font-weight-bold mb-3">Tiện nghi phòng</h4>
          <div class="d-flex flex-wrap ga-2">
            <v-chip v-for="amenity in roomAmenities" :key="amenity.label" size="small" variant="tonal" :prepend-icon="amenity.icon" color="primary">
              {{ amenity.label }}
            </v-chip>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" md="4">
        <v-card class="pa-5 mb-4" style="border:1px solid #e5e7eb">
          <h4 class="text-subtitle-2 font-weight-bold mb-4">
            Bạn cùng phòng ({{ roomInfo.currentOccupants }}/{{ roomInfo.maxOccupants }})
          </h4>
          <v-empty-states v-if="roommates.length === 0" text="Chưa có thông tin bạn cùng phòng" />
          <div v-else v-for="mate in roommates" :key="mate.id" class="d-flex align-center ga-3 mb-3">
            <v-avatar size="36" color="primary" variant="tonal">
              <v-icon size="16">mdi-account</v-icon>
            </v-avatar>
            <div>
              <div class="text-body-2 font-weight-medium">{{ mate.name }}</div>
              <div class="text-caption text-medium-emphasis">{{ mate.studentCode }}</div>
            </div>
          </div>
        </v-card>

        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <h4 class="text-subtitle-2 font-weight-bold mb-3">Nội quy phòng</h4>
          <v-list density="compact">
            <v-list-item v-for="rule in rules" :key="rule" :title="rule" prepend-icon="mdi-check-circle-outline" class="text-body-2" />
          </v-list>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { contractService } from '@/services/contractService'

const loading = ref(false)
const hasRoom = ref(false)
const roomInfo = ref(null)
const roommates = ref([])

const rules = [
  'Giữ vệ sinh phòng sạch sẽ',
  'Không gây ồn ào sau 22:00',
  'Không sử dụng thiết bị điện công suất lớn',
  'Tiết kiệm điện nước',
  'Báo cáo khi có hư hỏng',
]

const roomAmenities = computed(() => {
  if (!roomInfo.value) return []
  
  const amenities = []
  
  // Default amenities
  amenities.push({ label: 'Giường', icon: 'mdi-bed' })
  amenities.push({ label: 'Bàn học', icon: 'mdi-desk' })
  amenities.push({ label: 'Tủ cá nhân', icon: 'mdi-locker' })
  amenities.push({ label: 'WiFi', icon: 'mdi-wifi' })
  
  return amenities
})

onMounted(async () => {
  await loadMyRoom()
})

async function loadMyRoom() {
  loading.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    console.log('User from localStorage:', user)
    console.log('Loading room for userId:', userId)

    if (!userId) {
      console.log('No userId found')
      hasRoom.value = false
      return
    }

    // 1. Check for active contracts first - use /user/{userId} endpoint
    const contracts = await contractService.getByUserId(userId)
    console.log('Contracts:', contracts)
    const activeContract = contracts.find(c => c.status === 'Active' || c.status === 'Pending')
    
    if (activeContract) {
      console.log('Found active contract:', activeContract)
      roomInfo.value = {
        roomNumber: activeContract.roomNumber,
        buildingName: activeContract.buildingName,
        floorNumber: '-',
        roomTypeName: activeContract.roomTypeName,
        currentOccupants: '-',
        maxOccupants: '-',
        pricePerMonth: activeContract.monthlyRent,
        area: '-'
      }
      hasRoom.value = true
      return
    }

    // 2. If no contract, check for applications - use /user/{userId} endpoint
    const applications = await roomApplicationService.getByUserId(userId)
    console.log('Applications:', applications)
    
    // Tìm đơn đang chờ xử lý hoặc đã duyệt
    const appWithRoom = applications.find(app => 
      app.status === 'Pending' || app.status === 'UnderReview' || app.status === 'Approved'
    )
    
    if (appWithRoom) {
      console.log('Found application with room:', appWithRoom)
      // Ưu tiên hiển thị assigned room, nếu không có thì hiển thị preferred room
      const roomNumber = appWithRoom.assignedRoomNumber || appWithRoom.preferredRoomNumber
      const buildingName = appWithRoom.assignedBuildingName || appWithRoom.preferredBuildingName
      
      roomInfo.value = {
        roomNumber: roomNumber,
        buildingName: buildingName,
        floorNumber: '-',
        roomTypeName: appWithRoom.preferredRoomTypeName,
        currentOccupants: '-',
        maxOccupants: '-',
        pricePerMonth: appWithRoom.preferredRoomPrice,
        area: '-',
        applicationStatus: appWithRoom.status
      }
      hasRoom.value = true
      return
    }

    // 3. No room assigned
    console.log('No room found')
    hasRoom.value = false
    
  } catch (error) {
    console.error('Error loading room:', error)
    hasRoom.value = false
  } finally {
    loading.value = false
  }
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const getStatusColor = (status) => {
  const colors = {
    'Pending': 'warning',
    'UnderReview': 'info',
    'Approved': 'success',
    'Active': 'success'
  }
  return colors[status] || 'grey'
}

const getStatusText = (status) => {
  const texts = {
    'Pending': 'Chờ duyệt',
    'UnderReview': 'Đang xem xét',
    'Approved': 'Đã duyệt',
    'Active': 'Đang hoạt động'
  }
  return texts[status] || status
}
</script>
