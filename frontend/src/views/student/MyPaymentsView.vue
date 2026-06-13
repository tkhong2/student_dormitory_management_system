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
        <a-card :bordered="false" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%)">
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
                @click="viewInvoiceDetail(record)"
              >
                <template #icon><CreditCardOutlined /></template>
                Thanh toán
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

    <!-- Invoice Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết Phiếu Thu"
      width="800px"
      :footer="null"
    >
      <a-descriptions v-if="detailTarget" bordered :column="2" size="small">
        <a-descriptions-item label="Mã Phiếu Thu" :span="2">
          <a-typography-text strong copyable>{{ detailTarget.invoiceCode }}</a-typography-text>
        </a-descriptions-item>
        
        <a-descriptions-item label="Kỳ thanh toán">
          Tháng {{ detailTarget.billingMonth }}/{{ detailTarget.billingYear }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          <a-tag color="blue">{{ detailTarget.roomNumber }}</a-tag> {{ detailTarget.buildingName }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Trạng thái" :span="2">
          <a-tag :color="getStatusColor(detailTarget.status)">
            {{ getStatusText(detailTarget.status) }}
          </a-tag>
          <span v-if="detailTarget.overdueDays > 0" style="margin-left: 8px; color: #ff4d4f;">
            (Quá hạn {{ detailTarget.overdueDays }} ngày)
          </span>
        </a-descriptions-item>
        
        <a-descriptions-item label="Chi tiết các khoản" :span="2">
          <a-list size="small" :data-source="detailTarget.items || getDefaultItems(detailTarget)" bordered>
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>{{ item.itemName }}</template>
                  <template #description>{{ item.itemDescription }}</template>
                </a-list-item-meta>
                <div style="font-weight: 500">{{ formatCurrency(item.amount) }}</div>
              </a-list-item>
            </template>
          </a-list>
        </a-descriptions-item>
        
        <a-descriptions-item label="Hạn thanh toán">
          {{ formatDate(detailTarget.dueDate) }}
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
      </a-descriptions>

      <a-alert
        v-if="detailTarget?.status !== 'Paid'"
        message="Hướng dẫn thanh toán"
        type="info"
        show-icon
        style="margin-top: 16px"
      >
        <template #description>
          <p>Vui lòng chuyển khoản đến:</p>
          <p><strong>Ngân hàng:</strong> Vietcombank - Chi nhánh Đà Nẵng</p>
          <p><strong>Số tài khoản:</strong> 0123456789</p>
          <p><strong>Chủ tài khoản:</strong> KTX Đại học DNU</p>
          <p><strong>Nội dung:</strong> {{ detailTarget.invoiceCode }} {{ detailTarget.studentCode }}</p>
        </template>
      </a-alert>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end;">
        <a-button @click="detailDialog = false">Đóng</a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { CreditCardOutlined, EyeOutlined } from '@ant-design/icons-vue'
import axios from 'axios'

const loading = ref(false)
const invoices = ref([])
const detailDialog = ref(false)
const detailTarget = ref(null)

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
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    console.log('User info:', user)
    
    if (user.studentId) {
      // Call real API
      const response = await axios.get(`http://localhost:5002/api/invoices/student/${user.studentId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      })
      
      console.log('Invoices loaded from API:', response.data)
      invoices.value = response.data || []
      
      if (invoices.value.length === 0) {
        message.info('Bạn chưa có phiếu thu nào')
      }
    } else {
      message.warning('Không tìm thấy thông tin sinh viên. Vui lòng đăng nhập lại.')
      invoices.value = []
    }
  } catch (error) {
    console.error('Error loading invoices:', error)
    const errorMsg = error.response?.data?.message || error.message || 'Lỗi tải dữ liệu'
    message.error(errorMsg)
    invoices.value = []
  } finally {
    loading.value = false
  }
}

const viewInvoiceDetail = async (record) => {
  try {
    const response = await axios.get(`http://localhost:5002/api/invoices/${record.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    detailTarget.value = response.data
    detailDialog.value = true
  } catch (error) {
    console.error('Error loading invoice detail:', error)
    message.error('Không thể tải chi tiết phiếu thu')
  }
}

onMounted(() => {
  fetchInvoices()
})
</script>

<style scoped>
:deep(.ant-statistic-content) {
  font-size: 24px;
}
</style>
