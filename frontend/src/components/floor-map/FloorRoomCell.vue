<template>
  <div class="floor-room" :style="{background:color+'18',borderColor:color}" @click="$emit('click')">
    <div class="text-subtitle-2 font-weight-bold" :style="{color}">{{ number }}</div>
    <div class="text-caption text-medium-emphasis">{{ occupancy }}/{{ capacity }}</div>
  </div>
</template>

<script setup>
defineProps({ number: String, occupancy: Number, capacity: Number, color: String })
defineEmits(['click'])
</script>

<style scoped>
.floor-room {
  aspect-ratio:1; display:flex; flex-direction:column; align-items:center; justify-content:center;
  border:2px solid; border-radius:12px; cursor:pointer; transition:all .2s;
}
.floor-room:hover { transform:scale(1.06); box-shadow:0 6px 20px rgba(0,0,0,.06); }
</style>
