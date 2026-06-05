<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">Hợp đồng thuê phòng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">Quản lý hợp đồng KTX của sinh viên</p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Tạo hợp đồng
      </a-button>
    </div>

    <!-- Stats Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb;">
          <a-statistic
            title="Tổng hợp đồng"
            :value="contracts.length"
            :value-style="{ color: '#5b21b6', fontSize: '24px', fontWeight: 700 }"
          >
            <template #prefix>
              <FileTextOutlined style="font-size: 20px;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb;">
          <a-statistic
            title="Đang hiệu lực"
            :value="countStatus('Hiệu lực')"
            :value-style="{ color: '#16a34a', fontSize: '24px', fontWeight: 700 }"
          >
            <template #prefix>
              <CheckCircleOutlined style="font-size: 20px;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb;">
          <a-statistic
            title="Sắp hết hạn"
            :value="countStatus('Sắp hết hạn')"
            :value-style="{ color: '#f59e0b', fontSize: '24px', fontWeight: 700 }"
          >
            <template #prefix>
              <ClockCircleOutlined style="font-size: 20px;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb;">
          <a-statistic
            title="Đã chấm dứt"
            :value="countStatus('Đã chấm dứt')"
            :value-style="{ color: '#ef4444', fontSize: '24px', fontWeight: 700 }"
          >
            <template #prefix>
              <CloseCircleOutlined style="font-size: 20px;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px;" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm kiếm hợp đồng..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="8">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="Hiệu lực">Hiệu lực</a-select-option>
            <a-select-option value="Sắp hết hạn">Sắp hết hạn</a-select-option>
            <a-select-option value="Đã chấm dứt">Đã chấm dứt</a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
        :columns="columns"
        :data-source="filteredContracts"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} hợp đồng` }"
        :scroll="{ x: 1200 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'studentName'">
            <div>
              <div style="font-weight: 600;">{{ record.studentName }}</div>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'price'">
            <span style="font-weight: 700; color: #1890ff;">{{ formatPrice(record.price) }}</span>
          </template>
          
          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.displayStatus)">
              {{ record.displayStatus }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Sửa">
                <a-button type="text" size="small" @click="openEdit(record)">
                  <template #icon><EditOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Chấm dứt">
                <a-button 
                  type="text" 
                  size="small" 
                  :disabled="record.status === 'Terminated'"
                  @click="terminateContract(record)"
                >
                  <template #icon><StopOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Xóa">
                <a-button type="text" danger size="small" @click="confirmDelete(record)">
                  <template #icon><DeleteOutlined /></template>
                </a-button>
              </a-tooltip>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editTarget ? 'Chỉnh sửa hợp đồng' : 'Tạo hợp đồng mới'"
      width="700px"
      @cancel="closeDialog"
    >
      <a-form layout="vertical">
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item 
              label="Mã hợp đồng"
              :validate-status="formErrors.code ? 'error' : ''"
              :help="formErrors.code"
            >
              <a-input v-model:value="form.code" placeholder="Nhập mã hợp đồng" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item 
              label="Mã sinh viên"
              :validate-status="formErrors.studentCode ? 'error' : ''"
              :help="formErrors.studentCode"
            >
              <a-input v-model:value="form.studentCode" placeholder="Nhập mã sinh viên" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item 
              label="Tên sinh viên"
              :validate-status="formErrors.studentName ? 'error' : ''"
              :help="formErrors.studentName"
            >
              <a-input v-model:value="form.studentName" placeholder="Nhập tên sinh viên" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Phòng"
              :validate-status="formErrors.roomNumber ? 'error' : ''"
              :help="formErrors.roomNumber"
            >
              <a-input v-model:value="form.roomNumber" placeholder="Nhập số phòng" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item 
              label="Giá thuê (đ/tháng)"
              :validate-status="formErrors.price ? 'error' : ''"
              :help="formErrors.price"
            >
              <a-input-number
                v-model:value="form.price"
                :min="0"
                :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
                :parser="value => value.replace(/\$\s?|(,*)/g, '')"
                style="width: 100%"
                placeholder="Nhập giá thuê"
              />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item 
              label="Ngày bắt đầu"
              :validate-status="formErrors.startDate ? 'error' : ''"
              :help="formErrors.startDate"
            >
              <a-date-picker
                v-model:value="form.startDate"
                format="DD/MM/YYYY"
                style="width: 100%"
                placeholder="Chọn ngày"
              />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item 
              label="Ngày kết thúc"
              :validate-status="formErrors.endDate ? 'error' : ''"
              :help="formErrors.endDate"
            >
              <a-date-picker
                v-model:value="form.endDate"
                format="DD/MM/YYYY"
                style="width: 100%"
                placeholder="Chọn ngày"
              />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Trạng thái">
              <a-select v-model:value="form.status" style="width: 100%">
                <a-select-option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">
                  {{ opt.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
      <template #footer>
        <a-button @click="closeDialog" :disabled="saving">Hủy</a-button>
        <a-button type="primary" @click="saveContract" :loading="saving">
          {{ editTarget ? 'Lưu thay đổi' : 'Tạo hợp đồng' }}
        </a-button>
      </template>
    </a-modal>

    <!-- Delete Confirmation Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      @ok="deleteContract"
      @cancel="deleteDialog = false"
      okText="Xóa"
      okType="danger"
      cancelText="Hủy"
      :confirmLoading="saving"
    >
      <p>Bạn có chắc muốn xóa hợp đồng <strong>{{ deleteTarget?.code }}</strong>?</p>
    </a-modal>
  </div>
</template>

<script setup>
import { onMounted, ref, computed } from 'vue'
import { 
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
  StopOutlined,
  FileTextOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
  CloseCircleOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import dayjs from 'dayjs'
import { contractService } from '@/services/contractService'

const columns = [
  { title: 'Mã HĐ', dataIndex: 'code', key: 'code', width: 120 },
  { title: 'Sinh viên', key: 'studentName', width: 200 },
  { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 100, align: 'center' },
  { title: 'Giá thuê', key: 'price', width: 140, align: 'right' },
  { title: 'Bắt đầu', dataIndex: 'startDate', key: 'startDate', width: 120, align: 'center' },
  { title: 'Kết thúc', dataIndex: 'endDate', key: 'endDate', width: 120, align: 'center' },
  { title: 'Trạng thái', key: 'status', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 140, align: 'center', fixed: 'right' }
]

const statusOptions = [
  { label: 'Hiệu lực', value: 'Active' },
  { label: 'Hết hạn', value: 'Expired' },
  { label: 'Đã chấm dứt', value: 'Terminated' },
  { label: 'Đã gia hạn', value: 'Renewed' }
]

const contracts = ref([])
const loading = ref(false)
const saving = ref(false)
const error = ref(null)
const dialog = ref(false)
const deleteDialog = ref(false)
const editTarget = ref(null)
const deleteTarget = ref(null)
const formErrors = ref({})
const search = ref('')
const statusFilter = ref(undefined)

const form = ref({
  code: '',
  studentId: '',
  studentCode: '',
  studentName: '',
  roomId: '',
  roomNumber: '',
  price: 0,
  startDate: null,
  endDate: null,
  status: 'Active'
})

const filteredContracts = computed(() => {
  let result = contracts.value
  
  if (search.value) {
    const keyword = search.value.toLowerCase()
    result = result.filter(c =>
      c.code?.toLowerCase().includes(keyword) ||
      c.studentName?.toLowerCase().includes(keyword) ||
      c.studentCode?.toLowerCase().includes(keyword)
    )
  }
  
  if (statusFilter.value) {
    result = result.filter(c => c.displayStatus === statusFilter.value)
  }
  
  return result
})

async function loadContracts() {
  loading.value = true
  error.value = null
  try {
    const data = await contractService.getAll()
    contracts.value = data.map((item) => ({
      ...item,
      startDate: formatDate(item.startDate),
      endDate: formatDate(item.endDate),
      displayStatus: displayStatus(item)
    }))
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách hợp đồng'
    message.error(error.value)
  } finally {
    loading.value = false
  }
}

function displayStatus(item) {
  if (item.status === 'Terminated') return 'Đã chấm dứt'
  if (item.status === 'Expired') return 'Hết hạn'
  if (item.status === 'Renewed') return 'Đã gia hạn'
  const daysRemaining = Math.ceil((new Date(item.endDate) - new Date()) / 86400000)
  return daysRemaining >= 0 && daysRemaining <= 30 ? 'Sắp hết hạn' : 'Hiệu lực'
}

function countStatus(status) {
  return contracts.value.filter((item) => item.displayStatus === status).length
}

function getStatusColor(status) {
  return {
    'Hiệu lực': 'success',
    'Sắp hết hạn': 'warning',
    'Đã gia hạn': 'cyan',
    'Hết hạn': 'default',
    'Đã chấm dứt': 'error'
  }[status] || 'default'
}

function formatPrice(value) {
  return `${Number(value).toLocaleString('vi-VN')}đ`
}

function formatDate(value) {
  return value ? dayjs(value).format('DD/MM/YYYY') : ''
}

function openCreate() {
  editTarget.value = null
  form.value = {
    code: '',
    studentId: '',
    studentCode: '',
    studentName: '',
    roomId: '',
    roomNumber: '',
    price: 0,
    startDate: null,
    endDate: null,
    status: 'Active'
  }
  formErrors.value = {}
  dialog.value = true
}

function openEdit(item) {
  editTarget.value = item
  form.value = {
    ...item,
    startDate: item.startDate ? dayjs(item.startDate, 'DD/MM/YYYY') : null,
    endDate: item.endDate ? dayjs(item.endDate, 'DD/MM/YYYY') : null
  }
  formErrors.value = {}
  dialog.value = true
}

function closeDialog() {
  dialog.value = false
  editTarget.value = null
}

function validate() {
  const errors = {}
  if (!form.value.code.trim()) errors.code = 'Vui lòng nhập mã hợp đồng'
  if (!form.value.studentCode.trim()) errors.studentCode = 'Vui lòng nhập mã sinh viên'
  if (!form.value.studentName.trim()) errors.studentName = 'Vui lòng nhập tên sinh viên'
  if (!form.value.roomNumber.trim()) errors.roomNumber = 'Vui lòng nhập phòng'
  if (form.value.price < 0) errors.price = 'Giá thuê không hợp lệ'
  if (!form.value.startDate) errors.startDate = 'Vui lòng chọn ngày bắt đầu'
  if (!form.value.endDate) errors.endDate = 'Vui lòng chọn ngày kết thúc'
  else if (form.value.endDate.isBefore(form.value.startDate)) {
    errors.endDate = 'Ngày kết thúc phải sau ngày bắt đầu'
  }
  formErrors.value = errors
  return Object.keys(errors).length === 0
}

function payload(item = form.value) {
  return {
    code: item.code.trim(),
    studentId: item.studentId || crypto.randomUUID(),
    studentCode: item.studentCode.trim(),
    studentName: item.studentName.trim(),
    roomId: item.roomId || crypto.randomUUID(),
    roomNumber: item.roomNumber.trim(),
    price: Number(item.price),
    startDate: item.startDate ? item.startDate.format('YYYY-MM-DD') : null,
    endDate: item.endDate ? item.endDate.format('YYYY-MM-DD') : null,
    status: item.status
  }
}

async function saveContract() {
  if (!validate()) return
  saving.value = true
  try {
    if (editTarget.value) {
      await contractService.update(editTarget.value.id, payload())
      message.success('Cập nhật hợp đồng thành công')
    } else {
      await contractService.create(payload())
      message.success('Tạo hợp đồng thành công')
    }
    closeDialog()
    await loadContracts()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  } finally {
    saving.value = false
  }
}

async function terminateContract(item) {
  saving.value = true
  try {
    await contractService.update(item.id, { ...item, status: 'Terminated' })
    message.success('Đã chấm dứt hợp đồng')
    await loadContracts()
  } catch (err) {
    message.error(err.message || 'Không thể chấm dứt hợp đồng')
  } finally {
    saving.value = false
  }
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function deleteContract() {
  saving.value = true
  try {
    await contractService.delete(deleteTarget.value.id)
    deleteDialog.value = false
    message.success('Đã xóa hợp đồng')
    await loadContracts()
  } catch (err) {
    message.error(err.message || 'Không thể xóa hợp đồng')
  } finally {
    saving.value = false
  }
}

onMounted(loadContracts)
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
