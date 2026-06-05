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

    <!-- Filters and Table Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 16px; flex-wrap: wrap">
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
        <a-button v-if="search || statusFilter !== 'all'" @click="resetFilters">
          Đặt lại
        </a-button>
      </div>
    </a-card>

    <a-card :bordered="false" :loading="loading">
      <a-table
          :columns="columns"
          :data-source="filteredApplications"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'studentName'">
              <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
                <a-avatar size="32" style="background: #e6f7ff; color: #1890ff">
                  {{ record.studentName?.charAt(0) || 'S' }}
                </a-avatar>
                <div>
                  <div style="font-weight: 600">{{ record.studentName }}</div>
                  <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'preferredBuilding'">
              <div>
                <div style="font-weight: 500">{{ record.preferredBuildingName }}</div>
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

    <!-- Approve Dialog -->
    <a-modal
      v-model:open="approveDialog"
      title="Duyệt đơn đăng ký"
      :confirm-loading="saving"
      :maskClosable="false"
      @ok="handleApprove"
      @cancel="approveDialog = false"
      okText="Duyệt đơn"
      cancelText="Hủy"
    >
      <div style="margin-bottom: 16px">Sinh viên: <strong>{{ approveTarget?.studentName }}</strong></div>
      <a-form layout="vertical">
        <a-form-item
          label="Số phòng được phân"
          :validate-status="approveErrors.assignedRoomNumber ? 'error' : ''"
          :help="approveErrors.assignedRoomNumber"
        >
          <a-input v-model:value="approveForm.assignedRoomNumber" placeholder="Nhập số phòng" />
        </a-form-item>
        <a-form-item
          label="Tên tòa nhà"
          :validate-status="approveErrors.assignedBuildingName ? 'error' : ''"
          :help="approveErrors.assignedBuildingName"
        >
          <a-input v-model:value="approveForm.assignedBuildingName" placeholder="Nhập tên tòa" />
        </a-form-item>
        <a-form-item
          label="ID Phòng"
          :validate-status="approveErrors.assignedRoomId ? 'error' : ''"
          :help="approveErrors.assignedRoomId"
        >
          <a-input-number
            v-model:value="approveForm.assignedRoomId"
            :min="1"
            style="width: 100%"
            placeholder="Nhập ID phòng"
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Reject Dialog -->
    <a-modal
      v-model:open="rejectDialog"
      title="Từ chối đơn đăng ký"
      :confirm-loading="saving"
      :maskClosable="false"
      @ok="handleReject"
      @cancel="rejectDialog = false"
      okText="Từ chối"
      cancelText="Hủy"
      ok-button-props="{ danger: true }"
    >
      <div style="margin-bottom: 16px">Sinh viên: <strong>{{ rejectTarget?.studentName }}</strong></div>
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
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { message } from 'ant-design-vue'
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
const approveDialog = ref(false)
const rejectDialog = ref(false)
const approveTarget = ref(null)
const rejectTarget = ref(null)
const approveForm = ref({ assignedRoomId: null, assignedRoomNumber: '', assignedBuildingName: '' })
const rejectForm = ref({ rejectReason: '' })
const approveErrors = ref({})
const rejectErrors = ref({})

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
  try {
    applications.value = await roomApplicationService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách đơn đăng ký')
  } finally {
    loading.value = false
  }
}

function openApprove(item) {
  approveTarget.value = item
  approveForm.value = {
    assignedRoomId: null,
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
    message.success('Đã duyệt đơn đăng ký thành công')
    approveDialog.value = false
    await loadApplications()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
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
    message.success('Đã từ chối đơn đăng ký')
    rejectDialog.value = false
    await loadApplications()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
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

onMounted(loadApplications)
</script>
