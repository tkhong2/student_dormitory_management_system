<template>
  <div>
    <!-- Page Header -->
    <div style="background: #fff; margin-bottom: 16px; border-radius: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); padding: 16px 24px">
      <div style="display: flex; justify-content: space-between; align-items: flex-start">
        <div>
          <h1 style="font-size: 24px; font-weight: 700; margin: 0 0 4px 0; color: #000">
            Thông Báo Tòa Nhà
          </h1>
          <p style="font-size: 14px; color: #8c8c8c; margin: 0">
            Quản lý thông báo và sự kiện trong ký túc xá
          </p>
        </div>
        <a-button type="primary" @click="openCreate" size="large">
          <template #icon><PlusOutlined /></template>
          Tạo Thông Báo
        </a-button>
      </div>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm tiêu đề, nội dung..."
            allow-clear
          >
            <template #prefix><SearchOutlined /></template>
          </a-input-search>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="categoryFilter"
            placeholder="Danh mục"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="General">Chung</a-select-option>
            <a-select-option value="Electric">Điện</a-select-option>
            <a-select-option value="Water">Nước</a-select-option>
            <a-select-option value="Maintenance">Bảo trì</a-select-option>
            <a-select-option value="Event">Sự kiện</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="priorityFilter"
            placeholder="Mức độ"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="Low">Thấp</a-select-option>
            <a-select-option value="Normal">Bình thường</a-select-option>
            <a-select-option value="High">Cao</a-select-option>
            <a-select-option value="Urgent">Khẩn cấp</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="4" style="text-align: right">
          <a-typography-text type="secondary">
            Tổng: {{ filteredAnnouncements.length }}
          </a-typography-text>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false">
      <a-table 
        :dataSource="filteredAnnouncements" 
        :columns="columns" 
        :loading="loading"
        :rowKey="r => r.id"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `Tổng ${total} thông báo` }"
        :scroll="{ x: 1200 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'title'">
            <div>
              <a-typography-text strong>{{ record.title }}</a-typography-text>
              <a-tag v-if="record.isPinned" color="red" size="small" style="margin-left: 8px">
                <template #icon><PushpinOutlined /></template>
                Ghim
              </a-tag>
            </div>
          </template>

          <template v-else-if="column.key === 'category'">
            <a-tag :color="getCategoryColor(record.category)">
              {{ getCategoryLabel(record.category) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'priority'">
            <a-tag :color="getPriorityColor(record.priority)">
              {{ getPriorityLabel(record.priority) }}
            </a-tag>
          </template>
          
          <template v-else-if="column.key === 'buildingName'">
            <a-typography-text v-if="record.buildingName">
              {{ record.buildingName }}
            </a-typography-text>
            <a-tag v-else color="blue">Toàn KTX</a-tag>
          </template>
          
          <template v-else-if="column.key === 'createdAt'">
            {{ formatDate(record.createdAt) }}
          </template>
          
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-tooltip title="Xem chi tiết">
                <a-button type="text" size="small" @click="viewDetail(record)">
                  <template #icon><EyeOutlined /></template>
                </a-button>
              </a-tooltip>
              <a-tooltip title="Sửa">
                <a-button type="text" size="small" @click="openEdit(record)">
                  <template #icon><EditOutlined /></template>
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
    
    <!-- Modal Create/Edit -->
    <a-modal 
      v-model:open="dialog" 
      :title="editTarget ? 'Sửa Thông Báo' : 'Tạo Thông Báo Mới'" 
      width="700px"
      @cancel="dialog = false"
    >
      <a-form layout="vertical">
        <a-form-item label="Tiêu Đề" required>
          <a-input v-model:value="form.title" placeholder="Nhập tiêu đề thông báo" />
        </a-form-item>
        
        <a-form-item label="Nội Dung" required>
          <a-textarea 
            v-model:value="form.content" 
            :rows="5" 
            placeholder="Nhập nội dung chi tiết"
          />
        </a-form-item>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Danh Mục">
              <a-select v-model:value="form.category">
                <a-select-option value="General">Chung</a-select-option>
                <a-select-option value="Electric">Điện</a-select-option>
                <a-select-option value="Water">Nước</a-select-option>
                <a-select-option value="Maintenance">Bảo trì</a-select-option>
                <a-select-option value="Event">Sự kiện</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          
          <a-col :span="12">
            <a-form-item label="Mức Độ Ưu Tiên">
              <a-select v-model:value="form.priority">
                <a-select-option value="Low">Thấp</a-select-option>
                <a-select-option value="Normal">Bình thường</a-select-option>
                <a-select-option value="High">Cao</a-select-option>
                <a-select-option value="Urgent">Khẩn cấp</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-form-item label="Áp Dụng Cho Tòa Nhà">
          <a-select 
            v-model:value="form.buildingId" 
            allow-clear
            placeholder="Chọn tòa nhà (để trống = toàn KTX)"
          >
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
        </a-form-item>
        
        <a-form-item>
          <a-checkbox v-model:checked="form.isPinned">
            <PushpinOutlined style="margin-right: 4px" />
            Ghim lên đầu trang
          </a-checkbox>
        </a-form-item>
      </a-form>
      
      <template #footer>
        <a-button @click="dialog = false">Hủy</a-button>
        <a-button type="primary" @click="save" :loading="saving">Lưu</a-button>
      </template>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  PlusOutlined,
  SearchOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  PushpinOutlined
} from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import { buildingAnnouncementService } from '@/services/buildingAnnouncementService'
import { buildingService } from '@/services/buildingService'

const announcements = ref([])
const buildings = ref([])
const loading = ref(false)
const saving = ref(false)
const dialog = ref(false)
const editTarget = ref(null)
const search = ref('')
const categoryFilter = ref(undefined)
const priorityFilter = ref(undefined)

const form = ref({
  title: '',
  content: '',
  category: 'General',
  priority: 'Normal',
  buildingId: null,
  isPinned: false,
  publishedAt: new Date().toISOString(),
  createdByUserId: 1,
  createdByName: 'Admin'
})

const columns = [
  { title: 'Tiêu đề', key: 'title', width: 280 },
  { title: 'Danh mục', key: 'category', width: 120, align: 'center' },
  { title: 'Mức độ', key: 'priority', width: 130, align: 'center' },
  { title: 'Tòa nhà', key: 'buildingName', width: 140, align: 'center' },
  { title: 'Người đăng', dataIndex: 'createdByName', key: 'createdByName', width: 150 },
  { title: 'Ngày tạo', key: 'createdAt', width: 130, align: 'center' },
  { title: 'Thao tác', key: 'actions', width: 150, align: 'center', fixed: 'right' }
]

const filteredAnnouncements = computed(() => {
  let result = announcements.value
  
  if (search.value) {
    const keyword = search.value.toLowerCase()
    result = result.filter(a => 
      a.title?.toLowerCase().includes(keyword) ||
      a.content?.toLowerCase().includes(keyword)
    )
  }
  
  if (categoryFilter.value) {
    result = result.filter(a => a.category === categoryFilter.value)
  }
  
  if (priorityFilter.value) {
    result = result.filter(a => a.priority === priorityFilter.value)
  }
  
  return result
})

async function loadAnnouncements() {
  loading.value = true
  try {
    announcements.value = await buildingAnnouncementService.getAll()
  } catch (err) {
    message.error(err.message || 'Lỗi tải dữ liệu')
  } finally {
    loading.value = false
  }
}

async function loadBuildings() {
  try {
    buildings.value = await buildingService.getAll()
  } catch (err) {
    console.error('Error loading buildings:', err)
  }
}

function openCreate() {
  editTarget.value = null
  form.value = {
    title: '',
    content: '',
    category: 'General',
    priority: 'Normal',
    buildingId: null,
    isPinned: false,
    publishedAt: new Date().toISOString(),
    createdByUserId: 1,
    createdByName: 'Admin'
  }
  dialog.value = true
}

function openEdit(item) {
  editTarget.value = item
  form.value = { ...item }
  dialog.value = true
}

function viewDetail(record) {
  Modal.info({
    title: record.title,
    content: record.content,
    width: 600,
    okText: 'Đóng'
  })
}

async function save() {
  if (!form.value.title || !form.value.content) {
    message.warning('Vui lòng điền đầy đủ thông tin')
    return
  }
  
  saving.value = true
  try {
    if (editTarget.value) {
      await buildingAnnouncementService.update(editTarget.value.id, form.value)
      message.success('Cập nhật thành công')
    } else {
      await buildingAnnouncementService.create(form.value)
      message.success('Tạo thông báo thành công')
    }
    dialog.value = false
    await loadAnnouncements()
  } catch (err) {
    message.error(err.message || 'Lỗi lưu dữ liệu')
  } finally {
    saving.value = false
  }
}

function confirmDelete(item) {
  Modal.confirm({
    title: 'Xác nhận xóa',
    content: `Bạn có chắc muốn xóa thông báo "${item.title}"?`,
    okText: 'Xóa',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        await buildingAnnouncementService.delete(item.id)
        message.success('Xóa thành công')
        await loadAnnouncements()
      } catch (err) {
        message.error('Xóa thất bại')
      }
    }
  })
}

function getPriorityColor(p) {
  const map = { Low: 'default', Normal: 'green', High: 'orange', Urgent: 'red' }
  return map[p] || 'default'
}

function getPriorityLabel(p) {
  const map = { Low: 'Thấp', Normal: 'Bình thường', High: 'Cao', Urgent: 'Khẩn cấp' }
  return map[p] || p
}

function getCategoryLabel(c) {
  const map = { 
    General: 'Chung', 
    Electric: 'Điện', 
    Water: 'Nước', 
    Maintenance: 'Bảo trì', 
    Event: 'Sự kiện' 
  }
  return map[c] || c
}

function getCategoryColor(c) {
  const map = { 
    General: 'blue', 
    Electric: 'gold', 
    Water: 'cyan', 
    Maintenance: 'orange', 
    Event: 'purple' 
  }
  return map[c] || 'default'
}

function formatDate(d) {
  if (!d) return ''
  return new Date(d).toLocaleDateString('vi-VN')
}

onMounted(() => {
  loadAnnouncements()
  loadBuildings()
})
</script>

<style scoped>
:deep(.ant-table-cell) {
  padding: 12px 8px !important;
}
</style>
