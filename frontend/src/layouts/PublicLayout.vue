<template>
  <v-app>
    <!-- Top utility bar -->
    <div class="util-bar d-none d-md-block">
      <v-container class="d-flex align-center justify-space-between py-0" style="max-width:1280px; height:40px">
        <div class="d-flex align-center ga-6 text-caption font-weight-medium">
          <span class="d-none d-lg-inline"><v-icon size="14" class="mr-1">mdi-phone</v-icon> 0243.869.1484</span>
          <span class="d-none d-lg-inline"><v-icon size="14" class="mr-1">mdi-email-outline</v-icon> ktx@dainam.edu.vn</span>
          <span class="opacity-70 d-none d-xl-inline"><v-icon size="14" class="mr-1">mdi-clock-outline</v-icon> T2-T6: 7h30 – 16h30</span>
        </div>
        <div class="d-flex align-center ga-4">
          <v-btn variant="text" size="x-small" class="text-white opacity-80" to="/login">
            <v-icon size="14" start>mdi-login</v-icon> <span class="d-none d-lg-inline">Đăng nhập</span>
          </v-btn>
          <v-btn size="x-small" color="warning" variant="flat" class="font-weight-bold" to="/rooms">
            Đăng ký phòng
          </v-btn>
        </div>
      </v-container>
    </div>

    <!-- Main nav -->
    <v-app-bar flat color="white" :height="$vuetify.display.mobile ? 64 : 80" class="main-nav">
      <v-container class="d-flex align-center py-0 px-4 px-md-3" style="max-width:1280px">
        <!-- Logo -->
        <router-link to="/" class="d-flex align-center text-decoration-none" :class="$vuetify.display.mobile ? 'ga-2' : 'ga-3'">
          <AppLogo :subtitle="$vuetify.display.mobile ? '' : 'Ban quản lý ký túc xá'" />
        </router-link>

        <v-spacer />

        <!-- Desktop Menu -->
        <div class="d-none d-lg-flex align-center ga-0">
          <router-link 
            v-for="item in navItems" 
            :key="item.to" 
            :to="item.to" 
            class="nav-link"
            :class="{'nav-link--active': $route.path === item.to}"
          >
            {{ item.title }}
          </router-link>
          <v-btn 
            color="primary" 
            variant="flat" 
            class="ml-4 font-weight-bold text-none" 
            size="default"
            rounded="lg"
            to="/login"
          >
            Đăng nhập
          </v-btn>
        </div>

        <!-- Mobile Menu Toggle -->
        <v-btn icon="mdi-menu" class="d-lg-none" variant="text" color="primary" @click="mob = !mob" />
      </v-container>
    </v-app-bar>

    <!-- Mobile Drawer -->
    <v-navigation-drawer v-model="mob" location="right" temporary :width="$vuetify.display.mobile ? 280 : 320">
      <v-list nav class="pa-4">
        <v-list-item 
          v-for="item in navItems" 
          :key="item.to" 
          :to="item.to" 
          :title="item.title"
          :prepend-icon="item.icon"
          rounded="lg"
          @click="mob=false"
        />
        <v-divider class="my-4" />
        <v-btn block color="primary" class="mb-2" size="large" to="/rooms">Đăng ký phòng</v-btn>
        <v-btn block variant="outlined" color="primary" size="large" to="/login">Đăng nhập</v-btn>
      </v-list>
    </v-navigation-drawer>

    <!-- Main Content -->
    <v-main style="background-color: #fff;">
      <router-view v-slot="{ Component }">
        <transition name="page" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>

      <!-- Footer -->
      <footer class="ft">
        <v-container class="px-4 px-md-3" style="max-width:1280px">
          <v-row class="mb-6 mb-md-10">
            <v-col cols="12" md="4" class="mb-6 mb-md-0">
              <div class="mb-4 mb-md-6">
                <AppLogo size="small" variant="white" subtitle="Ban quản lý ký túc xá" />
              </div>
              <p class="text-body-2 opacity-70" style="line-height:1.7">
                Ký túc xá Đại học Đại Nam — môi trường sinh hoạt lý tưởng, an toàn và hiện đại cho sinh viên yên tâm học tập và phát triển.
              </p>
            </v-col>

            <v-col cols="6" sm="4" md="2">
              <div class="text-subtitle-2 font-weight-bold mb-4 mb-md-6 opacity-80 uppercase tracking-widest">Liên kết</div>
              <div v-for="item in navItems" :key="item.to" class="mb-3">
                <router-link :to="item.to" class="ft-link">{{ item.title }}</router-link>
              </div>
            </v-col>

            <v-col cols="6" md="3">
              <div class="text-subtitle-2 font-weight-bold mb-6 opacity-80 uppercase tracking-widest">Liên hệ</div>
              <div class="text-body-2 mb-4 d-flex ga-3">
                <v-icon size="18" color="primary">mdi-map-marker</v-icon>
                <span>Xốm, Phú Lương, Hà Nội</span>
              </div>
              <div class="text-body-2 mb-4 d-flex ga-3">
                <v-icon size="18" color="primary">mdi-phone</v-icon>
                <span>0243.869.1484</span>
              </div>
              <div class="text-body-2 d-flex ga-3">
                <v-icon size="18" color="primary">mdi-email</v-icon>
                <span>ktx@dainam.edu.vn</span>
              </div>
            </v-col>

            <v-col cols="12" md="3">
              <div class="text-subtitle-2 font-weight-bold mb-6 opacity-80 uppercase tracking-widest">Kết nối</div>
              <div class="d-flex ga-2">
                <v-btn icon="mdi-facebook" size="small" variant="tonal" />
                <v-btn icon="mdi-youtube" size="small" variant="tonal" />
                <v-btn icon="mdi-web" size="small" variant="tonal" />
              </div>
            </v-col>
          </v-row>

          <v-divider class="opacity-10 mb-6" />
          <div class="text-center text-caption opacity-50">
            © 2026 Trường Đại học Đại Nam — Ban Quản lý Ký túc xá.
          </div>
        </v-container>
      </footer>
    </v-main>
  </v-app>
</template>

<script setup>
import { ref } from 'vue'
import AppLogo from '@/components/common/AppLogo.vue'

const mob = ref(false)
const navItems = [
  { title:'Trang chủ', to:'/', icon:'mdi-home-outline' },
  { title:'Giới thiệu', to:'/about', icon:'mdi-information-outline' },
  { title:'Đăng ký phòng', to:'/rooms', icon:'mdi-home-search-outline' },
  { title:'Tiện ích chung', to:'/facilities', icon:'mdi-washing-machine' },
  { title:'Tin tức', to:'/news', icon:'mdi-newspaper-variant-outline' },
  { title:'Nội quy', to:'/rules', icon:'mdi-book-open-outline' },
  { title:'Liên hệ', to:'/contact', icon:'mdi-phone-outline' },
]
</script>

<style scoped>
.util-bar { background: #0f172a; color: #94a3b8; }
.main-nav { box-shadow: 0 1px 3px rgba(0,0,0,.06) !important; border-bottom: 1px solid #f1f5f9; }

.nav-link { 
  padding: 10px 20px; 
  font-size: 15px; 
  font-weight: 600; 
  color: #6b7280; 
  text-decoration: none; 
  transition: all .3s ease;
  position: relative;
  display: inline-block;
}

.nav-link:hover { 
  color: #ff6b00; 
}

.nav-link--active { 
  color: #ff6b00;
  font-weight: 700;
}

.nav-link--active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 20px;
  right: 20px;
  height: 3px;
  background: #ff6b00;
  border-radius: 3px 3px 0 0;
}

.ft { background: #0f172a; color: #e2e8f0; padding: 60px 0 30px; }
.ft-link { color: rgba(255,255,255,.65); text-decoration: none; font-size: 14px; transition: color .2s; }
.ft-link:hover { color: #fff; }

.opacity-70 { opacity: 0.7; }
.opacity-10 { opacity: 0.1; }
.opacity-50 { opacity: 0.5; }
.opacity-60 { opacity: 0.6; }
.opacity-80 { opacity: 0.8; }
</style>
