<template>
  <v-card flat border class="building-card rounded-xl overflow-hidden h-100">
    <v-img :src="building.image || '/images/hero_dormitory.png'" height="180" cover>
      <div class="pa-4 d-flex justify-end">
        <v-chip :color="building.status === 'Active' ? 'success' : 'error'" variant="flat" size="small" class="font-weight-bold">
          {{ building.status === 'Active' ? 'Hoạt động' : 'Bảo trì' }}
        </v-chip>
      </div>
    </v-img>
    
    <v-card-text class="pa-6">
      <div class="d-flex align-center justify-space-between mb-4">
        <div>
          <h3 class="text-h6 font-weight-black mb-1">Tòa {{ building.name }}</h3>
          <div class="text-caption font-weight-bold opacity-60">
            <v-icon size="14" class="mr-1">mdi-map-marker</v-icon> {{ building.location || 'Khuôn viên KTX' }}
          </div>
        </div>
        <v-btn icon="mdi-dots-vertical" variant="text" size="small" />
      </div>

      <v-row dense class="mb-4">
        <v-col cols="6">
          <div class="pa-3 bg-grey-lighten-4 rounded-lg text-center">
            <div class="text-caption font-weight-bold opacity-60 uppercase mb-1">Số tầng</div>
            <div class="text-h6 font-weight-black">{{ building.floors || 0 }}</div>
          </div>
        </v-col>
        <v-col cols="6">
          <div class="pa-3 bg-grey-lighten-4 rounded-lg text-center">
            <div class="text-caption font-weight-bold opacity-60 uppercase mb-1">Số phòng</div>
            <div class="text-h6 font-weight-black">{{ building.rooms || 0 }}</div>
          </div>
        </v-col>
      </v-row>

      <div class="d-flex align-center justify-space-between">
        <div class="text-caption font-weight-bold opacity-70">Sức chứa: <strong>{{ building.capacity || 0 }}</strong></div>
        <v-btn color="primary" variant="tonal" class="rounded-lg font-weight-bold" @click="$emit('view', building)">Chi tiết</v-btn>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup>
defineProps({
  building: { type: Object, required: true }
})
defineEmits(['view'])
</script>

<style scoped>
.building-card { transition: all 0.3s; }
.building-card:hover { transform: translateY(-4px); box-shadow: 0 10px 30px rgba(0,0,0,0.05) !important; border-color: #ff6b00 !important; }
</style>
