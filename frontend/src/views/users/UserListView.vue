<template>
  <div>
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Quản lý Người dùng</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Quản lý tài khoản Admin, Nhân viên và Sinh viên</p>
    </div>

    <!-- Role stats -->
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

    <v-card style="border:1px solid #e5e7eb">
      <div class="pa-4 d-flex flex-wrap ga-3 align-center">
        <v-text-field v-model="search" placeholder="Tìm người dùng..." prepend-inner-icon="mdi-magnify" density="compact" hide-details style="max-width:300px" />
        <v-chip-group v-model="filterRole" class="ml-auto">
          <v-chip filter value="all">Tất cả</v-chip>
          <v-chip filter value="Admin" color="error">Admin</v-chip>
          <v-chip filter value="Nhân viên" color="info">Nhân viên</v-chip>
          <v-chip filter value="Sinh viên" color="success">Sinh viên</v-chip>
        </v-chip-group>
      </div>

      <v-data-table :headers="headers" :items="filteredUsers" :search="search" items-per-page="10">
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
          <div class="d-flex align-center ga-1">
            <span class="dot-pulse" :style="{background:item.active?'#16a34a':'#94a3b8'}" />
            <span class="text-body-2">{{ item.active ? 'Hoạt động' : 'Vô hiệu' }}</span>
          </div>
        </template>
        <template #item.actions>
          <v-btn icon="mdi-pencil-outline" size="small" variant="text" />
          <v-btn icon="mdi-lock-reset" size="small" variant="text" color="warning" title="Reset mật khẩu" />
          <v-btn icon="mdi-delete-outline" size="small" variant="text" color="error" />
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialog" max-width="560">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Thêm người dùng mới</h2>
        <v-row>
          <v-col cols="6"><v-text-field label="Tên đăng nhập" /></v-col>
          <v-col cols="6"><v-text-field label="Họ và tên" /></v-col>
          <v-col cols="6"><v-text-field label="Email" type="email" /></v-col>
          <v-col cols="6"><v-select label="Vai trò" :items="['Admin','Nhân viên','Sinh viên']" /></v-col>
          <v-col cols="6"><v-text-field label="Mật khẩu" type="password" /></v-col>
          <v-col cols="6"><v-text-field label="Xác nhận mật khẩu" type="password" /></v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog=false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog=false">Tạo tài khoản</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
const search = ref('')
const filterRole = ref('all')
const dialog = ref(false)

const roleStats = [
  { role:'Quản trị viên', count:2, icon:'mdi-shield-crown', color:'error', bg:'#fef2f2' },
  { role:'Nhân viên', count:8, icon:'mdi-account-hard-hat', color:'info', bg:'#e0f2fe' },
  { role:'Sinh viên', count:1248, icon:'mdi-school', color:'success', bg:'#dcfce7' },
]

const headers = [
  { title:'Người dùng', key:'fullName' },
  { title:'Username', key:'username', align:'center' },
  { title:'Vai trò', key:'role', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'Ngày tạo', key:'created', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]

const users = ref([
  { id:1,username:'admin',fullName:'Nguyễn Quản Trị',email:'admin@univ.edu.vn',role:'Admin',active:true,created:'01/01/2026' },
  { id:2,username:'admin2',fullName:'Trần Phó Giám đốc',email:'admin2@univ.edu.vn',role:'Admin',active:true,created:'01/01/2026' },
  { id:3,username:'nv001',fullName:'Lê Thị Nhân Viên',email:'nv1@univ.edu.vn',role:'Nhân viên',active:true,created:'15/01/2026' },
  { id:4,username:'nv002',fullName:'Phạm Văn Kế Toán',email:'nv2@univ.edu.vn',role:'Nhân viên',active:true,created:'15/01/2026' },
  { id:5,username:'sv001',fullName:'Nguyễn Văn An',email:'vanan@gmail.com',role:'Sinh viên',active:true,created:'01/09/2025' },
  { id:6,username:'sv002',fullName:'Trần Thị Bình',email:'thib@gmail.com',role:'Sinh viên',active:true,created:'01/09/2025' },
  { id:7,username:'sv003',fullName:'Lê Minh Cường',email:'minhc@gmail.com',role:'Sinh viên',active:false,created:'01/09/2025' },
  { id:8,username:'nv003',fullName:'Hoàng Bảo Trì',email:'nv3@univ.edu.vn',role:'Nhân viên',active:true,created:'01/02/2026' },
])

const filteredUsers = computed(() => filterRole.value === 'all' ? users.value : users.value.filter(u => u.role === filterRole.value))
const roleColor = r => ({'Admin':'error','Nhân viên':'info','Sinh viên':'success'}[r]||'grey')
const roleIcon = r => ({'Admin':'mdi-shield-crown','Nhân viên':'mdi-account-hard-hat','Sinh viên':'mdi-school'}[r]||'mdi-account')
</script>
