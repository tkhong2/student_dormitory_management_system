<template>
  <div class="news-page">
    <section class="banner py-12 text-white">
      <v-container style="max-width:1280px">
        <h1 class="text-h3 font-weight-black mb-2">TIN TỨC & SỰ KIỆN</h1>
        <div class="d-flex align-center ga-2 text-caption font-weight-bold opacity-80 uppercase">
          <router-link to="/" class="text-white text-decoration-none hover-underline">Trang chủ</router-link>
          <v-icon size="14">mdi-chevron-right</v-icon>
          <span>Tin tức KTX</span>
        </div>
      </v-container>
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
  { id: 1, category: 'TIN TỨC KTX', title: 'Hướng dẫn quy trình đăng ký ở KTX cho tân sinh viên khóa mới', desc: 'Ban quản lý KTX Đại học Đại Nam hướng dẫn chi tiết các bước đăng ký, nộp hồ sơ và làm thủ tục nhận phòng cho sinh viên khóa mới nhập học năm 2026...', date: '15/05/2026', image: '/images/student_life.png' },
  { id: 2, category: 'SỰ KIỆN', title: 'Giải bóng đá Nam - Nữ KTX Đại Nam Cup 2026 chính thức khởi tranh', desc: 'Nhằm tạo sân chơi lành mạnh, tăng cường đoàn kết giữa các sinh viên nội trú, giải bóng đá truyền thống KTX sẽ diễn ra vào cuối tuần này...', date: '12/05/2026', image: '/images/football_match.png' },
  { id: 3, category: 'THÔNG BÁO', title: 'Lịch bảo trì hệ thống điện lưới và mạng wifi tại các tòa nhà khu A', desc: 'Để nâng cao chất lượng phục vụ, BQL phối hợp cùng bộ phận kỹ thuật tiến hành bảo trì hệ thống định kỳ...', date: '10/05/2026', image: '/images/hero_dormitory.png' },
]

const categories = ['Tất cả tin tức', 'Thông báo chung', 'Tin tức sự kiện', 'Hoạt động SV', 'Quy định - Thủ tục']
</script>

<style scoped>
.banner { background: linear-gradient(135deg, #1e3a8a, #0f172a); }
.news-item-card { transition: all .3s; }
.news-item-card:hover { transform: translateY(-4px); box-shadow: 0 10px 30px rgba(0,0,0,0.05) !important; border-color: #ff6b00 !important; }
.text-primary { color: #ff6b00 !important; }
.line-clamp-2 { display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
</style>
