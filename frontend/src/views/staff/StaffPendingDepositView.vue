<template>
  <div>
    <PageHeaderAnt title="Xác nhận đóng cọc" subtitle="Danh sách hợp đồng chờ sinh viên đóng tiền cọc">
      <template #actions>
        <a-space>
          <a-input-search
            v-model:value="searchText"
            placeholder="Tìm theo tên, mã SV..."
            style="width: 300px"
            @search="handleSearch"
          />
          <a-button @click="loadContracts">
            <template #icon><ReloadOutlined /></template>
          </a-button>
        </a-space>
      </template>
    </PageHeaderAnt>

    <!-- Stats -->
    <a-row :gutter="16" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="12" :md="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Chờ đóng cọc" :value="contracts.length" :valueStyle="{ color: '#faad14' }">
            <template #prefix><ClockCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card size="small" :bordered="false">
          <a-statistic 
            title="Tổng tiền cọc" 
            :value="totalDepositAmount" 
            :valueStyle="{ color: '#1890ff' }"
            suffix="VNĐ"
          >
            <template #prefix><DollarOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card size="small" :bordered="false">
          <a-statistic 
            title="Quá hạn 7 ngày" 
            :value="overdueCount" 
            :valueStyle="{ color: '#ff4d4f' }"
          >
            <template #prefix><WarningOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card size="small" :bordered="false">
          <a-statistic 
            title="Đã xác nhận hôm nay" 
            :value="confirmedTodayCount" 
            :valueStyle="{ color: '#52c41a' }"
          >
            <template #prefix><CheckCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Table -->
    <a-card :bordered="false">
      <a-table
        :columns="columns"
        :data-source="filteredContracts"
        :loading="loading"
        :pagination="{ pageSize: 15, showTotal: (total) => `Tổng ${total} hợp đồng` }"
        :row-key="record => record.id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <div style="font-weight: 600;">{{ record.studentName }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'contract'">
            <div>
              <a-tag color="blue">{{ record.contractCode }}</a-tag>
              <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
                {{ formatDate(record.startDate) }} - {{ formatDate(record.endDate) }}
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'room'">
            <div>
              <div style="font-weight: 500;">{{ record.roomNumber }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.buildingName }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'deposit'">
            <div style="font-weight: 600; color: #1890ff;">
              {{ formatCurrency(record.depositAmount) }} VNĐ
            </div>
          </template>

          <template v-else-if="column.key === 'createdAt'">
            <div>
              {{ formatDate(record.createdAt) }}
              <div 
                v-if="getDaysWaiting(record.createdAt) >= 7" 
                style="font-size: 11px; color: #ff4d4f; margin-top: 4px;"
              >
                <WarningOutlined /> {{ getDaysWaiting(record.createdAt) }} ngày
              </div>
            </div>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="viewDetail(record)">
                <template #icon><EyeOutlined /></template>
                Xem
              </a-button>
              <a-button 
                size="small" 
                type="primary"
                @click="openConfirmModal(record)"
              >
                <template #icon><CheckOutlined /></template>
                Xác nhận
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailModalVisible"
      title="Chi tiết hợp đồng"
      width="800px"
      :footer="null"
    >
      <a-descriptions v-if="selectedContract" bordered :column="2" size="small">
        <a-descriptions-item label="Mã hợp đồng" :span="2">
          <strong>{{ selectedContract.contractCode }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Sinh viên">
          {{ selectedContract.studentName }}
        </a-descriptions-item>
        <a-descriptions-item label="Mã SV">
          {{ selectedContract.studentCode }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          {{ selectedContract.roomNumber }}
        </a-descriptions-item>
        <a-descriptions-item label="Tòa nhà">
          {{ selectedContract.buildingName }}
        </a-descriptions-item>
        <a-descriptions-item label="Loại phòng">
          {{ selectedContract.roomTypeName }}
        </a-descriptions-item>
        <a-descriptions-item label="Tiền phòng/tháng">
          {{ formatCurrency(selectedContract.monthlyRent) }} VNĐ
        </a-descriptions-item>
        <a-descriptions-item label="Tiền cọc" :span="2">
          <strong style="font-size: 16px; color: #1890ff;">
            {{ formatCurrency(selectedContract.depositAmount) }} VNĐ
          </strong>
        </a-descriptions-item>
        <a-descriptions-item label="Thời hạn">
          {{ formatDate(selectedContract.startDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Đến ngày">
          {{ formatDate(selectedContract.endDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày tạo">
          {{ formatDate(selectedContract.createdAt) }}
        </a-descriptions-item>
        <a-descriptions-item label="Thời gian chờ">
          {{ getDaysWaiting(selectedContract.createdAt) }} ngày
        </a-descriptions-item>
        <a-descriptions-item label="Ghi chú" :span="2">
          {{ selectedContract.notes || 'Không có' }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="detailModalVisible = false">Đóng</a-button>
        <a-button type="primary" @click="openConfirmModal(selectedContract)">
          Xác nhận đã đóng cọc
        </a-button>
      </div>
    </a-modal>

    <!-- Confirm Deposit Modal -->
    <a-modal
      v-model:open="confirmModalVisible"
      title="Xác nhận đóng tiền cọc"
      @ok="handleConfirmDeposit"
      :confirmLoading="confirming"
      okText="Xác nhận"
      cancelText="Hủy"
    >
      <a-alert
        message="Xác nhận sinh viên đã đóng tiền cọc"
        description="Sau khi xác nhận, hợp đồng sẽ được kích hoạt và sinh viên có thể nhận phòng."
        type="info"
        show-icon
        style="margin-bottom: 16px;"
      />

      <a-descriptions v-if="selectedContract" bordered size="small">
        <a-descriptions-item label="Sinh viên">
          {{ selectedContract.studentName }}
        </a-descriptions-item>
        <a-descriptions-item label="Hợp đồng">
          {{ selectedContract.contractCode }}
        </a-descriptions-item>
        <a-descriptions-item label="Tiền cọc" :span="2">
          <strong style="font-size: 16px; color: #1890ff;">
            {{ formatCurrency(selectedContract.depositAmount) }} VNĐ
          </strong>
        </a-descriptions-item>
      </a-descriptions>

      <a-divider />

      <a-form layout="vertical">
        <a-form-item label="Ghi chú (nếu có)">
          <a-textarea
            v-model:value="confirmNote"
            :rows="2"
            placeholder="Ghi chú về việc thu cọc..."
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  ReloadOutlined, ClockCircleOutlined, DollarOutlined, WarningOutlined,
  CheckCircleOutlined, EyeOutlined, CheckOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { useAuthStore } from '@/stores/auth'
import dayjs from 'dayjs'
import api from '@/services/api'

const authStore = useAuthStore()
const loading = ref(false)
const confirming = ref(false)
const searchText = ref('')
const contracts = ref([])
const detailModalVisible = ref(false)
const confirmModalVisible = ref(false)
const selectedContract = ref(null)
const confirmNote = ref('')
const confirmedTodayCount = ref(0)

const columns = [
  { title: 'Sinh viên', key: 'student', width: 180 },
  { title: 'Hợp đồng', key: 'contract', width: 150 },
  { title: 'Phòng', key: 'room', width: 120 },
  { title: 'Tiền cọc', key: 'deposit', width: 120 },
  { title: 'Ngày tạo', key: 'createdAt', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 180, fixed: 'right' }
]

const filteredContracts = computed(() => {
  if (!searchText.value) return contracts.value
  
  const search = searchText.value.toLowerCase()
  return contracts.value.filter(c =>
    c.studentName?.toLowerCase().includes(search) ||
    c.studentCode?.toLowerCase().includes(search) ||
    c.contractCode?.toLowerCase().includes(search)
  )
})

const totalDepositAmount = computed(() => {
  return contracts.value.reduce((sum, c) => sum + (c.depositAmount || 0), 0)
})

const overdueCount = computed(() => {
  return contracts.value.filter(c => getDaysWaiting(c.createdAt) >= 7).length
})

const loadContracts = async () => {
  loading.value = true
  try {
    const response = await api.get('/contracts/status/PendingDeposit')
    contracts.value = response || []
    
    // Load confirmed today count (mock for now)
    confirmedTodayCount.value = 0
  } catch (error) {
    console.error('Error loading contracts:', error)
    message.error('Lỗi tải danh sách hợp đồng')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  // Search is reactive via computed
}

const viewDetail = (contract) => {
  selectedContract.value = contract
  confirmModalVisible.value = false
  detailModalVisible.value = true
}

const openConfirmModal = (contract) => {
  selectedContract.value = contract
  confirmNote.value = ''
  detailModalVisible.value = false
  confirmModalVisible.value = true
}

const handleConfirmDeposit = async () => {
  if (!selectedContract.value) return

  confirming.value = true
  try {
    await api.post(`/contracts/${selectedContract.value.id}/confirm-deposit`, {
      confirmedByUserId: authStore.user.id,
      confirmedByName: authStore.user.fullName
    })

    message.success('Đã xác nhận đóng cọc thành công! Hợp đồng đã được kích hoạt.')
    confirmModalVisible.value = false
    await loadContracts()
    confirmedTodayCount.value++
  } catch (error) {
    console.error('Error confirming deposit:', error)
    message.error(error.message || 'Lỗi xác nhận đóng cọc')
  } finally {
    confirming.value = false
  }
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value || 0)
}

const getDaysWaiting = (createdAt) => {
  return dayjs().diff(dayjs(createdAt), 'day')
}

onMounted(() => {
  loadContracts()
})
</script>
