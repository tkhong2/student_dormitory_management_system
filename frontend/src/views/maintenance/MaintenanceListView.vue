<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">
          Yêu cầu Bảo trì
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Tổng số: {{ requests.length }} yêu cầu - {{ pendingCount }} đang chờ
        </p>
      </div>
      <a-button type="primary" style="background: #ff9800; border-color: #ff9800;">
        + Tạo yêu cầu
      </a-button>
    </div>

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
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="Chờ xử lý">Chờ xử lý</a-select-option>
            <a-select-option value="Đang xử lý">Đang xử lý</a-select-option>
            <a-select-option value="Hoàn thành">Hoàn thành</a-select-option>
          </a-select>
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
        <a-col :xs="24" :sm="12" :md="4">
          <a-button v-if="hasFilters" @click="resetFilters" block>
            <template #icon><ReloadOutlined /></template>
            Đặt lại
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
        :columns="columns"
        :data-source="filteredRequests"
        row-key="id"
        :pagination="{ pageSize: 10 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'title'">
            <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
              <a-avatar
                size="40"
                :style="{ backgroundColor: priColor(record.priority) }"
              >
                <template #icon>
                  <WarningOutlined v-if="record.priority === 'Khẩn cấp'" />
                  <ToolOutlined v-else />
                </template>
              </a-avatar>
              <div>
                <div style="font-weight: 600">{{ record.title }}</div>
                <div style="font-size: 12px; color: #8c8c8c">#{{ record.code }}</div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'description'">
            <a-typography-paragraph
              :ellipsis="{ rows: 2, tooltip: true }"
              style="margin: 0; max-width: 300px"
            >
              {{ record.desc }}
            </a-typography-paragraph>
          </template>
          <template v-else-if="column.key === 'priority'">
            <a-tag v-if="record.priority === 'Khẩn cấp'" color="red">
              Khẩn cấp
            </a-tag>
            <span v-else style="color: #8c8c8c">Bình thường</span>
          </template>
          <template v-else-if="column.key === 'status'">
            <a-tag :color="stColor(record.status)">{{ record.status }}</a-tag>
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button
                v-if="record.status === 'Chờ xử lý'"
                type="primary"
                size="small"
              >
                Xử lý
              </a-button>
              <a-button
                v-if="record.status === 'Đang xử lý'"
                type="primary"
                size="small"
                style="background: #52c41a; border-color: #52c41a"
              >
                Hoàn thành
              </a-button>
              <a-button type="link" size="small">
                Chi tiết
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  SearchOutlined,
  ReloadOutlined,
  WarningOutlined,
  ToolOutlined,
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import maintenanceRequestService from '@/services/maintenanceRequestService'

const loading = ref(false)
const requests = ref([])
const searchKeyword = ref('')
const statusFilter = ref('')
const roomFilter = ref(undefined)

const columns = [
  { title: 'Yêu cầu', key: 'title', width: 250 },
  { title: 'Mô tả', key: 'description', width: 300 },
  { title: 'Phòng', dataIndex: 'room', key: 'room', align: 'center', width: 100 },
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Ngày tạo', dataIndex: 'date', key: 'date', align: 'center', width: 120 },
  { title: 'Mức độ', key: 'priority', align: 'center', width: 120 },
  { title: 'Trạng thái', key: 'status', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 200 },
]

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

const hasFilters = computed(() => searchKeyword.value || statusFilter.value || roomFilter.value)

const pendingCount = computed(() => requests.value.filter((r) => r.status === 'Chờ xử lý').length)

const filteredRequests = computed(() => {
  const keyword = searchKeyword.value.trim().toLowerCase()

  return requests.value.filter((r) => {
    const matchesKeyword =
      !keyword ||
      r.title.toLowerCase().includes(keyword) ||
      r.desc.toLowerCase().includes(keyword) ||
      r.code.toLowerCase().includes(keyword)
    const matchesRoom = !roomFilter.value || r.room === roomFilter.value
    const matchesStatus = !statusFilter.value || r.status === statusFilter.value
    return matchesKeyword && matchesRoom && matchesStatus
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
    message.error(err.message || 'Không thể tải danh sách yêu cầu bảo trì')
  } finally {
    loading.value = false
  }
}

function resetFilters() {
  searchKeyword.value = ''
  statusFilter.value = ''
  roomFilter.value = undefined
}

const stColor = (s) =>
  ({ 'Chờ xử lý': 'orange', 'Đang xử lý': 'blue', 'Hoàn thành': 'green' })[s] || 'default'

const priColor = (p) => (p === 'Khẩn cấp' ? '#ff4d4f' : '#1890ff')

onMounted(loadData)
</script>
