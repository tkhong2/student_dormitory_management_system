<template>
  <div>
    <h1 style="font-size: 28px; font-weight: bold; margin-bottom: 4px;">Hợp đồng của tôi</h1>
    <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 24px;">Thông tin hợp đồng và đơn đăng ký thuê phòng KTX</p>

    <!-- Loading State -->
    <a-card v-if="loading" :bordered="true" style="padding: 24px;">
      <a-spin />
      <span style="margin-left: 12px;">Đang tải...</span>
    </a-card>

    <!-- Pending Contracts (Chờ sinh viên chấp thuận) -->
    <a-card v-for="contract in pendingContracts" :key="contract.id" :bordered="true" style="padding: 24px; margin-bottom: 16px;">
      <div style="display: flex; align-items: center; gap: 16px; margin-bottom: 16px;">
        <a-avatar :size="52" style="background-color: #fff4e6; border-radius: 8px;">
          <template #icon><file-text-outlined style="color: #faad14; font-size: 26px;" /></template>
        </a-avatar>
        <div>
          <div style="font-size: 18px; font-weight: bold;">Hợp đồng nháp - Mã {{ contract.contractCode }}</div>
          <a-tag color="orange" style="margin-top: 4px;">Chờ chấp thuận</a-tag>
        </div>
      </div>

      <a-alert type="success" show-icon style="margin-bottom: 16px;">
        <template #message>
          <strong>Chúc mừng!</strong> Đơn đăng ký của bạn đã được duyệt. Vui lòng xem hợp đồng bên dưới và chấp thuận để hoàn tất thủ tục.
        </template>
      </a-alert>

      <a-divider style="margin: 16px 0;" />

      <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 12px;">Thông tin hợp đồng</h4>
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Mã hợp đồng</div>
          <div style="font-size: 16px; font-weight: bold;">{{ contract.contractCode }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Phòng</div>
          <div style="font-size: 16px; font-weight: bold;">{{ contract.roomNumber }} — {{ contract.buildingName }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Loại phòng</div>
          <div style="font-size: 16px; font-weight: bold;">{{ contract.roomTypeName }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Giá thuê</div>
          <div style="font-size: 16px; font-weight: bold; color: #1890ff;">{{ formatCurrency(contract.monthlyRent) }}/tháng</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Tiền cọc</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatCurrency(contract.depositAmount) }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Ngày thanh toán</div>
          <div style="font-size: 16px; font-weight: bold;">Ngày {{ contract.paymentDueDay }} hàng tháng</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Ngày bắt đầu</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatDate(contract.startDate) }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Ngày kết thúc</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatDate(contract.endDate) }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Giá điện/nước</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatCurrency(contract.electricityRate) }} / {{ formatCurrency(contract.waterRate) }}</div>
        </a-col>
      </a-row>

      <a-divider style="margin: 16px 0;" />

      <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 12px;">Điều khoản quan trọng</h4>
      <a-list v-if="contract.terms && contract.terms.length > 0" size="small" :data-source="contract.terms">
        <template #renderItem="{ item: term, index }">
          <a-list-item>
            <a-list-item-meta :title="term.title" :description="term.content">
              <template #avatar>
                <span style="font-size: 20px;">{{ index + 1 }}.</span>
              </template>
            </a-list-item-meta>
          </a-list-item>
        </template>
      </a-list>
      <a-alert v-else type="info" show-icon message="Không có điều khoản nào được áp dụng cho hợp đồng này." />

      <div style="display: flex; gap: 12px; margin-top: 24px;">
        <a-button 
          type="primary"
          size="large"
          @click="acceptContract(contract)"
          :loading="accepting"
          style="background: #52c41a; border-color: #52c41a;"
        >
          <template #icon><check-circle-outlined /></template>
          Tôi chấp thuận hợp đồng này
        </a-button>
        <a-button size="large" danger disabled>
          <template #icon><close-outlined /></template>
          Từ chối
        </a-button>
      </div>
    </a-card>

    <!-- Active Contracts -->
    <a-card v-for="contract in activeContracts" :key="contract.id" :bordered="true" style="padding: 24px; margin-bottom: 16px;">
      <div style="display: flex; align-items: center; gap: 16px; margin-bottom: 24px;">
        <a-avatar :size="52" style="background-color: #dcfce7; border-radius: 8px;">
          <template #icon><file-done-outlined style="color: #52c41a; font-size: 26px;" /></template>
        </a-avatar>
        <div>
          <div style="font-size: 18px; font-weight: bold;">Hợp đồng #{{ contract.contractCode }}</div>
          <a-tag :color="getContractStatusColor(contract.status)" style="margin-top: 4px;">
            {{ getContractStatusLabel(contract.status) }}
          </a-tag>
        </div>
      </div>

      <a-divider style="margin: 20px 0;" />

      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Phòng</div>
          <div style="font-size: 16px; font-weight: bold;">{{ contract.roomNumber }} — {{ contract.buildingName }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Loại phòng</div>
          <div style="font-size: 16px; font-weight: bold;">{{ contract.roomTypeName }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Giá thuê</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatCurrency(contract.monthlyRent) }}/tháng</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Ngày bắt đầu</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatDate(contract.startDate) }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Ngày kết thúc</div>
          <div style="font-size: 16px; font-weight: bold;">{{ formatDate(contract.endDate) }}</div>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 4px;">Tiền cọc</div>
          <div style="font-size: 16px; font-weight: bold;">
            {{ formatCurrency(contract.depositAmount) }}
            <a-tag v-if="contract.isDepositPaid" color="success" style="margin-left: 8px;">Đã đóng</a-tag>
            <a-tag v-else color="orange" style="margin-left: 8px;">Chưa đóng</a-tag>
          </div>
        </a-col>
      </a-row>

      <a-divider style="margin: 20px 0;" />

      <h4 style="font-size: 14px; font-weight: bold; margin-bottom: 12px;">Điều khoản quan trọng</h4>
      <a-list v-if="contract.terms && contract.terms.length > 0" size="small" :data-source="contract.terms">
        <template #renderItem="{ item: term, index }">
          <a-list-item>
            <a-list-item-meta :title="term.title" :description="term.content">
              <template #avatar>
                <span style="font-size: 20px;">{{ index + 1 }}.</span>
              </template>
            </a-list-item-meta>
          </a-list-item>
        </template>
      </a-list>
      <a-alert v-else type="info" show-icon message="Không có điều khoản nào được áp dụng cho hợp đồng này." />

      <div style="display: flex; gap: 12px; margin-top: 24px;">
        <a-button disabled>
          <template #icon><download-outlined /></template>
          Tải hợp đồng PDF
        </a-button>
        <a-button 
          type="primary"
          @click="openExtensionDialog(contract)"
          style="background: #52c41a; border-color: #52c41a;"
        >
          <template #icon><reload-outlined /></template>
          Yêu cầu gia hạn
        </a-button>
        <a-button 
          @click="openTransferDialog(contract)"
          style="background: #faad14; border-color: #faad14; color: white;"
        >
          <template #icon><swap-outlined /></template>
          Yêu cầu chuyển phòng
        </a-button>
      </div>
    </a-card>

    <!-- No Data State -->
    <a-card v-if="!loading && pendingContracts.length === 0 && activeContracts.length === 0" :bordered="true" style="text-align: center; padding: 32px;">
      <file-text-outlined style="font-size: 64px; color: #d9d9d9;" />
      <div style="font-size: 18px; margin-top: 16px;">Chưa có hợp đồng</div>
      <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-top: 8px;">
        Bạn chưa có đơn đăng ký nào được duyệt hoặc hợp đồng nào.
      </p>
      <a-button type="primary" @click="$router.push('/student/rooms')" style="margin-top: 16px;">
        <template #icon><home-outlined /></template>
        Đăng ký phòng
      </a-button>
    </a-card>

    <!-- History Section: Extensions and Transfers -->
    <a-card v-if="activeContracts.length > 0" :bordered="true" style="padding: 24px; margin-top: 16px;">
      <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 16px;">
        <history-outlined style="color: #1890ff; font-size: 28px;" />
        <div style="font-size: 18px; font-weight: bold;">Lịch sử yêu cầu</div>
      </div>

      <a-tabs v-model:activeKey="historyTab">
        <a-tab-pane key="extensions">
          <template #tab>
            <reload-outlined />
            Gia hạn hợp đồng
          </template>

          <a-spin v-if="loadingHistory" style="display: block; text-align: center; padding: 16px;" />
          <a-empty v-else-if="extensions.length === 0" description="Chưa có yêu cầu gia hạn nào" />
          <a-list v-else size="small" :data-source="extensions">
            <template #renderItem="{ item: ext }">
              <a-list-item style="border: 1px solid #e5e7eb; border-radius: 8px; margin-bottom: 8px; padding: 12px;">
                <a-list-item-meta :title="`Hợp đồng ${ext.contractCode}`">
                  <template #avatar>
                    <a-avatar style="background-color: #52c41a;">
                      <template #icon><calendar-outlined /></template>
                    </a-avatar>
                  </template>
                  <template #description>
                    <div style="display: flex; flex-direction: column; gap: 4px;">
                      <div>
                        <close-outlined style="color: #ff4d4f; font-size: 16px;" />
                        Cũ: {{ formatDate(ext.oldEndDate) }}
                      </div>
                      <div>
                        <check-circle-outlined style="color: #52c41a; font-size: 16px;" />
                        Mới: {{ formatDate(ext.newEndDate) }}
                      </div>
                      <div style="font-size: 12px;">
                        {{ formatDateTime(ext.approvedAt) }}
                      </div>
                      <div v-if="ext.reason" style="font-size: 12px;">{{ ext.reason }}</div>
                    </div>
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </a-tab-pane>

        <a-tab-pane key="transfers">
          <template #tab>
            <swap-outlined />
            Chuyển phòng
          </template>

          <a-spin v-if="loadingHistory" style="display: block; text-align: center; padding: 16px;" />
          <a-empty v-else-if="transfers.length === 0" description="Chưa có yêu cầu chuyển phòng nào" />
          <a-list v-else size="small" :data-source="transfers">
            <template #renderItem="{ item: transfer }">
              <a-list-item style="border: 1px solid #e5e7eb; border-radius: 8px; margin-bottom: 8px; padding: 12px;">
                <a-list-item-meta>
                  <template #title>
                    {{ transfer.currentRoomNumber }} 
                    <swap-outlined style="font-size: 16px; margin: 0 4px;" />
                    {{ transfer.newRoomNumber || '?' }}
                  </template>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: getTransferStatusColorHex(transfer.status) }">
                      <template #icon>
                        <component :is="getTransferStatusIconComponent(transfer.status)" />
                      </template>
                    </a-avatar>
                  </template>
                  <template #description>
                    <div style="display: flex; flex-direction: column; gap: 4px;">
                      <div>
                        <a-tag :color="getTransferStatusColor(transfer.status)">
                          {{ getTransferStatusLabel(transfer.status) }}
                        </a-tag>
                      </div>
                      <div v-if="transfer.requestedRoomTypeName || transfer.requestedBuildingName" style="font-size: 12px;">
                        Yêu cầu: {{ transfer.requestedRoomTypeName || '' }} {{ transfer.requestedBuildingName || '' }}
                      </div>
                      <div style="font-size: 12px;">{{ transfer.reason }}</div>
                      <div style="font-size: 12px;">{{ formatDateTime(transfer.createdAt) }}</div>
                      <div v-if="transfer.rejectReason" style="font-size: 12px; color: #ff4d4f;">
                        Lý do từ chối: {{ transfer.rejectReason }}
                      </div>
                      <div v-if="transfer.reviewedByName" style="font-size: 12px;">
                        Xử lý bởi: {{ transfer.reviewedByName }}
                      </div>
                    </div>
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </a-tab-pane>
      </a-tabs>
    </a-card>

    <!-- Extension Request Modal -->
    <a-modal 
      v-model:open="extensionDialog" 
      title="Yêu cầu gia hạn hợp đồng"
      :width="600"
      @ok="submitExtension"
      ok-text="Gửi yêu cầu"
      cancel-text="Hủy"
      :confirm-loading="submitting"
    >
      <div style="margin-top: 16px;">
        <div style="margin-bottom: 16px;">
          <div style="font-size: 14px; font-weight: 600; margin-bottom: 8px;">Thông tin hợp đồng hiện tại</div>
          <a-card :bordered="true" size="small" style="padding: 12px;">
            <div style="display: flex; justify-content: space-between; margin-bottom: 8px;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Mã hợp đồng:</span>
              <span style="font-weight: bold;">{{ selectedContract?.contractCode }}</span>
            </div>
            <div style="display: flex; justify-content: space-between; margin-bottom: 8px;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Phòng:</span>
              <span style="font-weight: bold;">{{ selectedContract?.roomNumber }}</span>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Ngày kết thúc hiện tại:</span>
              <span style="font-weight: bold; color: #ff4d4f;">{{ formatDate(selectedContract?.endDate) }}</span>
            </div>
          </a-card>
        </div>

        <a-form-item label="Ngày kết thúc mới" required :validate-status="extensionErrors.newEndDate ? 'error' : ''" :help="extensionErrors.newEndDate">
          <a-input
            v-model:value="extensionForm.newEndDate"
            type="date"
            :min="getMinExtensionDate()"
          />
        </a-form-item>

        <a-form-item label="Lý do gia hạn" required :validate-status="extensionErrors.reason ? 'error' : ''" :help="extensionErrors.reason">
          <a-textarea
            v-model:value="extensionForm.reason"
            :rows="3"
            placeholder="Nhập lý do bạn muốn gia hạn hợp đồng..."
          />
        </a-form-item>
      </div>
    </a-modal>

    <!-- Transfer Request Modal -->
    <a-modal 
      v-model:open="transferDialog" 
      title="Yêu cầu chuyển phòng"
      :width="600"
      @ok="submitTransfer"
      ok-text="Gửi yêu cầu"
      cancel-text="Hủy"
      :confirm-loading="submitting"
    >
      <div style="margin-top: 16px;">
        <div style="margin-bottom: 16px;">
          <div style="font-size: 14px; font-weight: 600; margin-bottom: 8px;">Thông tin phòng hiện tại</div>
          <a-card :bordered="true" size="small" style="padding: 12px;">
            <div style="display: flex; justify-content: space-between; margin-bottom: 8px;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Phòng:</span>
              <span style="font-weight: bold;">{{ selectedContract?.roomNumber }}</span>
            </div>
            <div style="display: flex; justify-content: space-between; margin-bottom: 8px;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Tòa nhà:</span>
              <span style="font-weight: bold;">{{ selectedContract?.buildingName }}</span>
            </div>
            <div style="display: flex; justify-content: space-between;">
              <span style="font-size: 12px; color: rgba(0,0,0,0.45);">Loại phòng:</span>
              <span style="font-weight: bold;">{{ selectedContract?.roomTypeName }}</span>
            </div>
          </a-card>
        </div>

        <a-form-item label="Tòa nhà mong muốn (tùy chọn)">
          <a-input
            v-model:value="transferForm.requestedBuildingName"
            placeholder="Ví dụ: Nhà A, Nhà B..."
          />
        </a-form-item>

        <a-form-item label="Loại phòng mong muốn (tùy chọn)">
          <a-input
            v-model:value="transferForm.requestedRoomTypeName"
            placeholder="Ví dụ: Phòng 4 người, Phòng 6 người..."
          />
        </a-form-item>

        <a-form-item label="Lý do chuyển phòng" required :validate-status="transferErrors.reason ? 'error' : ''" :help="transferErrors.reason">
          <a-textarea
            v-model:value="transferForm.reason"
            :rows="3"
            placeholder="Nhập lý do bạn muốn chuyển phòng..."
          />
        </a-form-item>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { contractService } from '@/services/contractService'
import { contractExtensionService } from '@/services/contractExtensionService'
import { roomTransferService } from '@/services/roomTransferService'
import { 
  FileTextOutlined,
  FileDoneOutlined,
  CheckCircleOutlined,
  CloseOutlined,
  DownloadOutlined,
  ReloadOutlined,
  SwapOutlined,
  HomeOutlined,
  HistoryOutlined,
  CalendarOutlined,
  ExclamationCircleOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'

const loading = ref(false)
const accepting = ref(false)
const submitting = ref(false)
const loadingHistory = ref(false)
const contracts = ref([])
const extensions = ref([])
const transfers = ref([])
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
      message.error('Không tìm thấy thông tin người dùng')
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
    message.error(err.message || 'Không thể tải dữ liệu hợp đồng')
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
      message.error('Không tìm thấy thông tin người dùng')
      return
    }

    const response = await contractService.acceptContract(contract.id, { userId })
    
    message.success(`Hợp đồng ${contract.contractCode} đã được chấp thuận thành công!`)
    
    // Reload data
    await loadData()
  } catch (err) {
    console.error('Error accepting contract:', err)
    message.error(err.response?.data?.message || err.message || 'Không thể chấp thuận hợp đồng')
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
      message.error('Không tìm thấy thông tin người dùng')
      return
    }

    await contractExtensionService.create({
      contractId: selectedContract.value.id,
      newEndDate: extensionForm.value.newEndDate,
      reason: extensionForm.value.reason.trim(),
      approvedByUserId: userId,
      approvedByName: userName
    })

    message.success('Yêu cầu gia hạn đã được gửi thành công!')
    extensionDialog.value = false
    
    // Reload data
    await loadData()
  } catch (err) {
    console.error('Error submitting extension:', err)
    message.error(err.response?.data?.message || err.message || 'Không thể gửi yêu cầu gia hạn')
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
      message.error('Không tìm thấy thông tin người dùng')
      return
    }

    await roomTransferService.create({
      contractId: selectedContract.value.contractId,
      studentId: selectedContract.value.studentId,
      currentRoomId: selectedContract.value.roomId,
      currentRoomNumber: selectedContract.value.roomNumber,
      requestedBuildingName: transferForm.value.requestedBuildingName.trim() || null,
      requestedRoomTypeName: transferForm.value.requestedRoomTypeName.trim() || null,
      reason: transferForm.value.reason.trim()
    })

    message.success('Yêu cầu chuyển phòng đã được gửi thành công!')
    transferDialog.value = false
  } catch (err) {
    console.error('Error submitting transfer:', err)
    message.error(err.response?.data?.message || err.message || 'Không thể gửi yêu cầu chuyển phòng')
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
    'Pending': 'orange',
    'Active': 'green',
    'Terminated': 'red',
    'Expired': 'default'
  }
  return colors[status] || 'default'
}

function getTransferStatusColor(status) {
  const colors = {
    'Pending': 'orange',
    'Approved': 'green',
    'Rejected': 'red',
    'Completed': 'blue'
  }
  return colors[status] || 'default'
}

function getTransferStatusColorHex(status) {
  const colors = {
    'Pending': '#faad14',
    'Approved': '#52c41a',
    'Rejected': '#ff4d4f',
    'Completed': '#1890ff'
  }
  return colors[status] || '#d9d9d9'
}

function getTransferStatusLabel(status) {
  const labels = {
    'Pending': 'Chờ xử lý',
    'Approved': 'Đã duyệt',
    'Rejected': 'Từ chối',
    'Completed': 'Hoàn thành'
  }
  return labels[status] || status
}

function getTransferStatusIconComponent(status) {
  const icons = {
    'Pending': CalendarOutlined,
    'Approved': CheckCircleOutlined,
    'Rejected': CloseOutlined,
    'Completed': CheckCircleOutlined
  }
  return icons[status] || ExclamationCircleOutlined
}

function formatDateTime(value) {
  if (!value) return ''
  const date = new Date(value)
  return date.toLocaleString('vi-VN')
}

onMounted(() => {
  loadData()
})
</script>
