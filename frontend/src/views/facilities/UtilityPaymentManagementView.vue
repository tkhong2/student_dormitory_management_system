<template>
  <div>
    <!-- Header -->
    <div style="margin-bottom: 24px;">
      <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 8px 0;">Quản lý Thanh toán Tiện ích</h1>
      <p style="font-size: 14px; color: #8c8c8c; margin: 0;">Quản lý và theo dõi thanh toán sử dụng tiện ích chung</p>
    </div>

    <!-- Statistics Cards -->
    <a-row :gutter="16" style="margin-bottom: 24px;">
      <a-col :xs="24" :sm="6">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
          <a-statistic
            title="Tổng booking"
            :value="stats.total"
            :value-style="{ color: '#fff', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #prefix>
              <CalendarOutlined />
            </template>
            <template #title>
              <span style="color: rgba(255,255,255,0.9); font-size: 13px;">Tổng booking</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="6">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
          <a-statistic
            title="Chưa thanh toán"
            :value="stats.unpaid"
            :value-style="{ color: '#fff', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #prefix>
              <CloseCircleOutlined />
            </template>
            <template #title>
              <span style="color: rgba(255,255,255,0.9); font-size: 13px;">Chưa thanh toán</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="6">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
          <a-statistic
            title="Đã thanh toán"
            :value="stats.paid"
            :value-style="{ color: '#fff', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #prefix>
              <CheckCircleOutlined />
            </template>
            <template #title>
              <span style="color: rgba(255,255,255,0.9); font-size: 13px;">Đã thanh toán</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="6">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);">
          <a-statistic
            title="Tổng doanh thu"
            :value="stats.revenue"
            :precision="0"
            suffix="đ"
            :value-style="{ color: '#fff', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #prefix>
              <DollarOutlined />
            </template>
            <template #title>
              <span style="color: rgba(255,255,255,0.9); font-size: 13px;">Tổng doanh thu</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters -->
    <a-card style="margin-bottom: 16px;" :bordered="false">
      <a-row :gutter="16">
        <a-col :xs="24" :sm="8">
          <a-input
            v-model:value="filters.search"
            placeholder="Tìm theo sinh viên, phòng..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input>
        </a-col>
        <a-col :xs="24" :sm="6">
          <a-select
            v-model:value="filters.utilityId"
            placeholder="Loại tiện ích"
            allow-clear
            style="width: 100%"
            @change="loadUsageLogs"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option v-for="util in utilities" :key="util.id" :value="util.id">
              {{ util.name }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="6">
          <a-select
            v-model:value="filters.paymentStatus"
            placeholder="Trạng thái thanh toán"
            style="width: 100%"
            @change="loadUsageLogs"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="paid">Đã thanh toán</a-select-option>
            <a-select-option value="unpaid">Chưa thanh toán</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="4">
          <a-button type="primary" block @click="loadUsageLogs">
            <template #icon><ReloadOutlined /></template>
            Làm mới
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table -->
    <a-card :bordered="false" :loading="loading">
      <template #title>
        <span style="font-weight: 600;">Danh sách Booking</span>
      </template>
      
      <a-table
        :columns="columns"
        :data-source="filteredLogs"
        :pagination="{ pageSize: 10 }"
        row-key="id"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <div style="font-weight: 500;">{{ record.studentName || 'N/A' }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.roomNumber || '-' }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'utility'">
            <a-tag color="blue">{{ record.utilityName }}</a-tag>
          </template>

          <template v-else-if="column.key === 'time'">
            <div>
              <div><CalendarOutlined /> {{ formatDateTime(record.startTime) }}</div>
              <div v-if="record.endTime" style="font-size: 12px; color: #8c8c8c;">
                → {{ formatDateTime(record.endTime) }}
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'duration'">
            {{ calculateDuration(record) }}
          </template>

          <template v-else-if="column.key === 'fee'">
            <strong :style="{ color: record.feeCharged > 0 ? '#1890ff' : '#8c8c8c' }">
              {{ record.feeCharged ? formatCurrency(record.feeCharged) : 'Miễn phí' }}
            </strong>
          </template>

          <template v-else-if="column.key === 'paymentStatus'">
            <a-tag :color="record.isPaid ? 'success' : 'warning'">
              {{ record.isPaid ? 'Đã thanh toán' : 'Chưa thanh toán' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button
                size="small"
                @click="viewDetail(record)"
              >
                <template #icon><EyeOutlined /></template>
                Chi tiết
              </a-button>
              <a-button
                v-if="!record.isPaid && record.feeCharged > 0"
                type="primary"
                size="small"
                @click="confirmPayment(record)"
              >
                <template #icon><CheckOutlined /></template>
                Xác nhận TT
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết Booking"
      width="700px"
      :footer="null"
    >
      <a-descriptions v-if="selectedLog" bordered :column="2" size="default">
        <a-descriptions-item label="ID" :span="2">
          <a-typography-text strong copyable>{{ selectedLog.id }}</a-typography-text>
        </a-descriptions-item>

        <a-descriptions-item label="Sinh viên">
          {{ selectedLog.studentName || 'N/A' }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          {{ selectedLog.roomNumber || '-' }}
        </a-descriptions-item>

        <a-descriptions-item label="Tiện ích" :span="2">
          <a-tag color="blue">{{ selectedLog.utilityName }}</a-tag>
        </a-descriptions-item>

        <a-descriptions-item label="Thời gian bắt đầu">
          {{ formatDateTime(selectedLog.startTime) }}
        </a-descriptions-item>
        <a-descriptions-item label="Thời gian kết thúc">
          {{ selectedLog.endTime ? formatDateTime(selectedLog.endTime) : 'Chưa kết thúc' }}
        </a-descriptions-item>

        <a-descriptions-item label="Thời lượng">
          {{ calculateDuration(selectedLog) }}
        </a-descriptions-item>
        <a-descriptions-item label="Phí">
          <strong style="color: #1890ff; font-size: 16px;">
            {{ selectedLog.feeCharged ? formatCurrency(selectedLog.feeCharged) : 'Miễn phí' }}
          </strong>
        </a-descriptions-item>

        <a-descriptions-item label="Trạng thái thanh toán" :span="2">
          <a-tag :color="selectedLog.isPaid ? 'success' : 'warning'" style="font-size: 14px; padding: 4px 12px;">
            {{ selectedLog.isPaid ? 'Đã thanh toán' : 'Chưa thanh toán' }}
          </a-tag>
        </a-descriptions-item>

        <a-descriptions-item v-if="selectedLog.notes" label="Ghi chú" :span="2">
          <a-alert type="info" :message="selectedLog.notes" show-icon />
        </a-descriptions-item>

        <a-descriptions-item label="Ngày tạo" :span="2">
          {{ formatDateTime(selectedLog.createdAt) }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: space-between;">
        <a-button @click="detailDialog = false">Đóng</a-button>
        <a-button
          v-if="!selectedLog.isPaid && selectedLog.feeCharged > 0"
          type="primary"
          @click="detailDialog = false; confirmPayment(selectedLog)"
        >
          <template #icon><CheckOutlined /></template>
          Xác nhận thanh toán
        </a-button>
      </div>
    </a-modal>

    <!-- Confirm Payment Modal -->
    <a-modal
      v-model:open="confirmDialog"
      title="Xác nhận thanh toán"
      @ok="markAsPaid"
      :confirmLoading="submitting"
    >
      <p>Xác nhận đã nhận thanh toán cho booking này?</p>
      <a-descriptions v-if="selectedLog" bordered size="small">
        <a-descriptions-item label="Sinh viên" :span="2">
          {{ selectedLog.studentName }}
        </a-descriptions-item>
        <a-descriptions-item label="Tiện ích" :span="2">
          {{ selectedLog.utilityName }}
        </a-descriptions-item>
        <a-descriptions-item label="Số tiền" :span="2">
          <strong style="color: #1890ff; font-size: 16px;">{{ formatCurrency(selectedLog.feeCharged) }}</strong>
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  CalendarOutlined,
  CloseCircleOutlined,
  CheckCircleOutlined,
  DollarOutlined,
  SearchOutlined,
  ReloadOutlined,
  EyeOutlined,
  CheckOutlined
} from '@ant-design/icons-vue'
import * as utilityService from '@/services/sharedUtilityService'
import dayjs from 'dayjs'

const loading = ref(false)
const usageLogs = ref([])
const utilities = ref([])
const selectedLog = ref(null)
const detailDialog = ref(false)
const confirmDialog = ref(false)
const submitting = ref(false)

const filters = ref({
  search: '',
  utilityId: '',
  paymentStatus: ''
})

const columns = [
  { title: 'ID', dataIndex: 'id', key: 'id', width: 60, fixed: 'left' },
  { title: 'Sinh viên / Phòng', key: 'student', width: 150 },
  { title: 'Tiện ích', key: 'utility', width: 130 },
  { title: 'Thời gian', key: 'time', width: 180 },
  { title: 'Thời lượng', key: 'duration', width: 100, align: 'center' },
  { title: 'Phí', key: 'fee', width: 120, align: 'right' },
  { title: 'Thanh toán', key: 'paymentStatus', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 200, fixed: 'right', align: 'center' }
]

const stats = computed(() => {
  const total = usageLogs.value.length
  const paid = usageLogs.value.filter(log => log.isPaid).length
  const unpaid = usageLogs.value.filter(log => !log.isPaid && log.feeCharged > 0).length
  const revenue = usageLogs.value
    .filter(log => log.isPaid)
    .reduce((sum, log) => sum + (log.feeCharged || 0), 0)

  return { total, paid, unpaid, revenue }
})

const filteredLogs = computed(() => {
  let result = usageLogs.value

  // Filter by search
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    result = result.filter(log =>
      log.studentName?.toLowerCase().includes(search) ||
      log.roomNumber?.toLowerCase().includes(search) ||
      log.utilityName?.toLowerCase().includes(search)
    )
  }

  // Filter by utility
  if (filters.value.utilityId) {
    result = result.filter(log => log.sharedUtilityId === filters.value.utilityId)
  }

  // Filter by payment status
  if (filters.value.paymentStatus === 'paid') {
    result = result.filter(log => log.isPaid)
  } else if (filters.value.paymentStatus === 'unpaid') {
    result = result.filter(log => !log.isPaid)
  }

  return result
})

onMounted(async () => {
  await Promise.all([
    loadUtilities(),
    loadUsageLogs()
  ])
})

const loadUtilities = async () => {
  try {
    utilities.value = await utilityService.getAllUtilities()
  } catch (error) {
    console.error('Error loading utilities:', error)
  }
}

const loadUsageLogs = async () => {
  loading.value = true
  try {
    usageLogs.value = await utilityService.getAllUsageLogs()
  } catch (error) {
    message.error('Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const viewDetail = (record) => {
  selectedLog.value = record
  detailDialog.value = true
}

const confirmPayment = (record) => {
  selectedLog.value = record
  confirmDialog.value = true
}

const markAsPaid = async () => {
  if (!selectedLog.value) return

  submitting.value = true
  try {
    await utilityService.markUsageLogAsPaid(selectedLog.value.id)
    message.success('Đã xác nhận thanh toán')
    confirmDialog.value = false
    await loadUsageLogs()
  } catch (error) {
    message.error('Lỗi xác nhận thanh toán')
  } finally {
    submitting.value = false
  }
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

const formatDateTime = (date) => {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

const calculateDuration = (log) => {
  if (!log.endTime) return '-'
  const minutes = dayjs(log.endTime).diff(dayjs(log.startTime), 'minute')
  if (minutes < 60) return `${minutes} phút`
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  return `${hours}h${mins > 0 ? ` ${mins}p` : ''}`
}
</script>

<style scoped>
.ant-card {
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}
</style>
