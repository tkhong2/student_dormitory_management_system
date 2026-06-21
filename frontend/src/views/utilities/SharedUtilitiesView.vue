<template>
  <div>
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Quản lý Tiện ích chung
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý máy giặt, máy sấy, phòng tập và các tiện ích chung của tòa nhà
        </p>
      </div>
      <a-button type="primary" @click="showCreateModal" style="background: #ff9800; border-color: #ff9800;">
        + Thêm Tiện ích
      </a-button>
    </div>

    <!-- Filters -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-input-search
            v-model:value="searchText"
            placeholder="Tìm tên, mã tiện ích..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select v-model:value="filters.buildingId" placeholder="Tất cả tòa nhà" allow-clear style="width: 100%" @change="handleSearch">
            <a-select-option :value="null">Tất cả tòa nhà</a-select-option>
            <a-select-option v-for="building in buildings" :key="building.id" :value="building.id">
              {{ building.name }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select v-model:value="filters.category" placeholder="Tất cả loại" allow-clear style="width: 100%" @change="handleSearch">
            <a-select-option :value="null">Tất cả loại</a-select-option>
            <a-select-option v-for="cat in categories" :key="cat.value" :value="cat.value">
              {{ cat.icon }} {{ cat.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select v-model:value="filters.status" placeholder="Tất cả trạng thái" allow-clear style="width: 100%" @change="handleSearch">
            <a-select-option :value="null">Tất cả trạng thái</a-select-option>
            <a-select-option v-for="st in statuses" :key="st.value" :value="st.value">
              {{ st.label }}
            </a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Statistics and Table -->
    <a-card :bordered="false" :loading="loading">
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: <strong>{{ filteredUtilities.length }}</strong> tiện ích —  
          <span style="color: #16a34a">{{ countByStatus('Available') }} sẵn sàng</span> ·
          <span style="color: #2563eb">{{ countByStatus('InUse') }} đang dùng</span> ·
          <span style="color: #dc2626">{{ countByStatus('OutOfOrder') }} hỏng</span> ·
          <span style="color: #d97706">{{ countByStatus('Maintenance') }} bảo trì</span>
        </p>
      </div>

      <a-table 
        :dataSource="filteredUtilities" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 15, showTotal: (total) => `Tổng ${total} tiện ích` }"
        :scroll="{ x: 1400 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'name'">
            <div>
              <strong><i :class="getCategoryFaIcon(record.category)"></i> {{ record.name }}</strong>
              <div style="font-size: 11px; color: #8c8c8c;">{{ record.utilityId }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'building'">
            {{ record.buildingName || '-' }}
          </template>

          <template v-else-if="column.key === 'category'">
            <a-tag color="blue">{{ getCategoryLabel(record.category) }}</a-tag>
          </template>

          <template v-else-if="column.key === 'location'">
            {{ record.location || '-' }}
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusLabel(record.status) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'fee'">
            <span v-if="record.feePerUse" style="font-weight: 500; color: #16a34a">
              {{ formatCurrency(record.feePerUse) }}
            </span>
            <span v-else style="color: #8c8c8c">Miễn phí</span>
          </template>

          <template v-else-if="column.key === 'usage'">
            <a-tag color="processing">{{ record.totalUsageCount || 0 }} lần</a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="viewDetail(record)">Chi tiết</a-button>
              <a-button size="small" @click="editUtility(record)">Sửa</a-button>
              <a-button size="small" @click="viewEvents(record)">Sự kiện</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="modalVisible"
      :title="isEdit ? 'Sửa Tiện ích' : 'Thêm Tiện ích mới'"
      width="800px"
      @ok="handleSubmit"
      @cancel="resetForm"
    >
      <a-form :model="form" layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Tòa nhà" required>
              <a-select v-model:value="form.buildingId">
                <a-select-option v-for="building in buildings" :key="building.id" :value="building.id">
                  {{ building.name }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Loại tiện ích" required>
              <a-select v-model:value="form.category">
                <a-select-option v-for="cat in categories" :key="cat.value" :value="cat.value">
                  {{ cat.icon }} {{ cat.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tên tiện ích" required>
              <a-input v-model:value="form.name" placeholder="Máy giặt tầng 1" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Mã tiện ích" required>
              <a-input v-model:value="form.utilityId" placeholder="WM-A-01" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Thương hiệu">
              <a-input v-model:value="form.brand" placeholder="Samsung, LG..." />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Trạng thái">
              <a-select v-model:value="form.status">
                <a-select-option v-for="st in statuses" :key="st.value" :value="st.value">
                  {{ st.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item label="Vị trí">
              <a-input v-model:value="form.location" placeholder="Tầng 1, phòng giặt" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Phí sử dụng (VNĐ)">
              <a-input-number v-model:value="form.feePerUse" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="16">
            <a-form-item label="Giờ hoạt động">
              <a-input v-model:value="form.operatingHours" placeholder="6:00 - 22:00" />
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item label="Mô tả">
              <a-textarea v-model:value="form.description" :rows="2" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Người quản lý">
              <a-input v-model:value="form.managerName" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Điện thoại">
              <a-input v-model:value="form.managerPhone" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Email">
              <a-input v-model:value="form.managerEmail" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailModalVisible"
      title="Chi tiết Tiện ích"
      width="900px"
      :footer="null"
    >
      <a-descriptions v-if="selectedUtility" :column="2" bordered>
        <a-descriptions-item label="Tên">{{ selectedUtility.name }}</a-descriptions-item>
        <a-descriptions-item label="Mã">{{ selectedUtility.utilityId }}</a-descriptions-item>
        <a-descriptions-item label="Loại">{{ getCategoryLabel(selectedUtility.category) }}</a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="getStatusColor(selectedUtility.status)">{{ getStatusLabel(selectedUtility.status) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Tòa nhà">{{ selectedUtility.buildingName }}</a-descriptions-item>
        <a-descriptions-item label="Vị trí">{{ selectedUtility.location }}</a-descriptions-item>
        <a-descriptions-item label="Thương hiệu">{{ selectedUtility.brand || '-' }}</a-descriptions-item>
        <a-descriptions-item label="Phí sử dụng">
          {{ selectedUtility.feePerUse ? formatCurrency(selectedUtility.feePerUse) : 'Miễn phí' }}
        </a-descriptions-item>
        <a-descriptions-item label="Giờ hoạt động" :span="2">{{ selectedUtility.operatingHours || '-' }}</a-descriptions-item>
        <a-descriptions-item label="Người quản lý">{{ selectedUtility.managerName || '-' }}</a-descriptions-item>
        <a-descriptions-item label="Liên hệ">
          {{ selectedUtility.managerPhone || '-' }}
          <br />
          {{ selectedUtility.managerEmail || '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Lượt sử dụng">{{ selectedUtility.totalUsageCount }} lần</a-descriptions-item>
        <a-descriptions-item label="Lần sử dụng cuối">
          {{ selectedUtility.lastUsedAt ? formatDate(selectedUtility.lastUsedAt) : 'Chưa sử dụng' }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày mua">
          {{ selectedUtility.purchaseDate ? formatDate(selectedUtility.purchaseDate) : '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày lắp đặt">
          {{ selectedUtility.installationDate ? formatDate(selectedUtility.installationDate) : '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Bảo trì gần nhất">
          {{ selectedUtility.lastMaintenanceDate ? formatDate(selectedUtility.lastMaintenanceDate) : '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Bảo trì tiếp theo">
          {{ selectedUtility.nextMaintenanceDate ? formatDate(selectedUtility.nextMaintenanceDate) : '-' }}
        </a-descriptions-item>
        <a-descriptions-item label="Mô tả" :span="2">{{ selectedUtility.description || '-' }}</a-descriptions-item>
      </a-descriptions>
    </a-modal>

    <!-- Events Modal with Tabs -->
    <a-modal
      v-model:open="eventsModalVisible"
      :title="`Bảo trì & Sự kiện: ${selectedUtility?.name}`"
      width="1200px"
      :footer="null"
    >
      <a-tabs v-model:activeKey="activeEventTab">
        <!-- Tab 1: Sự kiện bảo trì -->
        <a-tab-pane key="events" tab="📅 Sự kiện bảo trì">
          <a-button type="primary" @click="showCreateEventModal" style="margin-bottom: 16px">
            <i class="fas fa-plus"></i> Thêm sự kiện
          </a-button>
          
          <a-table :columns="eventColumns" :data-source="utilityEvents" :loading="eventsLoading" rowKey="id" :pagination="{ pageSize: 10 }">
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'eventType'">
                <a-tag :color="getEventTypeColor(record.eventType)">{{ getEventTypeLabel(record.eventType) }}</a-tag>
              </template>
              <template v-else-if="column.key === 'status'">
                <a-tag :color="getEventStatusColor(record.status)">{{ getEventStatusLabel(record.status) }}</a-tag>
              </template>
              <template v-else-if="column.key === 'eventDate'">
                {{ formatDate(record.eventDate) }}
              </template>
              <template v-else-if="column.key === 'cost'">
                {{ record.estimatedCost ? formatCurrency(record.estimatedCost) : '-' }}
              </template>
              <template v-else-if="column.key === 'actions'">
                <a-space>
                  <a-button size="small" @click="editEvent(record)">Sửa</a-button>
                  <a-button size="small" danger @click="deleteEvent(record.id)">Xóa</a-button>
                </a-space>
              </template>
            </template>
          </a-table>
        </a-tab-pane>

        <!-- Tab 2: Yêu cầu sửa chữa -->
        <a-tab-pane key="maintenance" tab="🔧 Yêu cầu sửa chữa">
          <a-alert 
            message="Danh sách yêu cầu sửa chữa liên quan đến tiện ích này" 
            type="info" 
            show-icon 
            style="margin-bottom: 16px"
          />
          
          <a-table 
            :columns="maintenanceColumns" 
            :data-source="maintenanceRequests" 
            :loading="maintenanceLoading" 
            rowKey="id"
            :pagination="{ pageSize: 10 }"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'student'">
                <div>
                  <strong>{{ record.studentName || 'N/A' }}</strong>
                  <div style="font-size: 12px; color: #8c8c8c;">{{ record.roomNumber || '-' }}</div>
                </div>
              </template>
              <template v-else-if="column.key === 'issue'">
                <div>
                  <div style="font-weight: 500;">{{ record.issueType || 'Khác' }}</div>
                  <div style="font-size: 12px; color: #666;">{{ record.description }}</div>
                </div>
              </template>
              <template v-else-if="column.key === 'priority'">
                <a-tag :color="record.priority === 'High' ? 'red' : record.priority === 'Medium' ? 'orange' : 'blue'">
                  {{ record.priority === 'High' ? 'Cao' : record.priority === 'Medium' ? 'Trung bình' : 'Thấp' }}
                </a-tag>
              </template>
              <template v-else-if="column.key === 'status'">
                <a-tag :color="getMaintenanceStatusColor(record.status)">
                  {{ getMaintenanceStatusLabel(record.status) }}
                </a-tag>
              </template>
              <template v-else-if="column.key === 'createdAt'">
                {{ formatDate(record.createdAt) }}
              </template>
              <template v-else-if="column.key === 'actions'">
                <a-button size="small" @click="viewMaintenanceDetail(record)">Xem</a-button>
              </template>
            </template>
          </a-table>
        </a-tab-pane>
      </a-tabs>
    </a-modal>

    <!-- Event Create/Edit Modal -->
    <a-modal
      v-model:open="eventModalVisible"
      title="Thêm Sự kiện"
      @ok="handleEventSubmit"
      @cancel="resetEventForm"
    >
      <a-form :model="eventForm" layout="vertical">
        <a-form-item label="Loại sự kiện" required>
          <a-select v-model:value="eventForm.eventType">
            <a-select-option v-for="type in eventTypes" :key="type.value" :value="type.value">
              {{ type.label }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Tiêu đề">
          <a-input v-model:value="eventForm.title" />
        </a-form-item>
        <a-form-item label="Mô tả">
          <a-textarea v-model:value="eventForm.description" :rows="3" />
        </a-form-item>
        <a-form-item label="Ngày thực hiện" required>
          <a-date-picker v-model:value="eventForm.eventDate" show-time style="width: 100%" />
        </a-form-item>
        <a-form-item label="Chi phí ước tính">
          <a-input-number v-model:value="eventForm.estimatedCost" :min="0" style="width: 100%" />
        </a-form-item>
        <a-form-item label="Công ty kỹ thuật">
          <a-input v-model:value="eventForm.technicianCompany" />
        </a-form-item>
        <a-form-item label="Liên hệ kỹ thuật">
          <a-input v-model:value="eventForm.technicianContact" />
        </a-form-item>
        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="eventForm.notes" :rows="2" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { SearchOutlined } from '@ant-design/icons-vue'
import * as utilityService from '@/services/sharedUtilityService'
import { buildingService } from '@/services/buildingService'
import dayjs from 'dayjs'

const loading = ref(false)
const utilities = ref([])
const buildings = ref([])
const searchText = ref('')
const filters = ref({
  buildingId: null,
  category: null,
  status: null
})

const categories = utilityService.UTILITY_CATEGORIES
const statuses = utilityService.UTILITY_STATUSES
const eventTypes = utilityService.EVENT_TYPES
const eventStatuses = utilityService.EVENT_STATUSES

// Modals
const modalVisible = ref(false)
const detailModalVisible = ref(false)
const eventsModalVisible = ref(false)
const eventModalVisible = ref(false)
const isEdit = ref(false)
const selectedUtility = ref(null)

// Table columns
const columns = [
  { title: 'Tên tiện ích', key: 'name', width: 220, fixed: 'left' },
  { title: 'Tòa nhà', key: 'building', width: 130 },
  { title: 'Loại', key: 'category', width: 120 },
  { title: 'Vị trí', key: 'location', width: 150 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Phí sử dụng', key: 'fee', width: 120 },
  { title: 'Lượt dùng', key: 'usage', width: 100 },
  { title: 'Thao tác', key: 'actions', width: 280, fixed: 'right' }
]

// Forms
const form = ref({
  buildingId: null,
  name: '',
  category: 'WashingMachine',
  brand: '',
  utilityId: '',
  status: 'Available',
  location: '',
  managerName: '',
  managerPhone: '',
  managerEmail: '',
  description: '',
  feePerUse: null,
  operatingHours: ''
})

const eventForm = ref({
  sharedUtilityId: null,
  eventType: 'Maintenance',
  title: '',
  description: '',
  eventDate: null,
  estimatedCost: null,
  technicianCompany: '',
  technicianContact: '',
  notes: ''
})

// Events
const utilityEvents = ref([])
const eventsLoading = ref(false)
const activeEventTab = ref('events')
const maintenanceRequests = ref([])
const maintenanceLoading = ref(false)
const eventColumns = [
  { title: 'Loại', dataIndex: 'eventType', key: 'eventType', width: 120 },
  { title: 'Tiêu đề', dataIndex: 'title', key: 'title', width: 200 },
  { title: 'Ngày', dataIndex: 'eventDate', key: 'eventDate', width: 150 },
  { title: 'Trạng thái', dataIndex: 'status', key: 'status', width: 120 },
  { title: 'Chi phí ƯT', key: 'cost', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150 }
]

const maintenanceColumns = [
  { title: 'Sinh viên / Phòng', key: 'student', width: 150 },
  { title: 'Vấn đề', key: 'issue', width: 250 },
  { title: 'Độ ưu tiên', key: 'priority', width: 110 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Ngày tạo', key: 'createdAt', width: 150 },
  { title: 'Thao tác', key: 'actions', width: 100 }
]

const filteredUtilities = computed(() => {
  let result = utilities.value

  if (filters.value.buildingId) {
    result = result.filter(u => u.buildingId === filters.value.buildingId)
  }

  if (filters.value.category) {
    result = result.filter(u => u.category === filters.value.category)
  }

  if (filters.value.status) {
    result = result.filter(u => u.status === filters.value.status)
  }

  if (searchText.value) {
    const search = searchText.value.toLowerCase()
    result = result.filter(u => 
      u.name.toLowerCase().includes(search) ||
      u.utilityId.toLowerCase().includes(search) ||
      u.brand?.toLowerCase().includes(search)
    )
  }

  return result
})

function handleSearch() {
  // Trigger computed property re-evaluation
}

function countByStatus(status) {
  return filteredUtilities.value.filter(u => u.status === status).length
}

onMounted(async () => {
  await loadBuildings()
  await loadUtilities()
})

const loadBuildings = async () => {
  try {
    const result = await buildingService.getAll()
    console.log('Buildings response:', result)
    // buildingService.getAll() trả về data trực tiếp từ api.js interceptor
    buildings.value = Array.isArray(result) ? result : []
  } catch (error) {
    message.error('Lỗi tải danh sách tòa nhà')
    console.error('Building load error:', error)
  }
}

const loadUtilities = async () => {
  loading.value = true
  try {
    utilities.value = await utilityService.getAllUtilities()
  } catch (error) {
    message.error('Lỗi tải danh sách tiện ích')
  } finally {
    loading.value = false
  }
}

const showCreateModal = () => {
  isEdit.value = false
  resetForm()
  modalVisible.value = true
}

const editUtility = (utility) => {
  isEdit.value = true
  form.value = { ...utility }
  modalVisible.value = true
}

const handleSubmit = async () => {
  try {
    if (isEdit.value) {
      await utilityService.updateUtility(form.value.id, form.value)
      message.success('Cập nhật thành công')
    } else {
      await utilityService.createUtility(form.value)
      message.success('Thêm tiện ích thành công')
    }
    modalVisible.value = false
    await loadUtilities()
  } catch (error) {
    message.error(error.message || 'Có lỗi xảy ra')
  }
}

const confirmDelete = (utility) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: `Bạn có chắc muốn xóa tiện ích "${utility.name}"?`,
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    onOk: async () => {
      try {
        await utilityService.deleteUtility(utility.id)
        message.success('Xóa thành công')
        await loadUtilities()
      } catch (error) {
        message.error('Lỗi xóa tiện ích')
      }
    }
  })
}

const viewDetail = (utility) => {
  selectedUtility.value = utility
  detailModalVisible.value = true
}

const viewEvents = async (utility) => {
  selectedUtility.value = utility
  eventsModalVisible.value = true
  activeEventTab.value = 'events'
  await Promise.all([
    loadEvents(utility.id),
    loadMaintenanceRequests(utility.id, utility.name)
  ])
}

const loadEvents = async (utilityId) => {
  eventsLoading.value = true
  try {
    utilityEvents.value = await utilityService.getUtilityEventsByUtility(utilityId)
  } catch (error) {
    message.error('Lỗi tải danh sách sự kiện')
  } finally {
    eventsLoading.value = false
  }
}

const loadMaintenanceRequests = async (utilityId, utilityName) => {
  maintenanceLoading.value = true
  try {
    // Gọi API lấy tất cả maintenance requests
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/maintenancerequests`)
    const allRequests = await response.json()
    
    // Lọc các yêu cầu có liên quan đến tiện ích này (theo tên hoặc category)
    maintenanceRequests.value = allRequests.filter(req => 
      req.description?.toLowerCase().includes(utilityName.toLowerCase()) ||
      req.title?.toLowerCase().includes(utilityName.toLowerCase()) ||
      req.category?.toLowerCase().includes('tiện ích') ||
      req.category?.toLowerCase().includes('utility')
    )
  } catch (error) {
    console.error('Error loading maintenance requests:', error)
    maintenanceRequests.value = []
  } finally {
    maintenanceLoading.value = false
  }
}

const viewMaintenanceDetail = (record) => {
  // TODO: Mở modal xem chi tiết yêu cầu bảo trì
  message.info('Chi tiết yêu cầu: ' + record.title)
}

const getMaintenanceStatusLabel = (status) => {
  const labels = {
    'New': 'Mới',
    'Assigned': 'Đã phân công',
    'InProgress': 'Đang xử lý',
    'Done': 'Hoàn thành',
    'Rejected': 'Từ chối'
  }
  return labels[status] || status
}

const getMaintenanceStatusColor = (status) => {
  const colors = {
    'New': 'blue',
    'Assigned': 'cyan',
    'InProgress': 'orange',
    'Done': 'green',
    'Rejected': 'red'
  }
  return colors[status] || 'default'
}

const showCreateEventModal = () => {
  resetEventForm()
  eventForm.value.sharedUtilityId = selectedUtility.value.id
  eventModalVisible.value = true
}

const handleEventSubmit = async () => {
  try {
    const data = {
      ...eventForm.value,
      eventDate: eventForm.value.eventDate?.toISOString()
    }
    await utilityService.createUtilityEvent(data)
    message.success('Thêm sự kiện thành công')
    eventModalVisible.value = false
    await loadEvents(selectedUtility.value.id)
  } catch (error) {
    message.error('Lỗi thêm sự kiện')
  }
}

const deleteEvent = async (eventId) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: 'Bạn có chắc muốn xóa sự kiện này?',
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    onOk: async () => {
      try {
        await utilityService.deleteUtilityEvent(eventId)
        message.success('Xóa thành công')
        await loadEvents(selectedUtility.value.id)
      } catch (error) {
        message.error('Lỗi xóa sự kiện')
      }
    }
  })
}

const resetForm = () => {
  form.value = {
    buildingId: null,
    name: '',
    category: 'WashingMachine',
    brand: '',
    utilityId: '',
    status: 'Available',
    location: '',
    managerName: '',
    managerPhone: '',
    managerEmail: '',
    description: '',
    feePerUse: null,
    operatingHours: ''
  }
}

const resetEventForm = () => {
  eventForm.value = {
    sharedUtilityId: null,
    eventType: 'Maintenance',
    title: '',
    description: '',
    eventDate: null,
    estimatedCost: null,
    technicianCompany: '',
    technicianContact: '',
    notes: ''
  }
}

const getCategoryIcon = (category) => {
  return categories.find(c => c.value === category)?.icon || '📦'
}

const getCategoryFaIcon = (category) => {
  return categories.find(c => c.value === category)?.faIcon || 'fas fa-box'
}

const getCategoryLabel = (category) => {
  return categories.find(c => c.value === category)?.label || category
}

const getStatusLabel = (status) => {
  return statuses.find(s => s.value === status)?.label || status
}

const getStatusColor = (status) => {
  return statuses.find(s => s.value === status)?.color || 'default'
}

const getEventTypeLabel = (type) => {
  return eventTypes.find(t => t.value === type)?.label || type
}

const getEventTypeColor = (type) => {
  return eventTypes.find(t => t.value === type)?.color || 'default'
}

const getEventStatusLabel = (status) => {
  return eventStatuses.find(s => s.value === status)?.label || status
}

const getEventStatusColor = (status) => {
  return eventStatuses.find(s => s.value === status)?.color || 'default'
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

const formatDate = (date) => {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}
</script>
