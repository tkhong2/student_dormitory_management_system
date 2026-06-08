<template>
  <div>
    <PageHeaderAnt title="Quản lý đơn đăng ký" subtitle="Xử lý đơn đăng ký phòng từ sinh viên">
      <template #actions>
        <a-space>
          <a-input-search
            v-model:value="searchText"
            placeholder="Tìm theo tên, mã SV..."
            style="width: 300px"
            @search="handleSearch"
          />
          <a-button @click="loadApplications">
            <template #icon><ReloadOutlined /></template>
          </a-button>
        </a-space>
      </template>
    </PageHeaderAnt>

    <!-- Stats -->
    <a-row :gutter="16" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Chờ xử lý" :value="stats.pending" :valueStyle="{ color: '#faad14' }">
            <template #prefix><ClockCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Đã duyệt" :value="stats.approved" :valueStyle="{ color: '#52c41a' }">
            <template #prefix><CheckCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Từ chối" :value="stats.rejected" :valueStyle="{ color: '#ff4d4f' }">
            <template #prefix><CloseCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Tổng cộng" :value="stats.total" :valueStyle="{ color: '#1890ff' }">
            <template #prefix><FileTextOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Tabs -->
    <a-card :bordered="false">
      <a-tabs v-model:activeKey="activeTab" @change="handleTabChange">
        <a-tab-pane key="pending" tab="Chờ xử lý">
          <template #tab>
            <a-badge :count="stats.pending" :offset="[10, 0]">
              <span>Chờ xử lý</span>
            </a-badge>
          </template>
        </a-tab-pane>
        <a-tab-pane key="approved" tab="Đã duyệt" />
        <a-tab-pane key="rejected" tab="Từ chối" />
        <a-tab-pane key="all" tab="Tất cả" />
      </a-tabs>

      <!-- Table -->
      <a-table
        :columns="columns"
        :data-source="filteredApplications"
        :loading="loading"
        :pagination="{
          pageSize: 15,
          showTotal: (total) => `Tổng ${total} đơn`
        }"
        :row-key="record => record.id"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <div style="font-weight: 600;">{{ record.studentName }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'room'">
            <div>
              <a-tag color="blue">{{ record.assignedRoomNumber }}</a-tag>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.assignedBuildingName }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'dates'">
            <div style="font-size: 12px;">
              <div>Từ: {{ formatDate(record.requestedStartDate) }}</div>
              <div>Đến: {{ formatDate(record.requestedEndDate) }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
            <div v-if="record.status === 'Pending' && getDaysWaiting(record.createdAt) > 2" 
                 style="font-size: 11px; color: #ff4d4f; margin-top: 4px;">
              <ClockCircleOutlined /> {{ getDaysWaiting(record.createdAt) }} ngày
            </div>
          </template>

          <template v-else-if="column.key === 'priority'">
            <a-tag :color="record.isLocalStudent ? 'default' : 'orange'">
              {{ record.isLocalStudent ? 'Nội tỉnh' : 'Ngoại tỉnh' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="viewDetail(record)">
                <template #icon><EyeOutlined /></template>
                Xem
              </a-button>
              <a-button 
                v-if="record.status === 'Pending'" 
                size="small" 
                type="primary" 
                @click="openApproveModal(record)"
              >
                <template #icon><CheckOutlined /></template>
                Duyệt
              </a-button>
              <a-button 
                v-if="record.status === 'Pending'" 
                size="small" 
                danger 
                @click="openRejectModal(record)"
              >
                <template #icon><CloseOutlined /></template>
                Từ chối
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailModalVisible"
      title="Chi tiết đơn đăng ký"
      width="800px"
      :footer="null"
    >
      <a-descriptions v-if="selectedApplication" bordered :column="2" size="small">
        <a-descriptions-item label="Sinh viên" :span="2">
          <strong>{{ selectedApplication.studentName }}</strong> ({{ selectedApplication.studentCode }})
        </a-descriptions-item>
        <a-descriptions-item label="Phòng đã chọn" :span="2">
          <a-tag color="blue">{{ selectedApplication.assignedRoomNumber }}</a-tag>
          {{ selectedApplication.assignedBuildingName }}
        </a-descriptions-item>
        <a-descriptions-item label="Loại phòng">
          {{ selectedApplication.preferredRoomTypeName }}
        </a-descriptions-item>
        <a-descriptions-item label="Giá phòng">
          {{ formatCurrency(selectedApplication.preferredRoomPrice) }} VNĐ/tháng
        </a-descriptions-item>
        <a-descriptions-item label="Ngày bắt đầu">
          {{ formatDate(selectedApplication.requestedStartDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc">
          {{ formatDate(selectedApplication.requestedEndDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Loại SV">
          <a-tag :color="selectedApplication.isLocalStudent ? 'default' : 'orange'">
            {{ selectedApplication.isLocalStudent ? 'Nội tỉnh' : 'Ngoại tỉnh' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="getStatusColor(selectedApplication.status)">
            {{ getStatusText(selectedApplication.status) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Yêu cầu đặc biệt" :span="2">
          {{ selectedApplication.specialRequirements || 'Không có' }}
        </a-descriptions-item>
        <a-descriptions-item label="Ghi chú" :span="2">
          {{ selectedApplication.note || 'Không có' }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedApplication.reviewedByName" label="Người duyệt" :span="2">
          {{ selectedApplication.reviewedByName }} - {{ formatDate(selectedApplication.reviewedAt) }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedApplication.rejectReason" label="Lý do từ chối" :span="2">
          <a-alert type="error" :message="selectedApplication.rejectReason" show-icon />
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="detailModalVisible = false">Đóng</a-button>
        <a-button 
          v-if="selectedApplication.status === 'Pending'" 
          type="primary" 
          @click="openApproveModal(selectedApplication)"
        >
          Duyệt đơn
        </a-button>
        <a-button 
          v-if="selectedApplication.status === 'Pending'" 
          danger 
          @click="openRejectModal(selectedApplication)"
        >
          Từ chối
        </a-button>
      </div>
    </a-modal>

    <!-- Approve Modal -->
    <a-modal
      v-model:open="approveModalVisible"
      title="Duyệt đơn đăng ký"
      @ok="handleApprove"
      :confirmLoading="approving"
    >
      <a-alert
        message="Xác nhận duyệt đơn"
        description="Sau khi duyệt, hệ thống sẽ tự động tạo hợp đồng nháp và gửi email thông báo cho sinh viên."
        type="info"
        show-icon
        style="margin-bottom: 16px;"
      />
      <a-descriptions v-if="selectedApplication" bordered size="small">
        <a-descriptions-item label="Sinh viên">
          {{ selectedApplication.studentName }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          {{ selectedApplication.assignedRoomNumber }}
        </a-descriptions-item>
        <a-descriptions-item label="Giá" :span="2">
          {{ formatCurrency(selectedApplication.preferredRoomPrice) }} VNĐ/tháng
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>

    <!-- Reject Modal -->
    <a-modal
      v-model:open="rejectModalVisible"
      title="Từ chối đơn đăng ký"
      @ok="handleReject"
      :confirmLoading="rejecting"
    >
      <a-form layout="vertical">
        <a-form-item label="Lý do từ chối *" required>
          <a-textarea
            v-model:value="rejectReason"
            :rows="4"
            placeholder="Nhập lý do từ chối đơn đăng ký..."
          />
        </a-form-item>
      </a-form>
      <a-alert
        message="Sinh viên sẽ nhận được email thông báo từ chối kèm lý do."
        type="warning"
        show-icon
      />
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  ReloadOutlined, ClockCircleOutlined, CheckCircleOutlined, CloseCircleOutlined,
  FileTextOutlined, EyeOutlined, CheckOutlined, CloseOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { useAuthStore } from '@/stores/auth'
import dayjs from 'dayjs'

const authStore = useAuthStore()
const loading = ref(false)
const searchText = ref('')
const activeTab = ref('pending')
const applications = ref([])
const detailModalVisible = ref(false)
const approveModalVisible = ref(false)
const rejectModalVisible = ref(false)
const selectedApplication = ref(null)
const approving = ref(false)
const rejecting = ref(false)
const rejectReason = ref('')

const columns = [
  { title: 'Sinh viên', key: 'student', width: 180, fixed: 'left' },
  { title: 'Phòng', key: 'room', width: 150 },
  { title: 'Thời gian', key: 'dates', width: 150 },
  { title: 'Loại SV', key: 'priority', width: 100 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Ngày tạo', dataIndex: 'createdAt', width: 120, customRender: ({ text }) => formatDate(text) },
  { title: 'Thao tác', key: 'actions', width: 200, fixed: 'right' }
]

const stats = computed(() => ({
  pending: applications.value.filter(a => a.status === 'Pending').length,
  approved: applications.value.filter(a => a.status === 'Approved').length,
  rejected: applications.value.filter(a => a.status === 'Rejected').length,
  total: applications.value.length
}))

const filteredApplications = computed(() => {
  let filtered = applications.value

  // Filter by tab
  if (activeTab.value !== 'all') {
    const statusMap = {
      pending: 'Pending',
      approved: 'Approved',
      rejected: 'Rejected'
    }
    filtered = filtered.filter(a => a.status === statusMap[activeTab.value])
  }

  // Filter by search
  if (searchText.value) {
    const search = searchText.value.toLowerCase()
    filtered = filtered.filter(a =>
      a.studentName?.toLowerCase().includes(search) ||
      a.studentCode?.toLowerCase().includes(search) ||
      a.assignedRoomNumber?.toLowerCase().includes(search)
    )
  }

  return filtered
})

const loadApplications = async () => {
  loading.value = true
  try {
    applications.value = await roomApplicationService.getAll()
    // Sort by created date DESC
    applications.value.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
  } catch (error) {
    message.error('Lỗi tải danh sách đơn đăng ký')
    console.error(error)
  } finally {
    loading.value = false
  }
}

const handleTabChange = () => {
  // Tab changed, data already filtered by computed
}

const handleSearch = () => {
  // Search triggered, data already filtered by computed
}

const viewDetail = (record) => {
  selectedApplication.value = record
  detailModalVisible.value = true
}

const openApproveModal = (record) => {
  selectedApplication.value = record
  detailModalVisible.value = false
  approveModalVisible.value = true
}

const openRejectModal = (record) => {
  selectedApplication.value = record
  rejectReason.value = ''
  detailModalVisible.value = false
  rejectModalVisible.value = true
}

const handleApprove = async () => {
  if (!selectedApplication.value) return

  approving.value = true
  try {
    await roomApplicationService.approve(selectedApplication.value.id, {
      reviewedByUserId: authStore.user.id,
      reviewedByName: authStore.user.fullName
    })

    message.success('Đã duyệt đơn đăng ký thành công! Hợp đồng nháp đã được tạo.')
    approveModalVisible.value = false
    await loadApplications()
  } catch (error) {
    message.error(error.message || 'Lỗi duyệt đơn')
  } finally {
    approving.value = false
  }
}

const handleReject = async () => {
  if (!selectedApplication.value) return
  if (!rejectReason.value.trim()) {
    message.warning('Vui lòng nhập lý do từ chối')
    return
  }

  rejecting.value = true
  try {
    await roomApplicationService.reject(selectedApplication.value.id, {
      reviewedByUserId: authStore.user.id,
      reviewedByName: authStore.user.fullName,
      rejectReason: rejectReason.value
    })

    message.success('Đã từ chối đơn đăng ký')
    rejectModalVisible.value = false
    await loadApplications()
  } catch (error) {
    message.error(error.message || 'Lỗi từ chối đơn')
  } finally {
    rejecting.value = false
  }
}

const getStatusColor = (status) => {
  const colors = {
    Pending: 'orange',
    Approved: 'green',
    Rejected: 'red',
    UnderReview: 'blue'
  }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    Pending: 'Chờ xử lý',
    Approved: 'Đã duyệt',
    Rejected: 'Từ chối',
    UnderReview: 'Đang xem xét'
  }
  return texts[status] || status
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const getDaysWaiting = (createdAt) => {
  return dayjs().diff(dayjs(createdAt), 'day')
}

onMounted(() => {
  loadApplications()
})
</script>
