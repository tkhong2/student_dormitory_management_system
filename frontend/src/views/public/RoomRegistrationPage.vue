<template>
  <div class="room-registration-page">
    <!-- Hero Section -->
    <section class="hero-section">
      <v-container style="max-width: 1280px" class="hero-container">
        <v-row align="center">
          <!-- Left Content -->
          <v-col cols="12" md="6">
            <div class="hero-badge mb-4">
              <i class="fas fa-calendar-alt mr-2"></i>
              NĂM HỌC 2024-2025
            </div>
            
            <h1 class="hero-title mb-4">
              Tìm phòng phù hợp<br>với bạn
            </h1>
            
            <p class="hero-subtitle mb-8">
              Chọn phòng theo nhu cầu và ngân sách. Đăng ký nhanh,<br>
              nhận phòng dễ dàng tại Ký túc xá Đại học Đà Nẵng.
            </p>

            <!-- Stats -->
            <v-row class="stats-row mt-8">
              <v-col cols="4">
                <div class="stat-number">{{ stats.totalRooms }}</div>
                <div class="stat-label">PHÒNG CÓ SẴN</div>
              </v-col>
              <v-col cols="4">
                <div class="stat-number">{{ stats.totalBuildings }}</div>
                <div class="stat-label">TÒA NHÀ</div>
              </v-col>
              <v-col cols="4">
                <div class="stat-number">{{ stats.totalRoomTypes }}</div>
                <div class="stat-label">LOẠI PHÒNG</div>
              </v-col>
            </v-row>
          </v-col>

          <!-- Right Filter Card -->
          <v-col cols="12" md="6">
            <v-card class="filter-card pa-6 rounded-xl" elevation="4">
              <div class="filter-header mb-6">
                <i class="fas fa-search text-orange mr-2"></i>
                <span class="filter-title">Tìm phòng phù hợp</span>
              </div>

              <div class="mb-4">
                <div class="input-label mb-2">TÒA NHÀ</div>
                <select 
                  v-model="filters.building" 
                  class="filter-select"
                >
                  <option :value="null">Tất cả tòa nhà</option>
                  <option v-for="building in buildings" :key="building" :value="building">
                    {{ building }}
                  </option>
                </select>
              </div>

              <v-row class="mb-4">
                <v-col cols="6" class="pr-2">
                  <div class="input-label mb-2">LOẠI PHÒNG</div>
                  <select 
                    v-model="filters.roomType" 
                    class="filter-select"
                  >
                    <option :value="null">Tất cả</option>
                    <option v-for="roomType in roomTypes" :key="roomType" :value="roomType">
                      {{ roomType }}
                    </option>
                  </select>
                </v-col>
                <v-col cols="6" class="pl-2">
                  <div class="input-label mb-2">MỨC GIÁ</div>
                  <select 
                    v-model="filters.priceRange" 
                    class="filter-select"
                  >
                    <option :value="null">Tất cả</option>
                    <option v-for="price in priceRanges" :key="price" :value="price">
                      {{ price }}
                    </option>
                  </select>
                </v-col>
              </v-row>

              <v-btn
                block
                size="large"
                class="search-btn"
                @click="searchRooms"
              >
                <i class="fas fa-search mr-2"></i>
                Tìm kiếm phòng
              </v-btn>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- Announcement Bar -->
    <div class="announcement-bar">
      <v-container style="max-width: 1280px">
        <div class="announcement-content">
          <div v-for="building in buildingStats" :key="building.name" class="announcement-item">
            <i class="fas fa-map-marker-alt mr-2 announcement-icon"></i>
            {{ building.name }}: {{ building.availableRooms }} phòng trống
          </div>
          <div class="announcement-deadline">
            <i class="fas fa-calendar-check mr-2 announcement-icon"></i>
            Hạn đăng ký: 30/06/2025
          </div>
        </div>
      </v-container>
    </div>

    <!-- Room List Section -->
    <section class="room-list-section py-12">
      <v-container style="max-width: 1280px">
        <!-- Header -->
        <div class="section-header mb-6">
          <div>
            <h2 class="section-title">Phòng có sẵn</h2>
            <p class="section-subtitle">Tìm thấy {{ filteredRooms.length }} phòng phù hợp</p>
          </div>

          <!-- View Toggle -->
          <div class="view-toggle">
            <button
              :class="['view-btn', { active: viewMode === 'grid' }]"
              @click="viewMode = 'grid'"
            >
              <i class="fas fa-th"></i>
            </button>
            <button
              :class="['view-btn', { active: viewMode === 'list' }]"
              @click="viewMode = 'list'"
            >
              <i class="fas fa-list"></i>
            </button>
          </div>
        </div>

        <!-- Filter Chips -->
        <div class="filter-chips mb-6">
          <button
            :class="['filter-chip', { active: filterChip === 'all' }]"
            @click="filterChip = 'all'"
          >
            Tất cả
            <span class="chip-count">{{ rooms.length }}</span>
          </button>
          <button
            :class="['filter-chip', { active: filterChip === '2' }]"
            @click="filterChip = '2'"
          >
            2 người
            <span class="chip-count orange">{{ rooms.filter(r => r.capacity === 2).length }}</span>
          </button>
          <button
            :class="['filter-chip', { active: filterChip === '4' }]"
            @click="filterChip = '4'"
          >
            4 người
            <span class="chip-count orange">{{ rooms.filter(r => r.capacity === 4).length }}</span>
          </button>
          <button
            :class="['filter-chip', { active: filterChip === '6' }]"
            @click="filterChip = '6'"
          >
            6 người
            <span class="chip-count orange">{{ rooms.filter(r => r.capacity === 6).length }}</span>
          </button>
          <button
            :class="['filter-chip', { active: filterChip === 'wc' }]"
            @click="filterChip = 'wc'"
          >
            WC riêng
          </button>
          <button
            :class="['filter-chip', { active: filterChip === 'ac' }]"
            @click="filterChip = 'ac'"
          >
            Máy lạnh
          </button>
        </div>

        <!-- Room Cards Grid -->
        <v-row v-if="viewMode === 'grid' && !loading">
          <v-col v-for="room in paginatedRooms" :key="room.id" cols="12" sm="6" md="4" lg="3">
            <v-card class="room-card" elevation="2">
              <!-- Room Image -->
              <div class="room-image-wrapper">
                <v-img
                  :src="room.image"
                  height="200"
                  cover
                  class="room-image"
                />
                <div class="room-badge">
                  <span :class="['badge', room.available > 0 ? 'success' : 'error']">
                    {{ room.available }} chỗ trống
                  </span>
                </div>
                <div class="room-building">{{ room.building }}</div>
              </div>

              <!-- Room Info -->
              <v-card-text class="pa-4">
                <h3 class="room-name mb-3">{{ room.name }}</h3>

                <div class="room-details mb-4">
                  <div class="detail-item">
                    <i class="fas fa-users"></i>
                    {{ room.capacity }} người/phòng
                  </div>
                  <div class="detail-item">
                    <i class="fas fa-ruler-combined"></i>
                    {{ room.area }} m²
                  </div>
                  <div class="detail-item">
                    <i class="fas fa-check-circle"></i>
                    {{ room.facilitiesShort }}
                  </div>
                </div>

                <div class="room-footer">
                  <div class="room-price">{{ formatPrice(room.price) }}</div>
                  <button 
                    class="details-btn" 
                    @click.stop="viewRoomDetails(room)"
                    style="cursor: pointer; pointer-events: auto;"
                  >
                    Xem chi tiết
                  </button>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- Loading -->
        <div v-if="loading" class="loading-wrapper">
          <v-progress-circular indeterminate color="orange" size="64" />
          <p class="loading-text">Đang tải danh sách phòng...</p>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && filteredRooms.length === 0" class="empty-state">
          <i class="fas fa-search fa-4x mb-4"></i>
          <h3>Không tìm thấy phòng phù hợp</h3>
          <v-btn color="orange" class="mt-4 text-white" @click="resetFilters">
            Xem tất cả phòng
          </v-btn>
        </div>

        <!-- Pagination -->
        <div v-if="!loading && filteredRooms.length > 0" class="pagination-wrapper mt-8">
          <v-pagination
            v-model="pagination.current"
            :length="Math.ceil(filteredRooms.length / pagination.pageSize)"
            :total-visible="7"
            color="orange"
          />
        </div>
      </v-container>
    </section>

    <!-- Room Details Dialog - Using Ant Design Modal -->
    <a-modal 
      v-model:open="detailsDialog" 
      :width="1000"
      :footer="null"
      :destroy-on-close="true"
    >
      <div v-if="selectedRoom" class="details-dialog">
        <div class="dialog-header">
          <div>
            <h2 class="dialog-title">Phòng {{ selectedRoom.name }}</h2>
            <p class="dialog-subtitle">
              <i class="fas fa-building mr-1"></i>
              {{ selectedRoom.building }} · {{ selectedRoom.roomTypeName }}
            </p>
          </div>
        </div>

        <!-- Image Section -->
        <div class="detail-image-wrapper">
          <v-img :src="selectedRoom.image" height="300" cover />
          <div class="detail-overlay">
            <div class="overlay-badge">
              <i class="fas fa-door-open mr-2"></i>
              {{ selectedRoom.available }} chỗ trống
            </div>
            <div class="overlay-price">
              {{ formatPrice(selectedRoom.price) }}<span class="price-unit">/tháng</span>
            </div>
          </div>
        </div>

        <!-- Content Section -->
        <div class="pa-6">
          <!-- Quick Info Cards -->
          <v-row class="mb-5">
            <v-col cols="6" sm="3">
              <div class="info-card">
                  <div class="info-card-icon">
                    <i class="fas fa-users"></i>
                  </div>
                  <div class="info-card-value">{{ selectedRoom.capacity }}</div>
                  <div class="info-card-label">Người/phòng</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-card-icon orange">
                    <i class="fas fa-ruler-combined"></i>
                  </div>
                  <div class="info-card-value">{{ selectedRoom.area }}</div>
                  <div class="info-card-label">m² diện tích</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-card-icon green">
                    <i class="fas fa-check-circle"></i>
                  </div>
                  <div class="info-card-value">{{ selectedRoom.amenities.length }}</div>
                  <div class="info-card-label">Tiện nghi</div>
                </div>
              </v-col>
              <v-col cols="6" sm="3">
                <div class="info-card">
                  <div class="info-card-icon purple">
                    <i class="fas fa-bed"></i>
                  </div>
                  <div class="info-card-value">{{ selectedRoom.available }}</div>
                  <div class="info-card-label">Chỗ trống</div>
                </div>
              </v-col>
            </v-row>

            <v-row>
              <!-- Left Column -->
              <v-col cols="12" md="7">
                <!-- Room Type Info -->
                <div class="detail-section mb-5">
                  <h3 class="section-title">
                    <i class="fas fa-info-circle mr-2"></i>
                    Thông tin loại phòng
                  </h3>
                  <div class="info-table">
                    <div class="info-row">
                      <span class="info-label">Loại phòng:</span>
                      <strong>{{ selectedRoom.roomTypeName }}</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Số phòng:</span>
                      <strong>{{ selectedRoom.name }}</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Tòa nhà:</span>
                      <strong>{{ selectedRoom.building }}</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Giới tính:</span>
                      <strong>
                        <i :class="['fas', selectedRoom.buildingGender === 'Male' ? 'fa-mars' : selectedRoom.buildingGender === 'Female' ? 'fa-venus' : 'fa-venus-mars', 'mr-1']"></i>
                        {{ selectedRoom.buildingGender === 'Male' ? 'Nam' : selectedRoom.buildingGender === 'Female' ? 'Nữ' : 'Cả hai' }}
                      </strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Sức chứa:</span>
                      <strong>{{ selectedRoom.capacity }} người</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Diện tích:</span>
                      <strong>{{ selectedRoom.area }} m²</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Hiện tại:</span>
                      <strong>{{ selectedRoom.capacity - selectedRoom.available }}/{{ selectedRoom.capacity }} người</strong>
                    </div>
                    <div class="info-row">
                      <span class="info-label">Trạng thái:</span>
                      <span :class="['status-badge', selectedRoom.available > 0 ? 'success' : 'full']">
                        {{ selectedRoom.available > 0 ? 'Còn chỗ' : 'Hết chỗ' }}
                      </span>
                    </div>
                  </div>
                </div>

                <!-- Amenities -->
                <div class="detail-section">
                  <h3 class="section-title">
                    <i class="fas fa-star mr-2"></i>
                    Tiện nghi phòng
                  </h3>
                  <div class="amenities-grid">
                    <div v-for="amenity in selectedRoom.amenities" :key="amenity" class="amenity-item">
                      <i class="fas fa-check-circle amenity-icon"></i>
                      <span>{{ amenity }}</span>
                    </div>
                    <div v-if="selectedRoom.amenities.length === 0" class="text-muted">
                      Tiện nghi cơ bản
                    </div>
                  </div>
                </div>
              </v-col>

              <!-- Right Column -->
              <v-col cols="12" md="5">
                <!-- Price Card -->
                <div class="price-card mb-5">
                  <div class="price-card-header">
                    <i class="fas fa-tag"></i>
                    Giá thuê
                  </div>
                  <div class="price-card-body">
                    <div class="price-main">{{ formatPrice(selectedRoom.price) }}</div>
                    <div class="price-period">mỗi tháng</div>
                    <v-divider class="my-3" />
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
                  </div>
                </div>

                <!-- Highlights -->
                <div class="highlights-card">
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
                </div>

                <!-- Register Button -->
                <v-btn
                  block
                  size="x-large"
                  class="register-btn mt-5"
                  :disabled="selectedRoom.available === 0"
                  @click="registerRoom"
                >
                  <i class="fas fa-pen-to-square mr-2"></i>
                  {{ selectedRoom.available > 0 ? 'Đăng ký phòng này' : 'Hết chỗ trống' }}
                </v-btn>

                <div v-if="selectedRoom.available > 0" class="register-note mt-3">
                  <i class="fas fa-info-circle mr-1"></i>
                  Còn {{ selectedRoom.available }} chỗ trống
                </div>
              </v-col>
            </v-row>
          </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const router = useRouter()
const loading = ref(true)
const viewMode = ref('grid')
const detailsDialog = ref(false)
const selectedRoom = ref(null)
const filterChip = ref('all')

const filters = ref({
  building: null,
  roomType: null,
  priceRange: null
})

const pagination = ref({
  current: 1,
  pageSize: 12
})

const stats = ref({
  totalRooms: 0,
  totalBuildings: 0,
  totalRoomTypes: 0
})

const buildingStats = ref([])
const rooms = ref([])
const buildings = ref([])
const roomTypes = ref([])
const priceRanges = ref([
  'Dưới 500k',
  '500k - 1tr',
  '1tr - 1.5tr',
  'Trên 1.5tr'
])

onMounted(async () => {
  await loadData()
})

async function loadData() {
  loading.value = true
  try {
    const apiUrl = import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5000'
    const [buildingsData, roomTypesData, roomsData] = await Promise.all([
      axios.get(`${apiUrl}/api/buildings`).then(r => r.data),
      axios.get(`${apiUrl}/api/roomtypes`).then(r => r.data),
      axios.get(`${apiUrl}/api/rooms`).then(r => r.data)
    ])

    console.log('Buildings data:', buildingsData)
    console.log('Room types data:', roomTypesData)
    console.log('Rooms data:', roomsData)

    buildings.value = buildingsData.map(b => b.name)
    roomTypes.value = roomTypesData.map(rt => rt.name)
    
    console.log('Buildings array:', buildings.value)
    console.log('Room types array:', roomTypes.value)
    
    // Calculate building stats
    buildingStats.value = buildingsData.map(building => {
      const buildingRooms = roomsData.filter(r => r.buildingId === building.id)
      const availableRooms = buildingRooms.filter(r => {
        const available = r.maxOccupants - r.currentOccupants
        return available > 0 && (r.status === 'Available' || r.status === 'Maintenance')
      })
      return {
        name: building.name,
        availableRooms: availableRooms.length
      }
    })
    
    stats.value = {
      totalRooms: roomsData.filter(r => {
        const available = r.maxOccupants - r.currentOccupants
        return available > 0 && (r.status === 'Available' || r.status === 'Maintenance')
      }).length,
      totalBuildings: buildingsData.length,
      totalRoomTypes: roomTypesData.length
    }

    const roomTypeAmenitiesMap = {}
    for (const roomType of roomTypesData) {
      try {
        const amenitiesResponse = await axios.get(`${apiUrl}/api/roomtypes/${roomType.id}/amenities`)
        roomTypeAmenitiesMap[roomType.id] = amenitiesResponse.data || []
      } catch {
        roomTypeAmenitiesMap[roomType.id] = []
      }
    }
    
    rooms.value = roomsData
      .filter(room => {
        // CHỈ HIỂN THỊ PHÒNG CÒN CHỖ TRỐNG (không vượt quá capacity)
        const available = room.maxOccupants - room.currentOccupants
        return available > 0 && room.status === 'Available'
      })
      .map(room => {
        const roomType = roomTypesData.find(rt => rt.id === room.roomTypeId)
        const building = buildingsData.find(b => b.id === room.buildingId)
        
        const roomTypeAmenities = roomTypeAmenitiesMap[room.roomTypeId] || []
        const amenityNames = roomTypeAmenities.map(a => a.name)
        
        if (roomType?.hasAirConditioner && !amenityNames.includes('Điều hòa')) amenityNames.push('Điều hòa')
        if (roomType?.hasWaterHeater && !amenityNames.includes('Nóng lạnh')) amenityNames.push('Nóng lạnh')
        if (roomType?.hasPrivateBathroom && !amenityNames.includes('WC riêng')) amenityNames.push('WC riêng')
        
        return {
          id: room.id,
          name: room.roomNumber,
          building: building?.name || 'N/A',
          buildingId: room.buildingId,
          buildingGender: building?.gender || 'Mixed', // Thêm giới tính tòa nhà
          capacity: room.maxOccupants,
          area: roomType?.area || 25,
          price: roomType?.pricePerMonth || 0,
          available: Math.max(0, room.maxOccupants - room.currentOccupants),
          facilities: amenityNames.join(', ') || 'Tiện nghi cơ bản',
          facilitiesShort: amenityNames.slice(0, 2).join(', ') || 'Tiện nghi cơ bản',
          amenities: amenityNames,
          image: roomType?.thumbnailUrl || `https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=400&h=300&fit=crop&seed=${room.id}`,
          roomTypeId: room.roomTypeId,
          roomTypeName: roomType?.name || 'N/A',
          status: room.status
        }
      })
      .sort((a, b) => b.available - a.available)
    
    console.log('Final rooms array:', rooms.value.length, 'rooms')
  } catch (error) {
    console.error('Error loading data:', error)
  } finally {
    loading.value = false
  }
}

const filteredRooms = computed(() => {
  let result = rooms.value

  if (filters.value.building) {
    result = result.filter(r => r.building === filters.value.building)
  }

  if (filters.value.roomType) {
    result = result.filter(r => r.roomTypeName === filters.value.roomType)
  }

  if (filters.value.priceRange) {
    result = result.filter(r => {
      if (filters.value.priceRange === 'Dưới 500k') return r.price < 500000
      if (filters.value.priceRange === '500k - 1tr') return r.price >= 500000 && r.price < 1000000
      if (filters.value.priceRange === '1tr - 1.5tr') return r.price >= 1000000 && r.price < 1500000
      if (filters.value.priceRange === 'Trên 1.5tr') return r.price >= 1500000
      return true
    })
  }

  if (filterChip.value !== 'all') {
    if (['2', '4', '6'].includes(filterChip.value)) {
      result = result.filter(r => r.capacity === parseInt(filterChip.value))
    } else if (filterChip.value === 'wc') {
      result = result.filter(r => r.amenities.some(a => a.includes('WC')))
    } else if (filterChip.value === 'ac') {
      result = result.filter(r => r.amenities.some(a => a.includes('Điều hòa') || a.includes('Máy lạnh')))
    }
  }

  return result
})

const paginatedRooms = computed(() => {
  const start = (pagination.value.current - 1) * pagination.value.pageSize
  const end = start + pagination.value.pageSize
  return filteredRooms.value.slice(start, end)
})

function searchRooms() {
  pagination.value.current = 1
  filterChip.value = 'all' // Reset chip filter when using dropdowns
}

function resetFilters() {
  filters.value = { building: null, roomType: null, priceRange: null }
  filterChip.value = 'all'
  pagination.value.current = 1
}

function formatPrice(price) {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

function viewRoomDetails(room) {
  console.log('Navigating to room details for:', room)
  router.push(`/rooms/${room.id}`)
}

function registerRoom() {
  localStorage.setItem('selectedRoom', JSON.stringify(selectedRoom.value))
  router.push('/student/room-registration')
}
</script>

<style scoped>
.room-registration-page {
  background: #f5f5f5;
  min-height: 100vh;
}

/* Hero Section */
.hero-section {
  background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
  color: white;
}

.hero-container {
  padding-top: 80px !important;
  padding-bottom: 80px !important;
}

.hero-badge {
  background: rgba(255, 255, 255, 0.15);
  border-radius: 20px;
  padding: 6px 14px;
  display: inline-block;
  font-size: 12px;
  font-weight: 600;
}

.hero-title {
  font-size: 42px;
  font-weight: 700;
  line-height: 1.2;
}



.hero-subtitle {
  font-size: 15px;
  opacity: 0.85;
  line-height: 1.6;
}

.stats-row .stat-number {
  font-size: 32px;
  font-weight: 700;
}

.stats-row .stat-label {
  font-size: 12px;
  opacity: 0.75;
}

/* Filter Card */
.filter-card {
  background: white !important;
}

.filter-header {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-title {
  font-weight: 600;
  color: #666;
}

.input-label {
  font-size: 12px;
  color: #999;
  font-weight: 500;
  text-transform: uppercase;
}

.search-btn {
  background: #ff9800 !important;
  color: white !important;
  text-transform: none;
  font-weight: 600;
}

/* Custom Select Dropdown */
.filter-select {
  width: 100%;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  color: #333;
  background: white;
  cursor: pointer;
  transition: border-color 0.2s;
}

.filter-select:hover {
  border-color: #ff9800;
}

.filter-select:focus {
  outline: none;
  border-color: #ff9800;
  box-shadow: 0 0 0 2px rgba(255, 152, 0, 0.1);
}

.filter-select option {
  padding: 8px;
}

/* Announcement Bar */
.announcement-bar {
  background: #1a1a1a;
  padding: 16px 0;
}

.announcement-content {
  display: flex;
  gap: 40px;
  font-size: 13px;
  overflow-x: auto;
  white-space: nowrap;
  font-weight: 500;
}

.announcement-icon {
  color: white !important;
}

.announcement-item {
  color: white;
  display: inline-flex;
  align-items: center;
}

.announcement-deadline {
  margin-left: auto;
  color: white;
  display: inline-flex;
  align-items: center;
}

/* Room List Section */
.room-list-section {
  background: #fafafa;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.section-title {
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 4px;
}

.section-subtitle {
  color: #666;
  font-size: 14px;
}

.view-toggle {
  display: flex;
  gap: 8px;
}

.view-btn {
  width: 40px;
  height: 40px;
  border: 1px solid #ddd;
  background: white;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.view-btn.active {
  background: #ff9800;
  color: white;
  border-color: #ff9800;
}

/* Filter Chips */
.filter-chips {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.filter-chip {
  padding: 8px 16px;
  border: 1px solid #ddd;
  background: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-chip.active {
  background: #1976d2;
  color: white;
  border-color: #1976d2;
}

.chip-count {
  background: rgba(0, 0, 0, 0.1);
  padding: 2px 8px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 12px;
}

.chip-count.orange {
  background: rgba(255, 152, 0, 0.2);
  color: #ff9800;
}

/* Room Card */
.room-card {
  border-radius: 12px !important;
  overflow: hidden;
  height: 100%;
  transition: transform 0.2s;
}

.room-card:hover {
  transform: translateY(-4px);
}

.room-image-wrapper {
  position: relative;
  background: #2c3e50;
}

.room-image {
  opacity: 0.7;
}

.room-badge {
  position: absolute;
  top: 12px;
  right: 12px;
}

.badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}

.badge.success {
  background: #4caf50;
  color: white;
}

.badge.error {
  background: #f44336;
  color: white;
}

.room-building {
  position: absolute;
  bottom: 12px;
  left: 12px;
  color: white;
  font-size: 11px;
  opacity: 0.8;
}

.room-name {
  font-size: 16px;
  font-weight: 700;
}

.room-details {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #666;
}

.detail-item i {
  width: 16px;
  color: #666;
}

.room-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.room-price {
  font-size: 20px;
  font-weight: 700;
  color: #ff9800;
}

.details-btn {
  background: #2c3e50;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 13px;
  transition: background 0.2s;
}

.details-btn:hover {
  background: #34495e;
}

/* Loading & Empty */
.loading-wrapper,
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #666;
}

.loading-text {
  margin-top: 16px;
}

/* Pagination */
.pagination-wrapper {
  display: flex;
  justify-content: center;
}

/* Dialog */
.details-dialog .dialog-header {
  background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
  color: white;
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.dialog-title {
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 4px;
}

.dialog-subtitle {
  font-size: 14px;
  opacity: 0.9;
  display: flex;
  align-items: center;
  gap: 4px;
}

.close-btn {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.detail-image-wrapper {
  position: relative;
}

.detail-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(to top, rgba(0, 0, 0, 0.7), transparent);
  padding: 24px;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
}

.overlay-badge {
  background: rgba(255, 255, 255, 0.95);
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
}

.overlay-price {
  color: white;
  font-size: 32px;
  font-weight: 700;
}

.price-unit {
  font-size: 16px;
  font-weight: 400;
  opacity: 0.9;
}

/* Info Cards */
.info-card {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
  text-align: center;
  border: 2px solid #e9ecef;
  transition: all 0.2s;
}

.info-card:hover {
  border-color: #ff9800;
  transform: translateY(-2px);
}

.info-card-icon {
  width: 48px;
  height: 48px;
  background: #2c3e50;
  color: white;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 12px;
  font-size: 20px;
}

.info-card-icon.orange {
  background: #ff9800;
}

.info-card-icon.green {
  background: #4caf50;
}

.info-card-icon.purple {
  background: #9c27b0;
}

.info-card-value {
  font-size: 24px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 4px;
}

.info-card-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Detail Sections */
.detail-section {
  background: white;
}

.section-title {
  font-size: 18px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
}

.section-title i {
  color: #ff9800;
}

.info-table {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e9ecef;
}

.info-row:last-child {
  border-bottom: none;
}

.info-label {
  color: #666;
  font-size: 14px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
}

.status-badge.success {
  background: #d4edda;
  color: #155724;
}

.status-badge.full {
  background: #f8d7da;
  color: #721c24;
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
  gap: 10px;
  padding: 10px 14px;
  background: #f8f9fa;
  border-radius: 8px;
  font-size: 14px;
  border: 1px solid #e9ecef;
}

.amenity-icon {
  color: #4caf50;
  font-size: 16px;
}

/* Price Card */
.price-card {
  background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
  color: white;
  border-radius: 16px;
  overflow: hidden;
}

.price-card-header {
  padding: 16px 20px;
  font-size: 14px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(0, 0, 0, 0.1);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.price-card-body {
  padding: 24px 20px;
}

.price-main {
  font-size: 36px;
  font-weight: 700;
  margin-bottom: 4px;
}

.price-period {
  font-size: 14px;
  opacity: 0.9;
  margin-bottom: 16px;
}

.price-includes {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.include-item {
  display: flex;
  align-items: center;
  font-size: 13px;
  opacity: 0.95;
}

/* Highlights Card */
.highlights-card {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 20px;
  border: 1px solid #e9ecef;
}

.highlight-item {
  display: flex;
  gap: 16px;
  padding: 12px 0;
  border-bottom: 1px solid #e9ecef;
}

.highlight-item:last-child {
  border-bottom: none;
}

.highlight-icon {
  width: 44px;
  height: 44px;
  background: white;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ff9800;
  font-size: 18px;
  flex-shrink: 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.highlight-title {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 2px;
}

.highlight-text {
  font-size: 12px;
  color: #666;
}

/* Register Button & Note */
.register-btn {
  background: #ff9800 !important;
  color: white !important;
  text-transform: none;
  font-weight: 600;
  font-size: 16px !important;
  box-shadow: 0 4px 12px rgba(255, 152, 0, 0.3) !important;
}

.register-btn:disabled {
  background: #ccc !important;
  box-shadow: none !important;
}

.register-note {
  text-align: center;
  font-size: 13px;
  color: #4caf50;
  font-weight: 500;
}

.info-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
}

.price-highlight {
  font-size: 18px;
  color: #ff9800;
}

.amenities-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.amenity-chip {
  background: #f5f5f5;
  padding: 6px 12px;
  border-radius: 16px;
  font-size: 13px;
  border: 1px solid #e0e0e0;
}

.text-muted {
  color: #999;
  font-size: 14px;
  font-style: italic;
}
</style>
