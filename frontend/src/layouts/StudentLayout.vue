<template>
  <v-app style="background-color: #f8fafc">
    <!-- Student Sidebar -->
    <v-navigation-drawer v-model="drawer" width="280" border="none" class="elevation-4">
      <div class="d-flex flex-column fill-height">
        <!-- Logo Area -->
        <router-link to="/student" class="d-flex align-center ga-2 pa-8 text-decoration-none border-b">
          <div class="uni-logo-sm">
            <img src="/images/logo.png" style="width: 100%; height: 100%; object-fit: contain;" alt="DNU Logo" />
          </div>
          <div>
            <div class="text-h6 font-weight-black text-primary leading-tight">DNU KTX</div>
            <div class="text-caption font-weight-bold opacity-60 uppercase tracking-widest">Cổng sinh viên</div>
          </div>
        </router-link>

        <!-- Navigation Menu -->
        <div class="flex-grow-1 overflow-y-auto pa-4 scrollbar-hide">
          <div class="px-4 text-caption font-weight-bold text-medium-emphasis uppercase tracking-widest mb-4">Menu Chính</div>
          <v-list nav class="pa-0 ga-1">
            <v-list-item
              v-for="item in menu"
              :key="item.to"
              :to="item.to"
              :prepend-icon="item.icon"
              :title="item.title"
              rounded="lg"
              active-color="secondary"
              class="font-weight-bold"
            />
          </v-list>
        </div>

        <!-- Student Info Bottom -->
        <div class="pa-6">
          <v-card flat color="indigo-darken-4" class="pa-4 rounded-xl d-flex align-center ga-3">
            <v-avatar size="40">
              <v-img src="https://i.pravatar.cc/150?u=student1" />
            </v-avatar>
            <div class="min-w-0 flex-grow-1">
              <div class="text-body-2 font-weight-black text-white text-truncate">Nguyễn Văn An</div>
              <div class="text-caption text-white opacity-50">SV001 · K65-CNTT</div>
            </div>
            <v-btn icon="mdi-logout" size="x-small" variant="text" color="white" @click="logout" />
          </v-card>
        </div>
      </div>
    </v-navigation-drawer>

    <!-- Top bar -->
    <v-app-bar flat color="transparent" height="80">
      <div class="d-flex align-center w-100 px-6">
        <v-btn icon="mdi-menu-variant" variant="text" @click="drawer = !drawer" class="mr-4" />
        
        <div class="d-none d-sm-flex align-center ga-2">
          <span class="text-caption font-weight-bold opacity-60 uppercase">Cổng SV</span>
          <v-icon size="14" class="opacity-30">mdi-chevron-right</v-icon>
          <span class="text-caption font-weight-black uppercase">{{ $route.meta?.title || 'Trang chủ' }}</span>
        </div>

        <v-spacer />

        <div class="d-none d-md-flex align-center mr-6">
          <v-chip color="success" variant="flat" size="small" class="font-weight-bold">
            <v-icon start size="14">mdi-circle</v-icon>
            Phòng: 101-A1
          </v-chip>
        </div>

        <v-btn icon variant="text" class="mr-2">
          <v-badge dot color="error">
            <v-icon>mdi-bell-outline</v-icon>
          </v-badge>
        </v-btn>

        <v-avatar size="40" class="cursor-pointer border">
          <v-img src="https://i.pravatar.cc/150?u=student1" />
        </v-avatar>
      </div>
    </v-app-bar>

    <!-- Main Content -->
    <v-main>
      <v-container fluid class="pa-8" style="max-width: 1400px">
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
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const drawer = ref(true)
const router = useRouter()

const menu = [
  { title: 'Trang chủ', icon: 'mdi-home-outline', to: '/student' },
  { title: 'Phòng của tôi', icon: 'mdi-door-closed', to: '/student/my-room' },
  { title: 'Hợp đồng', icon: 'mdi-file-document-outline', to: '/student/my-contract' },
  { title: 'Thanh toán', icon: 'mdi-credit-card-outline', to: '/student/my-payments' },
  { title: 'Yêu cầu sửa chữa', icon: 'mdi-wrench-outline', to: '/student/maintenance' },
  { title: 'Hồ sơ cá nhân', icon: 'mdi-account-outline', to: '/student/profile' },
]

const logout = () => { localStorage.clear(); router.push('/login') }
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
.text-primary { color: #ff6b00 !important; }
.scrollbar-hide::-webkit-scrollbar { display: none; }
</style>
