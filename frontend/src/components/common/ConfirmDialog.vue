<template>
  <v-dialog :model-value="modelValue" max-width="420" @update:model-value="$emit('update:modelValue', $event)">
    <v-card class="pa-6">
      <div class="text-center mb-4">
        <v-avatar :color="color" size="56" variant="tonal" class="mb-3">
          <v-icon size="28">{{ icon }}</v-icon>
        </v-avatar>
        <h3 class="text-h6 font-weight-bold">{{ title }}</h3>
        <p class="text-body-2 text-medium-emphasis mt-2">{{ message }}</p>
      </div>
      <div class="d-flex justify-center ga-3">
        <v-btn variant="text" @click="$emit('update:modelValue', false)">Hủy</v-btn>
        <v-btn :color="color" @click="$emit('confirm')">{{ confirmText }}</v-btn>
      </div>
    </v-card>
  </v-dialog>
</template>

<script setup>
defineProps({
  modelValue: { type: Boolean, default: false },
  title: { type: String, default: 'Xác nhận' },
  message: { type: String, default: 'Bạn có chắc muốn thực hiện thao tác này?' },
  confirmText: { type: String, default: 'Xác nhận' },
  color: { type: String, default: 'error' },
  icon: { type: String, default: 'mdi-alert-circle-outline' },
})

defineEmits(['update:modelValue', 'confirm'])
</script>
