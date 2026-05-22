<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Quản lý Phòng ở</h1>
        <p class="text-body-2 text-medium-emphasis">Tổng {{ rooms.length }} phòng — {{ rooms.filter(r=>r.status==='Trống').length }} phòng trống</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary">Thêm phòng</v-btn>
    </div>

    <!-- Filters -->
    <v-card class="pa-4 mb-6" style="border:1px solid #e5e7eb">
      <v-row align="center">
        <v-col cols="12" sm="3">
          <v-select v-model="filterBuilding" label="Tòa nhà" :items="['Tất cả','Tòa A1','Tòa A2','Tòa B1','Tòa B2','Tòa C1']" density="compact" hide-details />
        </v-col>
        <v-col cols="12" sm="3">
          <v-select v-model="filterStatus" label="Trạng thái" :items="['Tất cả','Trống','Đang ở','Đã đầy','Bảo trì']" density="compact" hide-details />
        </v-col>
        <v-col cols="12" sm="3">
          <v-select label="Tầng" :items="['Tất cả','Tầng 1','Tầng 2','Tầng 3','Tầng 4','Tầng 5']" density="compact" hide-details />
        </v-col>
        <v-col cols="12" sm="3">
          <v-text-field placeholder="Tìm số phòng..." prepend-inner-icon="mdi-magnify" density="compact" hide-details />
        </v-col>
      </v-row>
    </v-card>

    <!-- Room Grid -->
    <v-row>
      <v-col v-for="r in filteredRooms" :key="r.id" cols="6" sm="4" md="3" lg="2">
        <v-card
          style="border:1px solid #e5e7eb; cursor:pointer; transition: all .2s"
          class="text-center pa-4 room-card"
          @click="selectRoom(r)"
        >
          <div
            class="mx-auto mb-3 d-flex align-center justify-center rounded-lg"
            :style="{width:'48px',height:'48px',background:getStatusBg(r.status)}"
          >
            <v-icon :color="getStatusColor(r.status)" size="24">mdi-door</v-icon>
          </div>
          <div class="text-subtitle-2 font-weight-bold">{{ r.number }}</div>
          <div class="text-caption text-medium-emphasis mb-2">{{ r.building }} · T{{ r.floor }}</div>
          <v-chip :color="getStatusColor(r.status)" size="x-small" variant="tonal">{{ r.status }}</v-chip>
          <v-progress-linear :model-value="r.occupancy/r.capacity*100" :color="getStatusColor(r.status)" height="4" rounded class="mt-3" />
          <div class="text-caption text-medium-emphasis mt-1">{{ r.occupancy }}/{{ r.capacity }}</div>
        </v-card>
      </v-col>
    </v-row>

    <!-- Room Detail Drawer -->
    <v-navigation-drawer v-model="detailDrawer" location="right" width="400" temporary>
      <div v-if="selectedRoom" class="pa-6">
        <div class="d-flex align-center justify-space-between mb-6">
          <h3 class="text-h5 font-weight-bold">Phòng {{ selectedRoom.number }}</h3>
          <v-btn icon="mdi-close" variant="text" @click="detailDrawer=false" />
        </div>
        <v-chip :color="getStatusColor(selectedRoom.status)" class="mb-4">{{ selectedRoom.status }}</v-chip>
        <v-list density="compact">
          <v-list-item title="Tòa nhà" :subtitle="selectedRoom.building" />
          <v-list-item title="Tầng" :subtitle="'Tầng '+selectedRoom.floor" />
          <v-list-item title="Loại phòng" subtitle="Phòng Tiêu chuẩn (4 SV)" />
          <v-list-item title="Giá thuê" subtitle="800.000₫/tháng" />
          <v-list-item title="Sức chứa" :subtitle="selectedRoom.occupancy+'/'+selectedRoom.capacity+' sinh viên'" />
        </v-list>
        <v-divider class="my-4" />
        <h4 class="text-subtitle-2 font-weight-bold mb-3">Sinh viên trong phòng</h4>
        <div v-if="selectedRoom.occupancy>0">
          <div v-for="i in selectedRoom.occupancy" :key="i" class="d-flex align-center ga-3 mb-3">
            <v-avatar size="32" color="primary" variant="tonal">
              <v-icon size="16">mdi-account</v-icon>
            </v-avatar>
            <div>
              <div class="text-body-2 font-weight-medium">Sinh viên {{ i }}</div>
              <div class="text-caption text-medium-emphasis">SV00{{ i }}</div>
            </div>
          </div>
        </div>
        <div v-else class="text-center py-6 text-medium-emphasis text-body-2">Chưa có sinh viên</div>
      </div>
    </v-navigation-drawer>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
const filterBuilding = ref('Tất cả')
const filterStatus = ref('Tất cả')
const detailDrawer = ref(false)
const selectedRoom = ref(null)

const rooms = ref([
  { id:1,number:'101',building:'Tòa A1',floor:1,status:'Đang ở',occupancy:3,capacity:4 },
  { id:2,number:'102',building:'Tòa A1',floor:1,status:'Đã đầy',occupancy:4,capacity:4 },
  { id:3,number:'103',building:'Tòa A1',floor:1,status:'Trống',occupancy:0,capacity:4 },
  { id:4,number:'104',building:'Tòa A1',floor:1,status:'Bảo trì',occupancy:0,capacity:4 },
  { id:5,number:'201',building:'Tòa A1',floor:2,status:'Đang ở',occupancy:2,capacity:4 },
  { id:6,number:'202',building:'Tòa A1',floor:2,status:'Trống',occupancy:0,capacity:4 },
  { id:7,number:'101',building:'Tòa A2',floor:1,status:'Đã đầy',occupancy:4,capacity:4 },
  { id:8,number:'102',building:'Tòa A2',floor:1,status:'Đang ở',occupancy:1,capacity:4 },
  { id:9,number:'101',building:'Tòa B1',floor:1,status:'Đã đầy',occupancy:8,capacity:8 },
  { id:10,number:'102',building:'Tòa B1',floor:1,status:'Đang ở',occupancy:5,capacity:8 },
  { id:11,number:'301',building:'Tòa C1',floor:3,status:'Trống',occupancy:0,capacity:2 },
  { id:12,number:'302',building:'Tòa C1',floor:3,status:'Đang ở',occupancy:1,capacity:2 },
])

const filteredRooms = computed(() => rooms.value.filter(r =>
  (filterBuilding.value === 'Tất cả' || r.building === filterBuilding.value) &&
  (filterStatus.value === 'Tất cả' || r.status === filterStatus.value)
))

const selectRoom = r => { selectedRoom.value = r; detailDrawer.value = true }
const getStatusColor = s => ({ 'Trống':'success','Đã đầy':'error','Bảo trì':'warning','Đang ở':'info' }[s]||'grey')
const getStatusBg = s => ({ 'Trống':'#dcfce7','Đã đầy':'#fef2f2','Bảo trì':'#fff7ed','Đang ở':'#e0f2fe' }[s]||'#f3f4f6')
</script>

<style scoped>
.room-card:hover { transform:translateY(-3px); box-shadow: 0 8px 25px -5px rgba(0,0,0,.1); border-color:#c7d2fe!important; }
</style>
