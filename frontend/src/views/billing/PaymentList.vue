<template>
  <div>
    <!-- Page Header -->
    <div style="background: #fff; margin-bottom: 16px; border-radius: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); padding: 16px 24px">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 4px 0; color: #000">
            Quản Lý Thanh Toán
          </h1>
          <p style="font-size: 14px; color: #8c8c8c; margin: 0">
            Quản lý lịch sử thanh toán và ghi nhận thanh toán mới
          </p>
        </div>
        <a-button type="primary" @click="showCreateDialog" size="large">
          <template #icon><PlusOutlined /></template>
          Ghi Nhận Thanh Toán
        </a-button>
      </div>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm kiếm mã GD, mã phiếu thu..."
            allow-clear
            @search="handleSearch"
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="methodFilter"
            placeholder="Phương thức thanh toán"
            allow-clear
            style="width: 100%"
            @change="handleSearch"
          >
            <a-select-option v-for="item in methodOptions" :key="item.value" :value="item.value">
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-range-picker
            v-model:value="dateRange"
            style="width: 100%"
            format="DD/MM/YYYY"
            placeholder="['Từ ngày', 'Đến ngày']"
            @change="handleSearch"
          />
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="payments"
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

          <template v-else-if="column.key === 'amount'">
            <a-typography-text strong style="color: #52c41a">
              {{ formatCurrency(record.amount) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'method'">
            <a-tag :color="getMethodColor(record.method)">
              {{ getMethodText(record.method) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'paymentDate'">
            {{ formatDate(record.paymentDate) }}
          </template>

          <template v-else-if="column.key === 'transactionCode'">
            <a-typography-text v-if="record.transactionCode" code>
              {{ record.transactionCode }}
            </a-typography-text>
            <a-typography-text v-else type="secondary">
              ---
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'receivedByName'">
            {{ record.receivedByName || '---' }}
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small" @click="viewPayment(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Sửa">
                <a-button type="text" size="small" @click="editPayment(record)">
                  <template #icon><EditOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xóa">
                <a-button type="text" danger size="small" @click="deletePayment(record)">
                  <template #icon><DeleteOutlined /></template>
                </a-button>
              </a-tooltip>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editedIndex === -1 ? 'Ghi Nhận Thanh Toán' : 'Sửa Thanh Toán'"
      width="700px"
      @cancel="closeDialog"
    >
      <a-form :model="editedItem" layout="vertical">
        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="Mã Phiếu Thu" required>
              <a-input v-model:value="editedItem.invoiceCode" placeholder="Nhập mã phiếu thu" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số Tiền" required>
              <a-input-number
                v-model:value="editedItem.amount"
                :min="0"
                :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                style="width: 100%"
                placeholder="Nhập số tiền"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Phương Thức" required>
              <a-select v-model:value="editedItem.method" placeholder="Chọn phương thức">
                <a-select-option v-for="item in methodOptions" :key="item.value" :value="item.value">
                  {{ item.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Ngày Thanh Toán" required>
              <a-date-picker
                v-model:value="editedItem.paymentDate"
                format="DD/MM/YYYY"
                style="width: 100%"
                placeholder="Chọn ngày"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Mã Giao Dịch">
              <a-input v-model:value="editedItem.transactionCode" placeholder="Nhập mã giao dịch" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Ngân Hàng">
              <a-input v-model:value="editedItem.bankName" placeholder="Nhập tên ngân hàng" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số Tài Khoản">
              <a-input v-model:value="editedItem.bankAccountNumber" placeholder="Nhập số tài khoản" />
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item label="Ghi Chú">
              <a-textarea
                v-model:value="editedItem.note"
                :rows="3"
                placeholder="Nhập ghi chú (nếu có)"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
      <template #footer>
        <a-button @click="closeDialog">Hủy</a-button>
        <a-button type="primary" @click="savePayment">Lưu</a-button>
      </template>
    </a-modal>
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
import paymentService from '@/services/paymentService'

const loading = ref(false)
const payments = ref([])
const search = ref('')
const methodFilter = ref(undefined)
const dateRange = ref(null)
const dialog = ref(false)
const editedIndex = ref(-1)
const editedItem = ref({
  invoiceCode: '',
  amount: 0,
  method: 'Cash',
  paymentDate: null,
  transactionCode: '',
  bankName: '',
  bankAccountNumber: '',
  note: ''
})

const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `Tổng ${total} thanh toán`
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
    title: 'Số Tiền',
    key: 'amount',
    width: 150,
    align: 'right'
  },
  {
    title: 'Phương Thức',
    key: 'method',
    width: 150,
    align: 'center'
  },
  {
    title: 'Ngày Thanh Toán',
    key: 'paymentDate',
    width: 140,
    align: 'center'
  },
  {
    title: 'Mã Giao Dịch',
    key: 'transactionCode',
    width: 160
  },
  {
    title: 'Người Thu',
    key: 'receivedByName',
    width: 150
  },
  {
    title: 'Thao tác',
    key: 'actions',
    width: 150,
    align: 'center',
    fixed: 'right'
  }
]

const methodOptions = [
  { label: 'Tiền mặt', value: 'Cash' },
  { label: 'Chuyển khoản', value: 'BankTransfer' },
  { label: 'Momo', value: 'Momo' },
  { label: 'VNPay', value: 'VNPay' },
  { label: 'ZaloPay', value: 'ZaloPay' }
]

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

const formatDate = (date) => {
  if (!date) return '---'
  return new Date(date).toLocaleDateString('vi-VN')
}

const getMethodColor = (method) => {
  const colors = {
    'Cash': 'green',
    'BankTransfer': 'blue',
    'Momo': 'pink',
    'VNPay': 'orange',
    'ZaloPay': 'cyan'
  }
  return colors[method] || 'default'
}

const getMethodText = (method) => {
  const texts = {
    'Cash': 'Tiền mặt',
    'BankTransfer': 'Chuyển khoản',
    'Momo': 'Momo',
    'VNPay': 'VNPay',
    'ZaloPay': 'ZaloPay'
  }
  return texts[method] || method
}

const fetchPayments = async () => {
  loading.value = true
  try {
    const data = await paymentService.getAll()
    payments.value = data
    pagination.total = data.length
  } catch (error) {
    message.error(error.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  fetchPayments()
}

const handleTableChange = (pag) => {
  pagination.current = pag.current
  pagination.pageSize = pag.pageSize
}

const showCreateDialog = () => {
  editedIndex.value = -1
  editedItem.value = {
    invoiceCode: '',
    amount: 0,
    method: 'Cash',
    paymentDate: null,
    transactionCode: '',
    bankName: '',
    bankAccountNumber: '',
    note: ''
  }
  dialog.value = true
}

const viewPayment = (record) => {
  message.info(`Xem chi tiết: ${record.invoiceCode}`)
}

const editPayment = (record) => {
  editedIndex.value = payments.value.indexOf(record)
  editedItem.value = { ...record }
  dialog.value = true
}

const deletePayment = (record) => {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: `Bạn có chắc muốn xóa thanh toán này?`,
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await paymentService.delete(record.id)
        message.success('Xóa thành công')
        fetchPayments()
      } catch (error) {
        message.error('Xóa thất bại')
      }
    }
  })
}

const closeDialog = () => {
  dialog.value = false
}

const savePayment = async () => {
  try {
    if (editedIndex.value === -1) {
      await paymentService.create(editedItem.value)
      message.success('Ghi nhận thanh toán thành công')
    } else {
      await paymentService.update(editedItem.value.id, editedItem.value)
      message.success('Cập nhật thành công')
    }
    await fetchPayments()
    closeDialog()
  } catch (error) {
    message.error(error.message || 'Lỗi lưu dữ liệu')
  }
}

onMounted(() => {
  fetchPayments()
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
