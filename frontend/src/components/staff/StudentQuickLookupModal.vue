<template>
  <a-modal
    :open="visible"
    @cancel="handleClose"
    title="Tra cứu sinh viên"
    width="900px"
    :footer="null"
  >
    <!-- Search Bar -->
    <a-input-search
      v-model:value="searchQuery"
      size="large"
      placeholder="Nhập mã SV, tên, số phòng, SĐT..."
      @search="handleSearch"
      :loading="searching"
      style="margin-bottom: 16px;"
    >
      <template #enterButton>
        <a-button type="primary">
          <SearchOutlined /> Tìm kiếm
        </a-button>
      </template>
    </a-input-search>

    <!-- Results -->
    <div v-if="searchResults.length > 0">
      <a-list
        :data-source="searchResults"
        :pagination="{ pageSize: 5 }"
      >
        <template #renderItem="{ item }">
          <a-list-item>
            <a-list-item-meta>
              <template #title>
                <strong>{{ item.fullName }}</strong>
                <a-tag style="margin-left: 8px;" color="blue">{{ item.studentCode }}</a-tag>
              </template>
              <template #description>
                <a-space direction="vertical" size="small">
                  <div>
                    <PhoneOutlined /> {{ item.phone || 'N/A' }}
                    <a-divider type="vertical" />
                    <MailOutlined /> {{ item.email }}
                  </div>
                  <div v-if="item.roomNumber">
                    <HomeOutlined /> Phòng: <strong>{{ item.roomNumber }}</strong> - {{ item.buildingName }}
                  </div>
                  <div v-else style="color: #faad14;">
                    <WarningOutlined /> Chưa có phòng
                  </div>
                </a-space>
              </template>
            </a-list-item-meta>
            <template #actions>
              <a-button size="small" @click="viewStudentDetail(item)">
                <EyeOutlined /> Chi tiết
              </a-button>
              <a-button size="small" type="primary" @click="selectStudent(item)">
                <CheckOutlined /> Chọn
              </a-button>
            </template>
          </a-list-item>
        </template>
      </a-list>
    </div>

    <!-- Empty State -->
    <a-empty v-else-if="hasSearched && !searching" description="Không tìm thấy sinh viên">
      <template #image>
        <SearchOutlined style="font-size: 64px; color: #d9d9d9;" />
      </template>
    </a-empty>

    <a-empty v-else description="Nhập thông tin để tìm kiếm sinh viên">
      <template #image>
        <UserOutlined style="font-size: 64px; color: #d9d9d9;" />
      </template>
    </a-empty>

    <!-- Student Detail Drawer -->
    <a-drawer
      :open="detailDrawerVisible"
      @close="detailDrawerVisible = false"
      title="Thông tin chi tiết sinh viên"
      width="500"
    >
      <div v-if="selectedStudentDetail">
        <a-descriptions bordered :column="1" size="small">
          <a-descriptions-item label="Họ và tên">
            <strong>{{ selectedStudentDetail.fullName }}</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Mã sinh viên">
            {{ selectedStudentDetail.studentCode }}
          </a-descriptions-item>
          <a-descriptions-item label="Giới tính">
            {{ selectedStudentDetail.gender === 'Male' ? 'Nam' : 'Nữ' }}
          </a-descriptions-item>
          <a-descriptions-item label="Ngày sinh">
            {{ formatDate(selectedStudentDetail.dateOfBirth) }}
          </a-descriptions-item>
          <a-descriptions-item label="CMND/CCCD">
            {{ selectedStudentDetail.identificationCard }}
          </a-descriptions-item>
          <a-descriptions-item label="Số điện thoại">
            {{ selectedStudentDetail.phone }}
          </a-descriptions-item>
          <a-descriptions-item label="Email">
            {{ selectedStudentDetail.email }}
          </a-descriptions-item>
          <a-descriptions-item label="Địa chỉ">
            {{ selectedStudentDetail.address }}
          </a-descriptions-item>
          <a-descriptions-item label="Phòng hiện tại">
            <span v-if="selectedStudentDetail.roomNumber">
              <a-tag color="blue">{{ selectedStudentDetail.roomNumber }}</a-tag>
              {{ selectedStudentDetail.buildingName }}
            </span>
            <span v-else style="color: #8c8c8c;">Chưa có phòng</span>
          </a-descriptions-item>
        </a-descriptions>

        <!-- Quick Actions -->
        <a-divider>Thao tác nhanh</a-divider>
        <a-space direction="vertical" style="width: 100%;">
          <a-button block @click="viewContracts">
            <FileTextOutlined /> Xem hợp đồng
          </a-button>
          <a-button block @click="viewInvoices">
            <DollarOutlined /> Xem hóa đơn
          </a-button>
          <a-button block @click="viewPayments">
            <WalletOutlined /> Xem thanh toán
          </a-button>
          <a-button block @click="viewMaintenanceRequests">
            <ToolOutlined /> Xem yêu cầu bảo trì
          </a-button>
        </a-space>
      </div>
    </a-drawer>
  </a-modal>
</template>

<script setup>
import { ref, watch } from 'vue'
import { message } from 'ant-design-vue'
import {
  SearchOutlined, UserOutlined, EyeOutlined, CheckOutlined, PhoneOutlined,
  MailOutlined, HomeOutlined, WarningOutlined, FileTextOutlined,
  DollarOutlined, WalletOutlined, ToolOutlined
} from '@ant-design/icons-vue'
import { studentService } from '@/services/studentService'
import dayjs from 'dayjs'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['close', 'select'])

const searching = ref(false)
const searchQuery = ref('')
const searchResults = ref([])
const hasSearched = ref(false)
const detailDrawerVisible = ref(false)
const selectedStudentDetail = ref(null)

watch(() => props.visible, (newVal) => {
  if (!newVal) {
    // Reset when modal closes
    searchQuery.value = ''
    searchResults.value = []
    hasSearched.value = false
  }
})

const handleSearch = async () => {
  if (!searchQuery.value.trim()) {
    message.warning('Vui lòng nhập thông tin tìm kiếm')
    return
  }

  searching.value = true
  hasSearched.value = true
  try {
    // Search all students
    const allStudents = await studentService.getAll()
    const query = searchQuery.value.toLowerCase()

    // Filter by multiple fields
    searchResults.value = allStudents.filter(student =>
      student.studentCode?.toLowerCase().includes(query) ||
      student.fullName?.toLowerCase().includes(query) ||
      student.phone?.includes(query) ||
      student.email?.toLowerCase().includes(query) ||
      student.roomNumber?.toLowerCase().includes(query)
    )

    if (searchResults.value.length === 0) {
      message.info('Không tìm thấy sinh viên phù hợp')
    }
  } catch (error) {
    message.error('Lỗi tìm kiếm sinh viên')
    console.error(error)
  } finally {
    searching.value = false
  }
}

const viewStudentDetail = async (student) => {
  try {
    // Load full student details
    const fullDetail = await studentService.getById(student.id)
    selectedStudentDetail.value = fullDetail
    detailDrawerVisible.value = true
  } catch (error) {
    message.error('Lỗi tải thông tin sinh viên')
    console.error(error)
  }
}

const selectStudent = (student) => {
  emit('select', student)
  handleClose()
}

const handleClose = () => {
  emit('close')
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

// Quick action handlers
const viewContracts = () => {
  message.info('Chức năng xem hợp đồng đang được phát triển')
  // TODO: Navigate to contracts page with student filter
}

const viewInvoices = () => {
  message.info('Chức năng xem hóa đơn đang được phát triển')
  // TODO: Navigate to invoices page with student filter
}

const viewPayments = () => {
  message.info('Chức năng xem thanh toán đang được phát triển')
  // TODO: Navigate to payments page with student filter
}

const viewMaintenanceRequests = () => {
  message.info('Chức năng xem yêu cầu bảo trì đang được phát triển')
  // TODO: Navigate to maintenance requests page with student filter
}
</script>

<style scoped>
/* Add any custom styles if needed */
</style>
