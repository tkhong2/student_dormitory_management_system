<template>
  <div>
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Quản lý Tiện nghi
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý tiện nghi và trang thiết bị trong phòng
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm tiện nghi
      </a-button>
    </div>
    
    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="12">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm tên tiện nghi..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="categoryFilter"
            placeholder="Danh mục"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="Electric">Điện tử</a-select-option>
            <a-select-option value="Furniture">Nội thất</a-select-option>
            <a-select-option value="Sanitary">Vệ sinh</a-select-option>
            <a-select-option value="Other">Khác</a-select-option>
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
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="active">Đang dùng</a-select-option>
            <a-select-option value="inactive">Ngừng dùng</a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>
    
    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: <strong>{{ filteredAmenities.length }}</strong> tiện nghi
        </p>
      </div>
      
      <!-- Table -->
      <a-table 
        :dataSource="filteredAmenities" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} tiện nghi` }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'category'">
            <a-tag :color="getCategoryColor(record.category)">
              {{ getCategoryLabel(record.category) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'roomTypeCount'">
            <span style="font-weight: 600; color: #1890ff;">
              {{ record.roomTypeCount || 0 }} loại phòng
            </span>
          </template>
          
          <template v-else-if="column.key === 'totalQuantity'">
            <a-tag color="green" style="font-size: 14px; padding: 4px 12px;">
              {{ record.totalQuantity || 0 }} cái
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'isActive'">
            <a-tag :color="record.isActive ? 'green' : 'default'">
              {{ record.isActive ? 'Đang dùng' : 'Ngừng dùng' }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" type="link" @click="viewDetails(record)">
                <template #icon><EyeOutlined /></template>
                Chi tiết
              </a-button>
              <a-button size="small" @click="openEdit(record)">Sửa</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
    
    <!-- Modal View Details -->
    <a-modal 
      v-model:open="detailDialog" 
      :title="`Chi tiết tiện nghi: ${detailTarget?.name}`"
      width="700px"
      @cancel="detailDialog = false"
      :footer="null"
    >
      <a-spin :spinning="detailLoading">
        <div v-if="roomTypeDetails.length > 0">
          <p style="margin-bottom: 16px; color: #595959;">
            Tiện nghi này đang được sử dụng ở <strong>{{ roomTypeDetails.length }}</strong> loại phòng
          </p>
          
          <a-table 
            :dataSource="roomTypeDetails" 
            :columns="detailColumns"
            :pagination="false"
            size="small"
            :rowKey="r => r.id"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'roomTypeName'">
                <strong>{{ record.roomTypeName }}</strong>
              </template>
              <template v-else-if="column.key === 'quantity'">
                <a-tag color="blue">{{ record.quantity }} cái</a-tag>
              </template>
              <template v-else-if="column.key === 'roomCount'">
                <span style="color: #52c41a; font-weight: 600;">{{ record.roomCount }} phòng</span>
              </template>
            </template>
          </a-table>
          
          <a-divider />
          
          <div style="background: #f0f2f5; padding: 12px; border-radius: 8px;">
            <div style="font-size: 13px; color: #8c8c8c; margin-bottom: 8px;">Tổng quan</div>
            <a-row :gutter="16">
              <a-col :span="12">
                <a-statistic 
                  title="Tổng số loại phòng" 
                  :value="roomTypeDetails.length"
                  :value-style="{ fontSize: '20px', color: '#1890ff' }"
                />
              </a-col>
              <a-col :span="12">
                <a-statistic 
                  title="Tổng số phòng sử dụng" 
                  :value="totalRoomsUsingAmenity"
                  :value-style="{ fontSize: '20px', color: '#52c41a' }"
                />
              </a-col>
            </a-row>
          </div>
        </div>
        <div v-else>
          <a-empty description="Chưa có loại phòng nào sử dụng tiện nghi này" />
        </div>
      </a-spin>
    </a-modal>
    
    <!-- Modal Create/Edit -->
    <a-modal 
      v-model:open="dialog" 
      :title="editTarget ? 'Sửa tiện nghi' : 'Thêm tiện nghi'" 
      @ok="save" 
      @cancel="dialog = false"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Tên">
          <a-input v-model:value="form.name" />
        </a-form-item>
        <a-form-item label="Danh mục">
          <a-select v-model:value="form.category">
            <a-select-option value="Electric">Điện tử</a-select-option>
            <a-select-option value="Furniture">Nội thất</a-select-option>
            <a-select-option value="Sanitary">Vệ sinh</a-select-option>
            <a-select-option value="Other">Khác</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item>
          <a-checkbox v-model:checked="form.isActive">Đang dùng</a-checkbox>
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
      <p>Bạn có chắc muốn xóa tiện nghi <strong>{{ deleteTarget?.name }}</strong>? Hành động này không thể hoàn tác.</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { SearchOutlined, EyeOutlined } from '@ant-design/icons-vue'
import { amenityService } from '@/services/amenityService'
import { roomTypeService } from '@/services/roomTypeService'
import { roomService } from '@/services/roomService'

const amenities = ref([])
const loading = ref(false)
const dialog = ref(false)
const deleteDialog = ref(false)
const detailDialog = ref(false)
const detailLoading = ref(false)
const detailTarget = ref(null)
const roomTypeDetails = ref([])
const totalRoomsUsingAmenity = computed(() => {
  return roomTypeDetails.value.reduce((sum, rt) => sum + (rt.roomCount || 0), 0)
})

const detailColumns = [
  { title: 'Loại phòng', key: 'roomTypeName', dataIndex: 'roomTypeName' },
  { title: 'Số lượng/phòng', key: 'quantity', dataIndex: 'quantity', width: 130 },
  { title: 'Số phòng', key: 'roomCount', dataIndex: 'roomCount', width: 100 },
  { title: 'Ghi chú', dataIndex: 'note', key: 'note' }
]

// Filter states
const search = ref('')
const categoryFilter = ref('')
const statusFilter = ref('')

// Filtered amenities
const filteredAmenities = computed(() => {
  let result = amenities.value

  // Search by name
  if (search.value) {
    result = result.filter(a => 
      a.name.toLowerCase().includes(search.value.toLowerCase())
    )
  }

  // Filter by category
  if (categoryFilter.value) {
    result = result.filter(a => a.category === categoryFilter.value)
  }

  // Filter by status
  if (statusFilter.value === 'active') {
    result = result.filter(a => a.isActive === true)
  } else if (statusFilter.value === 'inactive') {
    result = result.filter(a => a.isActive === false)
  }

  return result
})

function handleSearch() {
  // Trigger computed property re-evaluation
}
const editTarget = ref(null)
const deleteTarget = ref(null)

const form = ref({
  name: '',
  category: 'Electric',
  iconUrl: '',
  isActive: true
})

const columns = [
  { title: 'Tên tiện nghi', dataIndex: 'name', key: 'name', width: 200 },
  { title: 'Danh mục', dataIndex: 'category', key: 'category', width: 120 },
  { title: 'Số loại phòng sử dụng', key: 'roomTypeCount', width: 180 },
  { title: 'Tổng số lượng', key: 'totalQuantity', width: 130 },
  { title: 'Trạng thái', key: 'isActive', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150, fixed: 'right' }
]

async function loadAmenities() {
  loading.value = true
  try {
    const [amenitiesData, roomTypeAmenitiesData] = await Promise.all([
      amenityService.getAll(),
      fetch('http://localhost:5003/api/roomtypeamenities').then(r => r.json()).catch(() => [])
    ])
    
    // Tính toán số loại phòng và tổng số lượng cho mỗi amenity
    amenities.value = amenitiesData.map(amenity => {
      const usages = roomTypeAmenitiesData.filter(rta => rta.amenityId === amenity.id)
      return {
        ...amenity,
        roomTypeCount: usages.length, // Số loại phòng sử dụng
        totalQuantity: usages.reduce((sum, rta) => sum + (rta.quantity || 1), 0) // Tổng số lượng
      }
    })
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách tiện nghi')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editTarget.value = null
  form.value = { name: '', category: 'Electric', iconUrl: '', isActive: true }
  dialog.value = true
}

async function viewDetails(amenity) {
  detailTarget.value = amenity
  detailDialog.value = true
  detailLoading.value = true
  
  try {
    // Get all room type amenities for this amenity
    const roomTypeAmenitiesResponse = await fetch('http://localhost:5003/api/roomtypeamenities')
    const allRoomTypeAmenities = await roomTypeAmenitiesResponse.json()
    
    // Filter for current amenity
    const amenityUsages = allRoomTypeAmenities.filter(rta => rta.amenityId === amenity.id)
    
    if (amenityUsages.length === 0) {
      roomTypeDetails.value = []
      return
    }
    
    // Get all room types and rooms
    const [roomTypes, rooms] = await Promise.all([
      roomTypeService.getAll(),
      roomService.getAll()
    ])
    
    // Build details with room counts
    roomTypeDetails.value = amenityUsages.map(usage => {
      const roomType = roomTypes.find(rt => rt.id === usage.roomTypeId)
      const roomCount = rooms.filter(r => r.roomTypeId === usage.roomTypeId).length
      
      return {
        id: usage.id,
        roomTypeName: roomType ? roomType.name : 'N/A',
        quantity: usage.quantity || 1,
        note: usage.note || '-',
        roomCount
      }
    })
  } catch (err) {
    console.error('Error loading amenity details:', err)
    message.error('Không thể tải chi tiết tiện nghi')
  } finally {
    detailLoading.value = false
  }
}

function openEdit(item) {
  editTarget.value = item
  form.value = { ...item }
  dialog.value = true
}

async function save() {
  try {
    if (editTarget.value) {
      await amenityService.update(editTarget.value.id, form.value)
      message.success('Cập nhật tiện nghi thành công')
    } else {
      await amenityService.create(form.value)
      message.success('Thêm tiện nghi thành công')
    }
    dialog.value = false
    await loadAmenities()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await amenityService.delete(deleteTarget.value.id)
    message.success('Đã xóa tiện nghi')
    deleteDialog.value = false
    await loadAmenities()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function getCategoryLabel(category) {
  const map = { Electric: 'Điện tử', Furniture: 'Nội thất', Sanitary: 'Vệ sinh', Other: 'Khác' }
  return map[category] || category
}

function getCategoryColor(category) {
  const map = { Electric: 'orange', Furniture: 'green', Sanitary: 'blue', Other: 'default' }
  return map[category] || 'default'
}

onMounted(loadAmenities)
</script>
