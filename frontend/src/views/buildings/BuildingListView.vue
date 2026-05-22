<template>
  <v-container fluid class="pa-4">
    <div class="d-flex align-center justify-space-between mb-8">
      <div>
        <h2 class="text-h4 font-weight-black mb-1">Quản lý Tòa nhà</h2>
        <p class="text-subtitle-2 text-medium-emphasis">Danh sách các dãy nhà trong khuôn viên KTX</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary" class="rounded-lg font-weight-bold" @click="dialog = true">Thêm tòa nhà</v-btn>
    </div>

    <v-row>
      <v-col v-for="b in buildings" :key="b.id" cols="12" sm="6" lg="3">
        <BuildingCard :building="b" @view="handleView" />
      </v-col>
    </v-row>

    <!-- Add Building Dialog -->
    <v-dialog v-model="dialog" max-width="600">
      <v-card class="rounded-xl pa-6">
        <v-card-title class="text-h5 font-weight-black px-0">Thêm tòa nhà mới</v-card-title>
        <v-card-text class="px-0 py-6">
          <v-form>
            <v-row>
              <v-col cols="12">
                <v-text-field label="Tên tòa nhà" placeholder="Ví dụ: A1, B2" variant="outlined" color="primary" rounded="lg" />
              </v-col>
              <v-col cols="6">
                <v-text-field label="Số tầng" type="number" variant="outlined" color="primary" rounded="lg" />
              </v-col>
              <v-col cols="6">
                <v-text-field label="Số phòng dự kiến" type="number" variant="outlined" color="primary" rounded="lg" />
              </v-col>
              <v-col cols="12">
                <v-select label="Khu vực" :items="['Khu A', 'Khu B', 'Khu C']" variant="outlined" color="primary" rounded="lg" />
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions class="px-0">
          <v-spacer />
          <v-btn variant="text" class="rounded-lg font-weight-bold" @click="dialog = false">Hủy</v-btn>
          <v-btn color="primary" variant="flat" class="rounded-lg font-weight-bold px-6" @click="dialog = false">Lưu thông tin</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import BuildingCard from '@/components/buildings/BuildingCard.vue'

const dialog = ref(false)
const buildings = ref([
  { id: 1, name: 'A1', floors: 5, rooms: 100, capacity: 400, status: 'Active', image: '/images/hero_dormitory.png' },
  { id: 2, name: 'A2', floors: 5, rooms: 100, capacity: 400, status: 'Active', image: '/images/student_life.png' },
  { id: 3, name: 'B1', floors: 7, rooms: 140, capacity: 560, status: 'Active', image: '/images/hero_dormitory.png' },
  { id: 4, name: 'C1', floors: 4, rooms: 80, capacity: 320, status: 'Maintenance', image: '/images/student_life.png' },
])

const handleView = (b) => {
  console.log('View building', b)
}
</script>

<style scoped>
</style>
