<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Hợp đồng thuê phòng</h1>
        <p class="text-body-2 text-medium-emphasis">Quản lý hợp đồng KTX của sinh viên</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary" @click="dialog=true">Tạo hợp đồng mới</v-btn>
    </div>

    <!-- Summary -->
    <v-row class="mb-6">
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#ede9fe" size="40" rounded="lg"><v-icon color="primary" size="20">mdi-file-document-multiple</v-icon></v-avatar>
            <div><div class="text-h6 font-weight-bold">{{ contracts.length }}</div><div class="text-caption text-medium-emphasis">Tổng hợp đồng</div></div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#dcfce7" size="40" rounded="lg"><v-icon color="success" size="20">mdi-check-circle</v-icon></v-avatar>
            <div><div class="text-h6 font-weight-bold">{{ contracts.filter(c=>c.status==='Hiệu lực').length }}</div><div class="text-caption text-medium-emphasis">Đang hiệu lực</div></div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fff7ed" size="40" rounded="lg"><v-icon color="warning" size="20">mdi-clock-alert</v-icon></v-avatar>
            <div><div class="text-h6 font-weight-bold">3</div><div class="text-caption text-medium-emphasis">Sắp hết hạn</div></div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fef2f2" size="40" rounded="lg"><v-icon color="error" size="20">mdi-cancel</v-icon></v-avatar>
            <div><div class="text-h6 font-weight-bold">{{ contracts.filter(c=>c.status==='Đã chấm dứt').length }}</div><div class="text-caption text-medium-emphasis">Đã chấm dứt</div></div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-card style="border:1px solid #e5e7eb">
      <v-data-table :headers="headers" :items="contracts" items-per-page="10">
        <template #item.student="{ item }">
          <span class="font-weight-medium">{{ item.student }}</span>
        </template>
        <template #item.price="{ item }">
          <span class="font-weight-bold">{{ item.price }}</span>
        </template>
        <template #item.status="{ item }">
          <v-chip :color="item.status==='Hiệu lực'?'success':item.status==='Sắp hết hạn'?'warning':'error'" size="x-small" variant="tonal">{{ item.status }}</v-chip>
        </template>
        <template #item.actions>
          <v-btn icon="mdi-eye-outline" size="small" variant="text" color="primary" />
          <v-btn icon="mdi-refresh" size="small" variant="text" color="info" title="Gia hạn" />
          <v-btn icon="mdi-close-circle-outline" size="small" variant="text" color="error" title="Chấm dứt" />
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="dialog" max-width="600">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Tạo hợp đồng mới</h2>
        <v-row>
          <v-col cols="12"><v-select label="Sinh viên" :items="['SV001 - Nguyễn Văn An','SV003 - Lê Minh Cường']" /></v-col>
          <v-col cols="6"><v-select label="Phòng" :items="['101-A1','103-A1','202-A1']" /></v-col>
          <v-col cols="6"><v-text-field label="Giá thuê" suffix="₫/tháng" type="number" /></v-col>
          <v-col cols="6"><v-text-field label="Ngày bắt đầu" type="date" /></v-col>
          <v-col cols="6"><v-text-field label="Ngày kết thúc" type="date" /></v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog=false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog=false">Tạo hợp đồng</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
const dialog = ref(false)
const headers = [
  { title:'Mã HĐ', key:'code', align:'start' },
  { title:'Sinh viên', key:'student' },
  { title:'Phòng', key:'room', align:'center' },
  { title:'Giá thuê', key:'price', align:'end' },
  { title:'Bắt đầu', key:'start', align:'center' },
  { title:'Kết thúc', key:'end', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]
const contracts = ref([
  { id:1,code:'HD-001',student:'Nguyễn Văn An',room:'101-A1',price:'800.000₫',start:'01/01/2026',end:'31/12/2026',status:'Hiệu lực' },
  { id:2,code:'HD-002',student:'Trần Thị Bình',room:'102-A2',price:'1.500.000₫',start:'01/02/2026',end:'31/01/2027',status:'Hiệu lực' },
  { id:3,code:'HD-003',student:'Phạm Hoàng Duy',room:'201-B1',price:'800.000₫',start:'01/09/2025',end:'31/05/2026',status:'Sắp hết hạn' },
  { id:4,code:'HD-004',student:'Võ Thanh Phong',room:'301-C1',price:'2.500.000₫',start:'01/03/2026',end:'28/02/2027',status:'Hiệu lực' },
  { id:5,code:'HD-005',student:'Ngô Thị Giang',room:'103-A2',price:'1.500.000₫',start:'15/01/2025',end:'14/01/2026',status:'Đã chấm dứt' },
])
</script>
