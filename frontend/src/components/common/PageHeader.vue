<template>
  <div class="page-header">
    <h1 class="page-title">{{ title }}</h1>
    <p class="page-subtitle">{{ subtitle }}</p>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  subtitle: {
    type: String,
    required: true
  }
})
</script>

<style scoped>
.page-header {
  margin-bottom: 24px;
}

.page-title {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.page-subtitle {
  font-size: 14px;
  font-weight: 400;
  color: #8c8c8c;
  margin: 0;
  line-height: 1.5;
}
</style>
