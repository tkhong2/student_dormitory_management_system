<template>
  <div>
    <!-- Page Header -->
    <div
      style="
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 16px;
        flex-wrap: wrap;
        gap: 12px;
      "
    >
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Tòa nhà
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Tổng <strong>{{ buildings.length }}</strong> tòa nhà —
          <span style="color: #2563eb">{{ countByGender("Male") }} Nam</span>
          ·
          <span style="color: #ec4899">{{ countByGender("Female") }} Nữ</span>
          ·
          <span style="color: #8b5cf6">{{ countByGender("Mixed") }} Hỗn hợp</span>
        </p>
      </div>
      <a-button type="primary" @click="openCreate" style="background: #ff9800; border-color: #ff9800;">
        + Thêm tòa nhà
      </a-button>
    </div>

    <!-- Filter Card -->
    <a-card style="margin-bottom: 16px" :bordered="false">
      <a-row :gutter="16">
        <a-col :xs="24" :sm="8" :md="6">
          <a-select
            v-model:value="genderFilter"
            placeholder="Loại tòa"
            style="width: 100%"
          >
            <a-select-option value="">Tất cả loại tòa</a-select-option>
            <a-select-option value="Male">Nam</a-select-option>
            <a-select-option value="Female">Nữ</a-select-option>
            <a-select-option value="Mixed">Hỗn hợp</a-select-option>
          </a-select>
        </a-col>
        <a-col :xs="24" :sm="8" :md="6">
          <a-input-number
            v-model:value="floorFilter"
            placeholder="Số tầng"
            style="width: 100%"
            :min="0"
            :max="50"
            allow-clear
          />
        </a-col>
        <a-col :xs="24" :sm="8" :md="12">
          <a-input
            v-model:value="search"
            placeholder="Tìm tên tòa nhà..."
            allow-clear
          >
            <template #prefix>
              <SearchOutlined />
            </template>
          </a-input>
        </a-col>
      </a-row>
    </a-card>

    <!-- Buildings Grid Card -->
    <a-card :bordered="false" :loading="loading">
      <!-- Empty State -->
      <div
        v-if="filteredBuildings.length === 0"
        style="text-align: center; padding: 60px 0; color: #9ca3af"
      >
        <i class="anticon anticon-home" style="font-size: 48px"></i>
        <p style="margin-top: 12px; font-size: 15px">
          {{
            buildings.length === 0
              ? "Chưa có tòa nhà nào"
              : "Không tìm thấy kết quả phù hợp"
          }}
        </p>
      </div>

      <!-- Buildings Grid -->
      <div v-else style="display: flex; flex-wrap: wrap; margin: -12px">
        <div
          v-for="b in filteredBuildings"
          :key="b.id"
          style="width: 25%; padding: 12px"
        >
          <a-card
            hoverable
            @click="selectBuilding(b)"
            :style="{
              borderRadius: '16px',
              overflow: 'hidden',
              cursor: 'pointer',
              border: '1px solid #f0f0f0',
              boxShadow: '0 2px 8px rgba(0,0,0,0.06)',
              transition: 'all 0.3s ease',
            }"
            :body-style="{ padding: '0' }"
          >
            <!-- Image with overlay badge -->
            <div style="position: relative">
              <div
                v-if="b.thumbnailUrl"
                :style="{
                  width: '100%',
                  height: '180px',
                  overflow: 'hidden',
                  background: '#f5f5f5',
                }"
              >
                <img
                  :src="b.thumbnailUrl"
                  style="width: 100%; height: 100%; object-fit: cover"
                />
              </div>

              <!-- Fallback icon -->
              <div
                v-else
                :style="{
                  width: '100%',
                  height: '180px',
                  background: getGenderBg(b.gender),
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }"
              >
                <i
                  class="anticon anticon-office-building"
                  :style="{
                    fontSize: '64px',
                    color: getGenderIconColor(b.gender),
                    opacity: 0.3,
                  }"
                ></i>
              </div>

              <!-- Status Badge -->
              <div
                v-if="b.status !== 'Active'"
                :style="{
                  position: 'absolute',
                  top: '12px',
                  right: '12px',
                  background: b.status === 'UnderMaintenance' ? '#faad14' : '#ff4d4f',
                  color: 'white',
                  padding: '4px 12px',
                  borderRadius: '12px',
                  fontSize: '11px',
                  fontWeight: '600',
                  boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
                }"
              >
                {{ b.status === 'UnderMaintenance' ? 'Bảo trì' : 'Đóng cửa' }}
              </div>

              <!-- Amenities Icons -->
              <div
                :style="{
                  position: 'absolute',
                  bottom: '12px',
                  left: '12px',
                  display: 'flex',
                  gap: '6px',
                }"
              >
                <div
                  v-if="b.hasElevator"
                  :style="{
                    background: 'rgba(255,255,255,0.95)',
                    padding: '4px 10px',
                    borderRadius: '8px',
                    fontSize: '12px',
                    fontWeight: '500',
                    color: '#1f2937',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '4px',
                  }"
                  title="Thang máy"
                >
                  <VerticalAlignTopOutlined />
                  <span>Thang máy</span>
                </div>
                <div
                  v-if="b.hasParking"
                  :style="{
                    background: 'rgba(255,255,255,0.95)',
                    padding: '4px 10px',
                    borderRadius: '8px',
                    fontSize: '12px',
                    fontWeight: '500',
                    color: '#1f2937',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '4px',
                  }"
                  title="Bãi đỗ xe"
                >
                  <CarOutlined />
                  <span>Đỗ xe</span>
                </div>
                <div
                  v-if="b.hasLaundry"
                  :style="{
                    background: 'rgba(255,255,255,0.95)',
                    padding: '4px 10px',
                    borderRadius: '8px',
                    fontSize: '12px',
                    fontWeight: '500',
                    color: '#1f2937',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '4px',
                  }"
                  title="Giặt ủi"
                >
                  <ShoppingOutlined />
                  <span>Giặt ủi</span>
                </div>
              </div>
            </div>

            <!-- Content -->
            <div style="padding: 16px">
              <div
                style="
                  font-weight: 700;
                  font-size: 17px;
                  margin-bottom: 6px;
                  color: #1f2937;
                "
              >
                {{ b.name }}
              </div>

              <div
                style="
                  display: flex;
                  align-items: center;
                  gap: 12px;
                  margin-bottom: 10px;
                  font-size: 13px;
                  color: #6b7280;
                "
              >
                <span style="display: flex; align-items: center; gap: 4px">
                  <HomeOutlined />
                  {{ b.totalRooms }} phòng
                </span>
                <span style="color: #d1d5db">•</span>
                <span style="display: flex; align-items: center; gap: 4px">
                  <ApartmentOutlined />
                  {{ b.totalFloors }} tầng
                </span>
              </div>

              <a-tag
                :color="getGenderColor(b.gender)"
                style="margin-bottom: 12px; font-size: 12px; padding: 2px 10px"
              >
                {{ getGenderLabel(b.gender) }}
              </a-tag>

              <p
                style="
                  font-size: 13px;
                  color: #6b7280;
                  margin-bottom: 14px;
                  min-height: 40px;
                  line-height: 1.5;
                "
              >
                {{
                  b.description
                    ? b.description.length > 60
                      ? b.description.substring(0, 60) + "..."
                      : b.description
                    : "Chưa có mô tả"
                }}
              </p>

              <!-- Actions -->
              <div style="display: flex; gap: 8px">
                <a-button
                  type="primary"
                  size="middle"
                  @click.stop="openEdit(b)"
                  style="flex: 1; border-radius: 8px"
                >
                  <EditOutlined />
                  Sửa
                </a-button>
                <a-button
                  danger
                  size="middle"
                  @click.stop="confirmDelete(b)"
                  style="flex: 1; border-radius: 8px"
                >
                  <DeleteOutlined />
                  Xóa
                </a-button>
              </div>
            </div>
          </a-card>
        </div>
      </div>
    </a-card>

    <!-- Add/Edit Drawer -->
    <a-drawer
      v-model:open="dialog"
      :title="editTarget ? 'Chỉnh sửa tòa nhà' : 'Thêm tòa nhà mới'"
      placement="right"
      :width="480"
      :mask-closable="false"
      @close="closeDialog"
    >
      <a-form
        :model="form"
        :label-col="{ span: 24 }"
        :wrapper-col="{ span: 24 }"
        layout="vertical"
        @finish="saveBuilding"
      >
        <a-form-item label="Tên tòa nhà" required>
          <a-input
            v-model:value="form.name"
            placeholder="Ví dụ: Tòa A - Khu Nam"
            size="large"
            :status="formErrors.name ? 'error' : ''"
          />
          <div v-if="formErrors.name" class="form-error">
            {{ formErrors.name }}
          </div>
        </a-form-item>

        <a-form-item label="Địa chỉ">
          <a-input
            v-model:value="form.address"
            placeholder="Địa chỉ tòa nhà..."
            size="large"
          />
        </a-form-item>

        <a-form-item label="Mô tả">
          <a-textarea
            v-model:value="form.description"
            placeholder="Mô tả về tòa nhà..."
            :rows="3"
          />
        </a-form-item>

        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="Số tầng" required>
              <a-input-number
                v-model:value="form.totalFloors"
                :min="1"
                size="large"
                style="width: 100%"
                :status="formErrors.totalFloors ? 'error' : ''"
              />
              <div v-if="formErrors.totalFloors" class="form-error">
                {{ formErrors.totalFloors }}
              </div>
            </a-form-item>
          </a-col>

          <a-col :span="8">
            <a-form-item label="Số phòng" required>
              <a-input-number
                v-model:value="form.totalRooms"
                :min="1"
                size="large"
                style="width: 100%"
                :status="formErrors.totalRooms ? 'error' : ''"
              />
              <div v-if="formErrors.totalRooms" class="form-error">
                {{ formErrors.totalRooms }}
              </div>
            </a-form-item>
          </a-col>

          <a-col :span="8">
            <a-form-item label="Giới tính" required>
              <a-select
                v-model:value="form.gender"
                placeholder="Chọn loại"
                size="large"
                :status="formErrors.gender ? 'error' : ''"
              >
                <a-select-option value="Male">Nam</a-select-option>
                <a-select-option value="Female">Nữ</a-select-option>
                <a-select-option value="Mixed">Hỗn hợp</a-select-option>
              </a-select>
              <div v-if="formErrors.gender" class="form-error">
                {{ formErrors.gender }}
              </div>
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Tên quản lý">
              <a-input
                v-model:value="form.managerName"
                placeholder="Họ tên quản lý"
                size="large"
              />
            </a-form-item>
          </a-col>

          <a-col :span="12">
            <a-form-item label="SĐT quản lý">
              <a-input
                v-model:value="form.managerPhone"
                placeholder="Số điện thoại"
                size="large"
              />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Năm xây dựng">
              <a-input
                v-model:value="form.constructionYear"
                placeholder="Ví dụ: 2020"
                size="large"
              />
            </a-form-item>
          </a-col>

          <a-col :span="12">
            <a-form-item label="Trạng thái">
              <a-select
                v-model:value="form.status"
                size="large"
              >
                <a-select-option value="Active">Hoạt động</a-select-option>
                <a-select-option value="UnderMaintenance">Bảo trì</a-select-option>
                <a-select-option value="Closed">Đóng cửa</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="Tiện ích">
          <a-checkbox-group v-model:value="form.amenities">
            <a-checkbox value="hasElevator">Thang máy</a-checkbox>
            <a-checkbox value="hasParking">Bãi đỗ xe</a-checkbox>
            <a-checkbox value="hasLaundry">Giặt ủi</a-checkbox>
          </a-checkbox-group>
        </a-form-item>

        <a-form-item label="Đường dẫn hình ảnh (URL)">
          <ImageUpload 
            v-model="form.thumbnailUrl"
            button-text="Chọn ảnh tòa nhà"
            preview-width="100%"
            preview-height="200px"
            placeholder="Hoặc nhập URL hình ảnh"
          />
        </a-form-item>

        <a-form-item>
          <a-checkbox v-model:checked="form.isActive">Kích hoạt</a-checkbox>
        </a-form-item>

        <div class="drawer-footer">
          <a-button @click="closeDialog" size="large">Hủy</a-button>
          <a-button
            type="primary"
            html-type="submit"
            :loading="saving"
            size="large"
          >
            {{ editTarget ? "Lưu thay đổi" : "Thêm mới" }}
          </a-button>
        </div>
      </a-form>
    </a-drawer>

    <!-- Delete Confirm Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      ok-text="Xóa"
      cancel-text="Hủy"
      ok-button-props="{ danger: true }"
      @ok="deleteBuilding"
    >
      <p>
        Bạn có chắc muốn xóa tòa nhà <strong>{{ deleteTarget?.name }}</strong
        >? Hành động này không thể hoàn tác.
      </p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { message } from "ant-design-vue";
import {
  SearchOutlined,
  VerticalAlignTopOutlined,
  CarOutlined,
  ShoppingOutlined,
  HomeOutlined,
  ApartmentOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import ImageUpload from "@/components/common/ImageUpload.vue";
import { buildingService } from "@/services/buildingService";

// ─── State ───────────────────────────────────────────────────────────────────
const buildings = ref([]);
const loading = ref(false);
const saving = ref(false);

// ─── Filter state ─────────────────────────────────────────────────────────────
const search = ref("");
const genderFilter = ref("");
const floorFilter = ref(null);

const dialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);

const form = ref({
  name: "",
  address: "",
  description: "",
  totalFloors: 5,
  totalRooms: 50,
  gender: "Male",
  managerName: "",
  managerPhone: "",
  constructionYear: "",
  status: "Active",
  amenities: [],
  thumbnailUrl: "",
  isActive: true,
});
const formErrors = ref({});

// ─── Computed - filtered buildings ────────────────────────────────────────────
const filteredBuildings = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  const genderValue = genderFilter.value || null;
  const floorValue = floorFilter.value || null;

  return buildings.value.filter((b) => {
    const matchesName = !keyword || b.name.toLowerCase().includes(keyword);
    const matchesGender = !genderValue || b.gender === genderValue;
    const matchesFloor = !floorValue || b.totalFloors === floorValue;
    return matchesName && matchesGender && matchesFloor;
  });
});

function countByGender(gender) {
  return buildings.value.filter((b) => b.gender === gender).length;
}

// ─── Load data ───────────────────────────────────────────────────────────────
async function loadBuildings() {
  loading.value = true;
  try {
    buildings.value = await buildingService.getAll();
  } catch (err) {
    message.error(err.message || "Không thể tải danh sách tòa nhà");
  } finally {
    loading.value = false;
  }
}

onMounted(loadBuildings);

// ─── CRUD helpers ─────────────────────────────────────────────────────────────
function openCreate() {
  editTarget.value = null;
  form.value = {
    name: "",
    address: "",
    description: "",
    totalFloors: 5,
    totalRooms: 50,
    gender: "Male",
    managerName: "",
    managerPhone: "",
    constructionYear: "",
    status: "Active",
    amenities: [],
    thumbnailUrl: "",
    isActive: true,
  };
  formErrors.value = {};
  dialog.value = true;
}

function openEdit(b) {
  editTarget.value = b;
  const amenities = [];
  if (b.hasElevator) amenities.push("hasElevator");
  if (b.hasParking) amenities.push("hasParking");
  if (b.hasLaundry) amenities.push("hasLaundry");
  
  form.value = {
    name: b.name,
    address: b.address || "",
    description: b.description || "",
    totalFloors: b.totalFloors,
    totalRooms: b.totalRooms,
    gender: b.gender,
    managerName: b.managerName || "",
    managerPhone: b.managerPhone || "",
    constructionYear: b.constructionYear || "",
    status: b.status,
    amenities: amenities,
    thumbnailUrl: b.thumbnailUrl || "",
    isActive: b.isActive !== false,
  };
  formErrors.value = {};
  dialog.value = true;
}

function closeDialog() {
  dialog.value = false;
  editTarget.value = null;
}

function selectBuilding(b) {
  // Can be used to view building details
  // Currently just placeholder for card click
}

function validate() {
  const errors = {};
  if (!form.value.name.trim()) errors.name = "Vui lòng nhập tên tòa nhà";
  if (!form.value.totalFloors || form.value.totalFloors < 1)
    errors.totalFloors = "Số tầng phải ≥ 1";
  if (!form.value.totalRooms || form.value.totalRooms < 1)
    errors.totalRooms = "Số phòng phải ≥ 1";
  if (!form.value.gender) errors.gender = "Vui lòng chọn giới tính";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

async function saveBuilding() {
  if (!validate()) return;
  saving.value = true;
  try {
    const payload = {
      name: form.value.name,
      address: form.value.address,
      description: form.value.description,
      totalFloors: form.value.totalFloors,
      totalRooms: form.value.totalRooms,
      gender: form.value.gender,
      managerName: form.value.managerName,
      managerPhone: form.value.managerPhone,
      constructionYear: form.value.constructionYear,
      status: form.value.status,
      hasElevator: form.value.amenities.includes("hasElevator"),
      hasParking: form.value.amenities.includes("hasParking"),
      hasLaundry: form.value.amenities.includes("hasLaundry"),
      thumbnailUrl: form.value.thumbnailUrl,
      isActive: form.value.isActive,
    };
    
    if (editTarget.value) {
      await buildingService.update(editTarget.value.id, payload);
      message.success("Cập nhật tòa nhà thành công!");
    } else {
      await buildingService.create(payload);
      message.success("Thêm tòa nhà thành công!");
    }
    closeDialog();
    await loadBuildings();
  } catch (err) {
    message.error(err.message || "Có lỗi xảy ra");
  } finally {
    saving.value = false;
  }
}

function confirmDelete(b) {
  deleteTarget.value = b;
  deleteDialog.value = true;
}

async function deleteBuilding() {
  saving.value = true;
  try {
    await buildingService.delete(deleteTarget.value.id);
    message.success("Đã xóa tòa nhà!");
    deleteDialog.value = false;
    await loadBuildings();
  } catch (err) {
    message.error(err.message || "Có lỗi xảy ra");
  } finally {
    saving.value = false;
  }
}

// ─── UI helpers ───────────────────────────────────────────────────────────────
function getGenderLabel(gender) {
  const map = { Male: "Khu Nam", Female: "Khu Nữ", Mixed: "Hỗn hợp" };
  return map[gender] || gender;
}
function getGenderColor(gender) {
  const map = { Male: "blue", Female: "pink", Mixed: "purple" };
  return map[gender] || "grey";
}
function getBuildingColor(gender) {
  const map = { Male: "#3b82f6", Female: "#ec4899", Mixed: "#8b5cf6" };
  return map[gender] || "#e5e7eb";
}
function getGenderBg(gender) {
  const map = { Male: "#eff6ff", Female: "#fdf2f8", Mixed: "#f5f3ff" };
  return map[gender] || "#f9fafb";
}
function getGenderIconColor(gender) {
  const map = { Male: "#3b82f6", Female: "#ec4899", Mixed: "#8b5cf6" };
  return map[gender] || "#9ca3af";
}
</script>

<style scoped>
.form-error {
  color: #ff4d4f;
  font-size: 12px;
  margin-top: 4px;
}

.drawer-footer {
  display: flex;
  gap: 8px;
  justify-content: flex-end;
  border-top: 1px solid #f0f0f0;
  padding-top: 16px;
  margin-top: 24px;
}

:deep(.ant-input),
:deep(.ant-input-number),
:deep(.ant-select),
:deep(.ant-picker) {
  border-radius: 6px;
}

:deep(.ant-btn-primary) {
  background-color: #1890ff;
  border-color: #1890ff;
  border-radius: 6px;
}

:deep(.ant-drawer-header) {
  border-bottom: 1px solid #f0f0f0;
}

:deep(.ant-drawer-body) {
  padding: 24px;
}

:deep(.ant-card-hoverable:hover) {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12) !important;
}

:deep(.ant-btn) {
  font-weight: 500;
}
</style>
