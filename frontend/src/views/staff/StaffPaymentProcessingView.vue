<template>
  <div>
    <PageHeaderAnt title="Ghi nhận thanh toán" subtitle="Thu tiền và xác nhận thanh toán từ sinh viên">
      <template #actions>
        <a-button type="primary" @click="showQuickPaymentModal">
          <template #icon><DollarOutlined /></template>
          Thu tiền nhanh
        </a-button>
      </template>
    </PageHeaderAnt>

    <!-- Search Student -->
    <a-card :bordered="false" style="margin-bottom: 16px;">
      <a-row :gutter="16">
        <a-col :xs="24" :md="12">
          <a-input-search
            v-model:value="searchQuery"
            size="large"
            placeholder="Tìm sinh viên: Mã SV, Tên, Số phòng, SĐT..."
            @search="searchStudent"
            :loading="searching"
          >
            <template #enterButton>
              <a-button type="primary">
                <SearchOutlined /> Tìm kiếm
              </a-button>
            </template>
          </a-input-search>
        </a-col>
      </a-row>
    </a-card>

    <!-- Student Info & Invoices -->
    <a-row v-if="selectedStudent" :gutter="[16, 16]">
      <!-- Student Card -->
      <a-col :xs="24" :md="8">
        <a-card title="Thông tin sinh viên" :bordered="false">
          <a-descriptions :column="1" size="small">
            <a-descriptions-item label="Họ tên">
              <strong>{{ selectedStudent.fullName }}</strong>
            </a-descriptions-item>
            <a-descriptions-item label="Mã SV">
              {{ selectedStudent.studentCode }}
            </a-descriptions-item>
            <a-descriptions-item label="Phòng">
              <a-tag color="blue">{{ selectedStudent.roomNumber || 'Chưa có' }}</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="SĐT">
              {{ selectedStudent.phone }}
            </a-descriptions-item>
            <a-descriptions-item label="Email">
              {{ selectedStudent.email }}
            </a-descriptions-item>
          </a-descriptions>
          
          <a-divider />
          
          <a-statistic
            title="Tổng nợ"
            :value="totalDebt"
            :precision="0"
            suffix="VNĐ"
            :valueStyle="{ color: totalDebt > 0 ? '#ff4d4f' : '#52c41a', fontWeight: 700 }"
          />
        </a-card>
      </a-col>

      <!-- Unpaid Invoices -->
      <a-col :xs="24" :md="16">
        <a-card title="Hóa đơn chưa thanh toán" :bordered="false">
          <a-table
            :columns="invoiceColumns"
            :data-source="unpaidInvoices"
            :loading="loadingInvoices"
            :pagination="false"
            :row-key="record => record.id"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'invoiceCode'">
                <strong>{{ record.invoiceCode }}</strong>
              </template>

              <template v-else-if="column.key === 'type'">
                <a-tag :color="getInvoiceTypeColor(record.type)">
                  {{ getInvoiceTypeText(record.type) }}
                </a-tag>
              </template>

              <template v-else-if="column.key === 'amount'">
                <div>
                  <div>Tổng: <strong>{{ formatCurrency(record.totalAmount) }}</strong></div>
                  <div style="font-size: 11px; color: #52c41a;">Đã trả: {{ formatCurrency(record.paidAmount) }}</div>
                  <div style="font-size: 11px; color: #ff4d4f;">Còn: {{ formatCurrency(record.debtAmount) }}</div>
                </div>
              </template>

              <template v-else-if="column.key === 'status'">
                <a-tag :color="getStatusColor(record.status)">
                  {{ getStatusText(record.status) }}
                </a-tag>
              </template>

              <template v-else-if="column.key === 'dueDate'">
                <div :style="{ color: isOverdue(record.dueDate) ? '#ff4d4f' : undefined }">
                  {{ formatDate(record.dueDate) }}
                  <div v-if="isOverdue(record.dueDate)" style="font-size: 11px;">
                    <WarningOutlined /> Quá hạn
                  </div>
                </div>
              </template>

              <template v-else-if="column.key === 'actions'">
                <a-button 
                  size="small" 
                  type="primary"
                  :disabled="record.debtAmount <= 0"
                  @click="openPaymentModal(record)"
                >
                  <template #icon><DollarOutlined /></template>
                  Thu tiền
                </a-button>
              </template>
            </template>
          </a-table>

          <a-empty v-if="!loadingInvoices && unpaidInvoices.length === 0" description="Không có hóa đơn chưa thanh toán" />
        </a-card>
      </a-col>
    </a-row>

    <!-- Empty State -->
    <a-card v-else :bordered="false">
      <a-empty description="Tìm kiếm sinh viên để xem hóa đơn và thu tiền">
        <template #image>
          <SearchOutlined style="font-size: 64px; color: #d9d9d9;" />
        </template>
      </a-empty>
    </a-card>

    <!-- Payment Modal -->
    <a-modal
      v-model:open="paymentModalVisible"
      title="Ghi nhận thanh toán"
      width="600px"
      @ok="handlePayment"
      :confirmLoading="processing"
      okText="Xác nhận thu tiền"
    >
      <a-form v-if="selectedInvoice" layout="vertical">
        <a-alert
          message="Thông tin hóa đơn"
          :description="`${selectedInvoice.invoiceCode} - ${getInvoiceTypeText(selectedInvoice.type)}`"
          type="info"
          show-icon
          style="margin-bottom: 16px;"
        />

        <a-descriptions bordered size="small" :column="2" style="margin-bottom: 16px;">
          <a-descriptions-item label="Tổng tiền">
            {{ formatCurrency(selectedInvoice.totalAmount) }} VNĐ
          </a-descriptions-item>
          <a-descriptions-item label="Đã trả">
            {{ formatCurrency(selectedInvoice.paidAmount) }} VNĐ
          </a-descriptions-item>
          <a-descriptions-item label="Còn lại" :span="2">
            <strong style="color: #ff4d4f; font-size: 16px;">
              {{ formatCurrency(selectedInvoice.debtAmount) }} VNĐ
            </strong>
          </a-descriptions-item>
        </a-descriptions>

        <a-form-item label="Số tiền thu *" required>
          <a-input-number
            v-model:value="paymentForm.amount"
            :min="1"
            :max="selectedInvoice.debtAmount"
            :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
            style="width: 100%"
            size="large"
          >
            <template #addonAfter>VNĐ</template>
          </a-input-number>
          <div style="margin-top: 8px;">
            <a-button size="small" @click="paymentForm.amount = selectedInvoice.debtAmount">
              Trả hết
            </a-button>
          </div>
        </a-form-item>

        <a-form-item label="Phương thức thanh toán *" required>
          <a-radio-group v-model:value="paymentForm.method" button-style="solid">
            <a-radio-button value="Cash">Tiền mặt</a-radio-button>
            <a-radio-button value="BankTransfer">Chuyển khoản</a-radio-button>
            <a-radio-button value="MoMo">MoMo</a-radio-button>
            <a-radio-button value="ZaloPay">ZaloPay</a-radio-button>
          </a-radio-group>
        </a-form-item>

        <a-form-item v-if="paymentForm.method === 'BankTransfer'" label="Mã giao dịch">
          <a-input v-model:value="paymentForm.transactionCode" placeholder="Nhập mã giao dịch ngân hàng" />
        </a-form-item>

        <a-form-item v-if="paymentForm.method === 'BankTransfer'" label="Ngân hàng">
          <a-select v-model:value="paymentForm.bankName" placeholder="Chọn ngân hàng">
            <a-select-option value="Vietcombank">Vietcombank</a-select-option>
            <a-select-option value="VietinBank">VietinBank</a-select-option>
            <a-select-option value="BIDV">BIDV</a-select-option>
            <a-select-option value="Agribank">Agribank</a-select-option>
            <a-select-option value="Techcombank">Techcombank</a-select-option>
            <a-select-option value="MB">MB Bank</a-select-option>
            <a-select-option value="Other">Khác</a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="paymentForm.note" :rows="2" placeholder="Ghi chú thêm (không bắt buộc)" />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Receipt Modal -->
    <a-modal
      v-model:open="receiptModalVisible"
      title="Biên lai thanh toán"
      width="600px"
      :footer="null"
    >
      <div id="receipt-content" style="padding: 24px; background: white;">
        <div style="text-align: center; margin-bottom: 24px;">
          <h2 style="margin: 0;">BIÊN LAI THU TIỀN</h2>
          <p style="margin: 8px 0 0 0;">KÝ TÚC XÁ TRƯỜNG ĐH ABC</p>
        </div>

        <a-descriptions v-if="lastPayment" bordered :column="1" size="small">
          <a-descriptions-item label="Mã biên lai">
            <strong>{{ lastPayment.id }}</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Ngày thu">
            {{ formatDateTime(lastPayment.paidAt) }}
          </a-descriptions-item>
          <a-descriptions-item label="Sinh viên">
            {{ selectedStudent?.fullName }} ({{ selectedStudent?.studentCode }})
          </a-descriptions-item>
          <a-descriptions-item label="Hóa đơn">
            {{ selectedInvoice?.invoiceCode }}
          </a-descriptions-item>
          <a-descriptions-item label="Số tiền">
            <strong style="font-size: 18px; color: #52c41a;">
              {{ formatCurrency(lastPayment.amount) }} VNĐ
            </strong>
          </a-descriptions-item>
          <a-descriptions-item label="Phương thức">
            {{ getPaymentMethodText(lastPayment.method) }}
          </a-descriptions-item>
          <a-descriptions-item label="Người thu">
            {{ lastPayment.receivedByName }}
          </a-descriptions-item>
        </a-descriptions>

        <div style="margin-top: 32px; text-align: right;">
          <p style="margin: 0;">Người thu tiền</p>
          <p style="margin: 32px 0 0 0; font-style: italic;">(Ký và ghi rõ họ tên)</p>
        </div>
      </div>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="receiptModalVisible = false">Đóng</a-button>
        <a-button type="primary" @click="printReceipt">
          <template #icon><PrinterOutlined /></template>
          In biên lai
        </a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { message } from 'ant-design-vue'
import {
  DollarOutlined, SearchOutlined, WarningOutlined, PrinterOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { studentService } from '@/services/studentService'
import { invoiceService } from '@/services/invoiceService'
import { paymentService } from '@/services/paymentService'
import { useAuthStore } from '@/stores/auth'
import dayjs from 'dayjs'

const authStore = useAuthStore()
const searching = ref(false)
const loadingInvoices = ref(false)
const processing = ref(false)
const searchQuery = ref('')
const selectedStudent = ref(null)
const unpaidInvoices = ref([])
const paymentModalVisible = ref(false)
const receiptModalVisible = ref(false)
const selectedInvoice = ref(null)
const lastPayment = ref(null)

const paymentForm = ref({
  amount: 0,
  method: 'Cash',
  transactionCode: '',
  bankName: '',
  note: ''
})

const invoiceColumns = [
  { title: 'Mã HĐ', key: 'invoiceCode', width: 120 },
  { title: 'Loại', key: 'type', width: 120 },
  { title: 'Số tiền', key: 'amount', width: 180 },
  { title: 'Hạn', key: 'dueDate', width: 120 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 120 }
]

const totalDebt = computed(() => {
  return unpaidInvoices.value.reduce((sum, inv) => sum + inv.debtAmount, 0)
})

const searchStudent = async () => {
  if (!searchQuery.value.trim()) {
    message.warning('Vui lòng nhập thông tin tìm kiếm')
    return
  }

  searching.value = true
  try {
    // Search students - giả sử có API search
    const students = await studentService.getAll()
    const query = searchQuery.value.toLowerCase()
    const found = students.find(s =>
      s.studentCode?.toLowerCase().includes(query) ||
      s.fullName?.toLowerCase().includes(query) ||
      s.phone?.includes(query)
    )

    if (!found) {
      message.warning('Không tìm thấy sinh viên')
      selectedStudent.value = null
      unpaidInvoices.value = []
      return
    }

    selectedStudent.value = found
    await loadUnpaidInvoices(found.id)
  } catch (error) {
    message.error('Lỗi tìm kiếm sinh viên')
    console.error(error)
  } finally {
    searching.value = false
  }
}

const loadUnpaidInvoices = async (studentId) => {
  loadingInvoices.value = true
  try {
    const allInvoices = await invoiceService.getByStudentId(studentId)
    unpaidInvoices.value = allInvoices.filter(inv => inv.status !== 'Paid')
  } catch (error) {
    message.error('Lỗi tải hóa đơn')
    console.error(error)
  } finally {
    loadingInvoices.value = false
  }
}

const openPaymentModal = (invoice) => {
  selectedInvoice.value = invoice
  paymentForm.value = {
    amount: invoice.debtAmount,
    method: 'Cash',
    transactionCode: '',
    bankName: '',
    note: ''
  }
  paymentModalVisible.value = true
}

const handlePayment = async () => {
  if (!paymentForm.value.amount || paymentForm.value.amount <= 0) {
    message.warning('Vui lòng nhập số tiền')
    return
  }

  processing.value = true
  try {
    const payment = await paymentService.create({
      invoiceId: selectedInvoice.value.id,
      amount: paymentForm.value.amount,
      method: paymentForm.value.method,
      transactionCode: paymentForm.value.transactionCode,
      bankName: paymentForm.value.bankName,
      paymentDate: new Date().toISOString(),
      receivedByUserId: authStore.user.id,
      receivedByName: authStore.user.fullName,
      note: paymentForm.value.note
    })

    lastPayment.value = payment
    message.success('Ghi nhận thanh toán thành công!')
    
    paymentModalVisible.value = false
    receiptModalVisible.value = true
    
    // Reload invoices
    await loadUnpaidInvoices(selectedStudent.value.id)
  } catch (error) {
    message.error(error.message || 'Lỗi ghi nhận thanh toán')
  } finally {
    processing.value = false
  }
}

const printReceipt = () => {
  window.print()
}

const showQuickPaymentModal = () => {
  message.info('Vui lòng tìm sinh viên trước')
}

const getInvoiceTypeColor = (type) => {
  const colors = {
    Rent: 'blue',
    Deposit: 'orange',
    Service: 'purple',
    Other: 'default'
  }
  return colors[type] || 'default'
}

const getInvoiceTypeText = (type) => {
  const texts = {
    Rent: 'Tiền phòng',
    Deposit: 'Tiền cọc',
    Service: 'Dịch vụ',
    Other: 'Khác'
  }
  return texts[type] || type
}

const getStatusColor = (status) => {
  const colors = {
    Unpaid: 'red',
    PartialPaid: 'orange',
    Paid: 'green'
  }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    Unpaid: 'Chưa trả',
    PartialPaid: 'Trả 1 phần',
    Paid: 'Đã trả'
  }
  return texts[status] || status
}

const getPaymentMethodText = (method) => {
  const texts = {
    Cash: 'Tiền mặt',
    BankTransfer: 'Chuyển khoản',
    MoMo: 'Ví MoMo',
    ZaloPay: 'Ví ZaloPay'
  }
  return texts[method] || method
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

const formatDateTime = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY HH:mm') : ''
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const isOverdue = (dueDate) => {
  return dayjs(dueDate).isBefore(dayjs(), 'day')
}
</script>

<style scoped>
@media print {
  body * {
    visibility: hidden;
  }
  #receipt-content, #receipt-content * {
    visibility: visible;
  }
  #receipt-content {
    position: absolute;
    left: 0;
    top: 0;
  }
}
</style>
