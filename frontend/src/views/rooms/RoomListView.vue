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
          Quản lý Phòng ở
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Tổng <strong>{{ rooms.length }}</strong> phòng —
          <span style="color: #16a34a"
            >{{ countByStatus("Available") }} trống</span
          >
          ·
          <span style="color: #2563eb"
            >{{ countByStatus("Occupied") }} đang ở</span
          >
          ·
          <span style="color: #dc2626">{{ countByStatus("Full") }} đã đầy</span>
          ·
          <span style="color: #d97706"
            >{{ countByStatus("Maintenance") }} bảo trì</span
          >
        </p>
      </div>
      <a-button type="primary" @click="openCreate">
        <template #icon><PlusOutlined /></template>
        Thêm phòng
      </a-button>
    </div>

    <!-- Filters -->
    <a-card style="margin-bottom: 16px; background: #fafafa">
      <a-row :gutter="16">
        <a-col :span="6">
          <a-select
            v-model:value="filterBuilding"
            placeholder="Tòa nhà"
            style="width: 100%"
            :loading="loadingBuildings"
          >
            <a-select-option value="all">Tất cả tòa nhà</a-select-option>
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-select
            v-model:value="filterStatus"
            placeholder="Trạng thái"
            style="width: 100%"
          >
            <a-select-option value="all">Tất cả trạng thái</a-select-option>
            <a-select-option value="Available">Trống</a-select-option>
            <a-select-option value="Occupied">Đang ở</a-select-option>
            <a-select-option value="Full">Đã đầy</a-select-option>
            <a-select-option value="Maintenance">Bảo trì</a-select-option>
            <a-select-option value="Reserved">Đã đặt</a-select-option>
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-select
            v-model:value="filterFloor"
            placeholder="Tầng"
            style="width: 100%"
          >
            <a-select-option value="all">Tất cả tầng</a-select-option>
            <a-select-option v-for="f in floorOptions" :key="f" :value="f"
              >Tầng {{ f }}</a-select-option
            >
          </a-select>
        </a-col>
        <a-col :span="6">
          <a-input
            v-model:value="searchText"
            placeholder="Tìm số phòng..."
            allow-clear
          />
        </a-col>
      </a-row>
    </a-card>

    <!-- Loading -->
    <div
      v-if="loading"
      style="display: flex; justify-content: center; padding: 60px 0"
    >
      <a-spin size="large" tip="Đang tải dữ liệu..." />
    </div>

    <!-- Error -->
    <a-alert
      v-else-if="error"
      type="error"
      :message="error"
      show-icon
      style="margin-bottom: 16px"
    >
      <template #action>
        <a-button size="small" @click="loadRooms">Thử lại</a-button>
      </template>
    </a-alert>

    <!-- Room Grid -->
    <template v-else>
      <!-- Empty -->
      <div
        v-if="filteredRooms.length === 0"
        style="text-align: center; padding: 60px 0; color: #9ca3af"
      >
        <HomeOutlined style="font-size: 48px" />
        <p style="margin-top: 12px; font-size: 15px">
          Không tìm thấy phòng nào
        </p>
      </div>

      <div style="display: flex; flex-wrap: wrap; margin: -6px">
        <div
          v-for="r in filteredRooms"
          :key="r.id"
          style="width: 20%; padding: 6px"
        >
          <a-card
            hoverable
            @click="selectRoom(r)"
            style="
              text-align: center;
              border-radius: 12px;
              overflow: hidden;
              padding: 0;
            "
            :body-style="{ padding: '16px 12px 14px' }"
          >
            <!-- Status bar on top -->
            <div
              :style="{
                height: '4px',
                background: getStatusColor(r.status),
                marginBottom: '12px',
                marginTop: '-16px',
                marginLeft: '-12px',
                marginRight: '-12px',
              }"
            />

            <!-- Image / Icon -->
            <div
              v-if="r.imageUrl"
              :style="{
                width: '100%',
                height: '120px',
                borderRadius: '8px',
                overflow: 'hidden',
                marginBottom: '12px',
                marginTop: '-4px',
                background: '#f5f5f5',
              }"
            >
              <img
                :src="r.imageUrl"
                style="width: 100%; height: 100%; object-fit: cover"
              />
            </div>
            <div
              v-else
              :style="{
                width: '44px',
                height: '44px',
                background: getStatusBg(r.status),
                borderRadius: '10px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                margin: '0 auto 10px',
              }"
            >
              <HomeOutlined
                :style="{ fontSize: '22px', color: getStatusColor(r.status) }"
              />
            </div>
            <div style="font-weight: 700; font-size: 16px; margin-bottom: 2px">
              {{ r.roomNumber }}
            </div>
            <div style="font-size: 12px; color: #8c8c8c; margin-bottom: 8px">
              {{ getBuildingName(r.buildingId) }} · Tầng {{ r.floorNumber }}
            </div>
            <a-tag
              :color="getStatusTagColor(r.status)"
              style="margin-bottom: 8px; font-size: 11px"
            >
              {{ getStatusLabel(r.status) }}
            </a-tag>
            <a-progress
              :percent="getCapacityPercent(r)"
              :stroke-color="getStatusColor(r.status)"
              size="small"
            />
            <div style="font-size: 11px; color: #8c8c8c; margin-top: 4px">
              {{ r.currentOccupancy }}/{{ getRoomCapacity(r) }} người
            </div>
            <div style="display: flex; gap: 8px; margin-top: 12px">
              <a-button size="small" block @click.stop="openEdit(r)">
                <template #icon><EditOutlined /></template>
                Sửa
              </a-button>
              <a-button
                size="small"
                danger
                block
                @click.stop="confirmDelete(r)"
              >
                <template #icon><DeleteOutlined /></template>
                Xóa
              </a-button>
            </div>
          </a-card>
        </div>
      </div>
    </template>

    <!-- Room Detail Drawer -->
    <a-drawer
      v-model:open="detailDrawer"
      title="Chi tiết phòng"
      placement="right"
      width="400"
    >
      <div v-if="selectedRoom">
        <!-- Room Image Cover in Drawer -->
        <div
          v-if="selectedRoom.imageUrl"
          style="
            margin: -24px -24px 20px -24px;
            height: 160px;
            overflow: hidden;
          "
        >
          <img
            :src="selectedRoom.imageUrl"
            style="width: 100%; height: 100%; object-fit: cover"
          />
        </div>

        <a-tag
          :color="getStatusTagColor(selectedRoom.status)"
          style="margin-bottom: 16px"
        >
          {{ getStatusLabel(selectedRoom.status) }}
        </a-tag>
        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="Số phòng">{{
            selectedRoom.roomNumber
          }}</a-descriptions-item>
          <a-descriptions-item label="Tòa nhà">{{
            getBuildingName(selectedRoom.buildingId)
          }}</a-descriptions-item>
          <a-descriptions-item label="Tầng"
            >Tầng {{ selectedRoom.floor }}</a-descriptions-item
          >
          <a-descriptions-item label="Loại phòng">{{
            getRoomTypeName(selectedRoom.roomTypeId)
          }}</a-descriptions-item>
          <a-descriptions-item label="Sức chứa">
            {{ selectedRoom.currentOccupancy }}/{{
              getRoomCapacity(selectedRoom)
            }}
            sinh viên
          </a-descriptions-item>
        </a-descriptions>

        <a-divider />

        <div style="display: flex; gap: 8px; margin-bottom: 16px">
          <a-button type="primary" @click="openEdit(selectedRoom)">
            <template #icon><EditOutlined /></template>
            Chỉnh sửa
          </a-button>
          <a-button danger @click="confirmDelete(selectedRoom)">
            <template #icon><DeleteOutlined /></template>
            Xóa
          </a-button>
        </div>

        <h4 style="font-weight: 700; margin-bottom: 12px">
          Sinh viên trong phòng
        </h4>
        <div v-if="selectedRoom.currentOccupancy > 0">
          <div
            v-for="i in selectedRoom.currentOccupancy"
            :key="i"
            style="
              display: flex;
              align-items: center;
              gap: 12px;
              margin-bottom: 12px;
            "
          >
            <a-avatar :style="{ backgroundColor: '#ff6b00' }">
              <template #icon><UserOutlined /></template>
            </a-avatar>
            <div>
              <div style="font-weight: 600">Sinh viên {{ i }}</div>
              <div style="font-size: 12px; color: #8c8c8c">SV00{{ i }}</div>
            </div>
          </div>
        </div>
        <a-empty v-else description="Chưa có sinh viên" />
      </div>
    </a-drawer>

    <!-- Create / Edit Modal -->
    <a-modal
      v-model:open="formModalOpen"
      :title="editTarget ? 'Chỉnh sửa phòng' : 'Thêm phòng mới'"
      centered
      :confirm-loading="saving"
      ok-text="Lưu"
      cancel-text="Hủy"
      @ok="saveRoom"
      @cancel="closeFormModal"
    >
      <a-form layout="vertical">
        <a-form-item
          label="Số phòng"
          :validate-status="formErrors.roomNumber ? 'error' : ''"
          :help="formErrors.roomNumber"
        >
          <a-input v-model:value="form.roomNumber" placeholder="Ví dụ: A101" />
        </a-form-item>

        <a-form-item
          label="Tòa nhà"
          :validate-status="formErrors.buildingId ? 'error' : ''"
          :help="formErrors.buildingId"
        >
          <a-select
            v-model:value="form.buildingId"
            placeholder="Chọn tòa nhà"
            :loading="loadingBuildings"
            @change="onBuildingChange"
          >
            <a-select-option v-for="b in buildings" :key="b.id" :value="b.id">
              {{ b.name }}
            </a-select-option>
          </a-select>
          <div
            v-if="buildings.length === 0"
            style="font-size: 12px; color: #ff4d4f; margin-top: 4px"
          >
            Chưa có tòa nhà nào. Vui lòng tạo tòa nhà trước.
          </div>
        </a-form-item>

        <a-form-item
          label="Tầng"
          :validate-status="formErrors.floorId ? 'error' : ''"
          :help="formErrors.floorId"
        >
          <a-select
            v-model:value="form.floorId"
            placeholder="Chọn tầng"
            :disabled="!form.buildingId"
          >
            <a-select-option
              v-for="floor in buildingFloorOptions"
              :key="floor.id"
              :value="floor.id"
            >
              {{ floor.label }}
            </a-select-option>
          </a-select>
          <div
            v-if="!form.buildingId"
            style="font-size: 12px; color: #8c8c8c; margin-top: 4px"
          >
            Vui lòng chọn tòa nhà trước
          </div>
          <div
            v-else-if="buildingFloorOptions.length === 0"
            style="font-size: 12px; color: #ff4d4f; margin-top: 4px"
          >
            Tòa nhà này chưa có tầng. Vui lòng tạo tầng trước.
          </div>
        </a-form-item>

        <a-form-item
          label="Loại phòng"
          :validate-status="formErrors.roomTypeId ? 'error' : ''"
          :help="formErrors.roomTypeId"
        >
          <a-select
            v-model:value="form.roomTypeId"
            placeholder="Chọn loại phòng"
            :disabled="!form.buildingId"
          >
            <a-select-option
              v-for="rt in filteredRoomTypes"
              :key="rt.id"
              :value="rt.id"
            >
              {{ rt.name }} ({{ rt.capacity }} người)
            </a-select-option>
          </a-select>
          <div
            v-if="!form.buildingId"
            style="font-size: 12px; color: #8c8c8c; margin-top: 4px"
          >
            Vui lòng chọn tòa nhà trước
          </div>
          <div
            v-else-if="filteredRoomTypes.length === 0"
            style="font-size: 12px; color: #ff4d4f; margin-top: 4px"
          >
            Tòa nhà này chưa có loại phòng. Vui lòng tạo loại phòng trước.
          </div>
        </a-form-item>

        <a-form-item
          label="Trạng thái"
          :validate-status="formErrors.status ? 'error' : ''"
          :help="formErrors.status"
        >
          <a-select v-model:value="form.status" placeholder="Chọn trạng thái">
            <a-select-option
              v-for="item in roomStatusOptions"
              :key="item.value"
              :value="item.value"
            >
              {{ item.label }}
            </a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item
          label="Số người hiện tại"
          :validate-status="formErrors.currentOccupancy ? 'error' : ''"
          :help="formErrors.currentOccupancy"
        >
          <a-input-number
            v-model:value="form.currentOccupancy"
            :min="0"
            style="width: 100%"
          />
        </a-form-item>

        <a-form-item label="Ảnh phòng">
          <a-input
            v-model:value="form.imageUrl"
            placeholder="https://... hoặc /images/..."
          />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Delete Modal -->
    <a-modal
      v-model:open="deleteModalOpen"
      title="Xác nhận xóa"
      centered
      :confirm-loading="saving"
      ok-text="Xóa"
      cancel-text="Hủy"
      :ok-button-props="{ danger: true }"
      @ok="deleteRoom"
      @cancel="closeDeleteModal"
    >
      <p>
        Bạn có chắc muốn xóa phòng
        <strong>{{ deleteTarget?.roomNumber }}</strong> không?
      </p>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import { message } from "ant-design-vue";
import {
  HomeOutlined,
  UserOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import { roomService } from "@/services/roomService";
import { buildingService } from "@/services/buildingService";
import { roomTypeService } from "@/services/roomTypeService";
import { floorService } from "@/services/floorService";

// ─── State ────────────────────────────────────────────────────────────────────
const rooms = ref([]);
const buildings = ref([]);
const roomTypes = ref([]);
const buildingFloors = ref([]);
const loading = ref(false);
const loadingBuildings = ref(false);
const error = ref(null);

const filterBuilding = ref("all");
const filterStatus = ref("all");
const filterFloor = ref("all");
const searchText = ref("");

const detailDrawer = ref(false);
const selectedRoom = ref(null);
const saving = ref(false);

const formModalOpen = ref(false);
const deleteModalOpen = ref(false);
const editTarget = ref(null);
const deleteTarget = ref(null);
const formErrors = ref({});

const form = ref(getDefaultForm());

const roomStatusOptions = [
  { label: "Trống", value: "Available" },
  { label: "Đang ở", value: "Occupied" },
  { label: "Bảo trì", value: "Maintenance" },
  { label: "Đã đặt", value: "Reserved" },
];

// ─── Computed ─────────────────────────────────────────────────────────────────
const floorOptions = computed(() => {
  const floors = [...new Set(rooms.value.map((r) => r.floorNumber))].sort(
    (a, b) => a - b,
  );
  return floors;
});

const buildingFloorOptions = computed(() => {
  console.log(
    "🔢 buildingFloorOptions computed called, buildingFloors:",
    buildingFloors.value,
  );
  const result = buildingFloors.value.map((floor) => ({
    id: floor.id,
    number: floor.floorNumber,
    label: floor.label,
  }));
  if (result.length > 0) {
    console.log("🏗️ First option structure:", result[0]);
    console.log("🔢 All options:", result);
  }
  return result;
});

const filteredRoomTypes = computed(() => {
  if (!form.value.buildingId) return [];
  return roomTypes.value.filter(
    (rt) => rt.buildingId === form.value.buildingId,
  );
});

const filteredRooms = computed(() => {
  return rooms.value.filter((r) => {
    const buildingMatch =
      filterBuilding.value === "all" || r.buildingId === filterBuilding.value;
    const statusMatch =
      filterStatus.value === "all" || r.status === filterStatus.value;
    const floorMatch =
      filterFloor.value === "all" || r.floorNumber === filterFloor.value;
    const searchMatch =
      !searchText.value ||
      r.roomNumber.toLowerCase().includes(searchText.value.toLowerCase());
    return buildingMatch && statusMatch && floorMatch && searchMatch;
  });
});

// ─── Load data ────────────────────────────────────────────────────────────────
function normalizeRoom(room) {
  return {
    ...room,
    currentOccupancy: room.currentOccupants ?? room.currentOccupancy ?? 0,
    imageUrl: room.imageUrl ?? room.imageUrl ?? null,
  };
}

async function loadRooms() {
  loading.value = true;
  error.value = null;
  try {
    rooms.value = (await roomService.getAll()).map(normalizeRoom);
  } catch (err) {
    error.value = err.message || "Không thể tải danh sách phòng";
  } finally {
    loading.value = false;
  }
}

async function loadBuildings() {
  loadingBuildings.value = true;
  try {
    buildings.value = await buildingService.getAll();
  } catch (_) {
    // silent fail
  } finally {
    loadingBuildings.value = false;
  }
}

async function loadRoomTypes() {
  try {
    roomTypes.value = await roomTypeService.getAll();
  } catch (_) {
    // silent fail
  }
}

async function loadFloors(buildingId) {
  if (!buildingId) {
    buildingFloors.value = [];
    return;
  }

  try {
    console.log("🔄 loadFloors called with buildingId:", buildingId);
    const floors = await floorService.getByBuildingId(buildingId);
    console.log("✅ Floors fetched:", floors);
    if (floors && floors.length > 0) {
      console.log("🏗️ First floor structure:", {
        id: floors[0].id,
        floorNumber: floors[0].floorNumber,
        label: floors[0].label,
        fullObject: floors[0],
      });
    }
    buildingFloors.value = floors;
  } catch (err) {
    console.error("❌ Error loading floors:", err);
    buildingFloors.value = [];
  }
}

function onBuildingChange(newBuildingId) {
  form.value.floorId = undefined;
  form.value.roomTypeId = undefined;
  return loadFloors(newBuildingId || form.value.buildingId);
}

onMounted(() => {
  loadRooms();
  loadBuildings();
  loadRoomTypes();
});

// ─── Helpers ──────────────────────────────────────────────────────────────────
function getDefaultForm() {
  return {
    roomNumber: "",
    floorId: undefined,
    buildingId: undefined,
    roomTypeId: undefined,
    status: "Available",
    currentOccupancy: 0,
    imageUrl: "",
  };
}

function countByStatus(status) {
  return rooms.value.filter((r) => r.status === status).length;
}

function getBuildingName(buildingId) {
  const b = buildings.value.find((b) => b.id === buildingId);
  return b ? b.name : buildingId?.slice(0, 8) + "...";
}

function getRoomTypeName(roomTypeId) {
  const rt = roomTypes.value.find((rt) => rt.id === roomTypeId);
  return rt ? rt.name : "Không rõ";
}

function getRoomCapacity(room) {
  const rt = roomTypes.value.find((rt) => rt.id === room.roomTypeId);
  return rt ? rt.capacity : "?";
}

function getCapacityPercent(room) {
  const cap = getRoomCapacity(room);
  if (!cap || cap === "?") return 0;
  return Math.round((room.currentOccupancy / cap) * 100);
}

function selectRoom(r) {
  selectedRoom.value = r;
  detailDrawer.value = true;
}

function openCreate() {
  editTarget.value = null;
  form.value = getDefaultForm();
  formErrors.value = {};
  buildingFloors.value = [];
  formModalOpen.value = true;
}

async function openEdit(room) {
  if (!room) return;
  console.log("📝 openEdit called with room:", room);
  editTarget.value = room;
  buildingFloors.value = [];
  form.value = {
    roomNumber: room.roomNumber,
    floorId: room.floorId,
    buildingId: room.buildingId,
    roomTypeId: room.roomTypeId,
    status: room.status,
    currentOccupancy: room.currentOccupancy,
    imageUrl: room.imageUrl || "",
  };
  console.log("✏️ Form after init, floorId set to:", form.value.floorId);
  formErrors.value = {};
  console.log("🏢 About to load floors for buildingId:", room.buildingId);
  await loadFloors(room.buildingId);
  console.log("📊 buildingFloors after load:", buildingFloors.value);
  console.log("📊 buildingFloorOptions computed:", buildingFloorOptions.value);
  console.log(
    "🔍 Looking for floor with id:",
    form.value.floorId,
    "in options:",
    buildingFloorOptions.value.map((o) => ({ id: o.id, label: o.label })),
  );
  formModalOpen.value = true;
}

function closeFormModal() {
  formModalOpen.value = false;
  editTarget.value = null;
  formErrors.value = {};
}

function confirmDelete(room) {
  if (!room) return;
  deleteTarget.value = room;
  deleteModalOpen.value = true;
}

function closeDeleteModal() {
  deleteModalOpen.value = false;
  deleteTarget.value = null;
}

function validateForm() {
  const errors = {};
  const selectedRoomType = roomTypes.value.find(
    (rt) => rt.id === form.value.roomTypeId,
  );

  if (!form.value.roomNumber?.trim())
    errors.roomNumber = "Vui lòng nhập số phòng";
  if (!form.value.buildingId) errors.buildingId = "Vui lòng chọn tòa nhà";
  if (!form.value.floorId) errors.floorId = "Vui lòng chọn tầng";
  if (!form.value.roomTypeId) errors.roomTypeId = "Vui lòng chọn loại phòng";
  if (!form.value.status) errors.status = "Vui lòng chọn trạng thái";
  if (form.value.currentOccupancy == null || form.value.currentOccupancy < 0) {
    errors.currentOccupancy = "Số người hiện tại không hợp lệ";
  } else if (
    selectedRoomType &&
    form.value.currentOccupancy > selectedRoomType.capacity
  ) {
    errors.currentOccupancy = `Số người hiện tại không được vượt quá ${selectedRoomType.capacity}`;
  }

  formErrors.value = errors;
  return Object.keys(errors).length === 0;
}

async function saveRoom() {
  if (!validateForm()) return;

  saving.value = true;
  try {
    const payload = {
      roomNumber: form.value.roomNumber.trim(),
      floorId: form.value.floorId,
      buildingId: form.value.buildingId,
      roomTypeId: form.value.roomTypeId,
      status: form.value.status,
      currentOccupants: form.value.currentOccupancy,
      imageUrl: form.value.imageUrl?.trim() || null,
    };

    if (editTarget.value) {
      await roomService.update(editTarget.value.id, payload);
      message.success("Cập nhật phòng thành công");
      if (selectedRoom.value?.id === editTarget.value.id) {
        detailDrawer.value = false;
        selectedRoom.value = null;
      }
    } else {
      await roomService.create(payload);
      message.success("Thêm phòng thành công");
    }

    closeFormModal();
    await loadRooms();
  } catch (err) {
    message.error(err.message || "Có lỗi xảy ra");
  } finally {
    saving.value = false;
  }
}

async function deleteRoom() {
  if (!deleteTarget.value) return;

  saving.value = true;
  try {
    await roomService.delete(deleteTarget.value.id);
    message.success("Đã xóa phòng");
    if (selectedRoom.value?.id === deleteTarget.value.id) {
      detailDrawer.value = false;
      selectedRoom.value = null;
    }
    closeDeleteModal();
    await loadRooms();
  } catch (err) {
    message.error(err.message || "Không thể xóa phòng");
  } finally {
    saving.value = false;
  }
}

const statusMap = {
  Available: {
    label: "Trống",
    color: "#16a34a",
    bg: "#f0fdf4",
    tag: "success",
  },
  Occupied: {
    label: "Đang ở",
    color: "#2563eb",
    bg: "#eff6ff",
    tag: "processing",
  },
  Full: { label: "Đã đầy", color: "#dc2626", bg: "#fff1f2", tag: "error" },
  Maintenance: {
    label: "Bảo trì",
    color: "#d97706",
    bg: "#fffbeb",
    tag: "warning",
  },
  Reserved: {
    label: "Đã đặt",
    color: "#7c3aed",
    bg: "#f5f3ff",
    tag: "default",
  },
};

function getStatusLabel(s) {
  return statusMap[s]?.label || s;
}
function getStatusColor(s) {
  return statusMap[s]?.color || "#9ca3af";
}
function getStatusBg(s) {
  return statusMap[s]?.bg || "#f9fafb";
}
function getStatusTagColor(s) {
  return statusMap[s]?.tag || "default";
}
</script>
