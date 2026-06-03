<template>
  <div>
    <div class="d-flex justify-space-between align-center flex-wrap ga-3 mb-4">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Tài liệu Sinh viên
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Xác minh và quản lý hồ sơ giấy tờ của sinh viên
        </p>
      </div>
      <v-btn
        color="primary"
        prepend-icon="mdi-upload"
        variant="flat"
        @click="openUpload"
      >
        Tải tài liệu lên
      </v-btn>
    </div>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="documents"
      :treatEmptyAsError="false"
      @retry="loadDocuments"
    >
      <a-card
        style="border: 1px solid #e5e7eb; background: #fafafa"
        :body-style="{ padding: '0' }"
      >
        <div class="pa-4 d-flex flex-wrap align-center" style="gap: 12px">
          <a-input-search
            v-model:value="search"
            placeholder="Tìm theo tên sinh viên, loại tài liệu..."
            allowClear
            style="max-width: 350px; flex: 1"
          />
          <a-select
            v-model:value="typeFilter"
            placeholder="Loại tài liệu"
            allowClear
            style="max-width: 200px"
          >
            <a-select-option value="all">Tất cả loại</a-select-option>
            <a-select-option
              v-for="type in documentTypes"
              :key="type"
              :value="type"
            >
              {{ type }}
            </a-select-option>
          </a-select>
          <a-select
            v-model:value="verifiedFilter"
            placeholder="Trạng thái xác minh"
            allowClear
            style="max-width: 200px"
          >
            <a-select-option value="all">Tất cả</a-select-option>
            <a-select-option value="verified">Đã xác minh</a-select-option>
            <a-select-option value="unverified">Chưa xác minh</a-select-option>
          </a-select>
          <a-button
            v-if="search || typeFilter !== 'all' || verifiedFilter !== 'all'"
            type="text"
            size="small"
            @click="resetFilters"
          >
            Đặt lại
          </a-button>
        </div>

        <a-table
          :columns="columns"
          :data-source="filteredDocuments"
          row-key="id"
          :pagination="{ pageSize: 10 }"
          style="width: 100%"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'documentName'">
              <div class="d-flex align-center" style="gap: 12px; padding: 12px 0">
                <a-avatar
                  :style="{
                    background: getFileIcon(record.fileType).color,
                    color: '#fff',
                  }"
                  size="40"
                  shape="square"
                >
                  <v-icon size="20">{{ getFileIcon(record.fileType).icon }}</v-icon>
                </a-avatar>
                <div>
                  <div class="font-weight-bold">{{ record.documentName }}</div>
                  <div style="font-size: 12px; color: #8c8c8c">
                    {{ record.documentType }} • {{ formatFileSize(record.fileSizeBytes) }}
                  </div>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'student'">
              <div>
                <div class="font-weight-medium">{{ record.studentName }}</div>
                <div style="font-size: 12px; color: #8c8c8c">{{ record.studentCode }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'uploadDate'">
              {{ formatDate(record.createdAt) }}
            </template>
            <template v-else-if="column.key === 'verified'">
              <a-tag :color="record.isVerified ? 'success' : 'warning'">
                {{ record.isVerified ? 'Đã xác minh' : 'Chưa xác minh' }}
              </a-tag>
            </template>
            <template v-else-if="column.key === 'actions'">
              <a-space size="small">
                <a-button type="link" @click="viewDocument(record)">Xem</a-button>
                <a-button
                  v-if="!record.isVerified"
                  type="link"
                  @click="verifyDocument(record)"
                >
                  Xác minh
                </a-button>
                <a-button type="text" danger @click="confirmDelete(record)">Xóa</a-button>
              </a-space>
            </template>
          </template>
        </a-table>
      </a-card>
    </DataStatus>

    <!-- Upload Dialog -->
    <v-dialog v-model="uploadDialog" max-width="560" persistent>
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-4">Tải tài liệu lên</h2>
        <v-row>
          <v-col cols="12">
            <v-select
              v-model="uploadForm.studentId"
              label="Sinh viên *"
              :items="students"
              item-title="fullName"
              item-value="id"
              :error-messages="uploadErrors.studentId"
            />
          </v-col>
          <v-col cols="12">
            <v-select
              v-model="uploadForm.documentType"
              label="Loại tài liệu *"
              :items="documentTypes"
              :error-messages="uploadErrors.documentType"
            />
          </v-col>
          <v-col cols="12">
            <v-text-field
              v-model="uploadForm.documentName"
              label="Tên tài liệu *"
              :error-messages="uploadErrors.documentName"
            />
          </v-col>
          <v-col cols="12">
            <v-file-input
              v-model="uploadForm.file"
              label="Chọn file *"
              accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
              :error-messages="uploadErrors.file"
            />
          </v-col>
          <v-col cols="12">
            <v-textarea
              v-model="uploadForm.notes"
              label="Ghi chú"
              rows="2"
            />
          </v-col>
        </v-row>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" :disabled="saving" @click="uploadDialog = false">Hủy</v-btn>
          <v-btn color="primary" :loading="saving" @click="handleUpload">Tải lên</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <!-- Delete Dialog -->
    <v-dialog v-model="deleteDialog" max-width="420">
      <v-card class="pa-6">
        <h2 class="text-h6 font-weight-bold mb-3">Xác nhận xóa</h2>
        <p>
          Bạn có chắc muốn xóa tài liệu
          <strong>{{ deleteTarget?.documentName }}</strong>?
        </p>
        <div class="d-flex justify-end ga-3 mt-4">
          <v-btn variant="text" @click="deleteDialog = false">Hủy</v-btn>
          <v-btn color="error" :loading="saving" @click="deleteDocument">Xóa</v-btn>
        </div>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackbar.show" :color="snackbar.color" location="bottom right">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import DataStatus from '@/components/common/DataStatus.vue'
import { studentDocumentService } from '@/services/studentDocumentService'
import { studentService } from '@/services/studentService'

const columns = [
  { title: 'Tài liệu', key: 'documentName', width: 300 },
  { title: 'Sinh viên', key: 'student', width: 200 },
  { title: 'Ngày tải lên', key: 'uploadDate', align: 'center', width: 150 },
  { title: 'Trạng thái', key: 'verified', align: 'center', width: 140 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 200 },
]

const documentTypes = [
  'CMND/CCCD',
  'Giấy khai sinh',
  'Bảng điểm',
  'Giấy xác nhận sinh viên',
  'Hộ khẩu',
  'Giấy khám sức khỏe',
  'Ảnh 3x4',
  'Khác',
]

const documents = ref([])
const students = ref([])
const search = ref('')
const typeFilter = ref('all')
const verifiedFilter = ref('all')
const loading = ref(false)
const saving = ref(false)
const error = ref(null)
const uploadDialog = ref(false)
const deleteDialog = ref(false)
const deleteTarget = ref(null)
const uploadForm = ref({
  studentId: null,
  documentType: '',
  documentName: '',
  file: null,
  notes: '',
})
const uploadErrors = ref({})
const snackbar = ref({ show: false, message: '', color: 'success' })

const filteredDocuments = computed(() => {
  const keyword = search.value.trim().toLowerCase()
  return documents.value.filter((item) => {
    const matchesText =
      !keyword ||
      item.documentName?.toLowerCase().includes(keyword) ||
      item.studentName?.toLowerCase().includes(keyword) ||
      item.documentType?.toLowerCase().includes(keyword)
    const matchesType = typeFilter.value === 'all' || item.documentType === typeFilter.value
    const matchesVerified =
      verifiedFilter.value === 'all' ||
      (verifiedFilter.value === 'verified' && item.isVerified) ||
      (verifiedFilter.value === 'unverified' && !item.isVerified)
    return matchesText && matchesType && matchesVerified
  })
})

function resetFilters() {
  search.value = ''
  typeFilter.value = 'all'
  verifiedFilter.value = 'all'
}

async function loadDocuments() {
  loading.value = true
  error.value = null
  try {
    documents.value = await studentDocumentService.getAll()
  } catch (err) {
    error.value = err.message || 'Không thể tải danh sách tài liệu'
  } finally {
    loading.value = false
  }
}

async function loadStudents() {
  try {
    students.value = await studentService.getAll()
  } catch (err) {
    console.error('Không thể tải danh sách sinh viên', err)
  }
}

function openUpload() {
  uploadForm.value = {
    studentId: null,
    documentType: '',
    documentName: '',
    file: null,
    notes: '',
  }
  uploadErrors.value = {}
  uploadDialog.value = true
}

async function handleUpload() {
  const errors = {}
  if (!uploadForm.value.studentId) errors.studentId = 'Vui lòng chọn sinh viên'
  if (!uploadForm.value.documentType) errors.documentType = 'Vui lòng chọn loại tài liệu'
  if (!uploadForm.value.documentName.trim())
    errors.documentName = 'Vui lòng nhập tên tài liệu'
  if (!uploadForm.value.file) errors.file = 'Vui lòng chọn file'
  uploadErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    // TODO: Upload file to server first, then create document record
    // For now, create with mock URL
    await studentDocumentService.create({
      studentId: uploadForm.value.studentId,
      documentType: uploadForm.value.documentType,
      documentName: uploadForm.value.documentName.trim(),
      fileUrl: '/uploads/documents/' + uploadForm.value.file[0].name,
      fileType: uploadForm.value.file[0].type,
      fileSizeBytes: uploadForm.value.file[0].size,
      notes: uploadForm.value.notes?.trim() || null,
    })
    notify('Tải tài liệu lên thành công')
    uploadDialog.value = false
    await loadDocuments()
  } catch (err) {
    notify(err.message || 'Có lỗi xảy ra', 'error')
  } finally {
    saving.value = false
  }
}

async function verifyDocument(item) {
  saving.value = true
  try {
    await studentDocumentService.verify(item.id, {
      verifiedByUserId: 1, // TODO: Get from auth
    })
    notify('Đã xác minh tài liệu')
    await loadDocuments()
  } catch (err) {
    notify(err.message || 'Không thể xác minh tài liệu', 'error')
  } finally {
    saving.value = false
  }
}

function viewDocument(item) {
  window.open(item.fileUrl, '_blank')
}

function confirmDelete(item) {
  deleteTarget.value = item
  deleteDialog.value = true
}

async function deleteDocument() {
  saving.value = true
  try {
    await studentDocumentService.delete(deleteTarget.value.id)
    deleteDialog.value = false
    notify('Đã xóa tài liệu')
    await loadDocuments()
  } catch (err) {
    notify(err.message || 'Không thể xóa tài liệu', 'error')
  } finally {
    saving.value = false
  }
}

function getFileIcon(fileType) {
  if (fileType?.includes('pdf')) return { icon: 'mdi-file-pdf-box', color: '#f44336' }
  if (fileType?.includes('image')) return { icon: 'mdi-file-image', color: '#4caf50' }
  if (fileType?.includes('word') || fileType?.includes('document'))
    return { icon: 'mdi-file-word', color: '#2196f3' }
  return { icon: 'mdi-file-document', color: '#9e9e9e' }
}

function formatFileSize(bytes) {
  if (!bytes) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

function formatDate(value) {
  return value ? new Date(value).toLocaleDateString('vi-VN') : ''
}

function notify(message, color = 'success') {
  snackbar.value = { show: false, message, color }
  snackbar.value.show = true
}

onMounted(() => {
  loadDocuments()
  loadStudents()
})
</script>
