<template>
  <v-card class="stat-card" elevation="1">
    <v-card-text class="pa-5">
      <div class="d-flex align-center justify-space-between mb-4">
        <v-avatar :color="color" size="48" variant="tonal" class="rounded-lg">
          <v-icon :color="color" size="24">{{ icon }}</v-icon>
        </v-avatar>
        <v-chip 
          v-if="trend !== undefined" 
          :color="trend > 0 ? 'success' : 'error'" 
          size="small" 
          variant="tonal" 
          class="font-weight-bold"
        >
          <v-icon start size="14">{{ trend > 0 ? 'mdi-trending-up' : 'mdi-trending-down' }}</v-icon>
          {{ Math.abs(trend) }}%
        </v-chip>
      </div>
      <div class="text-h4 font-weight-bold mb-1">{{ value }}</div>
      <div class="text-body-2 text-medium-emphasis">{{ label }}</div>
    </v-card-text>
  </v-card>
</template>

<script setup>
defineProps({
  label: { type: String, required: true },
  value: { type: [String, Number], required: true },
  icon: { type: String, required: true },
  color: { type: String, default: 'primary' },
  trend: { type: Number, default: undefined },
})
</script>

<style scoped>
.stat-card {
  border-radius: 16px;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1) !important;
}
</style>
