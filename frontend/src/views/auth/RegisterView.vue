<template>
  <div class="register-container">
    <!-- Back Button -->
    <router-link to="/login" class="back-button">
      <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="margin-right: 8px;">
        <path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"/>
      </svg>
      Quay lại đăng nhập
    </router-link>

    <div class="register-card">
      <!-- Logo & Title -->
      <div class="register-header brand-header">
        <img src="/images/logo.png" style="width: 120px; height: 120px; object-fit: contain; margin-bottom: -4px;" alt="DNU Logo" />
        <div class="brand-text">
          <div class="brand-title">Đăng ký tài khoản</div>
        </div>
        <p style="margin-top: 8px; color: #8c8c8c; font-size: 14px;">Đăng ký để sử dụng dịch vụ ký túc xá</p>
      </div>

      <!-- Register Form -->
      <a-form
        :model="formState"
        :rules="rules"
        layout="vertical"
        @finish="handleRegister"
        style="margin-top: 24px;"
      >
        <!-- Thông tin sinh viên -->
        <a-divider orientation="left" style="font-size: 14px; color: #595959;">
          Thông tin sinh viên
        </a-divider>

        <a-form-item label="Mã sinh viên" name="studentCode" has-feedback>
          <a-input
            v-model:value="formState.studentCode"
            placeholder="VD: 2024001"
            size="large"
            :prefix="h(IdcardOutlined)"
          />
        </a-form-item>

        <a-form-item label="Họ và tên" name="fullName" has-feedback>
          <a-input
            v-model:value="formState.fullName"
            placeholder="Nhập họ tên đầy đủ"
            size="large"
            :prefix="h(UserOutlined)"
          />
        </a-form-item>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Ngày sinh" name="dateOfBirth" has-feedback>
              <a-date-picker
                v-model:value="formState.dateOfBirth"
                placeholder="Chọn ngày sinh"
                size="large"
                format="DD/MM/YYYY"
                style="width: 100%;"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Giới tính" name="gender" has-feedback>
              <a-select
                v-model:value="formState.gender"
                placeholder="Chọn giới tính"
                size="large"
              >
                <a-select-option value="Male">Nam</a-select-option>
                <a-select-option value="Female">Nữ</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="Số điện thoại" name="phone" has-feedback>
          <a-input
            v-model:value="formState.phone"
            placeholder="VD: 0901234567"
            size="large"
            :prefix="h(PhoneOutlined)"
          />
        </a-form-item>

        <a-form-item label="Email" name="email" has-feedback>
          <a-input
            v-model:value="formState.email"
            placeholder="student@example.com"
            size="large"
            :prefix="h(MailOutlined)"
          />
        </a-form-item>

        <a-form-item label="CMND/CCCD" name="identificationCard" has-feedback>
          <a-input
            v-model:value="formState.identificationCard"
            placeholder="Số CMND hoặc CCCD"
            size="large"
            :prefix="h(IdcardOutlined)"
          />
        </a-form-item>

        <!-- Thông tin tài khoản -->
        <a-divider orientation="left" style="font-size: 14px; color: #595959; margin-top: 24px;">
          Thông tin tài khoản
        </a-divider>

        <a-form-item label="Tên đăng nhập" name="username" has-feedback>
          <a-input
            v-model:value="formState.username"
            placeholder="VD: student2024"
            size="large"
            :prefix="h(UserOutlined)"
          />
        </a-form-item>

        <a-form-item label="Mật khẩu" name="password" has-feedback>
          <a-input-password
            v-model:value="formState.password"
            placeholder="Mật khẩu (tối thiểu 6 ký tự)"
            size="large"
            :prefix="h(LockOutlined)"
          />
        </a-form-item>

        <a-form-item label="Xác nhận mật khẩu" name="confirmPassword" has-feedback>
          <a-input-password
            v-model:value="formState.confirmPassword"
            placeholder="Nhập lại mật khẩu"
            size="large"
            :prefix="h(LockOutlined)"
          />
        </a-form-item>

        <!-- Terms -->
        <a-form-item name="agree">
          <a-checkbox v-model:checked="formState.agree">
            Tôi đã đọc và đồng ý với 
            <a href="#" @click.prevent="showTerms">Điều khoản sử dụng</a> và 
            <a href="#" @click.prevent="showPrivacy">Chính sách bảo mật</a>
          </a-checkbox>
        </a-form-item>

        <!-- Submit Button -->
        <a-form-item>
          <a-button
            type="primary"
            html-type="submit"
            size="large"
            block
            :loading="loading"
            style="background: #ff6b00; border-color: #ff6b00; height: 48px; font-size: 16px; font-weight: 600;"
          >
            Đăng ký
          </a-button>
        </a-form-item>

        <!-- Login Link -->
        <div style="text-align: center; margin-top: 16px;">
          <span style="color: #8c8c8c;">Đã có tài khoản?</span>
          <a @click="goToLogin" style="margin-left: 8px; font-weight: 600;">Đăng nhập ngay</a>
        </div>
      </a-form>
    </div>

    <!-- Contact Info -->
    <div class="contact-info">
      <a-alert
        message="Cần hỗ trợ?"
        type="info"
        show-icon
      >
        <template #description>
          <div>
            <p style="margin: 0;">Nếu gặp khó khăn trong quá trình đăng ký, vui lòng liên hệ:</p>
            <p style="margin: 8px 0 0 0;">
              <PhoneOutlined /> Hotline: <strong>1900-xxxx</strong><br />
              <MailOutlined /> Email: <strong>ktx@university.edu.vn</strong><br />
              <HomeOutlined /> Văn phòng KTX: Tầng 1, Tòa A
            </p>
          </div>
        </template>
      </a-alert>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, h } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import {
  UserOutlined, LockOutlined, MailOutlined, PhoneOutlined,
  IdcardOutlined, HomeOutlined
} from '@ant-design/icons-vue'
import api from '@/services/api'
import dayjs from 'dayjs'

const router = useRouter()
const loading = ref(false)

const formState = reactive({
  // Student info
  studentCode: '',
  fullName: '',
  dateOfBirth: null,
  gender: undefined,
  phone: '',
  email: '',
  identificationCard: '',
  // Account info
  username: '',
  password: '',
  confirmPassword: '',
  agree: false
})

const validateConfirmPassword = async (_rule, value) => {
  if (value === '') {
    return Promise.reject('Vui lòng xác nhận mật khẩu')
  } else if (value !== formState.password) {
    return Promise.reject('Mật khẩu xác nhận không khớp')
  } else {
    return Promise.resolve()
  }
}

const validateAgree = async (_rule, value) => {
  if (!value) {
    return Promise.reject('Bạn phải đồng ý với điều khoản sử dụng')
  }
  return Promise.resolve()
}

const rules = {
  studentCode: [
    { required: true, message: 'Vui lòng nhập mã sinh viên', trigger: 'blur' },
    { min: 5, max: 20, message: 'Mã sinh viên từ 5-20 ký tự', trigger: 'blur' }
  ],
  fullName: [
    { required: true, message: 'Vui lòng nhập họ tên', trigger: 'blur' },
    { min: 3, message: 'Họ tên phải có ít nhất 3 ký tự', trigger: 'blur' }
  ],
  dateOfBirth: [
    { required: true, message: 'Vui lòng chọn ngày sinh', trigger: 'change' }
  ],
  gender: [
    { required: true, message: 'Vui lòng chọn giới tính', trigger: 'change' }
  ],
  phone: [
    { required: true, message: 'Vui lòng nhập số điện thoại', trigger: 'blur' },
    { pattern: /^[0-9]{10}$/, message: 'Số điện thoại không hợp lệ', trigger: 'blur' }
  ],
  email: [
    { required: true, message: 'Vui lòng nhập email', trigger: 'blur' },
    { type: 'email', message: 'Email không hợp lệ', trigger: 'blur' }
  ],
  identificationCard: [
    { required: true, message: 'Vui lòng nhập số CMND/CCCD', trigger: 'blur' },
    { pattern: /^[0-9]{9,12}$/, message: 'CMND/CCCD không hợp lệ', trigger: 'blur' }
  ],
  username: [
    { required: true, message: 'Vui lòng nhập tên đăng nhập', trigger: 'blur' },
    { min: 4, max: 50, message: 'Tên đăng nhập từ 4-50 ký tự', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_]+$/, message: 'Chỉ chứa chữ, số và dấu gạch dưới', trigger: 'blur' }
  ],
  password: [
    { required: true, message: 'Vui lòng nhập mật khẩu', trigger: 'blur' },
    { min: 6, message: 'Mật khẩu phải có ít nhất 6 ký tự', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, validator: validateConfirmPassword, trigger: 'blur' }
  ],
  agree: [
    { required: true, validator: validateAgree, trigger: 'change' }
  ]
}

const handleRegister = async () => {
  loading.value = true
  try {
    // Prepare data
    const registerData = {
      // Student
      studentCode: formState.studentCode,
      fullName: formState.fullName,
      dateOfBirth: formState.dateOfBirth ? dayjs(formState.dateOfBirth).format('YYYY-MM-DD') : null,
      gender: formState.gender,
      phone: formState.phone,
      email: formState.email,
      identificationCard: formState.identificationCard,
      // User
      username: formState.username,
      password: formState.password,
      role: 'Student'
    }

    // Call API
    await api.post('/auth/register', registerData)

    message.success('Đăng ký thành công! Vui lòng đăng nhập.')
    
    // Redirect to login after 2s
    setTimeout(() => {
      router.push('/login')
    }, 2000)

  } catch (error) {
    console.error('Register error:', error)
    message.error(error.message || 'Đăng ký thất bại. Vui lòng thử lại.')
  } finally {
    loading.value = false
  }
}

const goToLogin = () => {
  router.push('/login')
}

const showTerms = () => {
  message.info('Tính năng xem điều khoản sử dụng')
}

const showPrivacy = () => {
  message.info('Tính năng xem chính sách bảo mật')
}
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #ff8800 0%, #ff6b00 100%);
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

.register-card {
  background: white;
  border-radius: 16px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  padding: 40px;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.register-header {
  text-align: center;
  margin-bottom: 24px;
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
  font-size: 28px;
  font-weight: 900;
  line-height: 1.2;
  letter-spacing: 1px;
  background: linear-gradient(135deg, #ff6b00 0%, #ffaa00 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  color: #ff6b00;
}

.logo {
  margin-bottom: 16px;
}

.contact-info {
  margin-top: 24px;
  width: 100%;
  max-width: 600px;
}

:deep(.ant-input-affix-wrapper) {
  border-radius: 8px;
}

:deep(.ant-input) {
  border-radius: 8px;
}

:deep(.ant-select-selector) {
  border-radius: 8px !important;
}

:deep(.ant-picker) {
  border-radius: 8px;
}

/* Scrollbar styling */
.register-card::-webkit-scrollbar {
  width: 6px;
}

.register-card::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.register-card::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 10px;
}

.register-card::-webkit-scrollbar-thumb:hover {
  background: #555;
}
/* Responsive */
@media (max-width: 768px) {
  .back-button {
    position: static;
    margin-bottom: 20px;
  }

  .register-card {
    padding: 32px 24px;
  }
}
</style>
