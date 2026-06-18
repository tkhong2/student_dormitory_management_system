<template>
  <div class="room-detail-page">
    <!-- Loading -->
    <div v-if="loading" class="loading-container">
      <v-progress-circular indeterminate color="orange" size="64" />
      <p class="mt-4">Đang tải thông tin phòng...</p>
    </div>

    <!-- Room Detail Content -->
    <div v-else-if="room" class="detail-content">
      <!-- Hero Image -->
      <section class="hero-image-section">
        <v-img :src="room.image" height="400" cover class="room-hero-image" />
        <div class="hero-overlay">
          <v-container style="max-width: 1280px">
            <v-btn icon variant="text" color="white" @click="goBack" class="back-btn">
              <i class="fas fa-arrow-left"></i>
            </v-btn>
            <div class="hero-content">
              <h1 class="hero-title">Phòng {{ room.name }}</h1>
              <p class="hero-subtitle">
                <i class="fas fa-building mr-2"></i>
                {{ room.building }} · {{ room.roomTypeName }}
              </p>
            </div>
          </v-container>
        </div>
      </section>

      <!-- Main Content -->
      <v-container style="max-width: 1280px" class="py-8">
        <v-row>
          <!-- Left Column - Info -->
          <v-col cols="12" md="8">
            <!-- Status Badge -->
            <div class="mb-6">
              <span :class="['status-badge', room.available > 0 ? 'success' : 'full']">
                <i :class="['fas', room.available > 0 ? 'fa-check-circle' : 'fa-times-circle', 'mr-2']"></i>
                {{ room.available > 0 ? `Còn ${room.available} chỗ trống` : 'Hết chỗ' }}
              </span>
            </div>

            <!-- Quick Info Cards -->
            <v-row class="mb-6">
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-icon primary">
                    <i class="fas fa-users"></i>
                  </div>
                  <div class="info-value">{{ room.capacity }}</div>
                  <div class="info-label">Người/phòng</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-icon orange">
                    <i class="fas fa-ruler-combined"></i>
                  </div>
                  <div class="info-value">{{ room.area }}</div>
                  <div class="info-label">m² diện tích</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-icon green">
                    <i class="fas fa-check-circle"></i>
                  </div>
                  <div class="info-value">{{ room.amenities.length }}</div>
                  <div class="info-label">Tiện nghi</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-icon purple">
                    <i class="fas fa-bed"></i>
                  </div>
                  <div class="info-value">{{ room.available }}/{{ room.capacity }}</div>
                  <div class="info-label">Chỗ trống</div>
                </div>
              </v-col>
            </v-row>

            <!-- Room Information -->
            <v-card class="mb-6">
              <v-card-title class="section-title">
                <i class="fas fa-info-circle mr-2"></i>
                Thông tin phòng
              </v-card-title>
              <v-card-text>
                <div class="info-table">
                  <div class="info-row">
                    <span class="info-label">Loại phòng:</span>
                    <strong>{{ room.roomTypeName }}</strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Số phòng:</span>
                    <strong>{{ room.name }}</strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Tòa nhà:</span>
                    <strong>{{ room.building }}</strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Giới tính:</span>
                    <strong>
                      <i :class="['fas', room.buildingGender === 'Male' ? 'fa-mars' : room.buildingGender === 'Female' ? 'fa-venus' : 'fa-venus-mars', 'mr-1']"></i>
                      {{ room.buildingGender === 'Male' ? 'Nam' : room.buildingGender === 'Female' ? 'Nữ' : 'Cả hai' }}
                    </strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Sức chứa:</span>
                    <strong>{{ room.capacity }} người</strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Diện tích:</span>
                    <strong>{{ room.area }} m²</strong>
                  </div>
                  <div class="info-row">
                    <span class="info-label">Hiện tại:</span>
                    <strong>{{ room.capacity - room.available }}/{{ room.capacity }} người</strong>
                  </div>
                </div>
              </v-card-text>
            </v-card>

            <!-- Amenities -->
            <v-card>
              <v-card-title class="section-title">
                <i class="fas fa-star mr-2"></i>
                Tiện nghi phòng
              </v-card-title>
              <v-card-text>
                <div class="amenities-grid">
                  <div v-for="amenity in room.amenities" :key="amenity" class="amenity-item">
                    <i class="fas fa-check-circle amenity-icon"></i>
                    <span>{{ amenity }}</span>
                  </div>
                  <div v-if="room.amenities.length === 0" class="text-muted">
                    Tiện nghi cơ bản
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Right Column - Booking -->
          <v-col cols="12" md="4">
            <!-- Price Card -->
            <v-card class="price-card sticky-card mb-4">
              <v-card-text>
                <div class="price-header">
                  <i class="fas fa-tag mr-2"></i>
                  Giá thuê
                </div>
                <div class="price-main">{{ formatPrice(room.price) }}</div>
                <div class="price-period">mỗi tháng</div>
                
                <v-divider class="my-4" />
                
                <div class="price-includes">
                  <div class="include-item">
                    <i class="fas fa-check mr-2"></i>
                    Tiền phòng cố định
                  </div>
                  <div class="include-item">
                    <i class="fas fa-plus mr-2"></i>
                    Tiền điện (theo số)
                  </div>
                  <div class="include-item">
                    <i class="fas fa-plus mr-2"></i>
                    Tiền nước (theo số)
                  </div>
                  <div class="include-item">
                    <i class="fas fa-plus mr-2"></i>
                    Phí dịch vụ
                  </div>
                </div>

                <v-btn
                  block
                  size="x-large"
                  class="register-btn mt-4"
                  color="#ff9800"
                  :disabled="room.available === 0"
                  @click="registerRoom"
                >
                  <i class="fas fa-pen-to-square mr-2"></i>
                  {{ room.available > 0 ? 'Đăng ký phòng này' : 'Hết chỗ trống' }}
                </v-btn>

                <div v-if="room.available > 0" class="register-note mt-3">
                  <i class="fas fa-info-circle mr-1"></i>
                  Còn {{ room.available }} chỗ trống
                </div>
              </v-card-text>
            </v-card>

            <!-- Highlights Card -->
            <v-card>
              <v-card-text>
                <div class="highlight-item">
                  <i class="fas fa-shield-alt highlight-icon"></i>
                  <div>
                    <div class="highlight-title">An toàn</div>
                    <div class="highlight-text">Bảo vệ 24/7, camera giám sát</div>
                  </div>
                </div>
                <div class="highlight-item">
                  <i class="fas fa-wifi highlight-icon"></i>
                  <div>
                    <div class="highlight-title">Internet</div>
                    <div class="highlight-text">Wifi tốc độ cao miễn phí</div>
                  </div>
                </div>
                <div class="highlight-item">
                  <i class="fas fa-broom highlight-icon"></i>
                  <div>
                    <div class="highlight-title">Vệ sinh</div>
                    <div class="highlight-text">Dịch vụ dọn dẹp định kỳ</div>
                  </div>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </div>

    <!-- Error State -->
    <div v-else class="error-container">
      <i class="fas fa-exclamation-triangle fa-4x mb-4 text-warning"></i>
      <h2>Không tìm thấy thông tin phòng</h2>
      <v-btn color="orange" class="mt-4" @click="goBack">
        <i class="fas fa-arrow-left mr-2"></i>
        Quay lại danh sách
      </v-btn>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const route = useRoute()
const loading = ref(true)
const room = ref(null)

onMounted(async () => {
  await loadRoomDetail()
})

async function loadRoomDetail() {
  loading.value = true
  try {
    const roomId = route.params.id
    
    // Fetch room data
    const [roomData, buildingsData, roomTypesData] = await Promise.all([
      axios.get(`http://localhost:5003/api/rooms/${roomId}`).then(r => r.data),
      axios.get('http://localhost:5003/api/buildings').then(r => r.data),
      axios.get('http://localhost:5003/api/roomtypes').then(r => r.data)
    ])

    const roomType = roomTypesData.find(rt => rt.id === roomData.roomTypeId)
    const building = buildingsData.find(b => b.id === roomData.buildingId)
    
    // Fetch amenities
    let amenityNames = []
    try {
      const amenitiesResponse = await axios.get(`http://localhost:5003/api/roomtypes/${roomData.roomTypeId}/amenities`)
      amenityNames = amenitiesResponse.data.map(a => a.name)
    } catch {
      amenityNames = []
    }
    
    if (roomType?.hasAirConditioner && !amenityNames.includes('Điều hòa')) amenityNames.push('Điều hòa')
    if (roomType?.hasWaterHeater && !amenityNames.includes('Nóng lạnh')) amenityNames.push('Nóng lạnh')
    if (roomType?.hasPrivateBathroom && !amenityNames.includes('WC riêng')) amenityNames.push('WC riêng')
    
    room.value = {
      id: roomData.id,
      name: roomData.roomNumber,
      building: building?.name || 'N/A',
      buildingId: roomData.buildingId,
      buildingGender: building?.gender || 'Mixed',
      capacity: roomData.maxOccupants,
      area: roomType?.area || 25,
      price: roomType?.pricePerMonth || 0,
      available: Math.max(0, roomData.maxOccupants - roomData.currentOccupants),
      amenities: amenityNames,
      image: roomType?.thumbnailUrl || `https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=800&h=400&fit=crop&seed=${roomData.id}`,
      roomTypeId: roomData.roomTypeId,
      roomTypeName: roomType?.name || 'N/A',
      status: roomData.status
    }
  } catch (error) {
    console.error('Error loading room detail:', error)
    room.value = null
  } finally {
    loading.value = false
  }
}

function formatPrice(price) {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

function goBack() {
  router.push('/rooms')
}

function registerRoom() {
  localStorage.setItem('selectedRoom', JSON.stringify(room.value))
  router.push('/student/room-registration')
}
</script>

<style scoped>
.room-detail-page {
  min-height: 100vh;
  background: #fafafa;
}

.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  padding: 40px;
}

/* Hero Section */
.hero-image-section {
  position: relative;
  height: 400px;
  overflow: hidden;
}

.room-hero-image {
  width: 100%;
  height: 100%;
}

.hero-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.8), transparent);
  padding: 40px 0 30px;
  color: white;
}

.back-btn {
  position: absolute;
  top: 20px;
  left: 20px;
  background: rgba(0,0,0,0.5) !important;
}

.hero-content {
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 24px;
}

.hero-title {
  font-size: 36px;
  font-weight: 700;
  margin-bottom: 8px;
}

.hero-subtitle {
  font-size: 16px;
  opacity: 0.9;
}

/* Status Badge */
.status-badge {
  display: inline-block;
  padding: 8px 16px;
  border-radius: 20px;
  font-weight: 600;
  font-size: 14px;
}

.status-badge.success {
  background: #d1fae5;
  color: #065f46;
}

.status-badge.full {
  background: #fee2e2;
  color: #991b1b;
}

/* Info Cards */
.info-card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}

.info-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 12px;
  font-size: 20px;
}

.info-icon.primary {
  background: #eff6ff;
  color: #1e40af;
}

.info-icon.orange {
  background: #ffedd5;
  color: #ea580c;
}

.info-icon.green {
  background: #d1fae5;
  color: #065f46;
}

.info-icon.purple {
  background: #f3e8ff;
  color: #6b21a8;
}

.info-value {
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 4px;
}

.info-label {
  font-size: 13px;
  color: #6b7280;
}

/* Section Title */
.section-title {
  font-size: 18px;
  font-weight: 600;
  display: flex;
  align-items: center;
}

/* Info Table */
.info-table .info-row {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid #f3f4f6;
}

.info-table .info-row:last-child {
  border-bottom: none;
}

.info-table .info-label {
  color: #6b7280;
}

/* Amenities Grid */
.amenities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 12px;
}

.amenity-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 0;
}

.amenity-icon {
  color: #22c55e;
  font-size: 16px;
}

/* Price Card - Remove sticky, keep it fixed in place */
.sticky-card {
  /* Removed sticky positioning */
}

@media (max-width: 960px) {
  .sticky-card {
    /* No special positioning on mobile */
  }
}

.price-header {
  font-size: 14px;
  color: #6b7280;
  margin-bottom: 8px;
}

.price-main {
  font-size: 32px;
  font-weight: 700;
  color: #ff9800;
}

.price-period {
  font-size: 14px;
  color: #6b7280;
  margin-bottom: 16px;
}

.price-includes .include-item {
  display: flex;
  align-items: center;
  padding: 8px 0;
  font-size: 14px;
  color: #4b5563;
}

.register-btn {
  text-transform: none;
  font-weight: 600;
  letter-spacing: 0;
}

.register-note {
  text-align: center;
  font-size: 13px;
  color: #22c55e;
  font-weight: 500;
}

/* Highlights */
.highlight-item {
  display: flex;
  gap: 16px;
  padding: 16px 0;
  border-bottom: 1px solid #f3f4f6;
}

.highlight-item:last-child {
  border-bottom: none;
}

.highlight-icon {
  width: 40px;
  height: 40px;
  background: #eff6ff;
  color: #2563eb;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.highlight-title {
  font-weight: 600;
  margin-bottom: 4px;
}

.highlight-text {
  font-size: 13px;
  color: #6b7280;
}
</style>
