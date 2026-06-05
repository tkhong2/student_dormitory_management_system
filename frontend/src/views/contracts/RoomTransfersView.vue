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
    <v-row class="mb-4">
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fff7ed" size="40" rounded="lg">
              <v-icon color="warning" size="20">mdi-clock-outline</v-icon>
            </v-avatar>
            <div>
              <div class="text-h6 font-weight-bold">{{ countStatus('Pending') }}</div>
              <div class="text-caption text-medium-emphasis">Chờ duyệt</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#dcfce7" size="40" rounded="lg">
              <v-icon color="success" size="20">mdi-check-circle</v-icon>
            </v-avatar>
            <div>
              <div class="text-h6 font-weight-bold">{{ countStatus('Approved') }}</div>
              <div class="text-caption text-medium-emphasis">Đã duyệt</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fee2e2" size="40" rounded="lg">
              <v-icon color="error" size="20">mdi-close-circle</v-icon>
            </v-avatar>
            <div>
              <div class="text-h6 font-weight-bold">{{ countStatus('Rejected') }}</div>
              <div class="text-caption text-medium-emphasis">Từ chối</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#e0f2fe" size="40" rounded="lg">
              <v-icon color="info" size="20">mdi-swap-horizontal</v-icon>
            </v-avatar>
            <div>
              <div class="text-h6 font-weight-bold">{{ countStatus('Completed') }}</div>
              <div class="text-caption text-medium-emphasis">Hoàn thành</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="transfers"
      :treatEmptyAsError="false"
      @retry="loadTransfers"
    >
      <a-card
        style="border: 1px solid #e5e7eb; background: #fafafa"
        :body-style="{ padding: '0' }"
      >
        <div class="pa-4 d-flex flex-wrap align-center" style="gap: 12px">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên, mã SV..."
            allowClear
            style="max-width: 300px; flex: 1"
          />
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            style="max-width: 200px"
          >
            <a-select-option
              v-for="option in filterStatusOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
          <a-button
            v-if="search || statusFilter !== 'all'"
            type="text"
            size="small"
            @click="resetFilters"
          >
            Đặt lại
          </a-button>
        </div>

        <a-table
          :columns="columns"
          :data-source="filteredTransfers"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'student'">
              <div class="d-flex align-center" style="gap: 12px; padding: 12px 0">
                <a-avatar size="36" style="background: #e6f7ff; color: #1890ff">
                  {{ record.studentName?.charAt(0) || 'S' }}
                </a-avatar>
                <div>
                  <div class="font-weight-bold">{{ record.studentName }}</div>
                  <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'rooms'">
              <div class="text-center">
                <div class="font-weight-medium">{{ record.currentRoomNumber }}</div>
                <v-icon size="16" color="primary">mdi-arrow-right</v-icon>
                <div class="font-weight-bold" style="color: #4caf50">
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
    </DataStatus>

    <!-- Create Dialog -->
    <v-dialog v-model="createDialog" max-width="600" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Tạo yêu cầu chuyển phòng</h2>
        <v-row>
          <v-col cols="12">
            <v-select
              v-model="createForm.contractId"
              label="Hợp đồng *"
              :items="activeContracts"
              item-title="displayText"
              item-value="id"
              :error-messages="createErrors.contractId"
              @update:model-value="onContractChange"
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="createForm.currentRoomNumber"
              label="Phòng hiện tại"
              readonly
              disabled
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="createForm.requestedBuildingName"
              label="Tòa nhà mong muốn"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="createForm.requestedRoomTypeName"
              label="Loại phòng mong muốn"
            />
          </v-col>
          <v-col cols="12">
            <v-textarea
              v-model="createForm.reason"
              label="Lý do chuyển phòng *"
              rows="3"
              :error-messages="createErrors.reason"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="createDialog = false">Hủy</v-btn>
          <v-btn color="primary" :loading="saving" @click="handleCreate">Tạo yêu cầu</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Approve Dialog -->
    <v-dialog v-model="approveDialog" max-width="500" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Duyệt chuyển phòng</h2>
        <p class="mb-3">
          Sinh viên: <strong>{{ approveTarget?.studentName }}</strong>
        </p>
        <p class="mb-4">
          Từ phòng <strong>{{ approveTarget?.currentRoomNumber }}</strong>
        </p>
        <v-row>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.newRoomNumber"
              label="Phòng mới *"
              :error-messages="approveErrors.newRoomNumber"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.newRoomId"
              label="ID Phòng mới *"
              type="number"
              :error-messages="approveErrors.newRoomId"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.transferDate"
              label="Ngày chuyển *"
              type="date"
              :error-messages="approveErrors.transferDate"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="approveDialog = false">Hủy</v-btn>
          <v-btn color="primary" :loading="saving" @click="handleApprove">Duyệt</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Reject Dialog -->
    <v-dialog v-model="rejectDialog" max-width="500" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Từ chối chuyển phòng</h2>
        <p class="mb-4">
          Sinh viên: <strong>{{ rejectTarget?.studentName }}</strong>
        </p>
        <v-textarea
          v-model="rejectForm.rejectReason"
          label="Lý do từ chối *"
          rows="3"
          :error-messages="rejectErrors.rejectReason"
        />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="rejectDialog = false">Hủy</v-btn>
          <v-btn color="error" :loading="saving" @click="handleReject">Từ chối</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Detail Dialog -->
    <v-dialog v-model="detailDialog" max-width="560">
      <v-card class="pa-6" v-if="detailTarget">
        <h2 class="text-h6 font-weight-bold mb-4">Chi tiết yêu cầu chuyển phòng</h2>
        <v-row dense>
          <v-col cols="5" class="text-caption text-medium-emphasis">Sinh viên:</v-col>
          <v-col cols="7" class="font-weight-bold">{{ detailTarget.studentName }}</v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Mã sinh viên:</v-col>
          <v-col cols="7">{{ detailTarget.studentCode }}</v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Phòng hiện tại:</v-col>
          <v-col cols="7" class="font-weight-medium">{{ detailTarget.currentRoomNumber }}</v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Phòng mới:</v-col>
          <v-col cols="7" class="font-weight-bold" style="color: #4caf50">
            {{ detailTarget.newRoomNumber || 'Chưa phân' }}
          </v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Yêu cầu:</v-col>
          <v-col cols="7">
            {{ detailTarget.requestedRoomTypeName || 'Không cụ thể' }} - 
            {{ detailTarget.requestedBuildingName || 'Không cụ thể' }}
          </v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Lý do:</v-col>
          <v-col cols="7">{{ detailTarget.reason }}</v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Trạng thái:</v-col>
          <v-col cols="7">
            <a-tag :color="statusColor(detailTarget.status)">
              {{ statusLabel(detailTarget.status) }}
            </a-tag>
          </v-col>
          
          <v-col cols="5" class="text-caption text-medium-emphasis">Ngày tạo:</v-col>
          <v-col cols="7">{{ formatDateTime(detailTarget.createdAt) }}</v-col>
          
          <template v-if="detailTarget.reviewedByName">
            <v-divider class="my-3" />
            <v-col cols="5" class="text-caption text-medium-emphasis">Người xử lý:</v-col>
            <v-col cols="7" class="font-weight-medium">{{ detailTarget.reviewedByName }}</v-col>
            
            <v-col cols="5" class="text-caption text-medium-emphasis">Ngày xử lý:</v-col>
            <v-col cols="7">{{ formatDateTime(detailTarget.reviewedAt) }}</v-col>
            
            <v-col
              v-if="detailTarget.rejectReason"
              cols="5"
              class="text-caption text-medium-emphasis"
            >
              Lý do từ chối:
            </v-col>
            <v-col v-if="detailTarget.rejectReason" cols="7" style="color: #f44336">
              {{ detailTarget.rejectReason }}
            </v-col>
            
            <v-col
              v-if="detailTarget.transferDate"
              cols="5"
              class="text-caption text-medium-emphasis"
            >
              Ngày chuyển:
            </v-col>
            <v-col v-if="detailTarget.transferDate" cols="7" class="font-weight-bold">
              {{ formatDate(detailTarget.transferDate) }}
            </v-col>
          </template>
        </v-row>
        <div class="d-flex justify-end mt-4">
          <v-btn variant="text" @click="detailDialog = false">Đóng</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import DataStatus from '@/components/common/DataStatus.vue'
import { roomTransferService } from '@/services/roomTransferService'
import { contractService } from '@/services/contractService'

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
const search = ref('')
const statusFilter = ref('all')
const loading = ref(false)
const saving = ref(false)
const error = ref(null)
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
const approveForm = ref({ newRoomId: '', newRoomNumber: '', transferDate: '' })
const rejectForm = ref({ rejectReason: '' })
const createErrors = ref({})
const approveErrors = ref({})
const rejectErrors = ref({})
const snackbar = ref({ show: false, message: '', color: 'success' })

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
  error.value = null
  try {
    transfers.value = await roomTransferService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách yêu cầu chuyển phòng'
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
    notify('Tạo yêu cầu thành công')
    createDialog.value = false
    await loadTransfers()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

function openApprove(item) {
  approveTarget.value = item
  approveForm.value = {
    newRoomId: '',
    newRoomNumber: '',
    transferDate: new Date().toISOString().slice(0, 10),
  }
  approveErrors.value = {}
  approveDialog.value = true
}

async function handleApprove() {
  const errors = {}
  if (!approveForm.value.newRoomId) errors.newRoomId = 'Vui lòng nhập ID phòng mới'
  if (!approveForm.value.newRoomNumber.trim())
    errors.newRoomNumber = 'Vui lòng nhập số phòng mới'
  if (!approveForm.value.transferDate) errors.transferDate = 'Vui lòng chọn ngày chuyển'
  approveErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await roomTransferService.approve(approveTarget.value.id, {
      newRoomId: parseInt(approveForm.value.newRoomId),
      newRoomNumber: approveForm.value.newRoomNumber.trim(),
      transferDate: approveForm.value.transferDate,
      reviewedByUserId: 1, // TODO: Get from auth
      reviewedByName: 'Admin', // TODO: Get from auth
    })
    notify('Đã duyệt yêu cầu chuyển phòng')
    approveDialog.value = false
    await loadTransfers()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
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
    notify('Đã từ chối yêu cầu chuyển phòng')
    rejectDialog.value = false
    await loadTransfers()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
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

function notify(message, color = 'success') {
  snackbar.value = { show: false, message, color }
  snackbar.value.show = true
}

onMounted(() => {
  loadTransfers()
  loadActiveContracts()
})
</script>
