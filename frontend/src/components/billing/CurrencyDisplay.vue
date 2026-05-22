<template>
  <span class="font-weight-bold">{{ formatted }}</span>
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({ amount: { type: Number, required: true } })
const formatted = computed(() => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(props.amount))
</script>
