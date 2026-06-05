<template>
  <a-config-provider :theme="{ token: { colorPrimary: '#ff6b00' } }">
    <a-layout style="min-height: 100vh">
      <!-- Sidebar -->
      <a-layout-sider
        v-model:collapsed="collapsed"
        :trigger="null"
        collapsible
        width="260"
        :collapsedWidth="80"
        style="
          background: #fff;
          box-shadow: 2px 0 8px rgba(0, 0, 0, 0.05);
          position: fixed;
          left: 0;
          top: 0;
          bottom: 0;
          overflow: hidden;
          z-index: 100;
        "
      >
        <div style="height: 100%; display: flex; flex-direction: column;">
          <div class="logo-container">
            <AppLogo :collapsed="collapsed" subtitle="Nhân viên" />
          </div>

          <div style="flex: 1; overflow-y: auto; overflow-x: hidden; min-height: 0;">
            <a-menu
              v-model:selectedKeys="selectedKeys"
              mode="inline"
              :items="menuItems"
              @click="handleMenuClick"
              style="border-right: 0;"
            />
          </div>

          <div class="user-info-fixed" v-if="!collapsed">
            <a-card
              size="small"
              style="background: #1f1f1f; border: none; margin: 0"
              :bodyStyle="{ padding: '8px 10px' }"
            >
              <div style="display: flex; align-items: center; gap: 8px">
                <a-avatar src="https://i.pravatar.cc/150?u=staff" :size="32" />
                <div style="flex: 1; min-width: 0">
                  <div style="color: white; font-weight: 600; font-size: 11px; line-height: 1.3">
                    Staff
                  </div>
                  <div style="color: #8c8c8c; font-size: 10px; line-height: 1.3; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">
                    {{ userEmail }}
                  </div>
                </div>
              </div>
              <a-divider style="margin: 8px 0" />
              <a-button type="text" size="small" block @click="logout">
                Đăng xuất
              </a-button>
            </a-card>
          </div>
        </div>
      </a-layout-sider>

      <!-- Main Layout -->
      <a-layout style="margin-left: 200px; transition: margin-left 0.2s">
        <!-- Header -->
        <a-layout-header
          style="
            background: white;
            padding: 0 24px;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: sticky;
            top: 0;
            z-index: 99;
          "
        >
          <div style="display: flex; align-items: center; gap: 16px">
            <a-button
              type="text"
              size="large"
              @click="collapsed = !collapsed"
              style="width: 64px; height: 64px"
            >
              <template #icon>
                <MenuFoldOutlined v-if="!collapsed" />
                <MenuUnfoldOutlined v-else />
              </template>
            </a-button>
          </div>

          <div style="display: flex; align-items: center; gap: 16px">
            <a-button type="text" size="large">
              <template #icon><BellOutlined /></template>
            </a-button>
          </div>
        </a-layout-header>

        <!-- Content -->
        <a-layout-content style="padding: 24px; background: #f5f5f5; min-height: calc(100vh - 64px)">
          <router-view />
        </a-layout-content>
      </a-layout>
    </a-layout>
  </a-config-provider>
</template>

<script setup>
import { h, ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  BellOutlined,
  DashboardOutlined,
  FileTextOutlined,
  DollarOutlined,
  ClockCircleOutlined,
  ToolOutlined,
} from '@ant-design/icons-vue'
import AppLogo from '@/components/common/AppLogo.vue'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const collapsed = ref(false)

const userEmail = computed(() => authStore.user?.email || 'staff@email.com')

const selectedKeys = ref(['/staff'])

const menuItems = [
  {
    key: '/staff',
    icon: () => h(DashboardOutlined),
    label: 'Dashboard',
  },
  {
    key: 'billing',
    label: 'TÀI CHÍNH',
    type: 'group',
    children: [
      {
        key: '/staff/invoices',
        icon: () => h(FileTextOutlined),
        label: 'Hóa đơn',
      },
      {
        key: '/staff/payments',
        icon: () => h(DollarOutlined),
        label: 'Thanh toán',
      },
      {
        key: '/staff/debt',
        icon: () => h(ClockCircleOutlined),
        label: 'Công nợ',
      },
    ],
  },
  {
    key: 'maintenance',
    label: 'BẢO TRÌ',
    type: 'group',
    children: [
      {
        key: '/staff/maintenance-requests',
        icon: () => h(ToolOutlined),
        label: 'Yêu cầu sửa chữa',
      },
    ],
  },
]

function handleMenuClick(e) {
  const path = e.key
  if (path.startsWith('/')) {
    router.push(path)
  }
}

function logout() {
  authStore.logout()
  router.push('/login')
  message.success('Đã đăng xuất')
}

onMounted(() => {
  // Set active menu based on current route
  const currentPath = router.currentRoute.value.path
  selectedKeys.value = [currentPath]
})
</script>

<style scoped>
.logo-container {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom: 1px solid #f0f0f0;
}

.user-info-fixed {
  padding: 12px;
  border-top: 1px solid #f0f0f0;
}

:deep(.ant-menu) {
  background: transparent;
}

:deep(.ant-menu-item) {
  margin-top: 4px !important;
  margin-bottom: 4px !important;
}
</style>
