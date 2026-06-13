<template>
  <div>
    <!-- Header with button on the right -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Quản lý Loại phòng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý các loại phòng và mức giá ký túc xá
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm loại phòng
      </a-button>
    </div>
    
    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm tên loại phòng..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
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
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="active">Hoạt động</a-select-option>
            <a-select-option value="inactive">Ngừng</a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>
    
    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <p style="margin-bottom: 16px">Tổng: <strong>{{ filteredRoomTypes.length }}</strong> loại phòng</p>
      
      <a-table 
        :dataSource="filteredRoomTypes" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} loại phòng` }"
        :scroll="{ x: 1600 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'building'">
            {{ getBuildingName(record.buildingId) }}
          </template>
          
          <template v-else-if="column.key === 'capacity'">
            <a-tag :color="getCapacityColor(record.capacity)">
              {{ record.capacity }} người
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'price'">
            <strong>{{ formatPrice(record.pricePerMonth) }}</strong>/tháng
          </template>
          
          <template v-else-if="column.key === 'deposit'">
            {{ formatPrice(record.depositAmount) }}
          </template>
          
          <template v-else-if="column.key === 'rates'">
            <div style="font-size: 12px">
              <div>Điện: {{ record.electricityRate }}₫/kWh</div>
              <div>Nước: {{ record.waterRate }}₫/m³</div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'amenities'">
            <a-tooltip>
              <template #title>
                <div v-if="record.amenityIds && record.amenityIds.length > 0">
                  <div v-for="amenityId in record.amenityIds" :key="amenityId" style="margin-bottom: 4px;">
                    • {{ getAmenityName(amenityId) }}
                  </div>
                </div>
                <div v-else-if="hasAnyAmenity(record)">
                  <div v-if="record.hasAirConditioner">• Điều hòa</div>
                  <div v-if="record.hasWaterHeater">• Nóng lạnh</div>
                  <div v-if="record.hasPrivateBathroom">• WC riêng</div>
                  <div v-if="record.hasWindowView">• Cửa sổ</div>
                </div>
                <div v-else>Không có tiện nghi</div>
              </template>
              <a-button size="small" type="link">
                <template v-if="record.amenityIds && record.amenityIds.length > 0">
                  {{ record.amenityIds.length }} tiện nghi
                </template>
                <template v-else-if="hasAnyAmenity(record)">
                  {{ countAmenities(record) }} tiện nghi
                </template>
                <template v-else>
                  Không có
                </template>
              </a-button>
            </a-tooltip>
          </template>
          
          <template v-else-if="column.key === 'isActive'">
            <a-tag :color="record.isActive ? 'green' : 'default'">
              {{ record.isActive ? 'Hoạt động' : 'Ngừng' }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="openEdit(record)">Sửa</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
    
    <!-- Modal Create/Edit -->
    <a-modal 
      v-model:open="dialog" 
      :title="editTarget ? 'Sửa loại phòng' : 'Thêm loại phòng'" 
      @ok="save" 
      @cancel="dialog = false" 
      width="700px"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Tòa nhà" required>
          <a-select v-model:value="form.buildingId">
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Mã">
              <a-input v-model:value="form.code" placeholder="Ví dụ: DBL, QUAD" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tên loại phòng" required>
              <a-input v-model:value="form.name" placeholder="Phòng đôi, Phòng 4 người" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="Sức chứa" required>
              <a-input-number v-model:value="form.capacity" :min="1" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Diện tích (m²)">
              <a-input-number v-model:value="form.area" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Loại giường">
              <a-select v-model:value="form.bedType">
                <a-select-option value="Single">Đơn</a-select-option>
                <a-select-option value="Bunk">Tầng</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Giá thuê/tháng (VNĐ)" required>
              <a-input-number v-model:value="form.pricePerMonth" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tiền cọc (VNĐ)">
              <a-input-number v-model:value="form.depositAmount" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Giá điện (VNĐ/kWh)">
              <a-input-number v-model:value="form.electricityRate" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Giá nước (VNĐ/m³)">
              <a-input-number v-model:value="form.waterRate" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="Tiện nghi">
          <a-checkbox-group v-model:value="form.amenityIds">
            <a-row>
              <a-col v-for="amenity in amenities" :key="amenity.id" :span="12">
                <a-checkbox :value="amenity.id">
                  {{ amenity.name }}
                </a-checkbox>
              </a-col>
            </a-row>
          </a-checkbox-group>
        </a-form-item>
        <a-form-item label="Mô tả">
          <a-textarea v-model:value="form.description" :rows="3" />
        </a-form-item>
        <a-form-item>
          <a-checkbox v-model:checked="form.isActive">Hoạt động</a-checkbox>
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
      <p>Bạn có chắc muốn xóa loại phòng <strong>{{ deleteTarget?.name }}</strong>? Hành động này không thể hoàn tác.</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined } from '@ant-design/icons-vue'
import { roomTypeService } from '@/services/roomTypeService'
import { buildingService } from '@/services/buildingService'
import { amenityService } from '@/services/amenityService'

const roomTypes = ref([])
const buildings = ref([])
const amenities = ref([])
const loading = ref(false)
const dialog = ref(false)
const deleteDialog = ref(false)
const editTarget = ref(null)
const deleteTarget = ref(null)

// Filter states
const search = ref('')
const buildingFilter = ref('')
const statusFilter = ref('')

const form = ref({
  buildingId: null,
  code: '',
  name: '',
  capacity: 4,
  pricePerMonth: 500000,
  depositAmount: 0,
  electricityRate: 0,
  waterRate: 0,
  area: 20,
  bedType: 'Single',
  amenityIds: [],
  description: '',
  isActive: true
})

// Filtered room types
const filteredRoomTypes = computed(() => {
  let result = roomTypes.value

  // Search by name
  if (search.value) {
    result = result.filter(rt => 
      rt.name.toLowerCase().includes(search.value.toLowerCase()) ||
      (rt.code && rt.code.toLowerCase().includes(search.value.toLowerCase()))
    )
  }

  // Filter by building
  if (buildingFilter.value) {
    result = result.filter(rt => rt.buildingId === buildingFilter.value)
  }

  // Filter by status
  if (statusFilter.value === 'active') {
    result = result.filter(rt => rt.isActive === true)
  } else if (statusFilter.value === 'inactive') {
    result = result.filter(rt => rt.isActive === false)
  }

  return result
})

function handleSearch() {
  // Trigger computed property re-evaluation
}

const columns = [
  { title: 'Tên', dataIndex: 'name', key: 'name', width: 180, fixed: 'left' },
  { title: 'Tòa nhà', key: 'building', width: 150 },
  { title: 'Mã', dataIndex: 'code', key: 'code', width: 100 },
  { title: 'Sức chứa', key: 'capacity', width: 100 },
  { title: 'Diện tích', dataIndex: 'area', key: 'area', width: 100 },
  { title: 'Giá thuê', key: 'price', width: 150 },
  { title: 'Tiền cọc', key: 'deposit', width: 130 },
  { title: 'Giá điện/nước', key: 'rates', width: 130 },
  { title: 'Tiện nghi', key: 'amenities', width: 120 },
  { title: 'Trạng thái', key: 'isActive', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150, fixed: 'right' }
]

async function loadRoomTypes() {
  loading.value = true
  try {
    roomTypes.value = await roomTypeService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách loại phòng')
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

async function loadAmenities() {
  try {
    amenities.value = await amenityService.getActives()
  } catch (err) {
    console.error(err)
  }
}

function getAmenityName(amenityId) {
  const amenity = amenities.value.find(a => a.id === amenityId)
  return amenity ? amenity.name : '?'
}

function getAmenityColor(amenityId) {
  const colors = ['cyan', 'orange', 'blue', 'green', 'purple', 'magenta', 'geekblue', 'volcano']
  // Dùng amenityId để chọn màu consistent
  return colors[amenityId % colors.length]
}

function getBuildingName(id) {
  const b = buildings.value.find(b => b.id === id)
  return b ? b.name : '-'
}

function formatPrice(price) {
  return new Intl.NumberFormat('vi-VN').format(price) + '₫'
}

function getCapacityColor(capacity) {
  if (capacity <= 2) return 'orange'
  if (capacity <= 4) return 'blue'
  return 'purple'
}

function hasAnyAmenity(record) {
  return record.hasAirConditioner || record.hasWaterHeater || 
         record.hasPrivateBathroom || record.hasWindowView
}

function countAmenities(record) {
  let count = 0
  if (record.hasAirConditioner) count++
  if (record.hasWaterHeater) count++
  if (record.hasPrivateBathroom) count++
  if (record.hasWindowView) count++
  return count
}

function openCreate() {
  editTarget.value = null
  form.value = {
    buildingId: null,
    code: '',
    name: '',
    capacity: 4,
    pricePerMonth: 500000,
    depositAmount: 0,
    electricityRate: 0,
    waterRate: 0,
    area: 20,
    bedType: 'Single',
    amenityIds: [],
    description: '',
    isActive: true
  }
  dialog.value = true
}

function openEdit(record) {
  editTarget.value = record
  
  // Sử dụng amenityIds từ backend nếu có, ngược lại map từ boolean fields
  let amenityIds = []
  
  if (record.amenityIds && record.amenityIds.length > 0) {
    // Backend đã trả về amenityIds từ RoomTypeAmenity table
    amenityIds = record.amenityIds
  } else {
    // Backward compatibility: map từ boolean fields
    const amenityMap = {
      hasAirConditioner: amenities.value.find(a => a.name === 'Điều hòa')?.id,
      hasWaterHeater: amenities.value.find(a => a.name === 'Nóng lạnh')?.id,
      hasPrivateBathroom: amenities.value.find(a => a.name === 'WC riêng')?.id,
      hasWindowView: amenities.value.find(a => a.name === 'Cửa sổ')?.id
    }
    
    if (record.hasAirConditioner && amenityMap.hasAirConditioner) {
      amenityIds.push(amenityMap.hasAirConditioner)
    }
    if (record.hasWaterHeater && amenityMap.hasWaterHeater) {
      amenityIds.push(amenityMap.hasWaterHeater)
    }
    if (record.hasPrivateBathroom && amenityMap.hasPrivateBathroom) {
      amenityIds.push(amenityMap.hasPrivateBathroom)
    }
    if (record.hasWindowView && amenityMap.hasWindowView) {
      amenityIds.push(amenityMap.hasWindowView)
    }
  }
  
  form.value = {
    buildingId: record.buildingId,
    code: record.code || '',
    name: record.name,
    capacity: record.capacity,
    pricePerMonth: record.pricePerMonth,
    depositAmount: record.depositAmount,
    electricityRate: record.electricityRate,
    waterRate: record.waterRate,
    area: record.area,
    bedType: record.bedType || 'Single',
    amenityIds,
    description: record.description || '',
    isActive: record.isActive
  }
  dialog.value = true
}

async function save() {
  if (!form.value.name || !form.value.buildingId) {
    message.warning('Vui lòng điền đầy đủ thông tin')
    return
  }
  try {
    // Map amenity IDs back to boolean fields for backward compatibility
    const amenityMap = {
      hasAirConditioner: amenities.value.find(a => a.name === 'Điều hòa')?.id,
      hasWaterHeater: amenities.value.find(a => a.name === 'Nóng lạnh')?.id,
      hasPrivateBathroom: amenities.value.find(a => a.name === 'WC riêng')?.id,
      hasWindowView: amenities.value.find(a => a.name === 'Cửa sổ')?.id
    }
    
    const payload = {
      ...form.value,
      hasAirConditioner: form.value.amenityIds.includes(amenityMap.hasAirConditioner),
      hasWaterHeater: form.value.amenityIds.includes(amenityMap.hasWaterHeater),
      hasPrivateBathroom: form.value.amenityIds.includes(amenityMap.hasPrivateBathroom),
      hasWindowView: form.value.amenityIds.includes(amenityMap.hasWindowView),
      amenityIds: form.value.amenityIds // Gửi amenityIds để backend lưu vào RoomTypeAmenity
    }
    
    if (editTarget.value) {
      await roomTypeService.update(editTarget.value.id, payload)
      message.success('Cập nhật loại phòng thành công')
    } else {
      await roomTypeService.create(payload)
      message.success('Thêm loại phòng thành công')
    }
    dialog.value = false
    await loadRoomTypes()
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
    await roomTypeService.delete(deleteTarget.value.id)
    message.success('Đã xóa loại phòng')
    deleteDialog.value = false
    await loadRoomTypes()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

onMounted(() => {
  loadRoomTypes()
  loadBuildings()
  loadAmenities()
})
</script>
