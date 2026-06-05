<template>
  <div>
    <PageHeaderAnt title="Hóa đơn" subtitle="Quản lý hóa đơn sinh viên">
      <template #actions>
        <a-button type="primary">
          <template #icon><PlusOutlined /></template>
          Tạo hóa đơn
        </a-button>
      </template>
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
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            :options="statusOptions"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-select
            v-model:value="monthFilter"
            placeholder="Tháng"
            :options="monthOptions"
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
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Tổng hóa đơn" 
          :value="invoices.length"
          :value-style="{ color: '#1890ff' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Chưa thanh toán" 
          :value="invoices.filter(x => x.status === 'Unpaid').length"
          :value-style="{ color: '#ff6b00' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Quá hạn" 
          :value="invoices.filter(x => x.status === 'Overdue').length"
          :value-style="{ color: '#ff4d4f' }"
        />
      </a-col>
    </a-row>

    <!-- Table -->
    <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-table
        :columns="columns"
        :data-source="filteredInvoices"
        :loading="loading"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} hóa đơn` }"
        size="small"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <a-tag 
              :color="
                record.status === 'Paid' ? 'green' :
                record.status === 'Unpaid' ? 'orange' :
                record.status === 'Overdue' ? 'red' : 'blue'
              "
            >
              {{ record.status }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small" @click="viewDetail(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="In">
                <a-button type="text" size="small">
                  <template #icon><PrinterOutlined /></template>
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
  EyeOutlined,
  PrinterOutlined,
  PlusOutlined,
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import billService from '@/services/billService'

const loading = ref(false)
const search = ref('')
const statusFilter = ref(null)
const monthFilter = ref(null)
const invoices = ref([])

const statusOptions = [
  { label: 'Chưa thanh toán', value: 'Unpaid' },
  { label: 'Thanh toán một phần', value: 'PartialPaid' },
  { label: 'Đã thanh toán', value: 'Paid' },
  { label: 'Quá hạn', value: 'Overdue' },
]

const monthOptions = [
  { label: 'Tháng 6/2026', value: '6/2026' },
  { label: 'Tháng 5/2026', value: '5/2026' },
  { label: 'Tháng 4/2026', value: '4/2026' },
]

const columns = [
  { title: 'Mã HĐ', dataIndex: 'id', key: 'id', width: 100 },
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Phòng', dataIndex: 'room', key: 'room', width: 80, align: 'center' },
  { title: 'Số tiền', dataIndex: 'amount', key: 'amount', width: 120, align: 'right' },
  { title: 'Còn nợ', dataIndex: 'debt', key: 'debt', width: 120, align: 'right' },
  { title: 'Trạng thái', dataIndex: 'status', key: 'status', width: 130, align: 'center' },
  { title: 'Ngày tạo', dataIndex: 'createdDate', key: 'createdDate', width: 120, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 120, align: 'center', fixed: 'right' },
]

const filteredInvoices = computed(() => {
  return invoices.value.filter(item => {
    const matchesSearch = !search.value || 
      item.student.toLowerCase().includes(search.value.toLowerCase()) ||
      item.id.toLowerCase().includes(search.value.toLowerCase())
    const matchesStatus = !statusFilter.value || item.status === statusFilter.value
    const matchesMonth = !monthFilter.value || item.month === monthFilter.value
    return matchesSearch && matchesStatus && matchesMonth
  })
})

async function loadData() {
  loading.value = true
  try {
    // TODO: thay bằng API thực tế
    invoices.value = [
      { key: 1, id: 'INV-001', student: 'Trần Văn A', room: '101', amount: '500,000₫', debt: '0₫', status: 'Paid', createdDate: '01/06/2026', month: '6/2026' },
      { key: 2, id: 'INV-002', student: 'Lê Thị B', room: '102', amount: '500,000₫', debt: '500,000₫', status: 'Unpaid', createdDate: '01/06/2026', month: '6/2026' },
      { key: 3, id: 'INV-003', student: 'Phạm Văn C', room: '103', amount: '500,000₫', debt: '500,000₫', status: 'Overdue', createdDate: '01/05/2026', month: '5/2026' },
    ]
    message.success('Tải dữ liệu thành công')
  } catch (error) {
    message.error('Lỗi khi tải dữ liệu')
    console.error(error)
  } finally {
    loading.value = false
  }
}

function viewDetail(record) {
  message.info(`Xem chi tiết hóa đơn: ${record.id}`)
}

onMounted(loadData)
</script>
