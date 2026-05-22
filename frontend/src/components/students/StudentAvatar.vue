<template>
  <div class="d-flex align-center py-2 ga-3">
    <v-avatar size="36" :color="gender==='Nam'?'primary':'secondary'" variant="tonal">
      <v-icon size="18">{{ gender==='Nam'?'mdi-face-man':'mdi-face-woman' }}</v-icon>
    </v-avatar>
    <div>
      <div class="text-body-2 font-weight-bold">{{ name }}</div>
      <div class="text-caption text-medium-emphasis">{{ email }}</div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  name: String,
  email: String,
  gender: { type: String, default: 'Nam' },
})
</script>
