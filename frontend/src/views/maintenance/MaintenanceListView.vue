<template>
  <div>
    <div style="margin-bottom: 16px">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0">
        Yêu cầu Bảo trì
      </h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
        {{ requests.length }} yêu cầu —
        {{ requests.filter((r) => r.status === "Chờ xử lý").length }} đang chờ
      </p>
    </div>

    <!-- Status tabs -->
    <v-tabs v-model="tab" color="primary" style="margin-bottom: 16px">
      <v-tab value="all">Tất cả</v-tab>
      <v-tab value="pending"
        >Chờ xử lý
        <v-badge
          :content="requests.filter((r) => r.status === 'Chờ xử lý').length"
          color="warning"
          inline
          class="ml-1"
      /></v-tab>
      <v-tab value="progress">Đang xử lý</v-tab>
      <v-tab value="done">Hoàn thành</v-tab>
    </v-tabs>

    <v-row>
      <v-col v-for="r in filtered" :key="r.id" cols="12" md="6">
        <v-card style="border: 1px solid #e5e7eb" class="pa-5">
          <div class="d-flex align-start justify-space-between mb-3">
            <div class="d-flex ga-3 align-start">
              <v-avatar
                :color="priColor(r.priority)"
                size="40"
                rounded="lg"
                variant="tonal"
              >
                <v-icon size="20">{{
                  r.priority === "Khẩn cấp" ? "mdi-alert" : "mdi-wrench"
                }}</v-icon>
              </v-avatar>
              <div>
                <div class="text-subtitle-1 font-weight-bold">
                  {{ r.title }}
                </div>
                <div class="text-caption text-medium-emphasis">
                  #{{ r.code }} · Phòng {{ r.room }} · {{ r.date }}
                </div>
              </div>
            </div>
            <v-chip :color="stColor(r.status)" size="x-small" variant="tonal">{{
              r.status
            }}</v-chip>
          </div>
          <p class="text-body-2 text-medium-emphasis mb-2">{{ r.desc }}</p>
          <p v-if="r.note" class="text-caption text-medium-emphasis mb-4">
            Ghi chú: {{ r.note }}
          </p>
          <v-divider class="mb-3" />
          <div class="d-flex align-center justify-space-between">
            <div
              class="d-flex align-center ga-2 text-caption text-medium-emphasis"
            >
              <v-icon size="14">mdi-account</v-icon>{{ r.student }}
              <v-chip
                v-if="r.priority === 'Khẩn cấp'"
                color="error"
                size="x-small"
                variant="flat"
                class="ml-2"
                >Khẩn cấp</v-chip
              >
            </div>
            <div class="d-flex ga-1">
              <v-btn
                v-if="r.status === 'Chờ xử lý'"
                size="small"
                color="primary"
                variant="tonal"
                >Xử lý</v-btn
              >
              <v-btn
                v-if="r.status === 'Đang xử lý'"
                size="small"
                color="success"
                variant="tonal"
                >Hoàn thành</v-btn
              >
              <v-btn icon="mdi-eye-outline" size="small" variant="text" />
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="dialog" max-width="520">
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">Tạo yêu cầu bảo trì</h2>
        <v-select
          label="Phòng"
          :items="['101-A1', '102-A1', '201-B1', '301-C1']"
          class="mb-3"
        />
        <v-text-field
          label="Tiêu đề"
          placeholder="VD: Hỏng vòi nước"
          class="mb-3"
        />
        <v-select
          label="Mức độ ưu tiên"
          :items="['Bình thường', 'Khẩn cấp']"
          class="mb-3"
        />
        <v-textarea label="Mô tả chi tiết" rows="3" />
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="dialog = false">Hủy</v-btn>
          <v-btn color="primary" @click="dialog = false">Gửi yêu cầu</v-btn>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import maintenanceRequestService from "@/services/maintenanceRequestService";

const tab = ref("all");
const dialog = ref(false);

const studentMap = {
  "30000000-0000-0000-0000-000000000001": "Nguyễn Văn A",
  "30000000-0000-0000-0000-000000000002": "Trần Thị B",
  "30000000-0000-0000-0000-000000000003": "Lê Văn C",
};

const roomMap = {
  "40000000-0000-0000-0000-000000000101": "A101",
  "40000000-0000-0000-0000-000000000102": "A102",
  "40000000-0000-0000-0000-000000000103": "A103",
};

const requestCode = (index) => `MT${String(index + 1).padStart(4, "0")}`;

const requests = ref([]);
onMounted(async () => {
  try {
    const res = await maintenanceRequestService.getAll();
    requests.value = res.map((r, index) => ({
      id: r.id,
      code: requestCode(index),
      title: r.description?.slice(0, 30) || "Yêu cầu bảo trì",
      room: roomMap[r.roomId] || r.roomId?.slice(0, 8) || "Không xác định",
      student:
        studentMap[r.studentId] || r.studentId?.slice(0, 8) || "Không xác định",
      date: new Date(r.createdAt).toLocaleDateString("vi-VN"),
      status:
        r.status === "Completed"
          ? "Hoàn thành"
          : r.status === "InProgress"
            ? "Đang xử lý"
            : "Chờ xử lý",
      priority: r.status === "Pending" ? "Khẩn cấp" : "Bình thường",
      desc: r.description,
      note: r.note || "",
    }));
  } catch (error) {
    console.error("Lỗi tải bảo trì:", error);
    requests.value = [];
  }
});

const filtered = computed(() => {
  if (tab.value === "all") return requests.value;
  const m = {
    pending: "Chờ xử lý",
    progress: "Đang xử lý",
    done: "Hoàn thành",
  };
  return requests.value.filter((r) => r.status === m[tab.value]);
});

const stColor = (s) =>
  ({ "Chờ xử lý": "warning", "Đang xử lý": "info", "Hoàn thành": "success" })[
    s
  ] || "grey";
const priColor = (p) => (p === "Khẩn cấp" ? "error" : "info");
</script>
