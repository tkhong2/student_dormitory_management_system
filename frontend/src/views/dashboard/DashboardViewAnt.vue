<template>
  <div>
    <PageHeaderAnt title="Dashboard" subtitle="Tổng quan hoạt động ký túc xá hôm nay">
      <template #actions>
        <a-button type="primary">
          <template #icon><DownloadOutlined /></template>
          Xuất báo cáo
        </a-button>
      </template>
    </PageHeaderAnt>

    <!-- Stat cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col v-for="s in stats" :key="s.label" :xs="24" :sm="12" :lg="6">
        <StatCardAnt v-bind="s" :loading="loading" />
      </a-col>
    </a-row>

    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <!-- Charts -->
      <a-col :xs="24" :lg="16">
        <a-card :bordered="true" :loading="loading" style="border-radius: 12px;">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 12px;">
            <h3 style="font-size: 16px; font-weight: 700; margin: 0;">Thống kê tỷ lệ lấp đầy & Doanh thu</h3>
            <a-radio-group v-model:value="chartPeriod" button-style="solid">
              <a-radio-button value="week">Tuần</a-radio-button>
              <a-radio-button value="month">Tháng</a-radio-button>
              <a-radio-button value="year">Năm</a-radio-button>
            </a-radio-group>
          </div>
          
          <a-row :gutter="16">
            <a-col :xs="24" :md="12">
              <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 12px; text-transform: uppercase;">Tỷ lệ lấp đầy (%)</div>
              <div style="height: 300px; position: relative;">
                <Bar :data="barData" :options="barOptions" />
              </div>
            </a-col>
            <a-col :xs="24" :md="12">
              <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 12px; text-transform: uppercase;">Xu hướng doanh thu</div>
              <div style="height: 300px; position: relative;">
                <Line :data="lineData" :options="lineOptions" />
              </div>
            </a-col>
          </a-row>
        </a-card>
      </a-col>

      <!-- Recent activity -->
      <a-col :xs="24" :lg="8">
        <a-card :bordered="true" :loading="loading" style="border-radius: 12px; height: 100%;">
          <h3 style="font-size: 16px; font-weight: 700; margin: 0 0 20px 0;">Hoạt động gần đây</h3>
          <a-timeline>
            <a-timeline-item v-for="a in activities" :key="a.id" :color="getTimelineColor(a.color)">
              <div style="font-size: 13px; font-weight: 600; margin-bottom: 4px;">{{ a.text }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ a.time }}</div>
            </a-timeline-item>
          </a-timeline>
        </a-card>
      </a-col>
    </a-row>

    <!-- Quick actions -->
    <a-row :gutter="[16, 16]" style="display: flex; align-items: stretch;">
      <a-col :xs="24" :md="8" style="display: flex;">
        <a-card :bordered="false" class="quick-action-card" style="background: linear-gradient(135deg, #ff6b00 0%, #ff8800 100%);">
          <div class="quick-action-content">
            <UserAddOutlined style="font-size: 48px; opacity: 0.3; position: absolute; right: 0; top: 0;" />
            <h3 style="font-size: 20px; font-weight: 700; margin: 0 0 8px 0; color: white;">Tiếp nhận SV mới</h3>
            <p class="quick-action-desc">Thêm hồ sơ sinh viên và xếp phòng nhanh chóng trong vài bước.</p>
            <div>
              <a-button type="default" size="large" style="font-weight: 600;">Bắt đầu</a-button>
            </div>
          </div>
        </a-card>
      </a-col>
      
      <a-col :xs="24" :md="8" style="display: flex;">
        <a-card :bordered="false" class="quick-action-card" style="background: linear-gradient(135deg, #1890ff 0%, #36cfc9 100%);">
          <div class="quick-action-content">
            <CalculatorOutlined style="font-size: 48px; opacity: 0.3; position: absolute; right: 0; top: 0;" />
            <h3 style="font-size: 20px; font-weight: 700; margin: 0 0 8px 0; color: white;">Tính lệ phí tháng</h3>
            <p class="quick-action-desc">Tự động tạo hóa đơn dịch vụ cho tất cả sinh viên đang ở.</p>
            <div>
              <a-button type="default" size="large" style="font-weight: 600;">Tạo hóa đơn</a-button>
            </div>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="8" style="display: flex;">
        <a-card :bordered="false" class="quick-action-card" style="background: linear-gradient(135deg, #52c41a 0%, #73d13d 100%);">
          <div class="quick-action-content">
            <FileTextOutlined style="font-size: 48px; opacity: 0.3; position: absolute; right: 0; top: 0;" />
            <h3 style="font-size: 20px; font-weight: 700; margin: 0 0 8px 0; color: white;">Xuất báo cáo</h3>
            <p class="quick-action-desc">Tổng hợp số liệu hoạt động KTX theo tháng/quý/năm.</p>
            <div>
              <a-button type="default" size="large" style="font-weight: 600;">Xem báo cáo</a-button>
            </div>
          </div>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { 
  Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, 
  PointElement, LineElement, ArcElement 
} from 'chart.js'
import { Bar, Line } from 'vue-chartjs'
import { DownloadOutlined, UserAddOutlined, CalculatorOutlined, FileTextOutlined } from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import StatCardAnt from '@/components/common/StatCardAnt.vue'
import { studentService } from '@/services/studentService'
import { roomService } from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { invoiceService } from '@/services/invoiceService'
import maintenanceRequestService from '@/services/maintenanceRequestService'
import { roomApplicationService } from '@/services/roomApplicationService'
import { contractService } from '@/services/contractService'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, PointElement, LineElement, ArcElement)

const chartPeriod = ref('month')
const loading = ref(false)

// Real data refs
const totalStudents = ref(0)
const emptyRooms = ref(0)
const unpaidAmount = ref(0)
const maintenanceCount = ref(0)
const buildingOccupancy = ref([])
const recentActivities = ref([])

const stats = computed(() => [
  { 
    label: 'Tổng sinh viên', 
    value: totalStudents.value.toString(), 
    icon: 'mdi-school-outline', 
    color: 'primary', 
    trend: 12 
  },
  { 
    label: 'Phòng trống', 
    value: emptyRooms.value.toString(), 
    icon: 'mdi-door-open', 
    color: 'info', 
    trend: -3 
  },
  { 
    label: 'Chưa thanh toán', 
    value: formatCurrency(unpaidAmount.value), 
    icon: 'mdi-cash-clock', 
    color: 'warning', 
    trend: 0 
  },
  { 
    label: 'Yêu cầu bảo trì', 
    value: maintenanceCount.value.toString(), 
    icon: 'mdi-wrench-clock', 
    color: 'error', 
    trend: 22 
  },
])

// Bar Chart Data - Building Occupancy
const barData = computed(() => ({
  labels: buildingOccupancy.value.map(b => b.name),
  datasets: [{
    label: 'Tỷ lệ lấp đầy (%)',
    backgroundColor: '#ff6b00',
    data: buildingOccupancy.value.map(b => b.occupancyRate),
    borderRadius: 8
  }]
}))

const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { 
    legend: { display: false }
  },
  scales: { 
    y: { 
      beginAtZero: true, 
      max: 100,
      grid: { color: '#f1f5f9' }
    },
    x: {
      grid: { display: false }
    }
  }
}

// Line Chart Data - Revenue trend (simulated for now)
const lineData = {
  labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6'],
  datasets: [{
    label: 'Doanh thu (Triệu VNĐ)',
    borderColor: '#1e3a8a',
    backgroundColor: 'rgba(30, 58, 138, 0.05)',
    data: [420, 450, 440, 480, 510, 490],
    fill: true,
    tension: 0.4,
    pointBackgroundColor: '#1e3a8a',
    pointRadius: 4
  }]
}

const lineOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { 
    legend: { position: 'top' } 
  },
  scales: {
    y: { grid: { color: '#f1f5f9' } },
    x: { grid: { display: false } }
  }
}

const activities = computed(() => recentActivities.value)

const getTimelineColor = (color) => {
  const colorMap = {
    'success': 'green',
    'error': 'red',
    'info': 'blue',
    'warning': 'orange',
  }
  return colorMap[color] || 'blue'
}

const formatCurrency = (amount) => {
  if (!amount || amount === 0) {
    return '0₫'
  }
  if (amount >= 1000000) {
    return (amount / 1000000).toFixed(1) + 'M₫'
  } else if (amount >= 1000) {
    return (amount / 1000).toFixed(0) + 'K₫'
  }
  return amount.toLocaleString('vi-VN') + '₫'
}

const getRelativeTime = (date) => {
  const now = new Date()
  const past = new Date(date)
  const diffMs = now - past
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 60) {
    return `${diffMins} phút trước`
  } else if (diffHours < 24) {
    return `${diffHours} giờ trước`
  } else if (diffDays === 1) {
    return 'Hôm qua'
  } else {
    return `${diffDays} ngày trước`
  }
}

onMounted(async () => {
  await loadDashboardData()
})

async function loadDashboardData() {
  loading.value = true
  try {
    // Load all data in parallel
    const [
      students,
      rooms,
      buildings,
      invoices,
      maintenanceRequests,
      applications,
      contracts
    ] = await Promise.all([
      studentService.getAll(),
      roomService.getAll(),
      buildingService.getAll(),
      invoiceService.getAll(),
      maintenanceRequestService.getAll(),
      roomApplicationService.getAll(),
      contractService.getAll()
    ])

    // Total students
    totalStudents.value = students.length

    // Empty rooms (available > 0)
    emptyRooms.value = rooms.filter(r => {
      const available = r.maxOccupants - r.currentOccupants
      return available > 0
    }).length

    // Unpaid invoices - calculate from payments or show all invoices total
    const unpaidInvoices = invoices.filter(inv => 
      inv.status === 'Pending' || inv.status === 'Overdue' || inv.status === 'Unpaid'
    )
    
    let calculatedUnpaid = unpaidInvoices.reduce((sum, inv) => {
      const amount = inv.totalAmount || inv.amount || inv.totalPrice || 0
      return sum + amount
    }, 0)
    
    // If no unpaid invoices or amount is 0, calculate expected monthly revenue from active contracts
    if (calculatedUnpaid === 0) {
      const activeContracts = contracts.filter(c => c.status === 'Active')
      // Calculate monthly rent from active contracts
      calculatedUnpaid = activeContracts.reduce((sum, contract) => {
        const monthlyRent = contract.monthlyRent || contract.rentAmount || 0
        return sum + monthlyRent
      }, 0)
    }
    
    unpaidAmount.value = calculatedUnpaid

    console.log('Unpaid invoices:', unpaidInvoices)
    console.log('All invoices:', invoices)
    console.log('Unpaid amount:', unpaidAmount.value)

    // Maintenance requests (Pending or InProgress)
    maintenanceCount.value = maintenanceRequests.filter(mr => 
      mr.status === 'Pending' || mr.status === 'InProgress'
    ).length

    // Building occupancy
    buildingOccupancy.value = buildings.map(building => {
      const buildingRooms = rooms.filter(r => r.buildingId === building.id)
      const totalCapacity = buildingRooms.reduce((sum, r) => sum + r.maxOccupants, 0)
      const currentOccupants = buildingRooms.reduce((sum, r) => sum + r.currentOccupants, 0)
      const occupancyRate = totalCapacity > 0 ? Math.round((currentOccupants / totalCapacity) * 100) : 0

      return {
        name: building.name,
        occupancyRate
      }
    }).filter(b => b.occupancyRate > 0) // Only show buildings with data

    // Recent activities
    const activities = []

    // Recent applications (last 3)
    const recentApps = applications
      .filter(app => app.createdAt) // Only apps with createdAt
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 3)
    
    recentApps.forEach(app => {
      activities.push({
        id: `app-${app.id}`,
        text: `${app.studentName || 'Sinh viên'} đăng ký phòng`,
        time: getRelativeTime(app.createdAt),
        icon: 'mdi-account-plus',
        color: 'success',
        date: new Date(app.createdAt)
      })
    })

    // Recent contracts (last 3)
    const recentContracts = contracts
      .filter(c => c.createdAt && (c.status === 'Active' || c.status === 'Pending'))
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 2)
    
    recentContracts.forEach(contract => {
      activities.push({
        id: `contract-${contract.id}`,
        text: `Hợp đồng ${contract.studentName || 'sinh viên'} - Phòng ${contract.roomNumber || 'N/A'}`,
        time: getRelativeTime(contract.createdAt),
        icon: 'mdi-file-document',
        color: 'info',
        date: new Date(contract.createdAt)
      })
    })

    // Recent maintenance requests (last 2)
    const recentMaintenance = maintenanceRequests
      .filter(mr => mr.createdAt)
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 2)
    
    recentMaintenance.forEach(mr => {
      const statusText = mr.status === 'Completed' ? 'hoàn tất' : 'đang xử lý'
      activities.push({
        id: `mr-${mr.id}`,
        text: `Bảo trì phòng ${mr.roomNumber || 'N/A'} ${statusText}`,
        time: getRelativeTime(mr.createdAt),
        icon: 'mdi-wrench',
        color: mr.status === 'Completed' ? 'success' : 'warning',
        date: new Date(mr.createdAt)
      })
    })

    // If no activities, add a placeholder
    if (activities.length === 0) {
      activities.push({
        id: 'placeholder',
        text: 'Chưa có hoạt động gần đây',
        time: 'Hôm nay',
        icon: 'mdi-information',
        color: 'info',
        date: new Date()
      })
    }

    // Sort all activities by date
    recentActivities.value = activities
      .sort((a, b) => b.date - a.date)
      .slice(0, 5)

    console.log('Recent activities:', recentActivities.value)

  } catch (error) {
    console.error('Error loading dashboard data:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.quick-action-card {
  width: 100%;
  border-radius: 16px;
  color: white;
  height: 100%;
}

.quick-action-card :deep(.ant-card-body) {
  height: 100%;
}

.quick-action-content {
  position: relative;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.quick-action-desc {
  font-size: 13px;
  opacity: 0.9;
  margin: 0 0 20px 0;
  flex: 1;
}
</style>
