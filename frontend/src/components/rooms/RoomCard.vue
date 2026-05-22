<template>
  <DataCard
    style="cursor:pointer; transition: all .2s"
    class="text-center pa-4 room-card"
    @click="$emit('click')"
  >
    <div class="mx-auto mb-3 d-flex align-center justify-center rounded-lg" :style="{width:'48px',height:'48px',background:statusBg}">
      <v-icon :color="statusColor" size="24">mdi-door</v-icon>
    </div>
    <div class="text-subtitle-2 font-weight-bold">{{ number }}</div>
    <div class="text-caption text-medium-emphasis mb-2">{{ building }} · T{{ floor }}</div>
    <StatusChip :status="status" />
    <v-progress-linear :model-value="occupancy/capacity*100" :color="statusColor" height="4" rounded class="mt-3" />
    <div class="text-caption text-medium-emphasis mt-1">{{ occupancy }}/{{ capacity }}</div>
  </DataCard>
</template>

<script setup>
import { computed } from 'vue'
import DataCard from '@/components/common/DataCard.vue'
import StatusChip from '@/components/common/StatusChip.vue'

const props = defineProps({
  number: String, building: String, floor: Number,
  status: String, occupancy: Number, capacity: Number,
})
defineEmits(['click'])

const colorMap = { 'Trống':'success','Đã đầy':'error','Bảo trì':'warning','Đang ở':'info' }
const bgMap = { 'Trống':'#dcfce7','Đã đầy':'#fef2f2','Bảo trì':'#fff7ed','Đang ở':'#e0f2fe' }
const statusColor = computed(() => colorMap[props.status] || 'grey')
const statusBg = computed(() => bgMap[props.status] || '#f3f4f6')
</script>

<style scoped>
.room-card:hover { transform:translateY(-3px); box-shadow: 0 8px 25px -5px rgba(0,0,0,.08); }
</style>
