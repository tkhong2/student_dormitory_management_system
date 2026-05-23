<template>
  <a-card :bordered="true" :hoverable="true" style="border-radius: 12px;">
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 16px;">
      <a-avatar :size="48" :style="{ backgroundColor: getColor(color) }">
        <template #icon>
          <component :is="getIcon(icon)" />
        </template>
      </a-avatar>
      <a-tag v-if="trend !== undefined" :color="trend > 0 ? 'success' : 'error'">
        <template #icon>
          <component :is="trend > 0 ? ArrowUpOutlined : ArrowDownOutlined" />
        </template>
        {{ Math.abs(trend) }}%
      </a-tag>
    </div>
    <div style="font-size: 24px; font-weight: 700; margin-bottom: 4px;">{{ value }}</div>
    <div style="font-size: 13px; color: #8c8c8c;">{{ label }}</div>
  </a-card>
</template>

<script setup>
import { h } from 'vue'
import {
  TeamOutlined,
  HomeOutlined,
  DollarOutlined,
  ToolOutlined,
  ArrowUpOutlined,
  ArrowDownOutlined,
} from '@ant-design/icons-vue'

defineProps({
  label: { type: String, required: true },
  value: { type: [String, Number], required: true },
  icon: { type: String, required: true },
  color: { type: String, default: 'primary' },
  trend: { type: Number, default: undefined },
})

const getIcon = (iconName) => {
  const iconMap = {
    'mdi-school-outline': TeamOutlined,
    'mdi-door-open': HomeOutlined,
    'mdi-cash-clock': DollarOutlined,
    'mdi-wrench-clock': ToolOutlined,
  }
  return iconMap[iconName] || TeamOutlined
}

const getColor = (colorName) => {
  const colorMap = {
    'primary': '#1890ff',
    'info': '#13c2c2',
    'warning': '#faad14',
    'error': '#f5222d',
  }
  return colorMap[colorName] || '#1890ff'
}
</script>
