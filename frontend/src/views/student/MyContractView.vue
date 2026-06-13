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
      <v-list v-if="contract.terms && contract.terms.length > 0" density="compact">
        <v-list-item 
          v-for="(term, index) in contract.terms" 
          :key="term.id"
          :title="term.title"
          :subtitle="term.content"
          :prepend-icon="term.icon || `mdi-numeric-${index + 1}-circle`"
        />
      </v-list>
      <v-alert v-else type="info" variant="tonal" density="compact">
        Không có điều khoản nào được áp dụng cho hợp đồng này.
      </v-alert>

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
    <v-card v-for="contract in activeContracts" :key="contract.id" class="pa-6 mb-4" style="border:1px solid #e5e7eb">
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
      <v-list v-if="contract.terms && contract.terms.length > 0" density="compact">
        <v-list-item 
          v-for="(term, index) in contract.terms" 
          :key="term.id"
          :title="term.title"
          :subtitle="term.content"
          :prepend-icon="term.icon || `mdi-numeric-${index + 1}-circle`"
        />
      </v-list>
      <v-alert v-else type="info" variant="tonal" density="compact">
        Không có điều khoản nào được áp dụng cho hợp đồng này.
      </v-alert>

      <div class="d-flex ga-3 mt-6">
        <v-btn prepend-icon="mdi-download" variant="tonal" color="primary" disabled>Tải hợp đồng PDF</v-btn>
        <v-btn 
          prepend-icon="mdi-refresh" 
          variant="tonal" 
          color="success"
          @click="openExtensionDialog(contract)"
        >
          Yêu cầu gia hạn
        </v-btn>
        <v-btn 
          prepend-icon="mdi-swap-horizontal" 
          variant="tonal" 
          color="warning"
          @click="openTransferDialog(contract)"
        >
          Yêu cầu chuyển phòng
        </v-btn>
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

    <!-- History Section: Extensions and Transfers -->
    <v-card v-if="activeContracts.length > 0" class="pa-6 mt-4" style="border:1px solid #e5e7eb">
      <div class="d-flex align-center ga-3 mb-4">
        <v-icon color="primary" size="28">mdi-history</v-icon>
        <div class="text-h6 font-weight-bold">Lịch sử yêu cầu</div>
      </div>

      <v-tabs v-model="historyTab" color="primary" class="mb-4">
        <v-tab value="extensions">
          <v-icon start>mdi-refresh</v-icon>
          Gia hạn hợp đồng
        </v-tab>
        <v-tab value="transfers">
          <v-icon start>mdi-swap-horizontal</v-icon>
          Chuyển phòng
        </v-tab>
      </v-tabs>

      <v-window v-model="historyTab">
        <!-- Extensions History -->
        <v-window-item value="extensions">
          <v-card v-if="loadingHistory" class="pa-4 text-center">
            <v-progress-circular indeterminate color="primary" size="32" />
          </v-card>
          <div v-else-if="extensions.length === 0" class="text-center pa-6 text-medium-emphasis">
            <v-icon size="48" color="grey-lighten-1">mdi-clipboard-text-outline</v-icon>
            <div class="mt-2">Chưa có yêu cầu gia hạn nào</div>
          </div>
          <v-list v-else density="compact">
            <v-list-item
              v-for="ext in extensions"
              :key="ext.id"
              class="mb-2"
              style="border: 1px solid #e5e7eb; border-radius: 8px;"
            >
              <template #prepend>
                <v-avatar color="success" size="40">
                  <v-icon>mdi-calendar-check</v-icon>
                </v-avatar>
              </template>
              <v-list-item-title class="font-weight-bold">
                Hợp đồng {{ ext.contractCode }}
              </v-list-item-title>
              <v-list-item-subtitle>
                <div class="d-flex flex-column ga-1 mt-1">
                  <div>
                    <v-icon size="16" color="error">mdi-calendar-remove</v-icon>
                    Cũ: {{ formatDate(ext.oldEndDate) }}
                  </div>
                  <div>
                    <v-icon size="16" color="success">mdi-calendar-plus</v-icon>
                    Mới: {{ formatDate(ext.newEndDate) }}
                  </div>
                  <div class="text-caption">
                    <v-icon size="14">mdi-clock-outline</v-icon>
                    {{ formatDateTime(ext.approvedAt) }}
                  </div>
                  <div v-if="ext.reason" class="text-caption">
                    <v-icon size="14">mdi-note-text</v-icon>
                    {{ ext.reason }}
                  </div>
                </div>
              </v-list-item-subtitle>
            </v-list-item>
          </v-list>
        </v-window-item>

        <!-- Transfers History -->
        <v-window-item value="transfers">
          <v-card v-if="loadingHistory" class="pa-4 text-center">
            <v-progress-circular indeterminate color="primary" size="32" />
          </v-card>
          <div v-else-if="transfers.length === 0" class="text-center pa-6 text-medium-emphasis">
            <v-icon size="48" color="grey-lighten-1">mdi-clipboard-text-outline</v-icon>
            <div class="mt-2">Chưa có yêu cầu chuyển phòng nào</div>
          </div>
          <v-list v-else density="compact">
            <v-list-item
              v-for="transfer in transfers"
              :key="transfer.id"
              class="mb-2"
              style="border: 1px solid #e5e7eb; border-radius: 8px;"
            >
              <template #prepend>
                <v-avatar :color="getTransferStatusColor(transfer.status)" size="40">
                  <v-icon>{{ getTransferStatusIcon(transfer.status) }}</v-icon>
                </v-avatar>
              </template>
              <v-list-item-title class="font-weight-bold">
                {{ transfer.currentRoomNumber }} 
                <v-icon size="16">mdi-arrow-right</v-icon>
                {{ transfer.newRoomNumber || '?' }}
              </v-list-item-title>
              <v-list-item-subtitle>
                <div class="d-flex flex-column ga-1 mt-1">
                  <div>
                    <v-chip size="x-small" :color="getTransferStatusColor(transfer.status)">
                      {{ getTransferStatusLabel(transfer.status) }}
                    </v-chip>
                  </div>
                  <div v-if="transfer.requestedRoomTypeName || transfer.requestedBuildingName" class="text-caption">
                    <v-icon size="14">mdi-home-search</v-icon>
                    Yêu cầu: {{ transfer.requestedRoomTypeName || '' }} {{ transfer.requestedBuildingName || '' }}
                  </div>
                  <div class="text-caption">
                    <v-icon size="14">mdi-note-text</v-icon>
                    {{ transfer.reason }}
                  </div>
                  <div class="text-caption">
                    <v-icon size="14">mdi-clock-outline</v-icon>
                    {{ formatDateTime(transfer.createdAt) }}
                  </div>
                  <div v-if="transfer.rejectReason" class="text-caption text-error">
                    <v-icon size="14" color="error">mdi-close-circle</v-icon>
                    Lý do từ chối: {{ transfer.rejectReason }}
                  </div>
                  <div v-if="transfer.reviewedByName" class="text-caption">
                    <v-icon size="14">mdi-account</v-icon>
                    Xử lý bởi: {{ transfer.reviewedByName }}
                  </div>
                </div>
              </v-list-item-subtitle>
            </v-list-item>
          </v-list>
        </v-window-item>
      </v-window>
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
          {{ successMessage }}
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="primary" @click="successDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Extension Request Dialog -->
    <v-dialog v-model="extensionDialog" max-width="600">
      <v-card>
        <v-card-title class="bg-primary">
          <v-icon class="mr-2">mdi-refresh</v-icon>
          Yêu cầu gia hạn hợp đồng
        </v-card-title>
        <v-card-text class="pt-4">
          <div class="mb-4">
            <div class="text-subtitle-2 mb-2">Thông tin hợp đồng hiện tại</div>
            <v-card variant="outlined" class="pa-3">
              <div class="d-flex justify-space-between mb-2">
                <span class="text-caption text-medium-emphasis">Mã hợp đồng:</span>
                <span class="font-weight-bold">{{ selectedContract?.contractCode }}</span>
              </div>
              <div class="d-flex justify-space-between mb-2">
                <span class="text-caption text-medium-emphasis">Phòng:</span>
                <span class="font-weight-bold">{{ selectedContract?.roomNumber }}</span>
              </div>
              <div class="d-flex justify-space-between">
                <span class="text-caption text-medium-emphasis">Ngày kết thúc hiện tại:</span>
                <span class="font-weight-bold text-error">{{ formatDate(selectedContract?.endDate) }}</span>
              </div>
            </v-card>
          </div>

          <v-text-field
            v-model="extensionForm.newEndDate"
            label="Ngày kết thúc mới"
            type="date"
            variant="outlined"
            :min="getMinExtensionDate()"
            :error-messages="extensionErrors.newEndDate"
            class="mb-3"
          />

          <v-textarea
            v-model="extensionForm.reason"
            label="Lý do gia hạn"
            variant="outlined"
            rows="3"
            :error-messages="extensionErrors.reason"
            placeholder="Nhập lý do bạn muốn gia hạn hợp đồng..."
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="extensionDialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="submitExtension" :loading="submitting">
            Gửi yêu cầu
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Transfer Request Dialog -->
    <v-dialog v-model="transferDialog" max-width="600">
      <v-card>
        <v-card-title class="bg-warning">
          <v-icon class="mr-2">mdi-swap-horizontal</v-icon>
          Yêu cầu chuyển phòng
        </v-card-title>
        <v-card-text class="pt-4">
          <div class="mb-4">
            <div class="text-subtitle-2 mb-2">Thông tin phòng hiện tại</div>
            <v-card variant="outlined" class="pa-3">
              <div class="d-flex justify-space-between mb-2">
                <span class="text-caption text-medium-emphasis">Phòng:</span>
                <span class="font-weight-bold">{{ selectedContract?.roomNumber }}</span>
              </div>
              <div class="d-flex justify-space-between mb-2">
                <span class="text-caption text-medium-emphasis">Tòa nhà:</span>
                <span class="font-weight-bold">{{ selectedContract?.buildingName }}</span>
              </div>
              <div class="d-flex justify-space-between">
                <span class="text-caption text-medium-emphasis">Loại phòng:</span>
                <span class="font-weight-bold">{{ selectedContract?.roomTypeName }}</span>
              </div>
            </v-card>
          </div>

          <v-text-field
            v-model="transferForm.requestedBuildingName"
            label="Tòa nhà mong muốn (tùy chọn)"
            variant="outlined"
            placeholder="Ví dụ: Nhà A, Nhà B..."
            class="mb-3"
          />

          <v-text-field
            v-model="transferForm.requestedRoomTypeName"
            label="Loại phòng mong muốn (tùy chọn)"
            variant="outlined"
            placeholder="Ví dụ: Phòng 4 người, Phòng 6 người..."
            class="mb-3"
          />

          <v-textarea
            v-model="transferForm.reason"
            label="Lý do chuyển phòng"
            variant="outlined"
            rows="3"
            :error-messages="transferErrors.reason"
            placeholder="Nhập lý do bạn muốn chuyển phòng..."
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn @click="transferDialog = false">Hủy</v-btn>
          <v-btn color="warning" @click="submitTransfer" :loading="submitting">
            Gửi yêu cầu
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { contractService } from '@/services/contractService'
import { contractExtensionService } from '@/services/contractExtensionService'
import { roomTransferService } from '@/services/roomTransferService'

const loading = ref(false)
const accepting = ref(false)
const submitting = ref(false)
const loadingHistory = ref(false)
const contracts = ref([])
const extensions = ref([])
const transfers = ref([])
const errorDialog = ref(false)
const errorMessage = ref('')
const successDialog = ref(false)
const successMessage = ref('')
const newContractCode = ref('')
const historyTab = ref('extensions')

// Extension Dialog
const extensionDialog = ref(false)
const selectedContract = ref(null)
const extensionForm = ref({
  newEndDate: '',
  reason: ''
})
const extensionErrors = ref({})

// Transfer Dialog
const transferDialog = ref(false)
const transferForm = ref({
  requestedBuildingName: '',
  requestedRoomTypeName: '',
  reason: ''
})
const transferErrors = ref({})

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

    // Load history for active contracts
    if (contracts.value.length > 0) {
      loadHistory()
    }
  } catch (err) {
    console.error('Error loading contracts:', err)
    errorMessage.value = err.message || 'Không thể tải dữ liệu hợp đồng'
    errorDialog.value = true
  } finally {
    loading.value = false
  }
}

async function loadHistory() {
  loadingHistory.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    if (!userId) return

    // Get student info from contracts
    const studentId = contracts.value[0]?.studentId

    if (studentId) {
      // Load extensions for all contracts
      const allExtensions = []
      for (const contract of contracts.value) {
        try {
          const contractExtensions = await contractExtensionService.getByContractId(contract.id)
          allExtensions.push(...contractExtensions)
        } catch (err) {
          console.error(`Error loading extensions for contract ${contract.id}:`, err)
        }
      }
      extensions.value = allExtensions.sort((a, b) => 
        new Date(b.approvedAt) - new Date(a.approvedAt)
      )

      // Load room transfers
      try {
        const allTransfers = await roomTransferService.getByStudentId(studentId)
        transfers.value = allTransfers.sort((a, b) => 
          new Date(b.createdAt) - new Date(a.createdAt)
        )
      } catch (err) {
        console.error('Error loading transfers:', err)
      }
    }
  } catch (err) {
    console.error('Error loading history:', err)
  } finally {
    loadingHistory.value = false
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
    successMessage.value = `Hợp đồng ${contract.contractCode} đã được chấp thuận thành công!`
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

function openExtensionDialog(contract) {
  selectedContract.value = contract
  extensionForm.value = {
    newEndDate: '',
    reason: ''
  }
  extensionErrors.value = {}
  extensionDialog.value = true
}

function getMinExtensionDate() {
  if (!selectedContract.value?.endDate) return ''
  const endDate = new Date(selectedContract.value.endDate)
  endDate.setDate(endDate.getDate() + 1)
  return endDate.toISOString().split('T')[0]
}

async function submitExtension() {
  const errors = {}
  
  if (!extensionForm.value.newEndDate) {
    errors.newEndDate = 'Vui lòng chọn ngày kết thúc mới'
  } else {
    const newDate = new Date(extensionForm.value.newEndDate)
    const currentEndDate = new Date(selectedContract.value.endDate)
    if (newDate <= currentEndDate) {
      errors.newEndDate = 'Ngày kết thúc mới phải sau ngày kết thúc hiện tại'
    }
  }
  
  if (!extensionForm.value.reason.trim()) {
    errors.reason = 'Vui lòng nhập lý do gia hạn'
  }

  extensionErrors.value = errors
  if (Object.keys(errors).length > 0) return

  submitting.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id
    const userName = user.fullName || user.username

    if (!userId) {
      errorMessage.value = 'Không tìm thấy thông tin người dùng'
      errorDialog.value = true
      return
    }

    await contractExtensionService.create({
      contractId: selectedContract.value.id,
      newEndDate: extensionForm.value.newEndDate,
      reason: extensionForm.value.reason.trim(),
      approvedByUserId: userId,
      approvedByName: userName
    })

    successMessage.value = 'Yêu cầu gia hạn đã được gửi thành công!'
    successDialog.value = true
    extensionDialog.value = false
    
    // Reload data
    await loadData()
  } catch (err) {
    console.error('Error submitting extension:', err)
    errorMessage.value = err.response?.data?.message || err.message || 'Không thể gửi yêu cầu gia hạn'
    errorDialog.value = true
  } finally {
    submitting.value = false
  }
}

function openTransferDialog(contract) {
  selectedContract.value = contract
  transferForm.value = {
    requestedBuildingName: '',
    requestedRoomTypeName: '',
    reason: ''
  }
  transferErrors.value = {}
  transferDialog.value = true
}

async function submitTransfer() {
  const errors = {}
  
  if (!transferForm.value.reason.trim()) {
    errors.reason = 'Vui lòng nhập lý do chuyển phòng'
  }

  transferErrors.value = errors
  if (Object.keys(errors).length > 0) return

  submitting.value = true
  try {
    const user = JSON.parse(localStorage.getItem('user') || '{}')
    const userId = user.id

    if (!userId) {
      errorMessage.value = 'Không tìm thấy thông tin người dùng'
      errorDialog.value = true
      return
    }

    await roomTransferService.create({
      contractId: selectedContract.value.id,
      studentId: selectedContract.value.studentId,
      currentRoomId: selectedContract.value.roomId,
      currentRoomNumber: selectedContract.value.roomNumber,
      requestedBuildingName: transferForm.value.requestedBuildingName.trim() || null,
      requestedRoomTypeName: transferForm.value.requestedRoomTypeName.trim() || null,
      reason: transferForm.value.reason.trim()
    })

    successMessage.value = 'Yêu cầu chuyển phòng đã được gửi thành công!'
    successDialog.value = true
    transferDialog.value = false
  } catch (err) {
    console.error('Error submitting transfer:', err)
    errorMessage.value = err.response?.data?.message || err.message || 'Không thể gửi yêu cầu chuyển phòng'
    errorDialog.value = true
  } finally {
    submitting.value = false
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
