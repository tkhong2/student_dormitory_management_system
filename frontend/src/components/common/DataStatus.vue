<template>
  <div>
    <div
      v-if="loading"
      style="display: flex; justify-content: center; padding: 60px 0"
    >
      <a-spin size="large" tip="Đang tải dữ liệu..." />
    </div>

    <a-alert
      v-else-if="hasError"
      type="error"
      show-icon
      :message="displayMessage"
      style="margin-bottom: 16px"
    >
      <template #action>
        <a-button size="small" @click="$emit('retry')">Thử lại</a-button>
      </template>
    </a-alert>

    <slot v-else />
  </div>
</template>

<script setup>
import { computed } from "vue";
import { defineProps } from "vue";

const props = defineProps({
  loading: { type: Boolean, default: false },
  error: { type: [String, null], default: null },
  items: { type: [Array, null], default: null },
  emptyMessage: { type: String, default: "Lỗi kết nối máy chủ" },
  treatEmptyAsError: { type: Boolean, default: true },
});

const hasError = computed(() => {
  if (props.loading) return false;
  if (props.error) return true;
  if (props.treatEmptyAsError)
    return Array.isArray(props.items) && props.items.length === 0;
  return false;
});

const displayMessage = computed(() => props.error || props.emptyMessage);
</script>
