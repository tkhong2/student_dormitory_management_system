<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Sinh viên
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Tổng số: {{ students.length }} sinh viên nội trú
        </p>
      </div>
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        variant="flat"
        @click="openCreate"
        >Thêm sinh viên</v-btn
      >
    </div>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="students"
      :treatEmptyAsError="false"
      @retry="loadStudents"
    >
      <a-card
        style="border: 1px solid #e5e7eb; background: #fafafa"
        :body-style="{ padding: '0' }"
      >
        <div class="pa-4 d-flex flex-wrap align-center" style="gap: 12px">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên, mã SV..."
            allowClear
            style="max-width: 300px; flex: 1"
          />
          <a-select
            v-model:value="buildingFilter"
            placeholder="Tòa nhà"
            allowClear
            style="max-width: 180px"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option
              v-for="option in buildingOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
          <a-select
            v-model:value="classFilter"
            placeholder="Lớp"
            allowClear
            style="max-width: 180px"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option
              v-for="option in classOptions"
              :key="option"
              :value="option"
            >
              {{ option }}
            </a-select-option>
          </a-select>
          <a-select
            v-model:value="genderFilter"
            placeholder="Giới tính"
            allowClear
            style="max-width: 150px"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option value="Nam">Nam</a-select-option>
            <a-select-option value="Nữ">Nữ</a-select-option>
          </a-select>
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allowClear
            style="max-width: 220px"
          >
            <a-select-option
              v-for="option in filterStatusOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
          <a-button
            v-if="
              search ||
              classFilter !== 'all' ||
              genderFilter ||
              statusFilter !== 'all' ||
              buildingFilter !== 'all'
            "
            type="text"
            size="small"
            @click="resetFilters"
          >
            Đặt lại
          </a-button>
        </div>

        <a-table
          :columns="studentColumns"
          :data-source="filteredStudents"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell_fullName="{ record }">
            <div class="d-flex align-center" style="gap: 12px; padding: 12px 0">
              <a-avatar size="32" style="background: #e6f7ff; color: #1890ff">
                {{ record.fullName.charAt(0) }}
              </a-avatar>
              <div class="font-weight-bold">{{ record.fullName }}</div>
            </div>
          </template>
          <template #bodyCell_joinDate="{ record }">
            {{ formatDate(record.joinDate) }}
          </template>
          <template #bodyCell_status="{ record }">
            <a-tag :color="statusColor(record.status)">{{
              statusLabel(record.status)
            }}</a-tag>
          </template>
          <template #bodyCell_actions="{ record }">
            <a-space size="small">
              <a-button type="link" @click="openEdit(record)"
                >Chỉnh sửa</a-button
              >
              <a-button type="text" danger @click="confirmDelete(record)"
                >Xóa</a-button
              >
            </a-space>
          </template>
        </a-table>
      </a-card>
    </DataStatus>

    <v-dialog v-model="dialog" max-width="760" persistent>
      <v-card class="pa-6">
        <h2 class="text-h5 font-weight-bold mb-6">
          {{ editTarget ? "Chỉnh sửa sinh viên" : "Thêm sinh viên" }}
        </h2>
        <v-row>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.studentCode"
              label="Mã sinh viên *"
              :error-messages="formErrors.studentCode"
          /></v-col>
          <v-col cols="12" sm="8"
            ><v-text-field
              v-model="form.fullName"
              label="Họ tên *"
              :error-messages="formErrors.fullName"
          /></v-col>
          <v-col cols="12" sm="6"
            ><v-text-field v-model="form.email" label="Email"
          /></v-col>
          <v-col cols="12" sm="6"
            ><v-text-field v-model="form.phoneNumber" label="Số điện thoại"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.roomNumber"
              label="Phòng *"
              :error-messages="formErrors.roomNumber"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.buildingName"
              label="Tòa *"
              :error-messages="formErrors.buildingName"
          /></v-col>
          <v-col cols="12" sm="4"
            ><v-text-field
              v-model="form.className"
              label="Lớp *"
              :error-messages="formErrors.className"
          /></v-col>
          <v-col cols="12" sm="6"
            ><v-text-field
              v-model="form.joinDate"
              label="Ngày vào *"
              type="date"
              :error-messages="formErrors.joinDate"
          /></v-col>
          <v-col cols="12" sm="6"
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
          <v-btn color="primary" :loading="saving" @click="saveStudent">{{
            editTarget ? "Lưu thay đổi" : "Thêm sinh viên"
          }}</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <v-dialog v-model="deleteDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-3">Xác nhận xóa</h2>
        <p>
          Bạn có chắc muốn xóa sinh viên
          <strong>{{ deleteTarget?.fullName }}</strong
          >?
        </p>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" :loading="saving" @click="deleteStudent"
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
import { computed, onMounted, ref } from "vue";
import DataStatus from "@/components/common/DataStatus.vue";
import { studentService } from "@/services/studentService";

const studentColumns = [
  { title: "Sinh viên", dataIndex: "fullName", key: "fullName", width: 260 },
  { title: "Mã SV", dataIndex: "studentCode", key: "studentCode" },
  {
    title: "Phòng",
    dataIndex: "roomNumber",
    key: "roomNumber",
    align: "center",
  },
  {
    title: "Tòa",
    dataIndex: "buildingName",
    key: "buildingName",
    align: "center",
  },
  { title: "Lớp", dataIndex: "className", key: "className" },
  {
    title: "Ngày vào",
    dataIndex: "joinDate",
    key: "joinDate",
    align: "center",
  },
  { title: "Trạng thái", dataIndex: "status", key: "status", align: "center" },
  {
    title: "Thao tác",
    dataIndex: "actions",
    key: "actions",
    align: "center",
    width: 160,
  },
];
const statusOptions = [
  { label: "Đang ở", value: "Active" },
  { label: "Sắp hết hạn", value: "Expiring" },
  { label: "Đã rời đi", value: "Departed" },
];
const filterStatusOptions = [
  { label: "Tất cả", value: "all" },
  ...statusOptions,
];
const students = ref([]);
const search = ref("");
const buildingFilter = ref("all");
const statusFilter = ref("all");
const classFilter = ref("all");
const genderFilter = ref("");
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

const buildingOptions = computed(() => [
  { label: "Tất cả", value: "all" },
  ...[...new Set(students.value.map((item) => item.buildingName))]
    .filter(Boolean)
    .map((name) => ({ label: `Tòa ${name}`, value: name })),
]);

const classOptions = computed(() =>
  [...new Set(students.value.map((item) => item.className))]
    .filter(Boolean)
    .sort(),
);

const filteredStudents = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  return students.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.fullName.toLowerCase().includes(keyword) ||
      item.studentCode.toLowerCase().includes(keyword);
    const matchesBuilding =
      buildingFilter.value === "all" ||
      item.buildingName === buildingFilter.value;
    const matchesStatus =
      statusFilter.value === "all" || item.status === statusFilter.value;
    const matchesClass =
      classFilter.value === "all" || item.className === classFilter.value;
    const matchesGender =
      !genderFilter.value || item.gender === genderFilter.value;
    return (
      matchesText &&
      matchesBuilding &&
      matchesStatus &&
      matchesClass &&
      matchesGender
    );
  });
});

function resetFilters() {
  search.value = "";
  buildingFilter.value = "all";
  statusFilter.value = "all";
  classFilter.value = "all";
  genderFilter.value = "";
}

function defaultForm() {
  return {
    studentCode: "",
    fullName: "",
    email: "",
    phoneNumber: "",
    address: "",
    dateOfBirth: null,
    gender: "",
    roomNumber: "",
    buildingName: "",
    className: "",
    joinDate: "",
    status: "Active",
  };
}

async function loadStudents() {
  loading.value = true;
  error.value = null;
  try {
    students.value = await studentService.getAll();
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách sinh viên";
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  editTarget.value = null;
  form.value = defaultForm();
  formErrors.value = {};
  dialog.value = true;
}

function openEdit(item) {
  editTarget.value = item;
  form.value = { ...item, joinDate: item.joinDate.slice(0, 10) };
  formErrors.value = {};
  dialog.value = true;
}

function closeDialog() {
  dialog.value = false;
  editTarget.value = null;
}

function validate() {
  const errors = {};
  if (!form.value.studentCode.trim())
    errors.studentCode = "Vui lòng nhập mã sinh viên";
  if (!form.value.fullName.trim()) errors.fullName = "Vui lòng nhập họ tên";
  if (!form.value.roomNumber.trim()) errors.roomNumber = "Vui lòng nhập phòng";
  if (!form.value.buildingName.trim())
    errors.buildingName = "Vui lòng nhập tòa";
  if (!form.value.className.trim()) errors.className = "Vui lòng nhập lớp";
  if (!form.value.joinDate) errors.joinDate = "Vui lòng nhập ngày vào";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

function payload() {
  return {
    ...form.value,
    studentCode: form.value.studentCode.trim(),
    fullName: form.value.fullName.trim(),
    email: form.value.email.trim(),
    phoneNumber: form.value.phoneNumber.trim(),
    roomNumber: form.value.roomNumber.trim(),
    buildingName: form.value.buildingName.trim(),
    className: form.value.className.trim(),
  };
}

async function saveStudent() {
  if (!validate()) return;
  saving.value = true;
  try {
    if (editTarget.value) {
      await studentService.update(editTarget.value.id, payload());
      notify("Cập nhật sinh viên thành công");
    } else {
      await studentService.create(payload());
      notify("Thêm sinh viên thành công");
    }
    closeDialog();
    await loadStudents();
  } catch (err) {
    notify(err.message || "Có lỗi xảy ra", "error");
  } finally {
    saving.value = false;
  }
}

function confirmDelete(item) {
  deleteTarget.value = item;
  deleteDialog.value = true;
}

async function deleteStudent() {
  saving.value = true;
  try {
    await studentService.delete(deleteTarget.value.id);
    deleteDialog.value = false;
    notify("Đã xóa sinh viên");
    await loadStudents();
  } catch (err) {
    notify(err.message || "Không thể xóa sinh viên", "error");
  } finally {
    saving.value = false;
  }
}

function statusLabel(status) {
  return statusOptions.find((item) => item.value === status)?.label || status;
}

function statusColor(status) {
  return (
    { Active: "success", Expiring: "warning", Departed: "grey" }[status] ||
    "grey"
  );
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString("vi-VN") : "";
}

function notify(message, color = "success") {
  snackbar.value = { show: false, message, color };
  snackbar.value.show = true;
}

onMounted(loadStudents);
</script>

<style scoped>
:deep(.v-data-table__th) {
  font-weight: 800 !important;
  text-transform: uppercase;
  font-size: 11px !important;
  letter-spacing: 0.5px;
  color: #64748b !important;
}
</style>
