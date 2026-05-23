<template>
  <div class="contact-page">
    <section class="banner">
      <div class="banner-overlay">
        <v-container style="max-width:1280px">
          <div class="banner-content">
            <div class="banner-badge mb-4">
              <v-icon size="18" class="mr-2">mdi-phone-in-talk</v-icon>
              <span>HỖ TRỢ & LIÊN HỆ</span>
            </div>
            <h1 class="banner-title mb-4">LIÊN HỆ VỚI CHÚNG TÔI</h1>
            <p class="banner-subtitle mb-6">Chúng tôi luôn sẵn sàng lắng nghe và hỗ trợ bạn 24/7</p>
            <div class="d-flex align-center ga-2 banner-breadcrumb">
              <router-link to="/" class="text-white text-decoration-none hover-underline">
                <v-icon size="16" class="mr-1">mdi-home</v-icon>
                Trang chủ
              </router-link>
              <v-icon size="16">mdi-chevron-right</v-icon>
              <span>Liên hệ</span>
            </div>
          </div>
        </v-container>
      </div>
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
                <p class="text-body-2 opacity-70">Xốm, Phú Lương, Hà Nội</p>
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
      <iframe 
        src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3723.473795934859!2d105.97252607533498!3d21.053824893123456!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135abc4ff1eab73%3A0x2be9d7d0b5931e20!2zVHLGsOG7nW5nIMSQ4bqhaSBo4buNYyDEkOG6oWkgTmFt!5e0!3m2!1svi!2s!4v1716234567890!5m2!1svi!2s&markers=color:red%7Clabel:D%7C21.053825,105.974726&zoom=15"
        width="100%" 
        height="450" 
        style="border:0;" 
        allowfullscreen="" 
        loading="lazy" 
        referrerpolicy="no-referrer-when-downgrade"
      ></iframe>
    </section>
  </div>
</template>

<script setup>
</script>

<style scoped>
.banner { 
  background: linear-gradient(135deg, rgba(255, 107, 0, 0.95), rgba(255, 136, 0, 0.9)), url('/images/hero_dormitory.png');
  background-size: cover;
  background-position: center;
  position: relative;
  overflow: hidden;
}
.banner::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 30% 50%, rgba(255,255,255,0.15) 0%, transparent 60%);
  pointer-events: none;
}
.banner-overlay {
  position: relative;
  z-index: 1;
  padding: 100px 0 80px;
  color: white;
}
.banner-content {
  max-width: 700px;
}
.banner-badge {
  display: inline-flex;
  align-items: center;
  background: rgba(255,255,255,0.2);
  backdrop-filter: blur(10px);
  padding: 8px 20px;
  border-radius: 50px;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 1.5px;
  border: 1px solid rgba(255,255,255,0.3);
}
.banner-title {
  font-size: 3.5rem;
  font-weight: 900;
  line-height: 1.1;
  text-shadow: 0 4px 20px rgba(0,0,0,0.3);
  letter-spacing: -1px;
}
.banner-subtitle {
  font-size: 1.2rem;
  font-weight: 500;
  opacity: 0.95;
  line-height: 1.6;
  text-shadow: 0 2px 10px rgba(0,0,0,0.2);
}
.banner-breadcrumb {
  font-size: 14px;
  font-weight: 600;
  opacity: 0.9;
}
.section-title { font-size: 1.8rem; font-weight: 900; color: #ff6b00; position: relative; padding-left: 20px; }
.section-title::before { content: ''; position: absolute; left: 0; top: 0; bottom: 0; width: 6px; background: #ff6b00; border-radius: 3px; }
.bg-slate-50 { background-color: #f8fafc; }
.hover-underline:hover { text-decoration: underline; }
</style>
