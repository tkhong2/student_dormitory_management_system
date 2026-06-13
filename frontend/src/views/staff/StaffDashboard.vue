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

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
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
  message.info('Chức năng thu tiền nhanh đang được phát triển')
}

const showStudentLookup = () => {
  message.info('Chức năng tra cứu sinh viên đang được phát triển')
}

onMounted(() => {
  refreshData()
})
</script>
