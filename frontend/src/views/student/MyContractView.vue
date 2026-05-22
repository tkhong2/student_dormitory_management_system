<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Hợp đồng của tôi</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Thông tin hợp đồng thuê phòng KTX</p>

    <v-card class="pa-6" style="border:1px solid #e5e7eb">
      <div class="d-flex align-center ga-4 mb-6">
        <v-avatar color="#dcfce7" size="52" rounded="lg"><v-icon color="success" size="26">mdi-file-sign</v-icon></v-avatar>
        <div>
          <div class="text-h6 font-weight-bold">Hợp đồng #HD-001</div>
          <v-chip color="success" size="small" variant="tonal" class="mt-1">Đang hiệu lực</v-chip>
        </div>
      </div>

      <v-divider class="mb-5" />

      <v-row>
        <v-col cols="12" sm="6" md="4" v-for="f in fields" :key="f.label">
          <div class="text-caption text-medium-emphasis mb-1">{{ f.label }}</div>
          <div class="text-body-1 font-weight-bold">{{ f.value }}</div>
        </v-col>
      </v-row>

      <v-divider class="my-5" />

      <h4 class="text-subtitle-2 font-weight-bold mb-3">Điều khoản quan trọng</h4>
      <v-list density="compact">
        <v-list-item title="Thanh toán tiền phòng trước ngày 20 hàng tháng" prepend-icon="mdi-numeric-1-circle" />
        <v-list-item title="Giữ gìn tài sản chung, bồi thường nếu hư hỏng" prepend-icon="mdi-numeric-2-circle" />
        <v-list-item title="Chấm dứt hợp đồng phải báo trước 30 ngày" prepend-icon="mdi-numeric-3-circle" />
        <v-list-item title="Tuân thủ nội quy KTX do Ban Quản lý ban hành" prepend-icon="mdi-numeric-4-circle" />
      </v-list>

      <div class="d-flex ga-3 mt-6">
        <v-btn prepend-icon="mdi-download" variant="tonal" color="primary">Tải hợp đồng PDF</v-btn>
        <v-btn prepend-icon="mdi-refresh" variant="tonal">Yêu cầu gia hạn</v-btn>
      </div>
    </v-card>
  </div>
</template>

<script setup>
const fields = [
  { label: 'Sinh viên', value: 'Nguyễn Văn An — SV001' },
  { label: 'Phòng', value: '101 — Tòa A1' },
  { label: 'Loại phòng', value: 'Phòng Tiêu chuẩn (4 SV)' },
  { label: 'Giá thuê', value: '800.000₫/tháng' },
  { label: 'Ngày bắt đầu', value: '01/01/2026' },
  { label: 'Ngày kết thúc', value: '31/12/2026' },
]
</script>
