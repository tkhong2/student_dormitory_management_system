<template>
  <v-container fluid :class="$vuetify.display.mobile ? 'pa-3' : 'pa-4'">
    <PageHeader title="Dashboard" subtitle="Tổng quan hoạt động ký túc xá hôm nay">
      <template #actions>
        <v-btn 
          prepend-icon="mdi-download" 
          variant="tonal" 
          color="primary" 
          class="rounded-lg font-weight-bold"
          :size="$vuetify.display.mobile ? 'small' : 'default'"
        >
          Xuất báo cáo
        </v-btn>
      </template>
    </PageHeader>

    <!-- Stat cards -->
    <v-row :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'">
      <v-col v-for="s in stats" :key="s.label" cols="12" sm="6" lg="3">
        <StatCard v-bind="s" />
      </v-col>
    </v-row>

    <v-row :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'">
      <!-- Occupancy chart -->
      <v-col cols="12" lg="8">
        <DataCard :class="$vuetify.display.mobile ? 'pa-4' : 'pa-6'" style="height: 100%;">
          <div class="d-flex flex-column flex-sm-row justify-space-between align-start align-sm-center" :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'" style="gap: 16px;">
            <h3 :class="$vuetify.display.mobile ? 'text-subtitle-1' : 'text-h6'" class="font-weight-bold">Thống kê tỷ lệ lấp đầy & Doanh thu</h3>
            <v-btn-toggle 
              v-model="chartPeriod" 
              density="compact" 
              color="primary" 
              variant="outlined" 
              divided 
              mandatory 
              class="rounded-lg"
            >
              <v-btn value="week" :size="$vuetify.display.mobile ? 'x-small' : 'small'">Tuần</v-btn>
              <v-btn value="month" :size="$vuetify.display.mobile ? 'x-small' : 'small'">Tháng</v-btn>
              <v-btn value="year" :size="$vuetify.display.mobile ? 'x-small' : 'small'">Năm</v-btn>
            </v-btn-toggle>
          </div>
          
          <v-row>
            <v-col cols="12" md="6">
              <div :class="$vuetify.display.mobile ? 'text-caption' : 'text-overline'" class="mb-4" style="opacity: 0.7;">Tỷ lệ lấp đầy (%)</div>
              <div :style="{ height: $vuetify.display.mobile ? '200px' : '300px', position: 'relative' }">
                <Bar :data="barData" :options="barOptions" />
              </div>
            </v-col>
            <v-col cols="12" md="6">
              <div :class="$vuetify.display.mobile ? 'text-caption' : 'text-overline'" class="mb-4" style="opacity: 0.7;">Xu hướng doanh thu</div>
              <div :style="{ height: $vuetify.display.mobile ? '200px' : '300px', position: 'relative' }">
                <Line :data="lineData" :options="lineOptions" />
              </div>
            </v-col>
          </v-row>
        </DataCard>
      </v-col>

      <!-- Recent activity -->
      <v-col cols="12" lg="4">
        <DataCard :class="$vuetify.display.mobile ? 'pa-4' : 'pa-6'" style="height: 100%;">
          <h3 :class="$vuetify.display.mobile ? 'text-subtitle-1' : 'text-h6'" class="font-weight-bold" :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'">Hoạt động gần đây</h3>
          <div class="d-flex flex-column" :style="{ gap: $vuetify.display.mobile ? '16px' : '24px' }">
            <div v-for="a in activities" :key="a.id" class="d-flex align-start" style="gap: 16px;">
              <v-avatar 
                :color="a.color" 
                :size="$vuetify.display.mobile ? 36 : 40" 
                variant="tonal" 
                class="rounded-lg flex-shrink-0"
              >
                <v-icon :size="$vuetify.display.mobile ? 18 : 20">{{ a.icon }}</v-icon>
              </v-avatar>
              <div style="min-width: 0; flex: 1;">
                <div :class="$vuetify.display.mobile ? 'text-caption' : 'text-body-2'" class="font-weight-bold text-truncate">{{ a.text }}</div>
                <div class="text-caption text-medium-emphasis mt-1">{{ a.time }}</div>
              </div>
            </div>
          </div>
        </DataCard>
      </v-col>
    </v-row>

    <!-- Quick actions -->
    <v-row>
      <v-col cols="12" md="4">
        <v-card :class="$vuetify.display.mobile ? 'pa-6' : 'pa-8'" class="rounded-xl text-white elevation-4" style="background-color: #ff6b00; position: relative; overflow: hidden;">
          <v-icon :size="$vuetify.display.mobile ? 48 : 64" :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'" style="opacity: 0.3; position: absolute; right: 16px; top: 16px;">mdi-account-plus</v-icon>
          <h3 :class="$vuetify.display.mobile ? 'text-subtitle-1' : 'text-h5'" class="font-weight-bold mb-2">Tiếp nhận SV mới</h3>
          <p :class="$vuetify.display.mobile ? 'text-caption mb-4' : 'text-body-2 mb-6'" style="opacity: 0.9;">Thêm hồ sơ sinh viên và xếp phòng nhanh chóng trong vài bước.</p>
          <v-btn 
            color="white" 
            variant="flat" 
            class="font-weight-bold rounded-lg px-6" 
            :size="$vuetify.display.mobile ? 'small' : 'default'"
            style="color: #ff6b00;"
          >
            Bắt đầu
          </v-btn>
        </v-card>
      </v-col>
      
      <v-col cols="12" md="4">
        <v-card :class="$vuetify.display.mobile ? 'pa-6' : 'pa-8'" class="rounded-xl text-white elevation-4" style="background-color: #2196F3; position: relative; overflow: hidden;">
          <v-icon :size="$vuetify.display.mobile ? 48 : 64" :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'" style="opacity: 0.3; position: absolute; right: 16px; top: 16px;">mdi-calculator-variant</v-icon>
          <h3 :class="$vuetify.display.mobile ? 'text-subtitle-1' : 'text-h5'" class="font-weight-bold mb-2">Tính lệ phí tháng</h3>
          <p :class="$vuetify.display.mobile ? 'text-caption mb-4' : 'text-body-2 mb-6'" style="opacity: 0.9;">Tự động tạo hóa đơn dịch vụ cho tất cả sinh viên đang ở.</p>
          <v-btn 
            color="white" 
            variant="flat" 
            class="font-weight-bold rounded-lg px-6" 
            :size="$vuetify.display.mobile ? 'small' : 'default'"
            style="color: #2196F3;"
          >
            Tạo hóa đơn
          </v-btn>
        </v-card>
      </v-col>

      <v-col cols="12" md="4">
        <v-card :class="$vuetify.display.mobile ? 'pa-6' : 'pa-8'" class="rounded-xl text-white elevation-4" style="background-color: #4CAF50; position: relative; overflow: hidden;">
          <v-icon :size="$vuetify.display.mobile ? 48 : 64" :class="$vuetify.display.mobile ? 'mb-4' : 'mb-6'" style="opacity: 0.3; position: absolute; right: 16px; top: 16px;">mdi-file-chart</v-icon>
          <h3 :class="$vuetify.display.mobile ? 'text-subtitle-1' : 'text-h5'" class="font-weight-bold mb-2">Xuất báo cáo</h3>
          <p :class="$vuetify.display.mobile ? 'text-caption mb-4' : 'text-body-2 mb-6'" style="opacity: 0.9;">Tổng hợp số liệu hoạt động KTX theo tháng/quý/năm.</p>
          <v-btn 
            color="white" 
            variant="flat" 
            class="font-weight-bold rounded-lg px-6" 
            :size="$vuetify.display.mobile ? 'small' : 'default'"
            style="color: #4CAF50;"
          >
            Xem báo cáo
          </v-btn>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import { 
  Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, 
  PointElement, LineElement, ArcElement 
} from 'chart.js'
import { Bar, Line } from 'vue-chartjs'
import PageHeader from '@/components/common/PageHeader.vue'
import StatCard from '@/components/common/StatCard.vue'
import DataCard from '@/components/common/DataCard.vue'

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
</script>

<style scoped>
.v-card { transition: all 0.3s; }
.v-btn { text-transform: none; }
</style>
