<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
            Quản Lý Phiếu Thu
          </h1>
          <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
            Tổng số: {{ invoices.length }} phiếu thu
          </p>
        </div>
        <a-space>
          <a-button @click="exportToExcelHandler" :loading="exporting">
            <template #icon><DownloadOutlined /></template>
            Xuất Excel
          </a-button>
          <a-button type="primary" @click="showCreateDialog" style="background: #ff9800; border-color: #ff9800;">
            + Tạo Phiếu Thu
          </a-button>
        </a-space>
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

          <template v-else-if="column.key === 'invoiceType'">
            <a-tag :color="getInvoiceTypeColor(record.invoiceType)">
              {{ getInvoiceTypeText(record.invoiceType) }}
            </a-tag>
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

          <template v-else-if="column.key === 'refundAmount'">
            <a-typography-text v-if="record.invoiceType === 'DepositRefund'" strong style="color: #722ed1">
              {{ formatCurrency(Math.abs(record.totalAmount)) }}
            </a-typography-text>
            <span v-else style="color: #d9d9d9">---</span>
          </template>

          <template v-else-if="column.key === 'debtAmount'">
            <a-typography-text :type="record.debtAmount > 0 ? 'danger' : 'success'" strong>
              {{ formatCurrency(Math.abs(record.debtAmount)) }}
            </a-typography-text>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="In phiếu thu">
                <a-button type="text" size="small" @click="printInvoiceHandler(record)" :loading="printing">
                  <template #icon><PrinterOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small" @click="viewInvoice(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip v-if="record.status === 'Unpaid' || record.status === 'PartialPaid' || record.status === 'Overdue'" title="Gửi nhắc nợ">
                <a-button type="text" size="small" @click="sendReminder(record)" style="color: #ff9800;">
                  <template #icon><BellOutlined /></template>
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

    <!-- Create Invoice Modal Component -->
    <CreateInvoiceModal
      v-model:open="createDialog"
      :form="editedItem"
      :contracts="contracts"
      :saving="saving"
      @save="saveInvoice"
      @contract-change="onContractChange"
      @invoice-type-change="onInvoiceTypeChange"
    />

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết Phiếu Thu"
      width="900px"
      :footer="null"
    >
      <a-descriptions v-if="detailTarget" bordered :column="2" size="small">
        <a-descriptions-item label="Mã Phiếu Thu" :span="2">
          <a-typography-text strong copyable>{{ detailTarget.invoiceCode }}</a-typography-text>
        </a-descriptions-item>
        
        <a-descriptions-item label="Sinh Viên">
          <strong>{{ detailTarget.studentName }}</strong> ({{ detailTarget.studentCode }})
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          <a-tag color="blue">{{ detailTarget.roomNumber }}</a-tag> {{ detailTarget.buildingName }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Kỳ thanh toán">
          Tháng {{ detailTarget.billingMonth }}/{{ detailTarget.billingYear }}
        </a-descriptions-item>
        <a-descriptions-item label="Loại phiếu thu">
          {{ detailTarget.invoiceType }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Trạng thái" :span="2">
          <a-tag :color="getStatusColor(detailTarget.status)">
            {{ getStatusText(detailTarget.status) }}
          </a-tag>
          <span v-if="detailTarget.overdueDays > 0" style="margin-left: 8px; color: #ff4d4f;">
            (Quá hạn {{ detailTarget.overdueDays }} ngày)
          </span>
        </a-descriptions-item>
        
        <a-descriptions-item label="Hạn thanh toán">
          {{ formatDate(detailTarget.dueDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Số lần nhắc nợ">
          <a-tag color="orange">{{ detailTarget.reminderCount || 0 }} lần</a-tag>
        </a-descriptions-item>
        
        <a-descriptions-item label="Chi tiết các khoản" :span="2">
          <a-table 
            :columns="itemColumns" 
            :data-source="detailTarget.items || []" 
            :pagination="false"
            size="small"
            bordered
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'amount'">
                {{ formatCurrency(record.amount) }}
              </template>
              <template v-else-if="column.key === 'unitPrice'">
                {{ formatCurrency(record.unitPrice) }}
              </template>
            </template>
          </a-table>
        </a-descriptions-item>
        
        <a-descriptions-item label="Tiền phòng">
          {{ formatCurrency(detailTarget.rentAmount) }}
        </a-descriptions-item>
        <a-descriptions-item label="Tiền điện">
          {{ formatCurrency(detailTarget.electricityAmount) }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Tiền nước">
          {{ formatCurrency(detailTarget.waterAmount) }}
        </a-descriptions-item>
        <a-descriptions-item label="Phí dịch vụ">
          {{ formatCurrency(detailTarget.serviceAmount) }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Nợ kỳ trước">
          {{ formatCurrency(detailTarget.previousDebt) }}
        </a-descriptions-item>
        <a-descriptions-item label="Giảm giá">
          {{ formatCurrency(detailTarget.discount) }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Tiền phạt">
          <span style="color: #ff4d4f;">{{ formatCurrency(detailTarget.penaltyAmount) }}</span>
        </a-descriptions-item>
        <a-descriptions-item label="Tổng tiền">
          <strong style="color: #1890ff; font-size: 16px;">{{ formatCurrency(detailTarget.totalAmount) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Đã thanh toán">
          <strong style="color: #52c41a; font-size: 16px;">{{ formatCurrency(detailTarget.paidAmount) }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Còn nợ">
          <strong style="color: #ff4d4f; font-size: 16px;">{{ formatCurrency(detailTarget.debtAmount) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.notes" label="Ghi chú" :span="2">
          {{ detailTarget.notes }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Ngày tạo" :span="2">
          {{ formatDate(detailTarget.createdAt) }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="detailDialog = false">Đóng</a-button>
        <a-button 
          v-if="detailTarget?.status !== 'Paid' && detailTarget?.status !== 'Cancelled'" 
          type="primary"
          style="background: #ff9800; border-color: #ff9800;"
          @click="sendReminder(detailTarget)"
        >
          <template #icon><BellOutlined /></template>
          Gửi nhắc nợ
        </a-button>
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
  DeleteOutlined,
  BellOutlined,
  DownloadOutlined,
  PrinterOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import billService from '@/services/billService'
import { contractService } from '@/services/contractService'
import notificationService from '@/services/notificationService'
import dayjs from 'dayjs'
import CreateInvoiceModal from './CreateInvoiceModal.vue'
import { exportToExcel } from '@/utils/excelExport'
import { usePrintPDF } from '@/composables/usePrintPDF'

const { printing, printInvoice } = usePrintPDF()

const loading = ref(false)
const saving = ref(false)
const exporting = ref(false)
const invoices = ref([])
const contracts = ref([])
const search = ref('')
const statusFilter = ref(undefined)
const monthFilter = ref(undefined)
const yearFilter = ref(undefined)
const createDialog = ref(false)
const detailDialog = ref(false)
const detailTarget = ref(null)

const editedItem = ref({
  invoiceCode: '',
  invoiceType: 'Monthly',
  contractId: null,
  studentId: null,
  studentName: '',
  studentCode: '',
  roomId: null,
  roomNumber: '',
  buildingName: '',
  billingMonth: new Date().getMonth() + 1,
  billingYear: new Date().getFullYear(),
  rentAmount: 0,
  // Electricity
  electricityStartIndex: 0,
  electricityEndIndex: 0,
  electricityUnitPrice: 3500,
  electricityAmount: 0,
  // Water
  waterStartIndex: 0,
  waterEndIndex: 0,
  waterUnitPrice: 15000,
  waterAmount: 0,
  // Other
  serviceAmount: 0,
  previousDebt: 0,
  discount: 0,
  discountReason: '',
  dueDate: dayjs().add(10, 'day'),
  createdByUserId: 1,
  createdByName: '',
  paymentMethod: 'BankTransfer',
  notes: '',
  items: []
})

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
    title: 'Loại',
    key: 'invoiceType',
    width: 130,
    align: 'center'
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
    title: 'Được Hoàn',
    key: 'refundAmount',
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

const itemColumns = [
  { title: 'Khoản thu', dataIndex: 'itemName', key: 'itemName' },
  { title: 'Mô tả', dataIndex: 'itemDescription', key: 'itemDescription' },
  { title: 'SL', dataIndex: 'quantity', key: 'quantity', align: 'center', width: 60 },
  { title: 'ĐVT', dataIndex: 'unit', key: 'unit', align: 'center', width: 80 },
  { title: 'Đơn giá', key: 'unitPrice', align: 'right', width: 120 },
  { title: 'Thành tiền', key: 'amount', align: 'right', width: 120 }
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

const formatDate = (value) => {
  if (!value) return '---'
  return new Date(value).toLocaleDateString('vi-VN')
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

const getInvoiceTypeColor = (type) => {
  const colors = {
    'Monthly': 'blue',
    'Deposit': 'green',
    'DepositRefund': 'purple',
    'Other': 'default'
  }
  return colors[type] || 'default'
}

const getInvoiceTypeText = (type) => {
  const texts = {
    'Monthly': 'Tiền phòng',
    'Deposit': 'Tiền cọc',
    'DepositRefund': 'Hoàn cọc',
    'Other': 'Khác'
  }
  return texts[type] || type
}

const fetchInvoices = async () => {
  loading.value = true
  try {
    const data = await billService.getAll()
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

const showCreateDialog = async () => {
  try {
    // Load active contracts
    loading.value = true
    const allContracts = await contractService.getAll()
    contracts.value = allContracts.filter(c => c.status === 'Active')
    
    if (contracts.value.length === 0) {
      message.warning('Không có hợp đồng Active nào. Vui lòng duyệt đơn đăng ký trước.')
      return
    }

    // Get next invoice code from backend
    const now = new Date()
    const billingMonth = now.getMonth() + 1
    const billingYear = now.getFullYear()
    
    let autoInvoiceCode = `PTT${billingYear}${String(billingMonth).padStart(2, '0')}001` // Default fallback
    
    try {
      const nextCodeResponse = await billService.getNextInvoiceCode('Monthly', billingMonth, billingYear)
      autoInvoiceCode = nextCodeResponse.invoiceCode
      console.log('Generated invoice code from backend:', autoInvoiceCode)
    } catch (error) {
      console.error('Failed to get next invoice code from backend, using fallback:', error)
    }

    const user = JSON.parse(localStorage.getItem('user') || '{}')
    editedItem.value = {
      invoiceCode: autoInvoiceCode,
      invoiceType: 'Monthly',
      contractId: null,
      studentId: null,
      studentName: '',
      studentCode: '',
      roomId: null,
      roomNumber: '',
      buildingName: '',
      billingMonth: billingMonth,
      billingYear: billingYear,
      rentAmount: 0,
      // Electricity
      electricityStartIndex: 0,
      electricityEndIndex: 0,
      electricityUnitPrice: 3500,
      electricityAmount: 0,
      // Water
      waterStartIndex: 0,
      waterEndIndex: 0,
      waterUnitPrice: 15000,
      waterAmount: 0,
      // Other
      serviceAmount: 50000,
      previousDebt: 0,
      discount: 0,
      discountReason: '',
      dueDate: dayjs().date(10).add(1, 'month'), // Ngày 10 tháng sau
      createdByUserId: user.id || 1,
      createdByName: user.fullName || 'Admin',
      paymentMethod: 'BankTransfer',
      notes: '',
      items: []
    }
    createDialog.value = true
  } catch (error) {
    message.error('Không thể tải danh sách hợp đồng')
  } finally {
    loading.value = false
  }
}

const onContractChange = async (contractId) => {
  try {
    const selectedContract = contracts.value.find(c => c.id === contractId)
    if (!selectedContract) return

    // Auto-fill from contract
    editedItem.value.studentId = selectedContract.studentId
    editedItem.value.studentName = selectedContract.studentName || selectedContract.studentFullName
    editedItem.value.studentCode = selectedContract.studentCode
    editedItem.value.roomId = selectedContract.roomId
    editedItem.value.roomNumber = selectedContract.roomNumber
    editedItem.value.buildingName = selectedContract.buildingName
    
    // Only auto-fill rent amount for Monthly invoice type
    if (editedItem.value.invoiceType === 'Monthly') {
      editedItem.value.rentAmount = selectedContract.monthlyRent || selectedContract.rentAmount || 0
    } else {
      // For Deposit, DepositRefund, and Other types, don't auto-fill rent amount
      editedItem.value.rentAmount = 0
    }
    
    // Tính hạn thanh toán: Lấy ngày bắt đầu hợp đồng + thêm tháng
    const contractStart = dayjs(selectedContract.startDate || selectedContract.effectiveDate)
    const dayOfMonth = contractStart.date() // Lấy ngày trong tháng
    editedItem.value.dueDate = dayjs().month(editedItem.value.billingMonth - 1).date(dayOfMonth)

    // Kiểm tra xem phiếu thu cho hợp đồng này ở tháng này đã tồn tại chưa
    const existingInvoice = invoices.value.find(inv => 
      inv.contractId === contractId && 
      inv.billingMonth === editedItem.value.billingMonth &&
      inv.billingYear === editedItem.value.billingYear &&
      inv.invoiceType === editedItem.value.invoiceType &&
      inv.status !== 'Cancelled'
    )
    
    if (existingInvoice) {
      message.warning(`Phiếu thu ${existingInvoice.invoiceCode} đã tồn tại cho hợp đồng này ở kỳ ${editedItem.value.billingMonth}/${editedItem.value.billingYear}`)
    } else {
      message.success('Đã tự động điền thông tin từ hợp đồng')
    }
  } catch (error) {
    message.error('Lỗi khi load thông tin hợp đồng')
  }
}

// Auto calculate electricity amount
const calculateElectricityAmount = () => {
  const { electricityStartIndex, electricityEndIndex, electricityUnitPrice } = editedItem.value
  const usage = (electricityEndIndex || 0) - (electricityStartIndex || 0)
  editedItem.value.electricityAmount = usage > 0 ? usage * electricityUnitPrice : 0
}

// Auto calculate water amount  
const calculateWaterAmount = () => {
  const { waterStartIndex, waterEndIndex, waterUnitPrice } = editedItem.value
  const usage = (waterEndIndex || 0) - (waterStartIndex || 0)
  editedItem.value.waterAmount = usage > 0 ? usage * waterUnitPrice : 0
}

// Handle invoice type change - regenerate invoice code
const onInvoiceTypeChange = async (invoiceType) => {
  try {
    const billingMonth = editedItem.value.billingMonth
    const billingYear = editedItem.value.billingYear
    
    const nextCodeResponse = await billService.getNextInvoiceCode(invoiceType, billingMonth, billingYear)
    editedItem.value.invoiceCode = nextCodeResponse.invoiceCode
    
    console.log('Updated invoice code after type change:', editedItem.value.invoiceCode, 'for type:', invoiceType)
    
    // Auto-adjust rent amount based on invoice type
    if (invoiceType === 'Monthly' && editedItem.value.contractId) {
      const selectedContract = contracts.value.find(c => c.id === editedItem.value.contractId)
      if (selectedContract) {
        editedItem.value.rentAmount = selectedContract.monthlyRent || selectedContract.rentAmount || 0
      }
    } else if (invoiceType === 'Deposit' && editedItem.value.contractId) {
      const selectedContract = contracts.value.find(c => c.id === editedItem.value.contractId)
      if (selectedContract) {
        editedItem.value.rentAmount = selectedContract.depositAmount || selectedContract.monthlyRent || 0
      }
    } else {
      editedItem.value.rentAmount = 0
    }
  } catch (error) {
    console.error('Failed to get next invoice code:', error)
    message.error('Không thể tạo mã phiếu thu mới')
  }
}

const closeCreateDialog = () => {
  createDialog.value = false
}

const calculateTotal = () => {
  const { rentAmount, electricityAmount, waterAmount, serviceAmount, previousDebt, discount } = editedItem.value
  return (rentAmount || 0) + (electricityAmount || 0) + (waterAmount || 0) + 
         (serviceAmount || 0) + (previousDebt || 0) - (discount || 0)
}

const filterContract = (input, option) => {
  const text = option.children[0].children
  if (Array.isArray(text)) {
    return text.some(child => 
      typeof child === 'string' && child.toLowerCase().includes(input.toLowerCase())
    )
  }
  return false
}

const saveInvoice = async () => {
  // Validate
  if (!editedItem.value.invoiceCode) {
    message.error('Vui lòng nhập mã phiếu thu')
    return
  }
  if (!editedItem.value.contractId || !editedItem.value.studentId) {
    message.error('Vui lòng nhập đầy đủ thông tin hợp đồng và sinh viên')
    return
  }
  if (!editedItem.value.dueDate) {
    message.error('Vui lòng chọn hạn thanh toán')
    return
  }

  saving.value = true
  try {
    const invoiceData = {
      ...editedItem.value,
      dueDate: dayjs(editedItem.value.dueDate).format('YYYY-MM-DD')
    }
    
    await billService.create(invoiceData)
    message.success('Tạo phiếu thu thành công')
    closeCreateDialog()
    await fetchInvoices()
  } catch (error) {
    console.error('Error creating invoice:', error)
    
    // Handle specific error cases
    if (error.response?.status === 409) {
      // Conflict - duplicate invoice
      const errorData = error.response.data
      message.error({
        content: errorData.message || 'Phiếu thu đã tồn tại cho kỳ này',
        duration: 5
      })
    } else {
      message.error(error.response?.data?.message || error.message || 'Lỗi tạo phiếu thu')
    }
  } finally {
    saving.value = false
  }
}

const viewInvoice = async (record) => {
  try {
    const data = await billService.getById(record.id)
    detailTarget.value = data
    detailDialog.value = true
  } catch (error) {
    message.error('Không thể tải chi tiết phiếu thu')
  }
}

const sendReminder = (record) => {
  console.log('=== SEND REMINDER DEBUG ===')
  console.log('Invoice record:', record)
  console.log('Student ID:', record.studentId)
  console.log('Student Name:', record.studentName)
  
  // TEST: Try with minimal data first
  const testData = {
    userId: 1, // Try with a simple ID first
    title: 'Test notification',
    body: 'This is a test',
    type: 'System'
  }
  console.log('TEST: Sending minimal notification:', testData)
  
  Modal.confirm({
    title: 'Gửi nhắc nợ',
    content: `Bạn có chắc muốn gửi thông báo nhắc nợ cho sinh viên ${record.studentName}?`,
    okText: 'Gửi',
    cancelText: 'Hủy',
    async onOk() {
      try {
        // TEST: Send minimal notification first
        console.log('=== TESTING MINIMAL NOTIFICATION ===')
        try {
          const testResult = await notificationService.create(testData)
          console.log('TEST SUCCESS:', testResult)
        } catch (testError) {
          console.error('TEST FAILED:', testError)
          console.error('TEST Error response:', testError.response)
        }
        
        // Send email reminder (existing functionality)
        try {
          await billService.sendReminder(record.id)
          console.log('Email reminder sent successfully')
        } catch (emailError) {
          console.warn('Email reminder failed:', emailError)
        }
        
        // Also create notification in student portal
        const notificationData = {
          userId: record.studentId,
          title: 'Nhắc nhở thanh toán',
          body: `Bạn có khoản thanh toán ${record.invoiceCode} đến hạn ngày ${formatDate(record.dueDate)}. Tổng tiền: ${formatCurrency(record.debtAmount)}. Vui lòng thanh toán đúng hạn để tránh phát sinh phí phạt.`,
          type: 'System',
          iconType: record.status === 'Overdue' ? 'error' : 'warning',
          actionUrl: '/student/my-payments',
          relatedEntityId: record.id,
          relatedEntityType: 'Invoice'
        }
        
        console.log('=== NOTIFICATION DATA ===')
        console.log('Sending notification with data:', JSON.stringify(notificationData, null, 2))
        
        try {
          const notifResponse = await notificationService.create(notificationData)
          console.log('Notification created successfully:', notifResponse)
          message.success(`Đã gửi thông báo nhắc nợ đến ${record.studentName}`)
        } catch (notifError) {
          console.error('=== NOTIFICATION ERROR ===')
          console.error('Error object:', notifError)
          console.error('Error response:', notifError.response)
          console.error('Error response data:', notifError.response?.data)
          console.error('Error status:', notifError.response?.status)
          message.warning(`Đã gửi email nhưng không thể tạo thông báo. Lỗi: ${notifError.response?.data?.message || notifError.message}`)
        }
        
        detailDialog.value = false
        fetchInvoices()
      } catch (error) {
        console.error('Send reminder error:', error)
        message.error(error.message || 'Gửi nhắc nợ thất bại')
      }
    }
  })
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
        await billService.delete(record.id)
        message.success('Xóa thành công')
        fetchInvoices()
      } catch (error) {
        message.error('Xóa thất bại')
      }
    }
  })
}

// Export to Excel
const exportToExcelHandler = async () => {
  if (invoices.value.length === 0) {
    message.warning('Không có dữ liệu để xuất')
    return
  }

  exporting.value = true
  try {
    const columnMapping = {
      invoiceCode: 'Mã phiếu thu',
      studentName: 'Sinh viên',
      studentCode: 'Mã SV',
      roomNumber: 'Phòng',
      buildingName: 'Tòa nhà',
      billingMonth: 'Tháng',
      billingYear: 'Năm',
      totalAmount: 'Tổng tiền',
      paidAmount: 'Đã trả',
      debtAmount: 'Còn nợ',
      dueDate: 'Hạn thanh toán',
      status: 'Trạng thái',
      createdAt: 'Ngày tạo'
    }

    const now = new Date()
    const dateStr = `${now.getDate()}_${now.getMonth() + 1}_${now.getFullYear()}`
    
    await exportToExcel(invoices.value, columnMapping, `Danh_sach_phieu_thu_${dateStr}`, 'Phiếu thu')
    message.success('Xuất Excel thành công')
  } catch (error) {
    message.error('Lỗi xuất Excel')
  } finally {
    exporting.value = false
  }
}

// Print invoice PDF
const printInvoiceHandler = async (record) => {
  await printInvoice(record)
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
