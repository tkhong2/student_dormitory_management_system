<template>
  <div>
    <div style="margin-bottom: 16px">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0">
        Hóa đơn & Thanh toán
      </h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
        Quản lý thu phí ký túc xá
      </p>
    </div>

    <!-- Revenue Summary -->
    <v-row style="margin-bottom: 16px">
      <v-col cols="12" md="4">
        <v-card class="pa-5 gradient-primary">
          <div class="text-body-2 mb-1" style="opacity: 0.7">
            Tổng thu tháng này
          </div>
          <div class="text-h4 font-weight-bold">{{ fmt(totalRevenue) }}</div>
          <div class="text-caption mt-2" style="opacity: 0.7">
            <v-icon size="14">mdi-trending-up</v-icon> +12% so với tháng trước
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card class="pa-5" style="border: 2px solid #f59e0b">
          <div class="text-body-2 text-medium-emphasis mb-1">
            Chưa thanh toán
          </div>
          <div class="text-h4 font-weight-bold text-warning">
            {{ fmt(unpaidAmount) }}
          </div>
          <div class="text-caption text-medium-emphasis mt-2">
            {{ unpaidCount }} hóa đơn đang chờ
          </div>
        </v-card>
      </v-col>
      <v-col cols="12" md="4">
        <v-card class="pa-5" style="border: 2px solid #dc2626">
          <div class="text-body-2 text-medium-emphasis mb-1">Quá hạn</div>
          <div class="text-h4 font-weight-bold text-error">
            {{ fmt(overdueAmount) }}
          </div>
          <div class="text-caption text-medium-emphasis mt-2">
            {{ overdueCount }} hóa đơn quá hạn
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-card style="border: 1px solid #e5e7eb">
      <div class="pa-4 d-flex flex-wrap ga-3 align-center">
        <v-text-field
          v-model="search"
          placeholder="Tìm mã HĐ, sinh viên..."
          prepend-inner-icon="mdi-magnify"
          density="compact"
          hide-details
          style="max-width: 300px"
        />
        <v-select
          label="Tháng"
          :items="months"
          density="compact"
          hide-details
          style="max-width: 180px"
        />
        <v-chip-group class="ml-auto">
          <v-chip filter value="all">Tất cả</v-chip>
          <v-chip filter value="unpaid" color="warning">Chưa TT</v-chip>
          <v-chip filter value="overdue" color="error">Quá hạn</v-chip>
        </v-chip-group>
      </div>

      <v-data-table
        :headers="headers"
        :items="bills"
        :search="search"
        items-per-page="10"
      >
        <template #item.description="{ item }">
          <span class="text-caption" style="white-space: normal">{{
            item.description
          }}</span>
        </template>
        <template #item.amount="{ item }">
          <span class="font-weight-bold">{{ fmt(item.amount) }}</span>
        </template>
        <template #item.status="{ item }">
          <v-chip :color="sColor(item.status)" size="x-small" variant="tonal">{{
            item.status
          }}</v-chip>
        </template>
        <template #item.actions="{ item }">
          <v-btn
            v-if="item.status !== 'Đã TT'"
            size="small"
            color="success"
            variant="tonal"
            class="mr-1"
            prepend-icon="mdi-cash-check"
            >Thu tiền</v-btn
          >
          <v-btn icon="mdi-printer" size="small" variant="text" />
          <v-btn
            icon="mdi-eye-outline"
            size="small"
            variant="text"
            color="primary"
          />
        </template>
      </v-data-table>
    </v-card>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
const search = ref("");
const months = ["Tháng 5/2026", "Tháng 4/2026", "Tháng 3/2026", "Tháng 2/2026"];
const headers = [
  { title: "Mã HĐ", key: "code" },
  { title: "Sinh viên", key: "student" },
  { title: "Phòng", key: "room", align: "center" },
  { title: "Mô tả", key: "description" },
  { title: "Số tiền", key: "amount", align: "end" },
  { title: "Hạn TT", key: "due", align: "center" },
  { title: "Trạng thái", key: "status", align: "center" },
  { title: "", key: "actions", align: "end", sortable: false },
];

const studentMap = {
  "30000000-0000-0000-0000-000000000001": "Nguyễn Văn A",
  "30000000-0000-0000-0000-000000000002": "Trần Thị B",
  "30000000-0000-0000-0000-000000000003": "Lê Văn C",
  "30000000-0000-0000-0000-000000000004": "Phạm Thị D",
  "30000000-0000-0000-0000-000000000005": "Hoàng Anh E",
  "30000000-0000-0000-0000-000000000006": "Ngô Minh F",
  "30000000-0000-0000-0000-000000000007": "Đỗ Thị G",
  "30000000-0000-0000-0000-000000000008": "Vũ Văn H",
  "30000000-0000-0000-0000-000000000009": "Bùi Thị I",
  "30000000-0000-0000-0000-000000000010": "Trương Văn K",
  "30000000-0000-0000-0000-000000000011": "Nguyễn Thị L",
  "30000000-0000-0000-0000-000000000012": "Lê Thị M",
  "30000000-0000-0000-0000-000000000013": "Phan Văn N",
  "30000000-0000-0000-0000-000000000014": "Nguyễn Hữu O",
  "30000000-0000-0000-0000-000000000015": "Trần Văn P",
  "30000000-0000-0000-0000-000000000016": "Lê Minh Q",
  "30000000-0000-0000-0000-000000000017": "Hoàng Thị R",
  "30000000-0000-0000-0000-000000000018": "Võ Văn S",
  "30000000-0000-0000-0000-000000000019": "Đặng Thị T",
};

const roomMap = {
  "40000000-0000-0000-0000-000000000101": "A101",
  "40000000-0000-0000-0000-000000000102": "A102",
  "40000000-0000-0000-0000-000000000103": "A103",
  "40000000-0000-0000-0000-000000000104": "A104",
  "40000000-0000-0000-0000-000000000105": "A105",
  "40000000-0000-0000-0000-000000000106": "A106",
  "40000000-0000-0000-0000-000000000107": "A107",
  "40000000-0000-0000-0000-000000000108": "A108",
  "40000000-0000-0000-0000-000000000109": "A109",
  "40000000-0000-0000-0000-000000000110": "A110",
  "40000000-0000-0000-0000-000000000111": "A111",
  "40000000-0000-0000-0000-000000000112": "A112",
  "40000000-0000-0000-0000-000000000201": "B201",
  "40000000-0000-0000-0000-000000000202": "B202",
  "40000000-0000-0000-0000-000000000203": "B203",
  "40000000-0000-0000-0000-000000000301": "C301",
  "40000000-0000-0000-0000-000000000302": "C302",
  "40000000-0000-0000-0000-000000000303": "C303",
  "40000000-0000-0000-0000-000000000304": "C304",
};

import billService from "@/services/billService";
const bills = ref([]);

onMounted(async () => {
  try {
    const loaded = await billService.getAll();
    bills.value = loaded.map((b, index) => {
      const roomId = b.RoomId || b.roomId || "";
      const studentId = b.StudentId || b.studentId || "";
      const roomKey =
        typeof roomId === "string" ? roomId.toLowerCase() : roomId;
      const studentKey =
        typeof studentId === "string" ? studentId.toLowerCase() : studentId;
      return {
        id: b.id,
        code: `HD${String(index + 1).padStart(4, "0")}`,
        student:
          studentMap[studentId] ||
          studentMap[studentKey] ||
          (typeof studentId === "string" ? studentId.slice(0, 8) : "") ||
          "Không xác định",
        room:
          roomMap[roomId] ||
          roomMap[roomKey] ||
          (typeof roomId === "string"
            ? roomId.slice(-4).toUpperCase()
            : "Không xác định"),
        description: b.description || "Hóa đơn tiền phòng",
        amount: b.amount,
        due: new Date(b.dueDate).toLocaleDateString("vi-VN"),
        status:
          b.status === "Paid"
            ? "Đã TT"
            : b.status === "Overdue"
              ? "Quá hạn"
              : "Chưa TT",
      };
    });
  } catch (error) {
    console.error("Lỗi tải hóa đơn:", error);
    bills.value = [];
  }
});

const totalRevenue = computed(() =>
  bills.value
    .filter((b) => b.status === "Đã TT")
    .reduce((sum, b) => sum + b.amount, 0),
);
const unpaidAmount = computed(() =>
  bills.value
    .filter((b) => b.status === "Chưa TT")
    .reduce((sum, b) => sum + b.amount, 0),
);
const overdueAmount = computed(() =>
  bills.value
    .filter((b) => b.status === "Quá hạn")
    .reduce((sum, b) => sum + b.amount, 0),
);
const unpaidCount = computed(
  () => bills.value.filter((b) => b.status === "Chưa TT").length,
);
const overdueCount = computed(
  () => bills.value.filter((b) => b.status === "Quá hạn").length,
);

const fmt = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(
    v,
  );
const sColor = (s) =>
  ({ "Đã TT": "success", "Chưa TT": "warning", "Quá hạn": "error" })[s] ||
  "grey";
</script>
