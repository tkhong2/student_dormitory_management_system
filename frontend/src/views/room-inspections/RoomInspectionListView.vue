<template>
  <div>
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Biên bản kiểm tra phòng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý biên bản kiểm tra định kỳ và sự cố
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Tạo biên bản
      </a-button>
    </div>
    
    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: <strong>{{ inspections.length }}</strong> biên bản
        </p>
      </div>
      
      <!-- Table -->
      <a-table 
        :dataSource="inspections" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} biên bản` }"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'inspectionDate'">
            {{ formatDate(record.inspectionDate) }}
          </template>
          
          <template v-else-if="column.key === 'inspectionType'">
            <a-tag :color="getTypeColor(record.inspectionType)">
              {{ getTypeLabel(record.inspectionType) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'overallCondition'">
            <a-tag :color="getConditionColor(record.overallCondition)">
              {{ getConditionLabel(record.overallCondition) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'isApproved'">
            <a-tag :color="record.isApproved ? 'green' : 'orange'">
              {{ record.isApproved ? 'Đã duyệt' : 'Chờ duyệt' }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="viewDetail(record)">Xem</a-button>
              <a-button v-if="!record.isApproved" size="small" type="primary" @click="approve(record)">Duyệt</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
    
    <!-- Modal Create -->
    <a-modal 
      v-model:open="dialog" 
      title="Tạo biên bản kiểm tra" 
      @ok="save" 
      @cancel="dialog = false" 
      width="600px"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Phòng">
          <a-select 
            v-model:value="form.roomId" 
            show-search
            placeholder="Chọn phòng"
            :options="roomOptions"
            :filter-option="filterOption"
          />
        </a-form-item>
        <a-form-item label="Ngày kiểm tra">
          <a-input v-model:value="form.inspectionDate" type="date" />
        </a-form-item>
        <a-form-item label="Loại">
          <a-select v-model:value="form.inspectionType">
            <a-select-option value="Routine">Định kỳ</a-select-option>
            <a-select-option value="CheckIn">Nhận phòng</a-select-option>
            <a-select-option value="CheckOut">Trả phòng</a-select-option>
            <a-select-option value="Incident">Sự cố</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Tình trạng chung">
          <a-select v-model:value="form.overallCondition">
            <a-select-option value="Good">Tốt</a-select-option>
            <a-select-option value="Fair">Khá</a-select-option>
            <a-select-option value="Poor">Kém</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="form.notes" :rows="3" />
        </a-form-item>
        <a-form-item label="Người kiểm tra">
          <a-input v-model:value="form.inspectorName" />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Modal View Detail -->
    <a-modal
      v-model:open="detailDialog"
      title="Chi tiết biên bản kiểm tra"
      width="600px"
      :footer="null"
    >
      <a-descriptions v-if="selectedInspection" :column="2" bordered size="small">
        <a-descriptions-item label="Phòng" :span="1">
          <a-tag color="blue">{{ selectedInspection.roomNumber }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày kiểm tra" :span="1">
          {{ formatDate(selectedInspection.inspectionDate) }}
        </a-descriptions-item>
        
        <a-descriptions-item label="Loại kiểm tra" :span="1">
          <a-tag :color="getTypeColor(selectedInspection.inspectionType)">
            {{ getTypeLabel(selectedInspection.inspectionType) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Tình trạng chung" :span="1">
          <a-tag :color="getConditionColor(selectedInspection.overallCondition)">
            {{ getConditionLabel(selectedInspection.overallCondition) }}
          </a-tag>
        </a-descriptions-item>
        
        <a-descriptions-item label="Người kiểm tra" :span="2">
          {{ selectedInspection.inspectorName }}
        </a-descriptions-item>
        
        <a-descriptions-item v-if="selectedInspection.notes" label="Ghi chú" :span="2">
          {{ selectedInspection.notes }}
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>
    
    <!-- Modal Delete -->
    <a-modal 
      v-model:open="deleteDialog" 
      title="Xác nhận xóa" 
      @ok="doDelete" 
      @cancel="deleteDialog = false"
      okText="Xóa"
      cancelText="Hủy"
      ok-button-props="{ danger: true }"
    >
      <p>Bạn có chắc muốn xóa biên bản kiểm tra này? Hành động này không thể hoàn tác.</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { message } from 'ant-design-vue'
import { roomInspectionService } from '@/services/roomInspectionService'
import { roomService } from '@/services/roomService'

const inspections = ref([])
const rooms = ref([])
const loading = ref(false)
const dialog = ref(false)
const detailDialog = ref(false)
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const selectedInspection = ref(null)

const form = ref({
  roomId: null,
  inspectorUserId: 1,
  inspectorName: 'Admin',
  inspectionDate: new Date().toISOString().split('T')[0],
  inspectionType: 'Routine',
  overallCondition: 'Good',
  electricalStatus: 'OK',
  plumbingStatus: 'OK',
  furnitureStatus: 'OK',
  wallStatus: 'OK',
  floorStatus: 'OK',
  notes: ''
})

const roomOptions = computed(() => {
  return rooms.value.map(r => ({
    value: r.id,
    label: `${r.roomNumber} - ${r.buildingName || 'N/A'}`
  }))
})

const columns = [
  { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 100 },
  { title: 'Ngày', key: 'inspectionDate', width: 120 },
  { title: 'Loại', key: 'inspectionType', width: 120 },
  { title: 'Tình trạng', key: 'overallCondition', width: 120 },
  { title: 'Người kiểm tra', dataIndex: 'inspectorName', key: 'inspectorName', width: 150 },
  { title: 'Trạng thái', key: 'isApproved', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 220, fixed: 'right' }
]

function filterOption(input, option) {
  return option.label.toLowerCase().indexOf(input.toLowerCase()) >= 0
}

function getTypeLabel(type) {
  const map = { Routine: 'Định kỳ', CheckIn: 'Nhận phòng', CheckOut: 'Trả phòng', Incident: 'Sự cố' }
  return map[type] || type
}

function getTypeColor(type) {
  const map = { Routine: 'blue', CheckIn: 'green', CheckOut: 'orange', Incident: 'red' }
  return map[type] || 'default'
}

function getConditionLabel(condition) {
  const map = { Good: 'Tốt', Fair: 'Khá', Poor: 'Kém' }
  return map[condition] || condition
}

function getConditionColor(condition) {
  const map = { Good: 'green', Fair: 'orange', Poor: 'red' }
  return map[condition] || 'default'
}

function formatDate(d) {
  if (!d) return ''
  return new Date(d).toLocaleDateString('vi-VN')
}

async function loadInspections() {
  loading.value = true
  try {
    inspections.value = await roomInspectionService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách biên bản')
  } finally {
    loading.value = false
  }
}

async function loadRooms() {
  try {
    const data = await roomService.getAll()
    console.log('Loaded rooms:', data)
    rooms.value = data || []
    if (rooms.value.length === 0) {
      message.warning('Không có phòng nào trong hệ thống')
    }
  } catch (err) {
    console.error('Error loading rooms:', err)
    message.error('Không thể tải danh sách phòng: ' + (err.message || 'Lỗi không xác định'))
    rooms.value = []
  }
}

function openCreate() {
  form.value = {
    roomId: null,
    inspectorUserId: 1,
    inspectorName: 'Admin',
    inspectionDate: new Date().toISOString().split('T')[0],
    inspectionType: 'Routine',
    overallCondition: 'Good',
    electricalStatus: 'OK',
    plumbingStatus: 'OK',
    furnitureStatus: 'OK',
    wallStatus: 'OK',
    floorStatus: 'OK',
    notes: ''
  }
  dialog.value = true
}

async function save() {
  if (!form.value.roomId) {
    message.warning('Vui lòng chọn phòng')
    return
  }
  try {
    await roomInspectionService.create(form.value)
    message.success('Tạo biên bản thành công')
    dialog.value = false
    await loadInspections()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function viewDetail(record) {
  selectedInspection.value = record
  detailDialog.value = true
}

async function approve(record) {
  try {
    await roomInspectionService.approve(record.id, {
      approvedByUserId: 1,
      approvedByName: 'Admin',
      approvalNote: 'OK'
    })
    message.success('Đã duyệt biên bản')
    await loadInspections()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function confirmDelete(record) {
  deleteTarget.value = record
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await roomInspectionService.delete(deleteTarget.value.id)
    message.success('Đã xóa biên bản')
    deleteDialog.value = false
    await loadInspections()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

onMounted(() => {
  loadInspections()
  loadRooms()
})
</script>
