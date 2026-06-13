<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Yêu cầu Sửa chữa & Bảo trì</h1>
        <p class="text-body-2 text-medium-emphasis">Gửi yêu cầu khi phòng có sự cố cần xử lý</p>
      </div>
      <v-btn color="primary" @click="openCreateDialog" prepend-icon="mdi-plus">
        Tạo yêu cầu mới
      </v-btn>
    </div>

    <!-- Statistics Cards -->
    <v-row class="mb-6">
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-4">
          <div class="d-flex align-center ga-3">
            <v-avatar color="warning" variant="tonal" size="48">
              <v-icon color="warning">mdi-clock-outline</v-icon>
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ stats.pending }}</div>
              <div class="text-caption text-medium-emphasis">Chờ xử lý</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-4">
          <div class="d-flex align-center ga-3">
            <v-avatar color="info" variant="tonal" size="48">
              <v-icon color="info">mdi-progress-wrench</v-icon>
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ stats.inProgress }}</div>
              <div class="text-caption text-medium-emphasis">Đang xử lý</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-4">
          <div class="d-flex align-center ga-3">
            <v-avatar color="success" variant="tonal" size="48">
              <v-icon color="success">mdi-check-circle</v-icon>
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ stats.done }}</div>
              <div class="text-caption text-medium-emphasis">Hoàn thành</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-4">
          <div class="d-flex align-center ga-3">
            <v-avatar color="grey" variant="tonal" size="48">
              <v-icon>mdi-sigma</v-icon>
            </v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">{{ myRequests.length }}</div>
              <div class="text-caption text-medium-emphasis">Tổng số</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- Request List -->
    <v-card class="rounded-lg">
      <v-card-text class="pa-6">
        <h3 class="text-subtitle-1 font-weight-bold mb-4">Yêu cầu đã gửi</h3>
        
        <div v-if="loading" class="text-center py-8">
          <v-progress-circular indeterminate color="primary" />
        </div>

        <div v-else-if="myRequests.length === 0" class="text-center py-12">
          <v-icon size="64" color="grey-lighten-2" class="mb-4">mdi-wrench-outline</v-icon>
          <div class="text-h6 font-weight-bold mb-2">Chưa có yêu cầu nào</div>
          <p class="text-body-2 text-medium-emphasis mb-4">
            Khi phòng có sự cố, hãy tạo yêu cầu để nhân viên xử lý
          </p>
          <v-btn color="primary" @click="openCreateDialog">
            Tạo yêu cầu đầu tiên
          </v-btn>
        </div>

        <div v-else>
          <v-card 
            v-for="r in myRequests" 
            :key="r.id" 
            variant="outlined" 
            class="mb-4 pa-4"
            @click="viewDetails(r)"
            style="cursor: pointer"
          >
            <div class="d-flex align-start justify-space-between mb-3">
              <div class="flex-grow-1">
                <div class="d-flex align-center ga-2 mb-1">
                  <v-icon :color="getCategoryColor(r.category)" size="20">
                    {{ getCategoryIcon(r.category) }}
                  </v-icon>
                  <div class="text-body-1 font-weight-bold">{{ r.title }}</div>
                </div>
                <div class="text-caption text-medium-emphasis mb-2">
                  <v-icon size="14">mdi-calendar</v-icon>
                  {{ formatDate(r.createdAt) }} · 
                  <v-icon size="14">mdi-door</v-icon>
                  Phòng {{ r.roomNumber }} - {{ r.buildingName }}
                </div>
              </div>
              <div class="d-flex flex-column align-end ga-2">
                <v-chip :color="getStatusColor(r.status)" size="small" variant="flat">
                  {{ getStatusText(r.status) }}
                </v-chip>
                <v-chip :color="getPriorityColor(r.priority)" size="x-small" variant="tonal">
                  {{ getPriorityText(r.priority) }}
                </v-chip>
              </div>
            </div>

            <p class="text-body-2 text-medium-emphasis mb-3">{{ r.description }}</p>

            <!-- Assigned info -->
            <div v-if="r.assignedToName" class="d-flex align-center ga-2 mb-2">
              <v-icon size="16" color="info">mdi-account-wrench</v-icon>
              <span class="text-caption">Được giao cho: <strong>{{ r.assignedToName }}</strong></span>
            </div>

            <!-- Resolution note -->
            <div v-if="r.resolutionNote" class="pa-3 rounded-lg" style="background:#f0fdf4">
              <div class="text-caption font-weight-bold mb-1 text-success">
                <v-icon size="14" class="mr-1">mdi-check-circle</v-icon>
                Đã xử lý xong:
              </div>
              <div class="text-body-2">{{ r.resolutionNote }}</div>
              <div v-if="!r.rating && r.status === 'Done'" class="mt-3">
                <v-btn size="small" color="success" @click.stop="openRatingDialog(r)">
                  <v-icon start size="16">mdi-star</v-icon>
                  Đánh giá
                </v-btn>
              </div>
            </div>

            <!-- Rating -->
            <div v-if="r.rating" class="mt-2">
              <v-rating 
                :model-value="r.rating" 
                readonly 
                size="small" 
                color="amber"
                density="compact"
              />
              <div v-if="r.feedback" class="text-caption text-medium-emphasis mt-1">{{ r.feedback }}</div>
            </div>
          </v-card>
        </div>
      </v-card-text>
    </v-card>

    <!-- Create Request Dialog -->
    <v-dialog v-model="createDialog" max-width="600px">
      <v-card>
        <v-card-title class="text-h5 font-weight-bold pa-6">
          Tạo yêu cầu sửa chữa
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-6">
          <v-form ref="formRef">
            <v-text-field
              v-model="form.title"
              label="Tiêu đề"
              prepend-inner-icon="mdi-format-title"
              placeholder="VD: Hỏng vòi nước"
              :rules="[v => !!v || 'Vui lòng nhập tiêu đề']"
              class="mb-4"
            />

            <v-select
              v-model="form.category"
              label="Danh mục"
              prepend-inner-icon="mdi-tag"
              :items="categories"
              item-title="label"
              item-value="value"
              :rules="[v => !!v || 'Vui lòng chọn danh mục']"
              class="mb-4"
            />

            <v-select
              v-model="form.priority"
              label="Mức độ ưu tiên"
              prepend-inner-icon="mdi-alert-circle"
              :items="priorities"
              item-title="label"
              item-value="value"
              :rules="[v => !!v || 'Vui lòng chọn mức độ']"
              class="mb-4"
            />

            <v-textarea
              v-model="form.description"
              label="Mô tả chi tiết"
              prepend-inner-icon="mdi-text"
              rows="4"
              placeholder="Mô tả sự cố để nhân viên xử lý nhanh hơn..."
              :rules="[v => !!v || 'Vui lòng nhập mô tả']"
              class="mb-4"
            />

            <v-file-input
              v-model="form.images"
              label="Ảnh đính kèm"
              prepend-inner-icon="mdi-camera"
              accept="image/*"
              multiple
              chips
              hint="Có thể đính kèm nhiều ảnh"
              persistent-hint
            />
          </v-form>
        </v-card-text>
        <v-divider />
        <v-card-actions class="pa-6">
          <v-spacer />
          <v-btn variant="text" @click="createDialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="submitRequest" :loading="submitting">
            <v-icon start>mdi-send</v-icon>
            Gửi yêu cầu
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Rating Dialog -->
    <v-dialog v-model="ratingDialog" max-width="500px">
      <v-card>
        <v-card-title class="text-h5 font-weight-bold pa-6">
          Đánh giá dịch vụ
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-6">
          <div class="text-center mb-4">
            <v-rating
              v-model="ratingForm.rating"
              size="large"
              color="amber"
              hover
            />
            <div class="text-caption text-medium-emphasis mt-2">
              Nhấn để đánh giá từ 1-5 sao
            </div>
          </div>
          <v-textarea
            v-model="ratingForm.feedback"
            label="Nhận xét (tùy chọn)"
            rows="3"
            placeholder="Chia sẻ trải nghiệm của bạn..."
          />
        </v-card-text>
        <v-divider />
        <v-card-actions class="pa-6">
          <v-spacer />
          <v-btn variant="text" @click="ratingDialog = false">Hủy</v-btn>
          <v-btn color="success" @click="submitRating" :loading="submittingRating">
            Gửi đánh giá
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Detail Dialog -->
    <v-dialog v-model="detailDialog" max-width="700px">
      <v-card v-if="selectedRequest">
        <v-card-title class="text-h5 font-weight-bold pa-6">
          Chi tiết yêu cầu #{{ selectedRequest.id }}
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-6">
          <div class="d-flex align-center justify-space-between mb-4">
            <v-chip :color="getStatusColor(selectedRequest.status)" variant="flat">
              {{ getStatusText(selectedRequest.status) }}
            </v-chip>
            <v-chip :color="getPriorityColor(selectedRequest.priority)" size="small" variant="tonal">
              {{ getPriorityText(selectedRequest.priority) }}
            </v-chip>
          </div>

          <div class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Tiêu đề</div>
            <div class="text-h6">{{ selectedRequest.title }}</div>
          </div>

          <div class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Phòng</div>
            <div>{{ selectedRequest.roomNumber }} - {{ selectedRequest.buildingName }}</div>
          </div>

          <div class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Danh mục</div>
            <div>{{ getCategoryLabel(selectedRequest.category) }}</div>
          </div>

          <div class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Mô tả</div>
            <div>{{ selectedRequest.description }}</div>
          </div>

          <div v-if="selectedRequest.assignedToName" class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Người xử lý</div>
            <div>{{ selectedRequest.assignedToName }}</div>
          </div>

          <div v-if="selectedRequest.expectedCompletionDate" class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Dự kiến hoàn thành</div>
            <div>{{ formatDate(selectedRequest.expectedCompletionDate) }}</div>
          </div>

          <div v-if="selectedRequest.resolutionNote" class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Ghi chú xử lý</div>
            <div class="pa-3 rounded-lg bg-green-50">{{ selectedRequest.resolutionNote }}</div>
          </div>

          <div class="mb-4">
            <div class="text-caption text-medium-emphasis mb-1">Ngày tạo</div>
            <div>{{ formatDateTime(selectedRequest.createdAt) }}</div>
          </div>
        </v-card-text>
        <v-divider />
        <v-card-actions class="pa-6">
          <v-spacer />
          <v-btn variant="text" @click="detailDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { contractService } from '@/services/contractService'
import axios from 'axios'

const authStore = useAuthStore()
const currentUser = computed(() => authStore.user)

const loading = ref(false)
const submitting = ref(false)
const submittingRating = ref(false)
const createDialog = ref(false)
const ratingDialog = ref(false)
const detailDialog = ref(false)
const formRef = ref(null)

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
    const activeContract = contracts.find(c => c.status === 'Active')
    
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
    alert('Bạn chưa có phòng. Vui lòng đăng ký phòng trước.')
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
  const { valid } = await formRef.value.validate()
  if (!valid) return

  if (!currentRoomId.value) {
    alert('Không tìm thấy thông tin phòng')
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
    
    // Send notification to staff (optional)
    alert('Đã gửi yêu cầu thành công!')
  } catch (error) {
    console.error('Error creating request:', error)
    alert('Lỗi khi gửi yêu cầu')
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
  } catch (error) {
    console.error('Error rating request:', error)
    alert('Lỗi khi gửi đánh giá')
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
    'New': 'warning',
    'Assigned': 'info',
    'InProgress': 'info',
    'Done': 'success',
    'Cancelled': 'grey',
    'Rejected': 'error'
  }
  return colorMap[status] || 'grey'
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
    'Low': 'grey',
    'Medium': 'info',
    'High': 'warning',
    'Urgent': 'error'
  }
  return colorMap[priority] || 'grey'
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

const getCategoryIcon = (category) => {
  const iconMap = {
    'Electric': 'mdi-lightning-bolt',
    'Plumbing': 'mdi-water',
    'Furniture': 'mdi-sofa',
    'Network': 'mdi-wifi',
    'Structure': 'mdi-home-outline',
    'Other': 'mdi-wrench'
  }
  return iconMap[category] || 'mdi-wrench'
}

const getCategoryColor = (category) => {
  const colorMap = {
    'Electric': 'warning',
    'Plumbing': 'info',
    'Furniture': 'brown',
    'Network': 'purple',
    'Structure': 'grey',
    'Other': 'grey'
  }
  return colorMap[category] || 'grey'
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
