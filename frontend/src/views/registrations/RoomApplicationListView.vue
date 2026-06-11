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
                <div style="font-weight: 500">{{ record.assignedRoomNumber || 'Chưa chọn' }}</div>
                <div style="font-size: 12px; color: #8c8c8c">
                  {{ record.assignedBuildingName || record.preferredBuildingName }}
                </div>
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
                <a-tooltip title="Xem chi tiết">
                  <a-button type="text" size="small" @click="viewDetail(record)">
                    <template #icon><EyeOutlined /></template>
                  </a-button>
                </a-tooltip>
                <a-tooltip title="Duyệt">
                  <a-button v-if="record.status === 'Pending'" type="text" size="small" @click="openApprove(record)" style="color: #52c41a;">
                    <template #icon><CheckOutlined /></template>
                  </a-button>
                </a-tooltip>
                <a-tooltip title="Từ chối">
                  <a-button v-if="record.status === 'Pending'" type="text" size="small" danger @click="openReject(record)">
                    <template #icon><CloseOutlined /></template>
                  </a-button>
                </a-tooltip>
                <a-popconfirm
                  title="Bạn có chắc muốn xóa đơn này?"
                  ok-text="Xóa"
                  cancel-text="Hủy"
                  @confirm="deleteApplication(record)"
                >
                  <a-tooltip title="Xóa">
                    <a-button type="text" size="small" danger>
                      <template #icon><DeleteOutlined /></template>
                    </a-button>
                  </a-tooltip>
                </a-popconfirm>
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
      <div style="margin-bottom: 16px">
        <p>Sinh viên: <strong>{{ approveTarget?.studentName }}</strong></p>
        <p v-if="approveTarget?.assignedRoomNumber">
          Phòng đã chọn: <strong>{{ approveTarget?.assignedRoomNumber }} - {{ approveTarget?.assignedBuildingName }}</strong>
        </p>
        <a-alert
          v-else
          message="Đơn cũ - Chưa có thông tin phòng"
          description="Đây là đơn đăng ký cũ (trước khi cập nhật hệ thống). Bạn nên xóa đơn này và yêu cầu sinh viên đăng ký lại để có đầy đủ thông tin phòng."
          type="warning"
          show-icon
          style="margin-bottom: 16px"
        />
      </div>
      <a-alert
        v-if="approveTarget?.assignedRoomNumber"
        message="Xác nhận duyệt đơn"
        description="Sinh viên đã chọn phòng cụ thể. Bạn có chắc chắn muốn duyệt đơn này không?"
        type="info"
        show-icon
      />
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

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết đơn đăng ký"
      width="800px"
      :footer="null"
    >
      <a-descriptions v-if="detailTarget" bordered :column="2" size="small">
        <a-descriptions-item label="Sinh viên" :span="2">
          <strong>{{ detailTarget.studentName }}</strong> ({{ detailTarget.studentCode }})
        </a-descriptions-item>
        
        <a-descriptions-item label="Phòng đã chọn" :span="2">
          <a-tag color="blue">{{ detailTarget.assignedRoomNumber || 'Chưa chọn' }}</a-tag>
          {{ detailTarget.assignedBuildingName || detailTarget.preferredBuildingName }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Loại phòng">
          {{ detailTarget.preferredRoomTypeName }}
        </a-descriptions-item>
        <a-descriptions-item label="Giá thuê">
          <strong style="color: #1890ff;">{{ formatCurrency(detailTarget.preferredRoomPrice) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Ngày bắt đầu">
          {{ formatDate(detailTarget.requestedStartDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc">
          {{ formatDate(detailTarget.requestedEndDate) }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Thời hạn">
          {{ detailTarget.durationMonths }} tháng
        </a-descriptions-item>
        <a-descriptions-item label="Loại sinh viên">
          <a-tag :color="detailTarget.isLocalStudent ? 'default' : 'orange'">
            {{ detailTarget.isLocalStudent ? 'Nội tỉnh' : 'Ngoại tỉnh' }}
          </a-tag>
        </a-descriptions-item>
        
        <a-descriptions-item label="Trạng thái" :span="2">
          <a-tag :color="statusColor(detailTarget.status)">
            {{ statusLabel(detailTarget.status) }}
          </a-tag>
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.preferences" label="Yêu cầu đặc biệt" :span="2">
          {{ detailTarget.preferences }}
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.note" label="Ghi chú" :span="2">
          {{ detailTarget.note }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Liên hệ khẩn cấp" :span="2">
          {{ detailTarget.emergencyContactName }} - {{ detailTarget.emergencyContactPhone }}
          ({{ detailTarget.emergencyContactRelationship }})
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.reviewedByName" label="Người xử lý" :span="2">
          {{ detailTarget.reviewedByName }} - {{ formatDate(detailTarget.reviewedAt) }}
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.rejectReason" label="Lý do từ chối" :span="2">
          <a-alert type="error" :message="detailTarget.rejectReason" show-icon />
        </a-descriptions-item>
        
        <a-descriptions-item label="Ngày tạo đơn" :span="2">
          {{ formatDate(detailTarget.createdAt) }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="detailDialog = false">Đóng</a-button>
        <a-button 
          v-if="detailTarget?.status === 'Pending'" 
          type="primary" 
          @click="openApprove(detailTarget)"
        >
          <template #icon><CheckOutlined /></template>
          Duyệt đơn
        </a-button>
        <a-button 
          v-if="detailTarget?.status === 'Pending'" 
          danger 
          @click="openReject(detailTarget)"
        >
          <template #icon><CloseOutlined /></template>
          Từ chối
        </a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { message } from 'ant-design-vue'
import { EyeOutlined, CheckOutlined, CloseOutlined, DeleteOutlined } from '@ant-design/icons-vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { useRouter } from 'vue-router'

const router = useRouter()

const columns = [
  { title: 'Sinh viên', key: 'studentName', width: 220 },
  { title: 'Phòng đã chọn', key: 'preferredBuilding', width: 200 },
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
const detailDialog = ref(false)
const approveTarget = ref(null)
const rejectTarget = ref(null)
const detailTarget = ref(null)
const rejectForm = ref({ rejectReason: '' })
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
  // Kiểm tra xem sinh viên đã chọn phòng chưa
  if (!item.assignedRoomId || !item.assignedRoomNumber) {
    message.warning('Đơn này chưa có thông tin phòng. Đây có thể là đơn cũ, vui lòng xóa và yêu cầu sinh viên đăng ký lại.')
    // Tạm thời cho phép duyệt để xử lý đơn cũ
  }
  approveTarget.value = item
  approveDialog.value = true
}

function openReject(item) {
  rejectTarget.value = item
  rejectForm.value = { rejectReason: '' }
  rejectErrors.value = {}
  rejectDialog.value = true
}

async function handleApprove() {
  saving.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    await roomApplicationService.approve(approveTarget.value.id, {
      reviewedByUserId: user.id || 1,
      reviewedByName: user.fullName || 'Admin',
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
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    await roomApplicationService.reject(rejectTarget.value.id, {
      reviewedByUserId: user.id || 1,
      reviewedByName: user.fullName || 'Admin',
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

async function deleteApplication(item) {
  try {
    await roomApplicationService.delete(item.id)
    message.success('Đã xóa đơn đăng ký thành công')
    await loadApplications()
  } catch (err) {
    message.error(err.message || 'Không thể xóa đơn đăng ký')
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
