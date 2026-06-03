<template>
  <div>
    <!-- Page Header -->
    <div style="background: #fff; margin-bottom: 16px; border-radius: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); padding: 16px 24px">
      <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 4px 0; color: #000">
        Yêu Cầu Bảo Trì
      </h1>
      <p style="font-size: 14px; color: #8c8c8c; margin: 0">
        {{ requests.length }} yêu cầu - {{ pendingCount }} đang chờ
      </p>
    </div>

    <!-- Status Tabs -->
    <a-tabs v-model:activeKey="tab" style="margin-bottom: 16px" size="large">
      <a-tab-pane key="all" tab="Tất cả" />
      <a-tab-pane key="pending">
        <template #tab>
          Chờ xử lý
          <a-badge :count="pendingCount" :number-style="{ backgroundColor: '#faad14', marginLeft: '8px' }" />
        </template>
      </a-tab-pane>
      <a-tab-pane key="progress" tab="Đang xử lý" />
      <a-tab-pane key="done" tab="Hoàn thành" />
    </a-tabs>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="searchKeyword"
            placeholder="Tìm tiêu đề, mô tả..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="roomFilter"
            placeholder="Phòng"
            allow-clear
            style="width: 100%"
          >
            <a-select-option v-for="room in roomOptions" :key="room" :value="room">
              {{ room }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="priorityFilter"
            placeholder="Mức độ ưu tiên"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="Bình thường">Bình thường</a-select-option>
            <a-select-option value="Khẩn cấp">Khẩn cấp</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="4" style="text-align: right">
          <a-button v-if="hasFilters" @click="resetFilters" block>
            <template #icon><ReloadOutlined /></template>
            Đặt lại
          </a-button>
          <a-typography-text v-else type="secondary" style="line-height: 32px">
            Kết quả: {{ filteredRequests.length }}/{{ requests.length }}
          </a-typography-text>
        </a-col>
      </a-row>
    </a-card>

    <!-- Request Cards Grid -->
    <a-row :gutter="[16, 16]" v-if="!loading && filteredRequests.length > 0">
      <a-col :xs="24" :md="12" v-for="r in filteredRequests" :key="r.id">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: flex-start; justify-content: space-between; margin-bottom: 12px">
            <div style="display: flex; gap: 12px; align-items: flex-start">
              <a-avatar
                :size="48"
                :style="{ backgroundColor: priColor(r.priority) }"
              >
                <template #icon>
                  <WarningOutlined v-if="r.priority === 'Khẩn cấp'" />
                  <ToolOutlined v-else />
                </template>
              </a-avatar>
              <div>
                <a-typography-title :level="5" style="margin: 0">
                  {{ r.title }}
                </a-typography-title>
                <a-typography-text type="secondary" style="font-size: 12px">
                  #{{ r.code }} · Phòng {{ r.room }} · {{ r.date }}
                </a-typography-text>
              </div>
            </div>
            <a-tag :color="stColor(r.status)">{{ r.status }}</a-tag>
          </div>

          <a-typography-paragraph
            :ellipsis="{ rows: 2 }"
            type="secondary"
            style="margin-bottom: 8px"
          >
            {{ r.desc }}
          </a-typography-paragraph>

          <a-typography-text v-if="r.note" type="secondary" style="font-size: 12px">
            Ghi chú: {{ r.note }}
          </a-typography-text>

          <a-divider style="margin: 12px 0" />

          <div style="display: flex; align-items: center; justify-content: space-between">
            <div style="display: flex; align-items: center; gap: 8px">
              <UserOutlined style="color: #8c8c8c" />
              <a-typography-text type="secondary" style="font-size: 13px">
                {{ r.student }}
              </a-typography-text>
              <a-tag v-if="r.priority === 'Khẩn cấp'" color="red" size="small">
                Khẩn cấp
              </a-tag>
            </div>
            <a-space>
              <a-button
                v-if="r.status === 'Chờ xử lý'"
                type="primary"
                size="small"
              >
                Xử lý
              </a-button>
              <a-button
                v-if="r.status === 'Đang xử lý'"
                type="primary"
                size="small"
                style="background: #52c41a; border-color: #52c41a"
              >
                Hoàn thành
              </a-button>
              <a-button type="text" size="small">
                <template #icon><EyeOutlined /></template>
              </a-button>
            </a-space>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Loading State -->
    <a-card v-if="loading" :bordered="false">
      <a-skeleton active :paragraph="{ rows: 4 }" />
    </a-card>

    <!-- Empty State -->
    <a-empty
      v-if="!loading && filteredRequests.length === 0"
      description="Chưa có yêu cầu bảo trì nào"
      style="padding: 60px 0"
    >
      <a-button type="primary">Tạo yêu cầu mới</a-button>
    </a-empty>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  SearchOutlined,
  ReloadOutlined,
  WarningOutlined,
  ToolOutlined,
  UserOutlined,
  EyeOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import maintenanceRequestService from '@/services/maintenanceRequestService'

const loading = ref(false)
const requests = ref([])
const tab = ref('all')
const searchKeyword = ref('')
const roomFilter = ref(undefined)
const priorityFilter = ref(undefined)

const studentMap = {
  '30000000-0000-0000-0000-000000000001': 'Nguyễn Văn A',
  '30000000-0000-0000-0000-000000000002': 'Trần Thị B',
  '30000000-0000-0000-0000-000000000003': 'Lê Văn C'
}

const roomMap = {
  '40000000-0000-0000-0000-000000000101': 'A101',
  '40000000-0000-0000-0000-000000000102': 'A102',
  '40000000-0000-0000-0000-000000000103': 'A103'
}

const requestCode = (index) => `MT${String(index + 1).padStart(4, '0')}`

const roomOptions = computed(() =>
  [...new Set(requests.value.map((r) => r.room))].filter(Boolean).sort()
)

const hasFilters = computed(() => searchKeyword.value || roomFilter.value || priorityFilter.value)

const pendingCount = computed(() => requests.value.filter((r) => r.status === 'Chờ xử lý').length)

const statusFiltered = computed(() => {
  if (tab.value === 'all') return requests.value
  const m = {
    pending: 'Chờ xử lý',
    progress: 'Đang xử lý',
    done: 'Hoàn thành'
  }
  return requests.value.filter((r) => r.status === m[tab.value])
})

const filteredRequests = computed(() => {
  const keyword = searchKeyword.value.trim().toLowerCase()

  return statusFiltered.value.filter((r) => {
    const matchesKeyword =
      !keyword ||
      r.title.toLowerCase().includes(keyword) ||
      r.desc.toLowerCase().includes(keyword) ||
      r.code.toLowerCase().includes(keyword)
    const matchesRoom = !roomFilter.value || r.room === roomFilter.value
    const matchesPriority = !priorityFilter.value || r.priority === priorityFilter.value
    return matchesKeyword && matchesRoom && matchesPriority
  })
})

async function loadData() {
  loading.value = true
  try {
    const res = await maintenanceRequestService.getAll()
    requests.value = res.map((r, index) => ({
      id: r.id,
      code: requestCode(index),
      title: r.description?.slice(0, 50) || 'Yêu cầu bảo trì',
      room: roomMap[r.roomId] || 'N/A',
      student: studentMap[r.studentId] || 'Không xác định',
      date: new Date(r.createdAt).toLocaleDateString('vi-VN'),
      status:
        r.status === 'Completed'
          ? 'Hoàn thành'
          : r.status === 'InProgress'
            ? 'Đang xử lý'
            : 'Chờ xử lý',
      priority: r.status === 'Pending' ? 'Khẩn cấp' : 'Bình thường',
      desc: r.description || '',
      note: r.note || ''
    }))
  } catch (err) {
    console.error('Lỗi tải bảo trì:', err)
    message.error(err.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

function resetFilters() {
  searchKeyword.value = ''
  roomFilter.value = undefined
  priorityFilter.value = undefined
}

const stColor = (s) =>
  ({ 'Chờ xử lý': 'orange', 'Đang xử lý': 'blue', 'Hoàn thành': 'green' })[s] || 'default'

const priColor = (p) => (p === 'Khẩn cấp' ? '#ff4d4f' : '#1890ff')

onMounted(loadData)
</script>

<style scoped>
:deep(.ant-card-body) {
  padding: 20px;
}
</style>
