<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-4">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Yêu cầu Bảo trì</h1>
        <p class="text-body-2 text-medium-emphasis">{{ requests.length }} yêu cầu — {{ requests.filter(r=>r.status==='Chờ xử lý').length }} đang chờ</p>
      </div>
      <v-btn prepend-icon="mdi-plus" color="primary" @click="dialog=true">Tạo yêu cầu</v-btn>
    </div>

    <!-- Status tabs -->
    <v-tabs v-model="tab" color="primary" class="mb-6">
      <v-tab value="all">Tất cả</v-tab>
      <v-tab value="pending">Chờ xử lý <v-badge :content="requests.filter(r=>r.status==='Chờ xử lý').length" color="warning" inline class="ml-1" /></v-tab>
      <v-tab value="progress">Đang xử lý</v-tab>
      <v-tab value="done">Hoàn thành</v-tab>
    </v-tabs>

    <v-row>
      <v-col v-for="r in filtered" :key="r.id" cols="12" md="6">
        <v-card style="border:1px solid #e5e7eb" class="pa-5">
          <div class="d-flex align-start justify-space-between mb-3">
            <div class="d-flex ga-3 align-start">
              <v-avatar :color="priColor(r.priority)" size="40" rounded="lg" variant="tonal">
                <v-icon size="20">{{ r.priority==='Khẩn cấp'?'mdi-alert':'mdi-wrench' }}</v-icon>
              </v-avatar>
              <div>
                <div class="text-subtitle-1 font-weight-bold">{{ r.title }}</div>
                <div class="text-caption text-medium-emphasis">Phòng {{ r.room }} · {{ r.date }}</div>
              </div>
            </div>
            <v-chip :color="stColor(r.status)" size="x-small" variant="tonal">{{ r.status }}</v-chip>
          </div>
          <p class="text-body-2 text-medium-emphasis mb-4">{{ r.desc }}</p>
          <v-divider class="mb-3" />
          <div class="d-flex align-center justify-space-between">
            <div class="d-flex align-center ga-2 text-caption text-medium-emphasis">
              <v-icon size="14">mdi-account</v-icon>{{ r.student }}
              <v-chip v-if="r.priority==='Khẩn cấp'" color="error" size="x-small" variant="flat" class="ml-2">Khẩn cấp</v-chip>
            </div>
            <div class="d-flex ga-1">
              <v-btn v-if="r.status==='Chờ xử lý'" size="small" color="primary" variant="tonal">Xử lý</v-btn>
              <v-btn v-if="r.status==='Đang xử lý'" size="small" color="success" variant="tonal">Hoàn thành</v-btn>
              <v-btn icon="mdi-eye-outline" size="small" variant="text" />
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="dialog" max-width="520">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Tạo yêu cầu bảo trì</h2>
        <v-select label="Phòng" :items="['101-A1','102-A1','201-B1','301-C1']" class="mb-3" />
        <v-text-field label="Tiêu đề" placeholder="VD: Hỏng vòi nước" class="mb-3" />
        <v-select label="Mức độ ưu tiên" :items="['Bình thường','Khẩn cấp']" class="mb-3" />
        <v-textarea label="Mô tả chi tiết" rows="3" />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog=false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog=false">Gửi yêu cầu</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
const tab = ref('all')
const dialog = ref(false)

const requests = ref([
  { id:1,title:'Hỏng vòi nước',room:'101-A1',student:'Nguyễn Văn An',date:'15/05/2026',status:'Chờ xử lý',priority:'Bình thường',desc:'Vòi nước trong nhà vệ sinh bị rò rỉ mạnh, cần thay mới gấp.' },
  { id:2,title:'Cháy bóng đèn',room:'202-B2',student:'Trần Thị Bình',date:'14/05/2026',status:'Đang xử lý',priority:'Bình thường',desc:'Bóng đèn chính phòng bị hỏng, hiện tại rất tối.' },
  { id:3,title:'Hỏng khóa cửa',room:'305-C1',student:'Võ Thanh Phong',date:'15/05/2026',status:'Chờ xử lý',priority:'Khẩn cấp',desc:'Khóa cửa chính bị kẹt, không thể đóng được. Cần sửa gấp để đảm bảo an toàn.' },
  { id:4,title:'Điều hòa không mát',room:'105-A2',student:'Ngô Thị Giang',date:'12/05/2026',status:'Hoàn thành',priority:'Bình thường',desc:'Điều hòa chạy nhưng không ra hơi lạnh. Đã nạp gas xong.' },
  { id:5,title:'Tắc bồn rửa',room:'203-B1',student:'Phạm Hoàng Duy',date:'16/05/2026',status:'Chờ xử lý',priority:'Bình thường',desc:'Bồn rửa mặt bị tắc, nước thoát rất chậm.' },
])

const filtered = computed(() => {
  if (tab.value === 'all') return requests.value
  const m = { pending:'Chờ xử lý', progress:'Đang xử lý', done:'Hoàn thành' }
  return requests.value.filter(r => r.status === m[tab.value])
})

const stColor = s => ({'Chờ xử lý':'warning','Đang xử lý':'info','Hoàn thành':'success'}[s]||'grey')
const priColor = p => p==='Khẩn cấp'?'error':'info'
</script>
