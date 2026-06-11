<template>
  <v-app style="background-color: #f8fafc">
    <!-- Student Sidebar -->
    <v-navigation-drawer 
      :model-value="true"
      width="280" 
      border="none" 
      class="elevation-4"
      :scrim="false"
      permanent
      :rail="false"
    >
      <div class="d-flex flex-column fill-height">
        <!-- Logo Area -->
        <div style="
          background: #ffffff;
          border-bottom: 2px solid #ff6b00;
          flex-shrink: 0;
        ">
          <router-link
            to="/student"
            style="display:flex;align-items:center;gap:14px;padding:20px 24px;text-decoration:none;"
          >
            <!-- Logo box -->
            <div style="
              width:48px;height:48px;flex-shrink:0;
              background:#fff7f0;
              border:2px solid #ff6b00;
              border-radius:10px;
              display:flex;align-items:center;justify-content:center;
              padding:6px;
              box-shadow:0 2px 8px rgba(255,107,0,0.2);
            ">
              <img src="/images/logo.png" style="width:100%;height:100%;object-fit:contain;" alt="DNU Logo" />
            </div>
            <!-- Text -->
            <div>
              <div style="font-size:17px;font-weight:900;color:#1e293b;letter-spacing:0.3px;line-height:1.2;">DNU KTX</div>
              <div style="font-size:10px;font-weight:600;color:#ff6b00;text-transform:uppercase;letter-spacing:2px;margin-top:2px;">Cổng sinh viên</div>
            </div>
          </router-link>
        </div>

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
            <v-avatar size="40" style="flex-shrink: 0;">
              <v-img :src="userAvatarUrl" />
            </v-avatar>
            <div style="flex: 1; min-width: 0; overflow: hidden;">
              <div class="text-body-2 font-weight-black text-white" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                {{ currentUser.fullName }}
              </div>
              <div class="text-caption text-white opacity-50" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                {{ studentInfo }}
              </div>
            </div>
            <v-btn icon="mdi-logout" size="x-small" variant="text" color="white" @click="logout" style="flex-shrink: 0;" />
          </v-card>
        </div>
      </div>
    </v-navigation-drawer>

    <!-- Top bar -->
    <v-app-bar flat height="72" style="
      background: rgba(255,255,255,0.85);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      border-bottom: 1px solid rgba(0,0,0,0.07);
      box-shadow: 0 2px 16px rgba(0,0,0,0.06);
    ">
      <div class="d-flex align-center w-100 px-6" style="height: 100%;">

        <!-- Breadcrumb -->
        <div class="d-none d-sm-flex align-center ga-2">
          <v-icon size="16" color="indigo-darken-2" style="opacity:0.7">mdi-home-variant</v-icon>
          <span style="font-size:12px; font-weight:600; color:#6366f1; opacity:0.75; text-transform:uppercase; letter-spacing:0.8px;">Cổng SV</span>
          <v-icon size="14" style="opacity:0.3">mdi-chevron-right</v-icon>
          <span style="font-size:12px; font-weight:800; color:#1e293b; text-transform:uppercase; letter-spacing:0.8px;">{{ $route.meta?.title || 'Trang chủ' }}</span>
        </div>

        <v-spacer />

        <!-- Room chip -->
        <div class="d-none d-md-flex align-center mr-4">
          <v-chip
            variant="flat"
            size="small"
            :style="roomLabel === 'Chưa xếp phòng' 
              ? { background: '#64748b', color: 'white', fontWeight: '700', fontSize: '12px' } 
              : { background: 'linear-gradient(135deg, #10b981, #059669)', color: 'white', fontWeight: '700', fontSize: '12px', boxShadow: '0 2px 8px rgba(16,185,129,0.4)' }"
          >
            <v-icon start size="10" style="color:white">mdi-circle</v-icon>
            Phòng: {{ roomLabel }}
          </v-chip>
        </div>

        <!-- Notification bell -->
        <v-btn icon variant="text" class="mr-1" style="color:#475569;">
          <v-badge dot color="error">
            <v-icon>mdi-bell-outline</v-icon>
          </v-badge>
        </v-btn>

        <!-- Avatar -->
        <v-avatar
          size="38"
          class="cursor-pointer"
          style="border: 2px solid #6366f1; box-shadow: 0 2px 10px rgba(99,102,241,0.3);"
        >
          <v-img :src="userAvatarUrl" />
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
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { contractService } from '@/services/contractService'
import { roomApplicationService } from '@/services/roomApplicationService'

const drawer = ref(true) // Always keep drawer open
const router = useRouter()
const authStore = useAuthStore()
const roomLabel = ref('Chưa xếp phòng')

// Get current user from auth store
const currentUser = computed(() => authStore.user || {
  fullName: 'Sinh viên',
  studentCode: 'SV???',
  avatarUrl: null
})

// Process avatar URL
const userAvatarUrl = computed(() => {
  if (currentUser.value.avatarUrl) {
    if (currentUser.value.avatarUrl.startsWith('http')) {
      return currentUser.value.avatarUrl
    }
    return `http://localhost:5003${currentUser.value.avatarUrl}`
  }
  // Fallback to placeholder with user's name
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(currentUser.value.fullName || 'SV')}&background=6366f1&color=fff&size=128`
})

// Get student info text (StudentCode · Class)
const studentInfo = computed(() => {
  const code = currentUser.value.studentCode || 'SV???'
  // Try to get class from user or default
  return `${code}`
})

const menu = [
  { title: 'Trang chủ', icon: 'mdi-home-outline', to: '/student' },
  { title: 'Đăng ký phòng', icon: 'mdi-home-search', to: '/student/rooms' },
  { title: 'Phòng của tôi', icon: 'mdi-door-closed', to: '/student/my-room' },
  { title: 'Hợp đồng', icon: 'mdi-file-document-outline', to: '/student/my-contract' },
  { title: 'Thanh toán', icon: 'mdi-credit-card-outline', to: '/student/my-payments' },
  { title: 'Yêu cầu sửa chữa', icon: 'mdi-wrench-outline', to: '/student/maintenance' },
  { title: 'Hồ sơ cá nhân', icon: 'mdi-account-outline', to: '/student/profile' },
]

const logout = () => { 
  authStore.logout()
  router.push('/login') 
}

// Prevent drawer toggle
const toggleDrawer = () => {
  // Do nothing - keep drawer always open
}

const fetchRoomNumber = async () => {
  try {
    const user = currentUser.value
    const userId = user.id
    if (!userId) return

    // 1. Kiểm tra hợp đồng hoạt động (Active/Pending)
    const contracts = await contractService.getByUserId(userId)
    const activeContract = contracts.find(c => c.status === 'Active' || c.status === 'Pending')
    if (activeContract) {
      roomLabel.value = `${activeContract.roomNumber}-${activeContract.buildingName}`
      return
    }

    // 2. Nếu không có hợp đồng, kiểm tra các đơn đăng ký đã gửi
    const applications = await roomApplicationService.getByUserId(userId)
    const appWithRoom = applications.find(app => 
      app.status === 'Pending' || app.status === 'UnderReview' || app.status === 'Approved'
    )
    if (appWithRoom) {
      const roomNumber = appWithRoom.assignedRoomNumber || appWithRoom.preferredRoomNumber
      const buildingName = appWithRoom.assignedBuildingName || appWithRoom.preferredBuildingName
      if (roomNumber && buildingName) {
        roomLabel.value = `${roomNumber}-${buildingName}`
      }
    }
  } catch (error) {
    console.error('Lỗi khi lấy số phòng cho top bar:', error)
  }
}

onMounted(() => {
  fetchRoomNumber()
})
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

/* Force hide navigation drawer overlay */
:deep(.v-navigation-drawer__scrim) {
  display: none !important;
}

:deep(.v-overlay) {
  display: none !important;
}
</style>
