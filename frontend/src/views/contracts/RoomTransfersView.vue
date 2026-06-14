<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Yêu cầu Chuyển phòng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Xử lý yêu cầu chuyển phòng từ sinh viên
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Tạo yêu cầu
      </a-button>
    </div>

    <!-- Statistics Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px">
      <a-col :xs="12" :sm="12" :md="6">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Chờ duyệt"
            :value="countStatus('Pending')"
            :value-style="{ color: '#faad14' }"
          >
            <template #prefix>
              <ClockCircleOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :sm="12" :md="6">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Đã duyệt"
            :value="countStatus('Approved')"
            :value-style="{ color: '#52c41a' }"
          >
            <template #prefix>
              <CheckCircleOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :sm="12" :md="6">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Từ chối"
            :value="countStatus('Rejected')"
            :value-style="{ color: '#ff4d4f' }"
          >
            <template #prefix>
              <CloseCircleOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :sm="12" :md="6">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Hoàn thành"
            :value="countStatus('Completed')"
            :value-style="{ color: '#1890ff' }"
          >
            <template #prefix>
              <SwapOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="16">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên, mã SV..."
            allowClear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            style="width: 100%"
          >
            <a-select-option
              v-for="option in filterStatusOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-button
            v-if="search || statusFilter !== 'all'"
            @click="resetFilters"
          >
            Đặt lại
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
        :columns="columns"
        :data-source="filteredTransfers"
        row-key="id"
        :pagination="{ pageSize: 10 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div style="display: flex; align-items: center; gap: 12px">
              <a-avatar size="large" style="background: #e6f7ff; color: #1890ff">
                {{ record.studentName?.charAt(0) || 'S' }}
              </a-avatar>
              <div>
                <div style="font-weight: 600">{{ record.studentName }}</div>
                <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'rooms'">
            <div style="text-align: center">
              <div style="font-weight: 500">{{ record.currentRoomNumber }}</div>
              <ArrowRightOutlined style="color: #1890ff; margin: 4px 0" />
              <div style="font-weight: 600; color: #4caf50">
                {{ record.newRoomNumber || 'Chưa phân' }}
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'requestedRoom'">
            <div v-if="record.requestedRoomTypeName || record.requestedBuildingName">
              <div>{{ record.requestedRoomTypeName || 'Chưa chỉ định' }}</div>
              <div style="font-size: 12px; color: #8c8c8c">
                {{ record.requestedBuildingName || 'Chưa chỉ định' }}
              </div>
            </div>
            <div v-else style="color: #8c8c8c">Không yêu cầu cụ thể</div>
          </template>
          <template v-else-if="column.key === 'createdAt'">
            {{ formatDate(record.createdAt) }}
          </template>
          <template v-else-if="column.key === 'status'">
            <a-tag :color="statusColor(record.status)">{{ statusLabel(record.status) }}</a-tag>
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space size="small">
              <a-button
                v-if="record.status === 'Pending'"
                type="link"
                @click="openApprove(record)"
              >
                Duyệt
              </a-button>
              <a-button
                v-if="record.status === 'Pending'"
                type="text"
                danger
                @click="openReject(record)"
              >
                Từ chối
              </a-button>
              <a-button type="link" @click="viewDetail(record)">Chi tiết</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create Dialog -->
    <a-modal
      v-model:open="createDialog"
      title="Tạo yêu cầu chuyển phòng"
      :confirm-loading="saving"
      :maskClosable="false"
      @ok="handleCreate"
      @cancel="createDialog = false"
      okText="Tạo yêu cầu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item
          label="Hợp đồng"
          :validate-status="createErrors.contractId ? 'error' : ''"
          :help="createErrors.contractId"
        >
          <a-select
            v-model:value="createForm.contractId"
            placeholder="Chọn hợp đồng"
            @change="onContractChange"
          >
            <a-select-option
              v-for="contract in activeContracts"
              :key="contract.id"
              :value="contract.id"
            >
              {{ contract.displayText }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Phòng hiện tại">
              <a-input
                v-model:value="createForm.currentRoomNumber"
                disabled
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tòa nhà mong muốn">
              <a-input
                v-model:value="createForm.requestedBuildingName"
                placeholder="Nhập tòa nhà"
              />
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="Loại phòng mong muốn">
          <a-input
            v-model:value="createForm.requestedRoomTypeName"
            placeholder="Nhập loại phòng"
          />
        </a-form-item>
        <a-form-item
          label="Lý do chuyển phòng"
          :validate-status="createErrors.reason ? 'error' : ''"
          :help="createErrors.reason"
        >
          <a-textarea
            v-model:value="createForm.reason"
            :rows="3"
            placeholder="Nhập lý do"
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Approve Dialog -->
    <a-modal
      v-model:open="approveDialog"
      title="Duyệt chuyển phòng"
      :confirm-loading="saving"
      :maskClosable="false"
      @ok="handleApprove"
      @cancel="approveDialog = false"
      okText="Duyệt"
      cancelText="Hủy"
    >
      <div style="margin-bottom: 16px">
        <div>Sinh viên: <strong>{{ approveTarget?.studentName }}</strong></div>
        <div>Từ phòng <strong>{{ approveTarget?.currentRoomNumber }}</strong></div>
      </div>
      <a-form layout="vertical">
        <a-form-item
          label="Tòa nhà mới"
          :validate-status="approveErrors.buildingId ? 'error' : ''"
          :help="approveErrors.buildingId"
        >
          <a-select
            v-model:value="approveForm.buildingId"
            placeholder="Chọn tòa nhà"
            @change="onBuildingChange"
            :loading="loadingBuildings"
          >
            <a-select-option
              v-for="building in buildings"
              :key="building.id"
              :value="building.id"
            >
              {{ building.name }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item
          label="Phòng mới"
          :validate-status="approveErrors.newRoomId ? 'error' : ''"
          :help="approveErrors.newRoomId"
        >
          <a-select
            v-model:value="approveForm.newRoomId"
            placeholder="Chọn phòng"
            :disabled="!approveForm.buildingId || loadingRooms"
            :loading="loadingRooms"
            show-search
            :filter-option="filterRoomOption"
          >
            <a-select-option
              v-for="room in availableRooms"
              :key="room.id"
              :value="room.id"
            >
              {{ room.roomNumber }} - {{ room.roomTypeName }} ({{ room.currentOccupants }}/{{ room.maxOccupants }})
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item
          label="Ngày chuyển"
          :validate-status="approveErrors.transferDate ? 'error' : ''"
          :help="approveErrors.transferDate"
        >
          <a-date-picker
            v-model:value="approveForm.transferDate"
            format="DD/MM/YYYY"
            style="width: 100%"
            placeholder="Chọn ngày chuyển"
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Reject Dialog -->
    <a-modal
      v-model:open="rejectDialog"
      title="Từ chối chuyển phòng"
      :confirm-loading="saving"
      :maskClosable="false"
      @ok="handleReject"
      @cancel="rejectDialog = false"
      okText="Từ chối"
      cancelText="Hủy"
      ok-button-props="{ danger: true }"
    >
      <div style="margin-bottom: 16px">
        Sinh viên: <strong>{{ rejectTarget?.studentName }}</strong>
      </div>
      <a-form layout="vertical">
        <a-form-item
          label="Lý do từ chối"
          :validate-status="rejectErrors.rejectReason ? 'error' : ''"
          :help="rejectErrors.rejectReason"
        >
          <a-textarea
            v-model:value="rejectForm.rejectReason"
            :rows="3"
            placeholder="Nhập lý do từ chối"
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Detail Dialog -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết yêu cầu chuyển phòng"
      :footer="null"
      width="600px"
    >
      <a-descriptions bordered :column="1" v-if="detailTarget">
        <a-descriptions-item label="Sinh viên">
          <strong>{{ detailTarget.studentName }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Mã sinh viên">
          {{ detailTarget.studentCode }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng hiện tại">
          <strong>{{ detailTarget.currentRoomNumber }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Phòng mới">
          <strong style="color: #4caf50">{{ detailTarget.newRoomNumber || 'Chưa phân' }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Yêu cầu">
          {{ detailTarget.requestedRoomTypeName || 'Không cụ thể' }} - 
          {{ detailTarget.requestedBuildingName || 'Không cụ thể' }}
        </a-descriptions-item>
        <a-descriptions-item label="Lý do">
          {{ detailTarget.reason }}
        </a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="statusColor(detailTarget.status)">
            {{ statusLabel(detailTarget.status) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày tạo">
          {{ formatDateTime(detailTarget.createdAt) }}
        </a-descriptions-item>
        <template v-if="detailTarget.reviewedByName">
          <a-descriptions-item label="Người xử lý">
            <strong>{{ detailTarget.reviewedByName }}</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Ngày xử lý">
            {{ formatDateTime(detailTarget.reviewedAt) }}
          </a-descriptions-item>
          <a-descriptions-item v-if="detailTarget.rejectReason" label="Lý do từ chối">
            <span style="color: #f44336">{{ detailTarget.rejectReason }}</span>
          </a-descriptions-item>
          <a-descriptions-item v-if="detailTarget.transferDate" label="Ngày chuyển">
            <strong>{{ formatDate(detailTarget.transferDate) }}</strong>
          </a-descriptions-item>
        </template>
      </a-descriptions>
      <div style="display: flex; justify-content: flex-end; margin-top: 16px">
        <a-button @click="detailDialog = false">Đóng</a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { message } from 'ant-design-vue'
import dayjs from 'dayjs'
import {
  ClockCircleOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  SwapOutlined,
  ArrowRightOutlined
} from '@ant-design/icons-vue'
import { roomTransferService } from '@/services/roomTransferService'
import { contractService } from '@/services/contractService'
import axios from 'axios'

const API_BASE = 'http://localhost:5000' // API Gateway

const columns = [
  { title: 'Sinh viên', key: 'student', width: 220 },
  { title: 'Phòng hiện tại → Phòng mới', key: 'rooms', align: 'center', width: 180 },
  { title: 'Yêu cầu', key: 'requestedRoom', width: 180 },
  { title: 'Ngày tạo', key: 'createdAt', align: 'center', width: 120 },
  { title: 'Trạng thái', key: 'status', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 220 },
]

const statusOptions = [
  { label: 'Chờ duyệt', value: 'Pending' },
  { label: 'Đã duyệt', value: 'Approved' },
  { label: 'Từ chối', value: 'Rejected' },
  { label: 'Hoàn thành', value: 'Completed' },
]

const filterStatusOptions = [{ label: 'Tất cả', value: 'all' }, ...statusOptions]

const transfers = ref([])
const activeContracts = ref([])
const buildings = ref([])
const availableRooms = ref([])
const search = ref('')
const statusFilter = ref('all')
const loading = ref(false)
const saving = ref(false)
const loadingBuildings = ref(false)
const loadingRooms = ref(false)
const createDialog = ref(false)
const approveDialog = ref(false)
const rejectDialog = ref(false)
const detailDialog = ref(false)
const approveTarget = ref(null)
const rejectTarget = ref(null)
const detailTarget = ref(null)
const createForm = ref({
  contractId: null,
  currentRoomNumber: '',
  requestedBuildingName: '',
  requestedRoomTypeName: '',
  reason: '',
})
const approveForm = ref({ 
  buildingId: null,
  newRoomId: null, 
  transferDate: null 
})
const rejectForm = ref({ rejectReason: '' })
const createErrors = ref({})
const approveErrors = ref({})
const rejectErrors = ref({})

const filteredTransfers = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return transfers.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.studentName?.toLowerCase().includes(keyword) ||
      item.studentCode?.toLowerCase().includes(keyword)
    const matchesStatus = statusFilter.value === 'all' || item.status === statusFilter.value
    return matchesText && matchesStatus
  })
})

function countStatus(status) {
  return transfers.value.filter((item) => item.status === status).length
}

function resetFilters() {
  search.value = ''
  statusFilter.value = 'all'
}

async function loadTransfers() {
  loading.value = true
  try {
    transfers.value = await roomTransferService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách yêu cầu chuyển phòng')
  } finally {
    loading.value = false
  }
}

async function loadActiveContracts() {
  try {
    const allContracts = await contractService.getAll()
    activeContracts.value = allContracts
      .filter((c) => c.status === 'Active')
      .map((c) => ({
        ...c,
        displayText: `${c.contractCode} - ${c.studentName} - Phòng ${c.roomNumber}`,
      }))
  } catch (err) {
    console.error('Không thể tải danh sách hợp đồng', err)
  }
}

function openCreate() {
  createForm.value = {
    contractId: null,
    currentRoomNumber: '',
    requestedBuildingName: '',
    requestedRoomTypeName: '',
    reason: '',
  }
  createErrors.value = {}
  createDialog.value = true
}

function onContractChange(contractId) {
  const contract = activeContracts.value.find((c) => c.id === contractId)
  if (contract) {
    createForm.value.currentRoomNumber = contract.roomNumber
  }
}

async function handleCreate() {
  const errors = {}
  if (!createForm.value.contractId) errors.contractId = 'Vui lòng chọn hợp đồng'
  if (!createForm.value.reason.trim()) errors.reason = 'Vui lòng nhập lý do chuyển phòng'
  createErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    const contract = activeContracts.value.find((c) => c.id === createForm.value.contractId)
    await roomTransferService.create({
      contractId: createForm.value.contractId,
      studentId: contract.studentId,
      currentRoomId: contract.roomId,
      currentRoomNumber: createForm.value.currentRoomNumber,
      requestedBuildingName: createForm.value.requestedBuildingName?.trim() || null,
      requestedRoomTypeName: createForm.value.requestedRoomTypeName?.trim() || null,
      reason: createForm.value.reason.trim(),
    })
    message.success('Tạo yêu cầu thành công')
    createDialog.value = false
    await loadTransfers()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  } finally {
    saving.value = false
  }
}

function openApprove(item) {
  approveTarget.value = item
  approveForm.value = {
    buildingId: null,
    newRoomId: null,
    transferDate: dayjs(),
  }
  approveErrors.value = {}
  approveDialog.value = true
  loadBuildings()
}

async function loadBuildings() {
  loadingBuildings.value = true
  try {
    const response = await axios.get(`${API_BASE}/api/buildings`)
    buildings.value = response.data
  } catch (err) {
    console.error('Error loading buildings:', err)
    message.error('Không thể tải danh sách tòa nhà')
  } finally {
    loadingBuildings.value = false
  }
}

async function onBuildingChange(buildingId) {
  approveForm.value.newRoomId = null
  availableRooms.value = []
  
  if (!buildingId) return
  
  loadingRooms.value = true
  try {
    const response = await axios.get(`${API_BASE}/api/rooms/building/${buildingId}`)
    // Filter available rooms only
    availableRooms.value = response.data.filter(room => 
      room.status === 'Available' && room.currentOccupants < room.maxOccupants
    )
  } catch (err) {
    console.error('Error loading rooms:', err)
    message.error('Không thể tải danh sách phòng')
  } finally {
    loadingRooms.value = false
  }
}

function filterRoomOption(input, option) {
  return option.children[0].children.toLowerCase().includes(input.toLowerCase())
}

async function handleApprove() {
  const errors = {}
  if (!approveForm.value.buildingId) errors.buildingId = 'Vui lòng chọn tòa nhà'
  if (!approveForm.value.newRoomId) errors.newRoomId = 'Vui lòng chọn phòng mới'
  if (!approveForm.value.transferDate) errors.transferDate = 'Vui lòng chọn ngày chuyển'
  approveErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    const selectedRoom = availableRooms.value.find(r => r.id === approveForm.value.newRoomId)
    
    await roomTransferService.approve(approveTarget.value.id, {
      newRoomId: approveForm.value.newRoomId,
      newRoomNumber: selectedRoom.roomNumber,
      transferDate: approveForm.value.transferDate.format('YYYY-MM-DD'),
      reviewedByUserId: 1, // TODO: Get from auth
      reviewedByName: 'Admin', // TODO: Get from auth
    })
    message.success('Đã duyệt yêu cầu chuyển phòng')
    approveDialog.value = false
    await loadTransfers()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  } finally {
    saving.value = false
  }
}

function openReject(item) {
  rejectTarget.value = item
  rejectForm.value = { rejectReason: '' }
  rejectErrors.value = {}
  rejectDialog.value = true
}

async function handleReject() {
  const errors = {}
  if (!rejectForm.value.rejectReason.trim())
    errors.rejectReason = 'Vui lòng nhập lý do từ chối'
  rejectErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await roomTransferService.reject(rejectTarget.value.id, {
      reviewedByUserId: 1, // TODO: Get from auth
      reviewedByName: 'Admin', // TODO: Get from auth
      rejectReason: rejectForm.value.rejectReason.trim(),
    })
    message.success('Đã từ chối yêu cầu chuyển phòng')
    rejectDialog.value = false
    await loadTransfers()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  } finally {
    saving.value = false
  }
}

function viewDetail(item) {
  detailTarget.value = item
  detailDialog.value = true
}

function statusLabel(status) {
  return statusOptions.find((item) => item.value === status)?.label || status
}

function statusColor(status) {
  return (
    { Pending: 'warning', Approved: 'success', Rejected: 'error', Completed: 'info' }[status] ||
    'grey'
  )
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString('vi-VN') : ''
}

function formatDateTime(value) {
  return value
    ? new Date(value).toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
      })
    : ''
}

onMounted(() => {
  loadTransfers()
  loadActiveContracts()
})
</script>
