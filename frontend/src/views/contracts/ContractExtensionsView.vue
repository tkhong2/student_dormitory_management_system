<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">Quản lý Gia hạn Hợp đồng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">Xem và phê duyệt yêu cầu gia hạn hợp đồng thuê phòng</p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Tạo gia hạn
      </a-button>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px;" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="10">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo mã hợp đồng, sinh viên..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-range-picker
            v-model:value="dateRange"
            format="DD/MM/YYYY"
            :placeholder="['Từ ngày', 'Đến ngày']"
            style="width: 100%"
          />
        </a-col>
        <a-col :xs="24" :sm="12" :md="6" v-if="search || dateRange?.length">
          <a-button @click="resetFilters" block>
            <template #icon><ReloadOutlined /></template>
            Đặt lại
          </a-button>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
        :columns="columns"
        :data-source="filteredExtensions"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} gia hạn` }"
        :scroll="{ x: 1000 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'contract'">
            <div>
              <div style="font-weight: 600;">{{ record.contractCode }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">
                {{ record.studentName }} ({{ record.studentCode }})
              </div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'oldEndDate'">
            <div style="text-align: center;">
              <div>{{ formatDate(record.oldEndDate) }}</div>
              <ArrowDownOutlined style="color: #1890ff; margin: 4px 0;" />
              <div style="font-weight: 700; color: #52c41a;">
                {{ formatDate(record.newEndDate) }}
              </div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'duration'">
            <a-tag color="blue">{{ calculateDuration(record.oldEndDate, record.newEndDate) }} tháng</a-tag>
          </template>
          
          <template v-else-if="column.key === 'approvedBy'">
            <div>
              <div style="font-weight: 500;">{{ record.approvedByName }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ formatDate(record.approvedAt) }}</div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button type="link" size="small" @click="viewDetail(record)">Chi tiết</a-button>
              <a-button type="text" danger size="small" @click="confirmDelete(record)">
                <template #icon><DeleteOutlined /></template>
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create Extension Modal -->
    <a-modal
      v-model:open="createDialog"
      title="Tạo gia hạn hợp đồng"
      width="600px"
      @cancel="createDialog = false"
    >
      <a-form layout="vertical">
        <a-form-item 
          label="Hợp đồng"
          :validate-status="createErrors.contractId ? 'error' : ''"
          :help="createErrors.contractId"
        >
          <a-select
            v-model:value="createForm.contractId"
            placeholder="Chọn hợp đồng"
            show-search
            :filter-option="filterContract"
            @change="onContractChange"
          >
            <a-select-option v-for="c in activeContracts" :key="c.id" :value="c.id">
              {{ c.displayText }}
            </a-select-option>
          </a-select>
        </a-form-item>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Ngày kết thúc cũ">
              <a-input v-model:value="createForm.oldEndDate" disabled />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Ngày kết thúc mới"
              :validate-status="createErrors.newEndDate ? 'error' : ''"
              :help="createErrors.newEndDate"
            >
              <a-date-picker
                v-model:value="createForm.newEndDate"
                format="DD/MM/YYYY"
                style="width: 100%"
                placeholder="Chọn ngày"
                :disabled-date="disabledDate"
              />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-form-item label="Lý do gia hạn">
          <a-textarea
            v-model:value="createForm.reason"
            :rows="3"
            placeholder="Nhập lý do gia hạn"
          />
        </a-form-item>
        
        <a-form-item 
          label="Người phê duyệt"
          :validate-status="createErrors.approvedByName ? 'error' : ''"
          :help="createErrors.approvedByName"
        >
          <a-input v-model:value="createForm.approvedByName" placeholder="Nhập tên người phê duyệt" />
        </a-form-item>
      </a-form>
      
      <template #footer>
        <a-button @click="createDialog = false" :disabled="saving">Hủy</a-button>
        <a-button type="primary" @click="handleCreate" :loading="saving">Tạo gia hạn</a-button>
      </template>
    </a-modal>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết gia hạn"
      width="560px"
      @cancel="detailDialog = false"
      :footer="null"
    >
      <a-descriptions bordered :column="2" v-if="detailTarget">
        <a-descriptions-item label="Mã hợp đồng" :span="2">
          <strong>{{ detailTarget.contractCode }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Sinh viên" :span="2">
          <strong>{{ detailTarget.studentName }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc cũ">
          {{ formatDate(detailTarget.oldEndDate) }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kết thúc mới">
          <span style="color: #52c41a; font-weight: 700;">
            {{ formatDate(detailTarget.newEndDate) }}
          </span>
        </a-descriptions-item>
        <a-descriptions-item label="Thời gian gia hạn" :span="2">
          <a-tag color="blue">{{ calculateDuration(detailTarget.oldEndDate, detailTarget.newEndDate) }} tháng</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Lý do" :span="2">
          {{ detailTarget.reason || 'Không có' }}
        </a-descriptions-item>
        <a-descriptions-item label="Người phê duyệt">
          <strong>{{ detailTarget.approvedByName }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày phê duyệt">
          {{ formatDateTime(detailTarget.approvedAt) }}
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>

    <!-- Delete Confirmation Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      @ok="deleteExtension"
      @cancel="deleteDialog = false"
      okText="Xóa"
      okType="danger"
      cancelText="Hủy"
      :confirmLoading="saving"
    >
      <p>Bạn có chắc muốn xóa bản ghi gia hạn này?</p>
    </a-modal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import {
  SearchOutlined,
  ReloadOutlined,
  ArrowDownOutlined,
  DeleteOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import dayjs from 'dayjs'
import { contractExtensionService } from '@/services/contractExtensionService'
import { contractService } from '@/services/contractService'

const columns = [
  { title: 'Hợp đồng', key: 'contract', width: 240 },
  { title: 'Thời gian gia hạn', key: 'oldEndDate', align: 'center', width: 180 },
  { title: 'Kéo dài', key: 'duration', align: 'center', width: 100 },
  { title: 'Người phê duyệt', key: 'approvedBy', width: 200 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 150, fixed: 'right' }
]

const extensions = ref([])
const activeContracts = ref([])
const search = ref('')
const dateRange = ref(null)
const loading = ref(false)
const saving = ref(false)
const error = ref(null)
const createDialog = ref(false)
const detailDialog = ref(false)
const deleteDialog = ref(false)
const detailTarget = ref(null)
const deleteTarget = ref(null)

const createForm = ref({
  contractId: null,
  oldEndDate: '',
  newEndDate: null,
  reason: '',
  approvedByName: 'Admin'
})

const createErrors = ref({})

const filteredExtensions = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  let result = extensions.value
  
  if (keyword) {
    result = result.filter((item) =>
      item.contractCode?.toLowerCase().includes(keyword) ||
      item.studentName?.toLowerCase().includes(keyword)
    )
  }
  
  if (dateRange.value?.length === 2) {
    const [start, end] = dateRange.value
    result = result.filter((item) => {
      const itemDate = dayjs(item.approvedAt)
      return itemDate.isAfter(start) && itemDate.isBefore(end.add(1, 'day'))
    })
  }
  
  return result
})

function resetFilters() {
  search.value = ''
  dateRange.value = null
}

async function loadExtensions() {
  loading.value = true
  error.value = null
  try {
    extensions.value = await contractExtensionService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách gia hạn'
    message.error(error.value)
  } finally {
    loading.value = false
  }
}

async function loadActiveContracts() {
  try {
    const allContracts = await contractService.getAll()
    activeContracts.value = allContracts
      .filter((c) => c.status === 'Active')
      .map((c) => ({
        ...c,
        displayText: `${c.code} - ${c.studentName} (${c.studentCode})`
      }))
  } catch (err) {
    console.error('Không thể tải danh sách hợp đồng', err)
  }
}

function filterContract(input, option) {
  return option.children[0].children.toLowerCase().includes(input.toLowerCase())
}

function openCreate() {
  createForm.value = {
    contractId: null,
    oldEndDate: '',
    newEndDate: null,
    reason: '',
    approvedByName: 'Admin'
  }
  createErrors.value = {}
  createDialog.value = true
}

function onContractChange(contractId) {
  const contract = activeContracts.value.find((c) => c.id === contractId)
  if (contract) {
    createForm.value.oldEndDate = contract.endDate ? dayjs(contract.endDate).format('DD/MM/YYYY') : ''
  }
}

function disabledDate(current) {
  if (!createForm.value.oldEndDate) return false
  const oldDate = dayjs(createForm.value.oldEndDate, 'DD/MM/YYYY')
  return current && current.isBefore(oldDate, 'day')
}

async function handleCreate() {
  const errors = {}
  if (!createForm.value.contractId) errors.contractId = 'Vui lòng chọn hợp đồng'
  if (!createForm.value.newEndDate) errors.newEndDate = 'Vui lòng chọn ngày kết thúc mới'
  if (!createForm.value.approvedByName.trim()) errors.approvedByName = 'Vui lòng nhập người phê duyệt'
  
  createErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await contractExtensionService.create({
      contractId: createForm.value.contractId,
      newEndDate: createForm.value.newEndDate.format('YYYY-MM-DD'),
      reason: createForm.value.reason?.trim() || null,
      approvedByUserId: 1,
      approvedByName: createForm.value.approvedByName.trim()
    })
    message.success('Tạo gia hạn thành công')
    createDialog.value = false
    await loadExtensions()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  } finally {
    saving.value = false
  }
}

function viewDetail(item) {
  detailTarget.value = item
  detailDialog.value = true
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function deleteExtension() {
  saving.value = true
  try {
    await contractExtensionService.delete(deleteTarget.value.id)
    message.success('Đã xóa gia hạn')
    deleteDialog.value = false
    await loadExtensions()
  } catch (err) {
    message.error(err.message || 'Không thể xóa')
  } finally {
    saving.value = false
  }
}

function calculateDuration(oldDate, newDate) {
  const start = dayjs(oldDate)
  const end = dayjs(newDate)
  return end.diff(start, 'month')
}

function formatDate(value) {
  return value ? dayjs(value).format('DD/MM/YYYY') : ''
}

function formatDateTime(value) {
  return value ? dayjs(value).format('DD/MM/YYYY HH:mm') : ''
}

onMounted(() => {
  loadExtensions()
  loadActiveContracts()
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
