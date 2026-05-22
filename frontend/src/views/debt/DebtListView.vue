<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Công nợ sinh viên</h1>
        <p class="text-body-2 text-medium-emphasis">Theo dõi các khoản nợ chưa thanh toán</p>
      </div>
      <v-btn prepend-icon="mdi-bell-ring" variant="tonal" color="warning">Nhắc nợ hàng loạt</v-btn>
    </div>

    <v-card style="border:1px solid #e5e7eb">
      <v-data-table :headers="headers" :items="debts" items-per-page="10">
        <template #item.student="{ item }">
          <div class="d-flex align-center ga-3 py-2">
            <v-avatar size="32" color="error" variant="tonal"><v-icon size="16">mdi-account</v-icon></v-avatar>
            <div>
              <div class="text-body-2 font-weight-medium">{{ item.student }}</div>
              <div class="text-caption text-medium-emphasis">{{ item.code }}</div>
            </div>
          </div>
        </template>
        <template #item.total="{ item }">
          <span class="font-weight-bold text-error">{{ fmt(item.total) }}</span>
        </template>
        <template #item.months="{ item }">
          <v-chip color="error" size="x-small" variant="tonal">{{ item.months }} tháng</v-chip>
        </template>
        <template #item.actions="{ item }">
          <v-btn size="small" variant="tonal" color="warning" prepend-icon="mdi-bell" class="mr-1">Nhắc</v-btn>
          <v-btn size="small" variant="tonal" color="success" prepend-icon="mdi-cash">Thu</v-btn>
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script setup>
const headers = [
  { title:'Sinh viên', key:'student' },
  { title:'Phòng', key:'room', align:'center' },
  { title:'Tổng nợ', key:'total', align:'end' },
  { title:'Số tháng nợ', key:'months', align:'center' },
  { title:'Hạn cuối', key:'deadline', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]
const debts = [
  { student:'Lê Minh Cường',code:'SV003',room:'103-A1',total:1600000,months:2,deadline:'20/04/2026' },
  { student:'Ngô Thị Giang',code:'SV007',room:'103-A2',total:1500000,months:1,deadline:'20/04/2026' },
  { student:'Đặng Văn Hải',code:'SV008',room:'—',total:800000,months:1,deadline:'20/05/2026' },
]
const fmt = v => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v)
</script>
