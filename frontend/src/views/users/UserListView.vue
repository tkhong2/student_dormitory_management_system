<template>
  <div>
    <div class="d-flex align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Quan ly nguoi dung</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Quan ly tai khoan Admin, Staff va Student</p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-account-plus" class="ml-auto" @click="openCreate">
        Tao tai khoan
      </v-btn>
    </div>

    <v-row style="margin-bottom: 16px;">
      <v-col v-for="r in roleStats" :key="r.role" cols="12" sm="4">
        <v-card class="pa-5 d-flex align-center ga-4" style="border:1px solid #e5e7eb">
          <v-avatar :color="r.bg" size="48" rounded="lg">
            <v-icon :color="r.color" size="24">{{ r.icon }}</v-icon>
          </v-avatar>
          <div>
            <div class="text-h5 font-weight-bold">{{ r.count }}</div>
            <div class="text-body-2 text-medium-emphasis">{{ r.role }}</div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-alert v-if="error" type="error" variant="tonal" class="mb-4" closable @click:close="error = ''">
      {{ error }}
    </v-alert>

    <v-card style="border:1px solid #e5e7eb">
      <div class="pa-4 d-flex flex-wrap ga-3 align-center">
        <v-text-field v-model="search" placeholder="Tim nguoi dung..." prepend-inner-icon="mdi-magnify" density="compact" hide-details style="max-width:300px" />
        <v-chip-group v-model="filterRole" class="ml-auto">
          <v-chip filter value="all">Tat ca</v-chip>
          <v-chip filter value="Admin" color="error">Admin</v-chip>
          <v-chip filter value="Staff" color="info">Staff</v-chip>
          <v-chip filter value="Student" color="success">Student</v-chip>
        </v-chip-group>
      </div>

      <v-data-table :headers="headers" :items="filteredUsers" :search="search" :loading="loading" items-per-page="10">
        <template #item.fullName="{ item }">
          <div class="d-flex align-center ga-3 py-2">
            <v-avatar size="36" :color="roleColor(item.role)" variant="tonal">
              <v-icon size="18">{{ roleIcon(item.role) }}</v-icon>
            </v-avatar>
            <div>
              <div class="text-body-2 font-weight-bold">{{ item.fullName }}</div>
              <div class="text-caption text-medium-emphasis">{{ item.email }}</div>
            </div>
          </div>
        </template>
        <template #item.role="{ item }">
          <v-chip :color="roleColor(item.role)" size="x-small" variant="tonal">{{ item.role }}</v-chip>
        </template>
        <template #item.status="{ item }">
          <div class="d-flex align-center ga-1 justify-center">
            <span class="dot-pulse" :style="{ background: item.isActive ? '#16a34a' : '#94a3b8' }" />
            <span class="text-body-2">{{ item.isActive ? 'Active' : 'Locked' }}</span>
          </div>
        </template>
        <template #item.actions="{ item }">
          <v-btn icon="mdi-pencil-outline" size="small" variant="text" @click="openEdit(item)" />
          <v-btn
            :icon="item.isActive ? 'mdi-lock-outline' : 'mdi-lock-open-outline'"
            size="small"
            variant="text"
            color="warning"
            @click="toggleStatus(item)"
          />
          <v-btn icon="mdi-delete-outline" size="small" variant="text" color="error" @click="deleteUser(item)" />
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialog" max-width="640">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-6">{{ editingId ? 'Cap nhat tai khoan' : 'Tao tai khoan moi' }}</h2>
        <v-row>
          <v-col cols="12" sm="6">
            <v-text-field v-model="form.username" label="Username" :disabled="!!editingId" />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field v-model="form.fullName" label="Ho va ten" />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field v-model="form.email" label="Email" type="email" />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field v-model="form.phoneNumber" label="So dien thoai" />
          </v-col>
          <v-col cols="12" sm="6">
            <v-select v-model="form.role" label="Vai tro" :items="roleOptions" />
          </v-col>
          <v-col cols="12" sm="6">
            <v-switch v-model="form.isActive" color="success" label="Dang hoat dong" hide-details />
          </v-col>
          <v-col v-if="!editingId" cols="12">
            <v-text-field v-model="form.password" label="Mat khau" type="password" />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog = false">Huy</v-btn>
          <v-btn color="primary" :loading="saving" @click="saveUser">
            {{ editingId ? 'Luu' : 'Tao' }}
          </v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { billingApi } from '@/services/api'

const search = ref('')
const filterRole = ref('all')
const dialog = ref(false)
const loading = ref(false)
const saving = ref(false)
const error = ref('')
const users = ref([])
const editingId = ref(null)

const emptyForm = () => ({
  username: '',
  password: '',
  fullName: '',
  email: '',
  phoneNumber: '',
  role: 'Student',
  isActive: true,
})

const form = ref(emptyForm())

const roleOptions = ['Admin', 'Staff', 'Student']

const headers = [
  { title: 'Nguoi dung', key: 'fullName' },
  { title: 'Username', key: 'username', align: 'center' },
  { title: 'Vai tro', key: 'role', align: 'center' },
  { title: 'Trang thai', key: 'status', align: 'center' },
  { title: 'Ngay tao', key: 'createdAtText', align: 'center' },
  { title: '', key: 'actions', align: 'end', sortable: false },
]

const normalizeRole = (role) => {
  const r = String(role || '').toLowerCase()
  if (r === 'admin') return 'Admin'
  if (r === 'staff') return 'Staff'
  if (r === 'student') return 'Student'
  return role || ''
}

const formatDate = (value) => {
  if (!value) return ''
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return String(value)
  return d.toLocaleDateString('vi-VN')
}

const mapUser = (u) => ({
  id: u.id,
  username: u.username,
  fullName: u.fullName,
  email: u.email,
  phoneNumber: u.phoneNumber,
  role: normalizeRole(u.role),
  isActive: u.isActive !== false,
  createdAt: u.createdAt,
  createdAtText: formatDate(u.createdAt),
})

const loadUsers = async () => {
  loading.value = true
  error.value = ''
  try {
    const apiUsers = await billingApi.get('/users')
    users.value = (apiUsers || []).map(mapUser)
  } catch (err) {
    error.value = err.message || 'Khong tai duoc danh sach user'
  } finally {
    loading.value = false
  }
}

const openCreate = () => {
  editingId.value = null
  form.value = emptyForm()
  dialog.value = true
}

const openEdit = (user) => {
  editingId.value = user.id
  form.value = {
    username: user.username,
    password: '',
    fullName: user.fullName,
    email: user.email,
    phoneNumber: user.phoneNumber,
    role: user.role,
    isActive: user.isActive,
  }
  dialog.value = true
}

const saveUser = async () => {
  error.value = ''
  saving.value = true
  try {
    const payload = {
      username: form.value.username,
      password: form.value.password,
      fullName: form.value.fullName,
      email: form.value.email,
      phoneNumber: form.value.phoneNumber,
      role: form.value.role,
      isActive: form.value.isActive,
    }

    if (editingId.value) {
      await billingApi.put(`/users/${editingId.value}`, payload)
    } else {
      await billingApi.post('/users', payload)
    }

    dialog.value = false
    await loadUsers()
  } catch (err) {
    error.value = err.message || 'Khong luu duoc user'
  } finally {
    saving.value = false
  }
}

const toggleStatus = async (user) => {
  error.value = ''
  try {
    await billingApi.put(`/users/${user.id}/status`, { isActive: !user.isActive })
    await loadUsers()
  } catch (err) {
    error.value = err.message || 'Khong cap nhat trang thai'
  }
}

const deleteUser = async (user) => {
  if (!window.confirm(`Xoa tai khoan ${user.username}?`)) return
  error.value = ''
  try {
    await billingApi.delete(`/users/${user.id}`)
    await loadUsers()
  } catch (err) {
    error.value = err.message || 'Khong xoa duoc user'
  }
}

onMounted(loadUsers)

const filteredUsers = computed(() => (
  filterRole.value === 'all'
    ? users.value
    : users.value.filter((u) => u.role === filterRole.value)
))

const roleStats = computed(() => [
  { role: 'Admin', count: users.value.filter((x) => x.role === 'Admin').length, icon: 'mdi-shield-crown', color: 'error', bg: '#fef2f2' },
  { role: 'Staff', count: users.value.filter((x) => x.role === 'Staff').length, icon: 'mdi-account-hard-hat', color: 'info', bg: '#e0f2fe' },
  { role: 'Student', count: users.value.filter((x) => x.role === 'Student').length, icon: 'mdi-school', color: 'success', bg: '#dcfce7' },
])

const roleColor = (r) => ({ Admin: 'error', Staff: 'info', Student: 'success' }[r] || 'grey')
const roleIcon = (r) => ({ Admin: 'mdi-shield-crown', Staff: 'mdi-account-hard-hat', Student: 'mdi-school' }[r] || 'mdi-account')
</script>

<style scoped>
.dot-pulse {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
}
</style>
