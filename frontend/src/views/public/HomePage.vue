<template>
  <div class="home-page">
    <!-- ═══ HERO SLIDER ═══ -->
    <section class="hero">
      <v-carousel
        cycle
        height="650"
        hide-delimiter-background
        show-arrows="hover"
        :interval="5000"
      >
        <v-carousel-item v-for="(slide, i) in slides" :key="i" :src="slide.image" cover>
          <div class="hero-overlay">
            <v-container style="max-width:1280px">
              <v-row align="center" style="height: 100%;">
                <v-col cols="12" md="8" lg="7">
                  <div class="hero-badge mb-6">
                    <v-icon size="20" class="mr-2">mdi-shield-check</v-icon>
                    <span>HỆ THỐNG KÝ TÚC XÁ HIỆN ĐẠI</span>
                  </div>
                  <h1 class="hero-title">{{ slide.title }}</h1>
                  <p class="hero-subtitle">{{ slide.desc }}</p>
                  <div class="d-flex ga-4 mt-10 flex-wrap">
                    <v-btn 
                      color="white" 
                      size="x-large" 
                      class="font-weight-bold px-10 rounded-xl text-primary" 
                      height="60"
                      to="/rooms"
                      elevation="8"
                    >
                      <v-icon class="mr-2">mdi-home-search</v-icon>
                      ĐĂNG KÝ PHÒNG
                    </v-btn>
                    <v-btn 
                      variant="outlined" 
                      color="white" 
                      size="x-large" 
                      class="font-weight-bold px-10 rounded-xl" 
                      height="60"
                      to="/about"
                    >
                      <v-icon class="mr-2">mdi-information</v-icon>
                      TÌM HIỂU THÊM
                    </v-btn>
                  </div>

                  <!-- Stats Row -->
                  <div class="hero-stats mt-12">
                    <div class="stat-item">
                      <div class="stat-value">2.000+</div>
                      <div class="stat-label">Sinh viên</div>
                    </div>
                    <div class="stat-item">
                      <div class="stat-value">500+</div>
                      <div class="stat-label">Phòng ở</div>
                    </div>
                    <div class="stat-item">
                      <div class="stat-value">24/7</div>
                      <div class="stat-label">Bảo vệ</div>
                    </div>
                  </div>
                </v-col>
              </v-row>
            </v-container>
          </div>
        </v-carousel-item>
      </v-carousel>
    </section>

    <!-- ═══ QUICK INFO ═══ -->
    <v-container class="py-12" style="max-width:1280px">
      <v-row>
        <v-col v-for="info in quickInfos" :key="info.title" cols="12" sm="6" md="3">
          <v-card flat border class="info-card pa-8 rounded-xl h-100">
            <v-icon :color="info.color" size="48" class="mb-4">{{ info.icon }}</v-icon>
            <h3 class="text-h6 font-weight-bold mb-2">{{ info.title }}</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">{{ info.desc }}</p>
            <v-btn variant="text" :color="info.color" class="pa-0 font-weight-bold" append-icon="mdi-chevron-right">Xem chi tiết</v-btn>
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <!-- ═══ NEWS & ANNOUNCEMENTS ═══ -->
    <section class="bg-slate-50 py-16">
      <v-container style="max-width:1280px">
        <v-row>
          <!-- Left: Featured News -->
          <v-col cols="12" md="8">
            <div class="d-flex align-center justify-space-between mb-8">
              <h2 class="section-title">TIN TỨC & SỰ KIỆN</h2>
              <v-btn variant="text" color="primary" class="font-weight-bold" append-icon="mdi-arrow-right">TẤT CẢ TIN TỨC</v-btn>
            </div>
            
            <v-row>
              <v-col cols="12" sm="6" v-for="n in news" :key="n.id">
                <v-card flat border class="news-card rounded-xl overflow-hidden">
                  <v-img :src="n.image" height="200" cover>
                    <template v-slot:placeholder>
                      <div class="d-flex align-center justify-center fill-height bg-grey-lighten-4">
                        <v-icon size="48" color="grey-lighten-2">mdi-image</v-icon>
                      </div>
                    </template>
                  </v-img>
                  <v-card-text class="pa-6">
                    <div class="text-caption text-primary font-weight-bold mb-2">{{ n.category }}</div>
                    <h3 class="news-title mb-2">{{ n.title }}</h3>
                    <p class="text-body-2 text-medium-emphasis mb-4 line-clamp-3">{{ n.desc }}</p>
                    <div class="d-flex align-center text-caption text-medium-emphasis border-t pt-4 mt-4">
                      <v-icon size="14" class="mr-1">mdi-calendar-range</v-icon> {{ n.date }}
                      <v-spacer />
                      <v-btn variant="text" color="primary" size="small" class="font-weight-bold pa-0">ĐỌC TIẾP</v-btn>
                    </div>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>
          </v-col>

          <!-- Right: Notifications -->
          <v-col cols="12" md="4">
            <h2 class="section-title mb-8">THÔNG BÁO</h2>
            <v-card flat border class="pa-6 rounded-xl bg-white">
              <div v-for="notif in notices" :key="notif.id" class="mb-6 last:mb-0">
                <div class="d-flex ga-4">
                  <div class="notif-date">
                    <div class="notif-day">{{ notif.day }}</div>
                    <div class="notif-month">TH{{ notif.month }}</div>
                  </div>
                  <div class="min-w-0">
                    <h4 class="text-body-2 font-weight-bold line-clamp-2 mb-1">{{ notif.title }}</h4>
                    <v-chip size="x-small" :color="notif.urgent ? 'error' : 'primary'" variant="flat" class="font-weight-bold px-2">
                      {{ notif.urgent ? 'KHẨN' : 'THÔNG BÁO' }}
                    </v-chip>
                  </div>
                </div>
              </div>
              <v-btn block variant="outlined" color="primary" class="mt-8 font-weight-bold rounded-lg">XEM TẤT CẢ THÔNG BÁO</v-btn>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- ═══ STATISTICS ═══ -->
    <section class="stats-section py-16 text-white text-center">
      <v-container style="max-width:1280px">
        <v-row>
          <v-col v-for="s in statsItems" :key="s.label" cols="6" md="3">
            <div class="stat-num">{{ s.num }}</div>
            <div class="stat-label">{{ s.label }}</div>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- ═══ CALL TO ACTION ═══ -->
    <section class="py-16 text-center bg-white">
      <v-container>
        <h2 class="text-h3 font-weight-bold mb-4">Bạn đã sẵn sàng gia nhập?</h2>
        <p class="text-h6 text-medium-emphasis mb-10 mx-auto" style="max-width: 800px; line-height: 1.6;">
          Đăng ký ở nội trú để trải nghiệm môi trường sinh hoạt năng động, an toàn và hỗ trợ tối đa cho việc học tập tại Đại học Đại Nam.
        </p>
        <div class="d-flex justify-center ga-4 flex-wrap">
          <v-btn color="primary" size="x-large" class="font-weight-bold px-10 rounded-xl" height="64" to="/login">ĐĂNG KÝ NGAY</v-btn>
          <v-btn variant="outlined" color="primary" size="x-large" class="font-weight-bold px-10 rounded-xl" height="64">TẢI HỒ SƠ</v-btn>
        </div>
      </v-container>
    </section>
  </div>
</template>

<script setup>
const slides = [
  { title: 'Không gian sống lý tưởng', desc: 'Hệ thống KTX hiện đại, tiện nghi với đầy đủ các khu chức năng hỗ trợ học tập và giải trí tại Đại học Đại Nam.', image: '/images/hero_dormitory.png' },
  { title: 'AN TOÀN & HIỆN ĐẠI', desc: 'Môi trường sinh hoạt kỷ luật, văn minh giúp sinh viên an tâm học tập và rèn luyện kỹ năng sống.', image: '/images/student_life.png' }
]

const quickInfos = [
  { title: 'Thủ tục đăng ký', desc: 'Quy trình và hồ sơ cần thiết để đăng ký ở nội trú tại Ký túc xá Đại học Đại Nam.', icon: 'mdi-file-document-edit-outline', color: 'primary' },
  { title: 'Mức phí & Thanh toán', desc: 'Thông tin về đơn giá phòng ở và các phương thức thanh toán lệ phí KTX linh hoạt.', icon: 'mdi-cash-multiple', color: 'success' },
  { title: 'Nội quy & Quy định', desc: 'Các quy định về sinh hoạt, an ninh trật tự và trách nhiệm của sinh viên nội trú.', icon: 'mdi-shield-account-outline', color: 'warning' },
  { title: 'Hỗ trợ sinh viên', desc: 'Kênh tiếp nhận phản hồi và xử lý các vấn đề phát sinh trong quá trình ở KTX.', icon: 'mdi-face-agent', color: 'info' },
]

const news = [
  { id: 1, category: 'TIN TỨC KTX', title: 'Hướng dẫn quy trình đăng ký ở KTX cho tân sinh viên khóa mới', desc: 'Ban quản lý KTX Đại học Đại Nam hướng dẫn chi tiết các bước đăng ký, nộp hồ sơ và làm thủ tục nhận phòng cho sinh viên khóa mới nhập học năm 2026...', date: '15/05/2026', image: '/images/student_life.png' },
  { id: 2, category: 'SỰ KIỆN', title: 'Giải bóng đá Nam - Nữ KTX Đại Nam Cup 2026 chính thức khởi tranh', desc: 'Nhằm tạo sân chơi lành mạnh, tăng cường đoàn kết giữa các sinh viên nội trú, giải bóng đá truyền thống KTX sẽ diễn ra vào cuối tuần này...', date: '12/05/2026', image: '/images/football_match.png' },
]

const notices = [
  { id: 1, day: '18', month: '05', title: 'Thông báo về việc phun thuốc diệt côn trùng định kỳ tại các tòa nhà', urgent: false },
  { id: 2, day: '16', month: '05', title: 'Nhắc nộp lệ phí KTX tháng 05/2026 cho tất cả các phòng', urgent: true },
  { id: 3, day: '14', month: '05', title: 'Thông báo lịch kiểm tra phòng định kỳ học kỳ II năm 2026', urgent: false },
  { id: 4, day: '10', month: '05', title: 'Về việc đảm bảo an ninh trật tự và phòng chống cháy nổ tại KTX', urgent: true },
]

const statsItems = [
  { num: '2000+', label: 'CHỖ Ở SINH VIÊN' },
  { num: '100%', label: 'PHỦ SÓNG WIFI' },
  { num: '24/7', label: 'AN NINH GIÁM SÁT' },
  { num: '05+', label: 'TIỆN ÍCH HIỆN ĐẠI' },
]
</script>

<style scoped>
.hero-overlay { 
  height: 100%; 
  display: flex; 
  align-items: center; 
  background: linear-gradient(135deg, rgba(255,107,0,0.95) 0%, rgba(255,136,0,0.85) 50%, rgba(255,107,0,0.75) 100%);
  color: white; 
  position: relative;
}
.hero-overlay::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 20% 50%, rgba(255,255,255,0.1) 0%, transparent 50%);
  pointer-events: none;
}
.hero-badge {
  display: inline-flex;
  align-items: center;
  background: rgba(255,255,255,0.2);
  backdrop-filter: blur(10px);
  padding: 10px 24px;
  border-radius: 50px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 1px;
  border: 1px solid rgba(255,255,255,0.3);
}
.hero-title { 
  font-size: 3.5rem; 
  font-weight: 900; 
  line-height: 1.1; 
  margin-bottom: 24px; 
  text-shadow: 0 4px 20px rgba(0,0,0,0.3);
  letter-spacing: -1px;
}
.hero-subtitle { 
  font-size: 1.25rem; 
  font-weight: 500; 
  opacity: 0.95; 
  max-width: 600px; 
  line-height: 1.7;
  text-shadow: 0 2px 10px rgba(0,0,0,0.2);
}

/* Hero Stats */
.hero-stats {
  display: flex;
  gap: 48px;
  align-items: center;
}
.stat-item {
  text-align: left;
}
.stat-value {
  font-size: 2.5rem;
  font-weight: 900;
  line-height: 1;
  margin-bottom: 4px;
  text-shadow: 0 2px 10px rgba(0,0,0,0.2);
}
.stat-label {
  font-size: 14px;
  font-weight: 600;
  opacity: 0.9;
  letter-spacing: 0.5px;
}

.info-card { transition: all .3s; }
.info-card:hover { transform: translateY(-5px); border-color: #ff6b00 !important; box-shadow: 0 10px 30px rgba(255,107,0,0.1) !important; }

.section-title { font-size: 1.8rem; font-weight: 900; color: #ff6b00; position: relative; padding-left: 20px; }
.section-title::before { content: ''; position: absolute; left: 0; top: 0; bottom: 0; width: 6px; background: #ff6b00; border-radius: 3px; }

.news-title { font-size: 1.1rem; font-weight: 800; color: #1e293b; line-height: 1.4; }

.notif-date { flex: 0 0 54px; height: 60px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.notif-day { font-size: 1.3rem; font-weight: 900; color: #ff6b00; line-height: 1; }
.notif-month { font-size: 10px; font-weight: 800; color: #64748b; margin-top: 2px; }

.stats-section { background: linear-gradient(135deg, #ff6b00, #ff8800); }
.stat-num { font-size: 3.5rem; font-weight: 900; line-height: 1; margin-bottom: 8px; }
.stat-label { font-size: 14px; font-weight: 800; letter-spacing: 2px; opacity: 0.8; }

.bg-slate-50 { background-color: #f8fafc; }
.line-clamp-3 { display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; }
</style>
