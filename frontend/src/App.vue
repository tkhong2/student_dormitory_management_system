<template>
  <a-config-provider :locale="viVN">
    <router-view />
  </a-config-provider>
</template>

<script setup>
import viVN from 'ant-design-vue/es/locale/vi_VN'
</script>

<style>
/* Global style for Vuetify warning buttons - white text */
.v-btn.bg-warning {
  color: white !important;
}

.v-btn.bg-warning .v-icon {
  color: white !important;
}

/* Toàn bộ style chung sẽ nằm ở đây hoặc style.css */
</style>
