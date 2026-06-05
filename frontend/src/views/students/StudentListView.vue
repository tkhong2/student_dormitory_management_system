<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Sinh viên
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Tổng số: {{ students.length }} sinh viên nội trú
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm sinh viên
      </a-button>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="6">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên, mã SV..."
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
          <a-select
            v-model:value="buildingFilter"
            placeholder="Tòa nhà"
            allow-clear
            style="width: 100%"
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
        </a-col>
        <a-col :xs="24" :sm="12" :md="4">
          <a-select
            v-model:value="classFilter"
            placeholder="Lớp"
            allow-clear
            style="width: 100%"
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
        </a-col>
        <a-col :xs="24" :sm="12" :md="4">
          <a-select
            v-model:value="genderFilter"
            placeholder="Giới tính"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option value="Nam">Nam</a-select-option>
            <a-select-option value="Nữ">Nữ</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="4">
          <a-select
            v-model:value="statusFilter"
            placeholder="Trạng thái"
            allow-clear
            style="width: 100%"
          >
            <a-select-option
              v-for="option in filterStatusOptions"
              :key="option.value"
              :value="option.value"
            >
              {{ option.label }}
            </a-select-option>
          </a-select>
        </a-col>
      </a-row>
    </a-card>

    <!-- Table Card -->
    <a-card :bordered="false" :loading="loading">
      <a-table
          :columns="studentColumns"
          :data-source="filteredStudents"
          row-key="id"
          :pagination="{ pageSize: 10 }"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'fullName'">
              <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
                <a-avatar size="32" style="background: #e6f7ff; color: #1890ff">
                  {{ record.fullName.charAt(0) }}
                </a-avatar>
                <div style="font-weight: 600">{{ record.fullName }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'joinDate'">
              {{ formatDate(record.joinDate) }}
            </template>
            <template v-else-if="column.key === 'status'">
              <a-tag :color="statusColor(record.status)">{{
                statusLabel(record.status)
              }}</a-tag>
            </template>
            <template v-else-if="column.key === 'actions'">
              <a-space size="small">
                <a-button type="link" @click="openEdit(record)"
                  >Chỉnh sửa</a-button
                >
                <a-button type="link" danger @click="confirmDelete(record)"
                  >Xóa</a-button
                >
              </a-space>
            </template>
          </template>
        </a-table>
      </a-card>

    <!-- Create/Edit Modal -->
    <a-modal
      v-model:open="dialog"
      :title="editTarget ? 'Chỉnh sửa sinh viên' : 'Thêm sinh viên'"
      width="760px"
      @ok="saveStudent"
      @cancel="closeDialog"
      :confirmLoading="saving"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form layout="vertical" style="margin-top: 24px">
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="Mã sinh viên" required :validate-status="formErrors.studentCode ? 'error' : ''" :help="formErrors.studentCode">
              <a-input v-model:value="form.studentCode" />
            </a-form-item>
          </a-col>
          <a-col :span="16">
            <a-form-item label="Họ tên" required :validate-status="formErrors.fullName ? 'error' : ''" :help="formErrors.fullName">
              <a-input v-model:value="form.fullName" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Email">
              <a-input v-model:value="form.email" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số điện thoại">
              <a-input v-model:value="form.phoneNumber" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="Phòng" required :validate-status="formErrors.roomNumber ? 'error' : ''" :help="formErrors.roomNumber">
              <a-input v-model:value="form.roomNumber" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Tòa" required :validate-status="formErrors.buildingName ? 'error' : ''" :help="formErrors.buildingName">
              <a-input v-model:value="form.buildingName" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Lớp" required :validate-status="formErrors.className ? 'error' : ''" :help="formErrors.className">
              <a-input v-model:value="form.className" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Ngày vào" required :validate-status="formErrors.joinDate ? 'error' : ''" :help="formErrors.joinDate">
              <a-date-picker v-model:value="form.joinDate" style="width: 100%" format="YYYY-MM-DD" valueFormat="YYYY-MM-DD" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Trạng thái">
              <a-select v-model:value="form.status">
                <a-select-option v-for="option in statusOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>

    <!-- Delete Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      @ok="deleteStudent"
      @cancel="deleteDialog = false"
      :confirmLoading="saving"
      okText="Xóa"
      cancelText="Hủy"
      okType="danger"
    >
      <p>
        Bạn có chắc muốn xóa sinh viên
        <strong>{{ deleteTarget?.fullName }}</strong>?
      </p>
    </a-modal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from "vue";
import { message } from 'ant-design-vue';
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
const dialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);
const formErrors = ref({});
const form = ref(defaultForm());

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
  try {
    students.value = await studentService.getAll();
  } catch (err) {
    message.error(err.message || "Không thể tải danh sách sinh viên");
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
      message.success("Cập nhật sinh viên thành công");
    } else {
      await studentService.create(payload());
      message.success("Thêm sinh viên thành công");
    }
    closeDialog();
    await loadStudents();
  } catch (err) {
    message.error(err.message || "Có lỗi xảy ra");
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
    message.success("Đã xóa sinh viên");
    await loadStudents();
  } catch (err) {
    message.error(err.message || "Không thể xóa sinh viên");
  } finally {
    saving.value = false;
  }
}

function statusLabel(status) {
  return statusOptions.find((item) => item.value === status)?.label || status;
}

function statusColor(status) {
  return (
    { Active: "success", Expiring: "warning", Departed: "default" }[status] ||
    "default"
  );
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString("vi-VN") : "";
}

onMounted(loadStudents);
</script>
