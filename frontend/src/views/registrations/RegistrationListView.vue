<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Đăng ký phòng</h1>
        <p class="text-body-2 text-medium-emphasis">Quản lý các đơn đăng ký phòng ở của sinh viên</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary" @click="dialog=true">Tạo đơn đăng ký</v-btn>
    </div>

    <!-- Stats -->
    <v-row class="mb-6">
      <v-col v-for="s in regStats" :key="s.label" cols="6" md="3">
        <v-card class="pa-4 text-center" style="border:1px solid #e5e7eb">
          <div class="text-h5 font-weight-bold" :class="'text-'+s.color">{{ s.value }}</div>
          <div class="text-caption text-medium-emphasis">{{ s.label }}</div>
        </v-card>
      </v-col>
    </v-row>

    <v-card style="border:1px solid #e5e7eb">
      <v-data-table :headers="headers" :items="registrations" items-per-page="10">
        <template #item.student="{ item }">
          <div class="d-flex align-center ga-3 py-2">
            <v-avatar size="32" color="primary" variant="tonal"><v-icon size="16">mdi-account</v-icon></v-avatar>
            <div>
              <div class="text-body-2 font-weight-medium">{{ item.student }}</div>
              <div class="text-caption text-medium-emphasis">{{ item.code }}</div>
            </div>
          </div>
        </template>
        <template #item.status="{ item }">
          <v-chip :color="statusColor(item.status)" size="x-small" variant="tonal">{{ item.status }}</v-chip>
        </template>
        <template #item.actions="{ item }">
          <v-btn v-if="item.status==='Chờ duyệt'" size="small" color="success" variant="tonal" class="mr-1">Duyệt</v-btn>
          <v-btn v-if="item.status==='Chờ duyệt'" size="small" color="error" variant="tonal">Từ chối</v-btn>
          <v-btn v-else icon="mdi-eye-outline" size="small" variant="text" color="primary" />
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialog" max-width="560">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Tạo đơn đăng ký phòng</h2>
        <v-select label="Sinh viên" :items="['SV003 - Lê Minh Cường','SV005 - Hoàng Thị Ê','SV008 - Đặng Văn Hải']" class="mb-3" />
        <v-select label="Tòa nhà mong muốn" :items="['Tòa A1','Tòa A2','Tòa B1','Tòa C1']" class="mb-3" />
        <v-select label="Loại phòng" :items="['Tiêu chuẩn (4 SV)','Cao cấp (2 SV)','VIP (1 SV)']" class="mb-3" />
        <v-textarea label="Ghi chú" rows="3" />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog=false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog=false">Gửi đăng ký</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
const dialog = ref(false)
const regStats = [
  { label:'Tổng đơn', value:'24', color:'primary' },
  { label:'Chờ duyệt', value:'8', color:'warning' },
  { label:'Đã duyệt', value:'14', color:'success' },
  { label:'Từ chối', value:'2', color:'error' },
]
const headers = [
  { title:'Sinh viên', key:'student' },
  { title:'Phòng yêu cầu', key:'room', align:'center' },
  { title:'Ngày đăng ký', key:'date', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'Thao tác', key:'actions', align:'end', sortable:false },
]
const registrations = ref([
  { id:1,student:'Lê Minh Cường',code:'SV003',room:'Tiêu chuẩn – Tòa A1',date:'15/05/2026',status:'Chờ duyệt' },
  { id:2,student:'Hoàng Thị Ê',code:'SV005',room:'Cao cấp – Tòa A2',date:'14/05/2026',status:'Chờ duyệt' },
  { id:3,student:'Đặng Văn Hải',code:'SV008',room:'Tiêu chuẩn – Tòa B1',date:'13/05/2026',status:'Chờ duyệt' },
  { id:4,student:'Nguyễn Văn An',code:'SV001',room:'Tiêu chuẩn – Tòa A1',date:'01/04/2026',status:'Đã duyệt' },
  { id:5,student:'Trần Thị Bình',code:'SV002',room:'Tiêu chuẩn – Tòa A2',date:'01/04/2026',status:'Đã duyệt' },
  { id:6,student:'Phạm Văn X',code:'SV009',room:'VIP – Tòa D1',date:'10/05/2026',status:'Từ chối' },
])

const statusColor = s => ({ 'Chờ duyệt':'warning','Đã duyệt':'success','Từ chối':'error' }[s]||'grey')
</script>
