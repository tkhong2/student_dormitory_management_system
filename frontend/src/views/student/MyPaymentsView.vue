<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">Thanh toán của tôi</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Lịch sử và trạng thái thanh toán lệ phí KTX
        </p>
      </div>
    </div>

    <!-- Summary Cards -->
    <a-row :gutter="16" style="margin-bottom: 24px">
      <a-col :xs="24" :sm="8">
        <a-card :bordered="false" style="background: linear-gradient(135deg, #1890ff 0%, #096dd9 100%)">
          <a-statistic
            title="Tổng đã thanh toán"
            :value="totalPaid"
            :precision="0"
            suffix="đ"
            :value-style="{ color: '#fff', fontSize: '24px', fontWeight: 'bold' }"
          >
            <template #title>
              <span style="color: rgba(255,255,255,0.85); font-size: 13px">Tổng đã thanh toán</span>
            </template>
          </a-statistic>
          <div style="color: rgba(255,255,255,0.7); font-size: 12px; margin-top: 8px">
            {{ paidCount }} phiếu thu
          </div>
        </a-card>
      </a-col>
      
      <a-col :xs="24" :sm="8">
        <a-card :bordered="false" style="border: 2px solid #ff9800">
          <a-statistic
            title="Cần thanh toán"
            :value="totalUnpaid"
            :precision="0"
            suffix="đ"
            :value-style="{ color: '#ff9800', fontSize: '24px', fontWeight: 'bold' }"
          >
            <template #title>
              <span style="color: #666; font-size: 13px">Cần thanh toán</span>
            </template>
          </a-statistic>
          <div style="color: #8c8c8c; font-size: 12px; margin-top: 8px">
            {{ unpaidCount }} phiếu thu chưa thanh toán
          </div>
        </a-card>
      </a-col>
      
      <a-col :xs="24" :sm="8">
        <a-card :bordered="false" style="border: 2px solid #f44336">
          <a-statistic
            title="Quá hạn"
            :value="totalOverdue"
            :precision="0"
            suffix="đ"
            :value-style="{ color: '#f44336', fontSize: '24px', fontWeight: 'bold' }"
          >
            <template #title>
              <span style="color: #666; font-size: 13px">Quá hạn</span>
            </template>
          </a-statistic>
          <div style="color: #8c8c8c; font-size: 12px; margin-top: 8px">
            {{ overdueCount }} phiếu thu quá hạn
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Payment History -->
    <a-card :bordered="false" :loading="loading">
      <template #title>
        <span style="font-weight: 600">Lịch sử phiếu thu</span>
      </template>
      
      <a-table
        :columns="columns"
        :data-source="invoices"
        :pagination="{ pageSize: 10 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'invoiceCode'">
            <a-typography-text strong copyable>{{ record.invoiceCode }}</a-typography-text>
          </template>
          
          <template v-else-if="column.key === 'period'">
            <a-tag color="blue">{{ record.billingMonth }}/{{ record.billingYear }}</a-tag>
          </template>
          
          <template v-else-if="column.key === 'totalAmount'">
            <strong>{{ formatCurrency(record.totalAmount) }}</strong>
          </template>
          
          <template v-else-if="column.key === 'dueDate'">
            {{ formatDate(record.dueDate) }}
          </template>
          
          <template v-else-if="column.key === 'paidAmount'">
            <span style="color: #52c41a; font-weight: 500">
              {{ formatCurrency(record.paidAmount) }}
            </span>
          </template>
          
          <template v-else-if="column.key === 'debtAmount'">
            <span :style="{ color: record.debtAmount > 0 ? '#ff4d4f' : '#52c41a', fontWeight: '500' }">
              {{ formatCurrency(record.debtAmount) }}
            </span>
          </template>
          
          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button
                v-if="record.status !== 'Paid'"
                type="primary"
                size="small"
                style="background: #ff9800; border-color: #ff9800"
                @click="openQRPayment(record)"
              >
                <template #icon><QrcodeOutlined /></template>
                QR Thanh toán
              </a-button>
              <a-button
                v-if="record.status !== 'Paid'"
                type="default"
                size="small"
                @click="viewInvoiceDetail(record)"
              >
                <template #icon><CreditCardOutlined /></template>
                Chi tiết
              </a-button>
              <a-button
                v-else
                type="default"
                size="small"
                @click="viewInvoiceDetail(record)"
              >
                <template #icon><EyeOutlined /></template>
                Chi tiết
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết Phiếu Thu"
      width="900px"
      :footer="null"
    >
      <a-descriptions v-if="detailTarget" bordered :column="2" size="default">
        <a-descriptions-item label="Mã Phiếu Thu" :span="2">
          <a-typography-text strong copyable style="font-size: 16px;">{{ detailTarget.invoiceCode }}</a-typography-text>
        </a-descriptions-item>
        
        <a-descriptions-item label="Loại phiếu thu">
          <a-tag :color="getInvoiceTypeColor(detailTarget.invoiceType)">
            {{ getInvoiceTypeText(detailTarget.invoiceType) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Kỳ thanh toán">
          <strong>Tháng {{ detailTarget.billingMonth }}/{{ detailTarget.billingYear }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Phòng">
          <a-tag color="blue">{{ detailTarget.roomNumber }}</a-tag> {{ detailTarget.buildingName }}
        </a-descriptions-item>
        <a-descriptions-item label="Hạn thanh toán">
          <strong>{{ formatDate(detailTarget.dueDate) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Trạng thái" :span="2">
          <a-tag :color="getStatusColor(detailTarget.status)" style="font-size: 14px; padding: 4px 12px;">
            {{ getStatusText(detailTarget.status) }}
          </a-tag>
          <span v-if="detailTarget.overdueDays > 0" style="margin-left: 8px; color: #ff4d4f; font-weight: 500;">
            (Quá hạn {{ detailTarget.overdueDays }} ngày)
          </span>
        </a-descriptions-item>
        
        <!-- Chi tiết các khoản phí -->
        <a-descriptions-item label="Chi tiết các khoản" :span="2">
          <a-table 
            :columns="itemColumns" 
            :data-source="detailTarget.items && detailTarget.items.length > 0 ? detailTarget.items : getDefaultItems(detailTarget)" 
            :pagination="false"
            size="middle"
            bordered
            style="margin-top: 8px;"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'itemName'">
                <strong>{{ record.itemName }}</strong>
                <div v-if="record.itemDescription" style="font-size: 12px; color: #8c8c8c;">
                  {{ record.itemDescription }}
                </div>
              </template>
              <template v-else-if="column.key === 'quantity'">
                <span v-if="record.quantity">{{ record.quantity }}</span>
                <span v-else style="color: #d9d9d9;">-</span>
              </template>
              <template v-else-if="column.key === 'unit'">
                <span v-if="record.unit">{{ record.unit }}</span>
                <span v-else style="color: #d9d9d9;">-</span>
              </template>
              <template v-else-if="column.key === 'unitPrice'">
                <span v-if="record.unitPrice" style="font-weight: 500;">{{ formatCurrency(record.unitPrice) }}</span>
                <span v-else style="color: #d9d9d9;">-</span>
              </template>
              <template v-else-if="column.key === 'amount'">
                <strong :style="{ color: record.amount < 0 ? '#52c41a' : '#1890ff', fontSize: '15px' }">
                  {{ formatCurrency(record.amount) }}
                </strong>
              </template>
            </template>
            <template #summary>
              <a-table-summary fixed>
                <a-table-summary-row style="background: #fafafa;">
                  <a-table-summary-cell :col-span="4" align="right">
                    <strong style="font-size: 16px;">TỔNG CỘNG:</strong>
                  </a-table-summary-cell>
                  <a-table-summary-cell align="right">
                    <strong style="color: #1890ff; font-size: 18px;">
                      {{ formatCurrency(detailTarget.totalAmount) }}
                    </strong>
                  </a-table-summary-cell>
                </a-table-summary-row>
              </a-table-summary>
            </template>
          </a-table>
        </a-descriptions-item>
        
        <!-- Thông tin thanh toán -->
        <a-descriptions-item label="Tổng tiền" :span="2">
          <strong style="color: #1890ff; font-size: 18px;">{{ formatCurrency(detailTarget.totalAmount) }}</strong>
        </a-descriptions-item>
        
        <a-descriptions-item label="Đã thanh toán">
          <strong style="color: #52c41a; font-size: 16px;">{{ formatCurrency(detailTarget.paidAmount) }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Còn nợ">
          <strong :style="{ color: detailTarget.debtAmount > 0 ? '#ff4d4f' : '#52c41a', fontSize: '16px' }">
            {{ formatCurrency(detailTarget.debtAmount) }}
          </strong>
        </a-descriptions-item>
        
        <a-descriptions-item v-if="detailTarget.notes" label="Ghi chú" :span="2">
          <a-alert type="info" :message="detailTarget.notes" show-icon />
        </a-descriptions-item>
      </a-descriptions>

      <a-alert
        v-if="detailTarget?.status !== 'Paid'"
        message="Hướng dẫn thanh toán"
        type="warning"
        show-icon
        style="margin-top: 16px"
      >
        <template #description>
          <p style="margin-bottom: 8px;"><strong>Vui lòng chuyển khoản đến:</strong></p>
          <p style="margin: 4px 0;"><strong>🏦 Ngân hàng:</strong> Sacombank - Ngân hàng TMCP Sài Gòn Thương Tín</p>
          <p style="margin: 4px 0;"><strong>👤 Chủ tài khoản:</strong> TRAN KHAC HONG</p>
          <p style="margin: 4px 0;"><strong>💳 Số tài khoản:</strong> <a-typography-text copyable strong>025202102005</a-typography-text></p>
          <p style="margin: 4px 0;"><strong>💰 Số tiền:</strong> <span style="color: #ff9800; font-weight: bold;">{{ formatCurrency(detailTarget.debtAmount) }}</span></p>
          <p style="margin: 4px 0;"><strong>✍️ Nội dung:</strong> <a-typography-text copyable strong type="danger">{{ detailTarget.invoiceCode }} {{ detailTarget.studentCode }}</a-typography-text></p>
        </template>
      </a-alert>

      <div style="margin-top: 16px; display: flex; justify-content: space-between; align-items: center;">
        <a-button type="default" @click="detailDialog = false">Đóng</a-button>
        <a-button 
          v-if="detailTarget?.status !== 'Paid'" 
          type="primary" 
          style="background: #ff9800; border-color: #ff9800;"
          @click="detailDialog = false; openQRPayment(detailTarget)"
        >
          <template #icon><QrcodeOutlined /></template>
          Thanh toán ngay
        </a-button>
      </div>
    </a-modal>

    <!-- QR Payment Modal -->
    <a-modal
      v-model:open="qrDialog"
      title="Thanh toán QR Code"
      width="600px"
      :footer="null"
      @cancel="stopAutoCheckPayment"
    >
      <div v-if="selectedInvoice" style="text-align: center;">
        <!-- Auto-check status indicator -->
        <a-alert
          v-if="autoCheckInterval"
          message="Đang tự động kiểm tra thanh toán..."
          type="info"
          show-icon
          style="margin-bottom: 16px;"
        >
          <template #description>
            <div style="display: flex; align-items: center; gap: 8px;">
              <a-spin size="small" />
              <span>Hệ thống đang kiểm tra mỗi 5 giây ({{ checkCount }}/60)</span>
            </div>
          </template>
        </a-alert>

        <!-- Invoice Info -->
        <a-descriptions bordered :column="2" size="small" style="margin-bottom: 24px;">
          <a-descriptions-item label="Mã Phiếu Thu" :span="2">
            <a-typography-text strong copyable>{{ selectedInvoice.invoiceCode }}</a-typography-text>
          </a-descriptions-item>
          <a-descriptions-item label="Kỳ thanh toán">
            Tháng {{ selectedInvoice.billingMonth }}/{{ selectedInvoice.billingYear }}
          </a-descriptions-item>
          <a-descriptions-item label="Số tiền">
            <strong style="color: #ff9800; font-size: 18px;">{{ formatCurrency(selectedInvoice.debtAmount) }}</strong>
          </a-descriptions-item>
        </a-descriptions>

        <!-- QR Code -->
        <a-spin :spinning="qrLoading" tip="Đang tạo mã QR...">
          <div style="background: #f5f5f5; padding: 24px; border-radius: 8px; margin-bottom: 16px;">
            <img 
              v-if="qrCodeUrl" 
              :src="qrCodeUrl" 
              alt="QR Code" 
              style="max-width: 300px; width: 100%; height: auto; margin: 0 auto; display: block;"
            />
            <div v-else style="width: 300px; height: 300px; margin: 0 auto; background: #fff; display: flex; align-items: center; justify-content: center;">
              <a-typography-text type="secondary">Đang tải QR Code...</a-typography-text>
            </div>
          </div>
        </a-spin>

        <!-- Payment Instructions -->
        <a-alert
          message="Hướng dẫn thanh toán"
          type="info"
          show-icon
          style="text-align: left;"
        >
          <template #description>
            <ol style="margin: 0; padding-left: 20px;">
              <li>Mở ứng dụng ngân hàng của bạn</li>
              <li>Chọn chức năng quét mã QR</li>
              <li>Quét mã QR trên màn hình</li>
              <li>Kiểm tra thông tin và xác nhận thanh toán</li>
              <li><strong>Hệ thống sẽ tự động cập nhật sau khi thanh toán (1-5 giây)</strong></li>
            </ol>
          </template>
        </a-alert>

        <!-- Bank Info -->
        <a-descriptions bordered size="small" style="margin-top: 16px;">
          <a-descriptions-item label="Ngân hàng" :span="2">
            <strong>Sacombank - Ngân hàng TMCP Sài Gòn Thương Tín</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Chủ tài khoản" :span="2">
            TRAN KHAC HONG
          </a-descriptions-item>
          <a-descriptions-item label="Số tài khoản" :span="2">
            <a-typography-text copyable strong>025202102005</a-typography-text>
          </a-descriptions-item>
          <a-descriptions-item label="Nội dung chuyển khoản" :span="2">
            <a-typography-text copyable strong type="danger">
              {{ selectedInvoice.invoiceCode }} {{ selectedInvoice.studentCode }}
            </a-typography-text>
          </a-descriptions-item>
        </a-descriptions>

        <!-- Action Buttons -->
        <div style="margin-top: 24px; display: flex; justify-content: center; gap: 8px;">
          <a-button type="default" @click="stopAutoCheckPayment(); qrDialog = false">Đóng</a-button>
          <a-button type="primary" @click="checkPaymentStatus" :loading="qrLoading">
            Kiểm tra ngay
          </a-button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { message } from 'ant-design-vue'
import { CreditCardOutlined, EyeOutlined, QrcodeOutlined } from '@ant-design/icons-vue'
import { useAuthStore } from '@/stores/auth'
import { API_URLS, getAuthHeaders } from '@/services/apiService'
import axios from 'axios'

const authStore = useAuthStore()

const loading = ref(false)
const invoices = ref([])
const detailDialog = ref(false)
const detailTarget = ref(null)
const qrDialog = ref(false)
const qrLoading = ref(false)
const qrCodeUrl = ref('')
const selectedInvoice = ref(null)
const autoCheckInterval = ref(null)
const checkCount = ref(0)

const columns = [
  { title: 'Mã Phiếu Thu', key: 'invoiceCode', width: 140 },
  { title: 'Kỳ', key: 'period', align: 'center', width: 100 },
  { title: 'Tổng Tiền', key: 'totalAmount', align: 'right', width: 130 },
  { title: 'Hạn TT', key: 'dueDate', align: 'center', width: 110 },
  { title: 'Đã Trả', key: 'paidAmount', align: 'right', width: 130 },
  { title: 'Còn Nợ', key: 'debtAmount', align: 'right', width: 130 },
  { title: 'Trạng thái', key: 'status', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 150 }
]

const itemColumns = [
  { title: 'Khoản thu', key: 'itemName', dataIndex: 'itemName', width: '30%' },
  { title: 'Số lượng', key: 'quantity', dataIndex: 'quantity', align: 'center', width: '12%' },
  { title: 'Đơn vị', key: 'unit', dataIndex: 'unit', align: 'center', width: '12%' },
  { title: 'Đơn giá', key: 'unitPrice', dataIndex: 'unitPrice', align: 'right', width: '23%' },
  { title: 'Thành tiền', key: 'amount', dataIndex: 'amount', align: 'right', width: '23%' }
]

const totalPaid = computed(() => {
  return invoices.value
    .filter(inv => inv.status === 'Paid')
    .reduce((sum, inv) => sum + inv.paidAmount, 0)
})

const totalUnpaid = computed(() => {
  return invoices.value
    .filter(inv => inv.status === 'Unpaid' || inv.status === 'PartialPaid')
    .reduce((sum, inv) => sum + inv.debtAmount, 0)
})

const totalOverdue = computed(() => {
  return invoices.value
    .filter(inv => inv.status === 'Overdue')
    .reduce((sum, inv) => sum + inv.debtAmount, 0)
})

const paidCount = computed(() => {
  return invoices.value.filter(inv => inv.status === 'Paid').length
})

const unpaidCount = computed(() => {
  return invoices.value.filter(inv => inv.status === 'Unpaid' || inv.status === 'PartialPaid').length
})

const overdueCount = computed(() => {
  return invoices.value.filter(inv => inv.status === 'Overdue').length
})

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
    'Monthly': '💰 Tiền phòng tháng',
    'Deposit': '🏦 Tiền cọc',
    'DepositRefund': '💸 Hoàn cọc',
    'Other': '📝 Khác'
  }
  return texts[type] || type
}

const getDefaultItems = (invoice) => {
  const items = []
  if (invoice.rentAmount > 0) {
    items.push({ itemName: 'Tiền phòng', itemDescription: '', amount: invoice.rentAmount })
  }
  if (invoice.electricityAmount > 0) {
    items.push({ itemName: 'Tiền điện', itemDescription: '', amount: invoice.electricityAmount })
  }
  if (invoice.waterAmount > 0) {
    items.push({ itemName: 'Tiền nước', itemDescription: '', amount: invoice.waterAmount })
  }
  if (invoice.serviceAmount > 0) {
    items.push({ itemName: 'Phí dịch vụ', itemDescription: '', amount: invoice.serviceAmount })
  }
  if (invoice.previousDebt > 0) {
    items.push({ itemName: 'Nợ kỳ trước', itemDescription: '', amount: invoice.previousDebt })
  }
  if (invoice.discount > 0) {
    items.push({ itemName: 'Giảm giá', itemDescription: '', amount: -invoice.discount })
  }
  return items
}

const fetchInvoices = async () => {
  loading.value = true
  try {
    // Get user from auth store
    const currentUser = authStore.user
    console.log('=== FETCH INVOICES START ===')
    console.log('Current user from auth store:', currentUser)
    
    if (!currentUser) {
      message.warning('Vui lòng đăng nhập để xem phiếu thu')
      invoices.value = []
      loading.value = false
      return
    }
    
    let studentId = null
    
    // Method 1: Try to get student by UserId (most reliable method)
    // The Student table in ContractStudentService has UserId that links to BillingMaintenanceService User
    try {
      console.log('Method 1: Fetching student by userId:', currentUser.id)
      const studentResponse = await axios.get(`${API_URLS.STUDENTS}/by-user/${currentUser.id}`, {
        headers: getAuthHeaders()
      })
      studentId = studentResponse.data.id
      console.log('✓ Student found in ContractStudentService:', studentResponse.data)
      console.log('✓ Student ID:', studentId)
      console.log('✓ Student Code:', studentResponse.data.studentCode)
    } catch (err) {
      console.warn('Method 1 failed:', err.response?.status, err.response?.data)
      
      // Method 2: Try by StudentCode (if it looks like a real student code)
      if (currentUser.studentCode && /^SV\d+$/.test(currentUser.studentCode)) {
        try {
          console.log('Method 2: Trying by studentCode:', currentUser.studentCode)
          const studentResponse = await axios.get(`${API_URLS.STUDENTS}/code/${currentUser.studentCode}`, {
            headers: getAuthHeaders()
          })
          studentId = studentResponse.data.id
          console.log('✓ Student ID from ContractStudentService (by code):', studentId)
        } catch (err2) {
          console.warn('Method 2 failed:', err2.response?.status)
        }
      }
    }
    
    if (!studentId) {
      console.error('❌ Could not determine student ID')
      message.warning('Không tìm thấy thông tin sinh viên. Vui lòng liên hệ quản trị viên.')
      invoices.value = []
      loading.value = false
      return
    }
    
    console.log('===== Final Student ID:', studentId, '=====')
    
    // Call Invoice API with student ID
    console.log('Fetching invoices for student ID:', studentId)
    const response = await axios.get(`${API_URLS.INVOICES}/student/${studentId}`, {
      headers: getAuthHeaders()
    })
    
    console.log('✓ Invoices loaded:', response.data.length, 'invoices')
    invoices.value = response.data || []
    
    if (invoices.value.length === 0) {
      message.info('Bạn chưa có phiếu thu nào')
    } else {
      message.success(`Đã tải ${invoices.value.length} phiếu thu`)
    }
    console.log('=== FETCH INVOICES END ===')
  } catch (error) {
    console.error('Error loading invoices:', error)
    console.error('Error response:', error.response?.data)
    console.error('Error status:', error.response?.status)
    
    if (error.response?.status === 404) {
      message.info('Bạn chưa có phiếu thu nào')
      invoices.value = []
    } else {
      const errorMsg = error.response?.data?.message || error.message || 'Lỗi tải dữ liệu'
      message.error(errorMsg)
      invoices.value = []
    }
  } finally {
    loading.value = false
  }
}

const viewInvoiceDetail = async (record) => {
  try {
    const response = await axios.get(`${API_URLS.INVOICES}/${record.id}`, {
      headers: getAuthHeaders()
    })
    detailTarget.value = response.data
    detailDialog.value = true
  } catch (error) {
    console.error('Error loading invoice detail:', error)
    message.error('Không thể tải chi tiết phiếu thu')
  }
}

const openQRPayment = async (record) => {
  selectedInvoice.value = record
  qrDialog.value = true
  qrLoading.value = true
  checkCount.value = 0
  
  try {
    // Generate QR Code using VietQR API
    // Bank: Sacombank (970403), Account: 025202102005, Owner: TRAN KHAC HONG
    const amount = record.debtAmount
    const description = `${record.invoiceCode} ${record.studentCode}`
    
    // VietQR API format
    const bankCode = '970403' // Sacombank bank code
    const accountNo = '025202102005'
    const accountName = 'TRAN KHAC HONG'
    const template = 'compact2' // or 'compact', 'qr_only', 'print'
    
    // Using VietQR API
    qrCodeUrl.value = `https://img.vietqr.io/image/${bankCode}-${accountNo}-${template}.png?amount=${amount}&addInfo=${encodeURIComponent(description)}&accountName=${encodeURIComponent(accountName)}`
    
    console.log('QR Code URL:', qrCodeUrl.value)
    message.success('Đã tạo mã QR thanh toán')
    
    // Start auto-check interval (every 5 seconds, max 60 times = 5 minutes)
    startAutoCheckPayment()
  } catch (error) {
    console.error('Error generating QR code:', error)
    message.error('Không thể tạo mã QR. Vui lòng thử lại.')
  } finally {
    qrLoading.value = false
  }
}

const startAutoCheckPayment = () => {
  // Clear any existing interval
  stopAutoCheckPayment()
  
  console.log('🔄 Starting auto-check payment every 5 seconds...')
  
  autoCheckInterval.value = setInterval(async () => {
    checkCount.value++
    console.log(`⏱️ Auto-check #${checkCount.value}`)
    
    // Stop after 60 checks (5 minutes)
    if (checkCount.value >= 60) {
      console.log('⏹️ Stopping auto-check after 5 minutes')
      stopAutoCheckPayment()
      return
    }
    
    await checkPaymentStatusSilent()
  }, 5000) // Check every 5 seconds
}

const stopAutoCheckPayment = () => {
  if (autoCheckInterval.value) {
    clearInterval(autoCheckInterval.value)
    autoCheckInterval.value = null
    console.log('⏹️ Auto-check payment stopped')
  }
}

const checkPaymentStatusSilent = async () => {
  if (!selectedInvoice.value) return
  
  try {
    // Call check payment API
    const response = await axios.post(`${API_URLS.SEPAY}/check-payment`, {
      invoiceCode: selectedInvoice.value.invoiceCode
    }, {
      headers: getAuthHeaders()
    })
    
    const result = response.data
    
    if (result.status === 'Paid') {
      // Stop auto-check
      stopAutoCheckPayment()
      
      // Show success notification
      message.success({
        content: '🎉 Thanh toán thành công! Đã thanh toán đầy đủ.',
        duration: 5
      })
      
      // Close modal after 2 seconds
      setTimeout(() => {
        qrDialog.value = false
      }, 2000)
      
      // Reload all invoices
      await fetchInvoices()
      
      console.log('✅ Payment successful!')
    } else if (result.status === 'PartialPaid') {
      // Stop auto-check
      stopAutoCheckPayment()
      
      // Show partial payment notification
      message.info({
        content: `💰 Đã nhận được thanh toán một phần. Còn lại: ${formatCurrency(result.debtAmount)}`,
        duration: 5
      })
      
      // Update invoice data
      selectedInvoice.value.paidAmount = result.paidAmount
      selectedInvoice.value.debtAmount = result.debtAmount
      selectedInvoice.value.status = result.status
      
      await fetchInvoices()
      
      console.log('⚠️ Partial payment received')
    }
    // If still unpaid, continue checking silently
  } catch (error) {
    console.error('Error in auto-check payment:', error)
    // Don't show error message for auto-check to avoid spamming user
  }
}

const checkPaymentStatus = async () => {
  if (!selectedInvoice.value) return
  
  qrLoading.value = true
  try {
    // Call check payment API
    const response = await axios.post(`${API_URLS.SEPAY}/check-payment`, {
      invoiceCode: selectedInvoice.value.invoiceCode
    }, {
      headers: getAuthHeaders()
    })
    
    console.log('Payment check response:', response.data)
    
    const result = response.data
    
    if (result.status === 'Paid') {
      stopAutoCheckPayment()
      message.success('✅ Thanh toán thành công! Đã thanh toán đầy đủ.')
      qrDialog.value = false
      await fetchInvoices() // Reload all invoices
    } else if (result.status === 'PartialPaid') {
      stopAutoCheckPayment()
      message.info(`Đã nhận được thanh toán một phần. Còn lại: ${formatCurrency(result.debtAmount)}`)
      selectedInvoice.value.paidAmount = result.paidAmount
      selectedInvoice.value.debtAmount = result.debtAmount
      selectedInvoice.value.status = result.status
      await fetchInvoices()
    } else {
      message.warning('⏳ Chưa nhận được thanh toán. Vui lòng kiểm tra lại sau ít phút.')
    }
  } catch (error) {
    console.error('Error checking payment status:', error)
    message.error('Không thể kiểm tra trạng thái thanh toán')
  } finally {
    qrLoading.value = false
  }
}

onMounted(() => {
  fetchInvoices()
})

onUnmounted(() => {
  stopAutoCheckPayment()
})
</script>

<style scoped>
:deep(.ant-statistic-content) {
  font-size: 24px;
}
</style>
