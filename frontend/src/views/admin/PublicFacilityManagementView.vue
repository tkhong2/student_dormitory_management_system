<template>
  <div class="facility-management">
    <div class="page-header">
      <h1>Quản Lý Tiện Ích Chung</h1>
      <button class="btn btn-primary" @click="showCreateModal = true">
        <i class="fas fa-plus"></i> Thêm Tiện Ích
      </button>
    </div>

    <!-- Filters -->
    <div class="filters">
      <div class="filter-group">
        <label>Phân loại:</label>
        <select v-model="filterCategory">
          <option value="">Tất cả</option>
          <option value="Fitness">Thể thao</option>
          <option value="Study">Học tập</option>
          <option value="Recreation">Giải trí</option>
          <option value="Laundry">Giặt ủi</option>
          <option value="Parking">Bãi đỗ xe</option>
          <option value="Other">Khác</option>
        </select>
      </div>
      <div class="filter-group">
        <label>Trạng thái:</label>
        <select v-model="filterStatus">
          <option value="">Tất cả</option>
          <option value="Active">Hoạt động</option>
          <option value="UnderMaintenance">Bảo trì</option>
          <option value="Closed">Đóng cửa</option>
        </select>
      </div>
    </div>

    <!-- Facilities List -->
    <div class="facilities-grid">
      <div v-for="facility in filteredFacilities" :key="facility.id" class="facility-card">
        <div class="facility-image">
          <img :src="facility.imageUrl || '/default-facility.jpg'" :alt="facility.name">
          <span v-if="facility.isFeatured" class="badge-featured">Nổi bật</span>
          <span class="badge-status" :class="'status-' + facility.status.toLowerCase()">
            {{ getStatusText(facility.status) }}
          </span>
        </div>
        
        <div class="facility-content">
          <h3>{{ facility.name }}</h3>
          <p class="facility-id">Mã: {{ facility.facilityId }}</p>
          <p class="facility-brand">{{ facility.brandName }}</p>
          <p class="facility-category">
            <i class="fas fa-tag"></i> {{ getCategoryText(facility.category) }}
          </p>
          
          <div class="facility-info">
            <div v-if="facility.location">
              <i class="fas fa-map-marker-alt"></i> {{ facility.location }}
            </div>
            <div v-if="facility.managerName">
              <i class="fas fa-user"></i> {{ facility.managerName }}
            </div>
            <div v-if="facility.managerPhone">
              <i class="fas fa-phone"></i> {{ facility.managerPhone }}
            </div>
          </div>

          <div class="facility-stats">
            <div class="stat">
              <i class="fas fa-star"></i>
              <span>{{ facility.averageRating?.toFixed(1) || 'N/A' }}</span>
              <small>({{ facility.reviewCount }} đánh giá)</small>
            </div>
            <div class="stat">
              <i class="fas fa-users"></i>
              <span>{{ facility.totalUsageCount }}</span>
              <small>lượt sử dụng</small>
            </div>
          </div>

          <div class="facility-actions">
            <button class="btn btn-sm btn-info" @click="viewDetails(facility)">
              <i class="fas fa-eye"></i>
            </button>
            <button class="btn btn-sm btn-warning" @click="editFacility(facility)">
              <i class="fas fa-edit"></i>
            </button>
            <button class="btn btn-sm btn-danger" @click="deleteFacility(facility.id)">
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click.self="closeModal">
      <div class="modal-content facility-modal">
        <div class="modal-header">
          <h2>{{ showEditModal ? 'Chỉnh Sửa' : 'Thêm Mới' }} Tiện Ích</h2>
          <button class="btn-close" @click="closeModal">&times;</button>
        </div>
        
        <div class="modal-body">
          <form @submit.prevent="saveFacility">
            <div class="form-row">
              <div class="form-group">
                <label>Tên tiện ích *</label>
                <input v-model="form.name" type="text" required>
              </div>
              <div class="form-group">
                <label>Mã tiện ích *</label>
                <input v-model="form.facilityId" type="text" required>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Phân loại *</label>
                <select v-model="form.category" required>
                  <option value="Fitness">Thể thao</option>
                  <option value="Study">Học tập</option>
                  <option value="Recreation">Giải trí</option>
                  <option value="Laundry">Giặt ủi</option>
                  <option value="Parking">Bãi đỗ xe</option>
                  <option value="Other">Khác</option>
                </select>
              </div>
              <div class="form-group">
                <label>Thương hiệu</label>
                <input v-model="form.brandName" type="text">
              </div>
            </div>

            <div class="form-group">
              <label>Vị trí</label>
              <input v-model="form.location" type="text">
            </div>

            <div class="form-group">
              <label>Mô tả</label>
              <textarea v-model="form.description" rows="3"></textarea>
            </div>

            <div class="form-group">
              <label>URL hình ảnh</label>
              <input v-model="form.imageUrl" type="text">
            </div>

            <h4>Thông tin người quản lý</h4>
            <div class="form-row">
              <div class="form-group">
                <label>Tên người quản lý</label>
                <input v-model="form.managerName" type="text">
              </div>
              <div class="form-group">
                <label>Số điện thoại</label>
                <input v-model="form.managerPhone" type="tel">
              </div>
            </div>

            <div class="form-group">
              <label>Email</label>
              <input v-model="form.managerEmail" type="email">
            </div>

            <div class="form-group">
              <label>Mạng xã hội (JSON)</label>
              <textarea v-model="form.socialLinks" rows="2" 
                placeholder='{"facebook": "...", "zalo": "..."}'></textarea>
            </div>

            <h4>Cài đặt</h4>
            <div class="form-row">
              <div class="form-group">
                <label>Trạng thái</label>
                <select v-model="form.status">
                  <option value="Active">Hoạt động</option>
                  <option value="UnderMaintenance">Bảo trì</option>
                  <option value="Closed">Đóng cửa</option>
                  <option value="TemporarilyClosed">Tạm đóng</option>
                </select>
              </div>
              <div class="form-group">
                <label>Sức chứa tối đa</label>
                <input v-model.number="form.maxCapacity" type="number">
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Phí/giờ (VNĐ)</label>
                <input v-model.number="form.feePerHour" type="number">
              </div>
              <div class="form-group">
                <label>Phí/lần (VNĐ)</label>
                <input v-model.number="form.feePerSession" type="number">
              </div>
            </div>

            <div class="form-group">
              <label>
                <input v-model="form.isBookingRequired" type="checkbox">
                Yêu cầu đặt lịch
              </label>
            </div>

            <div class="form-group">
              <label>
                <input v-model="form.isVisibleOnHomepage" type="checkbox">
                Hiển thị trên trang chủ
              </label>
            </div>

            <div class="form-group">
              <label>
                <input v-model="form.isFeatured" type="checkbox">
                Tiện ích nổi bật
              </label>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label>Thứ tự hiển thị</label>
                <input v-model.number="form.displayOrder" type="number" min="0">
              </div>
            </div>

            <div class="modal-actions">
              <button type="button" class="btn btn-secondary" @click="closeModal">Hủy</button>
              <button type="submit" class="btn btn-primary">Lưu</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const facilities = ref([])
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingId = ref(null)

const filterCategory = ref('')
const filterStatus = ref('')

const form = ref({
  name: '',
  facilityId: '',
  category: 'Fitness',
  brandName: '',
  status: 'Active',
  description: '',
  location: '',
  imageUrl: '',
  managerName: '',
  managerPhone: '',
  managerEmail: '',
  socialLinks: '',
  isBookingRequired: false,
  maxCapacity: null,
  feePerHour: null,
  feePerSession: null,
  isVisibleOnHomepage: true,
  displayOrder: 0,
  isFeatured: false
})

const filteredFacilities = computed(() => {
  return facilities.value.filter(f => {
    if (filterCategory.value && f.category !== filterCategory.value) return false
    if (filterStatus.value && f.status !== filterStatus.value) return false
    return true
  })
})

const loadFacilities = async () => {
  try {
    const response = await axios.get('/api/publicfacilities')
    facilities.value = response.data
  } catch (error) {
    console.error('Error loading facilities:', error)
    alert('Không thể tải danh sách tiện ích')
  }
}

const saveFacility = async () => {
  try {
    if (showEditModal.value) {
      await axios.put(`/api/publicfacilities/${editingId.value}`, form.value)
      alert('Cập nhật tiện ích thành công')
    } else {
      await axios.post('/api/publicfacilities', form.value)
      alert('Thêm tiện ích thành công')
    }
    closeModal()
    loadFacilities()
  } catch (error) {
    console.error('Error saving facility:', error)
    alert('Có lỗi xảy ra khi lưu tiện ích')
  }
}

const editFacility = (facility) => {
  editingId.value = facility.id
  form.value = { ...facility }
  showEditModal.value = true
}

const deleteFacility = async (id) => {
  if (!confirm('Bạn có chắc muốn xóa tiện ích này?')) return
  
  try {
    await axios.delete(`/api/publicfacilities/${id}`)
    alert('Xóa tiện ích thành công')
    loadFacilities()
  } catch (error) {
    console.error('Error deleting facility:', error)
    alert('Không thể xóa tiện ích')
  }
}

const viewDetails = (facility) => {
  // Navigate to detail page or show detail modal
  console.log('View details:', facility)
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  resetForm()
}

const resetForm = () => {
  form.value = {
    name: '',
    facilityId: '',
    category: 'Fitness',
    brandName: '',
    status: 'Active',
    description: '',
    location: '',
    imageUrl: '',
    managerName: '',
    managerPhone: '',
    managerEmail: '',
    socialLinks: '',
    isBookingRequired: false,
    maxCapacity: null,
    feePerHour: null,
    feePerSession: null,
    isVisibleOnHomepage: true,
    displayOrder: 0,
    isFeatured: false
  }
  editingId.value = null
}

const getCategoryText = (category) => {
  const map = {
    'Fitness': 'Thể thao',
    'Study': 'Học tập',
    'Recreation': 'Giải trí',
    'Laundry': 'Giặt ủi',
    'Parking': 'Bãi đỗ xe',
    'Other': 'Khác'
  }
  return map[category] || category
}

const getStatusText = (status) => {
  const map = {
    'Active': 'Hoạt động',
    'UnderMaintenance': 'Bảo trì',
    'Closed': 'Đóng cửa',
    'TemporarilyClosed': 'Tạm đóng'
  }
  return map[status] || status
}

onMounted(() => {
  loadFacilities()
})
</script>

<style scoped>
.facility-management {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.filters {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.facilities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.facility-card {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: transform 0.2s;
}

.facility-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.facility-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.facility-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.badge-featured {
  position: absolute;
  top: 10px;
  left: 10px;
  background: #ffd700;
  color: #333;
  padding: 5px 10px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: bold;
}

.badge-status {
  position: absolute;
  top: 10px;
  right: 10px;
  padding: 5px 10px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: bold;
}

.status-active { background: #28a745; color: white; }
.status-undermaintenance { background: #ffc107; color: #333; }
.status-closed { background: #dc3545; color: white; }
.status-temporarilyclosed { background: #6c757d; color: white; }

.facility-content {
  padding: 15px;
}

.facility-content h3 {
  margin: 0 0 5px 0;
  font-size: 18px;
}

.facility-id, .facility-brand, .facility-category {
  margin: 5px 0;
  font-size: 14px;
  color: #666;
}

.facility-info {
  margin: 15px 0;
  font-size: 14px;
}

.facility-info div {
  margin: 5px 0;
}

.facility-info i {
  width: 20px;
  color: #007bff;
}

.facility-stats {
  display: flex;
  gap: 20px;
  margin: 15px 0;
  padding: 10px 0;
  border-top: 1px solid #eee;
  border-bottom: 1px solid #eee;
}

.stat {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 14px;
}

.stat i {
  color: #ffc107;
}

.stat small {
  color: #666;
}

.facility-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

.facility-modal {
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
}

.form-group input[type="text"],
.form-group input[type="email"],
.form-group input[type="tel"],
.form-group input[type="number"],
.form-group select,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.form-group input[type="checkbox"] {
  margin-right: 8px;
}

h4 {
  margin: 20px 0 15px 0;
  padding-top: 15px;
  border-top: 1px solid #eee;
  color: #007bff;
}

.modal-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}
</style>
