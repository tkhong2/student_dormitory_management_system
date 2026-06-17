<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Sơ đồ tầng</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Xem trực quan trạng thái phòng theo từng tầng</p>
    </div>

    <!-- Filters and Floor Map Card -->
    <a-card :bordered="false" :loading="loading">
      <a-row :gutter="[16, 16]" align="middle" style="margin-bottom: 24px;">
        <a-col :xs="24" :sm="8">
          <a-form-item label="Tòa nhà" style="margin-bottom: 0;">
            <a-select 
              v-model:value="selectedBuildingId" 
              placeholder="Chọn tòa nhà"
            >
              <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
                {{ b.name }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :xs="24" :sm="8">
          <a-form-item label="Tầng" style="margin-bottom: 0;">
            <a-select 
              v-model:value="selectedFloorId" 
              placeholder="Chọn tầng"
              :disabled="floors.length === 0"
            >
              <a-select-option v-for="f in floors" :key="f.id" :value="f.id">
                {{ f.label || `Tầng ${f.floorNumber}` }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :xs="24" :sm="8" style="display: flex; gap: 16px; justify-content: flex-end; flex-wrap: wrap;">
          <span v-for="l in legend" :key="l.label" style="display: flex; align-items: center; gap: 6px; font-size: 13px;">
            <span :style="{ width: '12px', height: '12px', borderRadius: '3px', background: l.color }" />
            {{ l.label }}
          </span>
        </a-col>
      </a-row>

      <!-- Statistics -->
      <a-row :gutter="16" style="margin-bottom: 24px;">
        <a-col :span="6">
          <a-statistic 
            title="Tổng phòng" 
            :value="floorStats.total" 
            :value-style="{ color: '#1890ff' }"
          >
            <template #prefix>
              <HomeOutlined />
            </template>
          </a-statistic>
        </a-col>
        <a-col :span="6">
          <a-statistic 
            title="Phòng trống" 
            :value="floorStats.available" 
            :value-style="{ color: '#16a34a' }"
          >
            <template #prefix>
              <CheckCircleOutlined />
            </template>
          </a-statistic>
        </a-col>
        <a-col :span="6">
          <a-statistic 
            title="Đang sử dụng" 
            :value="floorStats.occupied" 
            :value-style="{ color: '#0ea5e9' }"
          >
            <template #prefix>
              <UserOutlined />
            </template>
          </a-statistic>
        </a-col>
        <a-col :span="6">
          <a-statistic 
            title="Đã đầy" 
            :value="floorStats.full" 
            :value-style="{ color: '#dc2626' }"
          >
            <template #prefix>
              <StopOutlined />
            </template>
          </a-statistic>
        </a-col>
      </a-row>

      <!-- Floor Grid -->
      <div class="floor-grid">
        <div
          v-for="r in floorRooms"
          :key="r.id"
          class="floor-room"
          :style="{ 
            background: getRoomColor(r) + '18', 
            borderColor: getRoomColor(r),
            cursor: 'pointer'
          }"
          @click="openDetail(r)"
        >
          <div style="font-size: 16px; font-weight: 700;" :style="{ color: getRoomColor(r) }">
            {{ r.roomNumber }}
          </div>
          <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
            {{ r.currentOccupants }}/{{ r.maxOccupants }}
          </div>
          <a-tag 
            :color="getStatusTagColor(r.status)" 
            style="margin-top: 6px; font-size: 10px; padding: 0 6px;"
          >
            {{ getStatusLabel(r.status) }}
          </a-tag>
        </div>
      </div>

      <!-- Empty State -->
      <a-empty 
        v-if="floorRooms.length === 0" 
        description="Không có phòng nào ở tầng này"
        style="padding: 60px 0;"
      />
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDlg"
      :title="'Phòng ' + (selectedRoom?.roomNumber || '')"
      width="500px"
      @cancel="detailDlg = false"
      :footer="null"
    >
      <a-descriptions v-if="selectedRoom" :column="1" bordered size="small">
        <a-descriptions-item label="Tòa nhà">
          {{ getBuildingName(selectedRoom.buildingId) }}
        </a-descriptions-item>
        <a-descriptions-item label="Tầng">
          {{ getFloorLabel(selectedRoom.floorId) }}
        </a-descriptions-item>
        <a-descriptions-item label="Số phòng">
          {{ selectedRoom.roomNumber }}
        </a-descriptions-item>
        <a-descriptions-item label="Loại phòng">
          {{ getRoomTypeName(selectedRoom.roomTypeId) }}
        </a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="getStatusTagColor(selectedRoom.status)">
            {{ getStatusLabel(selectedRoom.status) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Số người">
          {{ selectedRoom.currentOccupants }}/{{ selectedRoom.maxOccupants }}
        </a-descriptions-item>
        <a-descriptions-item label="Còn trống">
          {{ selectedRoom.maxOccupants - selectedRoom.currentOccupants }} chỗ
        </a-descriptions-item>
        <a-descriptions-item label="Hướng" v-if="selectedRoom.orientation">
          {{ selectedRoom.orientation }}
        </a-descriptions-item>
        <a-descriptions-item label="Ghi chú" v-if="selectedRoom.notes">
          {{ selectedRoom.notes }}
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { 
  HomeOutlined, 
  CheckCircleOutlined, 
  UserOutlined, 
  StopOutlined 
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { buildingService } from '@/services/buildingService'
import { roomService } from '@/services/roomService'
import { roomTypeService } from '@/services/roomTypeService'
import { floorService } from '@/services/floorService'

const loading = ref(false)
const buildings = ref([])
const rooms = ref([])
const roomTypes = ref([])
const floors = ref([])
const selectedBuildingId = ref(null)
const selectedFloorId = ref(null)
const detailDlg = ref(false)
const selectedRoom = ref(null)

const legend = [
  { label: 'Đầy', color: '#52c41a' },
  { label: 'Trống', color: '#ff4d4f' },
  { label: 'Đang ở', color: '#0ea5e9' },
  { label: 'Bảo trì', color: '#fa8c16' },
]

onMounted(async () => {
  await loadData()
})

// Watch building change to load floors
watch(selectedBuildingId, async (newBuildingId) => {
  if (newBuildingId) {
    await loadFloors(newBuildingId)
  }
})

async function loadData() {
  loading.value = true
  try {
    const [buildingsData, roomsData, roomTypesData] = await Promise.all([
      buildingService.getAll(),
      roomService.getAll(),
      roomTypeService.getAll()
    ])

    buildings.value = buildingsData
    rooms.value = roomsData
    roomTypes.value = roomTypesData

    if (buildings.value.length > 0) {
      selectedBuildingId.value = buildings.value[0].id
    }
  } catch (error) {
    console.error('Error loading data:', error)
    message.error('Không thể tải dữ liệu')
  } finally {
    loading.value = false
  }
}

async function loadFloors(buildingId) {
  try {
    const floorsData = await floorService.getByBuildingId(buildingId)
    floors.value = floorsData.sort((a, b) => a.floorNumber - b.floorNumber)
    
    // Auto select first floor
    if (floors.value.length > 0) {
      selectedFloorId.value = floors.value[0].id
    } else {
      selectedFloorId.value = null
    }
  } catch (error) {
    console.error('Error loading floors:', error)
    message.error('Không thể tải danh sách tầng')
    floors.value = []
    selectedFloorId.value = null
  }
}

const floorRooms = computed(() => {
  if (!selectedFloorId.value) return []
  
  return rooms.value.filter(room => {
    return room.floorId === selectedFloorId.value
  })
})

const floorStats = computed(() => {
  const total = floorRooms.value.length
  const available = floorRooms.value.filter(r => r.currentOccupants === 0).length
  const occupied = floorRooms.value.filter(r => r.currentOccupants > 0 && r.currentOccupants < r.maxOccupants).length
  const full = floorRooms.value.filter(r => r.currentOccupants >= r.maxOccupants).length
  
  return { total, available, occupied, full }
})

function getRoomColor(room) {
  if (room.status === 'Maintenance') return '#fa8c16'
  if (room.currentOccupants === 0) return '#ff4d4f'
  if (room.currentOccupants >= room.maxOccupants) return '#52c41a'
  return '#0ea5e9'
}

function getStatusTagColor(status) {
  const map = {
    'Available': 'green',
    'Occupied': 'blue',
    'Full': 'red',
    'Maintenance': 'orange',
    'Reserved': 'purple'
  }
  return map[status] || 'default'
}

function getStatusLabel(status) {
  const map = {
    'Available': 'Trống',
    'Occupied': 'Đang ở',
    'Full': 'Đầy',
    'Maintenance': 'Bảo trì',
    'Reserved': 'Đặt trước'
  }
  return map[status] || status
}

function getBuildingName(buildingId) {
  const building = buildings.value.find(b => b.id === buildingId)
  return building?.name || 'N/A'
}

function getRoomTypeName(roomTypeId) {
  const roomType = roomTypes.value.find(rt => rt.id === roomTypeId)
  return roomType?.name || 'N/A'
}

function getFloorLabel(floorId) {
  const floor = floors.value.find(f => f.id === floorId)
  return floor?.label || `Tầng ${floor?.floorNumber || 'N/A'}`
}

function openDetail(room) {
  selectedRoom.value = room
  detailDlg.value = true
}
</script>

<style scoped>
.floor-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 12px;
}

.floor-room {
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border: 2px solid;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.floor-room:hover {
  transform: scale(1.06);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
}

:deep(.ant-form-item) {
  margin-bottom: 0;
}

:deep(.ant-form-item-label) {
  padding-bottom: 4px;
}

:deep(.ant-statistic-title) {
  font-size: 13px;
  margin-bottom: 4px;
}

:deep(.ant-statistic-content) {
  font-size: 20px;
  font-weight: 700;
}
</style>
