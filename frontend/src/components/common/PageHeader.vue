<template>
  <div class="d-flex align-center justify-space-between mb-2 flex-wrap" style="gap: 12px;">
    <div>
      <h1 class="text-h5 font-weight-bold mb-0">{{ title }}</h1>
      <p v-if="subtitle" class="text-caption text-medium-emphasis mt-1 mb-0">{{ subtitle }}</p>
    </div>
    <div class="d-flex" style="gap: 8px;">
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
