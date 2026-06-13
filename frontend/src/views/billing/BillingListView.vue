<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
        Hóa Đơn & Thanh Toán
      </h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
        Tổng số: {{ bills.length }} hóa đơn
      </p>
    </div>

    <!-- Revenue Summary -->
    <a-row :gutter="16" style="margin-bottom: 16px">
      <a-col :xs="24" :md="8">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #1890ff 0%, #096dd9 100%); color: white">
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
            <a-typography-text strong copyable>{{ record.invoiceCode }}</a-typography-text>
          </template>

          <template v-else-if="column.key === 'period'">
            <a-tag color="blue">{{ record.billingMonth }}/{{ record.billingYear }}</a-tag>
          </template>

          <template v-else-if="column.key === 'amount'">
            <a-typography-text strong style="color: #1890ff">
              {{ formatCurrency(record.totalAmount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'due'">
            {{ formatDate(record.dueDate) }}
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="sColor(record.status)">{{ sLabel(record.status) }}</a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button
                v-if="record.status !== 'Paid'"
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
import { useRouter } from 'vue-router'
import {
  SearchOutlined,
  ArrowUpOutlined,
  PrinterOutlined,
  EyeOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import billService from '@/services/billService'

const router = useRouter()
const loading = ref(false)
const bills = ref([])
const search = ref('')
const monthFilter = ref('all')
const statusFilter = ref('all')

const months = ref([])

const statusFilterOptions = [
  { label: 'Tất cả', value: 'all' },
  { label: 'Đã TT', value: 'Paid' },
  { label: 'Chưa TT', value: 'Unpaid' },
  { label: 'Quá hạn', value: 'Overdue' }
]

const billingColumns = [
  { title: 'Mã Phiếu Thu', key: 'code', dataIndex: 'invoiceCode', width: 140 },
  { title: 'Sinh viên', dataIndex: 'studentName', key: 'studentName', width: 150 },
  { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 80, align: 'center' },
  { title: 'Kỳ thanh toán', key: 'period', width: 120, align: 'center' },
  { title: 'Tổng tiền', key: 'amount', dataIndex: 'totalAmount', width: 140, align: 'right' },
  { title: 'Hạn TT', key: 'due', width: 120, align: 'center' },
  { title: 'Trạng thái', key: 'status', dataIndex: 'status', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 180, align: 'center', fixed: 'right' }
]

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

const formatDate = (dateString) => {
  if (!dateString) return '---'
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const sColor = (s) => {
  const colors = {
    'Paid': 'success',
    'Unpaid': 'warning',
    'PartialPaid': 'processing',
    'Overdue': 'error',
    'Cancelled': 'default'
  }
  return colors[s] || 'default'
}

const sLabel = (s) => {
  const labels = {
    'Paid': 'Đã TT',
    'Unpaid': 'Chưa TT',
    'PartialPaid': 'TT 1 phần',
    'Overdue': 'Quá hạn',
    'Cancelled': 'Đã hủy'
  }
  return labels[s] || s
}

async function loadData() {
  loading.value = true
  try {
    const loaded = await billService.getAll()
    bills.value = loaded
    
    // Extract unique months from bills
    const monthsSet = new Set()
    loaded.forEach(bill => {
      const monthLabel = `Tháng ${bill.billingMonth}/${bill.billingYear}`
      monthsSet.add(monthLabel)
    })
    months.value = Array.from(monthsSet).sort().reverse()
    
  } catch (err) {
    console.error('Lỗi tải hóa đơn:', err)
    message.error(err.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

// Calculate statistics from ACTUAL invoice data
const totalRevenue = computed(() =>
  bills.value
    .filter((b) => b.status === 'Paid')
    .reduce((sum, b) => sum + (b.totalAmount || 0), 0)
)

const unpaidAmount = computed(() =>
  bills.value
    .filter((b) => b.status === 'Unpaid' || b.status === 'PartialPaid')
    .reduce((sum, b) => sum + (b.debtAmount || 0), 0)
)

const overdueAmount = computed(() =>
  bills.value
    .filter((b) => b.status === 'Overdue')
    .reduce((sum, b) => sum + (b.debtAmount || 0), 0)
)

const unpaidCount = computed(() => 
  bills.value.filter((b) => b.status === 'Unpaid' || b.status === 'PartialPaid').length
)

const overdueCount = computed(() => 
  bills.value.filter((b) => b.status === 'Overdue').length
)

const filteredBills = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return bills.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.invoiceCode?.toLowerCase().includes(keyword) ||
      item.studentName?.toLowerCase().includes(keyword) ||
      item.studentCode?.toLowerCase().includes(keyword) ||
      item.roomNumber?.toLowerCase().includes(keyword)
    
    const matchesStatus = statusFilter.value === 'all' || item.status === statusFilter.value
    
    const itemMonthLabel = `Tháng ${item.billingMonth}/${item.billingYear}`
    const matchesMonth = monthFilter.value === 'all' || itemMonthLabel === monthFilter.value
    
    return matchesText && matchesStatus && matchesMonth
  })
})

function payBill(record) {
  // Navigate to payment page with selected invoice
  router.push({
    name: 'payments',
    query: { invoiceId: record.id }
  })
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
