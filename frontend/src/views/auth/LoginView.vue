<template>
  <div class="login-container">
    <!-- Back Button -->
    <router-link to="/" class="back-button">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="margin-right: 8px;">
        <path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"/>
      </svg>
      Về trang chủ
    </router-link>

    <!-- Login Card -->
    <div class="login-card">
      <!-- Logo & Brand as Header -->
      <div class="login-header brand-header">
        <img src="/images/logo.png" style="width: 150px; height: 150px; object-fit: contain; margin-bottom: -4px;" alt="DNU Logo" />
        <div class="brand-text">
          <div class="brand-title">Đăng nhập</div>
        </div>
        <p style="margin-top: 8px;">Nhập thông tin tài khoản để truy cập hệ thống quản lý ký túc xá.</p>
      </div>

      <a-form 
        :model="form"
        @finish="handleLogin"
        layout="vertical"
      >
        <a-form-item 
          label="MÃ SINH VIÊN"
          name="username"
          :rules="[{ required: true, message: 'Vui lòng nhập mã sinh viên!' }]"
        >
          <a-input 
            v-model:value="form.username" 
            size="large"
            placeholder="VD: DNU2021xxxxx"
          >
            <template #prefix>
              <UserOutlined style="color: #bfbfbf;" />
            </template>
          </a-input>
        </a-form-item>

        <a-form-item 
          label="MẬT KHẨU"
          name="password"
          :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]"
        >
          <a-input-password 
            v-model:value="form.password" 
            size="large"
            placeholder="••••••••"
          >
            <template #prefix>
              <LockOutlined style="color: #bfbfbf;" />
            </template>
          </a-input-password>
        </a-form-item>

        <div class="form-options">
          <a-checkbox v-model:checked="rememberMe">
            Ghi nhớ đăng nhập
          </a-checkbox>
          <a href="#" class="forgot-link">Quên mật khẩu?</a>
        </div>

        <a-button 
          type="primary" 
          html-type="submit"
          size="large" 
          block
          :loading="loading"
          class="login-button"
        >
          Đăng nhập
        </a-button>

        <div class="divider-text">hoặc</div>

        <div class="register-text">
          Chưa có tài khoản? <a href="#" class="register-link">Liên hệ quản trị viên</a>
        </div>
      </a-form>
    </div>


  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { 
  LockOutlined,
  UserOutlined
} from '@ant-design/icons-vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const loading = ref(false)
const rememberMe = ref(false)
const form = reactive({ username: '', password: '' })

const handleLogin = () => {
  if (!form.username || !form.password) {
    return
  }

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
.login-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #ff8800 0%, #ff6b00 100%);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
  position: relative;
}

/* Back Button */
.back-button {
  position: absolute;
  top: 32px;
  left: 32px;
  display: flex;
  align-items: center;
  color: white;
  text-decoration: none;
  font-weight: 600;
  font-size: 14px;
  padding: 10px 20px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.back-button:hover {
  background: rgba(255, 255, 255, 0.25);
  color: white;
}

/* Brand Header */
.brand-header {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.brand-header .brand-text {
  color: #ff6b00;
  text-align: center;
}

.brand-header .brand-title {
  font-size: 32px;
  font-weight: 900;
  line-height: 1.2;
  letter-spacing: 1px;
  background: linear-gradient(135deg, #ff6b00 0%, #ffaa00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  color: #ff6b00;
}



/* Login Card */
.login-card {
  width: 100%;
  max-width: 480px;
  background: white;
  border-radius: 24px;
  padding: 32px 40px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  margin: 0 0 20px 0;
}

.login-header {
  text-align: center;
  margin-bottom: 24px;
}

.login-header h1 {
  font-size: 32px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 12px 0;
}

.login-header p {
  font-size: 14px;
  color: #8c8c8c;
  margin: 0;
  line-height: 1.6;
}

/* Form Styling */
:deep(.ant-form-item-label > label) {
  font-size: 12px;
  font-weight: 700;
  color: #8c8c8c;
  letter-spacing: 0.5px;
}

:deep(.ant-input-affix-wrapper),
:deep(.ant-input-password) {
  border-radius: 10px;
  border: 2px solid #e8e8e8;
  padding: 12px 16px;
  transition: all 0.3s ease;
}

:deep(.ant-input-affix-wrapper:hover),
:deep(.ant-input-password:hover) {
  border-color: #ffb366;
}

:deep(.ant-input-affix-wrapper:focus),
:deep(.ant-input-affix-wrapper-focused),
:deep(.ant-input-password:focus) {
  border-color: #ff6b00;
  box-shadow: 0 4px 12px rgba(255, 107, 0, 0.15);
}

:deep(.ant-input) {
  font-size: 15px;
  color: #1a1a1a;
}

:deep(.ant-input::placeholder) {
  color: #d9d9d9;
}

:deep(.ant-input-prefix) {
  margin-right: 12px;
  font-size: 16px;
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

:deep(.ant-checkbox-wrapper) {
  font-size: 14px;
  color: #666;
}

:deep(.ant-checkbox-checked .ant-checkbox-inner) {
  background-color: #ff6b00;
  border-color: #ff6b00;
}

.forgot-link {
  color: #ff6b00;
  text-decoration: none;
  font-size: 14px;
  font-weight: 600;
}

.forgot-link:hover {
  color: #ff8800;
}

.login-button {
  height: 50px;
  font-size: 16px;
  font-weight: 700;
  border-radius: 10px;
  background: linear-gradient(135deg, #ff8800 0%, #ff6b00 100%);
  border: none;
  box-shadow: 0 6px 20px rgba(255, 107, 0, 0.3);
  transition: all 0.3s ease;
}

.login-button:hover {
  background: linear-gradient(135deg, #ff9900 0%, #ff7700 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(255, 107, 0, 0.4);
}

.divider-text {
  text-align: center;
  color: #bfbfbf;
  font-size: 13px;
  margin: 24px 0;
  position: relative;
}

.divider-text::before,
.divider-text::after {
  content: '';
  position: absolute;
  top: 50%;
  width: 40%;
  height: 1px;
  background: #e8e8e8;
}

.divider-text::before {
  left: 0;
}

.divider-text::after {
  right: 0;
}

.register-text {
  text-align: center;
  color: #8c8c8c;
  font-size: 14px;
}

.register-link {
  color: #ff6b00;
  text-decoration: none;
  font-weight: 600;
}

.register-link:hover {
  color: #ff8800;
}



/* Responsive */
@media (max-width: 768px) {
  .back-button {
    position: static;
    margin-bottom: 20px;
  }
  
  .login-card {
    padding: 32px 24px;
    margin: 20px 0;
  }
}
</style>
