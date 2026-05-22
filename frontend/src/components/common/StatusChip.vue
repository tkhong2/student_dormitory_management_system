<template>
  <v-chip :color="chipColor" :size="size" :variant="variant">{{ label }}</v-chip>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: { type: String, required: true },
  size: { type: String, default: 'x-small' },
  variant: { type: String, default: 'tonal' },
  colorMap: {
    type: Object,
    default: () => ({
      'Đang ở': 'success',
      'Đã đầy': 'error',
      'Trống': 'success',
      'Bảo trì': 'warning',
      'Đang ở': 'info',
      'Chờ duyệt': 'warning',
      'Hiệu lực': 'success',
      'Sắp hết hạn': 'warning',
      'Đã chấm dứt': 'error',
      'Đã TT': 'success',
      'Chưa TT': 'warning',
      'Quá hạn': 'error',
      'Chờ xử lý': 'warning',
      'Đang xử lý': 'info',
      'Hoàn thành': 'success',
      'Hoạt động': 'success',
      'Vô hiệu': 'grey',
      'Đã duyệt': 'success',
      'Từ chối': 'error',
    }),
  },
})

const chipColor = computed(() => props.colorMap[props.status] || 'grey')
const label = computed(() => props.status)
</script>
