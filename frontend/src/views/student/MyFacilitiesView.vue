<template>
  <div>
    <!-- Header -->
    <div style="margin-bottom: 24px;">
      <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 8px 0;">Tiện ích chung</h1>
      <p style="font-size: 14px; color: #8c8c8c; margin: 0;">Đặt lịch sử dụng máy giặt, máy sấy, phòng gym và các tiện ích khác</p>
    </div>

    <!-- Tabs -->
    <a-tabs v-model:activeKey="tab">
      <a-tab-pane key="available" tab="Tiện ích có sẵn">
        <template #tab>
          <AppstoreOutlined /> Tiện ích có sẵn
        </template>

        <!-- Filters -->
        <a-card style="margin-bottom: 16px;" :bordered="false">
          <a-row :gutter="16">
            <a-col :xs="24" :sm="8">
              <a-select
                v-model:value="filters.building"
                placeholder="Tòa nhà"
                allow-clear
                style="width: 100%"
                @change="loadUtilities"
              >
                <a-select-option v-for="building in buildings" :key="building.id" :value="building.id">
                  {{ building.name }}
                </a-select-option>
              </a-select>
            </a-col>
            <a-col :xs="24" :sm="8">
              <a-select
                v-model:value="filters.category"
                placeholder="Loại tiện ích"
                allow-clear
                style="width: 100%"
                @change="loadUtilities"
              >
                <a-select-option v-for="cat in categoryOptions" :key="cat.value" :value="cat.value">
                  {{ cat.label }}
                </a-select-option>
              </a-select>
            </a-col>
            <a-col :xs="24" :sm="8">
              <a-input
                v-model:value="filters.search"
                placeholder="Tìm kiếm"
                allow-clear
              >
                <template #prefix><SearchOutlined /></template>
              </a-input>
            </a-col>
          </a-row>
        </a-card>

        <!-- Utilities Grid -->
        <a-row :gutter="[16, 16]">
          <a-col
            v-for="utility in filteredUtilities"
            :key="utility.id"
            :xs="24"
            :sm="12"
            :md="8"
            :lg="6"
          >
            <a-card hoverable class="facility-card">
              <div class="facility-header" :style="{ background: getStatusGradient(utility.status) }">
                <i :class="getCategoryIcon(utility.category)" style="font-size: 48px; color: white;"></i>
                <a-tag
                  class="status-chip"
                  :color="getStatusColor(utility.status)"
                  style="position: absolute; top: 8px; right: 8px;"
                >
                  {{ getStatusLabel(utility.status) }}
                </a-tag>
              </div>

              <div style="padding: 16px;">
                <h3 style="font-size: 16px; font-weight: 600; margin: 0 0 12px 0;">{{ utility.name }}</h3>

                <div class="info-row">
                  <EnvironmentOutlined style="font-size: 14px; margin-right: 4px;" />
                  <span style="font-size: 13px;">{{ utility.buildingName }}</span>
                </div>
                <div class="info-row">
                  <HomeOutlined style="font-size: 14px; margin-right: 4px;" />
                  <span style="font-size: 13px;">{{ utility.location || '-' }}</span>
                </div>
                <div class="info-row">
                  <DollarOutlined style="font-size: 14px; margin-right: 4px;" />
                  <span v-if="utility.feePerUse" style="font-size: 13px; font-weight: 600; color: #52c41a;">
                    {{ formatCurrency(utility.feePerUse) }}
                  </span>
                  <span v-else style="font-size: 13px; color: #52c41a; font-weight: 600;">Miễn phí</span>
                </div>
                <div v-if="utility.operatingHours" class="info-row">
                  <ClockCircleOutlined style="font-size: 14px; margin-right: 4px;" />
                  <span style="font-size: 13px;">{{ utility.operatingHours }}</span>
                </div>

                <a-button
                  block
                  type="primary"
                  :disabled="utility.status !== 'Available'"
                  @click="openBookingDialog(utility)"
                  style="margin-top: 12px;"
                >
                  <CalendarOutlined /> Đặt lịch
                </a-button>
              </div>
            </a-card>
          </a-col>
        </a-row>

        <a-empty v-if="filteredUtilities.length === 0" description="Không tìm thấy tiện ích nào" />
      </a-tab-pane>

      <a-tab-pane key="mybookings">
        <template #tab>
          <ClockCircleOutlined /> Lịch đã đặt ({{ myBookings.length }})
        </template>

        <a-card :bordered="false">
          <a-list v-if="myBookings.length > 0" :data-source="myBookings">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: getBookingStatusColor(item) }" size="large">
                      <i :class="getCategoryIcon(item.utilityCategory)"></i>
                    </a-avatar>
                  </template>
                  <template #title>
                    <strong>{{ item.utilityName }}</strong>
                  </template>
                  <template #description>
                    <div>{{ formatDateTime(item.startTime) }}<span v-if="item.endTime"> - {{ formatTime(item.endTime) }}</span></div>
                    <div style="margin-top: 4px;">
                      <a-tag size="small" :color="getBookingStatusColor(item)">
                        {{ getBookingStatusLabel(item) }}
                      </a-tag>
                      <span v-if="item.feeCharged" style="margin-left: 8px;">Phí: {{ formatCurrency(item.feeCharged) }}</span>
                    </div>
                  </template>
                </a-list-item-meta>
                <template #actions>
                  <a-button
                    v-if="!item.isPaid && item.feeCharged"
                    type="primary"
                    size="small"
                    @click="showPaymentQR(item, { feePerUse: item.feeCharged })"
                  >
                    <DollarOutlined /> Thanh toán
                  </a-button>
                  <a-button
                    v-if="canCancelBooking(item)"
                    danger
                    size="small"
                    @click="cancelBooking(item)"
                  >
                    <DeleteOutlined /> Hủy
                  </a-button>
                </template>
              </a-list-item>
            </template>
          </a-list>

          <a-empty v-else description="Bạn chưa có lịch đặt nào" />
        </a-card>
      </a-tab-pane>

      <a-tab-pane key="history" tab="Lịch sử sử dụng">
        <template #tab>
          <HistoryOutlined /> Lịch sử sử dụng
        </template>

        <a-card :bordered="false">
          <a-table
            :columns="historyHeaders"
            :data-source="usageHistory"
            :loading="loadingHistory"
            :pagination="{ pageSize: 10 }"
            row-key="id"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'startTime'">
                {{ formatDateTime(record.startTime) }}
              </template>
              <template v-else-if="column.key === 'duration'">
                {{ calculateDuration(record) }}
              </template>
              <template v-else-if="column.key === 'feeCharged'">
                {{ record.feeCharged ? formatCurrency(record.feeCharged) : '-' }}
              </template>
              <template v-else-if="column.key === 'isPaid'">
                <a-tag :color="record.isPaid ? 'success' : 'warning'">
                  {{ record.isPaid ? 'Đã thanh toán' : 'Chưa thanh toán' }}
                </a-tag>
              </template>
            </template>
          </a-table>
        </a-card>
      </a-tab-pane>
    </a-tabs>

    <!-- Booking Modal -->
    <a-modal
      v-model:open="bookingDialog"
      title="Đặt lịch sử dụng tiện ích"
      @ok="createBooking"
      @cancel="bookingDialog = false"
      :confirmLoading="submitting"
      width="600px"
    >
      <template #title>
        <CalendarOutlined /> Đặt lịch: {{ selectedUtility?.name }}
      </template>

      <a-form v-if="selectedUtility" layout="vertical">
        <a-form-item label="Ngày" required>
          <a-date-picker
            v-model:value="bookingData.date"
            :disabled-date="disabledDate"
            format="DD/MM/YYYY"
            style="width: 100%"
            placeholder="Chọn ngày"
          />
        </a-form-item>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Giờ bắt đầu" required>
              <a-time-picker
                v-model:value="bookingData.startTime"
                format="HH:mm"
                style="width: 100%"
                placeholder="Chọn giờ"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Giờ kết thúc" required>
              <a-time-picker
                v-model:value="bookingData.endTime"
                format="HH:mm"
                style="width: 100%"
                placeholder="Chọn giờ"
              />
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="Ghi chú (tùy chọn)">
          <a-textarea
            v-model:value="bookingData.notes"
            :rows="2"
            placeholder="Nhập ghi chú nếu có"
          />
        </a-form-item>

        <a-alert
          type="info"
          show-icon
          style="margin-top: 16px;"
        >
          <template #message>
            <strong>Phí dự kiến:</strong> 
            {{ selectedUtility.feePerUse ? formatCurrency(selectedUtility.feePerUse) : 'Miễn phí' }}
          </template>
        </a-alert>
      </a-form>
    </a-modal>

    <!-- Payment QR Modal -->
    <a-modal
      v-model:open="paymentDialog"
      title="Thanh toán tiện ích"
      @cancel="closePaymentDialog"
      width="500px"
      :footer="null"
    >
      <div style="text-align: center; padding: 20px 0;">
        <!-- Auto-check status indicator -->
        <a-alert
          v-if="autoCheckInterval"
          message="Đang tự động kiểm tra thanh toán..."
          type="info"
          show-icon
          style="margin-bottom: 16px;"
        >
          <template #description>
            <div style="display: flex; align-items: center; gap: 8px;">
              <a-spin size="small" />
              <span>Hệ thống đang kiểm tra mỗi 5 giây ({{ checkCount }}/60)</span>
            </div>
          </template>
        </a-alert>

        <h3 style="margin-bottom: 16px;">Quét mã QR để thanh toán</h3>
        
        <div v-if="currentBooking" style="margin-bottom: 20px;">
          <p><strong>Tiện ích:</strong> {{ currentBooking.utilityName }}</p>
          <p><strong>Số tiền:</strong> <span style="color: #f5222d; font-size: 20px; font-weight: 600;">{{ formatCurrency(currentBooking.feeCharged) }}</span></p>
          <p style="font-size: 12px; color: #8c8c8c;">Nội dung: TIENICH {{ currentBooking.id }} {{ authStore.user.studentCode || authStore.user.username }}</p>
        </div>

        <div style="background: white; padding: 20px; border-radius: 8px; display: inline-block;">
          <img :src="paymentQrUrl" alt="QR Payment" style="width: 300px; height: 300px;" />
        </div>

        <a-alert
          type="info"
          show-icon
          style="margin-top: 20px; text-align: left;"
        >
          <template #message>
            <div>
              <p style="margin: 0 0 8px 0;"><strong>Hướng dẫn:</strong></p>
              <ol style="margin: 0; padding-left: 20px;">
                <li>Mở ứng dụng ngân hàng</li>
                <li>Quét mã QR trên</li>
                <li>Kiểm tra thông tin và xác nhận</li>
                <li><strong>Hệ thống sẽ tự động cập nhật sau khi thanh toán (1-5 giây)</strong></li>
              </ol>
              <div v-if="currentBooking" style="margin-top: 12px; padding-top: 12px; border-top: 1px solid #d9d9d9;">
                <p style="margin: 4px 0;"><strong>🏦 Ngân hàng:</strong> Sacombank - Ngân hàng TMCP Sài Gòn Thương Tín</p>
                <p style="margin: 4px 0;"><strong>👤 Chủ tài khoản:</strong> TRAN KHAC HONG</p>
                <p style="margin: 4px 0;"><strong>💳 Số tài khoản:</strong> 025202102005</p>
                <p style="margin: 4px 0;"><strong>✍️ Nội dung:</strong> TIENICH {{ currentBooking.id }} {{ authStore.user.studentCode || authStore.user.username }}</p>
              </div>
            </div>
          </template>
        </a-alert>

        <a-button type="primary" block @click="closePaymentDialog" style="margin-top: 16px;">
          Đã thanh toán
        </a-button>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { message } from 'ant-design-vue'
import { 
  AppstoreOutlined, 
  ClockCircleOutlined, 
  HistoryOutlined,
  SearchOutlined,
  EnvironmentOutlined,
  HomeOutlined,
  DollarOutlined,
  CalendarOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import { useAuthStore } from '@/stores/auth'
import * as utilityService from '@/services/sharedUtilityService'
import { buildingService } from '@/services/buildingService'
import dayjs from 'dayjs'

const authStore = useAuthStore()
const tab = ref('available')

// Data
const utilities = ref([])
const buildings = ref([])
const myBookings = ref([])
const usageHistory = ref([])
const loadingHistory = ref(false)
const selectedUtility = ref(null)
const bookingDialog = ref(false)
const submitting = ref(false)
const paymentDialog = ref(false)
const paymentQrUrl = ref('')
const currentBooking = ref(null)

// Filters
const filters = ref({
  building: null,
  category: null,
  search: ''
})

// Booking form
const bookingData = ref({
  date: null,
  startTime: null,
  endTime: null,
  notes: ''
})

const disabledDate = (current) => {
  return current && current < dayjs().startOf('day')
}

const categoryOptions = utilityService.UTILITY_CATEGORIES.map(c => ({
  value: c.value,
  label: c.label
}))

const filteredUtilities = computed(() => {
  let result = utilities.value.filter(u => u.status === 'Available' || u.status === 'InUse')
  
  if (filters.value.building) {
    result = result.filter(u => u.buildingId === filters.value.building)
  }
  
  if (filters.value.category) {
    result = result.filter(u => u.category === filters.value.category)
  }
  
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    result = result.filter(u => 
      u.name.toLowerCase().includes(search) ||
      u.location?.toLowerCase().includes(search)
    )
  }
  
  return result
})

const historyHeaders = [
  { title: 'Tiện ích', dataIndex: 'utilityName', key: 'utilityName' },
  { title: 'Thời gian', key: 'startTime' },
  { title: 'Thời lượng', key: 'duration' },
  { title: 'Phí', key: 'feeCharged' },
  { title: 'Trạng thái', key: 'isPaid' }
]

onMounted(async () => {
  await Promise.all([
    loadBuildings(),
    loadUtilities(),
    loadMyBookings(),
    loadUsageHistory()
  ])
})

onUnmounted(() => {
  // Clean up auto-check interval when component is unmounted
  stopAutoCheckUtilityPayment()
})

const loadBuildings = async () => {
  try {
    buildings.value = await buildingService.getAll()
  } catch (error) {
    message.error('Lỗi tải danh sách tòa nhà')
  }
}

const loadUtilities = async () => {
  try {
    utilities.value = await utilityService.getAllUtilities()
  } catch (error) {
    message.error('Lỗi tải danh sách tiện ích')
  }
}

const loadMyBookings = async () => {
  const userId = authStore.user.id
  if (!userId) {
    console.warn('User ID not found')
    myBookings.value = []
    return
  }
  try {
    // Tạm thời get tất cả và filter by studentName
    const allLogs = await utilityService.getAllUsageLogs()
    myBookings.value = allLogs.filter(log => 
      log.studentName === authStore.user.fullName &&
      (!log.endTime || dayjs(log.endTime).isAfter(dayjs())) &&
      !log.isPaid // Chỉ hiển thị booking chưa thanh toán
    )
  } catch (error) {
    message.error('Lỗi tải lịch đặt')
  }
}

const loadUsageHistory = async () => {
  loadingHistory.value = true
  const userId = authStore.user.id
  if (!userId) {
    console.warn('User ID not found')
    usageHistory.value = []
    loadingHistory.value = false
    return
  }
  try {
    // Tạm thời get tất cả và filter by studentName
    const allLogs = await utilityService.getAllUsageLogs()
    // Chỉ lấy những lịch đã thanh toán
    usageHistory.value = allLogs.filter(log => 
      log.studentName === authStore.user.fullName &&
      log.endTime &&
      log.isPaid === true // Chỉ hiển thị những lịch đã thanh toán
    )
  } catch (error) {
    message.error('Lỗi tải lịch sử')
  } finally {
    loadingHistory.value = false
  }
}

const openBookingDialog = (utility) => {
  selectedUtility.value = utility
  bookingData.value = {
    date: dayjs(),
    startTime: null,
    endTime: null,
    notes: ''
  }
  bookingDialog.value = true
}

const createBooking = async () => {
  if (!bookingData.value.date || !bookingData.value.startTime || !bookingData.value.endTime) {
    message.error('Vui lòng điền đầy đủ thông tin')
    return
  }
  
  const userId = authStore.user.id
  if (!userId) {
    message.error('Không tìm thấy thông tin người dùng')
    return
  }
  
  submitting.value = true
  try {
    const date = dayjs(bookingData.value.date).format('YYYY-MM-DD')
    const startTime = dayjs(`${date} ${dayjs(bookingData.value.startTime).format('HH:mm')}`).toISOString()
    const endTime = dayjs(`${date} ${dayjs(bookingData.value.endTime).format('HH:mm')}`).toISOString()
    
    const result = await utilityService.createUsageLog({
      sharedUtilityId: selectedUtility.value.id,
      studentId: null,
      studentName: authStore.user.fullName,
      roomNumber: authStore.user.roomNumber || '',
      startTime,
      endTime,
      feeCharged: selectedUtility.value.feePerUse,
      isPaid: false,
      notes: bookingData.value.notes
    })
    
    message.success('Đặt lịch thành công!')
    bookingDialog.value = false
    
    // Nếu có phí thì hiển thị QR thanh toán
    if (selectedUtility.value.feePerUse && selectedUtility.value.feePerUse > 0) {
      showPaymentQR(result, selectedUtility.value)
    }
    
    await loadMyBookings()
  } catch (error) {
    message.error(error.message || 'Đặt lịch thất bại')
  } finally {
    submitting.value = false
  }
}

const cancelBooking = async (booking) => {
  try {
    await utilityService.deleteUsageLog(booking.id)
    message.success('Hủy lịch thành công')
    await loadMyBookings()
  } catch (error) {
    message.error('Lỗi hủy lịch đặt')
  }
}

const showPaymentQR = (booking, utility) => {
  currentBooking.value = booking
  // Tạo QR Code sử dụng VietQR API giống như thanh toán hóa đơn
  // Bank: Sacombank (970403), Account: 025202102005, Owner: TRAN KHAC HONG
  const amount = utility.feePerUse
  const content = `TIENICH ${booking.id} ${authStore.user.studentCode || authStore.user.username}`
  
  const bankCode = '970403' // Sacombank
  const accountNo = '025202102005'
  const accountName = 'TRAN KHAC HONG'
  const template = 'compact2'
  
  // Sử dụng VietQR API giống như thanh toán hóa đơn
  paymentQrUrl.value = `https://img.vietqr.io/image/${bankCode}-${accountNo}-${template}.png?amount=${amount}&addInfo=${encodeURIComponent(content)}&accountName=${encodeURIComponent(accountName)}`
  
  paymentDialog.value = true
  
  // Start auto-check payment
  startAutoCheckUtilityPayment(booking.id)
}

// Auto-check utility payment
const autoCheckInterval = ref(null)
const checkCount = ref(0)

const startAutoCheckUtilityPayment = (usageLogId) => {
  // Clear any existing interval
  stopAutoCheckUtilityPayment()
  
  console.log('🔄 Starting auto-check utility payment every 5 seconds...')
  checkCount.value = 0
  
  autoCheckInterval.value = setInterval(async () => {
    checkCount.value++
    console.log(`⏱️ Auto-check #${checkCount.value} for usageLogId: ${usageLogId}`)
    
    // Stop after 60 checks (5 minutes)
    if (checkCount.value >= 60) {
      console.log('⏹️ Stopping auto-check after 5 minutes')
      stopAutoCheckUtilityPayment()
      return
    }
    
    await checkUtilityPaymentStatusSilent(usageLogId)
  }, 5000) // Check every 5 seconds
}

const stopAutoCheckUtilityPayment = () => {
  if (autoCheckInterval.value) {
    clearInterval(autoCheckInterval.value)
    autoCheckInterval.value = null
    console.log('⏹️ Auto-check utility payment stopped')
  }
}

const checkUtilityPaymentStatusSilent = async (usageLogId) => {
  try {
    const { API_URLS, getAuthHeaders } = await import('@/services/apiService')
    const axios = (await import('axios')).default
    
    console.log(`[Auto-check] Checking payment for usageLogId: ${usageLogId}`)
    console.log(`[Auto-check] API URL: ${API_URLS.SEPAY}/check-utility-payment`)
    
    // Call check payment API
    const response = await axios.post(`${API_URLS.SEPAY}/check-utility-payment`, {
      usageLogId: usageLogId
    }, {
      headers: getAuthHeaders()
    })
    
    console.log('[Auto-check] Response:', response.data)
    
    const result = response.data
    
    if (result.isPaid) {
      // Stop auto-check
      stopAutoCheckUtilityPayment()
      
      // Show success notification
      message.success({
        content: '🎉 Thanh toán thành công!',
        duration: 5
      })
      
      // Close modal after 2 seconds
      setTimeout(() => {
        paymentDialog.value = false
      }, 2000)
      
      // Reload bookings and history
      await Promise.all([loadMyBookings(), loadUsageHistory()])
      
      console.log('✅ Utility payment successful!')
    } else {
      console.log('⏳ Payment not completed yet')
    }
  } catch (error) {
    console.error('[Auto-check] Error:', error)
    console.error('[Auto-check] Error response:', error.response?.data)
    // Don't show error message for auto-check to avoid spamming user
  }
}

const closePaymentDialog = async () => {
  stopAutoCheckUtilityPayment()
  paymentDialog.value = false
  currentBooking.value = null
  // Reload để check trạng thái thanh toán
  await Promise.all([loadMyBookings(), loadUsageHistory()])
}

const canCancelBooking = (booking) => {
  return dayjs(booking.startTime).isAfter(dayjs())
}

const getBookingStatusColor = (booking) => {
  if (booking.endTime && dayjs(booking.endTime).isBefore(dayjs())) {
    return '#8c8c8c'
  }
  if (dayjs(booking.startTime).isBefore(dayjs()) && dayjs(booking.endTime).isAfter(dayjs())) {
    return '#52c41a'
  }
  return '#1890ff'
}

const getBookingStatusLabel = (booking) => {
  if (booking.endTime && dayjs(booking.endTime).isBefore(dayjs())) {
    return 'Đã hoàn thành'
  }
  if (dayjs(booking.startTime).isBefore(dayjs()) && dayjs(booking.endTime).isAfter(dayjs())) {
    return 'Đang sử dụng'
  }
  return 'Đã đặt'
}

const getCategoryIcon = (category) => {
  const cat = utilityService.UTILITY_CATEGORIES.find(c => c.value === category)
  return cat?.faIcon || 'fas fa-box'
}

const getStatusLabel = (status) => {
  const labels = {
    Available: 'Sẵn sàng',
    InUse: 'Đang dùng',
    Broken: 'Hỏng',
    UnderMaintenance: 'Bảo trì'
  }
  return labels[status] || status
}

const getStatusColor = (status) => {
  const colors = {
    Available: 'success',
    InUse: 'processing',
    Broken: 'error',
    UnderMaintenance: 'warning'
  }
  return colors[status] || 'default'
}

const getStatusGradient = (status) => {
  const gradients = {
    Available: 'linear-gradient(135deg, #11998e 0%, #38ef7d 100%)',
    InUse: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    Broken: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    UnderMaintenance: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)'
  }
  return gradients[status] || 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

const formatDateTime = (date) => {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

const formatTime = (date) => {
  return dayjs(date).format('HH:mm')
}

const calculateDuration = (item) => {
  if (!item.endTime) return '-'
  const minutes = dayjs(item.endTime).diff(dayjs(item.startTime), 'minute')
  if (minutes < 60) return `${minutes} phút`
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  return `${hours}h${mins > 0 ? ` ${mins}p` : ''}`
}
</script>

<style scoped>
.facility-card {
  height: 100%;
  overflow: hidden;
}

.facility-header {
  padding: 24px;
  text-align: center;
  position: relative;
  min-height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.info-row {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  color: rgba(0, 0, 0, 0.65);
}
</style>
