<template>
  <div>
    <!-- Page Header -->
    <div style="background: #fff; margin-bottom: 16px; border-radius: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); padding: 16px 24px">
      <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 4px 0; color: #000">
        Hóa Đơn & Thanh Toán
      </h1>
      <p style="font-size: 14px; color: #8c8c8c; margin: 0">
        Quản lý thu phí ký túc xá
      </p>
    </div>

    <!-- Revenue Summary -->
    <a-row :gutter="16" style="margin-bottom: 16px">
      <a-col :xs="24" :md="8">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white">
          <a-statistic
            title="Tổng Thu Tháng Này"
            :value="totalRevenue"
            :precision="0"
            suffix="₫"
            :value-style="{ color: 'white', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #formatter="{ value }">
              {{ formatCurrency(value) }}
            </template>
          </a-statistic>
          <div style="margin-top: 8px; opacity: 0.9; font-size: 13px">
            <ArrowUpOutlined /> +12% so với tháng trước
          </div>
        </a-card>
      </a-col>
      <a-col :xs="24" :md="8">
        <a-card :bordered="false">
          <a-statistic
            title="Chưa Thanh Toán"
            :value="unpaidAmount"
            :precision="0"
            suffix="₫"
            :value-style="{ color: '#faad14', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #formatter="{ value }">
              {{ formatCurrency(value) }}
            </template>
          </a-statistic>
          <div style="margin-top: 8px; color: #8c8c8c; font-size: 13px">
            {{ unpaidCount }} hóa đơn đang chờ
          </div>
        </a-card>
      </a-col>
      <a-col :xs="24" :md="8">
        <a-card :bordered="false">
          <a-statistic
            title="Quá Hạn"
            :value="overdueAmount"
            :precision="0"
            suffix="₫"
            :value-style="{ color: '#ff4d4f', fontSize: '28px', fontWeight: 'bold' }"
          >
            <template #formatter="{ value }">
              {{ formatCurrency(value) }}
            </template>
          </a-statistic>
          <div style="margin-top: 8px; color: #8c8c8c; font-size: 13px">
            {{ overdueCount }} hóa đơn quá hạn
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters & Table Card -->
    <a-card :bordered="false">
      <a-row :gutter="[16, 16]" style="margin-bottom: 16px">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm mã HĐ, sinh viên, phòng..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="monthFilter"
            placeholder="Tháng"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option v-for="month in months" :key="month" :value="month">
              {{ month }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="10">
          <a-segmented
            v-model:value="statusFilter"
            :options="statusFilterOptions"
            block
          />
        </a-col>
      </a-row>

      <a-table
        :columns="billingColumns"
        :data-source="filteredBills"
        :loading="loading"
        row-key="id"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} hóa đơn` }"
        :scroll="{ x: 1000 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'code'">
            <a-typography-text strong copyable>{{ record.code }}</a-typography-text>
          </template>

          <template v-else-if="column.key === 'amount'">
            <a-typography-text strong style="color: #1890ff">
              {{ formatCurrency(record.amount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="sColor(record.status)">{{ record.status }}</a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button
                v-if="record.status !== 'Đã TT'"
                type="primary"
                size="small"
                @click="payBill(record)"
              >
                Thu tiền
              </a-button>
              <a-tooltip title="In hóa đơn">
                <a-button type="text" size="small">
                  <template #icon><PrinterOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
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
  ArrowUpOutlined,
  PrinterOutlined,
  EyeOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import billService from '@/services/billService'

const loading = ref(false)
const bills = ref([])
const search = ref('')
const monthFilter = ref('all')
const statusFilter = ref('all')

const months = ['Tháng 5/2026', 'Tháng 4/2026', 'Tháng 3/2026', 'Tháng 2/2026']

const statusFilterOptions = [
  { label: 'Tất cả', value: 'all' },
  { label: 'Đã TT', value: 'Đã TT' },
  { label: 'Chưa TT', value: 'Chưa TT' },
  { label: 'Quá hạn', value: 'Quá hạn' }
]

const billingColumns = [
  { title: 'Mã HĐ', key: 'code', dataIndex: 'code', width: 120 },
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Phòng', dataIndex: 'room', key: 'room', width: 80, align: 'center' },
  { title: 'Mô tả', dataIndex: 'description', key: 'description' },
  { title: 'Số tiền', key: 'amount', dataIndex: 'amount', width: 140, align: 'right' },
  { title: 'Hạn TT', dataIndex: 'due', key: 'due', width: 120, align: 'center' },
  { title: 'Trạng thái', key: 'status', dataIndex: 'status', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 180, align: 'center', fixed: 'right' }
]

const studentMap = {
  '30000000-0000-0000-0000-000000000001': 'Nguyễn Văn A',
  '30000000-0000-0000-0000-000000000002': 'Trần Thị B',
  '30000000-0000-0000-0000-000000000003': 'Lê Văn C',
  '30000000-0000-0000-0000-000000000004': 'Phạm Thị D',
  '30000000-0000-0000-0000-000000000005': 'Hoàng Anh E'
}

const roomMap = {
  '40000000-0000-0000-0000-000000000101': 'A101',
  '40000000-0000-0000-0000-000000000102': 'A102',
  '40000000-0000-0000-0000-000000000103': 'A103',
  '40000000-0000-0000-0000-000000000104': 'A104',
  '40000000-0000-0000-0000-000000000105': 'A105'
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

const sColor = (s) =>
  ({ 'Đã TT': 'success', 'Chưa TT': 'warning', 'Quá hạn': 'error' })[s] || 'default'

async function loadData() {
  loading.value = true
  try {
    const loaded = await billService.getAll()
    bills.value = loaded.map((b, index) => {
      const roomId = b.roomId || b.RoomId || ''
      const studentId = b.studentId || b.StudentId || ''
      const dueDate = new Date(b.dueDate)
      return {
        id: b.id,
        code: `HD${String(index + 1).padStart(4, '0')}`,
        student: studentMap[studentId] || 'Không xác định',
        room: roomMap[roomId] || 'N/A',
        description: b.description || 'Hóa đơn tiền phòng',
        amount: b.amount || 0,
        due: dueDate.toLocaleDateString('vi-VN'),
        dueDate: dueDate.toISOString(),
        monthLabel: `Tháng ${dueDate.getMonth() + 1}/${dueDate.getFullYear()}`,
        status:
          b.status === 'Paid'
            ? 'Đã TT'
            : b.status === 'Overdue'
              ? 'Quá hạn'
              : 'Chưa TT'
      }
    })
  } catch (err) {
    console.error('Lỗi tải hóa đơn:', err)
    message.error(err.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const totalRevenue = computed(() =>
  bills.value
    .filter((b) => b.status === 'Đã TT')
    .reduce((sum, b) => sum + b.amount, 0)
)

const unpaidAmount = computed(() =>
  bills.value
    .filter((b) => b.status === 'Chưa TT')
    .reduce((sum, b) => sum + b.amount, 0)
)

const overdueAmount = computed(() =>
  bills.value
    .filter((b) => b.status === 'Quá hạn')
    .reduce((sum, b) => sum + b.amount, 0)
)

const unpaidCount = computed(() => bills.value.filter((b) => b.status === 'Chưa TT').length)
const overdueCount = computed(() => bills.value.filter((b) => b.status === 'Quá hạn').length)

const filteredBills = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return bills.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.code.toLowerCase().includes(keyword) ||
      item.student.toLowerCase().includes(keyword) ||
      item.room.toLowerCase().includes(keyword)
    const matchesStatus = statusFilter.value === 'all' || item.status === statusFilter.value
    const matchesMonth = monthFilter.value === 'all' || item.monthLabel === monthFilter.value
    return matchesText && matchesStatus && matchesMonth
  })
})

function payBill(record) {
  message.info(`Thu tiền cho hóa đơn: ${record.code}`)
}

onMounted(loadData)
</script>

<style scoped>
:deep(.ant-statistic-title) {
  color: inherit;
  opacity: 0.85;
  font-size: 14px;
  margin-bottom: 4px;
}

:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
