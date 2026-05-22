<template>
  <div>
    <h1 class="text-h4 font-weight-bold mb-1">Yêu cầu Sửa chữa</h1>
    <p class="text-body-2 text-medium-emphasis mb-6">Gửi yêu cầu khi phòng có sự cố cần xử lý</p>

    <v-row>
      <!-- Form gửi yêu cầu -->
      <v-col cols="12" md="5">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Gửi yêu cầu mới</h3>
          <v-text-field label="Tiêu đề" placeholder="VD: Hỏng vòi nước" class="mb-1" />
          <v-select label="Danh mục" :items="['Điện','Nước','Cửa/Khóa','Nội thất','Điều hòa','Khác']" class="mb-1" />
          <v-select label="Mức độ" :items="['Bình thường','Khẩn cấp']" class="mb-1" />
          <v-textarea label="Mô tả chi tiết" rows="4" placeholder="Mô tả sự cố để nhân viên xử lý nhanh hơn..." class="mb-3" />
          <v-btn color="primary" block prepend-icon="mdi-send">Gửi yêu cầu</v-btn>
        </v-card>
      </v-col>

      <!-- Lịch sử -->
      <v-col cols="12" md="7">
        <v-card class="pa-6" style="border:1px solid #e5e7eb">
          <h3 class="text-subtitle-1 font-weight-bold mb-4">Yêu cầu đã gửi</h3>
          <div v-for="r in myRequests" :key="r.id" class="mb-4">
            <v-card variant="outlined" class="pa-4">
              <div class="d-flex align-start justify-space-between mb-2">
                <div>
                  <div class="text-body-1 font-weight-bold">{{ r.title }}</div>
                  <div class="text-caption text-medium-emphasis">{{ r.date }} · Phòng {{ r.room }}</div>
                </div>
                <v-chip :color="stColor(r.status)" size="x-small" variant="tonal">{{ r.status }}</v-chip>
              </div>
              <p class="text-body-2 text-medium-emphasis mb-3">{{ r.desc }}</p>
              <div v-if="r.reply" class="pa-3 rounded-lg" style="background:#f8f9fb">
                <div class="text-caption font-weight-bold mb-1"><v-icon size="14" class="mr-1">mdi-reply</v-icon>Phản hồi từ Ban quản lý:</div>
                <div class="text-body-2">{{ r.reply }}</div>
              </div>
            </v-card>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup>
const myRequests = [
  { id:1, title:'Hỏng vòi nước', room:'101-A1', date:'15/05/2026', status:'Đang xử lý', desc:'Vòi nước nhà vệ sinh bị rò rỉ mạnh', reply:'Kỹ thuật sẽ đến sửa lúc 14:00 ngày 16/05.' },
  { id:2, title:'Bóng đèn hành lang cháy', room:'101-A1', date:'01/05/2026', status:'Hoàn thành', desc:'Bóng đèn LED hành lang tầng 1 bị cháy', reply:'Đã thay bóng mới ngày 02/05.' },
]
const stColor = s => ({'Chờ xử lý':'warning','Đang xử lý':'info','Hoàn thành':'success'}[s]||'grey')
</script>
