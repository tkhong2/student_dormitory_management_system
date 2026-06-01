<template>
  <div>
    <!-- Page Header -->
    <div
      style="
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 20px;
        flex-wrap: wrap;
        gap: 12px;
      "
    >
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">Loại phòng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Quản lý các loại phòng và giá thuê
        </p>
      </div>
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        variant="flat"
        class="rounded-lg font-weight-bold"
        @click="openCreate"
      >
        Thêm loại phòng
      </v-btn>
    </div>

    <!-- Loading -->
    <div
      v-if="loading"
      style="display: flex; justify-content: center; padding: 60px 0"
    >
      <v-progress-circular indeterminate color="primary" size="48" />
    </div>

    <!-- Error -->
    <v-alert
      v-else-if="error"
      type="error"
      variant="tonal"
      rounded="lg"
      class="mb-4"
    >
      {{ error }}
      <template #append>
        <v-btn variant="text" size="small" @click="loadRoomTypes"
          >Thử lại</v-btn
        >
      </template>
    </v-alert>

    <v-alert
      v-if="buildingError"
      type="warning"
      variant="tonal"
      rounded="lg"
      class="mb-4"
    >
      {{ buildingError }}
    </v-alert>

    <!-- Room Type Cards -->
    <v-row v-else>
      <v-col v-for="t in types" :key="t.id" cols="12" sm="6" lg="4">
        <v-card
          style="border: 1px solid #e5e7eb; transition: box-shadow 0.2s"
          class="pa-5 rounded-xl"
          hover
        >
          <div class="d-flex align-center mb-4">
            <v-avatar
              :color="getTypeColor(t.capacity)"
              size="44"
              rounded="lg"
              variant="tonal"
            >
              <v-icon>{{ getTypeIcon(t.capacity) }}</v-icon>
            </v-avatar>
            <div class="ml-3" style="flex: 1; min-width: 0">
              <div
                class="text-subtitle-1 font-weight-bold"
                style="
                  white-space: nowrap;
                  overflow: hidden;
                  text-overflow: ellipsis;
                "
              >
                {{ t.name }}
              </div>
              <div class="text-caption text-medium-emphasis">
                {{ getBuildingName(t.buildingId) }}
              </div>
              <div class="text-caption text-medium-emphasis">
                {{ t.capacity }} sinh viên/phòng
              </div>
            </div>
            <v-menu>
              <template #activator="{ props }">
                <v-btn
                  icon="mdi-dots-vertical"
                  size="small"
                  variant="text"
                  v-bind="props"
                />
              </template>
              <v-list density="compact" rounded="lg" min-width="130">
                <v-list-item
                  prepend-icon="mdi-pencil-outline"
                  title="Sửa"
                  @click="openEdit(t)"
                />
                <v-list-item
                  prepend-icon="mdi-delete-outline"
                  title="Xóa"
                  class="text-error"
                  @click="confirmDelete(t)"
                />
              </v-list>
            </v-menu>
          </div>

          <p
            class="text-body-2 text-medium-emphasis mb-4"
            style="min-height: 40px"
          >
            {{ t.description || "Chưa có mô tả" }}
          </p>

          <v-divider class="mb-4" />

          <div
            class="d-flex justify-space-between align-center"
            style="gap: 8px; flex-wrap: wrap"
          >
            <div>
              <span class="text-h6 font-weight-bold text-primary">{{
                formatPrice(t.pricePerMonth)
              }}</span>
              <span class="text-body-2 text-medium-emphasis">/tháng</span>
            </div>
            <div class="d-flex align-center" style="gap: 8px; flex-wrap: wrap">
              <v-chip
                size="small"
                variant="tonal"
                :color="getTypeColor(t.capacity)"
              >
                {{ t.capacity }} người
              </v-chip>
              <v-chip
                size="small"
                variant="tonal"
                :color="t.isActive ? 'success' : 'error'"
              >
                {{ t.isActive ? "Hoạt động" : "Không hoạt động" }}
              </v-chip>
            </div>
          </div>
        </v-card>
      </v-col>

      <!-- Empty state -->
      <v-col v-if="types.length === 0" cols="12">
        <div style="text-align: center; padding: 60px 0; color: #9ca3af">
          <v-icon size="64" color="grey-lighten-1">mdi-bed-outline</v-icon>
          <p style="margin-top: 12px; font-size: 15px">
            Chưa có loại phòng nào
          </p>
          <v-btn
            color="primary"
            prepend-icon="mdi-plus"
            variant="flat"
            class="mt-2"
            @click="openCreate"
            >Thêm loại phòng</v-btn
          >
        </div>
      </v-col>
    </v-row>

    <!-- Add/Edit Dialog -->
    <v-dialog v-model="dialog" max-width="500" persistent>
      <v-card class="pa-6 rounded-xl">
        <h2 class="text-h6 font-weight-bold mb-5">
          {{ editTarget ? "Chỉnh sửa loại phòng" : "Thêm loại phòng mới" }}
        </h2>

        <v-select
          v-model="form.buildingId"
          :items="buildings"
          item-title="name"
          item-value="id"
          label="Tòa nhà *"
          variant="outlined"
          rounded="lg"
          class="mb-3"
          :error-messages="formErrors.buildingId"
          clearable
        />

        <v-text-field
          v-model="form.code"
          label="Mã loại phòng"
          placeholder="Ví dụ: PHONG-4NGU"
          variant="outlined"
          rounded="lg"
          class="mb-3"
        />

        <v-text-field
          v-model="form.name"
          label="Tên loại phòng *"
          placeholder="Ví dụ: Phòng 4 người"
          variant="outlined"
          rounded="lg"
          class="mb-3"
          :error-messages="formErrors.name"
        />

        <v-row>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.capacity"
              label="Sức chứa *"
              type="number"
              suffix="SV"
              variant="outlined"
              rounded="lg"
              min="1"
              :error-messages="formErrors.capacity"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.pricePerMonth"
              label="Giá thuê *"
              type="number"
              suffix="₫/tháng"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.pricePerMonth"
            />
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.depositAmount"
              label="Tiền cọc"
              type="number"
              suffix="₫"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.depositAmount"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.area"
              label="Diện tích (m²)"
              type="number"
              suffix="m²"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.area"
            />
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.electricityRate"
              label="Giá điện"
              type="number"
              suffix="₫/kWh"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.electricityRate"
            />
          </v-col>
          <v-col cols="6">
            <v-text-field
              v-model.number="form.waterRate"
              label="Giá nước"
              type="number"
              suffix="₫/m³"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.waterRate"
            />
          </v-col>
        </v-row>

        <v-row>
          <v-col cols="6">
            <v-select
              v-model="form.bedType"
              :items="bedTypes"
              label="Loại giường"
              variant="outlined"
              rounded="lg"
            />
          </v-col>
          <v-col cols="6">
            <v-switch v-model="form.isActive" label="Hoạt động" inset />
          </v-col>
        </v-row>

        <v-textarea
          v-model="form.description"
          label="Mô tả tiện nghi"
          placeholder="Mô tả các tiện nghi của loại phòng..."
          variant="outlined"
          rounded="lg"
          rows="3"
          class="mt-1"
        />

        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="closeDialog"
            >Hủy</v-btn
          >
          <v-btn
            color="primary"
            variant="flat"
            class="rounded-lg"
            :loading="saving"
            @click="saveRoomType"
          >
            {{ editTarget ? "Lưu thay đổi" : "Thêm mới" }}
          </v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Delete Confirm Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400">
      <v-card class="rounded-xl pa-6">
        <v-card-title class="text-h6 font-weight-bold px-0"
          >Xác nhận xóa</v-card-title
        >
        <v-card-text class="px-0 py-2">
          Bạn có chắc muốn xóa loại phòng
          <strong>{{ deleteTarget?.name }}</strong
          >?
        </v-card-text>
        <v-card-actions class="px-0 pt-4">
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn
            color="error"
            variant="flat"
            class="rounded-lg"
            :loading="saving"
            @click="deleteRoomType"
            >Xóa</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Snackbar -->
    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      rounded="lg"
      location="bottom right"
    >
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { roomTypeService } from "@/services/roomTypeService";
import { buildingService } from "@/services/buildingService";

// ─── State ────────────────────────────────────────────────────────────────────
const types = ref([]);
const buildings = ref([]);
const loading = ref(false);
const saving = ref(false);
const error = ref(null);
const buildingError = ref(null);

const dialog = ref(false);
const deleteDialog = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);

const form = ref({
  buildingId: null,
  code: "",
  name: "",
  capacity: 4,
  pricePerMonth: 500000,
  depositAmount: 0,
  electricityRate: 0,
  waterRate: 0,
  area: 20,
  bedType: "Single",
  hasAirConditioner: false,
  hasWaterHeater: false,
  hasPrivateBathroom: false,
  hasWindowView: false,
  description: "",
  thumbnailUrl: "",
  isActive: true,
});
const formErrors = ref({});
const snackbar = ref({ show: false, message: "", color: "success" });

const bedTypes = ["Single", "Double", "Suite", "Family"];

// ─── Load ─────────────────────────────────────────────────────────────────────
async function loadRoomTypes() {
  loading.value = true;
  error.value = null;
  try {
    types.value = await roomTypeService.getAll();
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách loại phòng";
  } finally {
    loading.value = false;
  }
}

async function loadBuildings() {
  buildingError.value = null;
  try {
    buildings.value = await buildingService.getAll();
  } catch (err) {
    buildingError.value = err.message || "Không thể tải danh sách tòa nhà";
  }
}

onMounted(async () => {
  await Promise.all([loadBuildings(), loadRoomTypes()]);
});

// ─── CRUD ─────────────────────────────────────────────────────────────────────
function openCreate() {
  editTarget.value = null;
  form.value = {
    buildingId: null,
    code: "",
    name: "",
    capacity: 4,
    pricePerMonth: 500000,
    depositAmount: 0,
    electricityRate: 0,
    waterRate: 0,
    area: 20,
    bedType: "Single",
    hasAirConditioner: false,
    hasWaterHeater: false,
    hasPrivateBathroom: false,
    hasWindowView: false,
    description: "",
    thumbnailUrl: "",
    isActive: true,
  };
  formErrors.value = {};
  dialog.value = true;
}

function openEdit(t) {
  editTarget.value = t;
  form.value = {
    buildingId: t.buildingId,
    code: t.code || "",
    name: t.name,
    capacity: t.capacity,
    pricePerMonth: t.pricePerMonth ?? 0,
    depositAmount: t.depositAmount ?? 0,
    electricityRate: t.electricityRate ?? 0,
    waterRate: t.waterRate ?? 0,
    area: t.area ?? 0,
    bedType: t.bedType || "Single",
    hasAirConditioner: !!t.hasAirConditioner,
    hasWaterHeater: !!t.hasWaterHeater,
    hasPrivateBathroom: !!t.hasPrivateBathroom,
    hasWindowView: !!t.hasWindowView,
    description: t.description || "",
    thumbnailUrl: t.thumbnailUrl || "",
    isActive: t.isActive ?? true,
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
  if (!form.value.buildingId) errors.buildingId = "Vui lòng chọn tòa nhà";
  if (!form.value.name.trim()) errors.name = "Vui lòng nhập tên loại phòng";
  if (!form.value.capacity || form.value.capacity < 1)
    errors.capacity = "Sức chứa phải ≥ 1";
  if (form.value.pricePerMonth === "" || form.value.pricePerMonth < 0)
    errors.pricePerMonth = "Giá thuê không hợp lệ";
  if (form.value.depositAmount < 0)
    errors.depositAmount = "Tiền cọc không hợp lệ";
  if (form.value.electricityRate < 0)
    errors.electricityRate = "Giá điện không hợp lệ";
  if (form.value.waterRate < 0) errors.waterRate = "Giá nước không hợp lệ";
  if (form.value.area <= 0) errors.area = "Diện tích phải lớn hơn 0";
  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

function generateCode(name) {
  return name
    .trim()
    .toUpperCase()
    .replace(/\s+/g, "-")
    .replace(/[^A-Z0-9-]/g, "");
}

async function saveRoomType() {
  if (!validate()) return;
  saving.value = true;
  try {
    const payload = {
      buildingId: form.value.buildingId,
      code: form.value.code.trim() || generateCode(form.value.name),
      name: form.value.name.trim(),
      capacity: Number(form.value.capacity),
      pricePerMonth: Number(form.value.pricePerMonth),
      depositAmount: Number(form.value.depositAmount),
      electricityRate: Number(form.value.electricityRate),
      waterRate: Number(form.value.waterRate),
      area: Number(form.value.area),
      bedType: form.value.bedType,
      hasAirConditioner: form.value.hasAirConditioner,
      hasWaterHeater: form.value.hasWaterHeater,
      hasPrivateBathroom: form.value.hasPrivateBathroom,
      hasWindowView: form.value.hasWindowView,
      description: form.value.description.trim() || null,
      thumbnailUrl: form.value.thumbnailUrl.trim() || null,
      isActive: form.value.isActive,
    };

    if (editTarget.value) {
      await roomTypeService.update(editTarget.value.id, payload);
      showSnackbar("Cập nhật loại phòng thành công!", "success");
    } else {
      await roomTypeService.create(payload);
      showSnackbar("Thêm loại phòng thành công!", "success");
    }

    closeDialog();
    await loadRoomTypes();
  } catch (err) {
    showSnackbar(err.message || "Có lỗi xảy ra", "error");
  } finally {
    saving.value = false;
  }
}

function confirmDelete(t) {
  deleteTarget.value = t;
  deleteDialog.value = true;
}

async function deleteRoomType() {
  saving.value = true;
  try {
    await roomTypeService.delete(deleteTarget.value.id);
    showSnackbar("Đã xóa loại phòng!", "success");
    deleteDialog.value = false;
    await loadRoomTypes();
  } catch (err) {
    showSnackbar(err.message || "Có lỗi xảy ra", "error");
  } finally {
    saving.value = false;
  }
}

function showSnackbar(message, color = "success") {
  snackbar.value = { show: true, message, color };
}

// ─── UI helpers ───────────────────────────────────────────────────────────────
function formatPrice(price) {
  return new Intl.NumberFormat("vi-VN").format(price) + "₫";
}

function getTypeIcon(capacity) {
  if (capacity <= 2) return "mdi-bed-king-outline";
  if (capacity <= 4) return "mdi-bed-outline";
  if (capacity <= 6) return "mdi-bunk-bed-outline";
  return "mdi-bunk-bed-outline";
}

function getTypeColor(capacity) {
  if (capacity <= 2) return "warning";
  if (capacity <= 4) return "primary";
  if (capacity <= 6) return "secondary";
  return "info";
}

function getBuildingName(buildingId) {
  const building = buildings.value.find((item) => item.id === buildingId);
  return building?.name || "Không xác định";
}
</script>
