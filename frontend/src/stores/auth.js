import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { billingApi } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(JSON.parse(localStorage.getItem('user')) || null)

  const isAdmin = computed(() => user.value?.role === 'admin')
  const isStaff = computed(() => user.value?.role === 'staff')
  const isStudent = computed(() => user.value?.role === 'student')

  const normalizeRole = (role) => {
    if (!role) return null
    const r = String(role).trim().toLowerCase()
    if (r === 'admin') return 'admin'
    if (r === 'staff') return 'staff'
    if (r === 'student') return 'student'
    return r
  }

  const login = async ({ username, password }) => {
    const auth = await billingApi.post('/auth/login', { username, password })
    localStorage.setItem('token', auth.token)

    const me = auth.user || await billingApi.get('/auth/me')
    const userData = {
      id: me.id,
      name: me.fullName || me.username,
      username: me.username,
      role: normalizeRole(me.role),
    }

    user.value = userData
    localStorage.setItem('user', JSON.stringify(userData))
    localStorage.setItem('role', userData.role)

    return userData
  }

  const logout = () => {
    user.value = null
    localStorage.clear()
  }

  return { user, isAdmin, isStaff, isStudent, login, logout }
})
