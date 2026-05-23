<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 16px; flex-wrap: wrap; gap: 12px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Quản lý Phòng ở</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Tổng <strong>{{ rooms.length }}</strong> phòng —
          <span style="color: #16a34a;">{{ countByStatus('Available') }} trống</span> ·
          <span style="color: #2563eb;">{{ countByStatus('Occupied') }} đang ở</span> ·
          <span style="color: #dc2626;">{{ countByStatus('Full') }} đã đầy</span> ·
          <span style="color: #d97706;">{{ countByStatus('Maintenance') }} bảo trì</span>
        </p>
      </div>
    </div>

    <!-- Filters -->
    <a-card style="margin-bottom: 16px; background: #fafafa;">
      <a-row :gutter="16">
        <a-col :span="6">
          <a-select
            v-model:value="filterBuilding"
            placeholder="Tòa nhà"
            style="width: 100%"
            :loading="loadingBuildings"
          >
            <a-select-option value="all">Tất cả tòa nhà</a-select-option>
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-select v-model:value="filterStatus" placeholder="Trạng thái" style="width: 100%">
            <a-select-option value="all">Tất cả trạng thái</a-select-option>
            <a-select-option value="Available">Trống</a-select-option>
            <a-select-option value="Occupied">Đang ở</a-select-option>
            <a-select-option value="Full">Đã đầy</a-select-option>
            <a-select-option value="Maintenance">Bảo trì</a-select-option>
            <a-select-option value="Reserved">Đã đặt</a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-select v-model:value="filterFloor" placeholder="Tầng" style="width: 100%">
            <a-select-option value="all">Tất cả tầng</a-select-option>
            <a-select-option v-for="f in floorOptions" :key="f" :value="f">Tầng {{ f }}</a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-input v-model:value="searchText" placeholder="Tìm số phòng..." allow-clear />
        </a-col>
      </a-row>
    </a-card>

    <!-- Loading -->
    <div v-if="loading" style="display: flex; justify-content: center; padding: 60px 0;">
      <a-spin size="large" tip="Đang tải dữ liệu..." />
    </div>

    <!-- Error -->
    <a-alert
      v-else-if="error"
      type="error"
      :message="error"
      show-icon
      style="margin-bottom: 16px;"
    >
      <template #action>
        <a-button size="small" @click="loadRooms">Thử lại</a-button>
      </template>
    </a-alert>

    <!-- Room Grid -->
    <template v-else>
      <!-- Empty -->
      <div v-if="filteredRooms.length === 0" style="text-align:center; padding: 60px 0; color: #9ca3af;">
        <HomeOutlined style="font-size: 48px;" />
        <p style="margin-top: 12px; font-size: 15px;">Không tìm thấy phòng nào</p>
      </div>

      <div style="display: flex; flex-wrap: wrap; margin: -6px;">
        <div v-for="r in filteredRooms" :key="r.id" style="width: 20%; padding: 6px;">
          <a-card
            hoverable
            @click="selectRoom(r)"
            style="text-align: center; border-radius: 12px; overflow: hidden; padding: 0;"
            :body-style="{ padding: '16px 12px 14px' }"
          >
            <!-- Status bar on top -->
            <div :style="{ height: '4px', background: getStatusColor(r.status), marginBottom: '12px', marginTop: '-16px', marginLeft: '-12px', marginRight: '-12px' }" />
            
            <!-- Image / Icon -->
            <div v-if="r.imageUrl" :style="{ width:'100%', height:'120px', borderRadius:'8px', overflow:'hidden', marginBottom:'12px', marginTop:'-4px', background:'#f5f5f5' }">
              <img :src="r.imageUrl" style="width:100%; height:100%; object-fit:cover;" />
            </div>
            <div v-else :style="{ width:'44px', height:'44px', background: getStatusBg(r.status), borderRadius:'10px', display:'flex', alignItems:'center', justifyContent:'center', margin:'0 auto 10px' }">
              <HomeOutlined :style="{ fontSize:'22px', color: getStatusColor(r.status) }" />
            </div>
            <div style="font-weight: 700; font-size: 16px; margin-bottom: 2px;">{{ r.roomNumber }}</div>
            <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px;">
              {{ getBuildingName(r.buildingId) }} · Tầng {{ r.floor }}
            </div>
            <a-tag :color="getStatusTagColor(r.status)" style="margin-bottom: 8px; font-size: 11px;">
              {{ getStatusLabel(r.status) }}
            </a-tag>
            <a-progress
              :percent="getCapacityPercent(r)"
              :stroke-color="getStatusColor(r.status)"
              size="small"
            />
            <div style="font-size: 11px; color: #8c8c8c; margin-top: 4px;">
              {{ r.currentOccupancy }}/{{ getRoomCapacity(r) }} người
            </div>
          </a-card>
        </div>
      </div>
    </template>

    <!-- Room Detail Drawer -->
    <a-drawer
      v-model:open="detailDrawer"
      title="Chi tiết phòng"
      placement="right"
      width="400"
    >
      <div v-if="selectedRoom">
        <!-- Room Image Cover in Drawer -->
        <div v-if="selectedRoom.imageUrl" style="margin: -24px -24px 20px -24px; height: 160px; overflow: hidden;">
          <img :src="selectedRoom.imageUrl" style="width: 100%; height: 100%; object-fit: cover;" />
        </div>
        
        <a-tag :color="getStatusTagColor(selectedRoom.status)" style="margin-bottom: 16px;">
          {{ getStatusLabel(selectedRoom.status) }}
        </a-tag>
        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="Số phòng">{{ selectedRoom.roomNumber }}</a-descriptions-item>
          <a-descriptions-item label="Tòa nhà">{{ getBuildingName(selectedRoom.buildingId) }}</a-descriptions-item>
          <a-descriptions-item label="Tầng">Tầng {{ selectedRoom.floor }}</a-descriptions-item>
          <a-descriptions-item label="Loại phòng">{{ getRoomTypeName(selectedRoom.roomTypeId) }}</a-descriptions-item>
          <a-descriptions-item label="Sức chứa">
            {{ selectedRoom.currentOccupancy }}/{{ getRoomCapacity(selectedRoom) }} sinh viên
          </a-descriptions-item>
        </a-descriptions>

        <a-divider />

        <h4 style="font-weight: 700; margin-bottom: 12px;">Sinh viên trong phòng</h4>
        <div v-if="selectedRoom.currentOccupancy > 0">
          <div
            v-for="i in selectedRoom.currentOccupancy"
            :key="i"
            style="display: flex; align-items: center; gap: 12px; margin-bottom: 12px;"
          >
            <a-avatar :style="{ backgroundColor: '#ff6b00' }">
              <template #icon><UserOutlined /></template>
            </a-avatar>
            <div>
              <div style="font-weight: 600;">Sinh viên {{ i }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">SV00{{ i }}</div>
            </div>
          </div>
        </div>
        <a-empty v-else description="Chưa có sinh viên" />
      </div>
    </a-drawer>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { HomeOutlined, UserOutlined } from '@ant-design/icons-vue'
import { roomService }     from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { roomTypeService } from '@/services/roomTypeService'

// ─── State ────────────────────────────────────────────────────────────────────
const rooms        = ref([])
const buildings    = ref([])
const roomTypes    = ref([])
const loading      = ref(false)
const loadingBuildings = ref(false)
const error        = ref(null)

const filterBuilding = ref('all')
const filterStatus   = ref('all')
const filterFloor    = ref('all')
const searchText     = ref('')

const detailDrawer = ref(false)
const selectedRoom = ref(null)

// ─── Computed ─────────────────────────────────────────────────────────────────
const floorOptions = computed(() => {
  const floors = [...new Set(rooms.value.map(r => r.floor))].sort((a, b) => a - b)
  return floors
})

const filteredRooms = computed(() => {
  return rooms.value.filter(r => {
    const buildingMatch = filterBuilding.value === 'all' || r.buildingId === filterBuilding.value
    const statusMatch   = filterStatus.value   === 'all' || r.status     === filterStatus.value
    const floorMatch    = filterFloor.value    === 'all' || r.floor      === filterFloor.value
    const searchMatch   = !searchText.value || r.roomNumber.toLowerCase().includes(searchText.value.toLowerCase())
    return buildingMatch && statusMatch && floorMatch && searchMatch
  })
})

// ─── Load data ────────────────────────────────────────────────────────────────
async function loadRooms() {
  loading.value = true
  error.value   = null
  try {
    rooms.value = await roomService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách phòng'
  } finally {
    loading.value = false
  }
}

async function loadBuildings() {
  loadingBuildings.value = true
  try {
    buildings.value = await buildingService.getAll()
  } catch (_) {
    // silent fail
  } finally {
    loadingBuildings.value = false
  }
}

async function loadRoomTypes() {
  try {
    roomTypes.value = await roomTypeService.getAll()
  } catch (_) {
    // silent fail
  }
}

onMounted(() => {
  loadRooms()
  loadBuildings()
  loadRoomTypes()
})

// ─── Helpers ──────────────────────────────────────────────────────────────────
function countByStatus(status) {
  return rooms.value.filter(r => r.status === status).length
}

function getBuildingName(buildingId) {
  const b = buildings.value.find(b => b.id === buildingId)
  return b ? b.name : buildingId?.slice(0, 8) + '...'
}

function getRoomTypeName(roomTypeId) {
  const rt = roomTypes.value.find(rt => rt.id === roomTypeId)
  return rt ? rt.name : 'Không rõ'
}

function getRoomCapacity(room) {
  const rt = roomTypes.value.find(rt => rt.id === room.roomTypeId)
  return rt ? rt.capacity : '?'
}

function getCapacityPercent(room) {
  const cap = getRoomCapacity(room)
  if (!cap || cap === '?') return 0
  return Math.round((room.currentOccupancy / cap) * 100)
}

function selectRoom(r) {
  selectedRoom.value = r
  detailDrawer.value = true
}

const statusMap = {
  Available:   { label: 'Trống',    color: '#16a34a', bg: '#f0fdf4', tag: 'success'    },
  Occupied:    { label: 'Đang ở',   color: '#2563eb', bg: '#eff6ff', tag: 'processing' },
  Full:        { label: 'Đã đầy',   color: '#dc2626', bg: '#fff1f2', tag: 'error'      },
  Maintenance: { label: 'Bảo trì',  color: '#d97706', bg: '#fffbeb', tag: 'warning'    },
  Reserved:    { label: 'Đã đặt',   color: '#7c3aed', bg: '#f5f3ff', tag: 'default'    },
}

function getStatusLabel(s)    { return statusMap[s]?.label    || s }
function getStatusColor(s)    { return statusMap[s]?.color    || '#9ca3af' }
function getStatusBg(s)       { return statusMap[s]?.bg       || '#f9fafb' }
function getStatusTagColor(s) { return statusMap[s]?.tag      || 'default' }
</script>
