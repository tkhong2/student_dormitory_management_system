<template>
  <div>
    <!-- Hero Section -->
    <section class="hero-section">
      <v-container style="max-width: 1280px;">
        <div class="text-center py-16">
          <h1 class="text-h3 text-md-h2 font-weight-bold mb-4">Đăng ký phòng ký túc xá</h1>
          <p class="text-h6 text-md-h5 opacity-80 mb-8">Chọn phòng phù hợp với nhu cầu của bạn</p>
          
          <!-- Search & Filter -->
          <v-card class="mx-auto pa-6 rounded-xl elevation-8" max-width="900">
            <v-row align="center">
              <v-col cols="12" md="3">
                <v-select
                  v-model="filters.building"
                  :items="buildings"
                  label="Tòa nhà"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  clearable
                />
              </v-col>
              <v-col cols="12" md="3">
                <v-select
                  v-model="filters.roomType"
                  :items="roomTypes"
                  label="Loại phòng"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  clearable
                />
              </v-col>
              <v-col cols="12" md="3">
                <v-select
                  v-model="filters.priceRange"
                  :items="priceRanges"
                  label="Mức giá"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  clearable
                />
              </v-col>
              <v-col cols="12" md="3">
                <v-btn
                  color="primary"
                  size="large"
                  block
                  class="font-weight-bold"
                  @click="searchRooms"
                >
                  <v-icon start>mdi-magnify</v-icon>
                  Tìm kiếm
                </v-btn>
              </v-col>
            </v-row>
          </v-card>
        </div>
      </v-container>
    </section>

    <!-- Room List Section -->
    <section class="py-12" style="background-color: #f8fafc;">
      <v-container style="max-width: 1280px;">
        <div class="d-flex justify-space-between align-center mb-8">
          <div>
            <h2 class="text-h4 font-weight-bold mb-2">Phòng có sẵn</h2>
            <p class="text-body-1 opacity-70">Tìm thấy {{ filteredRooms.length }} phòng phù hợp</p>
          </div>
          <v-btn-toggle v-model="viewMode" mandatory density="compact" color="primary">
            <v-btn value="grid" icon="mdi-view-grid" />
            <v-btn value="list" icon="mdi-view-list" />
          </v-btn-toggle>
        </div>

        <!-- Grid View -->
        <v-row v-if="viewMode === 'grid'">
          <v-col v-for="room in filteredRooms" :key="room.id" cols="12" sm="6" lg="4">
            <v-card class="room-card rounded-xl" elevation="2">
              <div class="room-image">
                <v-img :src="room.image" height="200" cover>
                  <div class="room-badge">
                    <v-chip color="success" size="small" class="font-weight-bold">
                      {{ room.available }} chỗ trống
                    </v-chip>
                  </div>
                </v-img>
              </div>
              
              <v-card-text class="pa-6">
                <div class="d-flex justify-space-between align-center mb-3">
                  <h3 class="text-h6 font-weight-bold">{{ room.name }}</h3>
                  <v-chip color="primary" size="small" variant="tonal">{{ room.building }}</v-chip>
                </div>
                
                <div class="mb-4">
                  <div class="d-flex align-center mb-2">
                    <v-icon size="18" color="primary" class="mr-2">mdi-account-group</v-icon>
                    <span class="text-body-2">{{ room.capacity }} người/phòng</span>
                  </div>
                  <div class="d-flex align-center mb-2">
                    <v-icon size="18" color="primary" class="mr-2">mdi-ruler-square</v-icon>
                    <span class="text-body-2">{{ room.area }} m²</span>
                  </div>
                  <div class="d-flex align-center">
                    <v-icon size="18" color="primary" class="mr-2">mdi-air-conditioner</v-icon>
                    <span class="text-body-2">{{ room.facilities }}</span>
                  </div>
                </div>

                <v-divider class="my-4" />

                <div class="d-flex justify-space-between align-center">
                  <div>
                    <div class="text-caption opacity-70">Giá thuê/tháng</div>
                    <div class="text-h6 font-weight-bold text-primary">{{ formatPrice(room.price) }}</div>
                  </div>
                  <v-btn
                    color="primary"
                    variant="flat"
                    class="font-weight-bold"
                    @click="openRegistrationDialog(room)"
                  >
                    Đăng ký
                  </v-btn>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>

        <!-- List View -->
        <div v-else>
          <v-card v-for="room in filteredRooms" :key="room.id" class="mb-4 rounded-xl" elevation="2">
            <v-row no-gutters>
              <v-col cols="12" md="4">
                <v-img :src="room.image" height="100%" min-height="200" cover>
                  <div class="room-badge">
                    <v-chip color="success" size="small" class="font-weight-bold">
                      {{ room.available }} chỗ trống
                    </v-chip>
                  </div>
                </v-img>
              </v-col>
              <v-col cols="12" md="8">
                <v-card-text class="pa-6">
                  <div class="d-flex justify-space-between align-center mb-3">
                    <div>
                      <h3 class="text-h6 font-weight-bold mb-1">{{ room.name }}</h3>
                      <v-chip color="primary" size="small" variant="tonal">{{ room.building }}</v-chip>
                    </div>
                    <div class="text-right">
                      <div class="text-caption opacity-70">Giá thuê/tháng</div>
                      <div class="text-h6 font-weight-bold text-primary">{{ formatPrice(room.price) }}</div>
                    </div>
                  </div>

                  <v-row class="mb-4">
                    <v-col cols="4">
                      <div class="d-flex align-center">
                        <v-icon size="18" color="primary" class="mr-2">mdi-account-group</v-icon>
                        <span class="text-body-2">{{ room.capacity }} người</span>
                      </div>
                    </v-col>
                    <v-col cols="4">
                      <div class="d-flex align-center">
                        <v-icon size="18" color="primary" class="mr-2">mdi-ruler-square</v-icon>
                        <span class="text-body-2">{{ room.area }} m²</span>
                      </div>
                    </v-col>
                    <v-col cols="4">
                      <div class="d-flex align-center">
                        <v-icon size="18" color="primary" class="mr-2">mdi-air-conditioner</v-icon>
                        <span class="text-body-2">{{ room.facilities }}</span>
                      </div>
                    </v-col>
                  </v-row>

                  <div class="d-flex justify-end">
                    <v-btn
                      color="primary"
                      variant="flat"
                      class="font-weight-bold"
                      @click="openRegistrationDialog(room)"
                    >
                      Đăng ký ngay
                    </v-btn>
                  </div>
                </v-card-text>
              </v-col>
            </v-row>
          </v-card>
        </div>

        <!-- Empty State -->
        <v-card v-if="filteredRooms.length === 0" class="pa-12 text-center rounded-xl" elevation="0">
          <v-icon size="80" color="grey-lighten-2">mdi-home-search-outline</v-icon>
          <h3 class="text-h6 font-weight-bold mt-4 mb-2">Không tìm thấy phòng phù hợp</h3>
          <p class="text-body-2 opacity-70">Vui lòng thử lại với bộ lọc khác</p>
        </v-card>
      </v-container>
    </section>

    <!-- Registration Dialog -->
    <v-dialog v-model="registrationDialog" max-width="600">
      <v-card class="rounded-xl">
        <v-card-title class="pa-6 bg-primary text-white">
          <h3 class="text-h6 font-weight-bold">Đăng ký phòng {{ selectedRoom?.name }}</h3>
        </v-card-title>

        <v-card-text class="pa-6">
          <v-form ref="registrationForm">
            <v-row>
              <v-col cols="12">
                <v-text-field
                  v-model="registrationData.fullName"
                  label="Họ và tên *"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng nhập họ tên']"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="registrationData.studentId"
                  label="Mã sinh viên *"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng nhập mã sinh viên']"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="registrationData.phone"
                  label="Số điện thoại *"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng nhập số điện thoại']"
                />
              </v-col>
              <v-col cols="12">
                <v-text-field
                  v-model="registrationData.email"
                  label="Email *"
                  type="email"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng nhập email']"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-select
                  v-model="registrationData.gender"
                  :items="['Nam', 'Nữ']"
                  label="Giới tính *"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng chọn giới tính']"
                />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="registrationData.dateOfBirth"
                  label="Ngày sinh *"
                  type="date"
                  variant="outlined"
                  density="comfortable"
                  :rules="[v => !!v || 'Vui lòng nhập ngày sinh']"
                />
              </v-col>
              <v-col cols="12">
                <v-textarea
                  v-model="registrationData.address"
                  label="Địa chỉ thường trú"
                  variant="outlined"
                  density="comfortable"
                  rows="2"
                />
              </v-col>
              <v-col cols="12">
                <v-textarea
                  v-model="registrationData.note"
                  label="Ghi chú"
                  variant="outlined"
                  density="comfortable"
                  rows="2"
                  placeholder="Yêu cầu đặc biệt hoặc thông tin bổ sung..."
                />
              </v-col>
            </v-row>
          </v-form>

          <v-alert type="info" variant="tonal" class="mt-4">
            <div class="text-body-2">
              <strong>Lưu ý:</strong> Sau khi gửi đăng ký, bạn sẽ nhận được email xác nhận trong vòng 24h. 
              Vui lòng chuẩn bị đầy đủ giấy tờ theo yêu cầu.
            </div>
          </v-alert>
        </v-card-text>

        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn
            variant="text"
            @click="registrationDialog = false"
          >
            Hủy
          </v-btn>
          <v-btn
            color="primary"
            variant="flat"
            class="font-weight-bold px-6"
            @click="submitRegistration"
          >
            Gửi đăng ký
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Success Dialog -->
    <v-dialog v-model="successDialog" max-width="500">
      <v-card class="rounded-xl pa-8">
        <div class="text-center">
          <div class="d-flex justify-center mb-4">
            <v-icon size="80" color="success">mdi-check-circle</v-icon>
          </div>
          <h3 class="text-h5 font-weight-bold mb-2">Đăng ký thành công!</h3>
          <p class="text-body-1 opacity-70 mb-6">
            Chúng tôi đã nhận được đơn đăng ký của bạn. Vui lòng kiểm tra email để biết thêm chi tiết.
          </p>
          <v-btn color="primary" variant="flat" class="font-weight-bold" @click="successDialog = false">
            Đóng
          </v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const viewMode = ref('grid')
const registrationDialog = ref(false)
const successDialog = ref(false)
const selectedRoom = ref(null)
const registrationForm = ref(null)

const filters = ref({
  building: null,
  roomType: null,
  priceRange: null
})

const registrationData = ref({
  fullName: '',
  studentId: '',
  phone: '',
  email: '',
  gender: '',
  dateOfBirth: '',
  address: '',
  note: ''
})

const buildings = ['Tòa A1', 'Tòa A2', 'Tòa B1', 'Tòa B2', 'Tòa C1']
const roomTypes = ['Phòng 2 người', 'Phòng 4 người', 'Phòng 6 người', 'Phòng 8 người']
const priceRanges = ['Dưới 500k', '500k - 1tr', '1tr - 1.5tr', 'Trên 1.5tr']

// Sample room data
const rooms = ref([
  {
    id: 1,
    name: 'Phòng 101',
    building: 'Tòa A1',
    capacity: 4,
    area: 25,
    price: 800000,
    available: 2,
    facilities: 'Điều hòa, Nóng lạnh',
    image: 'https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=400&h=300&fit=crop'
  },
  {
    id: 2,
    name: 'Phòng 102',
    building: 'Tòa A1',
    capacity: 6,
    area: 35,
    price: 600000,
    available: 3,
    facilities: 'Quạt trần, Nóng lạnh',
    image: 'https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=400&h=300&fit=crop'
  },
  {
    id: 3,
    name: 'Phòng 201',
    building: 'Tòa A2',
    capacity: 2,
    area: 20,
    price: 1200000,
    available: 1,
    facilities: 'Điều hòa, Nóng lạnh, Ban công',
    image: 'https://images.unsplash.com/photo-1540518614846-7eded433c457?w=400&h=300&fit=crop'
  },
  {
    id: 4,
    name: 'Phòng 202',
    building: 'Tòa A2',
    capacity: 4,
    area: 28,
    price: 900000,
    available: 2,
    facilities: 'Điều hòa, Nóng lạnh',
    image: 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=400&h=300&fit=crop'
  },
  {
    id: 5,
    name: 'Phòng 301',
    building: 'Tòa B1',
    capacity: 8,
    area: 45,
    price: 500000,
    available: 4,
    facilities: 'Quạt trần, Nóng lạnh',
    image: 'https://images.unsplash.com/photo-1595526114035-0d45ed16cfbf?w=400&h=300&fit=crop'
  },
  {
    id: 6,
    name: 'Phòng 302',
    building: 'Tòa B1',
    capacity: 6,
    area: 32,
    price: 650000,
    available: 2,
    facilities: 'Điều hòa, Nóng lạnh',
    image: 'https://images.unsplash.com/photo-1616594039964-ae9021a400a0?w=400&h=300&fit=crop'
  }
])

const filteredRooms = computed(() => {
  let result = rooms.value

  if (filters.value.building) {
    result = result.filter(r => r.building === filters.value.building)
  }

  if (filters.value.roomType) {
    const capacity = parseInt(filters.value.roomType.match(/\d+/)[0])
    result = result.filter(r => r.capacity === capacity)
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

  return result
})

const formatPrice = (price) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price)
}

const searchRooms = () => {
  // Filter is reactive, so this just triggers a re-render
  console.log('Searching with filters:', filters.value)
}

const openRegistrationDialog = (room) => {
  selectedRoom.value = room
  registrationDialog.value = true
}

const submitRegistration = async () => {
  const { valid } = await registrationForm.value.validate()
  
  if (valid) {
    // TODO: Send registration data to API
    console.log('Registration data:', {
      ...registrationData.value,
      roomId: selectedRoom.value.id,
      roomName: selectedRoom.value.name
    })

    registrationDialog.value = false
    successDialog.value = true

    // Reset form
    registrationData.value = {
      fullName: '',
      studentId: '',
      phone: '',
      email: '',
      gender: '',
      dateOfBirth: '',
      address: '',
      note: ''
    }
  }
}
</script>

<style scoped>
.hero-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.room-card {
  transition: all 0.3s ease;
  height: 100%;
}

.room-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15) !important;
}

.room-image {
  position: relative;
  overflow: hidden;
}

.room-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 1;
}

.opacity-70 {
  opacity: 0.7;
}

.opacity-80 {
  opacity: 0.8;
}
</style>
