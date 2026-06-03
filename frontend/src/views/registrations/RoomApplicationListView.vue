<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Đơn đăng ký phòng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Tổng số: {{ applications.length }} đơn đăng ký
        </p>
      </div>
    </div>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="applications"
      :treatEmptyAsError="false"
      @retry="loadApplications"
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
            allowClear
            style="max-width: 220px"
          >
            <a-select-option
              v-for="option in filterStatusOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
          <a-button v-if="search || statusFilter !== 'all'" type="text" size="small" @click="resetFilters">
            Đặt lại
          </a-button>
        </div>

        <a-table
          :columns="columns"
          :data-source="filteredApplications"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'studentName'">
              <div class="d-flex align-center" style="gap: 12px; padding: 12px 0">
                <a-avatar size="32" style="background: #e6f7ff; color: #1890ff">
                  {{ record.studentName?.charAt(0) || 'S' }}
                </a-avatar>
                <div>
                  <div class="font-weight-bold">{{ record.studentName }}</div>
                  <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'preferredBuilding'">
              <div>
                <div class="font-weight-medium">{{ record.preferredBuildingName }}</div>
                <div style="font-size: 12px; color: #8c8c8c">{{ record.preferredRoomTypeName }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'requestedDate'">
              {{ formatDate(record.requestedStartDate) }} - {{ formatDate(record.requestedEndDate) }}
            </template>
            <template v-else-if="column.key === 'price'">
              {{ formatCurrency(record.preferredRoomPrice) }}
            </template>
            <template v-else-if="column.key === 'status'">
              <a-tag :color="statusColor(record.status)">{{ statusLabel(record.status) }}</a-tag>
            </template>
            <template v-else-if="column.key === 'actions'">
              <a-space size="small">
                <a-button v-if="record.status === 'Pending'" type="link" @click="openApprove(record)">
                  Duyệt
                </a-button>
                <a-button v-if="record.status === 'Pending'" type="text" danger @click="openReject(record)">
                  Từ chối
                </a-button>
                <a-button v-if="record.status === 'Approved'" type="link" @click="createContract(record)">
                  Tạo hợp đồng
                </a-button>
                <a-button type="link" @click="viewDetail(record)">Chi tiết</a-button>
              </a-space>
            </template>
          </template>
        </a-table>
      </a-card>
    </DataStatus>

    <!-- Approve Dialog -->
    <v-dialog v-model="approveDialog" max-width="560" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Duyệt đơn đăng ký</h2>
        <p class="mb-4">Sinh viên: <strong>{{ approveTarget?.studentName }}</strong></p>
        <v-row>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.assignedRoomNumber"
              label="Số phòng được phân *"
              :error-messages="approveErrors.assignedRoomNumber"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.assignedBuildingName"
              label="Tên tòa nhà *"
              :error-messages="approveErrors.assignedBuildingName"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="approveForm.assignedRoomId"
              label="ID Phòng *"
              type="number"
              :error-messages="approveErrors.assignedRoomId"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="approveDialog = false">Hủy</v-btn>
          <v-btn color="primary" :loading="saving" @click="handleApprove">Duyệt đơn</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Reject Dialog -->
    <v-dialog v-model="rejectDialog" max-width="560" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Từ chối đơn đăng ký</h2>
        <p class="mb-4">Sinh viên: <strong>{{ rejectTarget?.studentName }}</strong></p>
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

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import DataStatus from '@/components/common/DataStatus.vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { useRouter } from 'vue-router'

const router = useRouter()

const columns = [
  { title: 'Sinh viên', key: 'studentName', width: 220 },
  { title: 'Tòa & Loại phòng', key: 'preferredBuilding', width: 200 },
  { title: 'Thời gian yêu cầu', key: 'requestedDate', align: 'center' },
  { title: 'Giá thuê', key: 'price', align: 'right', width: 120 },
  { title: 'Trạng thái', key: 'status', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 220 },
]

const statusOptions = [
  { label: 'Chờ duyệt', value: 'Pending' },
  { label: 'Đã duyệt', value: 'Approved' },
  { label: 'Từ chối', value: 'Rejected' },
  { label: 'Đã hủy', value: 'Cancelled' },
]

const filterStatusOptions = [{ label: 'Tất cả', value: 'all' }, ...statusOptions]

const applications = ref([])
const search = ref('')
const statusFilter = ref('all')
const loading = ref(false)
const saving = ref(false)
const error = ref(null)
const approveDialog = ref(false)
const rejectDialog = ref(false)
const approveTarget = ref(null)
const rejectTarget = ref(null)
const approveForm = ref({ assignedRoomId: '', assignedRoomNumber: '', assignedBuildingName: '' })
const rejectForm = ref({ rejectReason: '' })
const approveErrors = ref({})
const rejectErrors = ref({})
const snackbar = ref({ show: false, message: '', color: 'success' })

const filteredApplications = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return applications.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.studentName?.toLowerCase().includes(keyword) ||
      item.studentCode?.toLowerCase().includes(keyword)
    const matchesStatus = statusFilter.value === 'all' || item.status === statusFilter.value
    return matchesText && matchesStatus
  })
})

function resetFilters() {
  search.value = ''
  statusFilter.value = 'all'
}

async function loadApplications() {
  loading.value = true
  error.value = null
  try {
    applications.value = await roomApplicationService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách đơn đăng ký'
  } finally {
    loading.value = false
  }
}

function openApprove(item) {
  approveTarget.value = item
  approveForm.value = {
    assignedRoomId: '',
    assignedRoomNumber: '',
    assignedBuildingName: item.preferredBuildingName,
  }
  approveErrors.value = {}
  approveDialog.value = true
}

function openReject(item) {
  rejectTarget.value = item
  rejectForm.value = { rejectReason: '' }
  rejectErrors.value = {}
  rejectDialog.value = true
}

async function handleApprove() {
  const errors = {}
  if (!approveForm.value.assignedRoomId) errors.assignedRoomId = 'Vui lòng nhập ID phòng'
  if (!approveForm.value.assignedRoomNumber.trim())
    errors.assignedRoomNumber = 'Vui lòng nhập số phòng'
  if (!approveForm.value.assignedBuildingName.trim())
    errors.assignedBuildingName = 'Vui lòng nhập tên tòa nhà'
  approveErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await roomApplicationService.approve(approveTarget.value.id, {
      reviewedByUserId: 1, // TODO: Get from auth
      reviewedByName: 'Admin', // TODO: Get from auth
      assignedRoomId: parseInt(approveForm.value.assignedRoomId),
      assignedRoomNumber: approveForm.value.assignedRoomNumber.trim(),
      assignedBuildingName: approveForm.value.assignedBuildingName.trim(),
    })
    notify('Đã duyệt đơn đăng ký thành công')
    approveDialog.value = false
    await loadApplications()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

async function handleReject() {
  const errors = {}
  if (!rejectForm.value.rejectReason.trim())
    errors.rejectReason = 'Vui lòng nhập lý do từ chối'
  rejectErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await roomApplicationService.reject(rejectTarget.value.id, {
      reviewedByUserId: 1, // TODO: Get from auth
      reviewedByName: 'Admin', // TODO: Get from auth
      rejectReason: rejectForm.value.rejectReason.trim(),
    })
    notify('Đã từ chối đơn đăng ký')
    rejectDialog.value = false
    await loadApplications()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

function createContract(item) {
  router.push({ name: 'contracts', query: { applicationId: item.id } })
}

function viewDetail(item) {
  router.push({ name: 'room-application-detail', params: { id: item.id } })
}

function statusLabel(status) {
  return statusOptions.find((item) => item.value === status)?.label || status
}

function statusColor(status) {
  return (
    { Pending: 'warning', Approved: 'success', Rejected: 'error', Cancelled: 'grey' }[status] ||
    'grey'
  )
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString('vi-VN') : ''
}

function formatCurrency(value) {
  return value ? value.toLocaleString('vi-VN') + 'đ' : ''
}

function notify(message, color = 'success') {
  snackbar.value = { show: false, message, color }
  snackbar.value.show = true
}

onMounted(loadApplications)
</script>
