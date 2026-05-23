<template>
  <div class="rules-page">
    <section class="banner">
      <div class="banner-overlay">
        <v-container style="max-width:1280px">
          <div class="banner-content">
            <div class="banner-badge mb-4">
              <v-icon size="18" class="mr-2">mdi-shield-check-outline</v-icon>
              <span>QUY ĐỊNH & NỘI QUY</span>
            </div>
            <h1 class="banner-title mb-4">NỘI QUY KÝ TÚC XÁ</h1>
            <p class="banner-subtitle mb-6">Quy định chung về sinh hoạt, an ninh và trách nhiệm của sinh viên nội trú</p>
            <div class="d-flex align-center ga-2 banner-breadcrumb">
              <router-link to="/" class="text-white text-decoration-none hover-underline">
                <v-icon size="16" class="mr-1">mdi-home</v-icon>
                Trang chủ
              </router-link>
              <v-icon size="16">mdi-chevron-right</v-icon>
              <span>Nội quy</span>
            </div>
          </div>
        </v-container>
      </div>
    </section>

    <v-container class="py-16" style="max-width:1280px">
      <v-row>
        <v-col cols="12" lg="8">
          <h2 class="section-title mb-8">QUY ĐỊNH CHUNG</h2>
          <v-expansion-panels variant="accordion" class="border rounded-xl overflow-hidden">
            <v-expansion-panel v-for="rule in ruleGroups" :key="rule.title">
              <v-expansion-panel-title class="font-weight-bold py-6">
                <v-icon :icon="rule.icon" class="mr-4" color="primary" />
                {{ rule.title }}
              </v-expansion-panel-title>
              <v-expansion-panel-text class="bg-grey-lighten-5 pa-4">
                <ul class="rule-list">
                  <li v-for="(item, idx) in rule.items" :key="idx">{{ item }}</li>
                </ul>
              </v-expansion-panel-text>
            </v-expansion-panel>
          </v-expansion-panels>

          <v-card flat border color="error-lighten-5" class="pa-10 rounded-xl mt-12 border-error border-opacity-25">
            <div class="d-flex align-center ga-4 mb-6">
              <v-icon color="error" size="32">mdi-alert-octagon</v-icon>
              <h3 class="text-h5 font-weight-black text-error">CÁC HÀNH VI NGHIÊM CẤM</h3>
            </div>
            <v-row>
              <v-col v-for="p in prohibited" :key="p" cols="12" md="6">
                <div class="d-flex align-start ga-3">
                  <v-icon color="error" size="18" class="mt-1">mdi-close-circle</v-icon>
                  <span class="text-body-2 font-weight-bold opacity-80">{{ p }}</span>
                </div>
              </v-col>
            </v-row>
          </v-card>
        </v-col>

        <v-col cols="12" lg="4">
          <v-card flat border class="pa-8 rounded-xl sticky-top">
            <h3 class="text-subtitle-1 font-weight-black mb-6 border-b pb-4">Tài liệu liên quan</h3>
            <div class="d-flex flex-column ga-4">
              <div v-for="doc in documents" :key="doc.name" class="doc-link d-flex align-center justify-space-between pa-4 border rounded-lg">
                <div class="d-flex align-center ga-3">
                  <v-icon color="error">mdi-file-pdf-box</v-icon>
                  <span class="text-body-2 font-weight-bold">{{ doc.name }}</span>
                </div>
                <v-btn icon="mdi-download" variant="text" size="small" />
              </div>
            </div>
            <v-divider class="my-8" />
            <div class="pa-4 bg-amber-lighten-5 rounded-lg border-amber border-opacity-25 border">
              <div class="text-caption font-weight-black text-amber-darken-4 mb-1">Lưu ý</div>
              <p class="text-caption opacity-70">Sinh viên vi phạm nội quy tùy theo mức độ sẽ bị xử lý kỷ luật từ nhắc nhở đến chấm dứt hợp đồng.</p>
            </div>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
const ruleGroups = [
  { title: 'Giờ giấc ra vào', icon: 'mdi-clock-outline', items: ['Giờ mở cửa KTX: 05h00 hàng ngày.', 'Giờ đóng cửa KTX: 23h00 hàng ngày.', 'Sinh viên ra vào phải mang theo thẻ nội trú.', 'Khách đến thăm phải đăng ký tại phòng bảo vệ.'] },
  { title: 'An ninh & Trật tự', icon: 'mdi-shield-lock-outline', items: ['Thực hiện quy định về an ninh trật tự, PCCC.', 'Không gây ồn ào sau 22h00.', 'Tự bảo quản tài sản cá nhân.', 'Nghiêm cấm cho người lạ vào phòng ở.'] },
  { title: 'Vệ sinh & Cảnh quan', icon: 'mdi-sparkles-outline', items: ['Giữ gìn vệ sinh trong phòng và khu vực chung.', 'Rác thải bỏ đúng nơi quy định.', 'Không phơi quần áo bừa bãi.', 'Thực hiện tổng vệ sinh định kỳ.'] },
  { title: 'Sử dụng Điện & Nước', icon: 'mdi-lightning-bolt-outline', items: ['Sử dụng tiết kiệm, đúng mục đích.', 'Tắt thiết bị khi ra khỏi phòng.', 'Không tự ý đấu nối hệ thống.', 'Không dùng thiết bị công suất lớn trong phòng.'] }
]

const prohibited = ['Tàng trữ, sử dụng ma túy.', 'Uống rượu, bia, hút thuốc lá.', 'Đánh bạc dưới mọi hình thức.', 'Tàng trữ vũ khí, chất nổ.', 'Tuyên truyền ấn phẩm đồi trụy.', 'Gây gổ, đánh nhau.', 'Nuôi động vật trong KTX.', 'Nấu ăn trong phòng ở.']
const documents = [{ name: 'Nội quy đầy đủ (PDF)' }, { name: 'Quy định kỷ luật (PDF)' }, { name: 'Hướng dẫn PCCC (PDF)' }]
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
.rule-list { list-style: disc; padding-left: 20px; }
.rule-list li { margin-bottom: 12px; }
.sticky-top { position: sticky; top: 100px; }
.doc-link:hover { border-color: #ff6b00 !important; color: #ff6b00; }
.hover-underline:hover { text-decoration: underline; }
</style>
