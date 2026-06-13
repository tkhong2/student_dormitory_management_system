<template>
  <div>
    <h1 style="font-size: 28px; font-weight: bold; margin-bottom: 4px;">Phòng của tôi</h1>
    <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 24px;">Thông tin chi tiết về phòng bạn đang ở</p>

    <!-- Loading State -->
    <a-card v-if="loading" :bordered="true" style="text-align: center; padding: 48px;">
      <a-spin size="large" />
      <p style="font-size: 16px; margin-top: 16px;">Đang tải thông tin phòng...</p>
    </a-card>

    <!-- No Room State -->
    <a-card v-else-if="!hasRoom" :bordered="true" style="text-align: center; padding: 48px;">
      <home-outlined style="font-size: 80px; color: #d9d9d9;" />
      <h3 style="font-size: 18px; font-weight: bold; margin-top: 16px; margin-bottom: 8px;">Chưa có phòng</h3>
      <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 24px;">
        Bạn chưa được phân phòng. Vui lòng đăng ký phòng hoặc liên hệ ban quản lý.
      </p>
      <a-button type="primary" size="large" @click="$router.push('/student/rooms')">
        <template #icon><plus-outlined /></template>
        Đăng ký phòng ngay
      </a-button>
    </a-card>

    <!-- Room Details -->
    <a-row v-else :gutter="16">
      <a-col :xs="24" :md="16">
        <a-card :bordered="true" style="padding: 24px;">
          <div style="display: flex; align-items: center; gap: 16px; margin-bottom: 24px;">
            <a-avatar :size="56" style="background-color: #e6f4ff; border-radius: 8px;">
              <template #icon><home-outlined style="color: #1890ff; font-size: 28px;" /></template>
            </a-avatar>
            <div style="flex: 1;">
              <div style="font-size: 20px; font-weight: bold;">Phòng {{ roomInfo.roomNumber }} — {{ roomInfo.buildingName }}</div>
              <div style="font-size: 14px; color: rgba(0,0,0,0.45);">
                Tầng {{ roomInfo.floorNumber }} · {{ roomInfo.roomTypeName }}
              </div>
            </div>
            <a-tag 
              :color="getStatusColor(roomInfo.applicationStatus)"
            >
              {{ getStatusText(roomInfo.applicationStatus) }}
            </a-tag>
          </div>

          <a-divider style="margin: 20px 0;" />

          <a-row :gutter="[16, 16]">
            <a-col :xs="12" :sm="6">
              <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Loại phòng</div>
              <div style="font-size: 14px; font-weight: bold;">{{ roomInfo.roomTypeName }}</div>
            </a-col>
            <a-col :xs="12" :sm="6">
              <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Sức chứa</div>
              <div style="font-size: 14px; font-weight: bold;">{{ roomInfo.currentOccupants }}/{{ roomInfo.maxOccupants }} sinh viên</div>
            </a-col>
            <a-col :xs="12" :sm="6">
              <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Giá thuê</div>
              <div style="font-size: 14px; font-weight: bold; color: #1890ff;">{{ formatPrice(roomInfo.pricePerMonth) }}/tháng</div>
            </a-col>
            <a-col :xs="12" :sm="6">
              <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Diện tích</div>
              <div style="font-size: 14px; font-weight: bold;">{{ roomInfo.area }}m²</div>
            </a-col>
          </a-row>

          <a-divider style="margin: 20px 0;" />

          <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 12px;">Tiện nghi phòng</h4>
          <div style="display: flex; flex-wrap: wrap; gap: 8px;">
            <a-tag v-for="amenity in roomAmenities" :key="amenity.label" color="blue">
              <component :is="amenity.iconComponent" style="margin-right: 4px;" />
              {{ amenity.label }}
            </a-tag>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="8">
        <a-card :bordered="true" style="padding: 20px; margin-bottom: 16px;">
          <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 16px;">
            Bạn cùng phòng ({{ actualOccupants }}/{{ roomInfo.maxOccupants || '?' }})
          </h4>
          
          <!-- Roommates list -->
          <div v-if="roommates.length > 0">
            <div v-for="mate in roommates" :key="mate.id" style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;">
              <a-avatar :size="36" style="background-color: #f0f0f0;">
                <template #icon><user-outlined style="color: #8c8c8c;" /></template>
              </a-avatar>
              <div>
                <div style="font-size: 14px; font-weight: 500;">{{ mate.name }}</div>
                <div style="font-size: 12px; color: rgba(0,0,0,0.45);">{{ mate.studentCode }}</div>
              </div>
            </div>
          </div>
          
          <!-- Empty state when alone -->
          <div v-else style="text-align: center; padding: 16px;">
            <team-outlined style="font-size: 40px; color: #d9d9d9;" />
            <p style="font-size: 12px; color: rgba(0,0,0,0.45); margin-top: 8px; margin-bottom: 0;">
              Bạn đang ở một mình
            </p>
          </div>
        </a-card>

        <a-card :bordered="true" style="padding: 20px;">
          <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 12px;">Nội quy phòng</h4>
          <a-list size="small" :data-source="rules">
            <template #renderItem="{ item }">
              <a-list-item style="padding: 8px 0; border: none;">
                <div style="display: flex; align-items: flex-start; gap: 8px;">
                  <check-circle-outlined style="color: #52c41a; font-size: 16px; margin-top: 2px; flex-shrink: 0;" />
                  <span style="font-size: 14px; text-align: left;">{{ item }}</span>
                </div>
              </a-list-item>
            </template>
          </a-list>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { contractService } from '@/services/contractService'
import { roomService } from '@/services/roomService'
import { roomTypeService } from '@/services/roomTypeService'
import { buildingService } from '@/services/buildingService'
import { floorService } from '@/services/floorService'
import { 
  HomeOutlined, 
  PlusOutlined, 
  UserOutlined, 
  TeamOutlined,
  CheckCircleOutlined,
  WifiOutlined,
  DesktopOutlined,
  InboxOutlined
} from '@ant-design/icons-vue'

const loading = ref(false)
const hasRoom = ref(false)
const roomInfo = ref(null)
const roommates = ref([])
const actualOccupants = ref(0)
const currentUser = JSON.parse(localStorage.getItem('user') || '{}')

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
  
  // Default amenities with Ant Design icons
  amenities.push({ label: 'Giường', iconComponent: HomeOutlined })
  amenities.push({ label: 'Bàn học', iconComponent: DesktopOutlined })
  amenities.push({ label: 'Tủ cá nhân', iconComponent: InboxOutlined })
  amenities.push({ label: 'WiFi', iconComponent: WifiOutlined })
  
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

    // 1. Check for active contracts first
    const contracts = await contractService.getByUserId(userId)
    console.log('Contracts:', contracts)
    const activeContract = contracts.find(c => c.status === 'Active' || c.status === 'Pending')
    
    if (activeContract) {
      console.log('Found active contract:', activeContract)
      
      // Try to load detailed room info, fallback to contract data if fails
      try {
        if (activeContract.roomId) {
          const room = await roomService.getById(activeContract.roomId)
          const roomType = await roomTypeService.getById(room.roomTypeId)
          const building = await buildingService.getById(room.buildingId)
          
          let floorNumber = '-'
          try {
            const floor = await floorService.getById(room.floorId)
            floorNumber = floor.floorNumber
          } catch (e) {
            console.warn('Could not load floor:', e)
          }
          
          roomInfo.value = {
            roomNumber: room.roomNumber,
            buildingName: building.name,
            floorNumber: floorNumber,
            roomTypeName: roomType.name,
            currentOccupants: room.currentOccupants,
            maxOccupants: room.maxOccupants,
            pricePerMonth: roomType.pricePerMonth,
            area: roomType.area,
            roomId: room.id
          }
          
          // Load roommates
          await loadRoommates(room.id)
        } else {
          throw new Error('No roomId in contract')
        }
      } catch (roomError) {
        console.warn('Could not load detailed room info, using contract data:', roomError)
        // Fallback to contract data with better defaults
        roomInfo.value = {
          roomNumber: activeContract.roomNumber || 'N/A',
          buildingName: activeContract.buildingName || 'N/A',
          floorNumber: activeContract.floorNumber || '-',
          roomTypeName: activeContract.roomTypeName || 'N/A',
          currentOccupants: 1, // At least the current user
          maxOccupants: 4, // Default assumption for dorm rooms
          pricePerMonth: activeContract.monthlyRent || 0,
          area: activeContract.roomArea || 20, // Default area
          roomId: activeContract.roomId,
          applicationStatus: 'Active'
        }
        
        // Try to load roommates even with fallback data
        if (activeContract.roomId) {
          await loadRoommates(activeContract.roomId)
        }
      }
      
      hasRoom.value = true
      return
    }

    // 2. Check for applications
    const applications = await roomApplicationService.getByUserId(userId)
    console.log('Applications:', applications)
    
    const appWithRoom = applications.find(app => 
      app.status === 'Approved' && app.assignedRoomId
    )
    
    if (appWithRoom) {
      console.log('Found application with room:', appWithRoom)
      
      try {
        const room = await roomService.getById(appWithRoom.assignedRoomId)
        const roomType = await roomTypeService.getById(room.roomTypeId)
        const building = await buildingService.getById(room.buildingId)
        
        let floorNumber = '-'
        try {
          const floor = await floorService.getById(room.floorId)
          floorNumber = floor.floorNumber
        } catch (e) {
          console.warn('Could not load floor:', e)
        }
        
        roomInfo.value = {
          roomNumber: room.roomNumber,
          buildingName: building.name,
          floorNumber: floorNumber,
          roomTypeName: roomType.name,
          currentOccupants: room.currentOccupants,
          maxOccupants: room.maxOccupants,
          pricePerMonth: roomType.pricePerMonth,
          area: roomType.area,
          applicationStatus: appWithRoom.status,
          roomId: room.id
        }
        
        await loadRoommates(room.id)
      } catch (roomError) {
        console.warn('Could not load detailed room info from application:', roomError)
        // Fallback with better defaults
        roomInfo.value = {
          roomNumber: appWithRoom.assignedRoomNumber || appWithRoom.preferredRoomNumber || 'N/A',
          buildingName: appWithRoom.assignedBuildingName || appWithRoom.preferredBuildingName || 'N/A',
          floorNumber: '-',
          roomTypeName: appWithRoom.preferredRoomTypeName || 'N/A',
          currentOccupants: 1, // At least the current user
          maxOccupants: 4, // Default
          pricePerMonth: appWithRoom.preferredRoomPrice || 0,
          area: 20, // Default area
          applicationStatus: appWithRoom.status,
          roomId: appWithRoom.assignedRoomId
        }
        
        // Try to load roommates
        if (appWithRoom.assignedRoomId) {
          await loadRoommates(appWithRoom.assignedRoomId)
        }
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

async function loadRoommates(roomId) {
  try {
    // Get all contracts for this room
    const allContracts = await contractService.getAll()
    const roomContracts = allContracts.filter(c => 
      c.roomId === roomId && 
      (c.status === 'Active' || c.status === 'Pending')
    )
    
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    
    // Update currentOccupants with actual count (at least 1 for current user)
    const occupantCount = roomContracts.length > 0 ? roomContracts.length : 1
    actualOccupants.value = occupantCount
    if (roomInfo.value) {
      roomInfo.value.currentOccupants = occupantCount
    }
    
    // Map to roommate data (exclude current user)
    roommates.value = roomContracts
      .filter(c => c.userId !== user.id)
      .map(c => ({
        id: c.userId,
        name: c.studentName || 'Sinh viên',
        studentCode: c.studentCode || 'N/A'
      }))
    
    console.log('Loaded roommates:', roommates.value)
    console.log('Total occupants:', occupantCount)
    console.log('actualOccupants.value:', actualOccupants.value)
  } catch (error) {
    console.error('Error loading roommates:', error)
    roommates.value = []
    // If error, still set to at least 1 (current user)
    actualOccupants.value = 1
    if (roomInfo.value) {
      roomInfo.value.currentOccupants = 1
    }
  }
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const getStatusColor = (status) => {
  const colors = {
    'Pending': 'orange',
    'UnderReview': 'blue',
    'Approved': 'green',
    'Active': 'green'
  }
  return colors[status] || 'default'
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
