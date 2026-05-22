import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user')) || null)

  const isAdmin = computed(() => user.value?.role === 'admin')
  const isStaff = computed(() => user.value?.role === 'staff')
  const isStudent = computed(() => user.value?.role === 'student')

  const login = (userData) => {
    user.value = userData
    localStorage.setItem('user', JSON.stringify(userData))
    localStorage.setItem('role', userData.role)
    localStorage.setItem('token', 'mock-jwt')
  }

  const logout = () => {
    user.value = null
    localStorage.clear()
  }

  return { user, isAdmin, isStaff, isStudent, login, logout }
})
