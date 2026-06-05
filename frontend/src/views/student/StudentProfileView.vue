<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Hồ sơ cá nhân</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Xem và cập nhật thông tin của bạn</p>

    <v-row>
      <!-- Avatar & basic -->
      <v-col cols="12" md="4">
        <v-card class="pa-6 text-center" style="border:1px solid #e5e7eb">
          <v-avatar size="100" class="mb-4 elevation-2">
            <v-img src="https://i.pravatar.cc/150?u=student1" />
          </v-avatar>
          <h3 class="text-h6 font-weight-bold mb-1">Nguyễn Văn An</h3>
          <div class="text-body-2 text-medium-emphasis mb-3">SV001 · K65-CNTT</div>
          <v-chip color="success" variant="tonal" class="mb-4">Đang ở KTX</v-chip>
          <v-divider class="my-4" />
          <div class="text-left">
            <div class="d-flex align-center ga-2 mb-3">
              <v-icon size="18" color="primary">mdi-email-outline</v-icon>
              <span class="text-body-2">vanan@gmail.com</span>
            </div>
            <div class="d-flex align-center ga-2 mb-3">
              <v-icon size="18" color="primary">mdi-phone-outline</v-icon>
              <span class="text-body-2">0901 234 567</span>
            </div>
            <div class="d-flex align-center ga-2">
              <v-icon size="18" color="primary">mdi-map-marker-outline</v-icon>
              <span class="text-body-2">Hà Nội, Việt Nam</span>
            </div>
          </div>
        </v-card>
      </v-col>

      <!-- Edit form -->
      <v-col cols="12" md="8">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-5">Chỉnh sửa thông tin</h3>
          <v-row>
            <v-col cols="6"><v-text-field label="Mã sinh viên" model-value="SV001" disabled /></v-col>
            <v-col cols="6"><v-text-field label="Họ và tên" model-value="Nguyễn Văn An" /></v-col>
            <v-col cols="6"><v-text-field label="Email" model-value="vanan@gmail.com" type="email" /></v-col>
            <v-col cols="6"><v-text-field label="Số điện thoại" model-value="0901234567" /></v-col>
            <v-col cols="6"><v-text-field label="Ngày sinh" model-value="2003-05-15" type="date" /></v-col>
            <v-col cols="6"><v-select label="Giới tính" :items="['Nam','Nữ']" model-value="Nam" /></v-col>
            <v-col cols="6"><v-text-field label="Lớp" model-value="K65-CNTT" disabled /></v-col>
            <v-col cols="6"><v-text-field label="Khoa" model-value="Công nghệ thông tin" disabled /></v-col>
            <v-col cols="12"><v-textarea label="Địa chỉ thường trú" model-value="Số 10, Ngõ 5, Phường X, Quận Y, Hà Nội" rows="2" /></v-col>
          </v-row>
          <div class="d-flex justify-end ga-3 mt-4">
            <v-btn variant="text">Hủy thay đổi</v-btn>
            <v-btn color="primary">Lưu thông tin</v-btn>
          </div>
        </v-card>

        <v-card class="pa-6 mt-4" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-5">Đổi mật khẩu</h3>
          <v-row>
            <v-col cols="12" sm="4"><v-text-field label="Mật khẩu hiện tại" type="password" /></v-col>
            <v-col cols="12" sm="4"><v-text-field label="Mật khẩu mới" type="password" /></v-col>
            <v-col cols="12" sm="4"><v-text-field label="Xác nhận mật khẩu" type="password" /></v-col>
          </v-row>
          <div class="d-flex justify-end mt-2">
            <v-btn color="warning" variant="tonal" style="color: white;">Đổi mật khẩu</v-btn>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
// Profile logic
</script>
