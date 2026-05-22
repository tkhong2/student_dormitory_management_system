<template>
  <DataCard class="pa-4 text-center">
    <div class="text-h5 font-weight-bold" :class="'text-'+color">{{ value }}</div>
    <div class="text-caption text-medium-emphasis">{{ label }}</div>
  </DataCard>
</template>

<script setup>
import DataCard from '@/components/common/DataCard.vue'
defineProps({
  value: [String, Number],
  label: String,
  color: { type: String, default: 'primary' },
})
</script>
