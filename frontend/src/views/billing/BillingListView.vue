<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Hóa đơn & Thanh toán</h1>
        <p class="text-body-2 text-medium-emphasis">Quản lý thu phí ký túc xá</p>
      </div>
      <div class="d-flex ga-3">
        <v-btn prepend-icon="mdi-calculator" variant="tonal" color="primary">Tính phí tháng</v-btn>
        <v-btn prepend-icon="mdi-download" variant="tonal">Xuất Excel</v-btn>
      </div>
    </div>

    <!-- Revenue Summary -->
    <v-row class="mb-6">
      <v-col cols="12" md="4">
        <v-card class="pa-5 gradient-primary">
          <div class="text-body-2 mb-1" style="opacity:.7">Tổng thu tháng này</div>
          <div class="text-h4 font-weight-bold">125.4M₫</div>
          <div class="text-caption mt-2" style="opacity:.7"><v-icon size="14">mdi-trending-up</v-icon> +12% so với tháng trước</div>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card class="pa-5" style="border:2px solid #f59e0b">
          <div class="text-body-2 text-medium-emphasis mb-1">Chưa thanh toán</div>
          <div class="text-h4 font-weight-bold text-warning">15.2M₫</div>
          <div class="text-caption text-medium-emphasis mt-2">12 hóa đơn đang chờ</div>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card class="pa-5" style="border:2px solid #dc2626">
          <div class="text-body-2 text-medium-emphasis mb-1">Quá hạn</div>
          <div class="text-h4 font-weight-bold text-error">2.5M₫</div>
          <div class="text-caption text-medium-emphasis mt-2">3 hóa đơn quá hạn</div>
        </v-card>
      </v-col>
    </v-row>

    <v-card style="border:1px solid #e5e7eb">
      <div class="pa-4 d-flex flex-wrap ga-3 align-center">
        <v-text-field v-model="search" placeholder="Tìm mã HĐ, sinh viên..." prepend-inner-icon="mdi-magnify" density="compact" hide-details style="max-width:300px" />
        <v-select label="Tháng" :items="months" density="compact" hide-details style="max-width:180px" />
        <v-chip-group class="ml-auto">
          <v-chip filter value="all">Tất cả</v-chip>
          <v-chip filter value="unpaid" color="warning">Chưa TT</v-chip>
          <v-chip filter value="overdue" color="error">Quá hạn</v-chip>
        </v-chip-group>
      </div>

      <v-data-table :headers="headers" :items="bills" :search="search" items-per-page="10">
        <template #item.amount="{ item }">
          <span class="font-weight-bold">{{ fmt(item.amount) }}</span>
        </template>
        <template #item.status="{ item }">
          <v-chip :color="sColor(item.status)" size="x-small" variant="tonal">{{ item.status }}</v-chip>
        </template>
        <template #item.actions="{ item }">
          <v-btn v-if="item.status!=='Đã TT'" size="small" color="success" variant="tonal" class="mr-1" prepend-icon="mdi-cash-check">Thu tiền</v-btn>
          <v-btn icon="mdi-printer" size="small" variant="text" />
          <v-btn icon="mdi-eye-outline" size="small" variant="text" color="primary" />
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
const search = ref('')
const months = ['Tháng 5/2026','Tháng 4/2026','Tháng 3/2026','Tháng 2/2026']
const headers = [
  { title:'Mã HĐ', key:'code' },
  { title:'Sinh viên', key:'student' },
  { title:'Phòng', key:'room', align:'center' },
  { title:'Số tiền', key:'amount', align:'end' },
  { title:'Hạn TT', key:'due', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]
const bills = ref([
  { id:1,code:'INV-05-001',student:'Nguyễn Văn An',room:'101-A1',amount:800000,due:'20/05/2026',status:'Chưa TT' },
  { id:2,code:'INV-05-002',student:'Trần Thị Bình',room:'102-A2',amount:1500000,due:'20/05/2026',status:'Đã TT' },
  { id:3,code:'INV-05-003',student:'Phạm Hoàng Duy',room:'201-B1',amount:800000,due:'20/05/2026',status:'Đã TT' },
  { id:4,code:'INV-04-004',student:'Lê Minh Cường',room:'103-A1',amount:800000,due:'20/04/2026',status:'Quá hạn' },
  { id:5,code:'INV-05-005',student:'Hoàng Thị Ê',room:'—',amount:1500000,due:'20/05/2026',status:'Chưa TT' },
  { id:6,code:'INV-05-006',student:'Võ Thanh Phong',room:'301-C1',amount:2500000,due:'20/05/2026',status:'Đã TT' },
  { id:7,code:'INV-04-007',student:'Ngô Thị Giang',room:'103-A2',amount:1500000,due:'20/04/2026',status:'Quá hạn' },
])

const fmt = v => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v)
const sColor = s => ({'Đã TT':'success','Chưa TT':'warning','Quá hạn':'error'}[s]||'grey')
</script>
