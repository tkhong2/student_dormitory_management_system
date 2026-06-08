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

const loading = ref(false)

const stats = ref({
  pendingApplications: 12,
  applicationsOver48h: 3,
  maintenanceRequests: 8,
  urgentMaintenance: 2,
  todayPayments: 15000000,
  paymentCount: 18,
  overdueStudents: 23,
  totalDebt: 45200000,
  occupancyRate: 85,
  totalRooms: 400,
  occupiedRooms: 340,
  availableRooms: 50,
  maintenanceRooms: 10
})

const alerts = ref([
  {
    id: 1,
    status: 'error',
    urgent: true,
    title: 'Đơn đăng ký chờ quá 48h',
    description: '3 đơn đăng ký chờ xử lý quá 48 giờ, cần duyệt ngay',
    action: 'applications'
  },
  {
    id: 2,
    status: 'error',
    urgent: true,
    title: 'Yêu cầu bảo trì khẩn cấp',
    description: '2 yêu cầu bảo trì mức độ khẩn cấp chưa phân công',
    action: 'maintenance'
  },
  {
    id: 3,
    status: 'warning',
    urgent: false,
    title: 'Hợp đồng sắp hết hạn',
    description: '15 hợp đồng sẽ hết hạn trong 7 ngày tới',
    action: 'contracts'
  },
  {
    id: 4,
    status: 'warning',
    urgent: false,
    title: 'Sinh viên nợ quá 2 tháng',
    description: '5 sinh viên nợ tiền phòng quá 2 tháng, cần liên hệ',
    action: 'debt'
  }
])

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const refreshData = async () => {
  loading.value = true
  try {
    // TODO: Call APIs to get real data
    await new Promise(resolve => setTimeout(resolve, 1000))
    message.success('Đã làm mới dữ liệu')
  } catch (error) {
    message.error('Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

const handleAlert = (item) => {
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
