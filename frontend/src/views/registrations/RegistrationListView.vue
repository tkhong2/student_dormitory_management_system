<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Đăng ký phòng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Quản lý các đơn đăng ký phòng ở của sinh viên</p>
      </div>
      <a-button type="primary" @click="dialog = true" style="background: #ff9800; border-color: #ff9800;">
        + Tạo đơn đăng ký
      </a-button>
    </div>

    <!-- Stats Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col v-for="s in regStats" :key="s.label" :xs="12" :md="6">
        <a-card :bordered="false" style="border: 1px solid #e5e7eb; text-align: center;">
          <div style="font-size: 24px; font-weight: 700;" :style="{ color: s.color }">{{ s.value }}</div>
          <div style="font-size: 13px; color: #8c8c8c; margin-top: 4px;">{{ s.label }}</div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
        :columns="columns"
        :data-source="registrations"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} đơn` }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div style="display: flex; align-items: center; gap: 12px; padding: 8px 0;">
              <a-avatar :size="36" style="background-color: #e0f2fe;">
                <template #icon><UserOutlined /></template>
              </a-avatar>
              <div>
                <div style="font-weight: 600; font-size: 14px;">{{ record.student }}</div>
                <div style="font-size: 12px; color: #8c8c8c;">{{ record.code }}</div>
              </div>
            </div>
          </template>
          
          <template v-else-if="column.key === 'status'">
            <a-tag :color="statusColor(record.status)">{{ record.status }}</a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space v-if="record.status === 'Chờ duyệt'">
              <a-button type="primary" size="small" @click="approveRegistration(record)" style="background: #16a34a; border-color: #16a34a;">
                Duyệt
              </a-button>
              <a-button danger size="small" @click="rejectRegistration(record)">
                Từ chối
              </a-button>
            </a-space>
            <a-tooltip v-else title="Xem chi tiết">
              <a-button type="text" size="small" @click="viewDetails(record)">
                <template #icon><EyeOutlined /></template>
              </a-button>
            </a-tooltip>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Create Registration Modal -->
    <a-modal
      v-model:open="dialog"
      title="Tạo đơn đăng ký phòng"
      width="560px"
      @ok="createRegistration"
      @cancel="dialog = false"
      okText="Gửi đăng ký"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Sinh viên">
          <a-select v-model:value="form.student" placeholder="Chọn sinh viên">
            <a-select-option value="SV003">SV003 - Lê Minh Cường</a-select-option>
            <a-select-option value="SV005">SV005 - Hoàng Thị Ê</a-select-option>
            <a-select-option value="SV008">SV008 - Đặng Văn Hải</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Tòa nhà mong muốn">
          <a-select v-model:value="form.building" placeholder="Chọn tòa nhà">
            <a-select-option value="A1">Tòa A1</a-select-option>
            <a-select-option value="A2">Tòa A2</a-select-option>
            <a-select-option value="B1">Tòa B1</a-select-option>
            <a-select-option value="C1">Tòa C1</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Loại phòng">
          <a-select v-model:value="form.roomType" placeholder="Chọn loại phòng">
            <a-select-option value="standard">Tiêu chuẩn (4 SV)</a-select-option>
            <a-select-option value="premium">Cao cấp (2 SV)</a-select-option>
            <a-select-option value="vip">VIP (1 SV)</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="form.note" :rows="3" placeholder="Nhập ghi chú" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { UserOutlined, EyeOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'

const dialog = ref(false)
const loading = ref(false)

const form = ref({
  student: undefined,
  building: undefined,
  roomType: undefined,
  note: ''
})

const regStats = [
  { label: 'Tổng đơn', value: '24', color: '#1890ff' },
  { label: 'Chờ duyệt', value: '8', color: '#faad14' },
  { label: 'Đã duyệt', value: '14', color: '#16a34a' },
  { label: 'Từ chối', value: '2', color: '#ef4444' },
]

const columns = [
  { title: 'Sinh viên', key: 'student', width: 250 },
  { title: 'Phòng yêu cầu', dataIndex: 'room', key: 'room', align: 'center' },
  { title: 'Ngày đăng ký', dataIndex: 'date', key: 'date', align: 'center', width: 150 },
  { title: 'Trạng thái', key: 'status', align: 'center', width: 130 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 180 },
]

const registrations = ref([
  { id: 1, student: 'Lê Minh Cường', code: 'SV003', room: 'Tiêu chuẩn – Tòa A1', date: '15/05/2026', status: 'Chờ duyệt' },
  { id: 2, student: 'Hoàng Thị Ê', code: 'SV005', room: 'Cao cấp – Tòa A2', date: '14/05/2026', status: 'Chờ duyệt' },
  { id: 3, student: 'Đặng Văn Hải', code: 'SV008', room: 'Tiêu chuẩn – Tòa B1', date: '13/05/2026', status: 'Chờ duyệt' },
  { id: 4, student: 'Nguyễn Văn An', code: 'SV001', room: 'Tiêu chuẩn – Tòa A1', date: '01/04/2026', status: 'Đã duyệt' },
  { id: 5, student: 'Trần Thị Bình', code: 'SV002', room: 'Tiêu chuẩn – Tòa A2', date: '01/04/2026', status: 'Đã duyệt' },
  { id: 6, student: 'Phạm Văn X', code: 'SV009', room: 'VIP – Tòa D1', date: '10/05/2026', status: 'Từ chối' },
])

const statusColor = (s) => {
  return { 'Chờ duyệt': 'orange', 'Đã duyệt': 'green', 'Từ chối': 'red' }[s] || 'default'
}

function createRegistration() {
  message.success('Gửi đơn đăng ký thành công')
  dialog.value = false
  form.value = { student: undefined, building: undefined, roomType: undefined, note: '' }
}

function approveRegistration(record) {
  message.success(`Đã duyệt đơn đăng ký của ${record.student}`)
  record.status = 'Đã duyệt'
}

function rejectRegistration(record) {
  message.warning(`Đã từ chối đơn đăng ký của ${record.student}`)
  record.status = 'Từ chối'
}

function viewDetails(record) {
  message.info(`Xem chi tiết đơn đăng ký của ${record.student}`)
}
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
