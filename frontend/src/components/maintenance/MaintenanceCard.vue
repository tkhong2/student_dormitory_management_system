<template>
  <DataCard class="pa-5">
    <div class="d-flex align-start justify-space-between mb-3">
      <div class="d-flex ga-3 align-start">
        <v-avatar :color="priority==='Khẩn cấp'?'error':'info'" size="40" rounded="lg" variant="tonal">
          <v-icon size="20">{{ priority==='Khẩn cấp'?'mdi-alert':'mdi-wrench' }}</v-icon>
        </v-avatar>
        <div>
          <div class="text-subtitle-1 font-weight-bold">{{ title }}</div>
          <div class="text-caption text-medium-emphasis">Phòng {{ room }} · {{ date }}</div>
        </div>
      </div>
      <StatusChip :status="status" />
    </div>
    <p class="text-body-2 text-medium-emphasis mb-4">{{ description }}</p>
    <v-divider class="mb-3" />
    <div class="d-flex align-center justify-space-between">
      <div class="d-flex align-center ga-2 text-caption text-medium-emphasis">
        <v-icon size="14">mdi-account</v-icon>{{ student }}
        <v-chip v-if="priority==='Khẩn cấp'" color="error" size="x-small" variant="flat" class="ml-2">Khẩn cấp</v-chip>
      </div>
      <slot name="actions" />
    </div>
  </DataCard>
</template>

<script setup>
import DataCard from '@/components/common/DataCard.vue'
import StatusChip from '@/components/common/StatusChip.vue'

defineProps({
  title: String, room: String, student: String,
  date: String, status: String, priority: String, description: String,
})
</script>
