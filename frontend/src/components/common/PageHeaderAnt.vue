<template>
  <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 16px; flex-wrap: wrap; gap: 12px;">
    <div>
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">{{ title }}</h1>
      <p v-if="subtitle" style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">{{ subtitle }}</p>
    </div>
    <div style="display: flex; gap: 8px;">
      <slot name="actions" />
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
})
</script>
