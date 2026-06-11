import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'

const API_URL = import.meta.env.VITE_BILLING_MAINTENANCE_API_URL || 'http://localhost:5002/api'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref(localStorage.getItem('token') || null)
  const refreshToken = ref(localStorage.getItem('refreshToken') || null)
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))

  // Getters
  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const isStaff = computed(() => user.value?.role === 'Staff')
  const isStudent = computed(() => user.value?.role === 'Student')

  // Actions
  const login = async (username, password) => {
    try {
      const response = await axios.post(`${API_URL}/auth/login`, {
        username,
        password
      })

      const { token: newToken, refreshToken: newRefreshToken, user: userData } = response.data

      // Save to state
      token.value = newToken
      refreshToken.value = newRefreshToken
      user.value = userData

      // Save to localStorage
      localStorage.setItem('token', newToken)
      localStorage.setItem('refreshToken', newRefreshToken)
      localStorage.setItem('user', JSON.stringify(userData))

      // Set axios default header
      axios.defaults.headers.common['Authorization'] = `Bearer ${newToken}`

      return userData
    } catch (error) {
      console.error('Login error:', error)
      if (error.response?.data?.message) {
        throw new Error(error.response.data.message)
      }
      throw new Error('Đăng nhập thất bại. Vui lòng thử lại.')
    }
  }

  const logout = () => {
    token.value = null
    refreshToken.value = null
    user.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')

    delete axios.defaults.headers.common['Authorization']
  }

  const refreshAccessToken = async () => {
    try {
      if (!refreshToken.value) {
        throw new Error('No refresh token')
      }

      const response = await axios.post(`${API_URL}/auth/refresh-token`, {
        refreshToken: refreshToken.value
      })

      const { token: newToken, refreshToken: newRefreshToken } = response.data

      token.value = newToken
      refreshToken.value = newRefreshToken

      localStorage.setItem('token', newToken)
      localStorage.setItem('refreshToken', newRefreshToken)

      axios.defaults.headers.common['Authorization'] = `Bearer ${newToken}`

      return newToken
    } catch (error) {
      console.error('Refresh token error:', error)
      logout()
      throw error
    }
  }

  const changePassword = async (userId, oldPassword, newPassword) => {
    try {
      await axios.post(`${API_URL}/auth/change-password`, {
        userId,
        oldPassword,
        newPassword
      })
    } catch (error) {
      console.error('Change password error:', error)
      if (error.response?.data?.message) {
        throw new Error(error.response.data.message)
      }
      throw new Error('Đổi mật khẩu thất bại. Vui lòng thử lại.')
    }
  }

  const updateUserProfile = (updatedUser) => {
    // Update user info in store and localStorage
    user.value = { ...user.value, ...updatedUser }
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  const refreshUserData = async () => {
    // Fetch fresh user data from API
    try {
      if (!user.value?.id) return
      
      const response = await axios.get(`${API_URL}/users/${user.value.id}`, {
        headers: {
          'Authorization': `Bearer ${token.value}`
        }
      })
      
      user.value = response.data
      localStorage.setItem('user', JSON.stringify(response.data))
    } catch (error) {
      console.error('Error refreshing user data:', error)
    }
  }

  // Initialize axios interceptor for auto token refresh
  const initializeInterceptor = () => {
    axios.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config

        if (
          error.response?.status === 401 &&
          !originalRequest._retry &&
          originalRequest.url &&
          !originalRequest.url.includes('/auth/login') &&
          !originalRequest.url.includes('/auth/refresh-token')
        ) {
          originalRequest._retry = true

          try {
            await refreshAccessToken()
            return axios(originalRequest)
          } catch (refreshError) {
            return Promise.reject(refreshError)
          }
        }

        return Promise.reject(error)
      }
    )
  }

  // Set token on store initialization
  if (token.value) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token.value}`
  }

  return {
    // State
    token,
    refreshToken,
    user,
    // Getters
    isAuthenticated,
    isAdmin,
    isStaff,
    isStudent,
    // Actions
    login,
    logout,
    refreshAccessToken,
    changePassword,
    updateUserProfile,
    refreshUserData,
    initializeInterceptor
  }
})
