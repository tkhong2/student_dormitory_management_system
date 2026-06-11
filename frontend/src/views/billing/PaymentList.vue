<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
            Quản Lý Thanh Toán
          </h1>
          <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
            Tổng số: {{ payments.length }} thanh toán
          </p>
        </div>
        <a-button type="primary" @click="showCreateDialog" style="background: #ff9800; border-color: #ff9800;">
          + Ghi Nhận Thanh Toán
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
            <a-form-item 
              label="Phiếu Thu" 
              required
              :validate-status="formErrors.invoiceId ? 'error' : ''"
              :help="formErrors.invoiceId"
            >
              <a-select 
                v-model:value="editedItem.invoiceId" 
                placeholder="Chọn phiếu thu cần thanh toán"
                show-search
                :filter-option="filterInvoice"
                @change="onInvoiceChange"
              >
                <a-select-option v-for="inv in invoices" :key="inv.id" :value="inv.id">
                  <span v-if="inv.invoiceType === 'DepositRefund'">
                    {{ inv.invoiceCode }} - {{ inv.studentName }} - Cần hoàn: {{ formatCurrency(Math.abs(inv.debtAmount)) }}
                  </span>
                  <span v-else>
                    {{ inv.invoiceCode }} - {{ inv.studentName }} - Nợ: {{ formatCurrency(inv.debtAmount) }}
                  </span>
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Số Tiền" 
              required
              :validate-status="formErrors.amount ? 'error' : ''"
              :help="formErrors.amount"
            >
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
            <a-form-item 
              label="Phương Thức" 
              required
              :validate-status="formErrors.method ? 'error' : ''"
              :help="formErrors.method"
            >
              <a-select v-model:value="editedItem.method" placeholder="Chọn phương thức">
                <a-select-option v-for="item in methodOptions" :key="item.value" :value="item.value">
                  {{ item.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Ngày Thanh Toán" 
              required
              :validate-status="formErrors.paymentDate ? 'error' : ''"
              :help="formErrors.paymentDate"
            >
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

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết Thanh Toán"
      width="700px"
      :footer="null"
    >
      <a-descriptions v-if="detailTarget" bordered :column="2" size="small">
        <a-descriptions-item label="Mã Phiếu Thu" :span="2">
          <a-typography-text strong copyable>{{ detailTarget.invoiceCode }}</a-typography-text>
        </a-descriptions-item>
        
        <a-descriptions-item label="Số Tiền" :span="2">
          <strong style="color: #52c41a; font-size: 18px;">{{ formatCurrency(detailTarget.amount) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Phương Thức">
          <a-tag :color="getMethodColor(detailTarget.method)">
            {{ getMethodText(detailTarget.method) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày Thanh Toán">
          {{ formatDate(detailTarget.paymentDate) }}
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.transactionCode" label="Mã Giao Dịch" :span="2">
          <a-typography-text code>{{ detailTarget.transactionCode }}</a-typography-text>
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.bankName" label="Ngân Hàng">
          {{ detailTarget.bankName }}
        </a-descriptions-item>
        <a-descriptions-item v-if="detailTarget.bankAccountNumber" label="Số Tài Khoản">
          {{ detailTarget.bankAccountNumber }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Người Thu" :span="2">
          {{ detailTarget.receivedByName }}
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.note" label="Ghi Chú" :span="2">
          {{ detailTarget.note }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Thời Gian Ghi Nhận" :span="2">
          {{ formatDate(detailTarget.paidAt) }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end;">
        <a-button @click="detailDialog = false">Đóng</a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import billService from '@/services/billService'
import { paymentService } from '@/services/paymentService'
import dayjs from 'dayjs'

const loading = ref(false)
const payments = ref([])
const invoices = ref([])
const search = ref('')
const methodFilter = ref(undefined)
const dateRange = ref(null)
const dialog = ref(false)
const detailDialog = ref(false)
const detailTarget = ref(null)
const editedIndex = ref(-1)
const editedItem = ref({
  invoiceId: null,
  amount: 0,
  method: 'Cash',
  paymentDate: dayjs(),
  transactionCode: '',
  bankName: '',
  bankAccountNumber: '',
  receivedByUserId: 1,
  receivedByName: 'Admin',
  note: ''
})

const formErrors = ref({})

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

const filterInvoice = (input, option) => {
  return option.children[0].children.toLowerCase().includes(input.toLowerCase())
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

const showCreateDialog = async () => {
  try {
    // Load invoices that need payment (including refunds)
    const allInvoices = await billService.getAll()
    invoices.value = allInvoices.filter(inv => 
      inv.status !== 'Paid' && inv.status !== 'Cancelled' && inv.debtAmount !== 0
    )
    
    if (invoices.value.length === 0) {
      message.warning('Không có phiếu thu nào cần thanh toán')
      return
    }

    editedIndex.value = -1
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    editedItem.value = {
      invoiceId: null,
      amount: 0,
      method: 'Cash',
      paymentDate: dayjs(),
      transactionCode: '',
      bankName: '',
      bankAccountNumber: '',
      receivedByUserId: user.id || 1,
      receivedByName: user.fullName || 'Admin',
      note: ''
    }
    formErrors.value = {}
    dialog.value = true
  } catch (error) {
    message.error('Không thể tải danh sách phiếu thu')
  }
}

const viewPayment = async (record) => {
  try {
    const data = await paymentService.getById(record.id)
    detailTarget.value = data
    detailDialog.value = true
  } catch (error) {
    message.error('Không thể tải chi tiết thanh toán')
  }
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
  // Validate
  const errors = {}
  if (!editedItem.value.invoiceId) {
    errors.invoiceId = 'Vui lòng chọn phiếu thu'
  }
  if (!editedItem.value.amount || editedItem.value.amount <= 0) {
    errors.amount = 'Vui lòng nhập số tiền hợp lệ'
  }
  if (!editedItem.value.method) {
    errors.method = 'Vui lòng chọn phương thức thanh toán'
  }
  if (!editedItem.value.paymentDate) {
    errors.paymentDate = 'Vui lòng chọn ngày thanh toán'
  }
  
  formErrors.value = errors
  if (Object.keys(errors).length > 0) {
    return
  }

  try {
    const paymentData = {
      ...editedItem.value,
      paymentDate: dayjs(editedItem.value.paymentDate).format('YYYY-MM-DD')
    }

    if (editedIndex.value === -1) {
      await paymentService.create(paymentData)
      message.success('Ghi nhận thanh toán thành công')
    } else {
      await paymentService.update(editedItem.value.id, paymentData)
      message.success('Cập nhật thành công')
    }
    await fetchPayments()
    closeDialog()
  } catch (error) {
    message.error(error.message || 'Lỗi lưu dữ liệu')
  }
}

const onInvoiceChange = (invoiceId) => {
  const selectedInvoice = invoices.value.find(inv => inv.id === invoiceId)
  if (selectedInvoice) {
    // For DepositRefund, amount should be absolute value (money to refund)
    if (selectedInvoice.invoiceType === 'DepositRefund') {
      editedItem.value.amount = Math.abs(selectedInvoice.debtAmount)
    } else {
      editedItem.value.amount = selectedInvoice.debtAmount
    }
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
