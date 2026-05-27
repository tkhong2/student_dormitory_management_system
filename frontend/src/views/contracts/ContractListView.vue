<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Hợp đồng thuê phòng
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Quản lý hợp đồng KTX của sinh viên
        </p>
      </div>
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        variant="flat"
        @click="openCreate"
        >Tạo hợp đồng</v-btn
      >
    </div>

    <v-row class="mb-4">
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#ede9fe" size="40" rounded="lg"
              ><v-icon color="primary" size="20"
                >mdi-file-document-multiple</v-icon
              ></v-avatar
            >
            <div>
              <div class="text-h6 font-weight-bold">{{ contracts.length }}</div>
              <div class="text-caption text-medium-emphasis">Tổng hợp đồng</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#dcfce7" size="40" rounded="lg"
              ><v-icon color="success" size="20"
                >mdi-check-circle</v-icon
              ></v-avatar
            >
            <div>
              <div class="text-h6 font-weight-bold">
                {{ countStatus("Hiệu lực") }}
              </div>
              <div class="text-caption text-medium-emphasis">Đang hiệu lực</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fff7ed" size="40" rounded="lg"
              ><v-icon color="warning" size="20"
                >mdi-clock-alert</v-icon
              ></v-avatar
            >
            <div>
              <div class="text-h6 font-weight-bold">
                {{ countStatus("Sắp hết hạn") }}
              </div>
              <div class="text-caption text-medium-emphasis">Sắp hết hạn</div>
            </div>
          </div>
        </v-card>
      </v-col>
      <v-col cols="6" md="3">
        <v-card class="pa-4" style="border: 1px solid #e5e7eb">
          <div class="d-flex align-center ga-3">
            <v-avatar color="#fef2f2" size="40" rounded="lg"
              ><v-icon color="error" size="20">mdi-cancel</v-icon></v-avatar
            >
            <div>
              <div class="text-h6 font-weight-bold">
                {{ countStatus("Đã chấm dứt") }}
              </div>
              <div class="text-caption text-medium-emphasis">Đã chấm dứt</div>
            </div>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="contracts"
      :treatEmptyAsError="false"
      @retry="loadContracts"
    >
      <v-card style="border: 1px solid #e5e7eb">
        <v-data-table :headers="headers" :items="contracts" items-per-page="10">
          <template #item.studentName="{ item }">
            <div class="font-weight-medium">{{ item.studentName }}</div>
            <div class="text-caption text-medium-emphasis">
              {{ item.studentCode }}
            </div>
          </template>
          <template #item.price="{ item }"
            ><span class="font-weight-bold">{{
              formatPrice(item.price)
            }}</span></template
          >
          <template #item.startDate="{ item }">{{
            formatDate(item.startDate)
          }}</template>
          <template #item.endDate="{ item }">{{
            formatDate(item.endDate)
          }}</template>
          <template #item.status="{ item }">
            <v-chip
              :color="statusColor(item.displayStatus)"
              size="x-small"
              variant="tonal"
              >{{ item.displayStatus }}</v-chip
            >
          </template>
          <template #item.actions="{ item }">
            <v-btn
              icon="mdi-pencil-outline"
              size="small"
              variant="text"
              color="primary"
              title="Chỉnh sửa"
              @click="openEdit(item)"
            />
            <v-btn
              icon="mdi-close-circle-outline"
              size="small"
              variant="text"
              color="error"
              title="Chấm dứt"
              :disabled="item.status === 'Terminated'"
              @click="terminateContract(item)"
            />
            <v-btn
              icon="mdi-delete-outline"
              size="small"
              variant="text"
              color="error"
              title="Xóa"
              @click="confirmDelete(item)"
            />
          </template>
        </v-data-table>
      </v-card>
    </DataStatus>

    <v-dialog v-model="dialog" max-width="680" persistent>
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">
          {{ editTarget ? "Chỉnh sửa hợp đồng" : "Tạo hợp đồng mới" }}
        </h2>
        <v-row>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.code"
              label="Mã hợp đồng *"
              :error-messages="formErrors.code"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.studentCode"
              label="Mã sinh viên *"
              :error-messages="formErrors.studentCode"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.studentName"
              label="Tên sinh viên *"
              :error-messages="formErrors.studentName"
          /></v-col>
          <v-col cols="12" sm="6"
            ><v-text-field
              v-model="form.roomNumber"
              label="Phòng *"
              :error-messages="formErrors.roomNumber"
          /></v-col>
          <v-col cols="12" sm="6"
            ><v-text-field
              v-model.number="form.price"
              label="Giá thuê *"
              suffix="đ/tháng"
              type="number"
              :error-messages="formErrors.price"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.startDate"
              label="Ngày bắt đầu *"
              type="date"
              :error-messages="formErrors.startDate"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.endDate"
              label="Ngày kết thúc *"
              type="date"
              :error-messages="formErrors.endDate"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-select
              v-model="form.status"
              label="Trạng thái"
              :items="statusOptions"
              item-title="label"
              item-value="value"
          /></v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="closeDialog"
            >Hủy</v-btn
          >
          <v-btn color="primary" :loading="saving" @click="saveContract">{{
            editTarget ? "Lưu thay đổi" : "Tạo hợp đồng"
          }}</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <v-dialog v-model="deleteDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-3">Xác nhận xóa</h2>
        <p>
          Bạn có chắc muốn xóa hợp đồng <strong>{{ deleteTarget?.code }}</strong
          >?
        </p>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" :loading="saving" @click="deleteContract"
            >Xóa</v-btn
          >
        </div>
      </v-card>
    </v-dialog>

    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      location="bottom right"
      >{{ snackbar.message }}</v-snackbar
    >
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import DataStatus from "@/components/common/DataStatus.vue";
import { contractService } from "@/services/contractService";

const headers = [
  { title: "Mã HĐ", key: "code" },
  { title: "Sinh viên", key: "studentName" },
  { title: "Phòng", key: "roomNumber", align: "center" },
  { title: "Giá thuê", key: "price", align: "end" },
  { title: "Bắt đầu", key: "startDate", align: "center" },
  { title: "Kết thúc", key: "endDate", align: "center" },
  { title: "Trạng thái", key: "status", align: "center" },
  { title: "", key: "actions", align: "end", sortable: false },
];
const statusOptions = [
  { label: "Hiệu lực", value: "Active" },
  { label: "Hết hạn", value: "Expired" },
  { label: "Đã chấm dứt", value: "Terminated" },
  { label: "Đã gia hạn", value: "Renewed" },
];
const contracts = ref([]);
const loading = ref(false);
const saving = ref(false);
const error = ref(null);
const dialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);
const formErrors = ref({});
const form = ref(defaultForm());
const snackbar = ref({ show: false, message: "", color: "success" });

function defaultForm() {
  return {
    code: "",
    studentId: "",
    studentCode: "",
    studentName: "",
    roomId: "",
    roomNumber: "",
    price: 0,
    startDate: "",
    endDate: "",
    status: "Active",
  };
}

async function loadContracts() {
  loading.value = true;
  error.value = null;
  try {
    contracts.value = (await contractService.getAll()).map((item) => ({
      ...item,
      displayStatus: displayStatus(item),
    }));
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách hợp đồng";
  } finally {
    loading.value = false;
  }
}

function displayStatus(item) {
  if (item.status === "Terminated") return "Đã chấm dứt";
  if (item.status === "Expired") return "Hết hạn";
  if (item.status === "Renewed") return "Đã gia hạn";
  const daysRemaining = Math.ceil(
    (new Date(item.endDate) - new Date()) / 86400000,
  );
  return daysRemaining >= 0 && daysRemaining <= 30 ? "Sắp hết hạn" : "Hiệu lực";
}

function countStatus(status) {
  return contracts.value.filter((item) => item.displayStatus === status).length;
}

function statusColor(status) {
  return (
    {
      "Hiệu lực": "success",
      "Sắp hết hạn": "warning",
      "Đã gia hạn": "info",
      "Hết hạn": "grey",
      "Đã chấm dứt": "error",
    }[status] || "grey"
  );
}

function formatPrice(value) {
  return `${Number(value).toLocaleString("vi-VN")}đ`;
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString("vi-VN") : "";
}

function openCreate() {
  editTarget.value = null;
  form.value = defaultForm();
  formErrors.value = {};
  dialog.value = true;
}

function openEdit(item) {
  editTarget.value = item;
  form.value = {
    ...item,
    startDate: item.startDate.slice(0, 10),
    endDate: item.endDate.slice(0, 10),
  };
  formErrors.value = {};
  dialog.value = true;
}

function closeDialog() {
  dialog.value = false;
  editTarget.value = null;
}

function validate() {
  const errors = {};
  if (!form.value.code.trim()) errors.code = "Vui lòng nhập mã hợp đồng";
  if (!form.value.studentCode.trim())
    errors.studentCode = "Vui lòng nhập mã sinh viên";
  if (!form.value.studentName.trim())
    errors.studentName = "Vui lòng nhập tên sinh viên";
  if (!form.value.roomNumber.trim()) errors.roomNumber = "Vui lòng nhập phòng";
  if (form.value.price < 0) errors.price = "Giá thuê không hợp lệ";
  if (!form.value.startDate) errors.startDate = "Vui lòng nhập ngày bắt đầu";
  if (!form.value.endDate || form.value.endDate < form.value.startDate)
    errors.endDate = "Ngày kết thúc không hợp lệ";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

function payload(item = form.value) {
  return {
    code: item.code.trim(),
    studentId: item.studentId || crypto.randomUUID(),
    studentCode: item.studentCode.trim(),
    studentName: item.studentName.trim(),
    roomId: item.roomId || crypto.randomUUID(),
    roomNumber: item.roomNumber.trim(),
    price: Number(item.price),
    startDate: item.startDate,
    endDate: item.endDate,
    status: item.status,
  };
}

async function saveContract() {
  if (!validate()) return;
  saving.value = true;
  try {
    if (editTarget.value) {
      await contractService.update(editTarget.value.id, payload());
      notify("Cập nhật hợp đồng thành công");
    } else {
      await contractService.create(payload());
      notify("Tạo hợp đồng thành công");
    }
    closeDialog();
    await loadContracts();
  } catch (err) {
    notify(err.message || "Có lỗi xảy ra", "error");
  } finally {
    saving.value = false;
  }
}

async function terminateContract(item) {
  saving.value = true;
  try {
    await contractService.update(
      item.id,
      payload({ ...item, status: "Terminated" }),
    );
    notify("Đã chấm dứt hợp đồng");
    await loadContracts();
  } catch (err) {
    notify(err.message || "Không thể chấm dứt hợp đồng", "error");
  } finally {
    saving.value = false;
  }
}

function confirmDelete(item) {
  deleteTarget.value = item;
  deleteDialog.value = true;
}

async function deleteContract() {
  saving.value = true;
  try {
    await contractService.delete(deleteTarget.value.id);
    deleteDialog.value = false;
    notify("Đã xóa hợp đồng");
    await loadContracts();
  } catch (err) {
    notify(err.message || "Không thể xóa hợp đồng", "error");
  } finally {
    saving.value = false;
  }
}

function notify(message, color = "success") {
  snackbar.value = { show: true, message, color };
}

onMounted(loadContracts);
</script>
