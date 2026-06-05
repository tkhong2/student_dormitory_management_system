<template>
  <div>
    <!-- Loading State -->
    <div
      v-if="loading"
      style="display: flex; justify-content: center; padding: 60px 0"
    >
      <a-spin size="large" tip="Đang tải dữ liệu..." />
    </div>

    <!-- Error State -->
    <a-alert
      v-else-if="error"
      type="error"
      show-icon
      :message="error"
      style="margin-bottom: 16px"
    >
      <template #action>
        <a-button size="small" @click="$emit('retry')">Thử lại</a-button>
      </template>
    </a-alert>

    <!-- Empty State -->
    <a-empty
      v-else-if="isEmpty"
      :description="emptyMessage"
      style="padding: 60px 0"
    >
      <a-button v-if="showCreateButton" type="primary" @click="$emit('create')">
        {{ createButtonText }}
      </a-button>
    </a-empty>

    <!-- Content -->
    <slot v-else />
  </div>
</template>

<script setup>
import { computed } from "vue";
import { defineProps, defineEmits } from "vue";

const props = defineProps({
  loading: { type: Boolean, default: false },
  error: { type: [String, null], default: null },
  items: { type: [Array, null], default: null },
  emptyMessage: { type: String, default: "Chưa có dữ liệu" },
  showCreateButton: { type: Boolean, default: false },
  createButtonText: { type: String, default: "Tạo mới" },
});

defineEmits(['retry', 'create']);

const isEmpty = computed(() => {
  return Array.isArray(props.items) && props.items.length === 0;
});
</script>
