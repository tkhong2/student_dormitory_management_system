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
          <span style="color: #2563eb">{{ countByType("Male") }} Nam</span>
          ·
          <span style="color: #ec4899">{{ countByType("Female") }} Nữ</span>
          ·
          <span style="color: #8b5cf6">{{ countByType("Mixed") }} Hỗn hợp</span>
        </p>
      </div>
      <a-button type="primary" @click="openCreate">
        <template #icon><i class="anticon anticon-plus"></i></template>
        Thêm tòa nhà
      </a-button>
    </div>

    <!-- Filter Section -->
    <a-card style="margin-bottom: 16px; background: #fafafa">
      <a-row :gutter="16">
        <a-col :span="6">
          <a-select
            v-model:value="typeFilter"
            placeholder="Loại tòa"
            style="width: 100%"
          >
            <a-select-option value="">Tất cả loại tòa</a-select-option>
            <a-select-option value="Male">Nam</a-select-option>
            <a-select-option value="Female">Nữ</a-select-option>
            <a-select-option value="Mixed">Hỗn hợp</a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-input-number
            v-model:value="floorFilter"
            placeholder="Số tầng"
            style="width: 100%"
            :min="0"
            :max="50"
            allow-clear
          />
        </a-col>
        <a-col :span="12">
          <a-input
            v-model:value="search"
            placeholder="Tìm tên tòa nhà..."
            allow-clear
          >
            <template #prefix>
              <i class="anticon anticon-search"></i>
            </template>
          </a-input>
        </a-col>
      </a-row>
    </a-card>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="buildings"
      :treatEmptyAsError="false"
      @retry="loadBuildings"
    >
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
      <div v-else style="display: flex; flex-wrap: wrap; margin: -8px">
        <div
          v-for="b in filteredBuildings"
          :key="b.id"
          style="width: 25%; padding: 8px"
        >
          <a-card
            hoverable
            @click="selectBuilding(b)"
            style="border-radius: 12px; overflow: hidden; cursor: pointer"
            :body-style="{ padding: '12px' }"
          >
            <!-- Color Header by type -->
            <div
              :style="{
                height: '4px',
                background: getBuildingColor(b.type),
                marginBottom: '12px',
                marginTop: '-12px',
                marginLeft: '-12px',
                marginRight: '-12px',
              }"
            />

            <!-- Image -->
            <div
              v-if="b.imageUrl"
              :style="{
                width: '100%',
                height: '120px',
                borderRadius: '8px',
                overflow: 'hidden',
                marginBottom: '12px',
                background: '#f5f5f5',
              }"
            >
              <img
                :src="b.imageUrl"
                style="width: 100%; height: 100%; object-fit: cover"
              />
            </div>

            <!-- Fallback icon -->
            <div
              v-else
              :style="{
                width: '100%',
                height: '120px',
                borderRadius: '8px',
                background: getTypeBg(b.type),
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                marginBottom: '12px',
              }"
            >
              <i
                class="anticon anticon-office-building"
                :style="{
                  fontSize: '48px',
                  color: getTypeIconColor(b.type),
                }"
              ></i>
            </div>

            <!-- Content -->
            <div style="text-align: center">
              <div
                style="font-weight: 700; font-size: 16px; margin-bottom: 2px"
              >
                {{ b.name }}
              </div>
              <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px">
                {{ getTypeLabel(b.type) }} · {{ b.numberOfFloors }} tầng
              </div>

              <a-tag
                :color="getTypeColor(b.type)"
                style="margin-bottom: 12px; font-size: 11px"
              >
                {{ getTypeLabel(b.type) }}
              </a-tag>

              <p
                style="
                  font-size: 12px;
                  color: #6b7280;
                  margin-bottom: 8px;
                  min-height: 24px;
                "
              >
                {{
                  b.description
                    ? b.description.substring(0, 40) + "..."
                    : "Chưa có mô tả"
                }}
              </p>

              <!-- Actions -->
              <a-space style="width: 100%">
                <a-button
                  type="primary"
                  size="small"
                  ghost
                  @click.stop="openEdit(b)"
                  style="flex: 1"
                >
                  <i class="anticon anticon-edit"></i>
                  Sửa
                </a-button>
                <a-button
                  danger
                  size="small"
                  @click.stop="confirmDelete(b)"
                  style="flex: 1"
                >
                  <i class="anticon anticon-delete"></i>
                  Xóa
                </a-button>
              </a-space>
            </div>
          </a-card>
        </div>
      </div>
    </DataStatus>

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
            placeholder="Ví dụ: Tòa A - Nam"
            size="large"
            :status="formErrors.name ? 'error' : ''"
          />
          <div v-if="formErrors.name" class="form-error">
            {{ formErrors.name }}
          </div>
        </a-form-item>

        <a-form-item label="Mô tả">
          <a-textarea
            v-model:value="form.description"
            placeholder="Mô tả về tòa nhà..."
            :rows="3"
          />
        </a-form-item>

        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Số tầng" required>
              <a-input-number
                v-model:value="form.numberOfFloors"
                :min="1"
                size="large"
                style="width: 100%"
                :status="formErrors.numberOfFloors ? 'error' : ''"
              />
              <div v-if="formErrors.numberOfFloors" class="form-error">
                {{ formErrors.numberOfFloors }}
              </div>
            </a-form-item>
          </a-col>

          <a-col :span="12">
            <a-form-item label="Loại tòa nhà" required>
              <a-select
                v-model:value="form.type"
                placeholder="Chọn loại"
                size="large"
                :status="formErrors.type ? 'error' : ''"
              >
                <a-select-option value="Male">Nam</a-select-option>
                <a-select-option value="Female">Nữ</a-select-option>
                <a-select-option value="Mixed">Hỗn hợp</a-select-option>
              </a-select>
              <div v-if="formErrors.type" class="form-error">
                {{ formErrors.type }}
              </div>
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="Đường dẫn hình ảnh (URL)">
          <a-input
            v-model:value="form.imageUrl"
            placeholder="https://..."
            size="large"
          />
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

    <!-- Notification -->
    <a-message
      v-if="snackbar.show"
      :type="snackbar.type"
      :message="snackbar.message"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import DataStatus from "@/components/common/DataStatus.vue";
import { buildingService } from "@/services/buildingService";

// ─── State ───────────────────────────────────────────────────────────────────
const buildings = ref([]);
const loading = ref(false);
const error = ref(null);
const saving = ref(false);

// ─── Filter state ─────────────────────────────────────────────────────────────
const search = ref("");
const typeFilter = ref("");
const floorFilter = ref(null);

const dialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);

const form = ref({
  name: "",
  description: "",
  numberOfFloors: 5,
  type: "Male",
  imageUrl: "",
});
const formErrors = ref({});

const snackbar = ref({ show: false, message: "", type: "success" });

// ─── Computed - filtered buildings ────────────────────────────────────────────
const filteredBuildings = computed(() => {
  const keyword = search.value.trim().toLowerCase();
  const typeValue = typeFilter.value || null;
  const floorValue = floorFilter.value || null;

  return buildings.value.filter((b) => {
    const matchesName = !keyword || b.name.toLowerCase().includes(keyword);
    const matchesType = !typeValue || b.type === typeValue;
    const matchesFloor = !floorValue || b.numberOfFloors === floorValue;
    return matchesName && matchesType && matchesFloor;
  });
});

function countByType(type) {
  return buildings.value.filter((b) => b.type === type).length;
}

// ─── Load data ───────────────────────────────────────────────────────────────
async function loadBuildings() {
  loading.value = true;
  error.value = null;
  try {
    buildings.value = await buildingService.getAll();
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách tòa nhà";
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
    description: "",
    numberOfFloors: 5,
    type: "Male",
    imageUrl: "",
  };
  formErrors.value = {};
  dialog.value = true;
}

function openEdit(b) {
  editTarget.value = b;
  form.value = {
    name: b.name,
    description: b.description,
    numberOfFloors: b.numberOfFloors,
    type: b.type,
    imageUrl: b.imageUrl || "",
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
  if (!form.value.numberOfFloors || form.value.numberOfFloors < 1)
    errors.numberOfFloors = "Số tầng phải ≥ 1";
  if (!form.value.type) errors.type = "Vui lòng chọn loại";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

async function saveBuilding() {
  if (!validate()) return;
  saving.value = true;
  try {
    if (editTarget.value) {
      await buildingService.update(editTarget.value.id, form.value);
      showSnackbar("Cập nhật tòa nhà thành công!", "success");
    } else {
      await buildingService.create(form.value);
      showSnackbar("Thêm tòa nhà thành công!", "success");
    }
    closeDialog();
    await loadBuildings();
  } catch (err) {
    showSnackbar(err.message || "Có lỗi xảy ra", "error");
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
    showSnackbar("Đã xóa tòa nhà!", "success");
    deleteDialog.value = false;
    await loadBuildings();
  } catch (err) {
    showSnackbar(err.message || "Có lỗi xảy ra", "error");
  } finally {
    saving.value = false;
  }
}

function showSnackbar(message, type = "success") {
  snackbar.value = { show: true, message, type };
}

// ─── UI helpers ───────────────────────────────────────────────────────────────
function getTypeLabel(type) {
  const map = { Male: "Khu Nam", Female: "Khu Nữ", Mixed: "Hỗn hợp" };
  return map[type] || type;
}
function getTypeColor(type) {
  const map = { Male: "blue", Female: "pink", Mixed: "purple" };
  return map[type] || "grey";
}
function getBuildingColor(type) {
  const map = { Male: "#3b82f6", Female: "#ec4899", Mixed: "#8b5cf6" };
  return map[type] || "#e5e7eb";
}
function getTypeBg(type) {
  const map = { Male: "#eff6ff", Female: "#fdf2f8", Mixed: "#f5f3ff" };
  return map[type] || "#f9fafb";
}
function getTypeIconColor(type) {
  const map = { Male: "#3b82f6", Female: "#ec4899", Mixed: "#8b5cf6" };
  return map[type] || "#9ca3af";
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

:deep(.ant-drawer-close) {
  color: #666;
}
</style>
