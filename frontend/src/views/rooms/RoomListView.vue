<template>
  <div>
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Quản lý Phòng ở
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý phòng, trạng thái và phân bổ sinh viên
        </p>
      </div>
      <a-space>
        <a-button @click="handleExport" :loading="exporting">
          <template #icon><DownloadOutlined /></template>
          Xuất Excel
        </a-button>
        <a-button v-if="!isReadOnly" type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
          + Thêm phòng
        </a-button>
        <a-tag v-else color="blue" style="padding: 8px 16px; font-size: 13px;">
          <EyeOutlined /> Chỉ xem
        </a-tag>
      </a-space>
    </div>
    
    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm số phòng..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="buildingFilter"
            placeholder="Tòa nhà"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả tòa nhà</a-select-option>
            <a-select-option v-for="building in buildings" :key="building.id" :value="building.id">
              {{ building.name }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả trạng thái</a-select-option>
            <a-select-option value="Available">Trống</a-select-option>
            <a-select-option value="Occupied">Đang ở</a-select-option>
            <a-select-option value="Full">Đầy</a-select-option>
            <a-select-option value="Maintenance">Bảo trì</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="roomTypeFilter"
            placeholder="Loại phòng"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả loại</a-select-option>
            <a-select-option v-for="rt in roomTypes" :key="rt.id" :value="rt.id">
              {{ rt.name }}
            </a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>
    
    <!-- Statistics and Table Card -->
    <a-card :bordered="false" :loading="loading">
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: <strong>{{ filteredRooms.length }}</strong> phòng —  
          <span style="color: #16a34a">{{ countByStatus('Available') }} trống</span> ·
          <span style="color: #2563eb">{{ countByStatus('Occupied') }} đang ở</span> ·
          <span style="color: #dc2626">{{ countByStatus('Full') }} đầy</span> ·
          <span style="color: #d97706">{{ countByStatus('Maintenance') }} bảo trì</span>
        </p>
      </div>
      
      <a-table 
        :dataSource="filteredRooms" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 15, showTotal: (total) => `Tổng ${total} phòng` }"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'roomNumber'">
            <a-tag color="blue">{{ record.roomNumber }}</a-tag>
          </template>
          
          <template v-else-if="column.key === 'building'">
            {{ getBuildingName(record.buildingId) }}
          </template>
          
          <template v-else-if="column.key === 'floor'">
            Tầng {{ record.floorNumber }}
          </template>
          
          <template v-else-if="column.key === 'roomType'">
            {{ getRoomTypeName(record.roomTypeId) }}
          </template>
          
          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusTagColor(record.status)">
              {{ getStatusLabel(record.status) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'occupancy'">
            <a-progress 
              :percent="getCapacityPercent(record)" 
              :stroke-color="getStatusColor(record.status)"
              size="small"
              style="width: 80px"
            />
            <div style="font-size: 11px; color: #8c8c8c">
              {{ record.currentOccupants || 0 }}/{{ getRoomCapacity(record) }}
            </div>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space v-if="!isReadOnly">
              <a-button size="small" @click="openEdit(record)">Sửa</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
            <a-tag v-else color="default">
              <EyeOutlined /> Chi tiết
            </a-tag>
          </template>
        </template>
      </a-table>
    </a-card>
    
    <!-- Modal Create/Edit -->
    <a-modal 
      v-model:open="dialog" 
      :title="editTarget ? 'Sửa phòng' : 'Thêm phòng'" 
      @ok="save" 
      @cancel="dialog = false" 
      width="600px"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Số phòng" required>
          <a-input v-model:value="form.roomNumber" placeholder="Ví dụ: A101" />
        </a-form-item>
        <a-form-item label="Tòa nhà" required>
          <a-select v-model:value="form.buildingId" @change="onBuildingChange">
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
          <div v-if="remainingRoomSlots" style="margin-top: 8px; font-size: 12px;">
            <a-tag v-if="remainingRoomSlots.remaining > 0" color="success">
              Còn có thể thêm {{ remainingRoomSlots.remaining }} phòng 
              ({{ remainingRoomSlots.current }}/{{ remainingRoomSlots.total }})
            </a-tag>
            <a-tag v-else color="error">
              Tòa nhà đã đủ {{ remainingRoomSlots.total }} phòng
            </a-tag>
          </div>
        </a-form-item>
        <a-form-item label="Tầng" required>
          <a-select v-model:value="form.floorId" :disabled="!form.buildingId">
            <a-select-option v-for="f in floors" :key="f.id" :value="f.id">
              {{ f.label }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Loại phòng" required>
          <a-select v-model:value="form.roomTypeId" :disabled="!form.buildingId">
            <a-select-option v-for="rt in filteredRoomTypes" :key="rt.id" :value="rt.id">
              {{ rt.name }} ({{ rt.capacity }} người)
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Trạng thái">
          <a-select v-model:value="form.status">
            <a-select-option value="Available">Trống</a-select-option>
            <a-select-option value="Occupied">Đang ở</a-select-option>
            <a-select-option value="Full">Đầy</a-select-option>
            <a-select-option value="Maintenance">Bảo trì</a-select-option>
            <a-select-option value="Reserved">Đã đặt</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Số người hiện tại">
          <a-input-number v-model:value="form.currentOccupants" :min="0" style="width: 100%" />
        </a-form-item>
        <a-form-item label="Hình ảnh phòng">
          <ImageUpload 
            v-model="form.imageUrl"
            button-text="Chọn ảnh phòng"
            preview-width="100%"
            preview-height="150px"
            placeholder="Hoặc nhập URL hình ảnh"
          />
        </a-form-item>
      </a-form>
    </a-modal>
    
    <!-- Modal Delete -->
    <a-modal 
      v-model:open="deleteDialog" 
      title="Xác nhận xóa" 
      @ok="doDelete" 
      @cancel="deleteDialog = false"
      okText="Xóa"
      cancelText="Hủy"
      ok-button-props="{ danger: true }"
    >
      <p>Bạn có chắc muốn xóa phòng <strong>{{ deleteTarget?.roomNumber }}</strong>? Hành động này không thể hoàn tác.</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { message } from 'ant-design-vue'
import { SearchOutlined, EyeOutlined, DownloadOutlined } from '@ant-design/icons-vue'
import { roomService } from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { roomTypeService } from '@/services/roomTypeService'
import { useExcelExport } from '@/composables/useExcelExport'

const { exporting, exportToExcel } = useExcelExport()
import { floorService } from '@/services/floorService'
import ImageUpload from '@/components/common/ImageUpload.vue'

const route = useRoute()
const isReadOnly = computed(() => route.meta.readonly === true)

const rooms = ref([])
const buildings = ref([])
const roomTypes = ref([])
const floors = ref([])
const loading = ref(false)
const dialog = ref(false)
const deleteDialog = ref(false)
const editTarget = ref(null)
const deleteTarget = ref(null)

// Filter states
const search = ref('')
const buildingFilter = ref('')
const statusFilter = ref('')
const roomTypeFilter = ref('')

const form = ref({
  roomNumber: '',
  buildingId: null,
  floorId: null,
  roomTypeId: null,
  status: 'Available',
  currentOccupants: 0,
  imageUrl: ''
})

const columns = [
  { title: 'Số phòng', key: 'roomNumber', width: 120, fixed: 'left' },
  { title: 'Tòa nhà', key: 'building', width: 150 },
  { title: 'Tầng', key: 'floor', width: 100 },
  { title: 'Loại phòng', key: 'roomType', width: 150 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Lấp đầy', key: 'occupancy', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150, fixed: 'right' }
]

const filteredRoomTypes = computed(() => {
  if (!form.value.buildingId) return []
  return roomTypes.value.filter(rt => rt.buildingId === form.value.buildingId)
})

// Tính số phòng còn có thể thêm cho tòa nhà được chọn
const remainingRoomSlots = computed(() => {
  if (!form.value.buildingId) return null
  const building = buildings.value.find(b => b.id === form.value.buildingId)
  if (!building) return null
  
  const currentRoomCount = rooms.value.filter(r => r.buildingId === form.value.buildingId).length
  const remaining = building.totalRooms - currentRoomCount
  return { current: currentRoomCount, total: building.totalRooms, remaining }
})

// Filtered rooms based on search and filters
const filteredRooms = computed(() => {
  let result = rooms.value

  // Search by room number
  if (search.value) {
    result = result.filter(r => 
      r.roomNumber.toLowerCase().includes(search.value.toLowerCase())
    )
  }

  // Filter by building
  if (buildingFilter.value) {
    result = result.filter(r => r.buildingId === buildingFilter.value)
  }

  // Filter by status
  if (statusFilter.value) {
    result = result.filter(r => r.status === statusFilter.value)
  }

  // Filter by room type
  if (roomTypeFilter.value) {
    result = result.filter(r => r.roomTypeId === roomTypeFilter.value)
  }

  return result
})

function handleSearch() {
  // Trigger computed property re-evaluation
}

async function loadRooms() {
  loading.value = true
  try {
    rooms.value = await roomService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách phòng')
  } finally {
    loading.value = false
  }
}

async function loadBuildings() {
  try {
    buildings.value = await buildingService.getAll()
  } catch (err) {
    console.error(err)
  }
}

async function loadRoomTypes() {
  try {
    roomTypes.value = await roomTypeService.getAll()
  } catch (err) {
    console.error(err)
  }
}

async function onBuildingChange(buildingId) {
  form.value.floorId = null
  form.value.roomTypeId = null
  if (buildingId) {
    try {
      floors.value = await floorService.getByBuildingId(buildingId)
    } catch (err) {
      console.error(err)
      floors.value = []
    }
  } else {
    floors.value = []
  }
}

function countByStatus(status) {
  return filteredRooms.value.filter(r => r.status === status).length
}

function getBuildingName(id) {
  const b = buildings.value.find(b => b.id === id)
  return b ? b.name : '-'
}

function getRoomTypeName(id) {
  const rt = roomTypes.value.find(rt => rt.id === id)
  return rt ? rt.name : '-'
}

function getRoomCapacity(room) {
  const rt = roomTypes.value.find(rt => rt.id === room.roomTypeId)
  return rt ? rt.capacity : 0
}

function getCapacityPercent(room) {
  const cap = getRoomCapacity(room)
  if (!cap) return 0
  return Math.round(((room.currentOccupants || 0) / cap) * 100)
}

function getStatusLabel(s) {
  const map = { Available: 'Trống', Occupied: 'Đang ở', Full: 'Đầy', Maintenance: 'Bảo trì', Reserved: 'Đã đặt' }
  return map[s] || s
}

// Export to Excel function
const handleExport = () => {
  const columnMapping = {
    roomNumber: 'Số phòng',
    buildingName: 'Tòa nhà',
    floorNumber: 'Tầng',
    roomTypeName: 'Loại phòng',
    capacity: 'Sức chứa',
    currentOccupants: 'Đã ở',
    availableSlots: 'Còn trống',
    monthlyRent: 'Giá thuê',
    status: 'Trạng thái',
    notes: 'Ghi chú'
  }
  
  const dataToExport = filteredRooms.value.map(room => ({
    roomNumber: room.roomNumber,
    buildingName: getBuildingName(room.buildingId),
    floorNumber: room.floorNumber,
    roomTypeName: getRoomTypeName(room.roomTypeId),
    capacity: getRoomCapacity(room),
    currentOccupants: room.currentOccupants || 0,
    availableSlots: getRoomCapacity(room) - (room.currentOccupants || 0),
    monthlyRent: room.monthlyRent,
    status: getStatusLabel(room.status),
    notes: room.notes || ''
  }))
  
  exportToExcel(dataToExport, columnMapping, 'Danh_sach_phong', 'Phòng')
}

function getStatusColor(s) {
  const map = { Available: '#16a34a', Occupied: '#2563eb', Full: '#dc2626', Maintenance: '#d97706', Reserved: '#7c3aed' }
  return map[s] || '#9ca3af'
}

function getStatusTagColor(s) {
  const map = { Available: 'success', Occupied: 'processing', Full: 'error', Maintenance: 'warning', Reserved: 'default' }
  return map[s] || 'default'
}

function openCreate() {
  editTarget.value = null
  form.value = { 
    roomNumber: '', 
    buildingId: null, 
    floorId: null, 
    roomTypeId: null, 
    status: 'Available', 
    currentOccupants: 0,
    imageUrl: ''
  }
  floors.value = []
  dialog.value = true
}

async function openEdit(record) {
  editTarget.value = record
  
  // Load floors trước khi set form để đảm bảo dropdown có data
  if (record.buildingId) {
    try {
      floors.value = await floorService.getByBuildingId(record.buildingId)
    } catch (err) {
      console.error('Lỗi load tầng:', err)
      floors.value = []
    }
  }
  
  // Set form value sau khi đã load floors
  form.value = {
    roomNumber: record.roomNumber,
    buildingId: record.buildingId,
    floorId: record.floorId,
    roomTypeId: record.roomTypeId,
    status: record.status,
    currentOccupants: record.currentOccupants || 0,
    imageUrl: record.imageUrl || ''
  }
  
  dialog.value = true
}

async function save() {
  if (!form.value.roomNumber || !form.value.buildingId || !form.value.floorId || !form.value.roomTypeId) {
    message.warning('Vui lòng điền đầy đủ thông tin')
    return
  }
  
  // Kiểm tra số phòng còn có thể thêm (chỉ khi tạo mới, không kiểm tra khi sửa)
  if (!editTarget.value && remainingRoomSlots.value && remainingRoomSlots.value.remaining <= 0) {
    message.error(`Không thể thêm phòng. Tòa nhà đã đủ ${remainingRoomSlots.value.total} phòng`)
    return
  }
  
  try {
    const payload = {
      roomNumber: form.value.roomNumber,
      buildingId: form.value.buildingId,
      floorId: form.value.floorId,
      roomTypeId: form.value.roomTypeId,
      status: form.value.status,
      currentOccupants: form.value.currentOccupants,
      imageUrl: form.value.imageUrl || null
    }
    
    if (editTarget.value) {
      await roomService.update(editTarget.value.id, payload)
      message.success('Cập nhật phòng thành công')
    } else {
      await roomService.create(payload)
      message.success('Thêm phòng thành công')
    }
    dialog.value = false
    await loadRooms()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function confirmDelete(record) {
  deleteTarget.value = record
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await roomService.delete(deleteTarget.value.id)
    message.success('Đã xóa phòng')
    deleteDialog.value = false
    await loadRooms()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

onMounted(() => {
  loadRooms()
  loadBuildings()
  loadRoomTypes()
})
</script>
