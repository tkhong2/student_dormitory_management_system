<template>
  <v-container fluid class="pa-4">
    <div class="d-flex align-center justify-space-between mb-8">
      <div>
        <h2 class="text-h4 font-weight-black mb-1">Quản lý Sinh viên</h2>
        <p class="text-subtitle-2 text-medium-emphasis">Tổng số: {{ students.length }} sinh viên nội trú</p>
      </div>
      <v-btn prepend-icon="mdi-account-plus" color="primary" class="rounded-lg font-weight-bold">Thêm sinh viên</v-btn>
    </div>

    <v-card flat border class="rounded-xl overflow-hidden">
      <v-card-title class="pa-6 d-flex flex-wrap align-center ga-4">
        <v-text-field
          v-model="search"
          placeholder="Tìm theo tên, mã SV, CMND..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          density="comfortable"
          rounded="lg"
          hide-details
          class="max-width-300 flex-grow-1"
        />
        <v-select
          placeholder="Tòa nhà"
          :items="['Tất cả', 'Tòa A1', 'Tòa A2', 'Tòa B1']"
          variant="outlined"
          density="comfortable"
          rounded="lg"
          hide-details
          class="max-width-150"
        />
        <v-select
          placeholder="Trạng thái"
          :items="['Đang ở', 'Đã rời đi', 'Sắp hết hạn']"
          variant="outlined"
          density="comfortable"
          rounded="lg"
          hide-details
          class="max-width-150"
        />
        <v-spacer class="d-none d-lg-block" />
        <v-btn icon="mdi-refresh" variant="text" />
        <v-btn icon="mdi-filter-variant" variant="text" />
      </v-card-title>

      <v-data-table
        :headers="headers"
        :items="students"
        :search="search"
        hover
      >
        <template v-slot:item.name="{ item }">
          <div class="d-flex align-center ga-3 py-2">
            <v-avatar size="32" color="primary-lighten-4">
              <v-img :src="`https://i.pravatar.cc/150?u=${item.id}`" />
            </v-avatar>
            <div class="font-weight-bold">{{ item.name }}</div>
          </div>
        </template>

        <template v-slot:item.status="{ item }">
          <v-chip :color="item.status === 'Đang ở' ? 'success' : 'warning'" size="x-small" variant="flat" class="font-weight-bold px-2">
            {{ item.status }}
          </v-chip>
        </template>

        <template v-slot:item.actions="{ item }">
          <div class="d-flex ga-1">
            <v-btn icon="mdi-pencil-outline" size="x-small" variant="text" color="primary" />
            <v-btn icon="mdi-eye-outline" size="x-small" variant="text" color="info" />
            <v-btn icon="mdi-delete-outline" size="x-small" variant="text" color="error" />
          </div>
        </template>
      </v-data-table>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'

const search = ref('')
const headers = [
  { title: 'Sinh viên', key: 'name', sortable: true },
  { title: 'Mã SV', key: 'studentId' },
  { title: 'Phòng', key: 'room' },
  { title: 'Tòa', key: 'building' },
  { title: 'Lớp', key: 'class' },
  { title: 'Ngày vào', key: 'joinDate' },
  { title: 'Trạng thái', key: 'status' },
  { title: 'Thao tác', key: 'actions', align: 'end', sortable: false },
]

const students = ref([
  { id: 1, name: 'Nguyễn Văn An', studentId: 'SV001', room: '101', building: 'A1', class: 'K65-CNTT', joinDate: '15/05/2026', status: 'Đang ở' },
  { id: 2, name: 'Trần Thị Bình', studentId: 'SV002', room: '102', building: 'A1', class: 'K64-KT', joinDate: '20/05/2026', status: 'Đang ở' },
  { id: 3, name: 'Lê Văn Cường', studentId: 'SV003', room: '201', building: 'B1', class: 'K66-NN', joinDate: '01/06/2026', status: 'Sắp hết hạn' },
  { id: 4, name: 'Phạm Thị Dung', studentId: 'SV004', room: '305', building: 'C1', class: 'K65-QTKD', joinDate: '10/06/2026', status: 'Đang ở' },
])
</script>

<style scoped>
.max-width-300 { max-width: 300px; }
.max-width-150 { max-width: 150px; }
:deep(.v-data-table__th) { font-weight: 800 !important; text-transform: uppercase; font-size: 11px !important; letter-spacing: 0.5px; color: #64748b !important; }
</style>
