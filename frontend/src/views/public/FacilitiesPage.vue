<template>
  <div class="facilities-page">
    <!-- Hero Section -->
    <section class="hero-section">
      <v-container style="max-width: 1280px" class="hero-container">
        <v-row align="center">
          <!-- Left Content -->
          <v-col cols="12" md="6">
            <div class="hero-badge mb-4">
              <i class="fas fa-star mr-2"></i>
              TIỆN ÍCH HIỆN ĐẠI
            </div>
            
            <h1 class="hero-title mb-4">
              Tiện ích chung<br>tại KTX
            </h1>
            
            <p class="hero-subtitle mb-8">
              Khám phá các tiện ích hiện đại và tiện nghi: máy giặt,<br>
              máy sấy, phòng gym, phòng học... đầy đủ và chất lượng.
            </p>

            <!-- Stats -->
            <v-row class="stats-row mt-8">
              <v-col cols="4">
                <div class="stat-number">{{ totalUtilities }}</div>
                <div class="stat-label">TIỆN ÍCH</div>
              </v-col>
              <v-col cols="4">
                <div class="stat-number">{{ availableCount }}</div>
                <div class="stat-label">SẴN SÀNG</div>
              </v-col>
              <v-col cols="4">
                <div class="stat-number">{{ categories.length }}</div>
                <div class="stat-label">LOẠI</div>
              </v-col>
            </v-row>
          </v-col>

          <!-- Right Filter Card -->
          <v-col cols="12" md="6">
            <v-card class="filter-card pa-6 rounded-xl" elevation="4">
              <div class="filter-header mb-6">
                <i class="fas fa-filter text-orange mr-2"></i>
                <span class="filter-title">Lọc tiện ích</span>
              </div>

              <div class="mb-4">
                <div class="input-label mb-2">TÒA NHÀ</div>
                <select 
                  v-model="selectedBuilding" 
                  class="filter-select"
                  @change="loadUtilities"
                >
                  <option value="">Tất cả tòa nhà</option>
                  <option v-for="building in buildings" :key="building.id" :value="building.id">
                    {{ building.name }}
                  </option>
                </select>
              </div>

              <div class="mb-4">
                <div class="input-label mb-2">LOẠI TIỆN ÍCH</div>
                <select 
                  v-model="selectedCategory" 
                  class="filter-select"
                  @change="loadUtilities"
                >
                  <option value="">Tất cả loại</option>
                  <option v-for="cat in categories" :key="cat.value" :value="cat.value">
                    {{ cat.label }}
                  </option>
                </select>
              </div>

              <v-btn
                block
                size="large"
                class="search-btn"
                @click="loadUtilities"
              >
                <i class="fas fa-search mr-2"></i>
                Tìm kiếm
              </v-btn>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- Utilities Grid Section -->
    <section class="utilities-section py-12">
      <v-container style="max-width: 1280px">
        <v-progress-circular v-if="loading" indeterminate color="primary" class="d-flex mx-auto" />
        
        <div v-else-if="filteredUtilities.length === 0" class="text-center py-12">
          <i class="fas fa-inbox fa-4x text-grey mb-4"></i>
          <p class="text-h6 text-grey">Không tìm thấy tiện ích nào</p>
        </div>

        <v-row v-else>
          <v-col 
            v-for="utility in filteredUtilities" 
            :key="utility.id" 
            cols="12" 
            sm="6" 
            md="4"
          >
            <v-card class="utility-card" elevation="2">
              <!-- Icon Header -->
              <div class="utility-header" :class="utility.status">
                <i :class="getCategoryFaIcon(utility.category)" class="utility-icon-fa"></i>
                <div class="utility-status-badge">
                  {{ getStatusLabel(utility.status) }}
                </div>
              </div>

              <!-- Card Content -->
              <v-card-text class="pa-5">
                <h3 class="utility-name mb-2">{{ utility.name }}</h3>
                
                <div class="utility-meta mb-3">
                  <div class="meta-item">
                    <i class="fas fa-building mr-1"></i>
                    <span>{{ utility.buildingName }}</span>
                  </div>
                  <div class="meta-item" v-if="utility.location">
                    <i class="fas fa-map-marker-alt mr-1"></i>
                    <span>{{ utility.location }}</span>
                  </div>
                </div>

                <!-- Info Grid -->
                <div class="utility-info-grid">
                  <div class="info-row">
                    <span class="info-label"><i class="fas fa-tag mr-1"></i> Phí:</span>
                    <span class="info-value" v-if="utility.feePerUse">
                      {{ formatCurrency(utility.feePerUse) }}
                    </span>
                    <span class="info-value free" v-else>Miễn phí</span>
                  </div>

                  <div class="info-row" v-if="utility.operatingHours">
                    <span class="info-label"><i class="fas fa-clock mr-1"></i> Giờ:</span>
                    <span class="info-value">{{ utility.operatingHours }}</span>
                  </div>

                  <div class="info-row" v-if="utility.brand">
                    <span class="info-label"><i class="fas fa-certificate mr-1"></i> Hãng:</span>
                    <span class="info-value">{{ utility.brand }}</span>
                  </div>
                </div>

                <!-- Contact Button -->
                <v-btn
                  v-if="utility.managerPhone"
                  block
                  variant="outlined"
                  color="primary"
                  size="small"
                  class="mt-4"
                  @click="showContactModal(utility)"
                >
                  <i class="fas fa-phone mr-2"></i>
                  Liên hệ
                </v-btn>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- Contact Modal -->
    <v-dialog v-model="contactModalVisible" max-width="400">
      <v-card v-if="selectedUtility">
        <v-card-title class="text-h6 pa-5 bg-grey-lighten-4">
          <i class="fas fa-address-card mr-2 text-primary"></i>
          Thông tin liên hệ
        </v-card-title>
        
        <v-card-text class="pa-5">
          <h3 class="mb-3">{{ selectedUtility.name }}</h3>
          
          <div class="contact-info-list">
            <div class="contact-item" v-if="selectedUtility.managerName">
              <i class="fas fa-user contact-icon"></i>
              <div>
                <div class="contact-label">Người quản lý</div>
                <div class="contact-value">{{ selectedUtility.managerName }}</div>
              </div>
            </div>

            <div class="contact-item" v-if="selectedUtility.managerPhone">
              <i class="fas fa-phone contact-icon"></i>
              <div>
                <div class="contact-label">Điện thoại</div>
                <a :href="`tel:${selectedUtility.managerPhone}`" class="contact-value link">
                  {{ selectedUtility.managerPhone }}
                </a>
              </div>
            </div>

            <div class="contact-item" v-if="selectedUtility.managerEmail">
              <i class="fas fa-envelope contact-icon"></i>
              <div>
                <div class="contact-label">Email</div>
                <a :href="`mailto:${selectedUtility.managerEmail}`" class="contact-value link">
                  {{ selectedUtility.managerEmail }}
                </a>
              </div>
            </div>
          </div>
        </v-card-text>

        <v-card-actions class="pa-5 pt-0">
          <v-spacer></v-spacer>
          <v-btn color="primary" variant="flat" @click="contactModalVisible = false">
            Đóng
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import * as publicUtilityService from '@/services/publicUtilityService'

const loading = ref(false)
const utilities = ref([])
const buildings = ref([])
const selectedBuilding = ref('')
const selectedCategory = ref('')
const contactModalVisible = ref(false)
const selectedUtility = ref(null)

const categories = publicUtilityService.UTILITY_CATEGORIES

const totalUtilities = computed(() => utilities.value.length)
const availableCount = computed(() => 
  utilities.value.filter(u => u.status === 'Available').length
)

const filteredUtilities = computed(() => {
  return utilities.value.filter(u => {
    // Chỉ hiển thị tiện ích sẵn sàng hoặc đang dùng
    if (u.status !== 'Available' && u.status !== 'InUse') return false
    
    if (selectedBuilding.value && u.buildingId !== selectedBuilding.value) return false
    if (selectedCategory.value && u.category !== selectedCategory.value) return false
    
    return true
  })
})

onMounted(async () => {
  await loadBuildings()
  await loadUtilities()
})

const loadBuildings = async () => {
  try {
    buildings.value = await publicUtilityService.getAllBuildings()
  } catch (error) {
    console.error('Error loading buildings:', error)
  }
}

const loadUtilities = async () => {
  loading.value = true
  try {
    utilities.value = await publicUtilityService.getAllUtilities()
  } catch (error) {
    message.error('Không thể tải danh sách tiện ích')
    console.error('Error loading utilities:', error)
  } finally {
    loading.value = false
  }
}

// Map category to FontAwesome icon
const getCategoryFaIcon = (category) => {
  const iconMap = {
    WashingMachine: 'fas fa-tshirt',
    Dryer: 'fas fa-wind',
    Gym: 'fas fa-dumbbell',
    StudyRoom: 'fas fa-book-reader',
    Kitchen: 'fas fa-utensils',
    LaundryRoom: 'fas fa-soap',
    RecreationRoom: 'fas fa-gamepad',
    MeetingRoom: 'fas fa-users',
    Other: 'fas fa-box'
  }
  return iconMap[category] || 'fas fa-box'
}

const getStatusLabel = (status) => {
  const labels = {
    Available: 'Sẵn sàng',
    InUse: 'Đang sử dụng',
    Broken: 'Hỏng',
    UnderMaintenance: 'Bảo trì',
    Retired: 'Ngừng dùng'
  }
  return labels[status] || status
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { 
    style: 'currency', 
    currency: 'VND' 
  }).format(value)
}

const showContactModal = (utility) => {
  selectedUtility.value = utility
  contactModalVisible.value = true
}
</script>

<style scoped>
.facilities-page {
  min-height: 100vh;
  background: #f5f5f5;
}

/* Hero Section */
.hero-section {
  background: linear-gradient(135deg, #ff6b00 0%, #ff8c00 100%);
  color: white;
  padding: 60px 20px;
  text-align: center;
}

.hero-content h1 {
  font-size: 42px;
  font-weight: 700;
  margin: 0 0 16px 0;
}

.hero-content p {
  font-size: 18px;
  margin: 0;
  opacity: 0.95;
}

/* Filters */
.filters-section {
  background: white;
  padding: 24px 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

.filters {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

/* Utilities Section */
.utilities-section {
  padding: 40px 20px;
}

.utilities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 24px;
  margin-top: 24px;
}

.utility-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s, box-shadow 0.2s;
  position: relative;
}

.utility-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.12);
}

/* Status Badge */
.status-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.status-badge.Available {
  background: #e6f7e6;
  color: #16a34a;
}

.status-badge.InUse {
  background: #e6f0ff;
  color: #2563eb;
}

/* Icon */
.utility-icon {
  font-size: 48px;
  text-align: center;
  margin-bottom: 16px;
}

/* Content */
.utility-name {
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 8px 0;
  color: #1a1a1a;
}

.utility-building {
  font-size: 14px;
  color: #666;
  margin: 4px 0;
}

.utility-location {
  font-size: 13px;
  color: #999;
  margin: 4px 0 16px 0;
}

/* Info */
.utility-info {
  border-top: 1px solid #f0f0f0;
  padding-top: 16px;
  margin-top: 16px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.info-item .label {
  color: #666;
}

.info-item .value {
  font-weight: 600;
  color: #1a1a1a;
}

.info-item .value.free {
  color: #16a34a;
}

/* Contact */
.utility-contact {
  margin-top: 16px;
  text-align: center;
}

/* Empty State */
.empty-state {
  padding: 60px 20px;
  text-align: center;
}

/* Contact Modal */
.contact-info h3 {
  margin-bottom: 16px;
  color: #ff6b00;
}

/* Responsive */
@media (max-width: 768px) {
  .hero-content h1 {
    font-size: 32px;
  }

  .utilities-grid {
    grid-template-columns: 1fr;
  }
}
</style>


<style scoped>
/* Import FontAwesome */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');

.facilities-page {
  background: #fafafa;
  min-height: 100vh;
}

/* Hero Section */
.hero-section {
  background: linear-gradient(135deg, #ff6b00 0%, #ff8c42 100%);
  padding: 80px 0 100px;
  position: relative;
  overflow: hidden;
}

.hero-section::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320"><path fill="rgba(255,255,255,0.05)" d="M0,96L48,112C96,128,192,160,288,160C384,160,480,128,576,112C672,96,768,96,864,112C960,128,1056,160,1152,165.3C1248,171,1344,149,1392,138.7L1440,128L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"></path></svg>') bottom no-repeat;
  background-size: 100% 120px;
  opacity: 0.3;
}

.hero-container {
  position: relative;
  z-index: 1;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 8px 20px;
  border-radius: 30px;
  color: white;
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.hero-title {
  font-size: 48px;
  font-weight: 800;
  color: white;
  line-height: 1.2;
  text-shadow: 0 2px 20px rgba(0, 0, 0, 0.1);
}

.hero-subtitle {
  font-size: 16px;
  color: rgba(255, 255, 255, 0.95);
  line-height: 1.7;
}

/* Stats */
.stats-row {
  margin-top: 40px;
}

.stat-number {
  font-size: 40px;
  font-weight: 800;
  color: white;
  line-height: 1;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 12px;
  font-weight: 600;
  color: rgba(255, 255, 255, 0.8);
  letter-spacing: 1px;
}

/* Filter Card */
.filter-card {
  background: white;
  border-radius: 16px !important;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1) !important;
}

.filter-header {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: 700;
  color: #1a1a1a;
}

.text-orange {
  color: #ff6b00;
}

.filter-title {
  font-size: 18px;
  font-weight: 700;
}

.input-label {
  font-size: 11px;
  font-weight: 700;
  color: #6b7280;
  letter-spacing: 0.5px;
  text-transform: uppercase;
}

.filter-select {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e5e7eb;
  border-radius: 10px;
  font-size: 15px;
  font-weight: 500;
  color: #1f2937;
  background: white;
  transition: all 0.3s ease;
}

.filter-select:focus {
  outline: none;
  border-color: #ff6b00;
  box-shadow: 0 0 0 3px rgba(255, 107, 0, 0.1);
}

.search-btn {
  background: linear-gradient(135deg, #ff6b00 0%, #ff8c42 100%) !important;
  color: white !important;
  font-weight: 600 !important;
  text-transform: none !important;
  letter-spacing: 0.3px !important;
  border-radius: 10px !important;
  box-shadow: 0 4px 12px rgba(255, 107, 0, 0.3) !important;
}

.search-btn:hover {
  box-shadow: 0 6px 20px rgba(255, 107, 0, 0.4) !important;
}

/* Utilities Section */
.utilities-section {
  padding: 60px 0;
}

/* Utility Card */
.utility-card {
  border-radius: 16px !important;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 1px solid #e5e7eb !important;
  height: 100%;
}

.utility-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12) !important;
}

.utility-header {
  padding: 40px 20px;
  text-align: center;
  position: relative;
  background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%);
}

.utility-header.Available {
  background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
}

.utility-header.InUse {
  background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
}

.utility-icon-fa {
  font-size: 48px;
  color: #374151;
}

.utility-header.Available .utility-icon-fa {
  color: #059669;
}

.utility-header.InUse .utility-icon-fa {
  color: #2563eb;
}

.utility-status-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
  background: white;
  color: #6b7280;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.utility-header.Available .utility-status-badge {
  background: #065f46;
  color: white;
}

.utility-header.InUse .utility-status-badge {
  background: #1e40af;
  color: white;
}

.utility-name {
  font-size: 18px;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 8px;
}

.utility-meta {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.meta-item {
  display: flex;
  align-items: center;
  font-size: 13px;
  color: #6b7280;
  gap: 4px;
}

.meta-item i {
  color: #9ca3af;
  font-size: 12px;
}

/* Info Grid */
.utility-info-grid {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #e5e7eb;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  font-size: 13px;
}

.info-label {
  color: #6b7280;
  font-weight: 500;
}

.info-label i {
  color: #9ca3af;
  font-size: 12px;
  margin-right: 4px;
}

.info-value {
  font-weight: 600;
  color: #1f2937;
}

.info-value.free {
  color: #059669;
}

/* Contact Modal */
.contact-info-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.contact-item {
  display: flex;
  gap: 16px;
  align-items: flex-start;
}

.contact-icon {
  width: 40px;
  height: 40px;
  background: #f3f4f6;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ff6b00;
  font-size: 18px;
  flex-shrink: 0;
}

.contact-label {
  font-size: 12px;
  color: #6b7280;
  font-weight: 500;
  margin-bottom: 2px;
}

.contact-value {
  font-size: 15px;
  font-weight: 600;
  color: #1f2937;
}

.contact-value.link {
  color: #ff6b00;
  text-decoration: none;
}

.contact-value.link:hover {
  text-decoration: underline;
}

/* Responsive */
@media (max-width: 768px) {
  .hero-title {
    font-size: 36px;
  }

  .stat-number {
    font-size: 32px;
  }
}
</style>
