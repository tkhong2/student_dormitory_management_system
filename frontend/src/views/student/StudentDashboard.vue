<template>
  <div>
    <!-- Welcome -->
    <v-card class="pa-6 mb-6 gradient-secondary rounded-xl">
      <div class="d-flex align-center justify-space-between flex-wrap ga-4">
        <div>
          <h1 class="text-h4 font-weight-bold mb-2">Xin chào, Nguyễn Văn An! 👋</h1>
          <p style="opacity:.85;max-width:500px">Chào mừng bạn đến với Cổng thông tin Sinh viên KTX. Tại đây bạn có thể theo dõi phòng ở, hợp đồng, thanh toán và gửi yêu cầu hỗ trợ.</p>
        </div>
        <v-avatar size="80" class="elevation-4">
          <v-img src="https://i.pravatar.cc/150?u=student1" />
        </v-avatar>
      </div>
    </v-card>

    <!-- Quick Info Cards -->
    <v-row class="mb-6">
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#ede9fe" size="42" rounded="lg"><v-icon color="primary" size="22">mdi-door-closed</v-icon></v-avatar>
            <div class="text-caption text-medium-emphasis">Phòng của tôi</div>
          </div>
          <div class="text-h5 font-weight-bold">101 - A1</div>
          <div class="text-caption text-medium-emphasis">Tầng 1 · Tòa A1</div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#dcfce7" size="42" rounded="lg"><v-icon color="success" size="22">mdi-file-check</v-icon></v-avatar>
            <div class="text-caption text-medium-emphasis">Hợp đồng</div>
          </div>
          <div class="text-h5 font-weight-bold text-success">Hiệu lực</div>
          <div class="text-caption text-medium-emphasis">Hết hạn: 31/12/2026</div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#fff7ed" size="42" rounded="lg"><v-icon color="warning" size="22">mdi-cash-clock</v-icon></v-avatar>
            <div class="text-caption text-medium-emphasis">Thanh toán</div>
          </div>
          <div class="text-h5 font-weight-bold text-warning">800.000₫</div>
          <div class="text-caption text-medium-emphasis">Hạn: 20/05/2026</div>
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card class="pa-5" style="border:1px solid #e5e7eb">
          <div class="d-flex align-center ga-3 mb-3">
            <v-avatar color="#e0f2fe" size="42" rounded="lg"><v-icon color="info" size="22">mdi-wrench</v-icon></v-avatar>
            <div class="text-caption text-medium-emphasis">Yêu cầu sửa chữa</div>
          </div>
          <div class="text-h5 font-weight-bold">1</div>
          <div class="text-caption text-medium-emphasis">Đang xử lý</div>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <!-- Roommates -->
      <v-col cols="12" md="6">
        <v-card class="pa-5 h-100" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Bạn cùng phòng</h3>
          <div v-for="mate in roommates" :key="mate.name" class="d-flex align-center ga-3 mb-4">
            <v-avatar size="40" color="primary" variant="tonal">
              <v-icon size="20">mdi-account</v-icon>
            </v-avatar>
            <div class="flex-grow-1">
              <div class="text-body-2 font-weight-bold">{{ mate.name }}</div>
              <div class="text-caption text-medium-emphasis">{{ mate.info }}</div>
            </div>
            <v-chip size="x-small" variant="tonal" color="success">Đang ở</v-chip>
          </div>
          <div class="text-center py-2">
            <v-chip variant="outlined" size="small" color="info">Còn 1 giường trống</v-chip>
          </div>
        </v-card>
      </v-col>

      <!-- Announcements -->
      <v-col cols="12" md="6">
        <v-card class="pa-5 h-100" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Thông báo mới</h3>
          <div v-for="a in announcements" :key="a.id" class="d-flex ga-3 mb-4">
            <v-avatar :color="a.color" size="32" variant="tonal"><v-icon size="16">{{ a.icon }}</v-icon></v-avatar>
            <div style="min-width:0">
              <div class="text-body-2 font-weight-medium">{{ a.title }}</div>
              <div class="text-caption text-medium-emphasis">{{ a.time }}</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
const roommates = [
  { name: 'Phạm Hoàng Duy', info: 'SV004 · K64-ĐT' },
  { name: 'Trần Minh Quân', info: 'SV012 · K65-CNTT' },
]

const announcements = [
  { id:1, title: 'Nhắc nộp lệ phí tháng 5/2026 — hạn 20/05', time: '2 giờ trước', icon: 'mdi-cash-clock', color: 'warning' },
  { id:2, title: 'Lịch kiểm tra phòng ở định kỳ: 18/05/2026', time: 'Hôm qua', icon: 'mdi-calendar-check', color: 'info' },
  { id:3, title: 'Cập nhật nội quy KTX học kỳ II', time: '3 ngày trước', icon: 'mdi-book-open', color: 'primary' },
  { id:4, title: 'Sự cố điện tòa B2 đã được khắc phục', time: '5 ngày trước', icon: 'mdi-flash', color: 'success' },
]
</script>
