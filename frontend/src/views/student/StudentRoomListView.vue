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

    <!-- Room List Table -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="filteredRooms"
        :loading="loading"
        :pagination="{
          current: pagination.current,
          pageSize: pagination.pageSize,
          total: filteredRooms.length,
          showTotal: (total) => `Tổng ${total} phòng`,
          showSizeChanger: true,
          pageSizeOptions: ['8', '12', '16', '24'],
          onChange: (page, pageSize) => {
            pagination.current = page
            pagination.pageSize = pageSize
          }
        }"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'roomNumber'">
            <div style="display: flex; align-items: center; gap: 12px;">
              <img 
                :src="record.image" 
                :alt="record.name"
                style="width: 60px; height: 60px; object-fit: cover; border-radius: 8px;"
              />
              <div>
                <div style="font-weight: 600; font-size: 15px;">{{ record.name }}</div>
                <div style="font-size: 12px; color: #8c8c8c;">{{ record.roomTypeName }}</div>
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'building'">
            <a-tag color="blue">{{ record.building }}</a-tag>
          </template>

          <template v-else-if="column.key === 'capacity'">
            <div style="display: flex; align-items: center; gap: 6px;">
              <UserOutlined style="color: #1890ff;" />
              <span>{{ record.capacity }} người</span>
            </div>
          </template>

          <template v-else-if="column.key === 'area'">
            <div style="display: flex; align-items: center; gap: 6px;">
              <ExpandOutlined style="color: #1890ff;" />
              <span>{{ record.area }} m²</span>
            </div>
          </template>

          <template v-else-if="column.key === 'facilities'">
            <a-tooltip :title="record.facilities">
              <div style="max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
                <ThunderboltOutlined style="color: #1890ff; margin-right: 6px;" />
                {{ record.facilities }}
              </div>
            </a-tooltip>
          </template>

          <template v-else-if="column.key === 'price'">
            <div style="font-weight: 700; color: #ff9800; font-size: 15px;">
              {{ formatPrice(record.price) }}
            </div>
          </template>

          <template v-else-if="column.key === 'available'">
            <a-tag 
              v-if="record.available > 0" 
              color="success"
              style="font-weight: 600;"
            >
              {{ record.available }} chỗ trống
            </a-tag>
            <a-tag 
              v-else 
              color="error"
              style="font-weight: 600;"
            >
              Đã đầy
            </a-tag>
          </template>

          <template v-else-if="column.key === 'action'">
            <a-button 
              type="primary" 
              size="small"
              :disabled="record.available === 0"
              @click="openRegistrationDialog(record)"
              style="background: #ff9800; border-color: #ff9800; color: white;"
            >
              Đăng ký
            </a-button>
          </template>
        </template>

        <template #emptyText>
          <a-empty description="Không tìm thấy phòng phù hợp">
            <a-button type="primary" @click="resetFilters" style="background: #ff9800; border-color: #ff9800; color: white;">
              Xem tất cả phòng
            </a-button>
          </a-empty>
        </template>
      </a-table>
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
import axios from 'axios'
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

const pagination = ref({
  current: 1,
  pageSize: 8
})

const columns = [
  {
    title: 'Phòng',
    key: 'roomNumber',
    width: 220,
    fixed: 'left'
  },
  {
    title: 'Tòa nhà',
    key: 'building',
    width: 120
  },
  {
    title: 'Sức chứa',
    key: 'capacity',
    width: 120
  },
  {
    title: 'Diện tích',
    key: 'area',
    width: 110
  },
  {
    title: 'Tiện nghi',
    key: 'facilities',
    width: 200
  },
  {
    title: 'Giá/tháng',
    key: 'price',
    width: 140,
    sorter: (a, b) => a.price - b.price
  },
  {
    title: 'Trạng thái',
    key: 'available',
    width: 130,
    sorter: (a, b) => a.available - b.available
  },
  {
    title: 'Thao tác',
    key: 'action',
    width: 110,
    fixed: 'right'
  }
]

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
    
    // Fetch amenities for each room type
    const roomTypeAmenitiesMap = {}
    for (const roomType of roomTypesData) {
      try {
        const amenitiesResponse = await axios.get(
          `http://localhost:5003/api/roomtypes/${roomType.id}/amenities`,
          {
            headers: {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
          }
        )
        roomTypeAmenitiesMap[roomType.id] = amenitiesResponse.data || []
      } catch (error) {
        console.error(`Error fetching amenities for room type ${roomType.id}:`, error)
        // If API fails, use fallback from boolean fields
        roomTypeAmenitiesMap[roomType.id] = []
      }
    }
    
    rooms.value = roomsData
      .filter(room => {
        // Chỉ hiển thị phòng Available (còn chỗ) và Maintenance (bảo trì)
        // Loại bỏ phòng Full (đầy) và các trạng thái khác
        const available = room.maxOccupants - room.currentOccupants
        return (available > 0 && room.status === 'Available') || room.status === 'Maintenance'
      })
      .map(room => {
        const roomType = roomTypesData.find(rt => rt.id === room.roomTypeId)
        const building = buildingsData.find(b => b.id === room.buildingId)
        
        // Get amenities for this room type from the map
        const roomTypeAmenities = roomTypeAmenitiesMap[room.roomTypeId] || []
        const amenityNames = roomTypeAmenities.map(a => a.name)
        
        // Add basic amenities from roomType boolean fields if not already in list
        if (roomType?.hasAirConditioner && !amenityNames.includes('Điều hòa')) {
          amenityNames.push('Điều hòa')
        }
        if (roomType?.hasWaterHeater && !amenityNames.includes('Nóng lạnh')) {
          amenityNames.push('Nóng lạnh')
        }
        if (roomType?.hasPrivateBathroom && !amenityNames.includes('WC riêng')) {
          amenityNames.push('WC riêng')
        }
        
        return {
          id: room.id,
          name: room.roomNumber,
          building: building?.name || 'N/A',
          buildingId: room.buildingId,
          capacity: room.maxOccupants,
          area: roomType?.area || 25,
          price: roomType?.pricePerMonth || 0,
          available: Math.max(0, room.maxOccupants - room.currentOccupants),
          facilities: amenityNames.length > 0 ? amenityNames.join(', ') : 'Tiện nghi cơ bản',
          amenities: amenityNames, // Array of amenity names
          image: roomType?.thumbnailUrl || 'https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=400&h=300&fit=crop',
          roomTypeId: room.roomTypeId,
          roomTypeName: roomType?.name || 'N/A',
          status: room.status
        }
      })
      .sort((a, b) => b.available - a.available)
  } catch (error) {
    console.error('Error loading rooms:', error)
    message.error('Không thể tải danh sách phòng')
  } finally {
    loading.value = false
  }
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
  pagination.value.current = 1
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
:deep(.ant-table) {
  font-size: 14px;
}

:deep(.ant-table-thead > tr > th) {
  background: #fafafa;
  font-weight: 600;
}

:deep(.ant-table-tbody > tr:hover) {
  background: #f5f5f5;
}

:deep(.ant-table-cell) {
  padding: 12px 16px;
}
</style>
