<template>
  <div>
    <a-space direction="vertical" style="width: 100%">
      <!-- Preview image nếu có -->
      <div v-if="modelValue" style="position: relative; display: inline-block">
        <img 
          :src="getImageUrl(modelValue)" 
          :alt="alt"
          :style="{
            width: previewWidth,
            height: previewHeight,
            objectFit: 'cover',
            borderRadius: '4px',
            border: '1px solid #d9d9d9'
          }"
        />
        <a-button
          danger
          size="small"
          type="text"
          :icon="h(DeleteOutlined)"
          @click="handleRemove"
          style="position: absolute; top: 4px; right: 4px; background: rgba(255,255,255,0.9)"
        />
      </div>

      <!-- Upload button -->
      <a-upload
        :before-upload="handleBeforeUpload"
        :show-upload-list="false"
        accept="image/*"
      >
        <a-button :loading="uploading" :icon="h(UploadOutlined)">
          {{ buttonText }}
        </a-button>
      </a-upload>

      <!-- Manual URL input -->
      <a-input
        v-if="allowManualInput"
        :value="modelValue"
        @update:value="handleManualInput"
        :placeholder="placeholder"
      />
    </a-space>
  </div>
</template>

<script setup>
import { ref, h } from 'vue'
import { message } from 'ant-design-vue'
import { UploadOutlined, DeleteOutlined } from '@ant-design/icons-vue'
import { fileService } from '@/services/fileService'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  buttonText: {
    type: String,
    default: 'Chọn ảnh'
  },
  placeholder: {
    type: String,
    default: 'Hoặc nhập URL trực tiếp'
  },
  previewWidth: {
    type: String,
    default: '200px'
  },
  previewHeight: {
    type: String,
    default: '150px'
  },
  alt: {
    type: String,
    default: 'Preview'
  },
  allowManualInput: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue'])
const uploading = ref(false)

function getImageUrl(url) {
  if (!url) return ''
  // Nếu URL bắt đầu bằng / thì thêm base URL
  if (url.startsWith('/')) {
    return import.meta.env.VITE_API_BASE_URL?.replace('/api', '') + url
  }
  return url
}

async function handleBeforeUpload(file) {
  // Validate file type
  const isImage = file.type.startsWith('image/')
  if (!isImage) {
    message.error('Chỉ chấp nhận file ảnh!')
    return false
  }

  // Validate file size (5MB)
  const isLt5M = file.size / 1024 / 1024 < 5
  if (!isLt5M) {
    message.error('Kích thước ảnh phải nhỏ hơn 5MB!')
    return false
  }

  uploading.value = true
  try {
    const response = await fileService.upload(file)
    emit('update:modelValue', response.url)
    message.success('Tải ảnh lên thành công!')
  } catch (error) {
    console.error('Upload error:', error)
    message.error('Lỗi khi tải ảnh lên!')
  } finally {
    uploading.value = false
  }

  return false // Prevent default upload behavior
}

function handleManualInput(value) {
  emit('update:modelValue', value)
}

function handleRemove() {
  emit('update:modelValue', '')
}
</script>
