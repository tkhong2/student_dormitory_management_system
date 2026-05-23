<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; flex-wrap: wrap; gap: 12px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Loại phòng</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Quản lý các loại phòng và giá thuê</p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" variant="flat" class="rounded-lg font-weight-bold" @click="openCreate">
        Thêm loại phòng
      </v-btn>
    </div>

    <!-- Loading -->
    <div v-if="loading" style="display: flex; justify-content: center; padding: 60px 0;">
      <v-progress-circular indeterminate color="primary" size="48" />
    </div>

    <!-- Error -->
    <v-alert v-else-if="error" type="error" variant="tonal" rounded="lg" class="mb-4">
      {{ error }}
      <template #append>
        <v-btn variant="text" size="small" @click="loadRoomTypes">Thử lại</v-btn>
      </template>
    </v-alert>

    <!-- Room Type Cards -->
    <v-row v-else>
      <v-col v-for="t in types" :key="t.id" cols="12" sm="6" lg="4">
        <v-card style="border: 1px solid #e5e7eb; transition: box-shadow 0.2s;" class="pa-5 rounded-xl" hover>
          <div class="d-flex align-center mb-4">
            <v-avatar :color="getTypeColor(t.capacity)" size="44" rounded="lg" variant="tonal">
              <v-icon>{{ getTypeIcon(t.capacity) }}</v-icon>
            </v-avatar>
            <div class="ml-3" style="flex: 1; min-width: 0;">
              <div class="text-subtitle-1 font-weight-bold" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">{{ t.name }}</div>
              <div class="text-caption text-medium-emphasis">{{ t.capacity }} sinh viên/phòng</div>
            </div>
            <v-menu>
              <template #activator="{ props }">
                <v-btn icon="mdi-dots-vertical" size="small" variant="text" v-bind="props" />
              </template>
              <v-list density="compact" rounded="lg" min-width="130">
                <v-list-item prepend-icon="mdi-pencil-outline" title="Sửa" @click="openEdit(t)" />
                <v-list-item prepend-icon="mdi-delete-outline" title="Xóa" class="text-error" @click="confirmDelete(t)" />
              </v-list>
            </v-menu>
          </div>

          <p class="text-body-2 text-medium-emphasis mb-4" style="min-height: 40px;">
            {{ t.description || 'Chưa có mô tả' }}
          </p>

          <v-divider class="mb-4" />

          <div class="d-flex justify-space-between align-center">
            <div>
              <span class="text-h6 font-weight-bold text-primary">{{ formatPrice(t.price) }}</span>
              <span class="text-body-2 text-medium-emphasis">/tháng</span>
            </div>
            <v-chip size="small" variant="tonal" :color="getTypeColor(t.capacity)">
              {{ t.capacity }} người
            </v-chip>
          </div>
        </v-card>
      </v-col>

      <!-- Empty state -->
      <v-col v-if="types.length === 0" cols="12">
        <div style="text-align: center; padding: 60px 0; color: #9ca3af;">
          <v-icon size="64" color="grey-lighten-1">mdi-bed-outline</v-icon>
          <p style="margin-top: 12px; font-size: 15px;">Chưa có loại phòng nào</p>
          <v-btn color="primary" prepend-icon="mdi-plus" variant="flat" class="mt-2" @click="openCreate">Thêm loại phòng</v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Add/Edit Dialog -->
    <v-dialog v-model="dialog" max-width="500" persistent>
      <v-card class="pa-6 rounded-xl">
        <h2 class="text-h6 font-weight-bold mb-5">
          {{ editTarget ? 'Chỉnh sửa loại phòng' : 'Thêm loại phòng mới' }}
        </h2>

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
              v-model.number="form.price"
              label="Giá thuê *"
              type="number"
              suffix="₫/tháng"
              variant="outlined"
              rounded="lg"
              min="0"
              :error-messages="formErrors.price"
            />
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
          <v-btn variant="text" :disabled="saving" @click="closeDialog">Hủy</v-btn>
          <v-btn color="primary" variant="flat" class="rounded-lg" :loading="saving" @click="saveRoomType">
            {{ editTarget ? 'Lưu thay đổi' : 'Thêm mới' }}
          </v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Delete Confirm Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400">
      <v-card class="rounded-xl pa-6">
        <v-card-title class="text-h6 font-weight-bold px-0">Xác nhận xóa</v-card-title>
        <v-card-text class="px-0 py-2">
          Bạn có chắc muốn xóa loại phòng <strong>{{ deleteTarget?.name }}</strong>?
        </v-card-text>
        <v-card-actions class="px-0 pt-4">
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" variant="flat" class="rounded-lg" :loading="saving" @click="deleteRoomType">Xóa</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Snackbar -->
    <v-snackbar v-model="snackbar.show" :color="snackbar.color" rounded="lg" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { roomTypeService } from '@/services/roomTypeService'

// ─── State ────────────────────────────────────────────────────────────────────
const types   = ref([])
const loading = ref(false)
const error   = ref(null)
const saving  = ref(false)

const dialog      = ref(false)
const deleteDialog = ref(false)
const editTarget  = ref(null)
const deleteTarget = ref(null)

const form = ref({ name: '', capacity: 4, price: 500000, description: '' })
const formErrors = ref({})
const snackbar = ref({ show: false, message: '', color: 'success' })

// ─── Load ─────────────────────────────────────────────────────────────────────
async function loadRoomTypes() {
  loading.value = true
  error.value   = null
  try {
    types.value = await roomTypeService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách loại phòng'
  } finally {
    loading.value = false
  }
}

onMounted(loadRoomTypes)

// ─── CRUD ─────────────────────────────────────────────────────────────────────
function openCreate() {
  editTarget.value = null
  form.value = { name: '', capacity: 4, price: 500000, description: '' }
  formErrors.value = {}
  dialog.value = true
}

function openEdit(t) {
  editTarget.value = t
  form.value = { name: t.name, capacity: t.capacity, price: t.price, description: t.description }
  formErrors.value = {}
  dialog.value = true
}

function closeDialog() {
  dialog.value = false
  editTarget.value = null
}

function validate() {
  const errors = {}
  if (!form.value.name.trim())         errors.name     = 'Vui lòng nhập tên loại phòng'
  if (!form.value.capacity || form.value.capacity < 1) errors.capacity = 'Sức chứa phải ≥ 1'
  if (form.value.price === '' || form.value.price < 0) errors.price    = 'Giá không hợp lệ'
  formErrors.value = errors
  return Object.keys(errors).length === 0
}

async function saveRoomType() {
  if (!validate()) return
  saving.value = true
  try {
    const payload = {
      name:        form.value.name,
      capacity:    form.value.capacity,
      price:       form.value.price,
      description: form.value.description,
    }
    if (editTarget.value) {
      await roomTypeService.update(editTarget.value.id, payload)
      showSnackbar('Cập nhật loại phòng thành công!', 'success')
    } else {
      await roomTypeService.create(payload)
      showSnackbar('Thêm loại phòng thành công!', 'success')
    }
    closeDialog()
    await loadRoomTypes()
  } catch (err) {
    showSnackbar(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

function confirmDelete(t) {
  deleteTarget.value = t
  deleteDialog.value = true
}

async function deleteRoomType() {
  saving.value = true
  try {
    await roomTypeService.delete(deleteTarget.value.id)
    showSnackbar('Đã xóa loại phòng!', 'success')
    deleteDialog.value = false
    await loadRoomTypes()
  } catch (err) {
    showSnackbar(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

function showSnackbar(message, color = 'success') {
  snackbar.value = { show: true, message, color }
}

// ─── UI helpers ───────────────────────────────────────────────────────────────
function formatPrice(price) {
  return new Intl.NumberFormat('vi-VN').format(price) + '₫'
}

function getTypeIcon(capacity) {
  if (capacity <= 2)  return 'mdi-bed-king-outline'
  if (capacity <= 4)  return 'mdi-bed-outline'
  if (capacity <= 6)  return 'mdi-bunk-bed-outline'
  return 'mdi-bunk-bed-outline'
}

function getTypeColor(capacity) {
  if (capacity <= 2)  return 'warning'
  if (capacity <= 4)  return 'primary'
  if (capacity <= 6)  return 'secondary'
  return 'info'
}
</script>
