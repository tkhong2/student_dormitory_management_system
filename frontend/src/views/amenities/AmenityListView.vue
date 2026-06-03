<template>
  <div>
    <h1>Quản lý Tiện nghi</h1>
    
    <!-- Loading -->
    <div v-if="loading">Đang tải...</div>
    
    <!-- Error -->
    <div v-else-if="error" style="color: red">{{ error }}</div>
    
    <!-- Success -->
    <div v-else>
      <p>Tổng: {{ amenities.length }} tiện nghi</p>
      
      <a-button type="primary" @click="openCreate" style="margin-bottom: 16px">
        Thêm tiện nghi
      </a-button>
      
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
    </div>
    
    <!-- Modal Create/Edit -->
    <a-modal v-model:open="dialog" :title="editTarget ? 'Sửa' : 'Thêm'" @ok="save" @cancel="dialog = false">
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
    <a-modal v-model:open="deleteDialog" title="Xóa" @ok="doDelete" @cancel="deleteDialog = false">
      <p>Xóa {{ deleteTarget?.name }}?</p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { amenityService } from '@/services/amenityService'

const amenities = ref([])
const loading = ref(false)
const error = ref(null)
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
  error.value = null
  try {
    amenities.value = await amenityService.getAll()
    console.log('Amenities loaded:', amenities.value)
  } catch (err) {
    error.value = err.message || 'Lỗi tải dữ liệu'
    console.error('Error loading amenities:', err)
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
    } else {
      await amenityService.create(form.value)
    }
    dialog.value = false
    await loadAmenities()
  } catch (err) {
    alert(err.message)
  }
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function doDelete() {
  try {
    await amenityService.delete(deleteTarget.value.id)
    deleteDialog.value = false
    await loadAmenities()
  } catch (err) {
    alert(err.message)
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
