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
        <StatCardAnt v-bind="s" />
      </a-col>
    </a-row>

    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <!-- Charts -->
      <a-col :xs="24" :lg="16">
        <a-card :bordered="true" style="border-radius: 12px;">
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
        <a-card :bordered="true" style="border-radius: 12px; height: 100%;">
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
import { ref } from 'vue'
import { 
  Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, 
  PointElement, LineElement, ArcElement 
} from 'chart.js'
import { Bar, Line } from 'vue-chartjs'
import { DownloadOutlined, UserAddOutlined, CalculatorOutlined, FileTextOutlined } from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import StatCardAnt from '@/components/common/StatCardAnt.vue'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, PointElement, LineElement, ArcElement)

const chartPeriod = ref('month')

const stats = [
  { label:'Tổng sinh viên', value:'1,248', icon:'mdi-school-outline', color:'primary', trend: 12 },
  { label:'Phòng trống', value:'84', icon:'mdi-door-open', color:'info', trend: -3 },
  { label:'Chưa thanh toán', value:'45.2M₫', icon:'mdi-cash-clock', color:'warning', trend: 8 },
  { label:'Yêu cầu bảo trì', value:'18', icon:'mdi-wrench-clock', color:'error', trend: 22 },
]

// Bar Chart Data
const barData = {
  labels: ['Tòa A1', 'Tòa A2', 'Tòa B1', 'Tòa B2', 'Tòa C1'],
  datasets: [{
    label: 'Tỷ lệ lấp đầy (%)',
    backgroundColor: '#ff6b00',
    data: [90, 96, 78, 69, 82],
    borderRadius: 8
  }]
}

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

// Line Chart Data
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

const activities = [
  { id:1, text:'SV Nguyễn Văn An đăng ký phòng 101-A1', time:'5 phút trước', icon:'mdi-account-plus', color:'success' },
  { id:2, text:'Phòng 202-B2 báo hỏng vòi nước', time:'2 giờ trước', icon:'mdi-wrench', color:'error' },
  { id:3, text:'SV Trần Thị Bình thanh toán hóa đơn T5', time:'3 giờ trước', icon:'mdi-cash-check', color:'info' },
  { id:4, text:'Hợp đồng #HD-045 sắp hết hạn', time:'6 giờ trước', icon:'mdi-file-alert', color:'warning' },
  { id:5, text:'Sửa chữa phòng 305-C1 hoàn tất', time:'Hôm qua', icon:'mdi-check-circle', color:'success' },
]

const getTimelineColor = (color) => {
  const colorMap = {
    'success': 'green',
    'error': 'red',
    'info': 'blue',
    'warning': 'orange',
  }
  return colorMap[color] || 'blue'
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
