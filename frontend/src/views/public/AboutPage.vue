<template>
  <div class="about-page">
    <section class="banner py-12 text-white">
      <v-container style="max-width:1280px">
        <h1 class="text-h3 font-weight-black mb-2">GIỚI THIỆU</h1>
        <div class="d-flex align-center ga-2 text-caption font-weight-bold opacity-80 uppercase">
          <router-link to="/" class="text-white text-decoration-none hover-underline">Trang chủ</router-link>
          <v-icon size="14">mdi-chevron-right</v-icon>
          <span>Giới thiệu KTX</span>
        </div>
      </v-container>
    </section>

    <v-container class="py-16" style="max-width:1280px">
      <v-row>
        <v-col cols="12" lg="8">
          <div class="content-card pa-10 rounded-xl bg-white border">
            <h2 class="text-h4 font-weight-black text-primary mb-6">TỔNG QUAN VỀ KÝ TÚC XÁ DNU</h2>
            <div class="text-body-1 text-slate-700 leading-relaxed mb-8">
              <p class="mb-6">Ký túc xá Trường Đại học Đại Nam là một trong những cơ sở lưu trú hiện đại bậc nhất dành cho sinh viên tại Hà Nội. Với mục tiêu xây dựng một môi trường sống "An toàn - Văn minh - Hiện đại", Ban quản lý KTX luôn nỗ lực mang đến những tiện ích tốt nhất cho sinh viên nội trú.</p>
              <p class="mb-6">Tọa lạc trong khuôn viên trường, KTX gồm các tòa nhà cao tầng với sức chứa hàng nghìn sinh viên. Các phòng được thiết kế thoáng mát, đầy đủ tiện nghi thiết yếu phục vụ đời sống sinh hoạt và học tập.</p>
            </div>

            <v-row class="mb-8">
              <v-col cols="12" md="6">
                <v-img src="/images/hero_dormitory.png" class="rounded-xl shadow-lg" cover height="250" />
              </v-col>
              <v-col cols="12" md="6">
                <v-img src="/images/student_life.png" class="rounded-xl shadow-lg" cover height="250" />
              </v-col>
            </v-row>

            <h3 class="text-h5 font-weight-bold mb-4">Mục tiêu và Tầm nhìn</h3>
            <ul class="d-flex flex-column ga-4 mb-8">
              <li class="d-flex align-start ga-3">
                <v-icon color="primary" class="mt-1">mdi-check-circle</v-icon>
                <span>Đảm bảo an ninh trật tự tuyệt đối 24/7.</span>
              </li>
              <li class="d-flex align-start ga-3">
                <v-icon color="primary" class="mt-1">mdi-check-circle</v-icon>
                <span>Hỗ trợ tối đa cho việc học tập và rèn luyện của sinh viên.</span>
              </li>
              <li class="d-flex align-start ga-3">
                <v-icon color="primary" class="mt-1">mdi-check-circle</v-icon>
                <span>Xây dựng nếp sống văn hóa, kỷ cương trong môi trường nội trú.</span>
              </li>
            </ul>
          </div>
        </v-col>

        <v-col cols="12" lg="4">
          <v-card flat border class="pa-8 rounded-xl sticky-top">
            <h3 class="text-subtitle-1 font-weight-black mb-6 text-uppercase tracking-widest border-b pb-4">Ban quản lý KTX</h3>
            <div class="d-flex flex-column ga-6">
              <div class="d-flex align-center ga-4">
                <v-avatar color="primary" size="48" class="rounded-lg">
                  <v-icon color="white">mdi-phone</v-icon>
                </v-avatar>
                <div>
                  <div class="text-caption font-weight-bold opacity-60">Hotline hỗ trợ</div>
                  <div class="text-body-1 font-weight-black">0243.869.1484</div>
                </div>
              </div>
              <div class="d-flex align-center ga-4">
                <v-avatar color="info" size="48" class="rounded-lg">
                  <v-icon color="white">mdi-email</v-icon>
                </v-avatar>
                <div>
                  <div class="text-caption font-weight-bold opacity-60">Email liên hệ</div>
                  <div class="text-body-1 font-weight-black">ktx@dainam.edu.vn</div>
                </div>
              </div>
            </div>
            <v-divider class="my-8" />
            <v-btn block color="primary" class="font-weight-bold rounded-lg" height="48">Liên hệ trực tiếp</v-btn>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
</script>

<style scoped>
.banner { background: linear-gradient(135deg, #1e3a8a, #0f172a); }
.content-card { box-shadow: 0 4px 20px rgba(0,0,0,0.03) !important; }
.sticky-top { position: sticky; top: 100px; }
.text-primary { color: #ff6b00 !important; }
.hover-underline:hover { text-decoration: underline; }
</style>
