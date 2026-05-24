<template>
  <div>
    <!-- Page Header -->
    <div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; flex-wrap: wrap; gap: 12px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Quản lý Tòa nhà</h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">
          Danh sách các dãy nhà trong khuôn viên KTX
        </p>
      </div>
      <v-btn style="color:white;" color="primary" prepend-icon="mdi-plus" variant="flat" class="rounded-lg font-weight-bold " @click="openCreate">
        Thêm tòa nhà
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
        <v-btn variant="text" size="small" @click="loadBuildings">Thử lại</v-btn>
      </template>
    </v-alert>

    <!-- Building Grid -->
    <v-row v-else>
      <v-col v-for="b in buildings" :key="b.id" cols="12" sm="6" lg="3">
        <v-card
          rounded="xl"
          class="pa-0"
          style="border: 1px solid #e5e7eb; overflow: hidden; transition: box-shadow 0.2s;"
          hover
        >
          <!-- Color Header by type -->
          <div :style="{ background: getBuildingColor(b.type), height: '6px' }" />

          <!-- Image -->
          <div v-if="b.imageUrl" style="height: 140px; position: relative;">
            <img :src="b.imageUrl" style="width: 100%; height: 100%; object-fit: cover;" />
          </div>
          <!-- Header image placeholder with icon (Fallback) -->
          <div
            v-else
            :style="{
              background: getTypeBg(b.type),
              height: '140px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }"
          >
            <v-icon size="48" :color="getTypeIconColor(b.type)">mdi-office-building</v-icon>
          </div>

          <div class="pa-5">
            <!-- Title row -->
            <div style="display: flex; align-items: flex-start; justify-content: space-between; margin-bottom: 12px;">
              <div>
                <div style="font-size: 17px; font-weight: 700; margin-bottom: 2px;">{{ b.name }}</div>
                <v-chip :color="getTypeColor(b.type)" size="x-small" variant="tonal" label>
                  {{ getTypeLabel(b.type) }}
                </v-chip>
              </div>
              <v-menu>
                <template #activator="{ props }">
                  <v-btn icon="mdi-dots-vertical" size="small" variant="text" v-bind="props" />
                </template>
                <v-list density="compact" rounded="lg" min-width="140">
                  <v-list-item prepend-icon="mdi-pencil-outline" title="Chỉnh sửa" @click="openEdit(b)" />
                  <v-list-item prepend-icon="mdi-delete-outline" title="Xóa" class="text-error" @click="confirmDelete(b)" />
                </v-list>
              </v-menu>
            </div>

            <!-- Description -->
            <p style="font-size: 13px; color: #6b7280; margin-bottom: 14px; min-height: 36px;">
              {{ b.description || 'Chưa có mô tả' }}
            </p>

            <!-- Stats -->
            <div style="display: flex; gap: 16px;">
              <div style="text-align: center; flex: 1; background: #f9fafb; border-radius: 8px; padding: 10px 0;">
                <div style="font-size: 20px; font-weight: 700; color: #1d4ed8;">{{ b.numberOfFloors }}</div>
                <div style="font-size: 11px; color: #9ca3af;">Tầng</div>
              </div>
            </div>
          </div>
        </v-card>
      </v-col>

      <!-- Empty state -->
      <v-col v-if="buildings.length === 0" cols="12">
        <div style="text-align: center; padding: 60px 0; color: #9ca3af;">
          <v-icon size="64" color="grey-lighten-1">mdi-office-building-outline</v-icon>
          <p style="margin-top: 12px; font-size: 15px;">Chưa có tòa nhà nào</p>
          <v-btn color="primary" prepend-icon="mdi-plus" variant="flat" class="mt-2" @click="openCreate">Thêm tòa nhà</v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Add/Edit Dialog -->
    <v-dialog v-model="dialog" max-width="560" persistent>
      <v-card class="rounded-xl pa-6">
        <v-card-title class="text-h6 font-weight-black px-0 mb-4">
          {{ editTarget ? 'Chỉnh sửa tòa nhà' : 'Thêm tòa nhà mới' }}
        </v-card-title>
        <v-card-text class="px-0 py-0">
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="form.name"
                label="Tên tòa nhà *"
                placeholder="Ví dụ: Tòa A - Nam"
                variant="outlined"
                color="primary"
                rounded="lg"
                :error-messages="formErrors.name"
              />
            </v-col>
            <v-col cols="12">
              <v-textarea
                v-model="form.description"
                label="Mô tả"
                placeholder="Mô tả về tòa nhà..."
                variant="outlined"
                color="primary"
                rounded="lg"
                rows="2"
                auto-grow
              />
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="form.numberOfFloors"
                label="Số tầng *"
                type="number"
                variant="outlined"
                color="primary"
                rounded="lg"
                min="1"
                :error-messages="formErrors.numberOfFloors"
              />
            </v-col>
            <v-col cols="6">
              <v-select
                v-model="form.type"
                label="Loại tòa nhà *"
                :items="buildingTypes"
                item-title="label"
                item-value="value"
                variant="outlined"
                color="primary"
                rounded="lg"
                :error-messages="formErrors.type"
              />
            </v-col>
            <v-col cols="12">
              <v-text-field
                v-model="form.imageUrl"
                label="Đường dẫn hình ảnh (URL)"
                placeholder="https://..."
                variant="outlined"
                color="primary"
                rounded="lg"
              />
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions class="px-0 pt-4">
          <v-spacer />
          <v-btn variant="text" class="rounded-lg font-weight-bold" :disabled="saving" @click="closeDialog">Hủy</v-btn>
          <v-btn
            color="primary"
            variant="flat"
            class="rounded-lg font-weight-bold px-6"
            :loading="saving"
            @click="saveBuilding"
          >
            {{ editTarget ? 'Lưu thay đổi' : 'Thêm mới' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirm Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400">
      <v-card class="rounded-xl pa-6">
        <v-card-title class="text-h6 font-weight-bold px-0">Xác nhận xóa</v-card-title>
        <v-card-text class="px-0 py-2">
          Bạn có chắc muốn xóa tòa nhà <strong>{{ deleteTarget?.name }}</strong>? Hành động này không thể hoàn tác.
        </v-card-text>
        <v-card-actions class="px-0 pt-4">
          <v-spacer />
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" variant="flat" class="rounded-lg" :loading="saving" @click="deleteBuilding">Xóa</v-btn>
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
import { buildingService } from '@/services/buildingService'

// ─── State ───────────────────────────────────────────────────────────────────
const buildings = ref([])
const loading   = ref(false)
const error     = ref(null)
const saving    = ref(false)

const dialog      = ref(false)
const deleteDialog = ref(false)
const editTarget  = ref(null)
const deleteTarget = ref(null)

const form = ref({ name: '', description: '', numberOfFloors: 5, type: 'Male', imageUrl: '' })
const formErrors = ref({})

const snackbar = ref({ show: false, message: '', color: 'success' })

const buildingTypes = [
  { label: 'Nam',      value: 'Male'   },
  { label: 'Nữ',      value: 'Female' },
  { label: 'Hỗn hợp', value: 'Mixed'  },
]

// ─── Load data ───────────────────────────────────────────────────────────────
async function loadBuildings() {
  loading.value = true
  error.value   = null
  try {
    buildings.value = await buildingService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách tòa nhà'
  } finally {
    loading.value = false
  }
}

onMounted(loadBuildings)

// ─── CRUD helpers ─────────────────────────────────────────────────────────────
function openCreate() {
  editTarget.value = null
  form.value = { name: '', description: '', numberOfFloors: 5, type: 'Male', imageUrl: '' }
  formErrors.value = {}
  dialog.value = true
}

function openEdit(b) {
  editTarget.value = b
  form.value = { name: b.name, description: b.description, numberOfFloors: b.numberOfFloors, type: b.type, imageUrl: b.imageUrl || '' }
  formErrors.value = {}
  dialog.value = true
}

function closeDialog() {
  dialog.value = false
  editTarget.value = null
}

function validate() {
  const errors = {}
  if (!form.value.name.trim())           errors.name = 'Vui lòng nhập tên tòa nhà'
  if (!form.value.numberOfFloors || form.value.numberOfFloors < 1) errors.numberOfFloors = 'Số tầng phải ≥ 1'
  if (!form.value.type)                  errors.type = 'Vui lòng chọn loại'
  formErrors.value = errors
  return Object.keys(errors).length === 0
}

async function saveBuilding() {
  if (!validate()) return
  saving.value = true
  try {
    if (editTarget.value) {
      await buildingService.update(editTarget.value.id, form.value)
      showSnackbar('Cập nhật tòa nhà thành công!', 'success')
    } else {
      await buildingService.create(form.value)
      showSnackbar('Thêm tòa nhà thành công!', 'success')
    }
    closeDialog()
    await loadBuildings()
  } catch (err) {
    showSnackbar(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

function confirmDelete(b) {
  deleteTarget.value = b
  deleteDialog.value = true
}

async function deleteBuilding() {
  saving.value = true
  try {
    await buildingService.delete(deleteTarget.value.id)
    showSnackbar('Đã xóa tòa nhà!', 'success')
    deleteDialog.value = false
    await loadBuildings()
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
function getTypeLabel(type) {
  const map = { Male: 'Khu Nam', Female: 'Khu Nữ', Mixed: 'Hỗn hợp' }
  return map[type] || type
}
function getTypeColor(type) {
  const map = { Male: 'blue', Female: 'pink', Mixed: 'purple' }
  return map[type] || 'grey'
}
function getBuildingColor(type) {
  const map = { Male: '#3b82f6', Female: '#ec4899', Mixed: '#8b5cf6' }
  return map[type] || '#e5e7eb'
}
function getTypeBg(type) {
  const map = { Male: '#eff6ff', Female: '#fdf2f8', Mixed: '#f5f3ff' }
  return map[type] || '#f9fafb'
}
function getTypeIconColor(type) {
  const map = { Male: '#3b82f6', Female: '#ec4899', Mixed: '#8b5cf6' }
  return map[type] || '#9ca3af'
}
</script>

<style scoped>
</style>
