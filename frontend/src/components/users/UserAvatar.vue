<template>
  <div class="d-flex align-center ga-3 py-2">
    <v-avatar size="36" :color="roleColor" variant="tonal">
      <v-icon size="18">{{ roleIcon }}</v-icon>
    </v-avatar>
    <div>
      <div class="text-body-2 font-weight-bold">{{ name }}</div>
      <div class="text-caption text-medium-emphasis">{{ email }}</div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({ name: String, email: String, role: String })
const roleColor = computed(() => ({'Admin':'error','Nhân viên':'info','Sinh viên':'success'}[props.role]||'grey'))
const roleIcon = computed(() => ({'Admin':'mdi-shield-crown','Nhân viên':'mdi-account-hard-hat','Sinh viên':'mdi-school'}[props.role]||'mdi-account'))
</script>
