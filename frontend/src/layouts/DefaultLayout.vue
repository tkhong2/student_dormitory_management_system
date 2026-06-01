<template>
  <v-app style="background-color: #f8fafc">
    <!-- Sidebar -->
    <v-navigation-drawer
      v-model="drawer"
      width="280"
      border="none"
      class="elevation-4"
    >
      <div class="d-flex flex-column fill-height">
        <!-- Logo Area -->
        <router-link
          to="/admin"
          class="d-flex align-center ga-2 pa-8 text-decoration-none border-b"
        >
          <div class="uni-logo-sm">
            <img
              src="/images/logo.png"
              style="width: 100%; height: 100%; object-fit: contain"
              alt="DNU Logo"
            />
          </div>
          <div>
            <div class="text-h6 font-weight-black text-primary leading-tight">
              DNU KTX
            </div>
            <div
              class="text-caption font-weight-bold opacity-60 uppercase tracking-widest"
            >
              Quản trị viên
            </div>
          </div>
        </router-link>

        <!-- Navigation Menu -->
        <div class="flex-grow-1 overflow-y-auto pa-4 scrollbar-hide">
          <div v-for="section in menu" :key="section.label" class="mb-6">
            <div
              class="px-4 text-caption font-weight-bold text-medium-emphasis uppercase tracking-widest mb-2"
            >
              {{ section.label }}
            </div>
            <v-list nav class="pa-0 ga-1">
              <v-list-item
                v-for="item in section.items"
                :key="item.to"
                :to="item.to"
                :prepend-icon="item.icon"
                :title="item.title"
                rounded="lg"
                color="primary"
                class="font-weight-bold"
              />
            </v-list>
          </div>
        </div>

        <!-- User Info Bottom -->
        <div class="user-info-fixed">
          <v-card
            flat
            color="grey-darken-4"
            class="pa-4 rounded-xl d-flex align-center ga-3"
          >
            <v-avatar size="40">
              <v-img src="https://i.pravatar.cc/150?u=admin" />
            </v-avatar>
            <div class="min-w-0 flex-grow-1">
              <div
                class="text-body-2 font-weight-black text-white text-truncate"
              >
                Administrator
              </div>
              <div class="text-caption text-white opacity-50">Hệ thống</div>
            </div>
            <v-btn
              icon="mdi-logout"
              size="x-small"
              variant="text"
              color="white"
              @click="logout"
            />
          </v-card>
        </div>
      </div>
    </v-navigation-drawer>

    <!-- Topbar -->
    <v-app-bar
      flat
      color="white"
      height="64"
      elevation="0"
      style="border-bottom: 1px solid #f1f5f9"
    >
      <div class="d-flex align-center w-100 px-4">
        <v-btn
          icon="mdi-menu"
          variant="text"
          size="small"
          @click="drawer = !drawer"
          class="mr-3"
        />

        <div class="d-none d-sm-flex align-center" style="gap: 6px">
          <span class="text-caption text-medium-emphasis">Trang chủ</span>
          <v-icon size="12" class="text-medium-emphasis"
            >mdi-chevron-right</v-icon
          >
          <span class="text-caption font-weight-bold">{{
            $route.meta?.title || $route.name
          }}</span>
        </div>

        <v-spacer />

        <!-- Search -->
        <v-text-field
          placeholder="Tìm nhanh..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          density="compact"
          rounded="lg"
          hide-details
          class="d-none d-md-block mr-3 search-field"
          style="max-width: 280px"
        />

        <v-btn icon variant="text" size="small" class="mr-2">
          <v-badge dot color="error">
            <v-icon size="20">mdi-bell-outline</v-icon>
          </v-badge>
        </v-btn>

        <v-avatar size="36" class="cursor-pointer">
          <v-img src="https://i.pravatar.cc/150?u=admin" />
        </v-avatar>
      </div>
    </v-app-bar>

    <!-- Content Area -->
    <v-main>
      <v-container fluid class="pa-6" style="max-width: 1600px">
        <router-view v-slot="{ Component }">
          <transition name="page" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";

const drawer = ref(true);
const router = useRouter();

const menu = [
  {
    label: "Tổng quan",
    items: [
      { title: "Dashboard", icon: "mdi-view-dashboard-outline", to: "/admin" },
    ],
  },
  {
    label: "Cơ sở vật chất",
    items: [
      {
        title: "Tòa nhà",
        icon: "mdi-office-building-outline",
        to: "/admin/buildings",
      },
      {
        title: "Loại phòng",
        icon: "mdi-format-list-bulleted-square",
        to: "/admin/room-types",
      },
      { title: "Phòng ở", icon: "mdi-door-open", to: "/admin/rooms" },
    ],
  },
  {
    label: "Sinh viên",
    items: [
      {
        title: "Quản lý Sinh viên",
        icon: "mdi-school-outline",
        to: "/admin/students",
      },
      { title: "Hợp đồng", icon: "mdi-file-sign", to: "/admin/contracts" },
    ],
  },
  {
    label: "Tài chính",
    items: [
      {
        title: "Hóa đơn",
        icon: "mdi-receipt-text-outline",
        to: "/admin/billing",
      },
      {
        title: "Bảo trì",
        icon: "mdi-wrench-outline",
        to: "/admin/maintenance",
      },
    ],
  },
];

const logout = () => {
  localStorage.clear();
  router.push("/login");
};
</script>

<style scoped>
.uni-logo-sm {
  width: 60px;
  height: 60px;
  background: white;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px;
  box-shadow: 0 2px 8px rgba(255, 107, 0, 0.15);
  flex-shrink: 0;
}
.text-primary {
  color: #ff6b00 !important;
}
.max-width-300 {
  max-width: 300px;
}
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.search-field :deep(.v-field__outline) {
  border-color: rgba(0, 0, 0, 0.2) !important;
}

.search-field :deep(.v-field--focused .v-field__outline) {
  border-color: #ff6b00 !important;
}

.user-info-fixed {
  position: fixed;
  bottom: 0;
  left: 0;
  width: 280px;
  padding: 16px;
  background: #fff;
  border-top: 1px solid #e0e0e0;
  z-index: 10;
}
</style>
