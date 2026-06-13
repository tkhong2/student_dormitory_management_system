<template>
  <div>
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Công nợ sinh viên</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Theo dõi các khoản nợ chưa thanh toán</p>
    </div>

    <!-- Summary Cards -->
    <a-row :gutter="16" style="margin-bottom: 16px">
      <a-col :xs="24" :md="8">
        <a-card :bordered="false">
          <a-statistic
            title="Tổng công nợ"
            :value="totalDebt"
            :precision="0"
            suffix="₫"
            :value-style="{ color: '#ff4d4f', fontSize: '24px', fontWeight: 'bold' }"
          >
            <template #formatter="{ value }">
              {{ formatCurrency(value) }}
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :md="8">
        <a-card :bordered="false">
          <a-statistic
            title="Số sinh viên nợ"
            :value="debtorCount"
            :value-style="{ color: '#faad14', fontSize: '24px', fontWeight: 'bold' }"
          />
        </a-card>
      </a-col>
      <a-col :xs="24" :md="8">
        <a-card :bordered="false">
          <a-statistic
            title="Quá hạn"
            :value="overdueDebt"
            :precision="0"
            suffix="₫"
            :value-style="{ color: '#f5222d', fontSize: '24px', fontWeight: 'bold' }"
          >
            <template #formatter="{ value }">
              {{ formatCurrency(value) }}
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm sinh viên, mã SV, phòng..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="Unpaid">Chưa thanh toán</a-select-option>
            <a-select-option value="Overdue">Quá hạn</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="sortBy"
            placeholder="Sắp xếp theo"
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option value="amount_desc">Nợ nhiều nhất</a-select-option>
            <a-select-option value="amount_asc">Nợ ít nhất</a-select-option>
            <a-select-option value="overdue_desc">Quá hạn lâu nhất</a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table 
        :columns="columns" 
        :data-source="filteredDebts" 
        :row-key="item => item.studentId"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} sinh viên` }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
              <a-avatar size="36" style="background: #fff1f0; color: #ff4d4f">
                <UserOutlined />
              </a-avatar>
              <div>
                <div style="font-weight: 600">{{ record.studentName }}</div>
                <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'room'">
            <div>
              <div style="font-weight: 500">{{ record.roomNumber || '---' }}</div>
              <div style="font-size: 12px; color: #8c8c8c">{{ record.buildingName || '' }}</div>
            </div>
          </template>
          <template v-else-if="column.key === 'total'">
            <span style="font-weight: 700; color: #ff4d4f; font-size: 15px">{{ formatCurrency(record.totalDebt) }}</span>
          </template>
          <template v-else-if="column.key === 'count'">
            <a-tag color="error">{{ record.invoiceCount }} phiếu</a-tag>
          </template>
          <template v-else-if="column.key === 'overdue'">
            <a-tag v-if="record.maxOverdueDays > 0" color="red">
              {{ record.maxOverdueDays }} ngày
            </a-tag>
            <span v-else style="color: #8c8c8c">---</span>
          </template>
          <template v-else-if="column.key === 'deadline'">
            {{ formatDate(record.earliestDueDate) }}
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Gửi nhắc nợ">
                <a-button type="default" size="small" @click="sendReminder(record)">
                  <template #icon><BellOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Thu tiền">
                <a-button type="primary" size="small" style="background: #52c41a; border-color: #52c41a" @click="collectDebt(record)">
                  <template #icon><DollarOutlined /></template>
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
import { UserOutlined, BellOutlined, DollarOutlined, SearchOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import billService from '@/services/billService'

const router = useRouter()
const loading = ref(false)
const invoices = ref([])
const search = ref('')
const statusFilter = ref('')
const sortBy = ref('amount_desc')

const columns = [
  { title: 'Sinh viên', key: 'student', width: 250 },
  { title: 'Phòng', key: 'room', width: 150 },
  { title: 'Tổng nợ', key: 'total', align: 'right', width: 150 },
  { title: 'Số phiếu nợ', key: 'count', align: 'center', width: 120 },
  { title: 'Quá hạn', key: 'overdue', align: 'center', width: 120 },
  { title: 'Hạn cuối', key: 'deadline', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 150 },
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

async function loadData() {
  loading.value = true
  try {
    const allInvoices = await billService.getAll()
    // Filter only unpaid and overdue invoices
    invoices.value = allInvoices.filter(inv => 
      inv.status === 'Unpaid' || inv.status === 'Overdue' || inv.status === 'PartialPaid'
    )
  } catch (err) {
    console.error('Lỗi tải công nợ:', err)
    message.error(err.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

// Group invoices by student and calculate debt
const debts = computed(() => {
  const debtMap = new Map()
  
  invoices.value.forEach(inv => {
    const key = inv.studentId
    if (!debtMap.has(key)) {
      debtMap.set(key, {
        studentId: inv.studentId,
        studentName: inv.studentName,
        studentCode: inv.studentCode,
        roomNumber: inv.roomNumber,
        buildingName: inv.buildingName,
        totalDebt: 0,
        invoiceCount: 0,
        maxOverdueDays: 0,
        earliestDueDate: inv.dueDate,
        invoices: []
      })
    }
    
    const debt = debtMap.get(key)
    // For DepositRefund invoices, debtAmount is negative (money to refund)
    // So we add it (which effectively subtracts the absolute value)
    debt.totalDebt += (inv.debtAmount || 0)
    debt.invoiceCount++
    debt.maxOverdueDays = Math.max(debt.maxOverdueDays, inv.overdueDays || 0)
    debt.earliestDueDate = new Date(debt.earliestDueDate) < new Date(inv.dueDate) ? debt.earliestDueDate : inv.dueDate
    debt.invoices.push(inv)
  })
  
  // Filter out students with zero or negative total debt (they don't owe money)
  return Array.from(debtMap.values()).filter(d => d.totalDebt > 0)
})

const totalDebt = computed(() => 
  debts.value.reduce((sum, d) => sum + d.totalDebt, 0)
)

const debtorCount = computed(() => debts.value.length)

const overdueDebt = computed(() => 
  debts.value
    .filter(d => d.maxOverdueDays > 0)
    .reduce((sum, d) => sum + d.totalDebt, 0)
)

const filteredDebts = computed(() => {
  let result = debts.value
  
  // Search filter
  if (search.value) {
    const keyword = search.value.toLowerCase()
    result = result.filter(d => 
      d.studentName.toLowerCase().includes(keyword) ||
      d.studentCode.toLowerCase().includes(keyword) ||
      (d.roomNumber && d.roomNumber.toLowerCase().includes(keyword))
    )
  }
  
  // Status filter
  if (statusFilter.value === 'Overdue') {
    result = result.filter(d => d.maxOverdueDays > 0)
  } else if (statusFilter.value === 'Unpaid') {
    result = result.filter(d => d.maxOverdueDays === 0)
  }
  
  // Sort
  if (sortBy.value === 'amount_desc') {
    result.sort((a, b) => b.totalDebt - a.totalDebt)
  } else if (sortBy.value === 'amount_asc') {
    result.sort((a, b) => a.totalDebt - b.totalDebt)
  } else if (sortBy.value === 'overdue_desc') {
    result.sort((a, b) => b.maxOverdueDays - a.maxOverdueDays)
  }
  
  return result
})

function handleSearch() {
  // Trigger computed property re-evaluation
}

async function sendReminder(debt) {
  try {
    // Send reminder for all unpaid invoices of this student
    for (const invoice of debt.invoices) {
      await billService.sendReminder(invoice.id)
    }
    message.success(`Đã gửi email nhắc nợ đến ${debt.studentName}`)
    loadData()
  } catch (err) {
    message.error('Gửi nhắc nợ thất bại')
  }
}

function collectDebt(debt) {
  // Navigate to payment page with student filter
  router.push({
    name: 'payments',
    query: { 
      studentId: debt.studentId,
      studentName: debt.studentName 
    }
  })
}

onMounted(loadData)
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
