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
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm phòng
      </a-button>
    </div>
    
    <DataStatus
      :loading="loading"
      :error="error"
      :items="rooms"
      empty-message="Chưa có phòng nào"
      :show-create-button="true"
      create-button-text="Thêm phòng"
      @retry="loadRooms"
      @create="openCreate"
    >
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: {{ rooms.length }} phòng —  
          <span style="color: #16a34a">{{ countByStatus('Available') }} trống</span> ·
          <span style="color: #2563eb">{{ countByStatus('Occupied') }} đang ở</span> ·
          <span style="color: #dc2626">{{ countByStatus('Full') }} đầy</span> ·
          <span style="color: #d97706">{{ countByStatus('Maintenance') }} bảo trì</span>
        </p>
      </div>
      
      <a-table 
        :dataSource="rooms" 
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
            <a-space>
              <a-button size="small" @click="openEdit(record)">Sửa</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </DataStatus>
    
    <!-- Modal Create/Edit -->
    <a-modal v-model:open="dialog" :title="editTarget ? 'Sửa' : 'Thêm'" @ok="save" @cancel="dialog = false" width="600px">
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
    <a-modal v-model:open="deleteDialog" title="Xóa" @ok="doDelete" @cancel="deleteDialog = false">
      <p>Xóa phòng {{ deleteTarget?.roomNumber }}?</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import { roomService } from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { roomTypeService } from '@/services/roomTypeService'
import { floorService } from '@/services/floorService'
import ImageUpload from '@/components/common/ImageUpload.vue'
import DataStatus from '@/components/common/DataStatus.vue'

const rooms = ref([])
const buildings = ref([])
const roomTypes = ref([])
const floors = ref([])
const loading = ref(false)
const error = ref(null)
const dialog = ref(false)
const deleteDialog = ref(false)
const editTarget = ref(null)
const deleteTarget = ref(null)

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

async function loadRooms() {
  loading.value = true
  error.value = null
  try {
    rooms.value = await roomService.getAll()
  } catch (err) {
    error.value = err.message
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
  return rooms.value.filter(r => r.status === status).length
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
    alert('Vui lòng điền đầy đủ thông tin')
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
    } else {
      await roomService.create(payload)
    }
    dialog.value = false
    await loadRooms()
  } catch (err) {
    alert(err.message)
  }
}

function confirmDelete(record) {
  deleteTarget.value = record
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await roomService.delete(deleteTarget.value.id)
    deleteDialog.value = false
    await loadRooms()
  } catch (err) {
    alert(err.message)
  }
}

onMounted(() => {
  loadRooms()
  loadBuildings()
  loadRoomTypes()
})
</script>
