<template>
  <DataCard class="pa-4 d-flex align-center ga-3">
    <v-avatar :color="bgColor" size="40" rounded="lg">
      <v-icon :color="color" size="20">{{ icon }}</v-icon>
    </v-avatar>
    <div>
      <div class="text-h6 font-weight-bold">{{ value }}</div>
      <div class="text-caption text-medium-emphasis">{{ label }}</div>
    </div>
  </DataCard>
</template>

<script setup>
import DataCard from '@/components/common/DataCard.vue'
defineProps({
  label: String, value: [String, Number],
  icon: String, color: String, bgColor: String,
})
</script>
