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
        <v-carousel-item
          v-for="(slide, i) in slides"
          :key="i"
          :src="slide.image"
          cover
        >
          <div class="hero-overlay">
            <v-container style="max-width: 1280px">
              <v-row align="center" style="height: 100%">
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
    <v-container class="py-12" style="max-width: 1280px">
      <v-row>
        <v-col
          v-for="info in quickInfos"
          :key="info.title"
          cols="12"
          sm="6"
          md="3"
        >
          <v-card flat border class="info-card pa-8 rounded-xl h-100">
            <v-icon :color="info.color" size="48" class="mb-4">{{
              info.icon
            }}</v-icon>
            <h3 class="text-h6 font-weight-bold mb-2">{{ info.title }}</h3>
            <p class="text-body-2 text-medium-emphasis mb-4">{{ info.desc }}</p>
            <v-btn
              variant="text"
              :color="info.color"
              class="pa-0 font-weight-bold"
              append-icon="mdi-chevron-right"
              >Xem chi tiết</v-btn
            >
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <!-- ═══ REGISTRATION PROCESS ═══ -->
    <section class="bg-slate-50 py-16">
      <v-container style="max-width: 1280px">
        <div class="text-center mb-12">
          <h2 class="section-title mb-4">QUY TRÌNH ĐĂNG KÝ Ở KTX</h2>
          <p class="text-body-1 text-medium-emphasis">
            4 bước đơn giản để trở thành cư dân KTX Đại học Đại Nam
          </p>
        </div>

        <v-row>
          <v-col
            v-for="(step, index) in registrationSteps"
            :key="step.id"
            cols="12"
            sm="6"
            md="3"
          >
            <v-card flat border class="process-card pa-6 rounded-xl h-100 text-center">
              <div class="step-number mb-4 mx-auto">{{ index + 1 }}</div>
              <v-icon :color="step.color" size="48" class="mb-4">{{ step.icon }}</v-icon>
              <h3 class="text-h6 font-weight-bold mb-3">{{ step.title }}</h3>
              <p class="text-body-2 text-medium-emphasis">{{ step.desc }}</p>
            </v-card>
          </v-col>
        </v-row>

        <div class="text-center mt-8">
          <v-btn
            color="primary"
            size="large"
            class="font-weight-bold px-8 rounded-lg text-white"
            to="/rooms"
          >
            BẮT ĐẦU ĐĂNG KÝ
            <v-icon class="ml-2">mdi-arrow-right</v-icon>
          </v-btn>
        </div>
      </v-container>
    </section>

    <!-- ═══ FACILITIES & AMENITIES ═══ -->
    <section class="py-16 bg-white">
      <v-container style="max-width: 1280px">
        <div class="text-center mb-12">
          <h2 class="section-title mb-4">TIỆN ÍCH & DỊCH VỤ</h2>
          <p class="text-body-1 text-medium-emphasis">
            Đầy đủ tiện nghi phục vụ sinh hoạt và học tập
          </p>
        </div>

        <v-row>
          <v-col
            v-for="facility in facilities"
            :key="facility.id"
            cols="12"
            sm="6"
            md="4"
          >
            <v-card flat border class="facility-card rounded-xl overflow-hidden h-100">
              <v-img :src="facility.image" height="200" cover>
                <div class="facility-overlay">
                  <v-icon size="48" color="white">{{ facility.icon }}</v-icon>
                </div>
              </v-img>
              <v-card-text class="pa-6">
                <h3 class="text-h6 font-weight-bold mb-2">{{ facility.title }}</h3>
                <p class="text-body-2 text-medium-emphasis">{{ facility.desc }}</p>
                <v-chip size="small" :color="facility.color" variant="flat" class="mt-3 free">
                  {{ facility.available }}
                </v-chip>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </section>

    <!-- ═══ STATISTICS ═══ -->
    <section class="stats-section py-16 text-white text-center">
      <v-container style="max-width: 1280px">
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
        <p
          class="text-h6 text-medium-emphasis mb-10 mx-auto"
          style="max-width: 800px; line-height: 1.6"
        >
          Đăng ký ở nội trú để trải nghiệm môi trường sinh hoạt năng động, an
          toàn và hỗ trợ tối đa cho việc học tập tại Đại học Đại Nam.
        </p>
        <div class="d-flex justify-center ga-4 flex-wrap">
          <v-btn
            color="primary"
            size="x-large"
            class="font-weight-bold px-10 rounded-xl text-white"
            height="64"
            to="/login"
            >ĐĂNG KÝ NGAY</v-btn
          >
          <v-btn
            variant="outlined"
            color="primary"
            size="x-large"
            class="font-weight-bold px-10 rounded-xl"
            height="64"
            >TẢI HỒ SƠ</v-btn
          >
        </div>
      </v-container>
    </section>
  </div>
</template>

<script setup>
const slides = [
  {
    title: "Không gian sống lý tưởng",
    desc: "Hệ thống KTX hiện đại, tiện nghi với đầy đủ các khu chức năng hỗ trợ học tập và giải trí tại Đại học Đại Nam.",
    image: "/images/hero_dormitory.png",
  },
  {
    title: "AN TOÀN & HIỆN ĐẠI",
    desc: "Môi trường sinh hoạt kỷ luật, văn minh giúp sinh viên an tâm học tập và rèn luyện kỹ năng sống.",
    image: "/images/student_life.png",
  },
];

const quickInfos = [
  {
    title: "Thủ tục đăng ký",
    desc: "Quy trình và hồ sơ cần thiết để đăng ký ở nội trú tại Ký túc xá Đại học Đại Nam.",
    icon: "mdi-file-document-edit-outline",
    color: "primary",
  },
  {
    title: "Mức phí & Thanh toán",
    desc: "Thông tin về đơn giá phòng ở và các phương thức thanh toán lệ phí KTX linh hoạt.",
    icon: "mdi-cash-multiple",
    color: "success",
  },
  {
    title: "Nội quy & Quy định",
    desc: "Các quy định về sinh hoạt, an ninh trật tự và trách nhiệm của sinh viên nội trú.",
    icon: "mdi-shield-account-outline",
    color: "warning",
  },
  {
    title: "Hỗ trợ sinh viên",
    desc: "Kênh tiếp nhận phản hồi và xử lý các vấn đề phát sinh trong quá trình ở KTX.",
    icon: "mdi-face-agent",
    color: "info",
  },
];

const registrationSteps = [
  {
    id: 1,
    title: "Đăng nhập hệ thống",
    desc: "Sử dụng mã sinh viên và mật khẩu được cấp để truy cập hệ thống quản lý KTX",
    icon: "mdi-login",
    color: "primary",
  },
  {
    id: 2,
    title: "Chọn phòng",
    desc: "Xem danh sách phòng trống, thông tin chi tiết và chọn phòng phù hợp",
    icon: "mdi-home-search",
    color: "success",
  },
  {
    id: 3,
    title: "Nộp hồ sơ",
    desc: "Điền thông tin cá nhân, upload giấy tờ cần thiết và gửi đơn đăng ký",
    icon: "mdi-file-upload",
    color: "warning",
  },
  {
    id: 4,
    title: "Nhận phòng",
    desc: "Sau khi được duyệt, đến KTX làm thủ tục nhận phòng và ký hợp đồng",
    icon: "mdi-key",
    color: "info",
  },
];

const facilities = [
  {
    id: 1,
    title: "Phòng ở đầy đủ tiện nghi",
    desc: "Giường tủ cá nhân, máy lạnh, nước nóng, Internet tốc độ cao miễn phí",
    icon: "mdi-bed",
    image: "https://images.unsplash.com/photo-1555854877-bab0e564b8d5?w=800&q=80",
    color: "primary",
    available: "Sẵn có",
  },
  {
    id: 2,
    title: "Khu vực học tập chung",
    desc: "Không gian yên tĩnh, bàn ghế ergonomic, điều hòa mát mẻ",
    icon: "mdi-book-open-page-variant",
    image: "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=800&q=80",
    color: "info",
    available: "24/7",
  },
  {
    id: 3,
    title: "Phòng gym & thể thao",
    desc: "Trang thiết bị hiện đại, sân bóng, sân cầu lông, bàn bóng bàn",
    icon: "mdi-dumbbell",
    image: "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800&q=80",
    color: "success",
    available: "Miễn phí",
  },
  {
    id: 4,
    title: "Căng tin & Minimart",
    desc: "Đa dạng món ăn, giá sinh viên, mở cửa từ sáng đến tối",
    icon: "mdi-food",
    image: "https://images.unsplash.com/photo-1567521464027-f127ff144326?w=800&q=80",
    color: "warning",
    available: "6:00 - 22:00",
  },
  {
    id: 5,
    title: "Giặt ủi tự động",
    desc: "Máy giặt, máy sấy công nghiệp, thanh toán qua thẻ tiện lợi",
    icon: "mdi-washing-machine",
    image: "https://images.unsplash.com/photo-1582735689369-4fe89db7114c?w=800&q=80",
    color: "error",
    available: "Có phí",
  },
  {
    id: 6,
    title: "An ninh 24/7",
    desc: "Bảo vệ túc trực, camera giám sát, kiểm soát ra vào nghiêm ngặt",
    icon: "mdi-shield-check",
    image: "https://images.unsplash.com/photo-1557597774-9d273605dfa9?w=800&q=80",
    color: "deep-purple",
    available: "24/7",
  },
];

const statsItems = [
  { num: "2000+", label: "CHỖ Ở SINH VIÊN" },
  { num: "100%", label: "PHỦ SÓNG WIFI" },
  { num: "24/7", label: "AN NINH GIÁM SÁT" },
  { num: "05+", label: "TIỆN ÍCH HIỆN ĐẠI" },
];
</script>

<style scoped>
.hero-overlay {
  height: 100%;
  display: flex;
  align-items: center;
  background: linear-gradient(
    135deg,
    rgba(15, 23, 42, 0.72) 0%,
    rgba(15, 23, 42, 0.48) 45%,
    rgba(15, 23, 42, 0.28) 100%
  );
  color: white;
  position: relative;
}
.hero-overlay::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(
    circle at 20% 50%,
    rgba(255, 255, 255, 0.1) 0%,
    transparent 50%
  );
  pointer-events: none;
}
.hero-badge {
  display: inline-flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 10px 24px;
  border-radius: 50px;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 1px;
  border: 1px solid rgba(255, 255, 255, 0.3);
}
.hero-title {
  font-size: 3.5rem;
  font-weight: 900;
  line-height: 1.1;
  margin-bottom: 24px;
  text-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  letter-spacing: -1px;
}
.hero-subtitle {
  font-size: 1.25rem;
  font-weight: 500;
  opacity: 0.95;
  max-width: 600px;
  line-height: 1.7;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}
.free{
  color:white;
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
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}
.stat-label {
  font-size: 14px;
  font-weight: 600;
  opacity: 0.9;
  letter-spacing: 0.5px;
}

.info-card {
  transition: all 0.3s;
}
.info-card:hover {
  transform: translateY(-5px);
  border-color: #ff6b00 !important;
  box-shadow: 0 10px 30px rgba(255, 107, 0, 0.1) !important;
}

.section-title {
  font-size: 1.8rem;
  font-weight: 900;
  color: #ff6b00;
  position: relative;
  padding-left: 20px;
}
.section-title::before {
  content: "";
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 6px;
  background: #ff6b00;
  border-radius: 3px;
}

.news-title {
  font-size: 1.1rem;
  font-weight: 800;
  color: #1e293b;
  line-height: 1.4;
}

.notif-date {
  flex: 0 0 54px;
  height: 60px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
.notif-day {
  font-size: 1.3rem;
  font-weight: 900;
  color: #ff6b00;
  line-height: 1;
}
.notif-month {
  font-size: 10px;
  font-weight: 800;
  color: #64748b;
  margin-top: 2px;
}

.stats-section {
  background: linear-gradient(135deg, #ff6b00, #ff8800);
}
.stat-num {
  font-size: 3.5rem;
  font-weight: 900;
  line-height: 1;
  margin-bottom: 8px;
}
.stat-label {
  font-size: 14px;
  font-weight: 800;
  letter-spacing: 2px;
  opacity: 0.8;
}

.bg-slate-50 {
  background-color: #f8fafc;
}
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Registration Process Styles */
.process-card {
  transition: all 0.3s ease;
  position: relative;
  border: 2px solid transparent;
}
.process-card:hover {
  transform: translateY(-8px);
  border-color: #ff6b00;
  box-shadow: 0 12px 40px rgba(255, 107, 0, 0.15) !important;
}
.step-number {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ff6b00, #ff8800);
  color: white;
  font-size: 2rem;
  font-weight: 900;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 20px rgba(255, 107, 0, 0.3);
}

/* Facilities Styles */
.facility-card {
  transition: all 0.3s ease;
  cursor: pointer;
}
.facility-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.1) !important;
}
.facility-card:hover .facility-overlay {
  opacity: 1;
}
.facility-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 107, 0, 0.85);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}
</style>
