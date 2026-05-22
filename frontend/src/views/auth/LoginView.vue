<template>
  <v-app>
    <v-container fluid class="pa-0 fill-height">
      <v-row no-gutters class="fill-height">
        <!-- Left Panel - Brand Section -->
        <v-col 
          cols="12" 
          md="6" 
          class="d-none d-md-flex align-center justify-center text-white left-panel"
        >
          <div class="pa-12">
            <div class="brand-logo mb-8">
              <v-icon size="60" color="white">mdi-white-balance-sunny</v-icon>
            </div>
            
            <h1 class="text-h2 font-weight-black mb-4">DNU KTX</h1>
            <div class="brand-divider mb-8"></div>
            
            <p class="text-h6 font-weight-regular mb-12" style="opacity: 0.95; line-height: 1.8; max-width: 500px;">
              Hệ thống quản lý ký túc xá thông minh — Môi trường sống lý tưởng cho sinh viên Đại học Đại Nam.
            </p>
            
            <div class="features-list">
              <div class="feature-item">
                <v-icon color="white" size="24">mdi-check-circle</v-icon>
                <span class="text-body-1">Quản lý phòng hiện đại</span>
              </div>
              <div class="feature-item">
                <v-icon color="white" size="24">mdi-check-circle</v-icon>
                <span class="text-body-1">Thanh toán điện tử tiện lợi</span>
              </div>
              <div class="feature-item">
                <v-icon color="white" size="24">mdi-check-circle</v-icon>
                <span class="text-body-1">Hỗ trợ SV 24/7</span>
              </div>
            </div>
          </div>
        </v-col>

        <!-- Right Panel - Login Form -->
        <v-col 
          cols="12" 
          md="6" 
          class="d-flex align-center justify-center right-panel"
        >
          <div class="login-container">
            <!-- Mobile Logo -->
            <div class="d-md-none text-center mb-8">
              <div class="mobile-logo mb-3">
                <v-icon size="48" color="primary">mdi-white-balance-sunny</v-icon>
              </div>
              <h2 class="text-h4 font-weight-black text-primary">DNU KTX</h2>
            </div>

            <!-- Login Header -->
            <div class="mb-8">
              <h2 class="text-h4 font-weight-bold mb-2">Đăng nhập</h2>
              <p class="text-body-1 text-medium-emphasis">Nhập thông tin tài khoản để truy cập hệ thống</p>
            </div>

            <!-- Login Form -->
            <v-form @submit.prevent="handleLogin">
              <div class="mb-6">
                <label class="form-label">Tên đăng nhập / Mã SV</label>
                <v-text-field
                  v-model="form.username"
                  placeholder="VD: admin, staff, hoặc sv001"
                  prepend-inner-icon="mdi-account-outline"
                  variant="outlined"
                  color="primary"
                  density="comfortable"
                  :error-messages="errors.username"
                  hide-details="auto"
                />
              </div>

              <div class="mb-4">
                <label class="form-label">Mật khẩu</label>
                <v-text-field
                  v-model="form.password"
                  placeholder="Nhập mật khẩu của bạn"
                  prepend-inner-icon="mdi-lock-outline"
                  :type="showPw ? 'text' : 'password'"
                  :append-inner-icon="showPw ? 'mdi-eye-off-outline' : 'mdi-eye-outline'"
                  @click:append-inner="showPw = !showPw"
                  variant="outlined"
                  color="primary"
                  density="comfortable"
                  :error-messages="errors.password"
                  @keyup.enter="handleLogin"
                  hide-details="auto"
                />
              </div>

              <div class="d-flex align-center justify-space-between mb-6">
                <v-checkbox 
                  label="Ghi nhớ đăng nhập" 
                  density="compact" 
                  hide-details 
                  color="primary"
                />
                <a href="#" class="text-primary text-decoration-none font-weight-medium">Quên mật khẩu?</a>
              </div>

              <v-btn 
                type="submit" 
                block 
                size="large"
                color="primary" 
                :loading="loading"
                class="font-weight-bold text-none mb-6"
                style="background-color: #ff6b00;"
              >
                Đăng nhập ngay
                <v-icon end>mdi-arrow-right</v-icon>
              </v-btn>

              <div class="text-center">
                <p class="text-body-2 text-medium-emphasis">
                  Bạn chưa có tài khoản? 
                  <a href="#" class="text-primary font-weight-bold text-decoration-none">Liên hệ Quản trị viên</a>
                </p>
              </div>
            </v-form>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const showPw = ref(false)
const loading = ref(false)
const form = reactive({ username: '', password: '' })
const errors = reactive({ username: '', password: '' })

const handleLogin = () => {
  errors.username = form.username ? '' : 'Vui lòng nhập tên đăng nhập'
  errors.password = form.password ? '' : 'Vui lòng nhập mật khẩu'
  if (errors.username || errors.password) return

  loading.value = true
  setTimeout(() => {
    let role = 'student'
    const userLower = form.username.toLowerCase()
    if (userLower.includes('admin')) role = 'admin'
    else if (userLower.includes('staff') || userLower.includes('nv')) role = 'staff'

    const userNames = { admin: 'Administrator', staff: 'Nhân viên KTX', student: 'Nguyễn Văn An' }
    authStore.login({ name: userNames[role] || 'User', role: role })
    router.push(role === 'student' ? '/student' : '/admin')
    loading.value = false
  }, 600)
}
</script>

<style scoped>
.left-panel {
  background: linear-gradient(135deg, #1e3a8a 0%, #0f172a 100%);
  position: relative;
  overflow: hidden;
}

.left-panel::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 0%, transparent 70%);
  animation: pulse 15s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: 0.5; }
  50% { transform: scale(1.2); opacity: 0.8; }
}

.brand-logo {
  width: 80px;
  height: 80px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(10px);
}

.brand-divider {
  width: 80px;
  height: 4px;
  background: linear-gradient(90deg, #ff6b00, transparent);
  border-radius: 2px;
}

.features-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 0;
}

.right-panel {
  background-color: #f8fafc;
}

.login-container {
  width: 100%;
  max-width: 480px;
  padding: 32px;
  background: white;
  border-radius: 24px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.mobile-logo {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #ff8800, #ff6b00);
  border-radius: 16px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.form-label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  color: #334155;
  margin-bottom: 8px;
}

:deep(.v-field) {
  border-radius: 12px;
}

:deep(.v-btn) {
  border-radius: 12px;
  text-transform: none;
  letter-spacing: 0;
}

@media (max-width: 960px) {
  .login-container {
    box-shadow: none;
    border-radius: 0;
  }
}
</style>
