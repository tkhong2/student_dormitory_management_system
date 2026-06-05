<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 16px;">
      <div>
        <h1 style="font-size: 20px; font-weight: 700; margin: 0">
          Quản lý Tài liệu Sinh viên
        </h1>
        <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0">
          Xác minh và quản lý hồ sơ giấy tờ của sinh viên
        </p>
      </div>
      <a-button type="primary" @click="openUpload" style="background: #ff9800; border-color: #ff9800;">
        <template #icon><UploadOutlined /></template>
        Tải tài liệu lên
      </a-button>
    </div>

    <DataStatus
      :loading="loading"
      :error="error"
      :items="documents"
      :treatEmptyAsError="false"
      @retry="loadDocuments"
    >
      <!-- Filters Card -->
      <a-card style="margin-bottom: 16px" :bordered="false">
        <a-row :gutter="[16, 16]">
          <a-col :xs="24" :sm="12" :md="8">
            <a-input-search
              v-model:value="search"
              placeholder="Tìm theo tên sinh viên, loại tài liệu..."
              allow-clear
            />
          </a-col>
          <a-col :xs="24" :sm="12" :md="8">
            <a-select
              v-model:value="typeFilter"
              placeholder="Loại tài liệu"
              allow-clear
              style="width: 100%"
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
          </a-col>
          <a-col :xs="24" :sm="12" :md="8">
            <a-select
              v-model:value="verifiedFilter"
              placeholder="Trạng thái xác minh"
              allow-clear
              style="width: 100%"
            >
              <a-select-option value="all">Tất cả</a-select-option>
              <a-select-option value="verified">Đã xác minh</a-select-option>
              <a-select-option value="unverified">Chưa xác minh</a-select-option>
            </a-select>
          </a-col>
        </a-row>
      </a-card>

      <!-- Table Card -->
      <a-card :bordered="false">
        <a-table
          :columns="columns"
          :data-source="filteredDocuments"
          row-key="id"
          :pagination="{ pageSize: 10 }"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'documentName'">
              <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
                <a-avatar
                  :style="{
                    background: getFileIcon(record.fileType).color,
                    color: '#fff',
                  }"
                  size="40"
                  shape="square"
                >
                  <template #icon>
                    <component :is="getFileIcon(record.fileType).icon" />
                  </template>
                </a-avatar>
                <div>
                  <div style="font-weight: 600">{{ record.documentName }}</div>
                  <div style="font-size: 12px; color: #8c8c8c">
                    {{ record.documentType }} • {{ formatFileSize(record.fileSizeBytes) }}
                  </div>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'student'">
              <div>
                <div style="font-weight: 500">{{ record.studentName }}</div>
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
                <a-button type="link" danger @click="confirmDelete(record)">Xóa</a-button>
              </a-space>
            </template>
          </template>
        </a-table>
      </a-card>
    </DataStatus>

    <!-- Upload Modal -->
    <a-modal
      v-model:open="uploadDialog"
      title="Tải tài liệu lên"
      width="560px"
      @ok="handleUpload"
      @cancel="uploadDialog = false"
      :confirmLoading="saving"
      okText="Tải lên"
      cancelText="Hủy"
    >
      <a-form layout="vertical" style="margin-top: 24px">
        <a-form-item label="Sinh viên" required :validate-status="uploadErrors.studentId ? 'error' : ''" :help="uploadErrors.studentId">
          <a-select v-model:value="uploadForm.studentId" placeholder="Chọn sinh viên">
            <a-select-option v-for="student in students" :key="student.id" :value="student.id">
              {{ student.fullName }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Loại tài liệu" required :validate-status="uploadErrors.documentType ? 'error' : ''" :help="uploadErrors.documentType">
          <a-select v-model:value="uploadForm.documentType" placeholder="Chọn loại tài liệu">
            <a-select-option v-for="type in documentTypes" :key="type" :value="type">
              {{ type }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="Tên tài liệu" required :validate-status="uploadErrors.documentName ? 'error' : ''" :help="uploadErrors.documentName">
          <a-input v-model:value="uploadForm.documentName" />
        </a-form-item>
        <a-form-item label="Chọn file" required :validate-status="uploadErrors.file ? 'error' : ''" :help="uploadErrors.file">
          <a-upload
            v-model:file-list="uploadForm.file"
            :before-upload="() => false"
            :max-count="1"
            accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
          >
            <a-button>
              <template #icon><UploadOutlined /></template>
              Chọn file
            </a-button>
          </a-upload>
        </a-form-item>
        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="uploadForm.notes" :rows="2" />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- Delete Modal -->
    <a-modal
      v-model:open="deleteDialog"
      title="Xác nhận xóa"
      @ok="deleteDocument"
      @cancel="deleteDialog = false"
      :confirmLoading="saving"
      okText="Xóa"
      cancelText="Hủy"
      okType="danger"
    >
      <p>
        Bạn có chắc muốn xóa tài liệu
        <strong>{{ deleteTarget?.documentName }}</strong>?
      </p>
    </a-modal>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { 
  UploadOutlined, 
  FilePdfOutlined, 
  FileImageOutlined, 
  FileWordOutlined, 
  FileOutlined 
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
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
  file: [],
  notes: '',
})
const uploadErrors = ref({})

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
    file: [],
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
  if (!uploadForm.value.file || uploadForm.value.file.length === 0) errors.file = 'Vui lòng chọn file'
  uploadErrors.value = errors
  if (Object.keys(errors).length > 0) return

  saving.value = true
  try {
    const fileObj = uploadForm.value.file[0]
    // TODO: Upload file to server first, then create document record
    // For now, create with mock URL
    await studentDocumentService.create({
      studentId: uploadForm.value.studentId,
      documentType: uploadForm.value.documentType,
      documentName: uploadForm.value.documentName.trim(),
      fileUrl: '/uploads/documents/' + fileObj.name,
      fileType: fileObj.type,
      fileSizeBytes: fileObj.size,
      notes: uploadForm.value.notes?.trim() || null,
    })
    message.success('Tải tài liệu lên thành công')
    uploadDialog.value = false
    await loadDocuments()
  } catch (err) {
    message.error(err.message || 'Có lỗi xảy ra')
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
    message.success('Đã xác minh tài liệu')
    await loadDocuments()
  } catch (err) {
    message.error(err.message || 'Không thể xác minh tài liệu')
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
    message.success('Đã xóa tài liệu')
    await loadDocuments()
  } catch (err) {
    message.error(err.message || 'Không thể xóa tài liệu')
  } finally {
    saving.value = false
  }
}

function getFileIcon(fileType) {
  if (fileType?.includes('pdf')) return { icon: FilePdfOutlined, color: '#f44336' }
  if (fileType?.includes('image')) return { icon: FileImageOutlined, color: '#4caf50' }
  if (fileType?.includes('word') || fileType?.includes('document'))
    return { icon: FileWordOutlined, color: '#2196f3' }
  return { icon: FileOutlined, color: '#9e9e9e' }
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

onMounted(() => {
  loadDocuments()
  loadStudents()
})
</script>
