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

    <DataStatus
      :loading="loading"
      :error="error"
      :items="bills"
      @retry="loadData"
    >
      <template #default>
        <a-card
          style="border: 1px solid #e5e7eb"
          :body-style="{ padding: '0' }"
        >
          <div class="pa-4 d-flex flex-wrap align-center" style="gap: 12px">
            <a-input-search
              v-model:value="search"
              placeholder="Tìm mã HĐ, sinh viên..."
              allowClear
              style="max-width: 300px; flex: 1"
            />
            <a-select
              v-model:value="monthFilter"
              placeholder="Tháng"
              allowClear
              style="max-width: 180px"
            >
              <a-select-option value="all">Tất cả</a-select-option>
              <a-select-option
                v-for="month in months"
                :key="month"
                :value="month"
              >
                {{ month }}
              </a-select-option>
            </a-select>
            <a-segmented
              v-model:value="statusFilter"
              :options="statusFilterOptions"
              class="ml-auto"
            />
          </div>

          <a-table
            :columns="billingColumns"
            :data-source="filteredBills"
            row-key="id"
            :pagination="{ pageSize: 10 }"
            style="width: 100%"
          >
            <template #bodyCell_amount="{ record }">
              <span class="font-weight-bold">{{ fmt(record.amount) }}</span>
            </template>
            <template #bodyCell_status="{ record }">
              <a-tag :color="sColor(record.status)">{{ record.status }}</a-tag>
            </template>
            <template #bodyCell_actions="{ record }">
              <a-space size="small">
                <a-button
                  v-if="record.status !== 'Đã TT'"
                  type="primary"
                  size="small"
                  @click="payBill(record)"
                >
                  Thu tiền
                </a-button>
                <a-button type="text" size="small">In</a-button>
                <a-button type="text" size="small">Xem</a-button>
              </a-space>
            </template>
          </a-table>
        </a-card>
      </template>
    </DataStatus>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import DataStatus from "@/components/common/DataStatus.vue";
const search = ref("");
const monthFilter = ref("all");
const statusFilter = ref("all");
const months = ["Tháng 5/2026", "Tháng 4/2026", "Tháng 3/2026", "Tháng 2/2026"];
const statusFilterOptions = [
  { label: "Tất cả", value: "all" },
  { label: "Đã TT", value: "Đã TT" },
  { label: "Chưa TT", value: "Chưa TT" },
  { label: "Quá hạn", value: "Quá hạn" },
];
const billingColumns = [
  { title: "Mã HĐ", dataIndex: "code", key: "code" },
  { title: "Sinh viên", dataIndex: "student", key: "student" },
  { title: "Phòng", dataIndex: "room", key: "room", align: "center" },
  { title: "Mô tả", dataIndex: "description", key: "description" },
  { title: "Số tiền", dataIndex: "amount", key: "amount", align: "end" },
  { title: "Hạn TT", dataIndex: "due", key: "due", align: "center" },
  { title: "Trạng thái", dataIndex: "status", key: "status", align: "center" },
  {
    title: "",
    dataIndex: "actions",
    key: "actions",
    align: "center",
    width: 180,
  },
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
const loading = ref(false);
const error = ref(null);

async function loadData() {
  loading.value = true;
  error.value = null;
  try {
    const loaded = await billService.getAll();
    bills.value = loaded.map((b, index) => {
      const roomId = b.RoomId || b.roomId || "";
      const studentId = b.StudentId || b.studentId || "";
      const roomKey =
        typeof roomId === "string" ? roomId.toLowerCase() : roomId;
      const studentKey =
        typeof studentId === "string" ? studentId.toLowerCase() : studentId;
      const dueDate = new Date(b.dueDate);
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
        due: dueDate.toLocaleDateString("vi-VN"),
        dueDate: dueDate.toISOString(),
        monthLabel: `Tháng ${dueDate.getMonth() + 1}/${dueDate.getFullYear()}`,
        status:
          b.status === "Paid"
            ? "Đã TT"
            : b.status === "Overdue"
              ? "Quá hạn"
              : "Chưa TT",
      };
    });
    if (Array.isArray(bills.value) && bills.value.length === 0) {
      error.value = "Lỗi kết nối máy chủ";
    }
  } catch (err) {
    console.error("Lỗi tải hóa đơn:", err);
    bills.value = [];
    error.value = "Lỗi kết nối máy chủ";
  } finally {
    loading.value = false;
  }
}

onMounted(loadData);

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
const filteredBills = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  return bills.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.code.toLowerCase().includes(keyword) ||
      item.student.toLowerCase().includes(keyword) ||
      item.room.toLowerCase().includes(keyword);
    const matchesStatus =
      statusFilter.value === "all" || item.status === statusFilter.value;
    const matchesMonth =
      monthFilter.value === "all" || item.monthLabel === monthFilter.value;
    return matchesText && matchesStatus && matchesMonth;
  });
});

const unpaidCount = computed(
  () => bills.value.filter((b) => b.status === "Chưa TT").length,
);
const overdueCount = computed(
  () => bills.value.filter((b) => b.status === "Quá hạn").length,
);

function payBill(record) {
  console.log("Pay bill", record.id);
}

const fmt = (v) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(
    v,
  );
const sColor = (s) =>
  ({ "Đã TT": "success", "Chưa TT": "warning", "Quá hạn": "error" })[s] ||
  "grey";
</script>
