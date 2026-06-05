<template>
  <div>
    <!-- Header -->
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; color: #1a1a1a; margin: 0;">
          Quản lý Tiện nghi
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Quản lý tiện nghi và trang thiết bị trong phòng
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm tiện nghi
      </a-button>
    </div>
    
    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <div style="margin-bottom: 16px;">
        <p style="font-size: 14px; color: #595959; margin: 0;">
          Tổng: <strong>{{ amenities.length }}</strong> tiện nghi
        </p>
      </div>
      
      <!-- Table -->
      <a-table 
        :dataSource="amenities" 
        :columns="columns" 
        :rowKey="r => r.id"
        :pagination="{ pageSize: 10, showTotal: (total) => `Tổng ${total} tiện nghi` }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'category'">
            <a-tag :color="getCategoryColor(record.category)">
              {{ getCategoryLabel(record.category) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'isActive'">
            <a-tag :color="record.isActive ? 'green' : 'default'">
              {{ record.isActive ? 'Đang dùng' : 'Ngừng dùng' }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button size="small" @click="openEdit(record)">Sửa</a-button>
              <a-button size="small" danger @click="confirmDelete(record)">Xóa</a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
    
    <!-- Modal Create/Edit -->
    <a-modal 
      v-model:open="dialog" 
      :title="editTarget ? 'Sửa tiện nghi' : 'Thêm tiện nghi'" 
      @ok="save" 
      @cancel="dialog = false"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical">
        <a-form-item label="Tên">
          <a-input v-model:value="form.name" />
        </a-form-item>
        <a-form-item label="Danh mục">
          <a-select v-model:value="form.category">
            <a-select-option value="Electric">Điện tử</a-select-option>
            <a-select-option value="Furniture">Nội thất</a-select-option>
            <a-select-option value="Sanitary">Vệ sinh</a-select-option>
            <a-select-option value="Other">Khác</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item>
          <a-checkbox v-model:checked="form.isActive">Đang dùng</a-checkbox>
        </a-form-item>
      </a-form>
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
      <p>Bạn có chắc muốn xóa tiện nghi <strong>{{ deleteTarget?.name }}</strong>? Hành động này không thể hoàn tác.</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { amenityService } from '@/services/amenityService'

const amenities = ref([])
const loading = ref(false)
const dialog = ref(false)
const deleteDialog = ref(false)
const editTarget = ref(null)
const deleteTarget = ref(null)

const form = ref({
  name: '',
  category: 'Electric',
  iconUrl: '',
  isActive: true
})

const columns = [
  { title: 'Tên tiện nghi', dataIndex: 'name', key: 'name', width: 250 },
  { title: 'Danh mục', dataIndex: 'category', key: 'category', width: 150 },
  { title: 'Trạng thái', key: 'isActive', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150, fixed: 'right' }
]

async function loadAmenities() {
  loading.value = true
  try {
    amenities.value = await amenityService.getAll()
  } catch (err) {
    message.error(err.message || 'Không thể tải danh sách tiện nghi')
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editTarget.value = null
  form.value = { name: '', category: 'Electric', iconUrl: '', isActive: true }
  dialog.value = true
}

function openEdit(item) {
  editTarget.value = item
  form.value = { ...item }
  dialog.value = true
}

async function save() {
  try {
    if (editTarget.value) {
      await amenityService.update(editTarget.value.id, form.value)
      message.success('Cập nhật tiện nghi thành công')
    } else {
      await amenityService.create(form.value)
      message.success('Thêm tiện nghi thành công')
    }
    dialog.value = false
    await loadAmenities()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await amenityService.delete(deleteTarget.value.id)
    message.success('Đã xóa tiện nghi')
    deleteDialog.value = false
    await loadAmenities()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
  }
}

function getCategoryLabel(category) {
  const map = { Electric: 'Điện tử', Furniture: 'Nội thất', Sanitary: 'Vệ sinh', Other: 'Khác' }
  return map[category] || category
}

function getCategoryColor(category) {
  const map = { Electric: 'orange', Furniture: 'green', Sanitary: 'blue', Other: 'default' }
  return map[category] || 'default'
}

onMounted(loadAmenities)
</script>
