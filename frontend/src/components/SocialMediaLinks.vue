<template>
  <div class="social-media-links">
    <a-space :size="12">
      <!-- Facebook -->
      <a-tooltip v-if="facebookUrl" title="Facebook">
        <a :href="normalizeUrl(facebookUrl)" target="_blank" rel="noopener noreferrer" class="social-link facebook">
          <FacebookOutlined />
        </a>
      </a-tooltip>

      <!-- Zalo -->
      <a-tooltip v-if="zaloPhone" :title="`Zalo: ${zaloPhone}`">
        <a :href="`https://zalo.me/${zaloPhone}`" target="_blank" rel="noopener noreferrer" class="social-link zalo">
          <PhoneOutlined />
        </a>
      </a-tooltip>

      <!-- Instagram -->
      <a-tooltip v-if="instagramUrl" title="Instagram">
        <a :href="normalizeUrl(instagramUrl)" target="_blank" rel="noopener noreferrer" class="social-link instagram">
          <InstagramOutlined />
        </a>
      </a-tooltip>

      <!-- LinkedIn -->
      <a-tooltip v-if="linkedInUrl" title="LinkedIn">
        <a :href="normalizeUrl(linkedInUrl)" target="_blank" rel="noopener noreferrer" class="social-link linkedin">
          <LinkedinOutlined />
        </a>
      </a-tooltip>

      <span v-if="!hasAnySocialMedia" class="no-social-media">
        Chưa có liên kết mạng xã hội
      </span>
    </a-space>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { FacebookOutlined, PhoneOutlined, InstagramOutlined, LinkedinOutlined } from '@ant-design/icons-vue'

const props = defineProps({
  facebookUrl: String,
  zaloPhone: String,
  instagramUrl: String,
  linkedInUrl: String
})

const hasAnySocialMedia = computed(() => {
  return props.facebookUrl || props.zaloPhone || props.instagramUrl || props.linkedInUrl
})

const normalizeUrl = (url) => {
  if (!url) return '#'
  if (url.startsWith('http://') || url.startsWith('https://')) {
    return url
  }
  return `https://${url}`
}
</script>

<style scoped>
.social-media-links {
  display: flex;
  align-items: center;
}

.social-link {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  font-size: 16px;
  color: white;
  transition: all 0.3s ease;
  text-decoration: none;
}

.social-link:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.social-link.facebook {
  background: #1877f2;
}

.social-link.facebook:hover {
  background: #145dbf;
}

.social-link.zalo {
  background: #0068ff;
}

.social-link.zalo:hover {
  background: #0052cc;
}

.social-link.instagram {
  background: linear-gradient(45deg, #f09433 0%, #e6683c 25%, #dc2743 50%, #cc2366 75%, #bc1888 100%);
}

.social-link.instagram:hover {
  opacity: 0.9;
}

.social-link.linkedin {
  background: #0077b5;
}

.social-link.linkedin:hover {
  background: #005885;
}

.no-social-media {
  color: #8c8c8c;
  font-size: 13px;
  font-style: italic;
}
</style>
