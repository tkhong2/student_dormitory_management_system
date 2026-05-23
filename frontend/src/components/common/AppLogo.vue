<template>
  <div :style="containerStyle">
    <div :style="iconStyle">
      <img src="/images/logo.png" :style="{width: '100%', height: '100%', objectFit: 'contain'}" alt="DNU KTX Logo" />
    </div>
    <div v-if="showText && !collapsed" :style="textStyle">
      <div :style="titleStyle">{{ title }}</div>
      <div v-if="subtitle" :style="subtitleStyle">{{ subtitle }}</div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  size: { type: String, default: 'medium' }, // small, medium, large
  showText: { type: Boolean, default: true },
  collapsed: { type: Boolean, default: false },
  title: { type: String, default: 'DNU KTX' },
  subtitle: { type: String, default: '' },
  variant: { type: String, default: 'default' } // default, white, dark
})

const sizeMap = {
  small: { icon: '40px', iconBox: '40px', title: '18px', subtitle: '10px' },
  medium: { icon: '60px', iconBox: '60px', title: '24px', subtitle: '12px' },
  large: { icon: '80px', iconBox: '80px', title: '28px', subtitle: '14px' }
}

const sizes = computed(() => sizeMap[props.size])
const iconSize = computed(() => sizes.value.icon)

const containerStyle = computed(() => ({
  display: 'flex',
  alignItems: 'center',
  height: '100%'
}))

const iconStyle = computed(() => ({
  width: sizes.value.iconBox,
  height: sizes.value.iconBox,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  flexShrink: '0'
}))

const textStyle = computed(() => ({
  flex: '1',
  minWidth: '0',
  display: 'flex',
  flexDirection: 'column',
  justifyContent: 'center',
  height: sizes.value.iconBox,

}))

const titleStyle = computed(() => ({
  fontSize: sizes.value.title,
  fontWeight: '900',
  color: props.variant === 'white' ? 'white' : '#ff6b00',
  lineHeight: '1',
  marginBottom: props.subtitle ? '4px' : '0'
}))

const subtitleStyle = computed(() => ({
  fontSize: sizes.value.subtitle,
  fontWeight: '700',
  color: props.variant === 'white' ? 'rgba(255,255,255,0.8)' : '#666',
  textTransform: 'uppercase',
  letterSpacing: '0.5px',
  lineHeight: '1'
}))
</script>
