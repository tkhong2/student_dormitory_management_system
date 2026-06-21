<template>
  <section class="facilities-section">
    <div class="container">
      <div class="section-header">
        <h2>Tiện Ích Chung</h2>
        <p>Khám phá các tiện ích hiện đại phục vụ sinh viên</p>
      </div>

      <!-- Category Tabs -->
      <div class="category-tabs">
        <button 
          v-for="cat in categories" 
          :key="cat.value"
          :class="['tab-btn', { active: selectedCategory === cat.value }]"
          @click="selectedCategory = cat.value"
        >
          <i :class="cat.icon"></i>
          {{ cat.label }}
        </button>
      </div>

      <!-- Facilities Grid -->
      <div class="facilities-grid">
        <div v-for="facility in filteredFacilities" :key="facility.id" class="facility-card">
          <div class="facility-image">
            <img :src="facility.imageUrl || '/default-facility.jpg'" :alt="facility.name">
            <span v-if="facility.isFeatured" class="badge-featured">
              <i class="fas fa-star"></i> Nổi bật
            </span>
          </div>

          <div class="facility-body">
            <h3>{{ facility.name }}</h3>
            <p class="facility-brand" v-if="facility.brandName">
              <i class="fas fa-award"></i> {{ facility.brandName }}
            </p>
            
            <p class="facility-description">
              {{ facility.description || 'Chưa có mô tả' }}
            </p>

            <div class="facility-details">
              <div class="detail-item" v-if="facility.location">
                <i class="fas fa-map-marker-alt"></i>
                <span>{{ facility.location }}</span>
              </div>

              <div class="detail-item" v-if="facility.operatingHours">
                <i class="fas fa-clock"></i>
                <span>{{ getOperatingHoursText(facility.operatingHours) }}</span>
              </div>

              <div class="detail-item" v-if="facility.feePerHour || facility.feePerSession">
                <i class="fas fa-tag"></i>
                <span>
                  <template v-if="facility.feePerSession">
                    {{ formatCurrency(facility.feePerSession) }}/lần
                  </template>
                  <template v-else-if="facility.feePerHour">
                    {{ formatCurrency(facility.feePerHour) }}/giờ
                  </template>
                  <template v-else>
                    Miễn phí
                  </template>
                </span>
              </div>
            </div>

            <!-- Manager Info -->
            <div class="manager-info" v-if="facility.managerName">
              <div class="manager-header">
                <i class="fas fa-user-tie"></i>
                <strong>Người quản lý</strong>
              </div>
              <p class="manager-name">{{ facility.managerName }}</p>
              <div class="contact-links">
                <a v-if="facility.managerPhone" :href="`tel:${facility.managerPhone}`" class="contact-link">
                  <i class="fas fa-phone"></i> {{ facility.managerPhone }}
                </a>
                <a v-if="facility.managerEmail" :href="`mailto:${facility.managerEmail}`" class="contact-link">
                  <i class="fas fa-envelope"></i> Email
                </a>
                <template v-if="facility.socialLinks">
                  <a 
                    v-for="(url, social) in parseSocialLinks(facility.socialLinks)" 
                    :key="social"
                    :href="url" 
                    target="_blank"
                    class="contact-link"
                  >
                    <i :class="getSocialIcon(social)"></i> {{ social }}
                  </a>
                </template>
              </div>
            </div>

            <!-- Rating & Stats -->
            <div class="facility-footer">
              <div class="rating" v-if="facility.averageRating">
                <i class="fas fa-star"></i>
                <span>{{ facility.averageRating.toFixed(1) }}</span>
                <small>({{ facility.reviewCount }})</small>
              </div>
              <div class="usage-count">
                <i class="fas fa-users"></i>
                <span>{{ facility.totalUsageCount }} lượt sử dụng</span>
              </div>
            </div>

            <!-- Actions -->
            <div class="facility-actions">
              <button 
                v-if="facility.isBookingRequired" 
                class="btn btn-primary"
                @click="bookFacility(facility)"
              >
                <i class="fas fa-calendar-check"></i> Đặt lịch
              </button>
              <button class="btn btn-outline" @click="viewDetails(facility)">
                <i class="fas fa-info-circle"></i> Chi tiết
              </button>
            </div>
          </div>
        </div>
      </div>

      <div v-if="filteredFacilities.length === 0" class="no-facilities">
        <i class="fas fa-inbox"></i>
        <p>Chưa có tiện ích nào trong danh mục này</p>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'

const router = useRouter()
const facilities = ref([])
const selectedCategory = ref('')

const categories = [
  { value: '', label: 'Tất cả', icon: 'fas fa-th' },
  { value: 'Fitness', label: 'Thể thao', icon: 'fas fa-dumbbell' },
  { value: 'Study', label: 'Học tập', icon: 'fas fa-book' },
  { value: 'Recreation', label: 'Giải trí', icon: 'fas fa-gamepad' },
  { value: 'Laundry', label: 'Giặt ủi', icon: 'fas fa-tshirt' },
  { value: 'Parking', label: 'Bãi đỗ xe', icon: 'fas fa-parking' },
]

const filteredFacilities = computed(() => {
  if (!selectedCategory.value) return facilities.value
  return facilities.value.filter(f => f.category === selectedCategory.value)
})

const loadFacilities = async () => {
  try {
    const response = await axios.get('/api/publicfacilities/homepage')
    facilities.value = response.data
  } catch (error) {
    console.error('Error loading facilities:', error)
  }
}

const bookFacility = (facility) => {
  router.push({ name: 'FacilityBooking', params: { id: facility.id } })
}

const viewDetails = (facility) => {
  router.push({ name: 'FacilityDetail', params: { id: facility.id } })
}

const parseSocialLinks = (jsonString) => {
  try {
    return JSON.parse(jsonString)
  } catch {
    return {}
  }
}

const getSocialIcon = (social) => {
  const icons = {
    'facebook': 'fab fa-facebook',
    'zalo': 'fas fa-comment-dots',
    'instagram': 'fab fa-instagram',
    'twitter': 'fab fa-twitter'
  }
  return icons[social.toLowerCase()] || 'fas fa-link'
}

const getOperatingHoursText = (jsonString) => {
  try {
    const hours = JSON.parse(jsonString)
    return hours.monday || '6:00 - 22:00'
  } catch {
    return '6:00 - 22:00'
  }
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value)
}

onMounted(() => {
  loadFacilities()
})
</script>

<style scoped>
.facilities-section {
  padding: 60px 0;
  background: #f8f9fa;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.section-header {
  text-align: center;
  margin-bottom: 40px;
}

.section-header h2 {
  font-size: 36px;
  color: #333;
  margin-bottom: 10px;
}

.section-header p {
  font-size: 18px;
  color: #666;
}

.category-tabs {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-bottom: 40px;
  flex-wrap: wrap;
}

.tab-btn {
  padding: 12px 24px;
  border: 2px solid #ddd;
  background: white;
  color: #666;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s;
  font-size: 14px;
  font-weight: 500;
}

.tab-btn i {
  margin-right: 8px;
}

.tab-btn:hover {
  border-color: #007bff;
  color: #007bff;
}

.tab-btn.active {
  background: #007bff;
  color: white;
  border-color: #007bff;
}

.facilities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 30px;
}

.facility-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0,0,0,0.07);
  transition: all 0.3s;
}

.facility-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 8px 16px rgba(0,0,0,0.12);
}

.facility-image {
  position: relative;
  height: 220px;
  overflow: hidden;
}

.facility-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.facility-card:hover .facility-image img {
  transform: scale(1.05);
}

.badge-featured {
  position: absolute;
  top: 15px;
  right: 15px;
  background: linear-gradient(135deg, #ffd700, #ffed4e);
  color: #333;
  padding: 8px 15px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: bold;
  box-shadow: 0 2px 8px rgba(255, 215, 0, 0.4);
}

.badge-featured i {
  margin-right: 5px;
}

.facility-body {
  padding: 20px;
}

.facility-body h3 {
  font-size: 20px;
  color: #333;
  margin: 0 0 10px 0;
}

.facility-brand {
  color: #007bff;
  font-size: 14px;
  margin: 0 0 10px 0;
  font-weight: 500;
}

.facility-description {
  color: #666;
  font-size: 14px;
  line-height: 1.6;
  margin: 15px 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.facility-details {
  margin: 15px 0;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 8px 0;
  font-size: 14px;
  color: #555;
}

.detail-item i {
  width: 18px;
  color: #007bff;
}

.manager-info {
  background: #f8f9fa;
  padding: 15px;
  border-radius: 8px;
  margin: 15px 0;
}

.manager-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
  color: #333;
}

.manager-name {
  font-weight: 500;
  margin: 5px 0;
}

.contact-links {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 10px;
}

.contact-link {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: 6px 12px;
  background: white;
  border: 1px solid #ddd;
  border-radius: 4px;
  color: #007bff;
  text-decoration: none;
  font-size: 13px;
  transition: all 0.2s;
}

.contact-link:hover {
  background: #007bff;
  color: white;
  border-color: #007bff;
}

.facility-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 0;
  border-top: 1px solid #eee;
  margin-top: 15px;
}

.rating {
  display: flex;
  align-items: center;
  gap: 5px;
  color: #ffc107;
  font-weight: 500;
}

.rating small {
  color: #999;
  margin-left: 5px;
}

.usage-count {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 14px;
  color: #666;
}

.facility-actions {
  display: flex;
  gap: 10px;
}

.btn {
  flex: 1;
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover {
  background: #0056b3;
}

.btn-outline {
  background: white;
  color: #007bff;
  border: 1px solid #007bff;
}

.btn-outline:hover {
  background: #007bff;
  color: white;
}

.no-facilities {
  text-align: center;
  padding: 60px 20px;
  color: #999;
}

.no-facilities i {
  font-size: 48px;
  margin-bottom: 20px;
  color: #ddd;
}

.no-facilities p {
  font-size: 16px;
}
</style>
