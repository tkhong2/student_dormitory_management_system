<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Đăng ký phòng</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
        Chọn phòng phù hợp để đăng ký ở ký túc xá
      </p>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px;" :bordered="false">
      <a-row :gutter="[16, 16]" align="middle">
        <a-col :xs="24" :sm="8" :md="6">
          <a-select
            v-model:value="filters.building"
            placeholder="Tòa nhà"
            allow-clear
            style="width: 100%"
          >
            <a-select-option v-for="building in buildings" :key="building" :value="building">
              {{ building }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="8" :md="6">
          <a-select
            v-model:value="filters.roomType"
            placeholder="Loại phòng"
            allow-clear
            style="width: 100%"
          >
            <a-select-option v-for="type in roomTypes" :key="type" :value="type">
              {{ type }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="8" :md="6">
          <a-select
            v-model:value="filters.priceRange"
            placeholder="Mức giá"
            allow-clear
            style="width: 100%"
          >
            <a-select-option v-for="range in priceRanges" :key="range" :value="range">
              {{ range }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="24" :md="6">
          <a-space>
            <a-badge :count="filteredRooms.length" :overflow-count="999" style="background: #52c41a;">
              <a-button type="primary">
                {{ filteredRooms.length }} phòng
              </a-button>
            </a-badge>
            <a-button v-if="hasFilters" @click="resetFilters">
              Đặt lại
            </a-button>
          </a-space>
        </a-col>
      </a-row>
    </a-card>

    <!-- Room List Card -->
    <a-card :bordered="false" :loading="loading">
      <a-row :gutter="[16, 16]">
        <a-col v-for="room in filteredRooms" :key="room.id" :xs="24" :sm="12" :lg="8" :xl="6">
          <a-card hoverable class="room-card" :bordered="false" style="border: 1px solid #f0f0f0;">
            <template #cover>
              <div style="position: relative; height: 180px; overflow: hidden;">
                <img :src="room.image" :alt="room.name" style="width: 100%; height: 100%; object-fit: cover;" />
                <a-tag 
                  v-if="room.available > 0" 
                  color="success" 
                  style="position: absolute; top: 12px; right: 12px; font-weight: 600;"
                >
                  {{ room.available }} chỗ trống
                </a-tag>
                <a-tag 
                  v-else 
                  color="error" 
                  style="position: absolute; top: 12px; right: 12px; font-weight: 600;"
                >
                  Đã đầy
                </a-tag>
              </div>
            </template>
            
            <a-card-meta>
              <template #title>
                <div style="display: flex; justify-content: space-between; align-items: center;">
                  <span style="font-size: 16px; font-weight: 600;">{{ room.name }}</span>
                  <a-tag color="blue">{{ room.building }}</a-tag>
                </div>
              </template>
              <template #description>
                <a-space direction="vertical" size="small" style="width: 100%; margin-top: 8px;">
                  <div style="display: flex; align-items: center; gap: 8px;">
                    <UserOutlined style="color: #1890ff;" />
                    <span style="font-size: 13px;">{{ room.capacity }} người/phòng</span>
                  </div>
                  <div style="display: flex; align-items: center; gap: 8px;">
                    <ExpandOutlined style="color: #1890ff;" />
                    <span style="font-size: 13px;">{{ room.area }} m²</span>
                  </div>
                  <div style="display: flex; align-items: center; gap: 8px;">
                    <ThunderboltOutlined style="color: #1890ff;" />
                    <span style="font-size: 13px;">{{ room.facilities }}</span>
                  </div>
                  <a-divider style="margin: 8px 0;" />
                  <div style="display: flex; justify-content: space-between; align-items: center;">
                    <div>
                      <div style="font-size: 11px; color: #8c8c8c;">Giá/tháng</div>
                      <div style="font-size: 16px; font-weight: 700; color: #ff9800;">
                        {{ formatPrice(room.price) }}
                      </div>
                    </div>
                    <a-button 
                      type="primary" 
                      :disabled="room.available === 0"
                      @click="openRegistrationDialog(room)"
                      style="background: #ff9800; border-color: #ff9800;"
                    >
                      Đăng ký
                    </a-button>
                  </div>
                </a-space>
              </template>
            </a-card-meta>
          </a-card>
        </a-col>
      </a-row>

      <!-- Empty State -->
      <a-empty 
        v-if="!loading && filteredRooms.length === 0" 
        description="Không tìm thấy phòng phù hợp"
        style="padding: 60px 0;"
      >
        <a-button type="primary" @click="resetFilters">Xem tất cả phòng</a-button>
      </a-empty>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  UserOutlined, 
  ExpandOutlined, 
  ThunderboltOutlined 
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { roomService } from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { roomTypeService } from '@/services/roomTypeService'
import { roomApplicationService } from '@/services/roomApplicationService'
import { contractService } from '@/services/contractService'

const router = useRouter()
const loading = ref(false)
const hasActiveRegistration = ref(false)

const filters = ref({
  building: undefined,
  roomType: undefined,
  priceRange: undefined
})

const rooms = ref([])
const buildings = ref([])
const roomTypes = ref([])
const priceRanges = ['Dưới 500k', '500k - 1tr', '1tr - 1.5tr', 'Trên 1.5tr']

onMounted(async () => {
  await checkExistingRegistration()
  await loadData()
})

const hasFilters = computed(() => {
  return filters.value.building || filters.value.roomType || filters.value.priceRange
})

async function checkExistingRegistration() {
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const student = JSON.parse(localStorage.getItem('student') || '{}')
    const studentId = student.id || user.studentId

    if (!studentId) return

    // Check for active applications (Pending or Approved)
    const applications = await roomApplicationService.getByStudentId(studentId)
    const activeApp = applications.find(app => 
      app.status === 'Pending' || app.status === 'Approved' || app.status === 'UnderReview'
    )

    // Check for active contracts
    const contracts = await contractService.getByStudentId(studentId)
    const activeContract = contracts.find(c => 
      c.status === 'Active' || c.status === 'Pending'
    )

    hasActiveRegistration.value = !!(activeApp || activeContract)

    if (hasActiveRegistration.value) {
      if (activeContract) {
        message.info('Bạn đã có hợp đồng phòng. Xem thông tin tại "Phòng của tôi"')
      } else if (activeApp) {
        message.info('Bạn đã có đơn đăng ký đang chờ xử lý')
      }
    }
  } catch (error) {
    console.error('Error checking registration:', error)
  }
}

async function loadData() {
  loading.value = true
  try {
    const [buildingsData, roomTypesData, roomsData] = await Promise.all([
      buildingService.getAll(),
      roomTypeService.getAll(),
      roomService.getAll()
    ])

    buildings.value = buildingsData.map(b => b.name)
    roomTypes.value = roomTypesData.map(rt => rt.name)
    
    rooms.value = roomsData
      // Tạm bỏ filter để hiển thị tất cả phòng
      // .filter(r => r.status === 'Available' && r.currentOccupants < r.maxOccupants)
      .map(room => {
        const roomType = roomTypesData.find(rt => rt.id === room.roomTypeId)
        const building = buildingsData.find(b => b.id === room.buildingId)
        
        return {
          id: room.id,
          name: room.roomNumber,
          building: building?.name || 'N/A',
          buildingId: room.buildingId,
          capacity: room.maxOccupants,
          area: roomType?.area || 25,
          price: roomType?.pricePerMonth || 0,
          available: Math.max(0, room.maxOccupants - room.currentOccupants),
          facilities: getFacilities(roomType),
          image: roomType?.thumbnailUrl || 'https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=400&h=300&fit=crop',
          roomTypeId: room.roomTypeId,
          roomTypeName: roomType?.name || 'N/A',
          status: room.status
        }
      })
      .sort((a, b) => b.available - a.available) // Ưu tiên phòng có nhiều chỗ trống
  } catch (error) {
    console.error('Error loading data:', error)
    message.error('Không thể tải danh sách phòng')
  } finally {
    loading.value = false
  }
}

function getFacilities(roomType) {
  if (!roomType) return 'Tiện nghi cơ bản'
  
  const facilities = []
  if (roomType.hasAirConditioner) facilities.push('Điều hòa')
  if (roomType.hasWaterHeater) facilities.push('Nóng lạnh')
  if (roomType.hasPrivateBathroom) facilities.push('WC riêng')
  
  return facilities.length > 0 ? facilities.join(', ') : 'Tiện nghi cơ bản'
}

const filteredRooms = computed(() => {
  let result = rooms.value

  if (filters.value.building) {
    result = result.filter(r => r.building === filters.value.building)
  }

  if (filters.value.roomType) {
    result = result.filter(r => r.roomTypeName === filters.value.roomType)
  }

  if (filters.value.priceRange) {
    result = result.filter(r => {
      if (filters.value.priceRange === 'Dưới 500k') return r.price < 500000
      if (filters.value.priceRange === '500k - 1tr') return r.price >= 500000 && r.price < 1000000
      if (filters.value.priceRange === '1tr - 1.5tr') return r.price >= 1000000 && r.price < 1500000
      if (filters.value.priceRange === 'Trên 1.5tr') return r.price >= 1500000
      return true
    })
  }

  return result
})

function resetFilters() {
  filters.value = {
    building: undefined,
    roomType: undefined,
    priceRange: undefined
  }
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

function openRegistrationDialog(room) {
  // Kiểm tra nếu sinh viên đã có đơn hoặc hợp đồng active
  if (hasActiveRegistration.value) {
    message.warning('Bạn đã có đơn đăng ký hoặc hợp đồng. Vui lòng kiểm tra "Phòng của tôi" hoặc "Hợp đồng của tôi"')
    return
  }

  if (room.available === 0) {
    message.warning('Phòng này đã hết chỗ trống')
    return
  }
  
  // Lưu thông tin phòng đã chọn và redirect đến trang đăng ký
  localStorage.setItem('selectedRoom', JSON.stringify(room))
  router.push('/student/room-registration')
}
</script>

<style scoped>
.room-card {
  transition: all 0.3s ease;
  height: 100%;
}

.room-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

:deep(.ant-card-body) {
  padding: 16px;
}

:deep(.ant-card-meta-title) {
  margin-bottom: 0 !important;
}
</style>
