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
              <a-button type="default" size="large" style="font-weight: 600;" @click="goToPage('/admin/room-applications')">Bắt đầu</a-button>
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
              <a-button type="default" size="large" style="font-weight: 600;" @click="goToPage('/admin/invoices')">Tạo hóa đơn</a-button>
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
              <a-button type="default" size="large" style="font-weight: 600;" @click="goToPage('/admin/payments')">Xem báo cáo</a-button>
            </div>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Additional info sections -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px;">
      <!-- Pending applications -->
      <a-col :xs="24" :md="12">
        <a-card title="Đơn đăng ký chờ duyệt" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-list :data-source="pendingApplications" size="small">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>
                    <a @click="goToPage('/admin/room-applications')">{{ item.studentName }}</a>
                  </template>
                  <template #description>
                    {{ item.roomTypeName }} - {{ item.date }}
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-tag :color="item.status === 'Pending' ? 'orange' : 'green'">{{ getStatusLabel(item.status) }}</a-tag>
                </template>
              </a-list-item>
            </template>
          </a-list>
          <div style="text-align: center; margin-top: 12px;" v-if="pendingApplications.length === 0">
            <a-empty description="Không có đơn chờ duyệt" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>

      <!-- Expiring contracts -->
      <a-col :xs="24" :md="12">
        <a-card title="Hợp đồng sắp hết hạn" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-list :data-source="expiringContracts" size="small">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>
                    <a @click="goToPage('/admin/contracts')">{{ item.studentName }}</a>
                  </template>
                  <template #description>
                    Phòng {{ item.roomNumber }} - Còn {{ item.daysLeft }} ngày
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-tag :color="item.daysLeft <= 7 ? 'red' : 'orange'">{{ item.daysLeft }} ngày</a-tag>
                </template>
              </a-list-item>
            </template>
          </a-list>
          <div style="text-align: center; margin-top: 12px;" v-if="expiringContracts.length === 0">
            <a-empty description="Không có hợp đồng sắp hết hạn" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- More info sections -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px;">
      <!-- Maintenance requests -->
      <a-col :xs="24" :md="12" :lg="8">
        <a-card title="Yêu cầu bảo trì" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-list :data-source="maintenanceRequests" size="small">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>
                    <a @click="goToPage('/admin/maintenance-requests')">Phòng {{ item.roomNumber }}</a>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-tag :color="getPriorityColor(item.priority)">{{ item.priority }}</a-tag>
                </template>
              </a-list-item>
            </template>
          </a-list>
          <div style="text-align: center; margin-top: 12px;" v-if="maintenanceRequests.length === 0">
            <a-empty description="Không có yêu cầu bảo trì" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>

      <!-- Recent payments -->
      <a-col :xs="24" :md="12" :lg="8">
        <a-card title="Thanh toán gần đây" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-list :data-source="recentPayments" size="small">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>
                    <a @click="goToPage('/admin/payments')">{{ item.studentName }}</a>
                  </template>
                  <template #description>
                    {{ formatCurrency(item.amount) }} - {{ item.date }}
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-tag color="green">{{ item.method }}</a-tag>
                </template>
              </a-list-item>
            </template>
          </a-list>
          <div style="text-align: center; margin-top: 12px;" v-if="recentPayments.length === 0">
            <a-empty description="Chưa có thanh toán" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>

      <!-- Room transfers -->
      <a-col :xs="24" :md="12" :lg="8">
        <a-card title="Yêu cầu chuyển phòng" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-list :data-source="roomTransfers" size="small">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #title>
                    <a @click="goToPage('/admin/room-transfers')">{{ item.studentName }}</a>
                  </template>
                  <template #description>
                    {{ item.fromRoom }} → {{ item.toRoom }}
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-tag :color="getTransferStatusColor(item.status)">{{ item.status }}</a-tag>
                </template>
              </a-list-item>
            </template>
          </a-list>
          <div style="text-align: center; margin-top: 12px;" v-if="roomTransfers.length === 0">
            <a-empty description="Không có yêu cầu chuyển phòng" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Statistics grid -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px;">
      <!-- Room status breakdown -->
      <a-col :xs="24" :md="12">
        <a-card title="Tình trạng phòng" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-row :gutter="[12, 12]">
            <a-col :span="12" v-for="status in roomStatusBreakdown" :key="status.label">
              <div style="padding: 16px; background: #f5f5f5; border-radius: 8px; text-align: center;">
                <div style="font-size: 24px; font-weight: 700; color: #262626;">{{ status.count }}</div>
                <div style="font-size: 13px; color: #8c8c8c; margin-top: 4px;">{{ status.label }}</div>
                <a-progress 
                  :percent="status.percent" 
                  :show-info="false" 
                  :stroke-color="status.color"
                  size="small"
                  style="margin-top: 8px;"
                />
              </div>
            </a-col>
          </a-row>
        </a-card>
      </a-col>

      <!-- Revenue summary -->
      <a-col :xs="24" :md="12">
        <a-card title="Tổng quan doanh thu tháng này" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-row :gutter="[12, 12]">
            <a-col :span="12">
              <a-statistic 
                title="Đã thu" 
                :value="revenueSummary.collected" 
                :precision="0"
                suffix="₫"
                :value-style="{ color: '#52c41a', fontSize: '20px' }"
              />
            </a-col>
            <a-col :span="12">
              <a-statistic 
                title="Chưa thu" 
                :value="revenueSummary.pending" 
                :precision="0"
                suffix="₫"
                :value-style="{ color: '#ff4d4f', fontSize: '20px' }"
              />
            </a-col>
            <a-col :span="24" style="margin-top: 12px;">
              <div style="padding: 12px; background: #e6f7ff; border-radius: 8px; border: 1px solid #91d5ff;">
                <div style="font-size: 12px; color: #0958d9; margin-bottom: 4px;">Tổng doanh thu</div>
                <div style="font-size: 24px; font-weight: 700; color: #0958d9;">
                  {{ formatCurrency(revenueSummary.total) }}
                </div>
              </div>
            </a-col>
            <a-col :span="24" style="margin-top: 8px;">
              <a-progress 
                :percent="revenueSummary.collectionRate" 
                :stroke-color="{ '0%': '#108ee9', '100%': '#87d068' }"
                :format="percent => `Tỷ lệ thu: ${percent}%`"
              />
            </a-col>
          </a-row>
        </a-card>
      </a-col>
    </a-row>

    <!-- Top students by debt -->
    <a-row :gutter="[16, 16]" style="margin-top: 16px;">
      <a-col :xs="24">
        <a-card title="Top sinh viên nợ nhiều nhất" :bordered="true" :loading="loading" style="border-radius: 12px;">
          <a-table 
            :dataSource="topDebtors" 
            :columns="debtorColumns"
            :pagination="false"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'studentName'">
                <a @click="goToPage('/admin/students')">{{ record.studentName }}</a>
              </template>
              <template v-else-if="column.key === 'amount'">
                <span style="color: #ff4d4f; font-weight: 600;">{{ formatCurrency(record.amount) }}</span>
              </template>
              <template v-else-if="column.key === 'months'">
                <a-tag :color="record.months >= 3 ? 'red' : 'orange'">{{ record.months }} tháng</a-tag>
              </template>
            </template>
          </a-table>
          <div style="text-align: center; margin-top: 12px;" v-if="topDebtors.length === 0">
            <a-empty description="Không có sinh viên nợ" :image-style="{ height: '60px' }" />
          </div>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
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
import api from '@/services/api'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, PointElement, LineElement, ArcElement)

const router = useRouter()
const chartPeriod = ref('month')
const loading = ref(false)

// Real data refs
const totalStudents = ref(0)
const emptyRooms = ref(0)
const unpaidAmount = ref(0)
const maintenanceCount = ref(0)
const buildingOccupancy = ref([])
const recentActivities = ref([])
const pendingApplications = ref([])
const expiringContracts = ref([])
const maintenanceRequests = ref([])
const recentPayments = ref([])
const roomTransfers = ref([])
const roomStatusBreakdown = ref([])
const revenueSummary = ref({ collected: 0, pending: 0, total: 0, collectionRate: 0 })
const topDebtors = ref([])

const debtorColumns = [
  { title: 'STT', dataIndex: 'index', key: 'index', width: 60 },
  { title: 'Sinh viên', dataIndex: 'studentName', key: 'studentName' },
  { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 100 },
  { title: 'Số tiền nợ', dataIndex: 'amount', key: 'amount', width: 150 },
  { title: 'Số tháng', dataIndex: 'months', key: 'months', width: 100 }
]

const goToPage = (path) => {
  router.push(path)
}

const getStatusLabel = (status) => {
  const labels = {
    'Pending': 'Chờ duyệt',
    'Approved': 'Đã duyệt',
    'Rejected': 'Từ chối'
  }
  return labels[status] || status
}

const getPriorityColor = (priority) => {
  const colors = {
    'Urgent': 'red',
    'High': 'orange',
    'Normal': 'blue',
    'Low': 'default'
  }
  return colors[priority] || 'default'
}

const getTransferStatusColor = (status) => {
  const colors = {
    'Pending': 'orange',
    'Approved': 'green',
    'Rejected': 'red',
    'Completed': 'blue'
  }
  return colors[status] || 'default'
}

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

    // Pending applications (last 5)
    pendingApplications.value = applications
      .filter(app => app.status === 'Pending')
      .sort((a, b) => new Date(b.submittedDate || b.createdAt) - new Date(a.submittedDate || a.createdAt))
      .slice(0, 5)
      .map(app => ({
        id: app.id,
        studentName: app.studentName || 'N/A',
        roomTypeName: app.roomTypeName || 'Chưa chọn',
        date: getRelativeTime(app.submittedDate || app.createdAt),
        status: app.status
      }))

    // Expiring contracts (within 30 days)
    expiringContracts.value = contracts
      .filter(c => {
        if (c.status !== 'Active') return false
        const endDate = new Date(c.endDate)
        const now = new Date()
        const daysLeft = Math.floor((endDate - now) / (1000 * 60 * 60 * 24))
        return daysLeft > 0 && daysLeft <= 30
      })
      .sort((a, b) => new Date(a.endDate) - new Date(b.endDate))
      .slice(0, 5)
      .map(c => {
        const daysLeft = Math.floor((new Date(c.endDate) - new Date()) / (1000 * 60 * 60 * 24))
        return {
          id: c.id,
          studentName: c.studentName || 'N/A',
          roomNumber: c.roomNumber || 'N/A',
          daysLeft
        }
      })

    // Maintenance requests (pending/in progress)
    maintenanceRequests.value = maintenanceRequests
      .filter(mr => mr.status === 'Pending' || mr.status === 'InProgress')
      .sort((a, b) => {
        // Sort by priority then date
        const priorityOrder = { 'Urgent': 4, 'High': 3, 'Normal': 2, 'Low': 1 }
        if (priorityOrder[a.priority] !== priorityOrder[b.priority]) {
          return priorityOrder[b.priority] - priorityOrder[a.priority]
        }
        return new Date(b.createdAt) - new Date(a.createdAt)
      })
      .slice(0, 5)
      .map(mr => ({
        id: mr.id,
        roomNumber: mr.roomNumber || 'N/A',
        description: mr.description ? mr.description.substring(0, 40) + '...' : 'Yêu cầu bảo trì',
        priority: mr.priority || 'Normal'
      }))

    // Recent payments (simulated - would come from payment API)
    try {
      const payments = await invoiceService.getAll() // Assuming this returns payment records
      recentPayments.value = payments
        .filter(p => p.status === 'Paid' || p.status === 'Completed')
        .sort((a, b) => new Date(b.paidDate || b.createdAt) - new Date(a.paidDate || a.createdAt))
        .slice(0, 5)
        .map(p => ({
          id: p.id,
          studentName: p.studentName || 'N/A',
          amount: p.totalAmount || p.amount || 0,
          date: getRelativeTime(p.paidDate || p.createdAt),
          method: p.paymentMethod || 'Tiền mặt'
        }))
    } catch (err) {
      console.error('Error loading payments:', err)
      recentPayments.value = []
    }

    // Room transfers
    try {
      const transfersResponse = await api.get('/roomtransfers')
      const transfers = transfersResponse.data || []
      roomTransfers.value = transfers
        .filter(t => t.status === 'Pending' || t.status === 'Approved')
        .sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate))
        .slice(0, 5)
        .map(t => ({
          id: t.id,
          studentName: t.studentName || 'N/A',
          fromRoom: t.fromRoomNumber || 'N/A',
          toRoom: t.toRoomNumber || 'N/A',
          status: t.status
        }))
    } catch (err) {
      console.error('Error loading room transfers:', err)
      roomTransfers.value = []
    }

    // Room status breakdown
    const totalRooms = rooms.length
    const availableCount = rooms.filter(r => r.status === 'Available').length
    const occupiedCount = rooms.filter(r => r.status === 'Occupied').length
    const fullCount = rooms.filter(r => r.status === 'Full').length
    const maintenanceRoomCount = rooms.filter(r => r.status === 'Maintenance').length

    roomStatusBreakdown.value = [
      { 
        label: 'Trống', 
        count: availableCount, 
        percent: totalRooms > 0 ? Math.round((availableCount / totalRooms) * 100) : 0,
        color: '#52c41a'
      },
      { 
        label: 'Đang ở', 
        count: occupiedCount, 
        percent: totalRooms > 0 ? Math.round((occupiedCount / totalRooms) * 100) : 0,
        color: '#1890ff'
      },
      { 
        label: 'Đầy', 
        count: fullCount, 
        percent: totalRooms > 0 ? Math.round((fullCount / totalRooms) * 100) : 0,
        color: '#ff4d4f'
      },
      { 
        label: 'Bảo trì', 
        count: maintenanceRoomCount, 
        percent: totalRooms > 0 ? Math.round((maintenanceRoomCount / totalRooms) * 100) : 0,
        color: '#fa8c16'
      }
    ]

    // Revenue summary (from invoices)
    const paidInvoices = invoices.filter(inv => inv.status === 'Paid' || inv.status === 'Completed')
    const unpaidInvoicesForRevenue = invoices.filter(inv => inv.status === 'Unpaid' || inv.status === 'Pending' || inv.status === 'Overdue')
    
    const collected = paidInvoices.reduce((sum, inv) => sum + (inv.totalAmount || inv.amount || 0), 0)
    const pending = unpaidInvoicesForRevenue.reduce((sum, inv) => sum + (inv.totalAmount || inv.amount || 0), 0)
    const total = collected + pending
    
    revenueSummary.value = {
      collected,
      pending,
      total,
      collectionRate: total > 0 ? Math.round((collected / total) * 100) : 0
    }

    // Top debtors (students with highest unpaid amounts)
    const debtByStudent = {}
    unpaidInvoicesForRevenue.forEach(inv => {
      const studentId = inv.studentId || inv.studentName || 'Unknown'
      if (!debtByStudent[studentId]) {
        debtByStudent[studentId] = {
          studentId,
          studentName: inv.studentName || 'N/A',
          roomNumber: inv.roomNumber || 'N/A',
          amount: 0,
          invoiceCount: 0
        }
      }
      debtByStudent[studentId].amount += (inv.totalAmount || inv.amount || 0)
      debtByStudent[studentId].invoiceCount += 1
    })

    topDebtors.value = Object.values(debtByStudent)
      .sort((a, b) => b.amount - a.amount)
      .slice(0, 5)
      .map((debtor, index) => ({
        index: index + 1,
        studentName: debtor.studentName,
        roomNumber: debtor.roomNumber,
        amount: debtor.amount,
        months: debtor.invoiceCount // Approximate months
      }))

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
