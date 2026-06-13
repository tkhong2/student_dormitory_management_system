<template>
  <div class="news-page">
    <section class="banner">
      <div class="banner-overlay">
        <v-container style="max-width:1280px">
          <div class="banner-content">
            <div class="banner-badge mb-4">
              <v-icon size="18" class="mr-2">mdi-newspaper-variant-outline</v-icon>
              <span>TIN TỨC MỚI NHẤT</span>
            </div>
            <h1 class="banner-title mb-4">TIN TỨC & SỰ KIỆN</h1>
            <p class="banner-subtitle mb-6">Cập nhật thông tin mới nhất về hoạt động và sự kiện tại Ký túc xá</p>
            <div class="d-flex align-center ga-2 banner-breadcrumb">
              <router-link to="/" class="text-white text-decoration-none hover-underline">
                <v-icon size="16" class="mr-1">mdi-home</v-icon>
                Trang chủ
              </router-link>
              <v-icon size="16">mdi-chevron-right</v-icon>
              <span>Tin tức</span>
            </div>
          </div>
        </v-container>
      </div>
    </section>

    <v-container class="py-16" style="max-width:1280px">
      <v-row>
        <v-col cols="12" lg="8">
          <v-row>
            <v-col v-for="n in news" :key="n.id" cols="12">
              <v-card flat border class="news-item-card rounded-xl overflow-hidden mb-6">
                <v-row no-gutters>
                  <v-col cols="12" sm="4">
                    <v-img :src="n.image" height="100%" cover class="h-100" />
                  </v-col>
                  <v-col cols="12" sm="8">
                    <v-card-text class="pa-6">
                      <div class="text-caption text-primary font-weight-bold mb-2 d-flex align-center justify-space-between">
                        <span>{{ n.category }}</span>
                        <span class="text-medium-emphasis"><v-icon size="14">mdi-calendar</v-icon> {{ n.date }}</span>
                      </div>
                      <h3 class="text-h6 font-weight-bold mb-3">{{ n.title }}</h3>
                      <p class="text-body-2 text-medium-emphasis mb-6 line-clamp-2">{{ n.desc }}</p>
                      <v-btn variant="text" color="primary" class="pa-0 font-weight-bold" append-icon="mdi-arrow-right">Đọc tiếp</v-btn>
                    </v-card-text>
                  </v-col>
                </v-row>
              </v-card>
            </v-col>
          </v-row>

          <div class="d-flex justify-center mt-12">
            <v-pagination :length="5" total-visible="5" color="primary" rounded="lg" />
          </div>
        </v-col>

        <v-col cols="12" lg="4">
          <v-card flat border class="pa-8 rounded-xl mb-8">
            <h3 class="text-subtitle-1 font-weight-black mb-6 border-b pb-4">Danh mục</h3>
            <v-list density="comfortable">
              <v-list-item v-for="cat in categories" :key="cat" :title="cat" rounded="lg" class="mb-1" link />
            </v-list>
          </v-card>

          <v-card flat border class="pa-8 rounded-xl bg-primary text-white">
            <v-icon size="48" class="mb-4">mdi-email-newsletter</v-icon>
            <h3 class="text-h6 font-weight-bold mb-2">Đăng ký nhận tin</h3>
            <p class="text-body-2 mb-6 opacity-80">Nhận những thông báo mới nhất từ Ban quản lý KTX.</p>
            <v-text-field placeholder="Email của bạn" density="compact" variant="solo" rounded="lg" hide-details class="mb-4" />
            <v-btn block color="white" class="text-primary font-weight-bold rounded-lg">Đăng ký</v-btn>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
const news = [
  { id: 1, category: 'TIN TỨC KTX', title: 'Hướng dẫn quy trình đăng ký ở KTX cho tân sinh viên khóa mới', desc: 'Ban quản lý KTX Đại học Đại Nam hướng dẫn chi tiết các bước đăng ký, nộp hồ sơ và làm thủ tục nhận phòng cho sinh viên khóa mới nhập học năm 2026...', date: '15/05/2026', image: 'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=800&q=80' },
 
  { id: 2, category: 'THÔNG BÁO', title: 'Lịch bảo trì hệ thống điện lưới và mạng wifi tại các tòa nhà khu A', desc: 'Để nâng cao chất lượng phục vụ, BQL phối hợp cùng bộ phận kỹ thuật tiến hành bảo trì hệ thống định kỳ...', date: '10/05/2026', image: 'https://images.unsplash.com/photo-1581094794329-c8112a89af12?w=800&q=80' },
  { id: 3, category: 'HOẠT ĐỘNG SV', title: 'Chương trình "Sinh nhật tập thể" tháng 5 cho sinh viên nội trú', desc: 'Chương trình được tổ chức hàng tháng, tạo không khí vui vẻ, gắn kết giữa các sinh viên có sinh nhật trong tháng...', date: '08/05/2026', image: 'https://images.unsplash.com/photo-1530103862676-de8c9debad1d?w=800&q=80' },
  { id: 4, category: 'TIN TỨC KTX', title: 'KTX trang bị thêm khu vực học tập chung hiện đại tại tòa B', desc: 'Đáp ứng nhu cầu học tập của sinh viên, KTX đầu tư nâng cấp khu học tập với trang thiết bị hiện đại, không gian yên tĩnh...', date: '05/05/2026', image: 'https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=800&q=80' },
  { id: 5, category: 'THÔNG BÁO', title: 'Triển khai thanh toán phí KTX qua VNPay và Momo', desc: 'Từ tháng 6/2026, sinh viên có thể thanh toán phí KTX qua ví điện tử VNPay và Momo, thuận tiện và nhanh chóng hơn...', date: '01/05/2026', image: 'https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=800&q=80' },
]

const categories = ['Tất cả tin tức', 'Thông báo chung', 'Tin tức sự kiện', 'Hoạt động SV', 'Quy định - Thủ tục']
</script>

<style scoped>
.banner { 
  background: linear-gradient(135deg, rgba(255, 107, 0, 0.95), rgba(255, 136, 0, 0.9)), url('/images/student_life.png');
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
.news-item-card { transition: all .3s; }
.news-item-card:hover { transform: translateY(-4px); box-shadow: 0 10px 30px rgba(0,0,0,0.05) !important; border-color: #ff6b00 !important; }
.text-primary { color: #ff6b00 !important; }
.line-clamp-2 { display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
</style>
