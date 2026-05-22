<template>
  <div class="contact-page">
    <section class="banner py-12 text-white">
      <v-container style="max-width:1280px">
        <h1 class="text-h3 font-weight-black mb-2">LIÊN HỆ</h1>
        <div class="d-flex align-center ga-2 text-caption font-weight-bold opacity-80 uppercase">
          <router-link to="/" class="text-white text-decoration-none hover-underline">Trang chủ</router-link>
          <v-icon size="14">mdi-chevron-right</v-icon>
          <span>Liên hệ BQL</span>
        </div>
      </v-container>
    </section>

    <v-container class="py-16" style="max-width:1280px">
      <v-row>
        <v-col cols="12" md="5">
          <h2 class="section-title mb-8">THÔNG TIN LIÊN HỆ</h2>
          <div class="d-flex flex-column ga-8">
            <div class="contact-info-item d-flex ga-6">
              <v-avatar color="primary" size="56" class="rounded-xl elevation-2">
                <v-icon color="white" size="24">mdi-map-marker</v-icon>
              </v-avatar>
              <div>
                <h4 class="text-subtitle-1 font-weight-black mb-1">Địa chỉ</h4>
                <p class="text-body-2 opacity-70">Số 56 Vũ Trọng Phụng, Thanh Xuân, Hà Nội</p>
              </div>
            </div>

            <div class="contact-info-item d-flex ga-6">
              <v-avatar color="info" size="56" class="rounded-xl elevation-2">
                <v-icon color="white" size="24">mdi-phone</v-icon>
              </v-avatar>
              <div>
                <h4 class="text-subtitle-1 font-weight-black mb-1">Điện thoại</h4>
                <p class="text-body-2 opacity-70">0243.869.1484</p>
              </div>
            </div>

            <div class="contact-info-item d-flex ga-6">
              <v-avatar color="success" size="56" class="rounded-xl elevation-2">
                <v-icon color="white" size="24">mdi-email</v-icon>
              </v-avatar>
              <div>
                <h4 class="text-subtitle-1 font-weight-black mb-1">Email</h4>
                <p class="text-body-2 opacity-70">ktx@dainam.edu.vn</p>
              </div>
            </div>
          </div>

          <v-card flat border class="pa-8 rounded-xl mt-12 bg-slate-50">
            <h4 class="text-subtitle-1 font-weight-black mb-4">Mạng xã hội</h4>
            <div class="d-flex ga-4">
              <v-btn icon="mdi-facebook" color="primary" variant="flat" class="rounded-lg" />
              <v-btn icon="mdi-youtube" color="error" variant="flat" class="rounded-lg" />
              <v-btn icon="mdi-web" color="info" variant="flat" class="rounded-lg" />
            </div>
          </v-card>
        </v-col>

        <v-col cols="12" md="7">
          <v-card flat border class="pa-10 rounded-xl">
            <h3 class="text-h5 font-weight-black mb-2">Gửi ý kiến phản hồi</h3>
            <p class="text-body-2 text-medium-emphasis mb-8">Chúng tôi luôn sẵn sàng lắng nghe ý kiến đóng góp của bạn.</p>
            
            <v-form>
              <v-row>
                <v-col cols="12" sm="6">
                  <v-text-field label="Họ và tên" variant="outlined" density="comfortable" color="primary" rounded="lg" />
                </v-col>
                <v-col cols="12" sm="6">
                  <v-text-field label="Mã sinh viên (nếu có)" variant="outlined" density="comfortable" color="primary" rounded="lg" />
                </v-col>
                <v-col cols="12">
                  <v-text-field label="Email liên hệ" variant="outlined" density="comfortable" color="primary" rounded="lg" />
                </v-col>
                <v-col cols="12">
                  <v-textarea label="Nội dung tin nhắn" variant="outlined" color="primary" rounded="lg" rows="5" />
                </v-col>
                <v-col cols="12">
                  <v-btn block color="primary" size="large" class="font-weight-bold rounded-lg" height="56">GỬI YÊU CẦU</v-btn>
                </v-col>
              </v-row>
            </v-form>
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <section class="map-section py-0">
      <div class="map-placeholder">
        <div class="text-center pa-16 bg-grey-lighten-4">
          <v-icon size="64" color="grey-lighten-2">mdi-map-search</v-icon>
          <div class="mt-4 text-subtitle-1 font-weight-bold opacity-40 uppercase tracking-widest">Bản đồ vị trí KTX Đại học Đại Nam</div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
</script>

<style scoped>
.banner { background: linear-gradient(135deg, #1e3a8a, #0f172a); }
.section-title { font-size: 1.8rem; font-weight: 900; color: #ff6b00; position: relative; padding-left: 20px; }
.section-title::before { content: ''; position: absolute; left: 0; top: 0; bottom: 0; width: 6px; background: #ff6b00; border-radius: 3px; }
.bg-slate-50 { background-color: #f8fafc; }
.map-placeholder { min-height: 400px; display: flex; align-items: center; justify-content: center; background: #f1f5f9; }
</style>
