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
            <AppLogo :collapsed="collapsed" subtitle="Quản trị viên" />
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
                <a-avatar src="https://i.pravatar.cc/150?u=admin" :size="32" />
                <div style="flex: 1; min-width: 0">
                  <div style="color: white; font-weight: 600; font-size: 11px; line-height: 1.3">
                    Administrator
                  </div>
                  <div style="color: rgba(255, 255, 255, 0.5); font-size: 10px; line-height: 1.2">
                    Hệ thống
                  </div>
                </div>
                <a-button
                  type="text"
                  size="small"
                  @click="logout"
                  style="color: white; padding: 2px; min-width: 28px; height: 28px"
                >
                  <template #icon><LogoutOutlined style="font-size: 12px" /></template>
                </a-button>
              </div>
            </a-card>
          </div>
        </div>
      </a-layout-sider>

      <a-layout :style="{ marginLeft: collapsed ? '80px' : '260px' }">
        <!-- Header -->
        <a-layout-header
          style="
            background: #fff;
            padding: 0 20px;
            display: flex;
            align-items: center;
            box-shadow: 0 1px 4px rgba(0, 0, 0, 0.08);
            height: 56px;
            position: fixed;
            top: 0;
            right: 0;
            z-index: 99;
          "
          :style="{ 
            left: collapsed ? '80px' : '260px',
            width: collapsed ? 'calc(100% - 80px)' : 'calc(100% - 260px)'
          }"
        >
          <a-button
            type="text"
            @click="collapsed = !collapsed"
            style="font-size: 16px"
          >
            <template #icon>
              <MenuUnfoldOutlined v-if="collapsed" />
              <MenuFoldOutlined v-else />
            </template>
          </a-button>

          <a-breadcrumb style="margin-left: 16px">
            <a-breadcrumb-item>Trang chủ</a-breadcrumb-item>
            <a-breadcrumb-item>{{
              $route.meta?.title || "Dashboard"
            }}</a-breadcrumb-item>
          </a-breadcrumb>

          <div style="flex: 1"></div>

          <a-input-search
            placeholder="Tìm nhanh..."
            style="width: 260px; margin-right: 16px"
            size="middle"
          />

          <a-badge :count="5" :offset="[-6, 4]" style="margin-right: 16px">
            <a-button type="text" shape="circle">
              <template #icon><BellOutlined /></template>
            </a-button>
          </a-badge>

          <a-dropdown placement="bottomRight">
            <a-avatar
              src="https://i.pravatar.cc/150?u=admin"
              :size="36"
              style="cursor: pointer"
            />
            <template #overlay>
              <a-menu>
                <a-menu-item key="profile">
                  <UserOutlined style="margin-right: 8px" /> Hồ sơ cá nhân
                </a-menu-item>
                <a-menu-item key="settings">
                  <SettingOutlined style="margin-right: 8px" /> Cài đặt
                </a-menu-item>
                <a-menu-divider />
                <a-menu-item
                  key="logout"
                  @click="logout"
                  style="color: #ff4d4f"
                >
                  <LogoutOutlined style="margin-right: 8px" /> Đăng xuất
                </a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>
        </a-layout-header>

        <!-- Content -->
        <a-layout-content
          style="
            padding: 20px 25px; 
            background: #f5f5f5; 
            min-height: calc(100vh - 56px);
            margin-top: 56px;
          "
        >
          <router-view v-slot="{ Component }">
            <transition name="fade" mode="out-in">
              <component :is="Component" />
            </transition>
          </router-view>
        </a-layout-content>
      </a-layout>
    </a-layout>
  </a-config-provider>
</template>

<script setup>
import { ref, h, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  DashboardOutlined,
  HomeOutlined,
  TeamOutlined,
  FileTextOutlined,
  DollarOutlined,
  ToolOutlined,
  BellOutlined,
  LogoutOutlined,
  UserOutlined,
  SettingOutlined,
  SafetyOutlined,
  NotificationOutlined,
  AppstoreOutlined,
} from "@ant-design/icons-vue";
import AppLogo from "@/components/common/AppLogo.vue";

const router = useRouter();
const route = useRoute();
const collapsed = ref(false);
const selectedKeys = ref([route.path]);

watch(
  () => route.path,
  (newPath) => {
    selectedKeys.value = [newPath];
  },
);

const menuItems = [
  {
    key: "overview",
    label: "TỔNG QUAN",
    type: "group",
    children: [
      {
        key: "/admin",
        icon: () => h(DashboardOutlined),
        label: "Dashboard",
      },
    ],
  },
  {
    key: "facility",
    label: "CƠ SỞ VẬT CHẤT",
    type: "group",
    children: [
      {
        key: "/admin/buildings",
        icon: () => h(HomeOutlined),
        label: "Tòa nhà",
      },
      {
        key: "/admin/room-types",
        icon: () => h(FileTextOutlined),
        label: "Loại phòng",
      },
      {
        key: "/admin/rooms",
        icon: () => h(HomeOutlined),
        label: "Phòng ở",
      },
      {
        key: "/admin/amenities",
        icon: () => h(AppstoreOutlined),
        label: "Tiện nghi",
      },
      {
        key: "/admin/room-inspections",
        icon: () => h(SafetyOutlined),
        label: "Kiểm tra phòng",
      },
    ],
  },
  {
    key: "student",
    label: "SINH VIÊN",
    type: "group",
    children: [
      {
        key: "/admin/students",
        icon: () => h(TeamOutlined),
        label: "Quản lý Sinh viên",
      },
      {
        key: "/admin/student-documents",
        icon: () => h(FileTextOutlined),
        label: "Tài liệu sinh viên",
      },
      {
        key: "/admin/room-applications",
        icon: () => h(FileTextOutlined),
        label: "Đơn đăng ký phòng",
      },
      {
        key: "/admin/contracts",
        icon: () => h(FileTextOutlined),
        label: "Hợp đồng thuê",
      },
      {
        key: "/admin/contract-extensions",
        icon: () => h(FileTextOutlined),
        label: "Gia hạn hợp đồng",
      },
      {
        key: "/admin/room-transfers",
        icon: () => h(FileTextOutlined),
        label: "Chuyển phòng",
      },
    ],
  },
  {
    key: "finance",
    label: "TÀI CHÍNH",
    type: "group",
    children: [
      {
        key: "/admin/invoices",
        icon: () => h(FileTextOutlined),
        label: "Phiếu thu",
      },
      {
        key: "/admin/payments",
        icon: () => h(DollarOutlined),
        label: "Thanh toán",
      },
      {
        key: "/admin/billing",
        icon: () => h(DollarOutlined),
        label: "Hóa đơn (cũ)",
      },
      {
        key: "/admin/debt",
        icon: () => h(DollarOutlined),
        label: "Công nợ",
      },
    ],
  },
  {
    key: "maintenance",
    label: "BẢO TRÌ & SỬA CHỮA",
    type: "group",
    children: [
      {
        key: "/admin/maintenance-requests",
        icon: () => h(ToolOutlined),
        label: "Yêu cầu sửa chữa",
      },
      {
        key: "/admin/maintenance",
        icon: () => h(ToolOutlined),
        label: "Bảo trì (cũ)",
      },
    ],
  },
  {
    key: "system",
    label: "HỆ THỐNG",
    type: "group",
    children: [
      {
        key: "/admin/announcements",
        icon: () => h(NotificationOutlined),
        label: "Thông báo",
      },
      {
        key: "/admin/users",
        icon: () => h(TeamOutlined),
        label: "Người dùng",
      },
    ],
  },
];

const handleMenuClick = ({ key }) => {
  selectedKeys.value = [key];
  router.push(key);
};

const logout = () => {
  localStorage.clear();
  router.push("/login");
};
</script>

<style scoped>
.logo-container {
  height: 56px;
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 0 16px;
  border-bottom: 1px solid #f0f0f0;
}

.user-info-fixed {
  width: 260px;
  padding: 8px 12px;
  background: #fff;
  border-top: 1px solid #f0f0f0;
}

:deep(.ant-menu) {
  scrollbar-width: thin;
  scrollbar-color: rgba(0, 0, 0, 0.2) transparent;
}

:deep(.ant-menu::-webkit-scrollbar) {
  width: 6px;
}

:deep(.ant-menu::-webkit-scrollbar-track) {
  background: transparent;
}

:deep(.ant-menu::-webkit-scrollbar-thumb) {
  background-color: rgba(0, 0, 0, 0.2);
  border-radius: 3px;
}

:deep(.ant-menu::-webkit-scrollbar-thumb:hover) {
  background-color: rgba(0, 0, 0, 0.3);
}

:deep(.ant-menu-item-group-title) {
  font-size: 11px;
  font-weight: 700;
  color: #999;
  padding-left: 24px;
}

:deep(.ant-menu-item) {
  margin: 4px 8px;
  border-radius: 8px;
}

:deep(.ant-menu-item-selected) {
  background-color: #fff2e8 !important;
  color: #ff6b00 !important;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
