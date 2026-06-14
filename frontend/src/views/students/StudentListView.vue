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
      <a-space>
        <a-button @click="handleExport" :loading="exporting">
          <template #icon><DownloadOutlined /></template>
          Xuất Excel
        </a-button>
        <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
          + Thêm sinh viên
        </a-button>
      </a-space>
    </div>

    <!-- Filters Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="[16, 16]">
        <a-col :xs="24" :sm="12" :md="8">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên, mã SV..."
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="12" :md="6">
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
        <a-col :xs="24" :sm="12" :md="5">
          <a-select
            v-model:value="genderFilter"
            placeholder="Giới tính"
            allow-clear
            style="width: 100%"
          >
            <a-select-option value="">Tất cả</a-select-option>
            <a-select-option value="Male">Nam</a-select-option>
            <a-select-option value="Female">Nữ</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="12" :md="5">
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
          :scroll="{ x: 1200 }"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'fullName'">
              <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
                <a-avatar 
                  :src="record.avatarUrl" 
                  :size="32" 
                  :style="{ 
                    background: record.avatarUrl ? 'transparent' : '#e6f7ff', 
                    color: '#1890ff' 
                  }"
                >
                  <template v-if="!record.avatarUrl">
                    {{ record.fullName.charAt(0) }}
                  </template>
                </a-avatar>
                <div style="font-weight: 600">{{ record.fullName }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'joinDate'">
              {{ formatDate(record.joinDate) }}
            </template>
            <template v-else-if="column.key === 'isActive'">
              <a-tag :color="record.isActive ? 'success' : 'default'">
                {{ record.isActive ? 'Hoạt động' : 'Không hoạt động' }}
              </a-tag>
            </template>
            <template v-else-if="column.key === 'actions'">
              <a-space size="small">
                <a-button type="link" @click="openView(record)"
                  >Xem</a-button
                >
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
          <a-col :span="8">
            <a-form-item label="Giới tính">
              <a-select v-model:value="form.gender">
                <a-select-option value="Male">Nam</a-select-option>
                <a-select-option value="Female">Nữ</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Ngày sinh">
              <a-date-picker v-model:value="form.dateOfBirth" style="width: 100%" format="YYYY-MM-DD" valueFormat="YYYY-MM-DD" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Năm học">
              <a-input-number v-model:value="form.academicYear" style="width: 100%" :min="1" :max="6" />
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
              <a-input v-model:value="form.phone" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="Lớp">
              <a-input v-model:value="form.classCode" placeholder="VD: K65-CNTT" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Khoa" required :validate-status="formErrors.faculty ? 'error' : ''" :help="formErrors.faculty">
              <a-input v-model:value="form.faculty" placeholder="VD: Công nghệ thông tin" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Ngành">
              <a-input v-model:value="form.major" placeholder="VD: Khoa học máy tính" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="CMND/CCCD" required :validate-status="formErrors.identityCard ? 'error' : ''" :help="formErrors.identityCard">
              <a-input v-model:value="form.identityCard" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Ngày cấp">
              <a-date-picker v-model:value="form.identityCardIssuedDate" style="width: 100%" format="YYYY-MM-DD" valueFormat="YYYY-MM-DD" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Nơi cấp">
              <a-input v-model:value="form.identityCardIssuedPlace" placeholder="VD: Công an TP Hà Nội" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Địa chỉ thường trú" required :validate-status="formErrors.permanentAddress ? 'error' : ''" :help="formErrors.permanentAddress">
              <a-input v-model:value="form.permanentAddress" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-divider orientation="left">Người liên hệ khẩn cấp</a-divider>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Họ tên" required :validate-status="formErrors.emergencyContactName ? 'error' : ''" :help="formErrors.emergencyContactName">
              <a-input v-model:value="form.emergencyContactName" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số điện thoại" required :validate-status="formErrors.emergencyContactPhone ? 'error' : ''" :help="formErrors.emergencyContactPhone">
              <a-input v-model:value="form.emergencyContactPhone" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Quan hệ">
              <a-input v-model:value="form.emergencyContactRelation" placeholder="VD: Cha/Mẹ" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Trạng thái">
              <a-select v-model:value="form.isActive">
                <a-select-option :value="true">Hoạt động</a-select-option>
                <a-select-option :value="false">Không hoạt động</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>

    <!-- View Modal -->
    <a-modal
      v-model:open="viewDialog"
      title="Thông tin chi tiết sinh viên"
      width="760px"
      @cancel="viewDialog = false"
      :footer="null"
    >
      <a-descriptions bordered :column="2" v-if="viewTarget">
        <a-descriptions-item label="Mã sinh viên" :span="1">{{ viewTarget.studentCode }}</a-descriptions-item>
        <a-descriptions-item label="Họ tên" :span="1">{{ viewTarget.fullName }}</a-descriptions-item>
        <a-descriptions-item label="Giới tính" :span="1">{{ viewTarget.gender === 'Male' ? 'Nam' : 'Nữ' }}</a-descriptions-item>
        <a-descriptions-item label="Ngày sinh" :span="1">{{ formatDate(viewTarget.dateOfBirth) }}</a-descriptions-item>
        <a-descriptions-item label="Email" :span="1">{{ viewTarget.email || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Số điện thoại" :span="1">{{ viewTarget.phone || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Khoa" :span="1">{{ viewTarget.faculty }}</a-descriptions-item>
        <a-descriptions-item label="Ngành" :span="1">{{ viewTarget.major || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Lớp" :span="1">{{ viewTarget.classCode || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Năm học" :span="1">{{ viewTarget.academicYear }}</a-descriptions-item>
        <a-descriptions-item label="CMND/CCCD" :span="2">{{ viewTarget.identityCard }}</a-descriptions-item>
        <a-descriptions-item label="Ngày cấp" :span="1">{{ formatDate(viewTarget.identityCardIssuedDate) }}</a-descriptions-item>
        <a-descriptions-item label="Nơi cấp" :span="1">{{ viewTarget.identityCardIssuedPlace || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Địa chỉ thường trú" :span="2">{{ viewTarget.permanentAddress }}</a-descriptions-item>
        <a-descriptions-item label="Người liên hệ khẩn cấp" :span="1">{{ viewTarget.emergencyContactName }}</a-descriptions-item>
        <a-descriptions-item label="SĐT người liên hệ" :span="1">{{ viewTarget.emergencyContactPhone }}</a-descriptions-item>
        <a-descriptions-item label="Quan hệ" :span="1">{{ viewTarget.emergencyContactRelation || 'Chưa có' }}</a-descriptions-item>
        <a-descriptions-item label="Trạng thái" :span="1">
          <a-tag :color="viewTarget.isActive ? 'success' : 'default'">
            {{ viewTarget.isActive ? 'Hoạt động' : 'Không hoạt động' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Độ hoàn thiện hồ sơ" :span="2">
          <a-progress :percent="viewTarget.profileCompletionPct" :status="viewTarget.profileCompletionPct < 100 ? 'active' : 'success'" />
        </a-descriptions-item>
      </a-descriptions>
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
import { DownloadOutlined } from '@ant-design/icons-vue';
import { studentService } from "@/services/studentService";
import { useExcelExport } from '@/composables/useExcelExport'

const { exporting, exportToExcel } = useExcelExport()

const studentColumns = [
  { title: "Sinh viên", dataIndex: "fullName", key: "fullName", width: 260 },
  { title: "Mã SV", dataIndex: "studentCode", key: "studentCode", width: 120 },
  { title: "Khoa", dataIndex: "faculty", key: "faculty", width: 180, ellipsis: true },
  { title: "Lớp", dataIndex: "classCode", key: "classCode", width: 120 },
  { title: "Email", dataIndex: "email", key: "email", width: 200, ellipsis: true },
  { title: "Số điện thoại", dataIndex: "phone", key: "phone", width: 130 },
  { title: "Trạng thái", key: "isActive", align: "center", width: 120 },
  {
    title: "Thao tác",
    dataIndex: "actions",
    key: "actions",
    align: "center",
    width: 200,
    fixed: 'right'
  },
];
const statusOptions = [
  { label: "Đang ở", value: "Active" },
  { label: "Sắp hết hạn", value: "Expiring" },
  { label: "Đã rời đi", value: "Departed" },
];
const filterStatusOptions = [
  { label: "Tất cả", value: "all" },
  { label: "Hoạt động", value: "active" },
  { label: "Không hoạt động", value: "inactive" },
];
const students = ref([]);
const search = ref("");
const statusFilter = ref("all");
const classFilter = ref("all");
const genderFilter = ref("");
const loading = ref(false);
const saving = ref(false);
const dialog = ref(false);
const viewDialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const viewTarget = ref(null);
const deleteTarget = ref(null);
const formErrors = ref({});
const form = ref(defaultForm());

const classOptions = computed(() =>
  [...new Set(students.value.map((item) => item.classCode))]
    .filter(Boolean)
    .sort(),
);

const filteredStudents = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  return students.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.fullName?.toLowerCase().includes(keyword) ||
      item.studentCode?.toLowerCase().includes(keyword) ||
      (item.email && item.email.toLowerCase().includes(keyword));
    const matchesStatus =
      statusFilter.value === "all" ||
      (statusFilter.value === "active" && item.isActive) ||
      (statusFilter.value === "inactive" && !item.isActive);
    const matchesClass =
      classFilter.value === "all" || item.classCode === classFilter.value;
    const matchesGender =
      !genderFilter.value || item.gender === genderFilter.value;
    return (
      matchesText &&
      matchesStatus &&
      matchesClass &&
      matchesGender
    );
  });
});

function resetFilters() {
  search.value = "";
  statusFilter.value = "all";
  classFilter.value = "all";
  genderFilter.value = "";
}

function defaultForm() {
  const today = new Date().toISOString().split('T')[0];
  return {
    studentCode: "",
    fullName: "",
    email: "",
    phone: "",
    gender: "Male",
    dateOfBirth: "2000-01-01",
    faculty: "",
    major: "",
    classCode: "",
    academicYear: 1,
    identityCard: "",
    identityCardIssuedDate: today,
    identityCardIssuedPlace: "",
    permanentAddress: "",
    emergencyContactName: "",
    emergencyContactPhone: "",
    emergencyContactRelation: "",
    isActive: true,
  };
}

async function loadStudents() {
  loading.value = true;
  try {
    const data = await studentService.getAll();
    console.log('Loaded students:', data);
    
    // Process avatar URLs
    students.value = data.map(student => {
      let avatarUrl = null;
      if (student.avatarUrl) {
        avatarUrl = student.avatarUrl.startsWith('http')
          ? student.avatarUrl
          : `http://localhost:5003${student.avatarUrl}`;
      }
      
      return {
        ...student,
        avatarUrl
      };
    });
    console.log('Processed students:', students.value.length);
  } catch (err) {
    console.error('Error loading students:', err);
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

function openView(item) {
  viewTarget.value = item;
  viewDialog.value = true;
}

function openEdit(item) {
  editTarget.value = item;
  form.value = {
    studentCode: item.studentCode || '',
    fullName: item.fullName || '',
    email: item.email || '',
    phone: item.phone || '',
    gender: item.gender || 'Male',
    dateOfBirth: item.dateOfBirth ? item.dateOfBirth.toString().slice(0, 10) : '2000-01-01',
    faculty: item.faculty || '',
    major: item.major || '',
    classCode: item.classCode || '',
    academicYear: item.academicYear || 1,
    identityCard: item.identityCard || '',
    identityCardIssuedDate: item.identityCardIssuedDate ? item.identityCardIssuedDate.toString().slice(0, 10) : new Date().toISOString().split('T')[0],
    identityCardIssuedPlace: item.identityCardIssuedPlace || '',
    permanentAddress: item.permanentAddress || '',
    emergencyContactName: item.emergencyContactName || '',
    emergencyContactPhone: item.emergencyContactPhone || '',
    emergencyContactRelation: item.emergencyContactRelation || '',
    isActive: item.isActive !== false,
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
  if (!form.value.studentCode?.trim())
    errors.studentCode = "Vui lòng nhập mã sinh viên";
  if (!form.value.fullName?.trim()) errors.fullName = "Vui lòng nhập họ tên";
  if (!form.value.faculty?.trim()) errors.faculty = "Vui lòng nhập khoa";
  if (!form.value.identityCard?.trim()) errors.identityCard = "Vui lòng nhập CMND/CCCD";
  if (!form.value.permanentAddress?.trim()) errors.permanentAddress = "Vui lòng nhập địa chỉ thường trú";
  if (!form.value.emergencyContactName?.trim()) errors.emergencyContactName = "Vui lòng nhập tên người liên hệ khẩn cấp";
  if (!form.value.emergencyContactPhone?.trim()) errors.emergencyContactPhone = "Vui lòng nhập SĐT người liên hệ khẩn cấp";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

function payload() {
  // Ensure dates are in YYYY-MM-DD format
  const dateOfBirth = form.value.dateOfBirth ? 
    (typeof form.value.dateOfBirth === 'string' ? form.value.dateOfBirth : form.value.dateOfBirth.toISOString().split('T')[0]) : 
    '2000-01-01';
  
  const identityCardIssuedDate = form.value.identityCardIssuedDate ?
    (typeof form.value.identityCardIssuedDate === 'string' ? form.value.identityCardIssuedDate : form.value.identityCardIssuedDate.toISOString().split('T')[0]) :
    new Date().toISOString().split('T')[0];

  return {
    studentCode: form.value.studentCode?.trim() || '',
    fullName: form.value.fullName?.trim() || '',
    email: form.value.email?.trim() || '',
    phone: form.value.phone?.trim() || '',
    gender: form.value.gender || 'Male',
    dateOfBirth: dateOfBirth,
    faculty: form.value.faculty?.trim() || '',
    major: form.value.major?.trim() || '',
    classCode: form.value.classCode?.trim() || '',
    academicYear: form.value.academicYear || 1,
    identityCard: form.value.identityCard?.trim() || '',
    identityCardIssuedDate: identityCardIssuedDate,
    identityCardIssuedPlace: form.value.identityCardIssuedPlace?.trim() || '',
    permanentAddress: form.value.permanentAddress?.trim() || '',
    emergencyContactName: form.value.emergencyContactName?.trim() || '',
    emergencyContactPhone: form.value.emergencyContactPhone?.trim() || '',
    emergencyContactRelation: form.value.emergencyContactRelation?.trim() || 'Cha/Mẹ',
    isActive: form.value.isActive !== false,
  };
}

async function saveStudent() {
  if (!validate()) return;
  saving.value = true;
  try {
    const data = payload();
    console.log('Sending student data:', data);
    
    if (editTarget.value) {
      const result = await studentService.update(editTarget.value.id, data);
      console.log('Update result:', result);
      message.success("Cập nhật sinh viên thành công");
    } else {
      const result = await studentService.create(data);
      console.log('Create result:', result);
      message.success("Thêm sinh viên thành công");
    }
    closeDialog();
    await loadStudents();
  } catch (err) {
    console.error('Error saving student:', err);
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

// Export to Excel function
const handleExport = () => {
  const columnMapping = {
    studentCode: 'Mã SV',
    fullName: 'Họ tên',
    dateOfBirth: 'Ngày sinh',
    gender: 'Giới tính',
    identityCard: 'CMND/CCCD',
    phone: 'Số điện thoại',
    email: 'Email',
    address: 'Địa chỉ',
    faculty: 'Khoa',
    className: 'Lớp',
    academicYear: 'Năm học',
    isActive: 'Trạng thái'
  }
  
  const dataToExport = filteredStudents.value.map(student => ({
    studentCode: student.studentCode,
    fullName: student.fullName,
    dateOfBirth: formatDate(student.dateOfBirth),
    gender: student.gender === 'Male' ? 'Nam' : 'Nữ',
    identityCard: student.identityCard,
    phone: student.phone,
    email: student.email,
    address: student.address,
    faculty: student.faculty,
    className: student.className,
    academicYear: student.academicYear,
    isActive: student.isActive ? 'Hoạt động' : 'Không hoạt động'
  }))
  
  exportToExcel(dataToExport, columnMapping, 'Danh_sach_sinh_vien', 'Sinh viên')
}

onMounted(loadStudents);
</script>
