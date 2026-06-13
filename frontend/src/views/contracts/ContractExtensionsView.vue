<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Yêu cầu Gia hạn Hợp đồng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Xử lý yêu cầu gia hạn hợp đồng từ sinh viên
        </p>
      </div>
    </div>

    <!-- Statistics Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px">
      <a-col :xs="24" :sm="12" :md="8">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Tổng yêu cầu"
            :value="extensions.length"
            :value-style="{ color: '#1890ff' }"
          >
            <template #prefix>
              <FileTextOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="8">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Hợp đồng đang hoạt động"
            :value="activeContractsCount"
            :value-style="{ color: '#52c41a' }"
          >
            <template #prefix>
              <CheckCircleOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="8">
        <a-card :bordered="false" style="box-shadow: 0 1px 2px rgba(0,0,0,0.03); border: 1px solid #e5e7eb">
          <a-statistic
            title="Gia hạn trong tháng"
            :value="extensionsThisMonth"
            :value-style="{ color: '#faad14' }"
          >
            <template #prefix>
              <CalendarOutlined />
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
            placeholder="Tìm theo tên, mã SV, mã HĐ..."
            allowClear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-button
            v-if="search"
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
        :data-source="filteredExtensions"
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
          <template v-else-if="column.key === 'contract'">
            <div style="font-weight: 500; color: #1890ff">{{ record.contractCode }}</div>
          </template>
          <template v-else-if="column.key === 'dates'">
            <div style="text-align: center">
              <div style="font-weight: 500; color: #ff4d4f">{{ formatDate(record.oldEndDate) }}</div>
              <ArrowDownOutlined style="color: #1890ff; margin: 4px 0" />
              <div style="font-weight: 600; color: #4caf50">{{ formatDate(record.newEndDate) }}</div>
            </div>
          </template>
          <template v-else-if="column.key === 'extension'">
            {{ calculateExtensionMonths(record.oldEndDate, record.newEndDate) }} tháng
          </template>
          <template v-else-if="column.key === 'approvedAt'">
            <div>{{ formatDateTime(record.approvedAt) }}</div>
            <div style="font-size: 12px; color: #8c8c8c">{{ record.approvedByName }}</div>
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space size="small">
              <a-button type="link" @click="viewDetail(record)">Chi tiết</a-button>
              <a-popconfirm
                title="Bạn có chắc muốn xóa yêu cầu này?"
                ok-text="Xóa"
                cancel-text="Hủy"
                @confirm="handleDelete(record.id)"
              >
                <a-button type="text" danger>Xóa</a-button>
              </a-popconfirm>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Detail Dialog -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết yêu cầu gia hạn"
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
        <a-descriptions-item label="Mã hợp đồng">
          <strong style="color: #1890ff">{{ detailTarget.contractCode }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc cũ">
          <span style="color: #ff4d4f">{{ formatDate(detailTarget.oldEndDate) }}</span>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc mới">
          <span style="color: #4caf50; font-weight: 600">{{ formatDate(detailTarget.newEndDate) }}</span>
        </a-descriptions-item>
        <a-descriptions-item label="Thời gian gia hạn">
          <strong>{{ calculateExtensionMonths(detailTarget.oldEndDate, detailTarget.newEndDate) }} tháng</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Lý do">
          {{ detailTarget.reason || 'Không có lý do' }}
        </a-descriptions-item>
        <a-descriptions-item label="Người phê duyệt">
          <strong>{{ detailTarget.approvedByName }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày phê duyệt">
          {{ formatDateTime(detailTarget.approvedAt) }}
        </a-descriptions-item>
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
import {
  FileTextOutlined,
  CheckCircleOutlined,
  CalendarOutlined,
  ArrowDownOutlined
} from '@ant-design/icons-vue'
import { contractExtensionService } from '@/services/contractExtensionService'

const columns = [
  { title: 'Sinh viên', key: 'student', width: 220 },
  { title: 'Mã HĐ', key: 'contract', align: 'center', width: 120 },
  { title: 'Ngày cũ → Ngày mới', key: 'dates', align: 'center', width: 180 },
  { title: 'Gia hạn', key: 'extension', align: 'center', width: 100 },
  { title: 'Ngày phê duyệt', key: 'approvedAt', align: 'center', width: 180 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 180 },
]

const extensions = ref([])
const search = ref('')
const loading = ref(false)
const detailDialog = ref(false)
const detailTarget = ref(null)

const filteredExtensions = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  if (!keyword) return extensions.value
  
  return extensions.value.filter((item) => {
    return (
      item.studentName?.toLowerCase().includes(keyword) ||
      item.studentCode?.toLowerCase().includes(keyword) ||
      item.contractCode?.toLowerCase().includes(keyword)
    )
  })
})

const activeContractsCount = computed(() => {
  // Get unique contract IDs
  const contractIds = new Set(extensions.value.map(e => e.contractId))
  return contractIds.size
})

const extensionsThisMonth = computed(() => {
  const now = new Date()
  const currentMonth = now.getMonth()
  const currentYear = now.getFullYear()
  
  return extensions.value.filter(item => {
    const approvedDate = new Date(item.approvedAt)
    return approvedDate.getMonth() === currentMonth && approvedDate.getFullYear() === currentYear
  }).length
})

function resetFilters() {
  search.value = ''
}

async function loadExtensions() {
  loading.value = true
  try {
    extensions.value = await contractExtensionService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách yêu cầu gia hạn')
  } finally {
    loading.value = false
  }
}

function viewDetail(item) {
  detailTarget.value = item
  detailDialog.value = true
}

async function handleDelete(id) {
  try {
    await contractExtensionService.delete(id)
    message.success('Đã xóa yêu cầu gia hạn')
    await loadExtensions()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
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

function calculateExtensionMonths(oldEndDate, newEndDate) {
  if (!oldEndDate || !newEndDate) return 0
  
  const old = new Date(oldEndDate)
  const newD = new Date(newEndDate)
  
  const diffTime = newD - old
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  const diffMonths = Math.round(diffDays / 30)
  
  return diffMonths
}

onMounted(() => {
  loadExtensions()
})
</script>
