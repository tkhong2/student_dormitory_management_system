<template>
  <div>
    <PageHeaderAnt title="Ghi nhận thanh toán" subtitle="Thu tiền và xác nhận thanh toán từ sinh viên">
      <template #extra>
        <a-button @click="handleExport" :loading="exporting" style="margin-right: 16px;">
          <template #icon><DownloadOutlined /></template>
          Xuất Excel
        </a-button>
        <a-statistic 
          title="Tổng sinh viên có nợ" 
          :value="studentsWithDebt.length" 
          suffix="SV"
          style="margin-right: 24px;"
        />
        <a-statistic 
          title="Tổng công nợ" 
          :value="totalSystemDebt" 
          :precision="0"
          suffix="VNĐ"
          :valueStyle="{ color: '#ff4d4f' }"
        />
      </template>
    </PageHeaderAnt>

    <!-- Search & Filters -->
    <a-card :bordered="false" style="margin-bottom: 16px;">
      <a-row :gutter="16" align="middle">
        <a-col :xs="24" :md="12">
          <a-input-search
            v-model:value="searchQuery"
            size="large"
            placeholder="Tìm sinh viên: Mã SV, Tên, Số phòng, SĐT..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :md="6">
          <a-select
            v-model:value="filterStatus"
            size="large"
            style="width: 100%"
            placeholder="Lọc theo trạng thái"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option value="overdue">Quá hạn</a-select-option>
            <a-select-option value="duesoon">Sắp đến hạn</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :md="6">
          <a-select
            v-model:value="sortBy"
            size="large"
            style="width: 100%"
            placeholder="Sắp xếp"
          >
            <a-select-option value="debt_desc">Nợ nhiều → ít</a-select-option>
            <a-select-option value="debt_asc">Nợ ít → nhiều</a-select-option>
            <a-select-option value="name_asc">Tên A → Z</a-select-option>
            <a-select-option value="room_asc">Phòng A → Z</a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Students with Debt List -->
    <a-card :bordered="false" title="Danh sách sinh viên có nợ">
      <a-table
        :columns="studentColumns"
        :data-source="filteredStudents"
        :loading="loadingStudents"
        :pagination="pagination"
        :row-key="record => record.id"
        size="middle"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <div><strong>{{ record.fullName }}</strong></div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'room'">
            <a-tag v-if="record.roomNumber" color="blue">{{ record.roomNumber }}</a-tag>
            <span v-else style="color: #bfbfbf;">Chưa có</span>
          </template>

          <template v-else-if="column.key === 'contact'">
            <div style="font-size: 12px;">
              <div>📱 {{ record.phone }}</div>
              <div style="color: #8c8c8c;">✉️ {{ record.email }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'debt'">
            <div>
              <div style="font-size: 16px; font-weight: 700; color: #ff4d4f;">
                {{ formatCurrency(record.totalDebt) }} VNĐ
              </div>
              <div style="font-size: 11px; color: #8c8c8c;">
                {{ record.unpaidCount }} hóa đơn
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag v-if="record.hasOverdue" color="red">
              <WarningOutlined /> Quá hạn
            </a-tag>
            <a-tag v-else-if="record.hasDueSoon" color="orange">
              Sắp hạn
            </a-tag>
            <a-tag v-else color="default">
              Bình thường
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button 
                size="small" 
                type="link"
                @click="viewStudentDetail(record)"
              >
                <template #icon><EyeOutlined /></template>
                Xem chi tiết
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>


    <!-- Student Detail Drawer -->
    <a-drawer
      v-model:open="detailDrawerVisible"
      title="Chi tiết thanh toán"
      width="800"
      placement="right"
    >
      <div v-if="selectedStudent">
        <!-- Student Info Card -->
        <a-card size="small" style="margin-bottom: 16px;">
          <a-descriptions :column="2" size="small" bordered>
            <a-descriptions-item label="Họ tên" :span="2">
              <strong style="font-size: 16px;">{{ selectedStudent.fullName }}</strong>
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
          
          <a-divider style="margin: 12px 0;" />
          
          <a-row :gutter="16">
            <a-col :span="12">
              <a-statistic
                title="Tổng nợ"
                :value="totalDebt"
                :precision="0"
                suffix="VNĐ"
                :valueStyle="{ color: '#ff4d4f', fontWeight: 700 }"
              />
            </a-col>
            <a-col :span="12">
              <a-statistic
                title="Số hóa đơn chưa trả"
                :value="unpaidInvoices.length"
                suffix="HĐ"
                :valueStyle="{ color: '#1890ff', fontWeight: 700 }"
              />
            </a-col>
          </a-row>
        </a-card>

        <!-- Invoices List -->
        <a-card size="small" title="Hóa đơn chưa thanh toán">
          <a-list
            :data-source="unpaidInvoices"
            :loading="loadingInvoices"
          >
            <template #renderItem="{ item }">
              <a-list-item>
                <template #actions>
                  <a-button 
                    type="primary"
                    size="small"
                    :disabled="item.debtAmount <= 0"
                    @click="openPaymentModal(item)"
                  >
                    <template #icon><DollarOutlined /></template>
                    Thu tiền
                  </a-button>
                </template>
                
                <a-list-item-meta>
                  <template #title>
                    <div style="display: flex; align-items: center; gap: 8px;">
                      <strong>{{ item.invoiceCode }}</strong>
                      <a-tag :color="getInvoiceTypeColor(item.type)">
                        {{ getInvoiceTypeText(item.type) }}
                      </a-tag>
                      <a-tag :color="getStatusColor(item.status)">
                        {{ getStatusText(item.status) }}
                      </a-tag>
                      <a-tag v-if="isOverdue(item.dueDate)" color="red">
                        <WarningOutlined /> Quá hạn
                      </a-tag>
                    </div>
                  </template>
                  <template #description>
                    <a-row :gutter="16" style="margin-top: 8px;">
                      <a-col :span="8">
                        <div style="font-size: 12px; color: #8c8c8c;">Tổng tiền</div>
                        <div style="font-weight: 600;">{{ formatCurrency(item.totalAmount) }} VNĐ</div>
                      </a-col>
                      <a-col :span="8">
                        <div style="font-size: 12px; color: #8c8c8c;">Đã trả</div>
                        <div style="color: #52c41a; font-weight: 600;">{{ formatCurrency(item.paidAmount) }} VNĐ</div>
                      </a-col>
                      <a-col :span="8">
                        <div style="font-size: 12px; color: #8c8c8c;">Còn lại</div>
                        <div style="color: #ff4d4f; font-weight: 600;">{{ formatCurrency(item.debtAmount) }} VNĐ</div>
                      </a-col>
                    </a-row>
                    <div style="margin-top: 8px; font-size: 12px; color: #8c8c8c;">
                      Hạn thanh toán: {{ formatDate(item.dueDate) }}
                    </div>
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
          
          <a-empty v-if="!loadingInvoices && unpaidInvoices.length === 0" description="Không có hóa đơn chưa thanh toán" />
        </a-card>
      </div>
    </a-drawer>

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
import { ref, computed, onMounted, watch } from 'vue'
import { message } from 'ant-design-vue'
import {
  DollarOutlined, SearchOutlined, WarningOutlined, PrinterOutlined, EyeOutlined, DownloadOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { studentService } from '@/services/studentService'
import { invoiceService } from '@/services/invoiceService'
import { paymentService } from '@/services/paymentService'
import { useAuthStore } from '@/stores/auth'
import dayjs from 'dayjs'
import { useExcelExport } from '@/composables/useExcelExport'

const { exporting, exportToExcel } = useExcelExport()

const authStore = useAuthStore()
const loadingStudents = ref(false)
const loadingInvoices = ref(false)
const processing = ref(false)
const searchQuery = ref('')
const filterStatus = ref('all')
const sortBy = ref('debt_desc')
const allStudents = ref([])
const studentsWithDebt = ref([])
const selectedStudent = ref(null)
const unpaidInvoices = ref([])
const detailDrawerVisible = ref(false)
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

const pagination = ref({
  current: 1,
  pageSize: 10,
  showSizeChanger: true,
  showTotal: (total) => `Tổng ${total} sinh viên`
})

const studentColumns = [
  { title: 'Sinh viên', key: 'student', width: 200 },
  { title: 'Phòng', key: 'room', width: 100 },
  { title: 'Liên hệ', key: 'contact', width: 200 },
  { title: 'Công nợ', key: 'debt', width: 150 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 120, fixed: 'right' }
]

const totalSystemDebt = computed(() => {
  return studentsWithDebt.value.reduce((sum, s) => sum + s.totalDebt, 0)
})

const totalDebt = computed(() => {
  return unpaidInvoices.value.reduce((sum, inv) => sum + inv.debtAmount, 0)
})

const filteredStudents = computed(() => {
  let result = [...studentsWithDebt.value]
  
  // Search filter
  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(s =>
      s.studentCode?.toLowerCase().includes(query) ||
      s.fullName?.toLowerCase().includes(query) ||
      s.phone?.includes(query) ||
      s.roomNumber?.toLowerCase().includes(query)
    )
  }
  
  // Status filter
  if (filterStatus.value === 'overdue') {
    result = result.filter(s => s.hasOverdue)
  } else if (filterStatus.value === 'duesoon') {
    result = result.filter(s => s.hasDueSoon && !s.hasOverdue)
  }
  
  // Sort
  if (sortBy.value === 'debt_desc') {
    result.sort((a, b) => b.totalDebt - a.totalDebt)
  } else if (sortBy.value === 'debt_asc') {
    result.sort((a, b) => a.totalDebt - b.totalDebt)
  } else if (sortBy.value === 'name_asc') {
    result.sort((a, b) => a.fullName.localeCompare(b.fullName))
  } else if (sortBy.value === 'room_asc') {
    result.sort((a, b) => (a.roomNumber || '').localeCompare(b.roomNumber || ''))
  }
  
  return result
})

onMounted(async () => {
  await loadStudentsWithDebt()
})

const loadStudentsWithDebt = async () => {
  loadingStudents.value = true
  try {
    // Load all students
    allStudents.value = await studentService.getAll()
    
    // Load all invoices
    const allInvoices = await invoiceService.getAll()
    
    // Group invoices by student and calculate debt
    const studentDebtMap = new Map()
    
    for (const invoice of allInvoices) {
      if (invoice.status !== 'Paid' && invoice.debtAmount > 0) {
        if (!studentDebtMap.has(invoice.studentId)) {
          studentDebtMap.set(invoice.studentId, {
            totalDebt: 0,
            unpaidCount: 0,
            invoices: [],
            hasOverdue: false,
            hasDueSoon: false
          })
        }
        
        const studentDebt = studentDebtMap.get(invoice.studentId)
        studentDebt.totalDebt += invoice.debtAmount
        studentDebt.unpaidCount += 1
        studentDebt.invoices.push(invoice)
        
        // Check overdue
        if (isOverdue(invoice.dueDate)) {
          studentDebt.hasOverdue = true
        }
        
        // Check due soon (within 7 days)
        if (isDueSoon(invoice.dueDate)) {
          studentDebt.hasDueSoon = true
        }
      }
    }
    
    // Merge student info with debt info
    studentsWithDebt.value = allStudents.value
      .filter(s => studentDebtMap.has(s.id))
      .map(s => ({
        ...s,
        ...studentDebtMap.get(s.id)
      }))
    
  } catch (error) {
    message.error('Lỗi tải danh sách sinh viên')
    console.error(error)
  } finally {
    loadingStudents.value = false
  }
}

// Export to Excel function
const handleExport = () => {
  const columnMapping = {
    studentCode: 'Mã SV',
    fullName: 'Sinh viên',
    roomNumber: 'Phòng',
    phone: 'Số điện thoại',
    email: 'Email',
    totalDebt: 'Tổng nợ',
    unpaidCount: 'Số hóa đơn chưa thanh toán',
    status: 'Trạng thái'
  }
  
  const dataToExport = filteredStudents.value.map(student => ({
    studentCode: student.studentCode,
    fullName: student.fullName,
    roomNumber: student.roomNumber || 'Chưa có',
    phone: student.phone,
    email: student.email,
    totalDebt: student.totalDebt,
    unpaidCount: student.unpaidCount,
    status: student.hasOverdue ? 'Quá hạn' : (student.hasDueSoon ? 'Sắp đến hạn' : 'Bình thường')
  }))
  
  exportToExcel(dataToExport, columnMapping, 'Danh_sach_cong_no', 'Công nợ')
}

const viewStudentDetail = async (student) => {
  selectedStudent.value = student
  detailDrawerVisible.value = true
  await loadUnpaidInvoices(student.id)
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
    
    // Reload data
    await loadUnpaidInvoices(selectedStudent.value.id)
    await loadStudentsWithDebt()
  } catch (error) {
    message.error(error.message || 'Lỗi ghi nhận thanh toán')
  } finally {
    processing.value = false
  }
}

const printReceipt = () => {
  window.print()
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

const isDueSoon = (dueDate) => {
  const daysUntilDue = dayjs(dueDate).diff(dayjs(), 'day')
  return daysUntilDue >= 0 && daysUntilDue <= 7
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
