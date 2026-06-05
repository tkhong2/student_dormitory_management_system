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
    
    <DataStatus
      :loading="loading"
      :error="error"
      :items="inspections"
      empty-message="Chưa có biên bản kiểm tra nào"
      :show-create-button="true"
      create-button-text="Tạo biên bản"
      @retry="loadInspections"
      @create="openCreate"
    >
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: {{ inspections.length }} biên bản
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
    </DataStatus>
    
    <!-- Modal Create -->
    <a-modal v-model:open="dialog" title="Tạo biên bản" @ok="save" @cancel="dialog = false" width="600px">
      <a-form layout="vertical">
        <a-form-item label="Phòng">
          <a-select v-model:value="form.roomId" show-search>
            <a-select-option v-for="r in rooms" :key="r.id" :value="r.id">
              {{ r.roomNumber }} - {{ r.buildingName }}
            </a-select-option>
          </a-select>
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
    
    <!-- Modal Delete -->
    <a-modal v-model:open="deleteDialog" title="Xóa" @ok="doDelete" @cancel="deleteDialog = false">
      <p>Xóa biên bản này?</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import { roomInspectionService } from '@/services/roomInspectionService'
import { roomService } from '@/services/roomService'
import DataStatus from '@/components/common/DataStatus.vue'

const inspections = ref([])
const rooms = ref([])
const loading = ref(false)
const error = ref(null)
const dialog = ref(false)
const deleteDialog = ref(false)
const deleteTarget = ref(null)

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

const columns = [
  { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 100 },
  { title: 'Ngày', key: 'inspectionDate', width: 120 },
  { title: 'Loại', key: 'inspectionType', width: 120 },
  { title: 'Tình trạng', key: 'overallCondition', width: 120 },
  { title: 'Người kiểm tra', dataIndex: 'inspectorName', key: 'inspectorName', width: 150 },
  { title: 'Trạng thái', key: 'isApproved', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 220, fixed: 'right' }
]

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
  error.value = null
  try {
    inspections.value = await roomInspectionService.getAll()
    console.log('Inspections loaded:', inspections.value)
  } catch (err) {
    error.value = err.message || 'Lỗi tải dữ liệu'
    console.error('Error loading inspections:', err)
  } finally {
    loading.value = false
  }
}

async function loadRooms() {
  try {
    rooms.value = await roomService.getAll()
  } catch (err) {
    console.error('Error loading rooms:', err)
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
    alert('Vui lòng chọn phòng')
    return
  }
  try {
    await roomInspectionService.create(form.value)
    dialog.value = false
    await loadInspections()
  } catch (err) {
    alert(err.message)
  }
}

function viewDetail(record) {
  alert(JSON.stringify(record, null, 2))
}

async function approve(record) {
  try {
    await roomInspectionService.approve(record.id, {
      approvedByUserId: 1,
      approvedByName: 'Admin',
      approvalNote: 'OK'
    })
    await loadInspections()
  } catch (err) {
    alert(err.message)
  }
}

function confirmDelete(record) {
  deleteTarget.value = record
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await roomInspectionService.delete(deleteTarget.value.id)
    deleteDialog.value = false
    await loadInspections()
  } catch (err) {
    alert(err.message)
  }
}

onMounted(() => {
  loadInspections()
  loadRooms()
})
</script>
