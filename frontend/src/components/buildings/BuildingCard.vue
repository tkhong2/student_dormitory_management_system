<template>
  <v-card flat border class="building-card rounded-xl overflow-hidden h-100">
    <!-- Color strip by type -->
    <div :style="{ background: getTypeGradient(building.type), height: '8px' }" />

    <!-- Image -->
    <div v-if="building.imageUrl" style="height: 140px; position: relative;">
      <img :src="building.imageUrl" style="width: 100%; height: 100%; object-fit: cover;" />
      <div style="position: absolute; top: 10px; right: 10px;">
        <v-chip
          :color="getTypeChipColor(building.type)"
          variant="flat"
          size="small"
          class="font-weight-bold"
        >
          {{ getTypeLabel(building.type) }}
        </v-chip>
      </div>
    </div>
    
    <!-- Header image placeholder with icon (Fallback) -->
    <div
      v-else
      :style="{
        background: getTypeBg(building.type),
        height: '120px',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        position: 'relative',
      }"
    >
      <v-icon size="56" :color="getTypeIconColor(building.type)">mdi-office-building</v-icon>
      <v-chip
        :color="getTypeChipColor(building.type)"
        variant="flat"
        size="small"
        class="font-weight-bold"
        style="position: absolute; top: 10px; right: 10px;"
      >
        {{ getTypeLabel(building.type) }}
      </v-chip>
    </div>

    <v-card-text class="pa-5">
      <div class="d-flex align-center justify-space-between mb-3">
        <div>
          <h3 class="text-subtitle-1 font-weight-black mb-0">{{ building.name }}</h3>
          <div class="text-caption font-weight-medium" style="color: #9ca3af;">
            <v-icon size="12" class="mr-1">mdi-map-marker</v-icon>Khuôn viên KTX
          </div>
        </div>
      </div>

      <p class="text-body-2 text-medium-emphasis mb-4" style="min-height: 32px; font-size: 12px;">
        {{ building.description || 'Chưa có mô tả' }}
      </p>

      <v-row dense class="mb-4">
        <v-col cols="12">
          <div class="pa-3 rounded-lg text-center" style="background: #f9fafb;">
            <div class="text-caption font-weight-bold mb-1" style="color: #9ca3af; text-transform: uppercase; font-size: 10px;">Số tầng</div>
            <div class="text-h6 font-weight-black">{{ building.numberOfFloors }}</div>
          </div>
        </v-col>
      </v-row>

      <v-btn
        color="primary"
        variant="tonal"
        class="rounded-lg font-weight-bold w-100"
        @click="$emit('view', building)"
      >
        Chi tiết
      </v-btn>
    </v-card-text>
  </v-card>
</template>

<script setup>
defineProps({
  building: { type: Object, required: true }
})
defineEmits(['view'])

function getTypeLabel(type) {
  const map = { Male: 'Khu Nam', Female: 'Khu Nữ', Mixed: 'Hỗn hợp' }
  return map[type] || type
}
function getTypeGradient(type) {
  const map = {
    Male:   'linear-gradient(90deg, #3b82f6, #60a5fa)',
    Female: 'linear-gradient(90deg, #ec4899, #f472b6)',
    Mixed:  'linear-gradient(90deg, #8b5cf6, #a78bfa)',
  }
  return map[type] || '#e5e7eb'
}
function getTypeBg(type) {
  const map = { Male: '#eff6ff', Female: '#fdf2f8', Mixed: '#f5f3ff' }
  return map[type] || '#f9fafb'
}
function getTypeIconColor(type) {
  const map = { Male: '#3b82f6', Female: '#ec4899', Mixed: '#8b5cf6' }
  return map[type] || '#9ca3af'
}
function getTypeChipColor(type) {
  const map = { Male: 'blue', Female: 'pink', Mixed: 'purple' }
  return map[type] || 'grey'
}
</script>

<style scoped>
.building-card { transition: all 0.25s ease; }
.building-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 32px rgba(0,0,0,0.08) !important;
}
</style>
