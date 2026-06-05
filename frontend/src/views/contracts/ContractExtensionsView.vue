<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Gia hạn Hợp đồng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Xem và phê duyệt yêu cầu gia hạn hợp đồng thuê phòng
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Tạo gia hạn
      </a-button>
    </div>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="extensions"
      :treatEmptyAsError="false"
      @retry="loadExtensions"
    >
      <a-card
        style="border: 1px solid #e5e7eb; background: #fafafa"
        :body-style="{ padding: '0' }"
      >
        <div class="pa-4 d-flex flex-wrap align-center" style="gap: 12px">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo mã hợp đồng, sinh viên..."
            allowClear
            style="max-width: 350px; flex: 1"
          />
          <a-range-picker
            v-model:value="dateRange"
            format="DD/MM/YYYY"
            placeholder="['Từ ngày', 'Đến ngày']"
            style="max-width: 260px"
          />
          <a-button
            v-if="search || dateRange?.length"
            type="text"
            size="small"
            @click="resetFilters"
          >
            Đặt lại
          </a-button>
        </div>

        <a-table
          :columns="columns"
          :data-source="filteredExtensions"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'contract'">
              <div>
                <div class="font-weight-bold">{{ record.contractCode }}</div>
                <div style="font-size: 12px; color: #8c8c8c">
                  {{ record.studentName }} ({{ record.studentCode }})
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'oldEndDate'">
              <div style="text-align: center">
                <div>{{ formatDate(record.oldEndDate) }}</div>
                <v-icon size="16" color="primary">mdi-arrow-down</v-icon>
                <div class="font-weight-bold" style="color: #4caf50">
                  {{ formatDate(record.newEndDate) }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'duration'">
              {{ calculateDuration(record.oldEndDate, record.newEndDate) }} tháng
            </template>
            <template v-else-if="column.key === 'approvedBy'">
              <div>
                <div class="font-weight-medium">{{ record.approvedByName }}</div>
                <div style="font-size: 12px; color: #8c8c8c">
                  {{ formatDate(record.approvedAt) }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'actions'">
              <a-space size="small">
                <a-button type="link" @click="viewDetail(record)">Chi tiết</a-button>
                <a-button type="text" danger @click="confirmDelete(record)">Xóa</a-button>
              </a-space>
            </template>
          </template>
        </a-table>
      </a-card>
    </DataStatus>

    <!-- Create Extension Dialog -->
    <v-dialog v-model="createDialog" max-width="600" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Tạo gia hạn hợp đồng</h2>
        <v-row>
          <v-col cols="12">
            <v-select
              v-model="createForm.contractId"
              label="Hợp đồng *"
              :items="activeContracts"
              item-title="displayText"
              item-value="id"
              :error-messages="createErrors.contractId"
              @update:model-value="onContractChange"
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="createForm.oldEndDate"
              label="Ngày kết thúc cũ"
              type="date"
              readonly
              disabled
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="createForm.newEndDate"
              label="Ngày kết thúc mới *"
              type="date"
              :error-messages="createErrors.newEndDate"
              :min="createForm.oldEndDate"
            />
          </v-col>
          <v-col cols="12">
            <v-textarea
              v-model="createForm.reason"
              label="Lý do gia hạn"
              rows="3"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="createForm.approvedByName"
              label="Người phê duyệt *"
              :error-messages="createErrors.approvedByName"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="createDialog = false">Hủy</v-btn>
          <v-btn color="primary" :loading="saving" @click="handleCreate">Tạo gia hạn</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Detail Dialog -->
    <v-dialog v-model="detailDialog" max-width="560">
      <v-card class="pa-6" v-if="detailTarget">
        <h2 class="text-h6 font-weight-bold mb-4">Chi tiết gia hạn</h2>
        <v-row dense>
          <v-col cols="6" class="text-caption text-medium-emphasis">Mã hợp đồng:</v-col>
          <v-col cols="6" class="font-weight-bold">{{ detailTarget.contractCode }}</v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Sinh viên:</v-col>
          <v-col cols="6" class="font-weight-bold">{{ detailTarget.studentName }}</v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Ngày kết thúc cũ:</v-col>
          <v-col cols="6">{{ formatDate(detailTarget.oldEndDate) }}</v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Ngày kết thúc mới:</v-col>
          <v-col cols="6" class="font-weight-bold" style="color: #4caf50">
            {{ formatDate(detailTarget.newEndDate) }}
          </v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Thời gian gia hạn:</v-col>
          <v-col cols="6">
            {{ calculateDuration(detailTarget.oldEndDate, detailTarget.newEndDate) }} tháng
          </v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Lý do:</v-col>
          <v-col cols="6">{{ detailTarget.reason || 'Không có' }}</v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Người phê duyệt:</v-col>
          <v-col cols="6" class="font-weight-bold">{{ detailTarget.approvedByName }}</v-col>
          
          <v-col cols="6" class="text-caption text-medium-emphasis">Ngày phê duyệt:</v-col>
          <v-col cols="6">{{ formatDateTime(detailTarget.approvedAt) }}</v-col>
        </v-row>
        <div class="d-flex justify-end mt-4">
          <v-btn variant="text" @click="detailDialog = false">Đóng</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Delete Dialog -->
    <v-dialog v-model="deleteDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-3">Xác nhận xóa</h2>
        <p>Bạn có chắc muốn xóa bản ghi gia hạn này?</p>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" :loading="saving" @click="deleteExtension">Xóa</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import DataStatus from '@/components/common/DataStatus.vue'
import { contractExtensionService } from '@/services/contractExtensionService'
import { contractService } from '@/services/contractService'

const columns = [
  { title: 'Hợp đồng', key: 'contract', width: 240 },
  { title: 'Thời gian gia hạn', key: 'oldEndDate', align: 'center', width: 180 },
  { title: 'Kéo dài', key: 'duration', align: 'center', width: 100 },
  { title: 'Người phê duyệt', key: 'approvedBy', width: 200 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 180 },
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
  newEndDate: '',
  reason: '',
  approvedByName: 'Admin',
})
const createErrors = ref({})
const snackbar = ref({ show: false, message: '', color: 'success' })

const filteredExtensions = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return extensions.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.contractCode?.toLowerCase().includes(keyword) ||
      item.studentName?.toLowerCase().includes(keyword)
    
    // Date range filter
    if (dateRange.value?.length === 2) {
      const itemDate = new Date(item.approvedAt)
      const [start, end] = dateRange.value
      if (itemDate < start || itemDate > end) return false
    }
    
    return matchesText
  })
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
        displayText: `${c.contractCode} - ${c.studentName} (${c.studentCode})`,
      }))
  } catch (err) {
    console.error('Không thể tải danh sách hợp đồng', err)
  }
}

function openCreate() {
  createForm.value = {
    contractId: null,
    oldEndDate: '',
    newEndDate: '',
    reason: '',
    approvedByName: 'Admin',
  }
  createErrors.value = {}
  createDialog.value = true
}

function onContractChange(contractId) {
  const contract = activeContracts.value.find((c) => c.id === contractId)
  if (contract) {
    createForm.value.oldEndDate = contract.endDate?.slice(0, 10) || ''
  }
}

async function handleCreate() {
  const errors = {}
  if (!createForm.value.contractId) errors.contractId = 'Vui lòng chọn hợp đồng'
  if (!createForm.value.newEndDate) errors.newEndDate = 'Vui lòng chọn ngày kết thúc mới'
  if (createForm.value.newEndDate <= createForm.value.oldEndDate)
    errors.newEndDate = 'Ngày kết thúc mới phải sau ngày kết thúc cũ'
  if (!createForm.value.approvedByName.trim())
    errors.approvedByName = 'Vui lòng nhập người phê duyệt'
  createErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    await contractExtensionService.create({
      contractId: createForm.value.contractId,
      newEndDate: createForm.value.newEndDate,
      reason: createForm.value.reason?.trim() || null,
      approvedByUserId: 1, // TODO: Get from auth
      approvedByName: createForm.value.approvedByName.trim(),
    })
    notify('Tạo gia hạn thành công')
    createDialog.value = false
    await loadExtensions()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
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
    notify('Đã xóa gia hạn')
    deleteDialog.value = false
    await loadExtensions()
  } catch (err) {
    notify(err.message || 'Không thể xóa', 'error')
  } finally {
    saving.value = false
  }
}

function calculateDuration(oldDate, newDate) {
  const start = new Date(oldDate)
  const end = new Date(newDate)
  const months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth())
  return months
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString('vi-VN') : ''
}

function formatDateTime(value) {
  return value
    ? new Date(value).toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
      })
    : ''
}

function notify(message, color = 'success') {
  snackbar.value = { show: false, message, color }
  snackbar.value.show = true
}

onMounted(() => {
  loadExtensions()
  loadActiveContracts()
})
</script>
