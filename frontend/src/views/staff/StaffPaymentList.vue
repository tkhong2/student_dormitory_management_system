<template>
  <div>
    <PageHeaderAnt title="Thanh toán" subtitle="Ghi nhận thanh toán hóa đơn sinh viên">
      <template #actions>
        <a-button type="primary">
          <template #icon><PlusOutlined /></template>
          Ghi nhận thanh toán
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
            v-model:value="methodFilter"
            placeholder="Phương thức"
            :options="methodOptions"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :lg="6">
          <a-range-picker
            v-model:value="dateRange"
            style="width: 100%"
            placeholder="Ngày thanh toán"
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
          title="Tổng thanh toán" 
          :value="payments.length"
          :value-style="{ color: '#1890ff' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Tổng số tiền" 
          value="18,500,000₫"
          :value-style="{ color: '#52c41a' }"
        />
      </a-col>
      <a-col :xs="24" :sm="12" :lg="8">
        <a-statistic 
          title="Hôm nay" 
          :value="payments.filter(x => x.date === '05/06/2026').length"
          :value-style="{ color: '#faad14' }"
        />
      </a-col>
    </a-row>

    <!-- Table -->
    <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
      <a-table
        :columns="columns"
        :data-source="filteredPayments"
        :loading="loading"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} thanh toán` }"
        size="small"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <a-tag :color="record.status === 'Confirmed' ? 'green' : 'blue'">
              {{ record.status }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Chi tiết">
                <a-button type="text" size="small">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip v-if="record.status === 'Pending'" title="Xác nhận">
                <a-button type="text" size="small" @click="confirmPayment(record)">
                  <template #icon><CheckOutlined /></template>
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
  PlusOutlined,
  EyeOutlined,
  CheckOutlined,
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'

const loading = ref(false)
const search = ref('')
const methodFilter = ref(null)
const dateRange = ref([])
const payments = ref([])

const methodOptions = [
  { label: 'Chuyển khoản', value: 'transfer' },
  { label: 'Tiền mặt', value: 'cash' },
  { label: 'Thẻ', value: 'card' },
]

const columns = [
  { title: 'Mã thanh toán', dataIndex: 'id', key: 'id', width: 120 },
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Số tiền', dataIndex: 'amount', key: 'amount', width: 130, align: 'right' },
  { title: 'Phương thức', dataIndex: 'method', key: 'method', width: 130, align: 'center' },
  { title: 'Ngày', dataIndex: 'date', key: 'date', width: 120, align: 'center' },
  { title: 'Trạng thái', dataIndex: 'status', key: 'status', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 120, align: 'center', fixed: 'right' },
]

const filteredPayments = computed(() => {
  return payments.value.filter(item => {
    const matchesSearch = !search.value || 
      item.student.toLowerCase().includes(search.value.toLowerCase()) ||
      item.id.toLowerCase().includes(search.value.toLowerCase())
    const matchesMethod = !methodFilter.value || item.method === methodFilter.value
    return matchesSearch && matchesMethod
  })
})

async function loadData() {
  loading.value = true
  try {
    payments.value = [
      { key: 1, id: 'PAY-001', student: 'Trần Văn A', amount: '500,000₫', method: 'Chuyển khoản', date: '05/06/2026', status: 'Confirmed' },
      { key: 2, id: 'PAY-002', student: 'Lê Thị B', amount: '1,000,000₫', method: 'Tiền mặt', date: '04/06/2026', status: 'Confirmed' },
      { key: 3, id: 'PAY-003', student: 'Phạm Văn C', amount: '500,000₫', method: 'Chuyển khoản', date: '03/06/2026', status: 'Pending' },
    ]
  } catch (error) {
    message.error('Lỗi khi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

function confirmPayment(record) {
  message.success(`Đã xác nhận thanh toán: ${record.id}`)
}

onMounted(loadData)
</script>
