<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
            Yêu Cầu Sửa Chữa / Bảo Trì
          </h1>
          <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
            Tổng số: {{ requests.length }} yêu cầu
          </p>
        </div>
        <a-button type="primary" @click="showCreateDialog" style="background: #ff9800; border-color: #ff9800;">
          + Tạo Yêu Cầu
        </a-button>
      </div>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm kiếm..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="item in statusOptions" :key="item.value" :value="item.value">
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="categoryFilter"
            placeholder="Danh mục"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="item in categoryOptions" :key="item.value" :value="item.value">
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="priorityFilter"
            placeholder="Độ ưu tiên"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="item in priorityOptions" :key="item.value" :value="item.value">
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="filteredRequests"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ x: 1400 }"
        @change="handleTableChange"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'roomInfo'">
            <div>
              <div style="font-weight: 500">{{ record.roomNumber }}</div>
              <a-typography-text type="secondary" style="font-size: 12px">
                {{ record.buildingName }}
              </a-typography-text>
            </div>
          </template>

          <template v-else-if="column.key === 'title'">
            <div>
              <div style="font-weight: 500">{{ record.title }}</div>
              <a-typography-text type="secondary" style="font-size: 12px">
                {{ record.requestedByStudentName }}
              </a-typography-text>
            </div>
          </template>

          <template v-else-if="column.key === 'category'">
            <a-tag :color="getCategoryColor(record.category)">
              {{ getCategoryText(record.category) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'priority'">
            <a-tag :color="getPriorityColor(record.priority)">
              {{ getPriorityText(record.priority) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'assignedToName'">
            <span v-if="record.assignedToName">{{ record.assignedToName }}</span>
            <a-typography-text v-else type="secondary">Chưa phân công</a-typography-text>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-dropdown>
              <a-button type="text" size="small">
                <template #icon><MoreOutlined /></template>
              </a-button>
              <template #overlay>
                <a-menu @click="handleMenuClick($event, record)">
                  <a-menu-item key="view">
                    <EyeOutlined />
                    <span style="margin-left: 8px">Xem chi tiết</span>
                  </a-menu-item>
                  <a-menu-item key="assign" v-if="record.status === 'New'">
                    <UserAddOutlined />
                    <span style="margin-left: 8px">Phân công</span>
                  </a-menu-item>
                  <a-menu-item key="start" v-if="record.status === 'Assigned'">
                    <PlayCircleOutlined />
                    <span style="margin-left: 8px">Bắt đầu xử lý</span>
                  </a-menu-item>
                  <a-menu-item key="resolve" v-if="record.status === 'InProgress'">
                    <CheckCircleOutlined />
                    <span style="margin-left: 8px">Hoàn thành</span>
                  </a-menu-item>
                  <a-menu-item key="rate" v-if="record.status === 'Done' && !record.rating">
                    <StarOutlined />
                    <span style="margin-left: 8px">Đánh giá</span>
                  </a-menu-item>
                  <a-menu-divider />
                  <a-menu-item key="edit">
                    <EditOutlined />
                    <span style="margin-left: 8px">Sửa</span>
                  </a-menu-item>
                  <a-menu-item key="reject" danger>
                    <CloseCircleOutlined />
                    <span style="margin-left: 8px">Từ chối</span>
                  </a-menu-item>
                  <a-menu-item key="delete" danger>
                    <DeleteOutlined />
                    <span style="margin-left: 8px">Xóa</span>
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editedIndex === -1 ? 'Tạo Yêu Cầu Mới' : 'Sửa Yêu Cầu'"
      width="800px"
      @cancel="closeDialog"
    >
      <a-form :model="editedItem" layout="vertical">
        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="Tiêu Đề" required>
              <a-input v-model:value="editedItem.title" placeholder="Nhập tiêu đề" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Danh Mục" required>
              <a-select v-model:value="editedItem.category" placeholder="Chọn danh mục">
                <a-select-option v-for="item in categoryOptions" :key="item.value" :value="item.value">
                  {{ item.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Độ Ưu Tiên" required>
              <a-select v-model:value="editedItem.priority" placeholder="Chọn độ ưu tiên">
                <a-select-option v-for="item in priorityOptions" :key="item.value" :value="item.value">
                  {{ item.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item label="Mô Tả Chi Tiết" required>
              <a-textarea
                v-model:value="editedItem.description"
                :rows="4"
                placeholder="Mô tả chi tiết vấn đề cần sửa chữa"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số Phòng">
              <a-input v-model:value="editedItem.roomNumber" placeholder="Nhập số phòng" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tòa Nhà">
              <a-input v-model:value="editedItem.buildingName" placeholder="Nhập tên tòa nhà" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
      <template #footer>
        <a-button @click="closeDialog">Hủy</a-button>
        <a-button type="primary" @click="saveRequest">Lưu</a-button>
      </template>
    </a-modal>

    <!-- Assign Modal -->
    <a-modal
      v-model:open="assignDialog"
      title="Phân Công Công Việc"
      width="500px"
      @cancel="closeAssignDialog"
    >
      <a-form layout="vertical">
        <a-form-item label="Phân công cho" required>
          <a-select v-model:value="assignedUserId" placeholder="Chọn người thực hiện" loading-text="Đang tải...">
            <a-select-option
              v-for="staff in staffList"
              :key="staff.id"
              :value="staff.id"
              :label="staff.fullName"
            >
              [ID: {{ staff.id }}] {{ staff.fullName }} - {{ staff.role }}
            </a-select-option>
          </a-select>
          <div style="margin-top: 4px; font-size: 12px; color: #8c8c8c;">
            {{ staffList.length > 0 ? `Có ${staffList.length} nhân viên khả dụng` : 'Đang tải danh sách nhân viên...' }}
          </div>
        </a-form-item>
        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="assignNote" :rows="3" placeholder="Ghi chú cho người thực hiện" />
        </a-form-item>
      </a-form>
      <template #footer>
        <a-button @click="closeAssignDialog">Hủy</a-button>
        <a-button type="primary" @click="confirmAssign">Phân Công</a-button>
      </template>
    </a-modal>

    <!-- Resolve Modal -->
    <a-modal
      v-model:open="resolveDialog"
      title="Hoàn Thành Công Việc"
      width="500px"
      @cancel="closeResolveDialog"
    >
      <a-form layout="vertical">
        <a-form-item label="Mô tả công việc đã thực hiện" required>
          <a-textarea v-model:value="resolutionNote" :rows="4" placeholder="Mô tả chi tiết công việc" />
        </a-form-item>
      </a-form>
      <template #footer>
        <a-button @click="closeResolveDialog">Hủy</a-button>
        <a-button type="primary" @click="confirmResolve">Hoàn Thành</a-button>
      </template>
    </a-modal>

    <!-- Rate Modal -->
    <a-modal
      v-model:open="rateDialog"
      title="Đánh Giá Công Việc"
      width="500px"
      @cancel="closeRateDialog"
    >
      <a-form layout="vertical">
        <a-form-item label="Đánh giá" required>
          <a-rate v-model:value="rating" :count="5" allow-half />
        </a-form-item>
        <a-form-item label="Nhận xét">
          <a-textarea v-model:value="ratingComment" :rows="3" placeholder="Nhận xét về chất lượng công việc" />
        </a-form-item>
      </a-form>
      <template #footer>
        <a-button @click="closeRateDialog">Hủy</a-button>
        <a-button type="primary" @click="confirmRate">Gửi Đánh Giá</a-button>
      </template>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  MoreOutlined,
  UserAddOutlined,
  PlayCircleOutlined,
  CheckCircleOutlined,
  StarOutlined,
  CloseCircleOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import maintenanceRequestService from '@/services/maintenanceRequestService'
import axios from 'axios'

const loading = ref(false)
const requests = ref([])
const search = ref('')
const statusFilter = ref(undefined)
const categoryFilter = ref(undefined)
const priorityFilter = ref(undefined)
const dialog = ref(false)
const editedIndex = ref(-1)
const editedItem = ref({
  title: '',
  description: '',
  category: 'Electric',
  priority: 'Medium',
  roomNumber: '',
  buildingName: ''
})

// Assign dialog
const assignDialog = ref(false)
const selectedRequest = ref(null)
const assignedUserId = ref(undefined)
const assignNote = ref('')
const staffList = ref([])  // Danh sách nhân viên từ API

// Resolve dialog
const resolveDialog = ref(false)
const resolutionNote = ref('')

// Rate dialog
const rateDialog = ref(false)
const rating = ref(0)
const ratingComment = ref('')

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `Tổng ${total} yêu cầu`
})

const columns = [
  {
    title: 'Phòng',
    key: 'roomInfo',
    width: 140,
    fixed: 'left'
  },
  {
    title: 'Tiêu Đề',
    key: 'title',
    width: 220
  },
  {
    title: 'Danh Mục',
    key: 'category',
    width: 120,
    align: 'center'
  },
  {
    title: 'Ưu Tiên',
    key: 'priority',
    width: 120,
    align: 'center'
  },
  {
    title: 'Trạng Thái',
    key: 'status',
    width: 140,
    align: 'center'
  },
  {
    title: 'Phân Công',
    key: 'assignedToName',
    width: 150
  },
  {
    title: 'Thao tác',
    key: 'actions',
    width: 80,
    align: 'center',
    fixed: 'right'
  }
]

const statusOptions = [
  { label: 'Mới', value: 'New' },
  { label: 'Đã phân công', value: 'Assigned' },
  { label: 'Đang xử lý', value: 'InProgress' },
  { label: 'Hoàn thành', value: 'Done' },
  { label: 'Đã hủy', value: 'Cancelled' },
  { label: 'Từ chối', value: 'Rejected' }
]

const categoryOptions = [
  { label: 'Điện', value: 'Electric' },
  { label: 'Nước', value: 'Plumbing' },
  { label: 'Nội thất', value: 'Furniture' },
  { label: 'Mạng', value: 'Network' },
  { label: 'Kết cấu', value: 'Structure' },
  { label: 'Khác', value: 'Other' }
]

const priorityOptions = [
  { label: 'Thấp', value: 'Low' },
  { label: 'Trung bình', value: 'Medium' },
  { label: 'Cao', value: 'High' },
  { label: 'Khẩn cấp', value: 'Urgent' }
]

const filteredRequests = computed(() => {
  let result = requests.value

  if (statusFilter.value) {
    result = result.filter(r => r.status === statusFilter.value)
  }
  if (categoryFilter.value) {
    result = result.filter(r => r.category === categoryFilter.value)
  }
  if (priorityFilter.value) {
    result = result.filter(r => r.priority === priorityFilter.value)
  }

  return result
})

const getCategoryColor = (category) => {
  const colors = {
    'Electric': 'orange',
    'Plumbing': 'blue',
    'Furniture': 'brown',
    'Network': 'purple',
    'Structure': 'red',
    'Other': 'default'
  }
  return colors[category] || 'default'
}

const getCategoryText = (category) => {
  const texts = {
    'Electric': 'Điện',
    'Plumbing': 'Nước',
    'Furniture': 'Nội thất',
    'Network': 'Mạng',
    'Structure': 'Kết cấu',
    'Other': 'Khác'
  }
  return texts[category] || category
}

const getPriorityColor = (priority) => {
  const colors = {
    'Low': 'default',
    'Medium': 'cyan',
    'High': 'orange',
    'Urgent': 'red'
  }
  return colors[priority] || 'default'
}

const getPriorityText = (priority) => {
  const texts = {
    'Low': 'Thấp',
    'Medium': 'Trung bình',
    'High': 'Cao',
    'Urgent': 'Khẩn cấp'
  }
  return texts[priority] || priority
}

const getStatusColor = (status) => {
  const colors = {
    'New': 'blue',
    'Assigned': 'cyan',
    'InProgress': 'gold',
    'Done': 'green',
    'Cancelled': 'default',
    'Rejected': 'red'
  }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    'New': 'Mới',
    'Assigned': 'Đã phân công',
    'InProgress': 'Đang xử lý',
    'Done': 'Hoàn thành',
    'Cancelled': 'Đã hủy',
    'Rejected': 'Từ chối'
  }
  return texts[status] || status
}

const fetchRequests = async () => {
  loading.value = true
  try {
    const data = await maintenanceRequestService.getAll()
    requests.value = data
    pagination.total = data.length
  } catch (error) {
    message.error(error.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const loadStaffList = async () => {
  try {
    const response = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`,
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache'
      }
    })
    
    console.log('MaintenanceRequestList - Raw users from API:', response.data.length)
    console.log('MaintenanceRequestList - All users:', response.data.map(u => ({ id: u.id, name: u.fullName, role: u.role, active: u.isActive })))
    
    // Filter STAFF only (not admin), and only active users
    staffList.value = response.data.filter(u => 
      u.role === 'Staff' && 
      u.isActive === true
    )
    
    console.log('MaintenanceRequestList - Filtered staff list:', staffList.value.length, 'users')
    console.log('MaintenanceRequestList - Staff list details:', staffList.value.map(s => ({ 
      id: s.id, 
      name: s.fullName, 
      role: s.role, 
      email: s.email,
      phone: s.phone 
    })))
    
    if (staffList.value.length === 0) {
      message.warning('Không tìm thấy nhân viên nào trong hệ thống')
    }
  } catch (error) {
    console.error('MaintenanceRequestList - Lỗi tải danh sách nhân viên:', error)
    message.error('Không thể tải danh sách nhân viên')
  }
}

const handleSearch = () => {
  fetchRequests()
}

const handleTableChange = (pag) => {
  pagination.current = pag.current
  pagination.pageSize = pag.pageSize
}

const handleMenuClick = ({ key }, record) => {
  selectedRequest.value = record
  switch (key) {
    case 'view':
      viewRequest(record)
      break
    case 'assign':
      assignRequest(record)
      break
    case 'start':
      startRequest(record)
      break
    case 'resolve':
      resolveRequest(record)
      break
    case 'rate':
      rateRequest(record)
      break
    case 'edit':
      editRequest(record)
      break
    case 'reject':
      rejectRequest(record)
      break
    case 'delete':
      deleteRequest(record)
      break
  }
}

const showCreateDialog = () => {
  editedIndex.value = -1
  editedItem.value = {
    title: '',
    description: '',
    category: 'Electric',
    priority: 'Medium',
    roomNumber: '',
    buildingName: ''
  }
  dialog.value = true
}

const closeDialog = () => {
  dialog.value = false
}

const saveRequest = async () => {
  try {
    if (editedIndex.value === -1) {
      await maintenanceRequestService.create(editedItem.value)
      message.success('Tạo yêu cầu thành công')
    } else {
      await maintenanceRequestService.update(editedItem.value.id, editedItem.value)
      message.success('Cập nhật thành công')
    }
    await fetchRequests()
    closeDialog()
  } catch (error) {
    message.error(error.message || 'Lỗi lưu dữ liệu')
  }
}

const viewRequest = (record) => {
  message.info(`Xem chi tiết: ${record.title}`)
}

const assignRequest = (record) => {
  selectedRequest.value = record
  assignedUserId.value = undefined
  assignNote.value = ''
  assignDialog.value = true
  // Load staff list when opening assign dialog
  if (staffList.value.length === 0) {
    loadStaffList()
  }
}

const closeAssignDialog = () => {
  assignDialog.value = false
  selectedRequest.value = null
}

const confirmAssign = async () => {
  if (!assignedUserId.value) {
    message.warning('Vui lòng chọn người thực hiện')
    return
  }
  
  try {
    const staff = staffList.value.find(s => s.id === assignedUserId.value)
    
    if (!staff) {
      message.error('Không tìm thấy nhân viên được chọn')
      console.error('MaintenanceRequestList - Selected staff ID not found:', assignedUserId.value)
      console.error('MaintenanceRequestList - Available staff IDs:', staffList.value.map(s => s.id))
      return
    }
    
    console.log('MaintenanceRequestList - Assigning to staff:', { 
      id: staff.id, 
      name: staff.fullName, 
      email: staff.email,
      phone: staff.phone 
    })
    
    // Calculate expected completion date (7 days from now)
    const expectedDate = new Date()
    expectedDate.setDate(expectedDate.getDate() + 7)
    
    const payload = {
      assignedToUserId: staff.id,
      assignedToName: staff.fullName,
      expectedCompletionDate: expectedDate.toISOString(),
      estimatedCost: 0
    }
    
    console.log('MaintenanceRequestList - Assignment payload:', payload)
    
    await axios.post(`http://localhost:5002/api/maintenancerequests/${selectedRequest.value.id}/assign`, payload, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`,
        'Content-Type': 'application/json'
      }
    })
    
    message.success('Phân công thành công')
    await fetchRequests()
    closeAssignDialog()
  } catch (error) {
    console.error('MaintenanceRequestList - Error assigning:', error)
    console.error('MaintenanceRequestList - Error response:', error.response?.data)
    message.error(error.response?.data?.message || error.message || 'Lỗi phân công')
  }
}

const startRequest = async (record) => {
  try {
    await maintenanceRequestService.start(record.id)
    message.success('Đã bắt đầu xử lý')
    await fetchRequests()
  } catch (error) {
    message.error(error.message || 'Lỗi bắt đầu xử lý')
  }
}

const resolveRequest = (record) => {
  selectedRequest.value = record
  resolutionNote.value = ''
  resolveDialog.value = true
}

const closeResolveDialog = () => {
  resolveDialog.value = false
  selectedRequest.value = null
}

const confirmResolve = async () => {
  if (!resolutionNote.value) {
    message.warning('Vui lòng mô tả công việc đã thực hiện')
    return
  }
  try {
    await maintenanceRequestService.resolve(selectedRequest.value.id, {
      resolutionNote: resolutionNote.value
    })
    message.success('Hoàn thành công việc')
    await fetchRequests()
    closeResolveDialog()
  } catch (error) {
    message.error(error.message || 'Lỗi hoàn thành')
  }
}

const rateRequest = (record) => {
  selectedRequest.value = record
  rating.value = 0
  ratingComment.value = ''
  rateDialog.value = true
}

const closeRateDialog = () => {
  rateDialog.value = false
  selectedRequest.value = null
}

const confirmRate = async () => {
  if (rating.value === 0) {
    message.warning('Vui lòng chọn số sao đánh giá')
    return
  }
  try {
    await maintenanceRequestService.rate(selectedRequest.value.id, {
      rating: rating.value,
      comment: ratingComment.value
    })
    message.success('Gửi đánh giá thành công')
    await fetchRequests()
    closeRateDialog()
  } catch (error) {
    message.error(error.message || 'Lỗi gửi đánh giá')
  }
}

const editRequest = (record) => {
  editedIndex.value = requests.value.indexOf(record)
  editedItem.value = { ...record }
  dialog.value = true
}

const rejectRequest = (record) => {
  Modal.confirm({
    title: 'Xác nhận từ chối',
    content: `Bạn có chắc muốn từ chối yêu cầu này?`,
    okText: 'Từ Chối',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await maintenanceRequestService.reject(record.id, {
          rejectionReason: 'Không thể thực hiện'
        })
        message.success('Đã từ chối yêu cầu')
        fetchRequests()
      } catch (error) {
        message.error('Lỗi từ chối yêu cầu')
      }
    }
  })
}

const deleteRequest = (record) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: `Bạn có chắc muốn xóa yêu cầu này?`,
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await maintenanceRequestService.delete(record.id)
        message.success('Xóa thành công')
        fetchRequests()
      } catch (error) {
        message.error('Xóa thất bại')
      }
    }
  })
}

onMounted(() => {
  fetchRequests()
  loadStaffList()  // Load staff list on mount
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
