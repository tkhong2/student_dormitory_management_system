<template>
  <div>
    <PageHeaderAnt title="Quản lý bảo trì" subtitle="Xử lý yêu cầu bảo trì từ sinh viên">
      <template #actions>
        <a-space>
          <a-input-search
            v-model:value="searchText"
            placeholder="Tìm kiếm yêu cầu..."
            style="width: 300px"
            @search="handleSearch"
          />
          <a-button @click="loadMaintenanceRequests">
            <template #icon><ReloadOutlined /></template>
          </a-button>
        </a-space>
      </template>
    </PageHeaderAnt>

    <!-- Stats -->
    <a-row :gutter="16" style="margin-bottom: 16px;">
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Chưa phân công" :value="stats.pending" :valueStyle="{ color: '#faad14' }">
            <template #prefix><ClockCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Đang xử lý" :value="stats.inProgress" :valueStyle="{ color: '#1890ff' }">
            <template #prefix><ToolOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Hoàn thành" :value="stats.completed" :valueStyle="{ color: '#52c41a' }">
            <template #prefix><CheckCircleOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="6">
        <a-card size="small" :bordered="false">
          <a-statistic title="Khẩn cấp" :value="stats.urgent" :valueStyle="{ color: '#ff4d4f' }">
            <template #prefix><WarningOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Kanban Board -->
    <a-card :bordered="false">
      <a-row :gutter="16">
        <!-- Chưa phân công -->
        <a-col :xs="24" :md="8">
          <div class="kanban-column">
            <div class="kanban-header" style="background: #fff7e6; color: #fa8c16;">
              <CalendarOutlined /> Chờ phân công ({{ filteredRequests.pending.length }})
            </div>
            <div class="kanban-body">
              <div
                v-for="item in filteredRequests.pending"
                :key="item.id"
                class="kanban-card"
                @click="viewDetail(item)"
              >
                <a-badge
                  v-if="item.priority === 'Urgent'"
                  status="error"
                  text="Khẩn cấp"
                  style="position: absolute; top: 8px; right: 8px; font-size: 11px;"
                />
                <div style="margin-bottom: 8px;">
                  <a-tag :color="getCategoryColor(item.category)">
                    {{ getCategoryText(item.category) }}
                  </a-tag>
                </div>
                <div style="font-weight: 600; margin-bottom: 4px;">{{ item.title }}</div>
                <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px;">
                  {{ truncate(item.description, 60) }}
                </div>
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 12px;">
                  <span>
                    <HomeOutlined /> {{ item.roomNumber }}
                  </span>
                  <span style="color: #8c8c8c;">
                    {{ formatDate(item.createdAt) }}
                  </span>
                </div>
                <a-divider style="margin: 8px 0;" />
                <a-button size="small" type="primary" block @click.stop="openAssignModal(item)">
                  <UserAddOutlined /> Phân công
                </a-button>
              </div>
              <a-empty v-if="filteredRequests.pending.length === 0" :image="simpleImage" description="Không có yêu cầu" />
            </div>
          </div>
        </a-col>

        <!-- Đang xử lý -->
        <a-col :xs="24" :md="8">
          <div class="kanban-column">
            <div class="kanban-header" style="background: #e6f7ff; color: #1890ff;">
              <ToolOutlined /> Đang xử lý ({{ filteredRequests.inProgress.length }})
            </div>
            <div class="kanban-body">
              <div
                v-for="item in filteredRequests.inProgress"
                :key="item.id"
                class="kanban-card"
                @click="viewDetail(item)"
              >
                <div style="margin-bottom: 8px;">
                  <a-tag :color="getCategoryColor(item.category)">
                    {{ getCategoryText(item.category) }}
                  </a-tag>
                </div>
                <div style="font-weight: 600; margin-bottom: 4px;">{{ item.title }}</div>
                <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px;">
                  {{ truncate(item.description, 60) }}
                </div>
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 12px; margin-bottom: 8px;">
                  <span>
                    <HomeOutlined /> {{ item.roomNumber }}
                  </span>
                  <span style="color: #8c8c8c;">
                    {{ getDaysInProgress(item.startedAt) }} ngày
                  </span>
                </div>
                <div style="font-size: 12px; margin-bottom: 8px;">
                  <UserOutlined /> {{ item.assignedToName }}
                </div>
                <a-divider style="margin: 8px 0;" />
                <a-button size="small" type="primary" block @click.stop="openResolveModal(item)">
                  <CheckCircleOutlined /> Hoàn thành
                </a-button>
              </div>
              <a-empty v-if="filteredRequests.inProgress.length === 0" :image="simpleImage" description="Không có yêu cầu" />
            </div>
          </div>
        </a-col>

        <!-- Hoàn thành -->
        <a-col :xs="24" :md="8">
          <div class="kanban-column">
            <div class="kanban-header" style="background: #f6ffed; color: #52c41a;">
              <CheckCircleOutlined /> Hoàn thành ({{ filteredRequests.completed.length }})
            </div>
            <div class="kanban-body">
              <div
                v-for="item in filteredRequests.completed"
                :key="item.id"
                class="kanban-card"
                @click="viewDetail(item)"
              >
                <div style="margin-bottom: 8px;">
                  <a-tag :color="getCategoryColor(item.category)">
                    {{ getCategoryText(item.category) }}
                  </a-tag>
                </div>
                <div style="font-weight: 600; margin-bottom: 4px;">{{ item.title }}</div>
                <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px;">
                  {{ truncate(item.description, 60) }}
                </div>
                <div style="display: flex; justify-content: space-between; align-items: center; font-size: 12px; margin-bottom: 8px;">
                  <span>
                    <HomeOutlined /> {{ item.roomNumber }}
                  </span>
                  <span style="color: #52c41a;">
                    <CheckCircleOutlined /> {{ formatDate(item.resolvedAt) }}
                  </span>
                </div>
                <div style="font-size: 12px;">
                  <UserOutlined /> {{ item.assignedToName }}
                </div>
              </div>
              <a-empty v-if="filteredRequests.completed.length === 0" :image="simpleImage" description="Không có yêu cầu" />
            </div>
          </div>
        </a-col>
      </a-row>
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailModalVisible"
      title="Chi tiết yêu cầu bảo trì"
      width="700px"
      :footer="null"
    >
      <a-descriptions v-if="selectedRequest" bordered :column="2" size="small">
        <a-descriptions-item label="Trạng thái" :span="2">
          <a-tag :color="getStatusColor(selectedRequest.status)">
            {{ getStatusText(selectedRequest.status) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Danh mục">
          <a-tag :color="getCategoryColor(selectedRequest.category)">
            {{ getCategoryText(selectedRequest.category) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Độ ưu tiên">
          <a-tag :color="selectedRequest.priority === 'Urgent' ? 'red' : 'default'">
            {{ selectedRequest.priority === 'Urgent' ? 'Khẩn cấp' : 'Bình thường' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Tiêu đề" :span="2">
          <strong>{{ selectedRequest.title }}</strong>
        </a-descriptions-item>
        <a-descriptions-item label="Mô tả" :span="2">
          {{ selectedRequest.description }}
        </a-descriptions-item>
        <a-descriptions-item label="Phòng">
          {{ selectedRequest.roomNumber }}
        </a-descriptions-item>
        <a-descriptions-item label="Tòa nhà">
          {{ selectedRequest.buildingName }}
        </a-descriptions-item>
        <a-descriptions-item label="Người báo cáo">
          {{ selectedRequest.requestedByStudentName }}
        </a-descriptions-item>
        <a-descriptions-item label="Ngày báo cáo">
          {{ formatDate(selectedRequest.createdAt) }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedRequest.assignedToName" label="Người xử lý">
          {{ selectedRequest.assignedToName }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedRequest.assignedAt" label="Ngày phân công">
          {{ formatDate(selectedRequest.assignedAt) }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedRequest.resolvedAt" label="Ngày hoàn thành">
          {{ formatDate(selectedRequest.resolvedAt) }}
        </a-descriptions-item>
        <a-descriptions-item v-if="selectedRequest.resolutionNote" label="Ghi chú xử lý" :span="2">
          {{ selectedRequest.resolutionNote }}
        </a-descriptions-item>
      </a-descriptions>

      <div style="margin-top: 16px; display: flex; justify-content: flex-end; gap: 8px;">
        <a-button @click="detailModalVisible = false">Đóng</a-button>
        <a-button
          v-if="selectedRequest.status === 'New'"
          type="primary"
          @click="openAssignModal(selectedRequest)"
        >
          Phân công
        </a-button>
        <a-button
          v-if="selectedRequest.status === 'Assigned' || selectedRequest.status === 'InProgress'"
          type="primary"
          @click="openResolveModal(selectedRequest)"
        >
          Hoàn thành
        </a-button>
      </div>
    </a-modal>

    <!-- Assign Modal -->
    <a-modal
      v-model:open="assignModalVisible"
      title="Phân công xử lý"
      @ok="handleAssign"
      :confirmLoading="assigning"
    >
      <a-form layout="vertical">
        <a-form-item label="Nhân viên xử lý *" required>
          <a-select
            v-model:value="assignForm.assignedToUserId"
            placeholder="Chọn nhân viên"
            show-search
            :filter-option="filterStaffOption"
          >
            <a-select-option
              v-for="staff in staffList"
              :key="staff.id"
              :value="staff.id"
              :label="staff.fullName"
            >
              {{ staff.fullName }} - {{ staff.phone }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="Ghi chú">
          <a-textarea
            v-model:value="assignForm.notes"
            :rows="3"
            placeholder="Ghi chú thêm cho nhân viên..."
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Resolve Modal -->
    <a-modal
      v-model:open="resolveModalVisible"
      title="Hoàn thành xử lý"
      @ok="handleResolve"
      :confirmLoading="resolving"
    >
      <a-form layout="vertical">
        <a-form-item label="Ghi chú xử lý *" required>
          <a-textarea
            v-model:value="resolveForm.resolutionNotes"
            :rows="4"
            placeholder="Mô tả công việc đã thực hiện, vật tư đã sử dụng..."
          />
        </a-form-item>

        <a-form-item label="Chi phí (nếu có)">
          <a-input-number
            v-model:value="resolveForm.cost"
            :min="0"
            style="width: 100%;"
            :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            :parser="value => value.replace(/\$\s?|(,*)/g, '')"
          >
            <template #addonAfter>VNĐ</template>
          </a-input-number>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { Empty } from 'ant-design-vue'
import {
  ReloadOutlined, ClockCircleOutlined, ToolOutlined, CheckCircleOutlined,
  WarningOutlined, CalendarOutlined, HomeOutlined, UserOutlined, UserAddOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { useAuthStore } from '@/stores/auth'
import axios from 'axios'
import dayjs from 'dayjs'

const authStore = useAuthStore()
const simpleImage = Empty.PRESENTED_IMAGE_SIMPLE
const loading = ref(false)
const searchText = ref('')
const maintenanceRequests = ref([])
const staffList = ref([])
const detailModalVisible = ref(false)
const assignModalVisible = ref(false)
const resolveModalVisible = ref(false)
const selectedRequest = ref(null)
const assigning = ref(false)
const resolving = ref(false)

const assignForm = ref({
  assignedToUserId: undefined,
  notes: ''
})

const resolveForm = ref({
  resolutionNotes: '',
  cost: 0
})

const stats = computed(() => ({
  pending: maintenanceRequests.value.filter(r => r.status === 'New').length,
  inProgress: maintenanceRequests.value.filter(r => r.status === 'Assigned' || r.status === 'InProgress').length,
  completed: maintenanceRequests.value.filter(r => r.status === 'Done').length,
  urgent: maintenanceRequests.value.filter(r => r.priority === 'Urgent' && r.status !== 'Done').length
}))

const filteredRequests = computed(() => {
  let filtered = maintenanceRequests.value

  if (searchText.value) {
    const search = searchText.value.toLowerCase()
    filtered = filtered.filter(r =>
      r.title?.toLowerCase().includes(search) ||
      r.description?.toLowerCase().includes(search) ||
      r.roomNumber?.toLowerCase().includes(search) ||
      r.requestedByStudentName?.toLowerCase().includes(search)
    )
  }

  return {
    pending: filtered.filter(r => r.status === 'New'),
    inProgress: filtered.filter(r => r.status === 'Assigned' || r.status === 'InProgress'),
    completed: filtered.filter(r => r.status === 'Done')
  }
})

const loadMaintenanceRequests = async () => {
  loading.value = true
  try {
    const response = await axios.get('http://localhost:5002/api/maintenancerequests', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    maintenanceRequests.value = response.data
    
    // Sort by priority and created date
    maintenanceRequests.value.sort((a, b) => {
      if (a.priority === 'Urgent' && b.priority !== 'Urgent') return -1
      if (a.priority !== 'Urgent' && b.priority === 'Urgent') return 1
      return new Date(b.createdAt) - new Date(a.createdAt)
    })
  } catch (error) {
    message.error('Lỗi tải danh sách yêu cầu bảo trì')
    console.error(error)
  } finally {
    loading.value = false
  }
}

const loadStaffList = async () => {
  try {
    const response = await axios.get('http://localhost:5002/api/users', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    // Filter staff and admin only
    staffList.value = response.data.filter(u => u.role === 'Staff' || u.role === 'Admin')
  } catch (error) {
    console.error('Lỗi tải danh sách nhân viên:', error)
  }
}

const handleSearch = () => {
  // Search triggered, data already filtered by computed
}

const viewDetail = (request) => {
  selectedRequest.value = request
  detailModalVisible.value = true
}

const openAssignModal = (request) => {
  selectedRequest.value = request
  assignForm.value = {
    assignedToUserId: undefined,
    notes: ''
  }
  detailModalVisible.value = false
  assignModalVisible.value = true
}

const openResolveModal = (request) => {
  selectedRequest.value = request
  resolveForm.value = {
    resolutionNotes: '',
    cost: 0
  }
  detailModalVisible.value = false
  resolveModalVisible.value = true
}

const handleAssign = async () => {
  if (!assignForm.value.assignedToUserId) {
    message.warning('Vui lòng chọn nhân viên xử lý')
    return
  }

  assigning.value = true
  try {
    const staff = staffList.value.find(s => s.id === assignForm.value.assignedToUserId)
    
    await axios.post(`http://localhost:5002/api/maintenancerequests/${selectedRequest.value.id}/assign`, {
      assignedToUserId: assignForm.value.assignedToUserId,
      assignedToName: staff?.fullName,
      expectedCompletionDate: new Date().toISOString().split('T')[0],
      estimatedCost: 0
    }, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    message.success('Đã phân công xử lý yêu cầu')
    assignModalVisible.value = false
    await loadMaintenanceRequests()
  } catch (error) {
    message.error(error.message || 'Lỗi phân công')
  } finally {
    assigning.value = false
  }
}

const handleResolve = async () => {
  if (!resolveForm.value.resolutionNotes.trim()) {
    message.warning('Vui lòng nhập ghi chú xử lý')
    return
  }

  resolving.value = true
  try {
    await axios.post(`http://localhost:5002/api/maintenancerequests/${selectedRequest.value.id}/resolve`, {
      resolutionNote: resolveForm.value.resolutionNotes,
      actualCost: resolveForm.value.cost || 0,
      afterImageUrls: null
    }, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    message.success('Đã hoàn thành xử lý yêu cầu')
    resolveModalVisible.value = false
    await loadMaintenanceRequests()
  } catch (error) {
    message.error(error.message || 'Lỗi hoàn thành')
  } finally {
    resolving.value = false
  }
}

const getCategoryColor = (category) => {
  const colors = {
    Electric: 'orange',
    Plumbing: 'blue',
    Furniture: 'purple',
    Network: 'green',
    Structure: 'red',
    Other: 'default'
  }
  return colors[category] || 'default'
}

const getCategoryText = (category) => {
  const texts = {
    Electric: 'Điện',
    Plumbing: 'Nước',
    Furniture: 'Nội thất',
    Network: 'Mạng',
    Structure: 'Kết cấu',
    Other: 'Khác'
  }
  return texts[category] || category
}

const getStatusColor = (status) => {
  const colors = {
    New: 'orange',
    Assigned: 'blue',
    InProgress: 'blue',
    Done: 'green',
    Cancelled: 'grey',
    Rejected: 'red'
  }
  return colors[status] || 'default'
}

const getStatusText = (status) => {
  const texts = {
    New: 'Chờ xử lý',
    Assigned: 'Đã phân công',
    InProgress: 'Đang xử lý',
    Done: 'Đã hoàn thành',
    Cancelled: 'Đã hủy',
    Rejected: 'Từ chối'
  }
  return texts[status] || status
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

const getDaysInProgress = (startedAt) => {
  return dayjs().diff(dayjs(startedAt), 'day')
}

const truncate = (text, length) => {
  if (!text) return ''
  return text.length > length ? text.substring(0, length) + '...' : text
}

const filterStaffOption = (input, option) => {
  return option.label.toLowerCase().includes(input.toLowerCase())
}

onMounted(() => {
  loadMaintenanceRequests()
  loadStaffList()
})
</script>

<style scoped>
.kanban-column {
  background: #f5f5f5;
  border-radius: 8px;
  overflow: hidden;
  height: calc(100vh - 350px);
  display: flex;
  flex-direction: column;
}

.kanban-header {
  padding: 12px;
  font-weight: 600;
  text-align: center;
  border-bottom: 2px solid rgba(0, 0, 0, 0.06);
}

.kanban-body {
  flex: 1;
  overflow-y: auto;
  padding: 12px;
}

.kanban-card {
  background: white;
  border-radius: 8px;
  padding: 12px;
  margin-bottom: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
}

.kanban-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}
</style>
