<template>
  <div>
    <PageHeaderAnt title="Dashboard Nhân viên" subtitle="Tổng quan công việc hôm nay">
      <template #actions>
        <a-space>
          <a-button @click="refreshData">
            <template #icon><ReloadOutlined /></template>
            Làm mới
          </a-button>
          <a-button type="primary">
            <template #icon><DownloadOutlined /></template>
            Xuất báo cáo
          </a-button>
        </a-space>
      </template>
    </PageHeaderAnt>

    <!-- Stats Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :bordered="false" :loading="loading">
          <a-statistic 
            title="Đơn đăng ký chờ duyệt"
            :value="stats.pendingApplications"
            :valueStyle="{ color: '#ff6b00', fontWeight: 700 }"
          >
            <template #prefix><FileTextOutlined /></template>
          </a-statistic>
          <div style="margin-top: 8px; font-size: 12px; color: #8c8c8c;">
            <ClockCircleOutlined /> {{ stats.applicationsOver48h }} đơn quá 48h
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :bordered="false" :loading="loading">
          <a-statistic 
            title="Yêu cầu bảo trì"
            :value="stats.maintenanceRequests"
            :valueStyle="{ color: '#f5222d', fontWeight: 700 }"
          >
            <template #prefix><ToolOutlined /></template>
          </a-statistic>
          <div style="margin-top: 8px; font-size: 12px; color: #8c8c8c;">
            <WarningOutlined style="color: #ff4d4f" /> {{ stats.urgentMaintenance }} khẩn cấp
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :bordered="false" :loading="loading">
          <a-statistic 
            title="Thanh toán hôm nay"
            :value="stats.todayPayments"
            :precision="0"
            suffix="VNĐ"
            :valueStyle="{ color: '#52c41a', fontWeight: 700 }"
          >
            <template #prefix><DollarOutlined /></template>
          </a-statistic>
          <div style="margin-top: 8px; font-size: 12px; color: #8c8c8c;">
            {{ stats.paymentCount }} giao dịch
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :sm="12" :lg="6">
        <a-card :bordered="false" :loading="loading">
          <a-statistic 
            title="Sinh viên nợ quá hạn"
            :value="stats.overdueStudents"
            :valueStyle="{ color: '#faad14', fontWeight: 700 }"
          >
            <template #prefix><ExclamationCircleOutlined /></template>
          </a-statistic>
          <div style="margin-top: 8px; font-size: 12px; color: #8c8c8c;">
            {{ formatCurrency(stats.totalDebt) }} VNĐ
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Alerts & Tasks -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col :xs="24" :lg="16">
        <a-card title="🚨 Cảnh báo ưu tiên" :bordered="false">
          <a-list 
            :data-source="alerts"
            :loading="loading"
          >
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-badge :status="item.status" />
                  </template>
                  <template #title>
                    <span :style="{ color: item.urgent ? '#ff4d4f' : undefined }">
                      {{ item.title }}
                    </span>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-button size="small" type="link" @click="handleAlert(item)">
                    Xử lý
                  </a-button>
                </template>
              </a-list-item>
            </template>
          </a-list>
        </a-card>
      </a-col>

      <a-col :xs="24" :lg="8">
        <a-card title="📊 Tỷ lệ lấp đầy" :bordered="false">
          <a-progress 
            type="dashboard" 
            :percent="stats.occupancyRate" 
            :strokeColor="{ '0%': '#108ee9', '100%': '#87d068' }"
          />
          <a-divider />
          <a-space direction="vertical" style="width: 100%;">
            <div style="display: flex; justify-content: space-between;">
              <span>Phòng đang ở:</span>
              <strong>{{ stats.occupiedRooms }}/{{ stats.totalRooms }}</strong>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span>Phòng trống:</span>
              <strong>{{ stats.availableRooms }}</strong>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span>Phòng bảo trì:</span>
              <strong style="color: #faad14;">{{ stats.maintenanceRooms }}</strong>
            </div>
          </a-space>
        </a-card>
      </a-col>
    </a-row>

    <!-- Quick Actions -->
    <a-card title="⚡ Thao tác nhanh" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-button 
            block 
            size="large" 
            style="height: 100px;"
            @click="$router.push('/staff/room-applications')"
          >
            <div style="display: flex; flex-direction: column; align-items: center; gap: 8px;">
              <FileTextOutlined style="font-size: 32px;" />
              <span style="font-weight: 600;">Duyệt đơn đăng ký</span>
            </div>
          </a-button>
        </a-col>

        <a-col :xs="24" :sm="12" :md="6">
          <a-button 
            block 
            size="large" 
            style="height: 100px;"
            @click="showPaymentModal"
          >
            <div style="display: flex; flex-direction: column; align-items: center; gap: 8px;">
              <DollarOutlined style="font-size: 32px;" />
              <span style="font-weight: 600;">Thu tiền nhanh</span>
            </div>
          </a-button>
        </a-col>

        <a-col :xs="24" :sm="12" :md="6">
          <a-button 
            block 
            size="large" 
            style="height: 100px;"
            @click="$router.push('/staff/maintenance-requests')"
          >
            <div style="display: flex; flex-direction: column; align-items: center; gap: 8px;">
              <ToolOutlined style="font-size: 32px;" />
              <span style="font-weight: 600;">Xử lý bảo trì</span>
            </div>
          </a-button>
        </a-col>

        <a-col :xs="24" :sm="12" :md="6">
          <a-button 
            block 
            size="large" 
            style="height: 100px;"
            @click="showStudentLookup"
          >
            <div style="display: flex; flex-direction: column; align-items: center; gap: 8px;">
              <SearchOutlined style="font-size: 32px;" />
              <span style="font-weight: 600;">Tra cứu sinh viên</span>
            </div>
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Quick Payment Modal -->
    <a-modal
      v-model:open="paymentModalVisible"
      title="💰 Thu tiền nhanh"
      width="700px"
      @ok="handleQuickPayment"
      :confirmLoading="processingPayment"
      okText="Xác nhận thanh toán"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Tìm sinh viên *" required>
          <a-input-search
            v-model:value="paymentForm.studentSearch"
            placeholder="Nhập mã SV hoặc tên sinh viên..."
            @search="searchStudentForPayment"
            :loading="searchingStudent"
          >
            <template #enterButton>
              <a-button type="primary">Tìm</a-button>
            </template>
          </a-input-search>
        </a-form-item>

        <a-alert
          v-if="paymentForm.selectedStudent"
          type="success"
          :message="`Sinh viên: ${paymentForm.selectedStudent.fullName}`"
          style="margin-bottom: 16px;"
        >
          <template #description>
            <div><strong>Mã SV:</strong> {{ paymentForm.selectedStudent.studentCode }}</div>
            <div><strong>Phòng:</strong> {{ paymentForm.selectedStudent.roomNumber || 'Chưa có' }}</div>
            <div><strong>Lớp:</strong> {{ paymentForm.selectedStudent.classCode || 'N/A' }}</div>
          </template>
        </a-alert>

        <a-form-item label="Phiếu thu chưa thanh toán" v-if="unpaidInvoices.length > 0">
          <a-select
            v-model:value="paymentForm.selectedInvoiceId"
            placeholder="Chọn phiếu thu"
            style="width: 100%"
            @change="onInvoiceSelected"
          >
            <a-select-option
              v-for="invoice in unpaidInvoices"
              :key="invoice.id"
              :value="invoice.id"
            >
              {{ invoice.invoiceCode }} - {{ formatCurrency(invoice.debtAmount) }} VNĐ
              ({{ invoice.billingMonth }}/{{ invoice.billingYear }})
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-alert
          v-if="paymentForm.selectedStudent && unpaidInvoices.length === 0"
          type="info"
          message="Sinh viên không có phiếu thu chưa thanh toán"
          style="margin-bottom: 16px;"
        />

        <a-form-item label="Số tiền thanh toán *" required v-if="paymentForm.selectedInvoiceId">
          <a-input-number
            v-model:value="paymentForm.amount"
            :min="0"
            :max="paymentForm.maxAmount"
            style="width: 100%"
            :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
          >
            <template #addonAfter>VNĐ</template>
          </a-input-number>
          <div style="margin-top: 4px; font-size: 12px; color: #8c8c8c;">
            Còn nợ: {{ formatCurrency(paymentForm.maxAmount) }} VNĐ
          </div>
        </a-form-item>

        <a-form-item label="Phương thức thanh toán *" v-if="paymentForm.selectedInvoiceId">
          <a-radio-group v-model:value="paymentForm.paymentMethod">
            <a-radio value="Cash">Tiền mặt</a-radio>
            <a-radio value="BankTransfer">Chuyển khoản</a-radio>
            <a-radio value="Card">Thẻ</a-radio>
          </a-radio-group>
        </a-form-item>

        <a-form-item label="Ghi chú" v-if="paymentForm.selectedInvoiceId">
          <a-textarea
            v-model:value="paymentForm.notes"
            :rows="2"
            placeholder="Ghi chú thêm (nếu có)..."
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Student Lookup Modal -->
    <a-modal
      v-model:open="lookupModalVisible"
      title="🔍 Tra cứu sinh viên"
      width="900px"
      :footer="null"
    >
      <a-input-search
        v-model:value="lookupSearch"
        placeholder="Nhập mã SV, tên, số điện thoại, email..."
        size="large"
        @search="performStudentLookup"
        :loading="lookingUp"
        style="margin-bottom: 16px;"
      >
        <template #enterButton>
          <a-button type="primary" size="large">Tìm kiếm</a-button>
        </template>
      </a-input-search>

      <!-- Search Results -->
      <div v-if="lookupResults.length > 0">
        <a-list :data-source="lookupResults" :pagination="{ pageSize: 5 }">
          <template #renderItem="{ item }">
            <a-list-item>
              <a-list-item-meta>
                <template #avatar>
                  <a-avatar :src="item.avatarUrl" :size="64">
                    {{ item.fullName?.charAt(0) }}
                  </a-avatar>
                </template>
                <template #title>
                  <strong>{{ item.fullName }}</strong>
                  <a-tag :color="item.isActive ? 'green' : 'red'" style="margin-left: 8px;">
                    {{ item.isActive ? 'Đang ở' : 'Đã chuyển đi' }}
                  </a-tag>
                </template>
                <template #description>
                  <a-space direction="vertical" size="small">
                    <div><strong>Mã SV:</strong> {{ item.studentCode }}</div>
                    <div><strong>Email:</strong> {{ item.email }}</div>
                    <div><strong>Điện thoại:</strong> {{ item.phone || 'N/A' }}</div>
                    <div><strong>Lớp:</strong> {{ item.classCode || 'N/A' }}</div>
                    <div><strong>Khoa:</strong> {{ item.faculty || 'N/A' }}</div>
                    <div v-if="item.roomNumber"><strong>Phòng:</strong> {{ item.roomNumber }} - {{ item.buildingName }}</div>
                  </a-space>
                </template>
              </a-list-item-meta>
              <template #actions>
                <a-button size="small" @click="viewStudentDetail(item)">
                  Chi tiết
                </a-button>
              </template>
            </a-list-item>
          </template>
        </a-list>
      </div>

      <a-empty v-else-if="hasSearched && lookupResults.length === 0" description="Không tìm thấy sinh viên" />
    </a-modal>

    <!-- Student Detail Modal -->
    <a-modal
      v-model:open="studentDetailVisible"
      title="Chi tiết sinh viên"
      width="800px"
      :footer="null"
    >
      <a-descriptions v-if="selectedStudentDetail" bordered :column="2" size="small">
        <a-descriptions-item label="Họ tên" :span="2">
          <strong>{{ selectedStudentDetail.fullName }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Mã sinh viên">
          {{ selectedStudentDetail.studentCode }}
        </a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="selectedStudentDetail.isActive ? 'green' : 'red'">
            {{ selectedStudentDetail.isActive ? 'Đang ở' : 'Đã chuyển đi' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Email">
          {{ selectedStudentDetail.email }}
        </a-descriptions-item>
        <a-descriptions-item label="Điện thoại">
          {{ selectedStudentDetail.phone || 'N/A' }}
        </a-descriptions-item>
        <a-descriptions-item label="Lớp">
          {{ selectedStudentDetail.classCode || 'N/A' }}
        </a-descriptions-item>
        <a-descriptions-item label="Khoa">
          {{ selectedStudentDetail.faculty || 'N/A' }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng hiện tại" :span="2">
          <span v-if="selectedStudentDetail.roomNumber">
            {{ selectedStudentDetail.roomNumber }} - {{ selectedStudentDetail.buildingName }}
          </span>
          <span v-else style="color: #8c8c8c;">Chưa có phòng</span>
        </a-descriptions-item>
        <a-descriptions-item label="Hợp đồng" :span="2">
          <div v-if="selectedStudentDetail.contractInfo">
            <div>Từ: {{ formatDate(selectedStudentDetail.contractInfo.startDate) }}</div>
            <div>Đến: {{ formatDate(selectedStudentDetail.contractInfo.endDate) }}</div>
            <div>Trạng thái: <a-tag>{{ selectedStudentDetail.contractInfo.status }}</a-tag></div>
          </div>
          <span v-else style="color: #8c8c8c;">Không có hợp đồng</span>
        </a-descriptions-item>
        <a-descriptions-item label="Tổng nợ" :span="2">
          <strong :style="{ color: selectedStudentDetail.totalDebt > 0 ? '#ff4d4f' : '#52c41a' }">
            {{ formatCurrency(selectedStudentDetail.totalDebt || 0) }} VNĐ
          </strong>
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="studentDetailVisible = false">Đóng</a-button>
        <a-button type="primary" @click="quickPayForStudent(selectedStudentDetail)">
          Thu tiền
        </a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { 
  ReloadOutlined, DownloadOutlined, FileTextOutlined, ToolOutlined, 
  DollarOutlined, ExclamationCircleOutlined, ClockCircleOutlined, 
  WarningOutlined, SearchOutlined 
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { message } from 'ant-design-vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import maintenanceRequestService from '@/services/maintenanceRequestService'
import { invoiceService } from '@/services/invoiceService'
import { paymentService } from '@/services/paymentService'
import { roomService } from '@/services/roomService'
import { contractService } from '@/services/contractService'
import axios from 'axios'
import dayjs from 'dayjs'

const loading = ref(false)

const stats = ref({
  pendingApplications: 0,
  applicationsOver48h: 0,
  maintenanceRequests: 0,
  urgentMaintenance: 0,
  todayPayments: 0,
  paymentCount: 0,
  overdueStudents: 0,
  totalDebt: 0,
  occupancyRate: 0,
  totalRooms: 0,
  occupiedRooms: 0,
  availableRooms: 0,
  maintenanceRooms: 0
})

const alerts = ref([])

// Quick Payment
const paymentModalVisible = ref(false)
const processingPayment = ref(false)
const searchingStudent = ref(false)
const unpaidInvoices = ref([])
const paymentForm = ref({
  studentSearch: '',
  selectedStudent: null,
  selectedInvoiceId: null,
  amount: 0,
  maxAmount: 0,
  paymentMethod: 'Cash',
  notes: ''
})

// Student Lookup
const lookupModalVisible = ref(false)
const studentDetailVisible = ref(false)
const lookingUp = ref(false)
const hasSearched = ref(false)
const lookupSearch = ref('')
const lookupResults = ref([])
const selectedStudentDetail = ref(null)

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const formatDate = (dateStr) => {
  return dateStr ? dayjs(dateStr).format('DD/MM/YYYY') : 'N/A'
}

const refreshData = async () => {
  loading.value = true
  try {
    // Load all data in parallel
    const [applications, maintenanceRequests, invoices, payments, rooms, contracts] = await Promise.all([
      roomApplicationService.getAll(),
      maintenanceRequestService.getAll(),
      invoiceService.getAll(),
      paymentService.getAll(),
      roomService.getAll(),
      contractService.getAll()
    ])

    // Pending applications
    const pendingApps = applications.filter(app => app.status === 'Pending')
    stats.value.pendingApplications = pendingApps.length

    // Applications over 48 hours
    const now = new Date()
    const hours48 = 48 * 60 * 60 * 1000
    stats.value.applicationsOver48h = pendingApps.filter(app => {
      const createdAt = new Date(app.createdAt)
      return (now - createdAt) > hours48
    }).length

    // Maintenance requests (Pending or InProgress)
    const activeMaintenance = maintenanceRequests.filter(mr => 
      mr.status === 'Pending' || mr.status === 'Assigned' || mr.status === 'InProgress'
    )
    stats.value.maintenanceRequests = activeMaintenance.length

    // Urgent maintenance (priority = High or Critical)
    stats.value.urgentMaintenance = activeMaintenance.filter(mr => 
      mr.priority === 'High' || mr.priority === 'Critical' || mr.priority === 'Urgent'
    ).length

    // Today's payments
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    const todayPayments = payments.filter(p => {
      const paymentDate = new Date(p.paidAt || p.paymentDate)
      return paymentDate >= today
    })
    stats.value.todayPayments = todayPayments.reduce((sum, p) => sum + (p.amount || 0), 0)
    stats.value.paymentCount = todayPayments.length

    // Overdue students
    const overdueInvoices = invoices.filter(inv => {
      if (inv.status !== 'Unpaid' && inv.status !== 'PartiallyPaid') return false
      const dueDate = new Date(inv.dueDate)
      return dueDate < now
    })
    
    // Count unique students with overdue invoices
    const overdueStudentIds = new Set(overdueInvoices.map(inv => inv.studentId))
    stats.value.overdueStudents = overdueStudentIds.size
    
    // Total debt from overdue invoices
    stats.value.totalDebt = overdueInvoices.reduce((sum, inv) => 
      sum + (inv.debtAmount || inv.totalAmount || 0), 0
    )

    // Room occupancy statistics
    stats.value.totalRooms = rooms.length
    stats.value.occupiedRooms = rooms.filter(r => r.currentOccupants > 0).length
    stats.value.availableRooms = rooms.filter(r => {
      const available = r.maxOccupants - r.currentOccupants
      return available > 0 && r.status === 'Available'
    }).length
    stats.value.maintenanceRooms = rooms.filter(r => r.status === 'Maintenance').length

    // Calculate occupancy rate
    const totalCapacity = rooms.reduce((sum, r) => sum + r.maxOccupants, 0)
    const currentOccupants = rooms.reduce((sum, r) => sum + r.currentOccupants, 0)
    stats.value.occupancyRate = totalCapacity > 0 
      ? Math.round((currentOccupants / totalCapacity) * 100) 
      : 0

    // Generate alerts based on real data
    alerts.value = []

    if (stats.value.applicationsOver48h > 0) {
      alerts.value.push({
        id: 1,
        status: 'error',
        urgent: true,
        title: 'Đơn đăng ký chờ quá 48h',
        description: `${stats.value.applicationsOver48h} đơn đăng ký chờ xử lý quá 48 giờ, cần duyệt ngay`,
        action: 'applications'
      })
    }

    if (stats.value.urgentMaintenance > 0) {
      alerts.value.push({
        id: 2,
        status: 'error',
        urgent: true,
        title: 'Yêu cầu bảo trì khẩn cấp',
        description: `${stats.value.urgentMaintenance} yêu cầu bảo trì mức độ khẩn cấp chưa phân công`,
        action: 'maintenance'
      })
    }

    // Contracts expiring soon (within 7 days)
    const days7 = 7 * 24 * 60 * 60 * 1000
    const expiringSoon = contracts.filter(c => {
      if (c.status !== 'Active') return false
      const endDate = new Date(c.endDate)
      const diff = endDate - now
      return diff > 0 && diff < days7
    })

    if (expiringSoon.length > 0) {
      alerts.value.push({
        id: 3,
        status: 'warning',
        urgent: false,
        title: 'Hợp đồng sắp hết hạn',
        description: `${expiringSoon.length} hợp đồng sẽ hết hạn trong 7 ngày tới`,
        action: 'contracts'
      })
    }

    // Students with debt over 2 months
    const overdueOver2Months = overdueInvoices.filter(inv => {
      const dueDate = new Date(inv.dueDate)
      const days60 = 60 * 24 * 60 * 60 * 1000
      return (now - dueDate) > days60
    })

    if (overdueOver2Months.length > 0) {
      const uniqueStudents = new Set(overdueOver2Months.map(inv => inv.studentId))
      alerts.value.push({
        id: 4,
        status: 'warning',
        urgent: false,
        title: 'Sinh viên nợ quá 2 tháng',
        description: `${uniqueStudents.size} sinh viên nợ tiền phòng quá 2 tháng, cần liên hệ`,
        action: 'debt'
      })
    }

    // If no alerts, add a placeholder
    if (alerts.value.length === 0) {
      alerts.value.push({
        id: 0,
        status: 'success',
        urgent: false,
        title: 'Không có cảnh báo',
        description: 'Tất cả công việc đang được xử lý tốt',
        action: null
      })
    }

    message.success('Đã làm mới dữ liệu')
  } catch (error) {
    console.error('Error loading staff dashboard data:', error)
    message.error('Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const handleAlert = (item) => {
  if (!item.action) return
  
  const routes = {
    applications: '/staff/room-applications',
    maintenance: '/staff/maintenance-requests',
    contracts: '/staff/contracts',
    debt: '/staff/debt'
  }
  if (routes[item.action]) {
    window.location.href = routes[item.action]
  }
}

const showPaymentModal = () => {
  paymentModalVisible.value = true
  paymentForm.value = {
    studentSearch: '',
    selectedStudent: null,
    selectedInvoiceId: null,
    amount: 0,
    maxAmount: 0,
    paymentMethod: 'Cash',
    notes: ''
  }
  unpaidInvoices.value = []
}

const searchStudentForPayment = async () => {
  if (!paymentForm.value.studentSearch.trim()) {
    message.warning('Vui lòng nhập mã sinh viên hoặc tên')
    return
  }

  searchingStudent.value = true
  try {
    // Search users with role Student
    const response = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    const students = response.data.filter(u => u.role === 'Student')
    const searchTerm = paymentForm.value.studentSearch.toLowerCase()
    
    const found = students.find(s => 
      s.studentCode?.toLowerCase().includes(searchTerm) ||
      s.fullName?.toLowerCase().includes(searchTerm)
    )

    if (!found) {
      message.error('Không tìm thấy sinh viên')
      paymentForm.value.selectedStudent = null
      unpaidInvoices.value = []
      return
    }

    paymentForm.value.selectedStudent = found

    // Load unpaid invoices for this student
    const allInvoices = await invoiceService.getAll()
    unpaidInvoices.value = allInvoices.filter(inv => 
      inv.studentId === found.id && 
      (inv.status === 'Unpaid' || inv.status === 'PartialPaid') &&
      inv.debtAmount > 0
    )

    if (unpaidInvoices.value.length === 0) {
      message.info('Sinh viên không có khoản nợ')
    } else {
      message.success(`Tìm thấy ${unpaidInvoices.value.length} phiếu thu chưa thanh toán`)
    }
  } catch (error) {
    console.error('Error searching student:', error)
    message.error('Lỗi tìm kiếm sinh viên')
  } finally {
    searchingStudent.value = false
  }
}

const onInvoiceSelected = () => {
  const invoice = unpaidInvoices.value.find(inv => inv.id === paymentForm.value.selectedInvoiceId)
  if (invoice) {
    paymentForm.value.maxAmount = invoice.debtAmount
    paymentForm.value.amount = invoice.debtAmount
  }
}

const handleQuickPayment = async () => {
  if (!paymentForm.value.selectedStudent) {
    message.warning('Vui lòng tìm sinh viên trước')
    return
  }

  if (!paymentForm.value.selectedInvoiceId) {
    message.warning('Vui lòng chọn phiếu thu')
    return
  }

  if (!paymentForm.value.amount || paymentForm.value.amount <= 0) {
    message.warning('Vui lòng nhập số tiền thanh toán')
    return
  }

  if (paymentForm.value.amount > paymentForm.value.maxAmount) {
    message.warning('Số tiền thanh toán không được lớn hơn số tiền còn nợ')
    return
  }

  processingPayment.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    
    await paymentService.create({
      invoiceId: paymentForm.value.selectedInvoiceId,
      amount: paymentForm.value.amount,
      paymentMethod: paymentForm.value.paymentMethod,
      paymentDate: new Date().toISOString(),
      notes: paymentForm.value.notes,
      createdByUserId: user.id,
      createdByName: user.fullName
    })

    message.success('Thanh toán thành công')
    paymentModalVisible.value = false
    await refreshData()
  } catch (error) {
    console.error('Payment error:', error)
    message.error(error.response?.data?.message || 'Lỗi thanh toán')
  } finally {
    processingPayment.value = false
  }
}

const showStudentLookup = () => {
  lookupModalVisible.value = true
  lookupSearch.value = ''
  lookupResults.value = []
  hasSearched.value = false
}

const performStudentLookup = async () => {
  if (!lookupSearch.value.trim()) {
    message.warning('Vui lòng nhập thông tin tìm kiếm')
    return
  }

  lookingUp.value = true
  hasSearched.value = true
  try {
    // Get all users
    const usersResponse = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    // Get all contracts to map room info
    const contractsData = await contractService.getAll()

    const students = usersResponse.data.filter(u => u.role === 'Student')
    const searchTerm = lookupSearch.value.toLowerCase()

    // Search in multiple fields
    const results = students.filter(s =>
      s.studentCode?.toLowerCase().includes(searchTerm) ||
      s.fullName?.toLowerCase().includes(searchTerm) ||
      s.email?.toLowerCase().includes(searchTerm) ||
      s.phone?.toLowerCase().includes(searchTerm) ||
      s.classCode?.toLowerCase().includes(searchTerm)
    )

    // Enrich with contract/room info
    lookupResults.value = results.map(student => {
      const activeContract = contractsData.find(c => 
        c.studentId === student.id && c.status === 'Active'
      )
      return {
        ...student,
        roomNumber: activeContract?.roomNumber,
        buildingName: activeContract?.buildingName,
        isActive: !!activeContract
      }
    })

    if (lookupResults.value.length === 0) {
      message.info('Không tìm thấy sinh viên phù hợp')
    } else {
      message.success(`Tìm thấy ${lookupResults.value.length} sinh viên`)
    }
  } catch (error) {
    console.error('Lookup error:', error)
    message.error('Lỗi tra cứu sinh viên')
  } finally {
    lookingUp.value = false
  }
}

const viewStudentDetail = async (student) => {
  try {
    // Get contract info
    const contracts = await contractService.getAll()
    const activeContract = contracts.find(c => 
      c.studentId === student.id && c.status === 'Active'
    )

    // Get unpaid invoices to calculate debt
    const allInvoices = await invoiceService.getAll()
    const unpaidInvoices = allInvoices.filter(inv => 
      inv.studentId === student.id && 
      (inv.status === 'Unpaid' || inv.status === 'PartialPaid')
    )

    const totalDebt = unpaidInvoices.reduce((sum, inv) => sum + (inv.debtAmount || 0), 0)

    selectedStudentDetail.value = {
      ...student,
      roomNumber: activeContract?.roomNumber,
      buildingName: activeContract?.buildingName,
      contractInfo: activeContract,
      totalDebt
    }

    studentDetailVisible.value = true
  } catch (error) {
    console.error('Error loading student details:', error)
    message.error('Lỗi tải thông tin chi tiết')
  }
}

const quickPayForStudent = (student) => {
  studentDetailVisible.value = false
  lookupModalVisible.value = false
  
  paymentForm.value.studentSearch = student.studentCode
  showPaymentModal()
  
  // Trigger search automatically
  setTimeout(() => {
    searchStudentForPayment()
  }, 100)
}

onMounted(() => {
  refreshData()
})
</script>
