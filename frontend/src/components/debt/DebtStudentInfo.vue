<template>
  <div class="d-flex align-center ga-3 py-2">
    <v-avatar size="32" color="error" variant="tonal"><v-icon size="16">mdi-account</v-icon></v-avatar>
    <div>
      <div class="text-body-2 font-weight-medium">{{ name }}</div>
      <div class="text-caption text-medium-emphasis">{{ code }}</div>
    </div>
  </div>
</template>

<script setup>
defineProps({ name: String, code: String })
</script>
