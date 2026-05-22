<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Loại phòng</h1>
        <p class="text-body-2 text-medium-emphasis">Quản lý các loại phòng và giá thuê</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary" @click="dialog=true">Thêm loại phòng</v-btn>
    </div>

    <v-row>
      <v-col v-for="t in types" :key="t.id" cols="12" sm="6" lg="4">
        <v-card style="border:1px solid #e5e7eb" class="pa-5">
          <div class="d-flex align-center mb-4">
            <v-avatar :color="t.color" size="44" rounded="lg" variant="tonal">
              <v-icon>{{ t.icon }}</v-icon>
            </v-avatar>
            <div class="ml-3">
              <div class="text-subtitle-1 font-weight-bold">{{ t.name }}</div>
              <div class="text-caption text-medium-emphasis">{{ t.capacity }} sinh viên/phòng</div>
            </div>
            <v-spacer />
            <v-menu>
              <template #activator="{ props }"><v-btn icon="mdi-dots-vertical" size="small" variant="text" v-bind="props" /></template>
              <v-list density="compact">
                <v-list-item prepend-icon="mdi-pencil-outline" title="Sửa" />
                <v-list-item prepend-icon="mdi-delete-outline" title="Xóa" class="text-error" />
              </v-list>
            </v-menu>
          </div>
          <p class="text-body-2 text-medium-emphasis mb-4">{{ t.description }}</p>
          <v-divider class="mb-4" />
          <div class="d-flex justify-space-between align-center">
            <div>
              <span class="text-h5 font-weight-bold text-primary">{{ t.price }}</span>
              <span class="text-body-2 text-medium-emphasis">/tháng</span>
            </div>
            <v-chip size="small" variant="tonal" color="info">{{ t.count }} phòng</v-chip>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="dialog" max-width="500">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Thêm loại phòng</h2>
        <v-text-field label="Tên loại phòng" class="mb-3" />
        <v-row>
          <v-col cols="6"><v-text-field label="Sức chứa" type="number" suffix="SV" /></v-col>
          <v-col cols="6"><v-text-field label="Giá thuê" type="number" suffix="đ/tháng" /></v-col>
        </v-row>
        <v-textarea label="Mô tả tiện nghi" rows="3" class="mt-1" />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog=false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog=false">Lưu</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
const dialog = ref(false)
const types = ref([
  { id:1, name:'Phòng Tiêu chuẩn', capacity:4, price:'800.000₫', description:'Giường tầng, quạt trần, nhà vệ sinh chung', count:180, icon:'mdi-bed-outline', color:'primary' },
  { id:2, name:'Phòng Cao cấp', capacity:2, price:'1.500.000₫', description:'Giường đơn, điều hòa, nhà vệ sinh riêng', count:60, icon:'mdi-bed-king-outline', color:'secondary' },
  { id:3, name:'Phòng VIP', capacity:1, price:'2.500.000₫', description:'Phòng đơn, đầy đủ tiện nghi, ban công', count:20, icon:'mdi-star-outline', color:'warning' },
  { id:4, name:'Phòng 8 giường', capacity:8, price:'500.000₫', description:'Giường tầng, quạt trần, nhà vệ sinh chung khu', count:40, icon:'mdi-bunk-bed-outline', color:'info' },
])
</script>
