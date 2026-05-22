<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Phòng của tôi</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Thông tin chi tiết về phòng bạn đang ở</p>

    <v-row>
      <v-col cols="12" md="8">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-4 mb-6">
            <v-avatar color="#ede9fe" size="56" rounded="lg"><v-icon color="primary" size="28">mdi-door-closed</v-icon></v-avatar>
            <div>
              <div class="text-h5 font-weight-bold">Phòng 101 — Tòa A1</div>
              <div class="text-body-2 text-medium-emphasis">Tầng 1 · Phòng Tiêu chuẩn (4 giường)</div>
            </div>
            <v-spacer />
            <v-chip color="success" variant="tonal" size="small">Đang hoạt động</v-chip>
          </div>

          <v-divider class="mb-5" />

          <v-row>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Loại phòng</div>
              <div class="text-body-2 font-weight-bold">Tiêu chuẩn</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Sức chứa</div>
              <div class="text-body-2 font-weight-bold">3/4 sinh viên</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Giá thuê</div>
              <div class="text-body-2 font-weight-bold text-primary">800.000₫/tháng</div>
            </v-col>
            <v-col cols="6" sm="3">
              <div class="text-caption text-medium-emphasis mb-1">Diện tích</div>
              <div class="text-body-2 font-weight-bold">24m²</div>
            </v-col>
          </v-row>

          <v-divider class="my-5" />

          <h4 class="text-subtitle-2 font-weight-bold mb-3">Tiện nghi phòng</h4>
          <div class="d-flex flex-wrap ga-2">
            <v-chip v-for="a in amenities" :key="a" size="small" variant="tonal" :prepend-icon="a.icon" color="primary">{{ a.label }}</v-chip>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" md="4">
        <v-card class="pa-5 mb-4" style="border:1px solid #e5e7eb">
          <h4 class="text-subtitle-2 font-weight-bold mb-4">Bạn cùng phòng (3/4)</h4>
          <div v-for="m in mates" :key="m.name" class="d-flex align-center ga-3 mb-3">
            <v-avatar size="36" :color="m.you?'secondary':'primary'" variant="tonal"><v-icon size="16">mdi-account</v-icon></v-avatar>
            <div>
              <div class="text-body-2 font-weight-medium">{{ m.name }} {{ m.you ? '(Bạn)' : '' }}</div>
              <div class="text-caption text-medium-emphasis">{{ m.info }}</div>
            </div>
          </div>
        </v-card>

        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <h4 class="text-subtitle-2 font-weight-bold mb-3">Nội quy phòng</h4>
          <v-list density="compact">
            <v-list-item v-for="r in rules" :key="r" :title="r" prepend-icon="mdi-check-circle-outline" class="text-body-2" />
          </v-list>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
const amenities = [
  { label: 'Quạt trần', icon: 'mdi-fan' },
  { label: 'Giường tầng', icon: 'mdi-bunk-bed' },
  { label: 'Bàn học', icon: 'mdi-desk' },
  { label: 'Tủ cá nhân', icon: 'mdi-locker' },
  { label: 'WiFi', icon: 'mdi-wifi' },
  { label: 'WC chung tầng', icon: 'mdi-toilet' },
]
const mates = [
  { name: 'Nguyễn Văn An', info: 'SV001 · K65-CNTT', you: true },
  { name: 'Phạm Hoàng Duy', info: 'SV004 · K64-ĐT', you: false },
  { name: 'Trần Minh Quân', info: 'SV012 · K65-CNTT', you: false },
]
const rules = [
  'Giữ vệ sinh phòng sạch sẽ',
  'Không gây ồn ào sau 22:00',
  'Không sử dụng thiết bị điện công suất lớn',
  'Tiết kiệm điện nước',
  'Báo cáo khi có hư hỏng',
]
</script>
