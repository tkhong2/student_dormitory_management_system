<template>
  <div>
    <PageHeaderAnt title="Công nợ" subtitle="Quản lý nợ các sinh viên">
    </PageHeaderAnt>

    <!-- Filters -->
    <a-card :bordered="false" style="border-radius: 12px; margin-bottom: 16px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-row :gutter="16">
        <a-col :xs="24" :sm="12" :lg="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm sinh viên..."
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-select
            v-model:value="debtFilter"
            placeholder="Mức nợ"
            :options="debtOptions"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="12">
          <a-button block @click="loadData">
            <template #icon><SearchOutlined /></template>
            Tìm kiếm
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Stats -->
    <a-row :gutter="16" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Sinh viên nợ" 
          :value="debts.length"
          :value-style="{ color: '#ff4d4f' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Tổng công nợ" 
          value="42,500,000₫"
          :value-style="{ color: '#ff6b00' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Quá hạn" 
          :value="debts.filter(x => x.overdue).length"
          :value-style="{ color: '#ff4d4f', fontWeight: 'bold' }"
        />
      </a-col>
    </a-row>

    <!-- Table -->
    <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-table
        :columns="columns"
        :data-source="filteredDebts"
        :loading="loading"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} sinh viên` }"
        size="small"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'debtStatus'">
            <a-tag :color="record.overdue ? 'red' : 'orange'">
              {{ record.overdue ? 'Quá hạn' : 'Nợ' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Ghi nhận thanh toán">
                <a-button type="primary" size="small" @click="recordPayment(record)">
                  <template #icon><DollarOutlined /></template>
                  Thu
                </a-button>
              </a-tooltip>
              <a-tooltip title="Chi tiết">
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
import { message } from 'ant-design-vue'
import {
  SearchOutlined,
  DollarOutlined,
  EyeOutlined,
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'

const loading = ref(false)
const search = ref('')
const debtFilter = ref(null)
const debts = ref([])

const debtOptions = [
  { label: 'Dưới 1 triệu', value: 'low' },
  { label: '1-5 triệu', value: 'medium' },
  { label: 'Trên 5 triệu', value: 'high' },
]

const columns = [
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 180 },
  { title: 'Phòng', dataIndex: 'room', key: 'room', width: 100, align: 'center' },
  { title: 'Tổng nợ', dataIndex: 'totalDebt', key: 'totalDebt', width: 140, align: 'right' },
  { title: 'Số tháng nợ', dataIndex: 'months', key: 'months', width: 120, align: 'center' },
  { title: 'Từ ngày', dataIndex: 'fromDate', key: 'fromDate', width: 120, align: 'center' },
  { title: 'Trạng thái', dataIndex: 'debtStatus', key: 'debtStatus', width: 120, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 180, align: 'center', fixed: 'right' },
]

const filteredDebts = computed(() => {
  return debts.value.filter(item => {
    const matchesSearch = !search.value || 
      item.student.toLowerCase().includes(search.value.toLowerCase()) ||
      item.room.includes(search.value)
    const matchesFilter = !debtFilter.value || item.debtLevel === debtFilter.value
    return matchesSearch && matchesFilter
  })
})

async function loadData() {
  loading.value = true
  try {
    debts.value = [
      { key: 1, student: 'Trần Văn A', room: '101', totalDebt: '1,500,000₫', months: 3, fromDate: '03/2026', debtLevel: 'medium', overdue: false },
      { key: 2, student: 'Lê Thị B', room: '102', totalDebt: '2,000,000₫', months: 4, fromDate: '02/2026', debtLevel: 'high', overdue: true },
      { key: 3, student: 'Phạm Văn C', room: '103', totalDebt: '500,000₫', months: 1, fromDate: '05/2026', debtLevel: 'low', overdue: false },
      { key: 4, student: 'Nguyễn Thị D', room: '201', totalDebt: '6,000,000₫', months: 6, fromDate: '12/2025', debtLevel: 'high', overdue: true },
    ]
  } catch (error) {
    message.error('Lỗi khi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

function recordPayment(record) {
  message.info(`Ghi nhận thanh toán cho: ${record.student}`)
}

onMounted(loadData)
</script>
