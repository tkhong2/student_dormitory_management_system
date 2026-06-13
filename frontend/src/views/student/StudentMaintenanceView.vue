<template>
  <div>
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 24px;">
      <div>
        <h1 style="font-size: 28px; font-weight: bold; margin-bottom: 4px;">Yêu cầu Sửa chữa & Bảo trì</h1>
        <p style="font-size: 14px; color: rgba(0,0,0,0.45);">Gửi yêu cầu khi phòng có sự cố cần xử lý</p>
      </div>
      <a-button type="primary" @click="openCreateDialog">
        <template #icon><plus-outlined /></template>
        Tạo yêu cầu mới
      </a-button>
    </div>

    <!-- Statistics Cards -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 24px;">
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true">
          <a-statistic title="Chờ xử lý" :value="stats.pending">
            <template #prefix>
              <clock-circle-outlined style="color: #faad14;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true">
          <a-statistic title="Đang xử lý" :value="stats.inProgress">
            <template #prefix>
              <sync-outlined :spin="true" style="color: #1890ff;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true">
          <a-statistic title="Hoàn thành" :value="stats.done">
            <template #prefix>
              <check-circle-outlined style="color: #52c41a;" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="24" :sm="12" :md="6">
        <a-card :bordered="true">
          <a-statistic title="Tổng số" :value="myRequests.length">
            <template #prefix>
              <span style="font-size: 20px;">Σ</span>
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Request List -->
    <a-card :bordered="true" style="border-radius: 8px;">
      <div style="padding: 24px;">
        <h3 style="font-size: 16px; font-weight: bold; margin-bottom: 16px;">Yêu cầu đã gửi</h3>
        
        <a-spin v-if="loading" style="display: block; text-align: center; padding: 32px 0;" />

        <a-empty v-else-if="myRequests.length === 0" style="padding: 48px 0;">
          <template #description>
            <div style="font-size: 18px; font-weight: bold; margin-bottom: 8px;">Chưa có yêu cầu nào</div>
            <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 16px;">
              Khi phòng có sự cố, hãy tạo yêu cầu để nhân viên xử lý
            </p>
            <a-button type="primary" @click="openCreateDialog">
              Tạo yêu cầu đầu tiên
            </a-button>
          </template>
        </a-empty>

        <div v-else>
          <a-card 
            v-for="r in myRequests" 
            :key="r.id" 
            :bordered="true" 
            style="margin-bottom: 16px; cursor: pointer;"
            @click="viewDetails(r)"
            :hoverable="true"
          >
            <div style="display: flex; align-items: flex-start; justify-content: space-between; margin-bottom: 12px;">
              <div style="flex: 1;">
                <div style="display: flex; align-items: center; gap: 8px; margin-bottom: 4px;">
                  <component :is="getCategoryIconComponent(r.category)" :style="{ color: getCategoryColor(r.category), fontSize: '20px' }" />
                  <div style="font-size: 16px; font-weight: bold;">{{ r.title }}</div>
                </div>
                <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-bottom: 8px;">
                  <calendar-outlined style="font-size: 14px;" />
                  {{ formatDate(r.createdAt) }} · 
                  <home-outlined style="font-size: 14px;" />
                  Phòng {{ r.roomNumber }} - {{ r.buildingName }}
                </div>
              </div>
              <div style="display: flex; flex-direction: column; align-items: flex-end; gap: 8px;">
                <a-tag :color="getStatusColor(r.status)">
                  {{ getStatusText(r.status) }}
                </a-tag>
                <a-tag :color="getPriorityColor(r.priority)">
                  {{ getPriorityText(r.priority) }}
                </a-tag>
              </div>
            </div>

            <p style="font-size: 14px; color: rgba(0,0,0,0.45); margin-bottom: 12px;">{{ r.description }}</p>

            <!-- Assigned info -->
            <div v-if="r.assignedToName" style="display: flex; align-items: center; gap: 8px; margin-bottom: 8px;">
              <user-outlined style="font-size: 16px; color: #1890ff;" />
              <span style="font-size: 12px;">Được giao cho: <strong>{{ r.assignedToName }}</strong></span>
            </div>

            <!-- Resolution note -->
            <div v-if="r.resolutionNote" style="padding: 12px; border-radius: 8px; background: #f0fdf4;">
              <div style="font-size: 12px; font-weight: bold; margin-bottom: 4px; color: #52c41a;">
                <check-circle-outlined style="font-size: 14px; margin-right: 4px;" />
                Đã xử lý xong:
              </div>
              <div style="font-size: 14px;">{{ r.resolutionNote }}</div>
              <div v-if="!r.rating && r.status === 'Done'" style="margin-top: 12px;">
                <a-button type="primary" size="small" @click.stop="openRatingDialog(r)" style="background: #52c41a; border-color: #52c41a;">
                  <template #icon><star-outlined /></template>
                  Đánh giá
                </a-button>
              </div>
            </div>

            <!-- Rating -->
            <div v-if="r.rating" style="margin-top: 8px;">
              <a-rate :value="r.rating" disabled allow-half />
              <div v-if="r.feedback" style="font-size: 12px; color: rgba(0,0,0,0.45); margin-top: 4px;">{{ r.feedback }}</div>
            </div>
          </a-card>
        </div>
      </div>
    </a-card>

    <!-- Create Request Dialog -->
    <a-modal v-model:open="createDialog" title="Tạo yêu cầu sửa chữa" :width="600" @ok="submitRequest" ok-text="Gửi yêu cầu" cancel-text="Hủy" :confirm-loading="submitting">
      <a-form layout="vertical" style="margin-top: 16px;">
        <a-form-item label="Tiêu đề" required>
          <a-input 
            v-model:value="form.title" 
            placeholder="VD: Hỏng vòi nước"
            :prefix="h(EditOutlined)"
          />
        </a-form-item>

        <a-form-item label="Danh mục" required>
          <a-select
            v-model:value="form.category"
            placeholder="Chọn danh mục"
            :options="categories"
          />
        </a-form-item>

        <a-form-item label="Mức độ ưu tiên" required>
          <a-select
            v-model:value="form.priority"
            placeholder="Chọn mức độ"
            :options="priorities"
          />
        </a-form-item>

        <a-form-item label="Mô tả chi tiết" required>
          <a-textarea
            v-model:value="form.description"
            :rows="4"
            placeholder="Mô tả sự cố để nhân viên xử lý nhanh hơn..."
          />
        </a-form-item>

        <a-form-item label="Ảnh đính kèm">
          <a-upload
            v-model:file-list="form.images"
            list-type="picture-card"
            accept="image/*"
            :multiple="true"
          >
            <div v-if="form.images.length < 5">
              <plus-outlined />
              <div style="margin-top: 8px;">Tải ảnh</div>
            </div>
          </a-upload>
          <div style="color: rgba(0,0,0,0.45); font-size: 12px; margin-top: 4px;">
            Có thể đính kèm nhiều ảnh
          </div>
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Rating Dialog -->
    <a-modal v-model:open="ratingDialog" title="Đánh giá dịch vụ" :width="500" @ok="submitRating" ok-text="Gửi đánh giá" cancel-text="Hủy" :confirm-loading="submittingRating">
      <div style="margin-top: 16px;">
        <div style="text-align: center; margin-bottom: 16px;">
          <a-rate 
            v-model:value="ratingForm.rating" 
            :style="{ fontSize: '32px' }"
            allow-half
          />
          <div style="font-size: 12px; color: rgba(0,0,0,0.45); margin-top: 8px;">
            Nhấn để đánh giá từ 1-5 sao
          </div>
        </div>
        <a-form-item label="Nhận xét (tùy chọn)">
          <a-textarea
            v-model:value="ratingForm.feedback"
            :rows="3"
            placeholder="Chia sẻ trải nghiệm của bạn..."
          />
        </a-form-item>
      </div>
    </a-modal>

    <!-- Detail Dialog -->
    <a-modal v-model:open="detailDialog" :title="`Chi tiết yêu cầu #${selectedRequest?.id || ''}`" :width="700" :footer="null">
      <div v-if="selectedRequest" style="margin-top: 16px;">
        <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 16px;">
          <a-tag :color="getStatusColor(selectedRequest.status)">
            {{ getStatusText(selectedRequest.status) }}
          </a-tag>
          <a-tag :color="getPriorityColor(selectedRequest.priority)">
            {{ getPriorityText(selectedRequest.priority) }}
          </a-tag>
        </div>

        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="Tiêu đề">
            <strong>{{ selectedRequest.title }}</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Phòng">
            {{ selectedRequest.roomNumber }} - {{ selectedRequest.buildingName }}
          </a-descriptions-item>
          <a-descriptions-item label="Danh mục">
            {{ getCategoryLabel(selectedRequest.category) }}
          </a-descriptions-item>
          <a-descriptions-item label="Mô tả">
            {{ selectedRequest.description }}
          </a-descriptions-item>
          <a-descriptions-item label="Người xử lý" v-if="selectedRequest.assignedToName">
            {{ selectedRequest.assignedToName }}
          </a-descriptions-item>
          <a-descriptions-item label="Dự kiến hoàn thành" v-if="selectedRequest.expectedCompletionDate">
            {{ formatDate(selectedRequest.expectedCompletionDate) }}
          </a-descriptions-item>
          <a-descriptions-item label="Ghi chú xử lý" v-if="selectedRequest.resolutionNote">
            <div style="padding: 8px; background: #f0fdf4; border-radius: 4px;">{{ selectedRequest.resolutionNote }}</div>
          </a-descriptions-item>
          <a-descriptions-item label="Ngày tạo">
            {{ formatDateTime(selectedRequest.createdAt) }}
          </a-descriptions-item>
        </a-descriptions>

        <div style="text-align: right; margin-top: 16px;">
          <a-button @click="detailDialog = false">Đóng</a-button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, h } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { contractService } from '@/services/contractService'
import axios from 'axios'
import { 
  PlusOutlined,
  ClockCircleOutlined,
  SyncOutlined,
  CheckCircleOutlined,
  StarOutlined,
  CalendarOutlined,
  HomeOutlined,
  TagOutlined,
  EditOutlined,
  UserOutlined,
  ThunderboltOutlined,
  DropboxOutlined,
  LaptopOutlined,
  BuildOutlined,
  ToolOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'

const authStore = useAuthStore()
const currentUser = computed(() => authStore.user)

const loading = ref(false)
const submitting = ref(false)
const submittingRating = ref(false)
const createDialog = ref(false)
const ratingDialog = ref(false)
const detailDialog = ref(false)

const myRequests = ref([])
const selectedRequest = ref(null)
const currentRoomId = ref(null)
const currentRoomInfo = ref(null)

const form = ref({
  title: '',
  category: '',
  priority: 'Medium',
  description: '',
  images: []
})

const ratingForm = ref({
  rating: 5,
  feedback: '',
  requestId: null
})

const categories = [
  { label: 'Điện', value: 'Electric' },
  { label: 'Nước', value: 'Plumbing' },
  { label: 'Nội thất', value: 'Furniture' },
  { label: 'Mạng', value: 'Network' },
  { label: 'Kết cấu', value: 'Structure' },
  { label: 'Khác', value: 'Other' }
]

const priorities = [
  { label: 'Thấp', value: 'Low' },
  { label: 'Bình thường', value: 'Medium' },
  { label: 'Cao', value: 'High' },
  { label: 'Khẩn cấp', value: 'Urgent' }
]

const stats = computed(() => {
  return {
    pending: myRequests.value.filter(r => r.status === 'New').length,
    inProgress: myRequests.value.filter(r => r.status === 'Assigned' || r.status === 'InProgress').length,
    done: myRequests.value.filter(r => r.status === 'Done').length
  }
})

// Fetch current room
const fetchCurrentRoom = async () => {
  try {
    const userId = currentUser.value.id
    const contracts = await contractService.getByUserId(userId)
    console.log('User contracts:', contracts)
    
    const activeContract = contracts.find(c => c.status === 'Active')
    console.log('Active contract:', activeContract)
    
    if (activeContract) {
      currentRoomId.value = activeContract.roomId
      currentRoomInfo.value = {
        roomNumber: activeContract.roomNumber,
        buildingName: activeContract.buildingName
      }
    }
  } catch (error) {
    console.error('Error fetching room:', error)
  }
}

// Fetch maintenance requests
const fetchRequests = async () => {
  loading.value = true
  try {
    const userId = currentUser.value.id
    const response = await axios.get(`http://localhost:5002/api/maintenancerequests/student/${userId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })
    myRequests.value = response.data
  } catch (error) {
    console.error('Error fetching requests:', error)
  } finally {
    loading.value = false
  }
}

// Open create dialog
const openCreateDialog = () => {
  if (!currentRoomId.value) {
    message.warning('Bạn chưa có phòng. Vui lòng đăng ký phòng trước.')
    return
  }
  form.value = {
    title: '',
    category: '',
    priority: 'Medium',
    description: '',
    images: []
  }
  createDialog.value = true
}

// Submit request
const submitRequest = async () => {
  if (!form.value.title || !form.value.category || !form.value.description) {
    message.error('Vui lòng điền đầy đủ thông tin!')
    return
  }

  if (!currentRoomId.value) {
    message.error('Không tìm thấy thông tin phòng')
    return
  }

  submitting.value = true
  try {
    await axios.post('http://localhost:5002/api/maintenancerequests', {
      roomId: currentRoomId.value,
      roomNumber: currentRoomInfo.value.roomNumber,
      buildingName: currentRoomInfo.value.buildingName,
      requestedByStudentId: currentUser.value.id,
      requestedByStudentName: currentUser.value.fullName,
      title: form.value.title,
      description: form.value.description,
      category: form.value.category,
      priority: form.value.priority,
      imageUrls: null // TODO: Upload images
    }, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    createDialog.value = false
    await fetchRequests()
    
    message.success('Đã gửi yêu cầu thành công!')
  } catch (error) {
    console.error('Error creating request:', error)
    message.error('Lỗi khi gửi yêu cầu')
  } finally {
    submitting.value = false
  }
}

// Open rating dialog
const openRatingDialog = (request) => {
  ratingForm.value = {
    rating: 5,
    feedback: '',
    requestId: request.id
  }
  ratingDialog.value = true
}

// Submit rating
const submitRating = async () => {
  submittingRating.value = true
  try {
    await axios.post(`http://localhost:5002/api/maintenancerequests/${ratingForm.value.requestId}/rate`, {
      rating: ratingForm.value.rating,
      feedback: ratingForm.value.feedback
    }, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    })

    ratingDialog.value = false
    await fetchRequests()
    message.success('Cảm ơn bạn đã đánh giá!')
  } catch (error) {
    console.error('Error rating request:', error)
    message.error('Lỗi khi gửi đánh giá')
  } finally {
    submittingRating.value = false
  }
}

// View details
const viewDetails = (request) => {
  selectedRequest.value = request
  detailDialog.value = true
}

// Helper functions
const getStatusColor = (status) => {
  const colorMap = {
    'New': 'orange',
    'Assigned': 'blue',
    'InProgress': 'blue',
    'Done': 'green',
    'Cancelled': 'default',
    'Rejected': 'red'
  }
  return colorMap[status] || 'default'
}

const getStatusText = (status) => {
  const textMap = {
    'New': 'Chờ xử lý',
    'Assigned': 'Đã phân công',
    'InProgress': 'Đang xử lý',
    'Done': 'Hoàn thành',
    'Cancelled': 'Đã hủy',
    'Rejected': 'Từ chối'
  }
  return textMap[status] || status
}

const getPriorityColor = (priority) => {
  const colorMap = {
    'Low': 'default',
    'Medium': 'blue',
    'High': 'orange',
    'Urgent': 'red'
  }
  return colorMap[priority] || 'default'
}

const getPriorityText = (priority) => {
  const textMap = {
    'Low': 'Thấp',
    'Medium': 'Bình thường',
    'High': 'Cao',
    'Urgent': 'Khẩn cấp'
  }
  return textMap[priority] || priority
}

const getCategoryIconComponent = (category) => {
  const iconMap = {
    'Electric': ThunderboltOutlined,
    'Plumbing': DropboxOutlined,
    'Furniture': BuildOutlined,
    'Network': LaptopOutlined,
    'Structure': BuildOutlined,
    'Other': ToolOutlined
  }
  return iconMap[category] || ToolOutlined
}

const getCategoryColor = (category) => {
  const colorMap = {
    'Electric': '#faad14',
    'Plumbing': '#1890ff',
    'Furniture': '#8b4513',
    'Network': '#722ed1',
    'Structure': '#595959',
    'Other': '#8c8c8c'
  }
  return colorMap[category] || '#8c8c8c'
}

const getCategoryLabel = (category) => {
  const cat = categories.find(c => c.value === category)
  return cat ? cat.label : category
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const formatDateTime = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleString('vi-VN')
}

onMounted(async () => {
  await fetchCurrentRoom()
  await fetchRequests()
})
</script>
