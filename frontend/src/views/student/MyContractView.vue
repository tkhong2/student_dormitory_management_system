<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Hợp đồng của tôi</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Thông tin hợp đồng và đơn đăng ký thuê phòng KTX</p>

    <!-- Loading State -->
    <v-card v-if="loading" class="pa-6">
      <v-progress-circular indeterminate color="primary" />
      <span class="ml-3">Đang tải...</span>
    </v-card>

    <!-- Pending Contracts (Chờ sinh viên chấp thuận) -->
    <v-card v-for="contract in pendingContracts" :key="contract.id" class="pa-6 mb-4" style="border:1px solid #e5e7eb">
      <div class="d-flex align-center ga-4 mb-4">
        <v-avatar color="#fff4e6" size="52" rounded="lg">
          <v-icon color="warning" size="26">mdi-file-document-check</v-icon>
        </v-avatar>
        <div>
          <div class="text-h6 font-weight-bold">Hợp đồng nháp - Mã {{ contract.contractCode }}</div>
          <v-chip color="warning" size="small" variant="tonal" class="mt-1">Chờ chấp thuận</v-chip>
        </div>
      </div>

      <v-alert type="success" variant="tonal" class="mb-4">
        <strong>Chúc mừng!</strong> Đơn đăng ký của bạn đã được duyệt. Vui lòng xem hợp đồng bên dưới và chấp thuận để hoàn tất thủ tục.
      </v-alert>

      <v-divider class="mb-4" />

      <h4 class="text-subtitle-2 font-weight-bold mb-3">Thông tin hợp đồng</h4>
      <v-row>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Mã hợp đồng</div>
          <div class="text-body-1 font-weight-bold">{{ contract.contractCode }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Phòng</div>
          <div class="text-body-1 font-weight-bold">{{ contract.roomNumber }} — {{ contract.buildingName }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Loại phòng</div>
          <div class="text-body-1 font-weight-bold">{{ contract.roomTypeName }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Giá thuê</div>
          <div class="text-body-1 font-weight-bold text-primary">{{ formatCurrency(contract.monthlyRent) }}/tháng</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Tiền cọc</div>
          <div class="text-body-1 font-weight-bold">{{ formatCurrency(contract.depositAmount) }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Ngày thanh toán</div>
          <div class="text-body-1 font-weight-bold">Ngày {{ contract.paymentDueDay }} hàng tháng</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Ngày bắt đầu</div>
          <div class="text-body-1 font-weight-bold">{{ formatDate(contract.startDate) }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Ngày kết thúc</div>
          <div class="text-body-1 font-weight-bold">{{ formatDate(contract.endDate) }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Giá điện/nước</div>
          <div class="text-body-1 font-weight-bold">{{ formatCurrency(contract.electricityRate) }} / {{ formatCurrency(contract.waterRate) }}</div>
        </v-col>
      </v-row>

      <v-divider class="my-4" />

      <h4 class="text-subtitle-2 font-weight-bold mb-3">Điều khoản quan trọng</h4>
      <v-list density="compact">
        <v-list-item title="Thanh toán tiền phòng trước ngày 5 hàng tháng" prepend-icon="mdi-numeric-1-circle" />
        <v-list-item title="Giữ gìn tài sản chung, bồi thường nếu hư hỏng" prepend-icon="mdi-numeric-2-circle" />
        <v-list-item title="Chấm dứt hợp đồng phải báo trước 30 ngày" prepend-icon="mdi-numeric-3-circle" />
        <v-list-item title="Tuân thủ nội quy KTX do Ban Quản lý ban hành" prepend-icon="mdi-numeric-4-circle" />
      </v-list>

      <div class="d-flex ga-3 mt-6">
        <v-btn 
          prepend-icon="mdi-check-circle" 
          color="success" 
          size="large"
          @click="acceptContract(contract)"
          :loading="accepting"
        >
          Tôi chấp thuận hợp đồng này
        </v-btn>
        <v-btn prepend-icon="mdi-close" variant="outlined" color="error" size="large" disabled>
          Từ chối
        </v-btn>
      </div>
    </v-card>

    <!-- Active Contracts -->
    <v-card v-for="contract in contracts" :key="contract.id" class="pa-6 mb-4" style="border:1px solid #e5e7eb">
      <div class="d-flex align-center ga-4 mb-6">
        <v-avatar color="#dcfce7" size="52" rounded="lg">
          <v-icon color="success" size="26">mdi-file-sign</v-icon>
        </v-avatar>
        <div>
          <div class="text-h6 font-weight-bold">Hợp đồng #{{ contract.contractCode }}</div>
          <v-chip :color="getContractStatusColor(contract.status)" size="small" variant="tonal" class="mt-1">
            {{ getContractStatusLabel(contract.status) }}
          </v-chip>
        </div>
      </div>

      <v-divider class="mb-5" />

      <v-row>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Phòng</div>
          <div class="text-body-1 font-weight-bold">{{ contract.roomNumber }} — {{ contract.buildingName }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Loại phòng</div>
          <div class="text-body-1 font-weight-bold">{{ contract.roomTypeName }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Giá thuê</div>
          <div class="text-body-1 font-weight-bold">{{ formatCurrency(contract.monthlyRent) }}/tháng</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Ngày bắt đầu</div>
          <div class="text-body-1 font-weight-bold">{{ formatDate(contract.startDate) }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Ngày kết thúc</div>
          <div class="text-body-1 font-weight-bold">{{ formatDate(contract.endDate) }}</div>
        </v-col>
        <v-col cols="12" sm="6" md="4">
          <div class="text-caption text-medium-emphasis mb-1">Tiền cọc</div>
          <div class="text-body-1 font-weight-bold">
            {{ formatCurrency(contract.depositAmount) }}
            <v-chip v-if="contract.isDepositPaid" size="x-small" color="success" class="ml-2">Đã đóng</v-chip>
            <v-chip v-else size="x-small" color="warning" class="ml-2">Chưa đóng</v-chip>
          </div>
        </v-col>
      </v-row>

      <v-divider class="my-5" />

      <h4 class="text-subtitle-2 font-weight-bold mb-3">Điều khoản quan trọng</h4>
      <v-list density="compact">
        <v-list-item title="Thanh toán tiền phòng trước ngày 5 hàng tháng" prepend-icon="mdi-numeric-1-circle" />
        <v-list-item title="Giữ gìn tài sản chung, bồi thường nếu hư hỏng" prepend-icon="mdi-numeric-2-circle" />
        <v-list-item title="Chấm dứt hợp đồng phải báo trước 30 ngày" prepend-icon="mdi-numeric-3-circle" />
        <v-list-item title="Tuân thủ nội quy KTX do Ban Quản lý ban hành" prepend-icon="mdi-numeric-4-circle" />
      </v-list>

      <div class="d-flex ga-3 mt-6">
        <v-btn prepend-icon="mdi-download" variant="tonal" color="primary" disabled>Tải hợp đồng PDF</v-btn>
        <v-btn prepend-icon="mdi-refresh" variant="tonal" disabled>Yêu cầu gia hạn</v-btn>
      </div>
    </v-card>

    <!-- No Data State -->
    <v-card v-if="!loading && pendingContracts.length === 0 && activeContracts.length === 0" class="pa-8 text-center">
      <v-icon size="64" color="grey-lighten-1">mdi-file-document-outline</v-icon>
      <div class="text-h6 mt-4">Chưa có hợp đồng</div>
      <p class="text-body-2 text-medium-emphasis mt-2">
        Bạn chưa có đơn đăng ký nào được duyệt hoặc hợp đồng nào.
      </p>
      <v-btn prepend-icon="mdi-home-search" color="primary" to="/student/rooms" class="mt-4">
        Đăng ký phòng
      </v-btn>
    </v-card>

    <!-- Error Dialog -->
    <v-dialog v-model="errorDialog" max-width="500">
      <v-card>
        <v-card-title class="bg-error">
          <v-icon class="mr-2">mdi-alert-circle</v-icon>
          Lỗi
        </v-card-title>
        <v-card-text class="pt-4">
          {{ errorMessage }}
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="primary" @click="errorDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success Dialog -->
    <v-dialog v-model="successDialog" max-width="500">
      <v-card>
        <v-card-title class="bg-success">
          <v-icon class="mr-2">mdi-check-circle</v-icon>
          Thành công
        </v-card-title>
        <v-card-text class="pt-4">
          Hợp đồng {{ newContractCode }} đã được chấp thuận thành công!
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="primary" @click="successDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { contractService } from '@/services/contractService'

const loading = ref(false)
const accepting = ref(false)
const contracts = ref([])
const errorDialog = ref(false)
const errorMessage = ref('')
const successDialog = ref(false)
const newContractCode = ref('')

const pendingContracts = computed(() => {
  return contracts.value.filter(c => c.status === 'Pending')
})

const activeContracts = computed(() => {
  return contracts.value.filter(c => c.status === 'Active' || c.status === 'Terminated' || c.status === 'Expired')
})

async function loadData() {
  loading.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    if (!userId) {
      errorMessage.value = 'Không tìm thấy thông tin người dùng'
      errorDialog.value = true
      return
    }

    // Load contracts by userId
    contracts.value = await contractService.getByUserId(userId)
    console.log('Loaded contracts:', contracts.value)
  } catch (err) {
    console.error('Error loading contracts:', err)
    errorMessage.value = err.message || 'Không thể tải dữ liệu hợp đồng'
    errorDialog.value = true
  } finally {
    loading.value = false
  }
}

async function acceptContract(contract) {
  accepting.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    if (!userId) {
      errorMessage.value = 'Không tìm thấy thông tin người dùng'
      errorDialog.value = true
      return
    }

    const response = await contractService.acceptContract(contract.id, { userId })
    
    newContractCode.value = contract.contractCode
    successDialog.value = true
    
    // Reload data
    await loadData()
  } catch (err) {
    console.error('Error accepting contract:', err)
    errorMessage.value = err.response?.data?.message || err.message || 'Không thể chấp thuận hợp đồng'
    errorDialog.value = true
  } finally {
    accepting.value = false
  }
}

function formatDate(value) {
  if (!value) return ''
  const date = new Date(value)
  return date.toLocaleDateString('vi-VN')
}

function formatCurrency(value) {
  if (!value) return '0đ'
  return value.toLocaleString('vi-VN') + 'đ'
}

function getContractStatusLabel(status) {
  const labels = {
    'Pending': 'Chờ ký',
    'Active': 'Đang hiệu lực',
    'Terminated': 'Đã chấm dứt',
    'Expired': 'Đã hết hạn'
  }
  return labels[status] || status
}

function getContractStatusColor(status) {
  const colors = {
    'Pending': 'warning',
    'Active': 'success',
    'Terminated': 'error',
    'Expired': 'grey'
  }
  return colors[status] || 'grey'
}

onMounted(() => {
  loadData()
})
</script>
