<template>
  <div>
    <!-- Page Header -->
    <div style="background: #fff; margin-bottom: 16px; border-radius: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); padding: 16px 24px">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 4px 0; color: #000">
            Quản Lý Phiếu Thu
          </h1>
          <p style="font-size: 14px; color: #8c8c8c; margin: 0">
            Quản lý phiếu thu tiền hàng tháng
          </p>
        </div>
        <a-button type="primary" @click="showCreateDialog" size="large">
          <template #icon><PlusOutlined /></template>
          Tạo Phiếu Thu
        </a-button>
      </div>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm kiếm mã phiếu, tên SV..."
            allow-clear
            @search="handleSearch"
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
            @change="handleSearch"
          >
            <a-select-option v-for="item in statusOptions" :key="item.value" :value="item.value">
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="monthFilter"
            placeholder="Tháng"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="m in 12" :key="m" :value="m">
              Tháng {{ m }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="yearFilter"
            placeholder="Năm"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="y in yearOptions" :key="y" :value="y">
              Năm {{ y }}
            </a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="invoices"
        :loading="loading"
        :pagination="pagination"
        :scroll="{ x: 1200 }"
        @change="handleTableChange"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'invoiceCode'">
            <a-typography-text strong copyable>
              {{ record.invoiceCode }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'studentInfo'">
            <div>
              <div style="font-weight: 500">{{ record.studentName }}</div>
              <a-typography-text type="secondary" style="font-size: 12px">
                {{ record.studentCode }}
              </a-typography-text>
            </div>
          </template>

          <template v-else-if="column.key === 'roomInfo'">
            <div>
              <div>{{ record.roomNumber }}</div>
              <a-typography-text type="secondary" style="font-size: 12px">
                {{ record.buildingName }}
              </a-typography-text>
            </div>
          </template>

          <template v-else-if="column.key === 'period'">
            <a-tag color="blue">
              {{ record.billingMonth }}/{{ record.billingYear }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'totalAmount'">
            <a-typography-text strong style="color: #1890ff">
              {{ formatCurrency(record.totalAmount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'paidAmount'">
            <a-typography-text style="color: #52c41a">
              {{ formatCurrency(record.paidAmount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'debtAmount'">
            <a-typography-text :type="record.debtAmount > 0 ? 'danger' : 'success'" strong>
              {{ formatCurrency(record.debtAmount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small" @click="viewInvoice(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Sửa">
                <a-button type="text" size="small" @click="editInvoice(record)">
                  <template #icon><EditOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xóa">
                <a-button type="text" danger size="small" @click="deleteInvoice(record)">
                  <template #icon><DeleteOutlined /></template>
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
import { ref, reactive, onMounted } from 'vue'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import invoiceService from '@/services/invoiceService'

const loading = ref(false)
const invoices = ref([])
const search = ref('')
const statusFilter = ref(undefined)
const monthFilter = ref(undefined)
const yearFilter = ref(undefined)

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `Tổng ${total} phiếu thu`
})

const columns = [
  {
    title: 'Mã Phiếu Thu',
    dataIndex: 'invoiceCode',
    key: 'invoiceCode',
    width: 140,
    fixed: 'left'
  },
  {
    title: 'Sinh Viên',
    key: 'studentInfo',
    width: 180
  },
  {
    title: 'Phòng',
    key: 'roomInfo',
    width: 150
  },
  {
    title: 'Kỳ',
    key: 'period',
    width: 100,
    align: 'center'
  },
  {
    title: 'Tổng Tiền',
    key: 'totalAmount',
    width: 140,
    align: 'right'
  },
  {
    title: 'Đã Trả',
    key: 'paidAmount',
    width: 140,
    align: 'right'
  },
  {
    title: 'Còn Nợ',
    key: 'debtAmount',
    width: 140,
    align: 'right'
  },
  {
    title: 'Trạng thái',
    key: 'status',
    width: 130,
    align: 'center'
  },
  {
    title: 'Thao tác',
    key: 'actions',
    width: 150,
    align: 'center',
    fixed: 'right'
  }
]

const statusOptions = [
  { label: 'Chưa thanh toán', value: 'Unpaid' },
  { label: 'Thanh toán một phần', value: 'PartialPaid' },
  { label: 'Đã thanh toán', value: 'Paid' },
  { label: 'Quá hạn', value: 'Overdue' },
  { label: 'Đã hủy', value: 'Cancelled' }
]

const currentYear = new Date().getFullYear()
const yearOptions = Array.from({ length: 5 }, (_, i) => currentYear - 2 + i)

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

const getStatusColor = (status) => {
  const colors = {
    'Unpaid': 'orange',
    'PartialPaid': 'cyan',
    'Paid': 'green',
    'Overdue': 'red',
    'Cancelled': 'default'
  }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    'Unpaid': 'Chưa thanh toán',
    'PartialPaid': 'Thanh toán 1 phần',
    'Paid': 'Đã thanh toán',
    'Overdue': 'Quá hạn',
    'Cancelled': 'Đã hủy'
  }
  return texts[status] || status
}

const fetchInvoices = async () => {
  loading.value = true
  try {
    const data = await invoiceService.getAll()
    invoices.value = data
    pagination.total = data.length
  } catch (error) {
    message.error(error.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  fetchInvoices()
}

const handleTableChange = (pag) => {
  pagination.current = pag.current
  pagination.pageSize = pag.pageSize
}

const showCreateDialog = () => {
  message.info('Chức năng đang phát triển')
}

const viewInvoice = (record) => {
  message.info(`Xem chi tiết: ${record.invoiceCode}`)
}

const editInvoice = (record) => {
  message.info(`Sửa: ${record.invoiceCode}`)
}

const deleteInvoice = (record) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: `Bạn có chắc muốn xóa phiếu thu ${record.invoiceCode}?`,
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await invoiceService.delete(record.id)
        message.success('Xóa thành công')
        fetchInvoices()
      } catch (error) {
        message.error('Xóa thất bại')
      }
    }
  })
}

onMounted(() => {
  fetchInvoices()
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
