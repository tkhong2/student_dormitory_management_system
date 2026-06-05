<template>
  <div>
    <PageHeaderAnt title="Yêu cầu sửa chữa" subtitle="Quản lý yêu cầu bảo trì phòng">
    </PageHeaderAnt>

    <!-- Filters -->
    <a-card :bordered="false" style="border-radius: 12px; margin-bottom: 16px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-row :gutter="16">
        <a-col :xs="24" :sm="12" :lg="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm phòng..."
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            :options="statusOptions"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-select
            v-model:value="priorityFilter"
            placeholder="Mức độ ưu tiên"
            :options="priorityOptions"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-button block @click="loadData">
            <template #icon><SearchOutlined /></template>
            Tìm kiếm
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Stats -->
    <a-row :gutter="16" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="12" :lg="6">
        <a-statistic 
          title="Chờ xử lý" 
          :value="requests.filter(x => x.status === 'Pending').length"
          :value-style="{ color: '#faad14' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="6">
        <a-statistic 
          title="Đang xử lý" 
          :value="requests.filter(x => x.status === 'In Progress').length"
          :value-style="{ color: '#1890ff' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="6">
        <a-statistic 
          title="Hoàn thành" 
          :value="requests.filter(x => x.status === 'Completed').length"
          :value-style="{ color: '#52c41a' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="6">
        <a-statistic 
          title="Tổng cộng" 
          :value="requests.length"
          :value-style="{ color: '#000' }"
        />
      </a-col>
    </a-row>

    <!-- Table -->
    <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-table
        :columns="columns"
        :data-source="filteredRequests"
        :loading="loading"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} yêu cầu` }"
        size="small"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'priority'">
            <a-tag 
              :color="
                record.priority === 'High' ? 'red' :
                record.priority === 'Medium' ? 'orange' :
                'blue'
              "
            >
              {{ record.priority }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag 
              :color="
                record.status === 'Pending' ? 'orange' :
                record.status === 'In Progress' ? 'blue' :
                record.status === 'Completed' ? 'green' :
                'red'
              "
            >
              {{ record.status }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Chi tiết">
                <a-button type="text" size="small" @click="viewDetail(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-dropdown v-if="record.status !== 'Completed'">
                <template #overlay>
                  <a-menu @click="({ key }) => updateStatus(record, key)">
                    <a-menu-item key="In Progress">Đang xử lý</a-menu-item>
                    <a-menu-item key="Completed">Hoàn thành</a-menu-item>
                    <a-menu-divider />
                    <a-menu-item key="Cancelled">Hủy</a-menu-item>
                  </a-menu>
                </template>
                <a-button type="text" size="small">
                  <template #icon><EditOutlined /></template>
                </a-button>
              </a-dropdown>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'

const loading = ref(false)
const search = ref('')
const statusFilter = ref(null)
const priorityFilter = ref(null)
const requests = ref([])

const statusOptions = [
  { label: 'Chờ xử lý', value: 'Pending' },
  { label: 'Đang xử lý', value: 'In Progress' },
  { label: 'Hoàn thành', value: 'Completed' },
  { label: 'Hủy', value: 'Cancelled' },
]

const priorityOptions = [
  { label: 'Cao', value: 'High' },
  { label: 'Trung bình', value: 'Medium' },
  { label: 'Thấp', value: 'Low' },
]

const columns = [
  { title: 'Phòng', dataIndex: 'room', key: 'room', width: 100, align: 'center' },
  { title: 'Mô tả', dataIndex: 'description', key: 'description', width: 250 },
  { title: 'Mức độ', dataIndex: 'priority', key: 'priority', width: 120, align: 'center' },
  { title: 'Trạng thái', dataIndex: 'status', key: 'status', width: 130, align: 'center' },
  { title: 'Ngày yêu cầu', dataIndex: 'createdDate', key: 'createdDate', width: 120, align: 'center' },
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Thao tác', key: 'actions', width: 150, align: 'center', fixed: 'right' },
]

const filteredRequests = computed(() => {
  return requests.value.filter(item => {
    const matchesSearch = !search.value || 
      item.room.includes(search.value) ||
      item.description.toLowerCase().includes(search.value.toLowerCase())
    const matchesStatus = !statusFilter.value || item.status === statusFilter.value
    const matchesPriority = !priorityFilter.value || item.priority === priorityFilter.value
    return matchesSearch && matchesStatus && matchesPriority
  })
})

async function loadData() {
  loading.value = true
  try {
    requests.value = [
      { key: 1, room: '101', description: 'Cửa bị hỏng', priority: 'High', status: 'Pending', createdDate: '05/06/2026', student: 'Trần Văn A' },
      { key: 2, room: '102', description: 'Điều hòa không lạnh', priority: 'Medium', status: 'In Progress', createdDate: '04/06/2026', student: 'Lê Thị B' },
      { key: 3, room: '103', description: 'Bóng đèn cháy', priority: 'Low', status: 'Completed', createdDate: '03/06/2026', student: 'Phạm Văn C' },
      { key: 4, room: '201', description: 'Vòi nước bị rò', priority: 'High', status: 'Pending', createdDate: '05/06/2026', student: 'Nguyễn Thị D' },
    ]
  } catch (error) {
    message.error('Lỗi khi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

function viewDetail(record) {
  message.info(`Xem chi tiết: Phòng ${record.room} - ${record.description}`)
}

function updateStatus(record, newStatus) {
  record.status = newStatus
  message.success(`Cập nhật trạng thái: ${newStatus}`)
}

onMounted(loadData)
</script>
