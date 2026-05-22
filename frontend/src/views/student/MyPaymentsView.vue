<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Thanh toán</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Lịch sử và trạng thái thanh toán lệ phí KTX</p>

    <!-- Summary -->
    <v-row class="mb-6">
      <v-col cols="12" sm="4">
        <v-card class="pa-5 gradient-primary">
          <div class="text-body-2 mb-1" style="opacity:.7">Tổng đã thanh toán</div>
          <div class="text-h4 font-weight-bold">3.200.000₫</div>
          <div class="text-caption mt-1" style="opacity:.7">4 tháng</div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="pa-5" style="border:2px solid #f59e0b">
          <div class="text-body-2 text-medium-emphasis mb-1">Cần thanh toán</div>
          <div class="text-h4 font-weight-bold text-warning">800.000₫</div>
          <div class="text-caption text-medium-emphasis mt-1">Hạn: 20/05/2026</div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="4">
        <v-card class="pa-5 d-flex align-center justify-center" style="border:1px solid #e5e7eb">
          <v-btn color="primary" size="large" prepend-icon="mdi-credit-card-outline" class="px-8">Thanh toán ngay</v-btn>
        </v-card>
      </v-col>
    </v-row>

    <!-- Payment History -->
    <v-card style="border:1px solid #e5e7eb">
      <div class="pa-4">
        <h3 class="text-subtitle-1 font-weight-bold">Lịch sử thanh toán</h3>
      </div>
      <v-data-table :headers="headers" :items="payments" items-per-page="10">
        <template #item.amount="{ item }">
          <span class="font-weight-bold">{{ fmt(item.amount) }}</span>
        </template>
        <template #item.status="{ item }">
          <v-chip :color="item.status==='Đã TT'?'success':'warning'" size="x-small" variant="tonal">{{ item.status }}</v-chip>
        </template>
        <template #item.actions="{ item }">
          <v-btn v-if="item.status==='Đã TT'" icon="mdi-receipt-text-outline" size="small" variant="text" color="primary" title="Xem biên lai" />
          <v-btn v-else size="small" variant="tonal" color="primary">Thanh toán</v-btn>
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script setup>
const headers = [
  { title:'Mã HĐ', key:'code' },
  { title:'Tháng', key:'month', align:'center' },
  { title:'Số tiền', key:'amount', align:'end' },
  { title:'Hạn TT', key:'due', align:'center' },
  { title:'Ngày TT', key:'paidDate', align:'center' },
  { title:'Trạng thái', key:'status', align:'center' },
  { title:'', key:'actions', align:'end', sortable:false },
]
const payments = [
  { code:'INV-05-001',month:'Tháng 5/2026',amount:800000,due:'20/05/2026',paidDate:'—',status:'Chưa TT' },
  { code:'INV-04-001',month:'Tháng 4/2026',amount:800000,due:'20/04/2026',paidDate:'18/04/2026',status:'Đã TT' },
  { code:'INV-03-001',month:'Tháng 3/2026',amount:800000,due:'20/03/2026',paidDate:'15/03/2026',status:'Đã TT' },
  { code:'INV-02-001',month:'Tháng 2/2026',amount:800000,due:'20/02/2026',paidDate:'19/02/2026',status:'Đã TT' },
  { code:'INV-01-001',month:'Tháng 1/2026',amount:800000,due:'20/01/2026',paidDate:'10/01/2026',status:'Đã TT' },
]
const fmt = v => new Intl.NumberFormat('vi-VN',{style:'currency',currency:'VND'}).format(v)
</script>
