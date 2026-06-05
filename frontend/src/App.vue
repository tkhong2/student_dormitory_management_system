<template>
  <a-config-provider :locale="viVN">
    <router-view />
  </a-config-provider>
</template>

<script setup>
import viVN from 'ant-design-vue/es/locale/vi_VN'
</script>

<style>
/* Toàn bộ style chung sẽ nằm ở đây hoặc style.css */
</style>
