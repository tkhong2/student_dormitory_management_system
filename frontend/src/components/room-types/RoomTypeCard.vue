<template>
  <DataCard class="pa-5">
    <div class="d-flex align-center mb-4">
      <v-avatar :color="color" size="44" rounded="lg" variant="tonal">
        <v-icon>{{ icon }}</v-icon>
      </v-avatar>
      <div class="ml-3">
        <div class="text-subtitle-1 font-weight-bold">{{ name }}</div>
        <div class="text-caption text-medium-emphasis">{{ capacity }} sinh viên/phòng</div>
      </div>
      <v-spacer />
      <slot name="menu" />
    </div>
    <p class="text-body-2 text-medium-emphasis mb-4">{{ description }}</p>
    <v-divider class="mb-4" />
    <div class="d-flex justify-space-between align-center">
      <div>
        <span class="text-h5 font-weight-bold text-primary">{{ price }}</span>
        <span class="text-body-2 text-medium-emphasis">/tháng</span>
      </div>
      <v-chip size="small" variant="tonal" color="info">{{ count }} phòng</v-chip>
    </div>
  </DataCard>
</template>

<script setup>
import DataCard from '@/components/common/DataCard.vue'
defineProps({
  name: String, capacity: Number, price: String,
  description: String, count: Number, icon: String,
  color: { type: String, default: 'primary' },
})
</script>
