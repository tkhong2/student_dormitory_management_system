<template>
  <v-app>
    <!-- Top utility bar -->
    <div class="util-bar d-none d-sm-block">
      <v-container class="d-flex align-center justify-space-between py-0" style="max-width:1280px; height:40px">
        <div class="d-flex align-center ga-6 text-caption font-weight-medium">
          <span><v-icon size="14" class="mr-1">mdi-phone</v-icon> 0243.869.1484</span>
          <span><v-icon size="14" class="mr-1">mdi-email-outline</v-icon> ktx@dainam.edu.vn</span>
          <span class="opacity-70"><v-icon size="14" class="mr-1">mdi-clock-outline</v-icon> T2-T6: 7h30 – 16h30</span>
        </div>
        <div class="d-flex align-center ga-4">
          <v-btn variant="text" size="x-small" class="text-white opacity-80" to="/login">
            <v-icon size="14" start>mdi-login</v-icon> Đăng nhập
          </v-btn>
          <v-btn size="x-small" color="warning" variant="flat" class="font-weight-bold" to="/rooms">
            Đăng ký phòng
          </v-btn>
        </div>
      </v-container>
    </div>

    <!-- Main nav -->
    <v-app-bar flat color="white" height="80" class="main-nav">
      <v-container class="d-flex align-center py-0" style="max-width:1280px">
        <!-- Logo -->
        <router-link to="/" class="d-flex align-center text-decoration-none ga-3">
          <div class="uni-logo">
            <v-icon size="26" color="white">mdi-white-balance-sunny</v-icon>
          </div>
          <div>
            <div class="uni-name">ĐẠI HỌC ĐẠI NAM</div>
            <div class="uni-sub">BAN QUẢN LÝ KÝ TÚC XÁ</div>
          </div>
        </router-link>

        <v-spacer />

        <!-- Desktop Menu -->
        <div class="d-none d-md-flex align-center ga-1 nav-container pa-1">
          <router-link 
            v-for="item in navItems" 
            :key="item.to" 
            :to="item.to" 
            class="nav-link"
            :class="{'nav-link--active': $route.path === item.to}"
          >
            {{ item.title }}
          </router-link>
        </div>

        <div class="d-none d-lg-flex align-center ml-6 ga-3">
          <v-btn to="/login" variant="text" color="primary" class="font-weight-bold">Đăng nhập</v-btn>
          <!-- <v-btn to="/login" color="primary" class="font-weight-bold px-6">Bắt đầu</v-btn> -->
        </div>

        <!-- Mobile Menu Toggle -->
        <v-btn icon="mdi-menu" class="d-md-none" variant="text" color="primary" @click="mob = !mob" />
      </v-container>
    </v-app-bar>

    <!-- Mobile Drawer -->
    <v-navigation-drawer v-model="mob" location="right" temporary width="300">
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
        <v-btn block color="primary" class="mb-2" to="/rooms">Đăng ký phòng</v-btn>
        <v-btn block variant="outlined" color="primary" to="/login">Đăng nhập</v-btn>
      </v-list>
    </v-navigation-drawer>

    <!-- Main Content -->
    <v-main style="background-color: #fff">
      <router-view v-slot="{ Component }">
        <transition name="page" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>

      <!-- Footer -->
      <footer class="ft">
        <v-container style="max-width:1280px">
          <v-row class="mb-10">
            <v-col cols="12" md="4">
              <div class="d-flex align-center ga-3 mb-6">
                <div class="uni-logo-sm"><v-icon color="white" size="20">mdi-school</v-icon></div>
                <div>
                  <div class="font-weight-bold">Trường Đại học Đại Nam</div>
                  <div class="text-caption opacity-60 uppercase">Ban quản lý ký túc xá</div>
                </div>
              </div>
              <p class="text-body-2 opacity-70" style="line-height:1.7">
                Ký túc xá Đại học Đại Nam — môi trường sinh hoạt lý tưởng, an toàn và hiện đại cho sinh viên yên tâm học tập và phát triển.
              </p>
            </v-col>

            <v-col cols="6" md="2">
              <div class="text-subtitle-2 font-weight-bold mb-6 opacity-80 uppercase tracking-widest">Liên kết</div>
              <div v-for="item in navItems" :key="item.to" class="mb-3">
                <router-link :to="item.to" class="ft-link">{{ item.title }}</router-link>
              </div>
            </v-col>

            <v-col cols="6" md="3">
              <div class="text-subtitle-2 font-weight-bold mb-6 opacity-80 uppercase tracking-widest">Liên hệ</div>
              <div class="text-body-2 mb-4 d-flex ga-3">
                <v-icon size="18" color="primary">mdi-map-marker</v-icon>
                <span>56 Vũ Trọng Phụng, Thanh Xuân, HN</span>
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
const mob = ref(false)
const navItems = [
  { title:'Trang chủ', to:'/', icon:'mdi-home-outline' },
  { title:'Giới thiệu', to:'/about', icon:'mdi-information-outline' },
  { title:'Đăng ký phòng', to:'/rooms', icon:'mdi-home-search-outline' },
  { title:'Tin tức', to:'/news', icon:'mdi-newspaper-variant-outline' },
  { title:'Nội quy', to:'/rules', icon:'mdi-book-open-outline' },
  { title:'Liên hệ', to:'/contact', icon:'mdi-phone-outline' },
]
</script>

<style scoped>
.util-bar { background: #0f172a; color: #94a3b8; }
.main-nav { box-shadow: 0 1px 3px rgba(0,0,0,.06) !important; border-bottom: 1px solid #f1f5f9; }

.uni-logo { width: 42px; height: 42px; background: linear-gradient(135deg, #ff8800, #ff6b00); border-radius: 12px; display: flex; align-items: center; justify-content: center; }
.uni-logo-sm { width: 34px; height: 34px; background: linear-gradient(135deg, #ff8800, #ff6b00); border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.uni-name { font-size: 18px; font-weight: 900; color: #ff6b00; line-height: 1; }
.uni-sub { font-size: 10px; font-weight: 700; color: #1e3a8a; margin-top: 4px; }

.nav-container { background: #f8fafc; border-radius: 12px; }
.nav-link { padding: 8px 16px; font-size: 14px; font-weight: 700; color: #64748b; text-decoration: none; border-radius: 10px; transition: all .2s; }
.nav-link:hover { color: #ff6b00; }
.nav-link--active { background: white; color: #ff6b00; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }

.ft { background: #0f172a; color: #e2e8f0; padding: 60px 0 30px; }
.ft-link { color: rgba(255,255,255,.65); text-decoration: none; font-size: 14px; transition: color .2s; }
.ft-link:hover { color: #fff; }

.opacity-70 { opacity: 0.7; }
.opacity-10 { opacity: 0.1; }
.opacity-50 { opacity: 0.5; }
.opacity-60 { opacity: 0.6; }
</style>
