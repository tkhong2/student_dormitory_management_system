<template>
  <div>
    <PageHeaderAnt title="Check-in / Check-out" subtitle="Quản lý nhận phòng và trả phòng">
      <template #actions>
        <a-radio-group v-model:value="viewMode" button-style="solid">
          <a-radio-button value="checkin">Check-in</a-radio-button>
          <a-radio-button value="checkout">Check-out</a-radio-button>
        </a-radio-group>
      </template>
    </PageHeaderAnt>

    <!-- Check-in View -->
    <a-card v-if="viewMode === 'checkin'" title="Danh sách sinh viên sắp nhận phòng" :bordered="false">
      <a-table
        :columns="checkinColumns"
        :data-source="checkinList"
        :loading="loading"
        :row-key="record => record.id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <strong>{{ record.studentName }}</strong>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'room'">
            <a-tag color="blue">{{ record.roomNumber }}</a-tag>
            <div style="font-size: 12px;">{{ record.buildingName }}</div>
          </template>

          <template v-else-if="column.key === 'contractDate'">
            {{ formatDate(record.startDate) }}
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="record.checkedIn ? 'green' : 'orange'">
              {{ record.checkedIn ? 'Đã nhận' : 'Chưa nhận' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-button 
              v-if="!record.checkedIn" 
              type="primary" 
              size="small"
              @click="openCheckinModal(record)"
            >
              Nhận phòng
            </a-button>
            <a-tag v-else color="success">
              <CheckCircleOutlined /> Đã xử lý
            </a-tag>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Check-out View -->
    <a-card v-if="viewMode === 'checkout'" title="Danh sách sinh viên sắp trả phòng" :bordered="false">
      <a-table
        :columns="checkoutColumns"
        :data-source="checkoutList"
        :loading="loading"
        :row-key="record => record.id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div>
              <strong>{{ record.studentName }}</strong>
              <div style="font-size: 12px; color: #8c8c8c;">{{ record.studentCode }}</div>
            </div>
          </template>

          <template v-else-if="column.key === 'room'">
            <a-tag color="blue">{{ record.roomNumber }}</a-tag>
            <div style="font-size: 12px;">{{ record.buildingName }}</div>
          </template>

          <template v-else-if="column.key === 'endDate'">
            {{ formatDate(record.endDate) }}
            <div v-if="isOverdue(record.endDate)" style="font-size: 11px; color: #ff4d4f;">
              <WarningOutlined /> Quá hạn
            </div>
          </template>

          <template v-else-if="column.key === 'status'">
            <a-tag :color="record.checkedOut ? 'default' : 'processing'">
              {{ record.checkedOut ? 'Đã trả' : 'Chưa trả' }}
            </a-tag>
          </template>

          <template v-else-if="column.key === 'actions'">
            <a-button 
              v-if="!record.checkedOut" 
              type="primary" 
              size="small"
              @click="openCheckoutModal(record)"
            >
              Trả phòng
            </a-button>
            <a-tag v-else color="success">
              <CheckCircleOutlined /> Đã xử lý
            </a-tag>
          </template>
        </template>
      </a-table>
    </a-card>

    <!-- Check-in Modal -->
    <a-modal
      v-model:open="checkinModalVisible"
      title="Quy trình nhận phòng"
      width="700px"
      :footer="null"
    >
      <a-steps :current="checkinStep" style="margin-bottom: 24px;">
        <a-step title="Xác minh" />
        <a-step title="Kiểm tra phòng" />
        <a-step title="Hoàn tất" />
      </a-steps>

      <!-- Step 1: Xác minh -->
      <div v-if="checkinStep === 0">
        <a-descriptions v-if="selectedRecord" bordered :column="2" size="small">
          <a-descriptions-item label="Sinh viên" :span="2">
            <strong>{{ selectedRecord.studentName }}</strong>
          </a-descriptions-item>
          <a-descriptions-item label="Phòng">
            {{ selectedRecord.roomNumber }}
          </a-descriptions-item>
          <a-descriptions-item label="Tòa nhà">
            {{ selectedRecord.buildingName }}
          </a-descriptions-item>
        </a-descriptions>

        <a-divider />

        <a-form layout="vertical">
          <a-form-item label="Upload CMND/CCCD">
            <a-upload
              :file-list="checkinForm.idCardImages"
              list-type="picture-card"
              @change="handleIdCardUpload"
            >
              <div>
                <PlusOutlined />
                <div style="margin-top: 8px;">Upload</div>
              </div>
            </a-upload>
          </a-form-item>

          <a-form-item label="Xác nhận tiền cọc">
            <a-checkbox v-model:checked="checkinForm.depositPaid">
              Đã nhận tiền cọc {{ formatCurrency(selectedRecord?.depositAmount || 0) }} VNĐ
            </a-checkbox>
          </a-form-item>
        </a-form>

        <div style="text-align: right;">
          <a-button @click="checkinModalVisible = false" style="margin-right: 8px;">Hủy</a-button>
          <a-button type="primary" @click="checkinStep = 1">Tiếp theo</a-button>
        </div>
      </div>

      <!-- Step 2: Kiểm tra phòng -->
      <div v-if="checkinStep === 1">
        <a-alert
          message="Kiểm tra tình trạng phòng ban đầu"
          description="Chụp ảnh và đánh giá tình trạng phòng trước khi bàn giao"
          type="info"
          show-icon
          style="margin-bottom: 16px;"
        />

        <a-form layout="vertical">
          <a-form-item label="Checklist kiểm tra">
            <a-checkbox-group v-model:value="checkinForm.checklist" style="width: 100%;">
              <a-row>
                <a-col :span="12"><a-checkbox value="walls">Tường, trần, sàn</a-checkbox></a-col>
                <a-col :span="12"><a-checkbox value="doors">Cửa, cửa sổ, khóa</a-checkbox></a-col>
                <a-col :span="12"><a-checkbox value="electric">Điện, đèn, quạt</a-checkbox></a-col>
                <a-col :span="12"><a-checkbox value="water">Vòi nước, bồn cầu</a-checkbox></a-col>
                <a-col :span="12"><a-checkbox value="furniture">Giường, tủ, bàn</a-checkbox></a-col>
                <a-col :span="12"><a-checkbox value="clean">Vệ sinh chung</a-checkbox></a-col>
              </a-row>
            </a-checkbox-group>
          </a-form-item>

          <a-form-item label="Chụp ảnh phòng (4-6 ảnh)">
            <a-upload
              :file-list="checkinForm.roomImages"
              list-type="picture-card"
              @change="handleRoomImagesUpload"
            >
              <div>
                <PlusOutlined />
                <div style="margin-top: 8px;">Upload</div>
              </div>
            </a-upload>
          </a-form-item>

          <a-form-item label="Ghi chú">
            <a-textarea v-model:value="checkinForm.notes" :rows="3" placeholder="Ghi chú về tình trạng phòng..." />
          </a-form-item>
        </a-form>

        <div style="text-align: right;">
          <a-button @click="checkinStep = 0" style="margin-right: 8px;">Quay lại</a-button>
          <a-button type="primary" @click="checkinStep = 2">Tiếp theo</a-button>
        </div>
      </div>

      <!-- Step 3: Hoàn tất -->
      <div v-if="checkinStep === 2">
        <a-result
          status="success"
          title="Sẵn sàng hoàn tất check-in"
          sub-title="Xác nhận thông tin và hoàn tất quy trình nhận phòng"
        >
          <template #extra>
            <a-space direction="vertical" style="width: 100%;">
              <a-button type="primary" size="large" block @click="handleCheckin" :loading="processing">
                <CheckCircleOutlined /> Hoàn tất check-in
              </a-button>
              <a-button size="large" block @click="checkinStep = 1">Quay lại</a-button>
            </a-space>
          </template>
        </a-result>
      </div>
    </a-modal>

    <!-- Check-out Modal -->
    <a-modal
      v-model:open="checkoutModalVisible"
      title="Quy trình trả phòng"
      width="700px"
      :footer="null"
    >
      <a-steps :current="checkoutStep" style="margin-bottom: 24px;">
        <a-step title="Kiểm tra" />
        <a-step title="Tính toán" />
        <a-step title="Hoàn tất" />
      </a-steps>

      <!-- Step 1: Kiểm tra -->
      <div v-if="checkoutStep === 0">
        <a-form layout="vertical">
          <a-form-item label="Đánh giá tình trạng phòng">
            <a-radio-group v-model:value="checkoutForm.roomCondition" style="width: 100%;">
              <a-space direction="vertical">
                <a-radio value="good">✅ Tốt - Không hư hỏng</a-radio>
                <a-radio value="minor">⚠️ Hư hỏng nhẹ</a-radio>
                <a-radio value="major">❌ Hư hỏng nặng</a-radio>
              </a-space>
            </a-radio-group>
          </a-form-item>

          <a-form-item v-if="checkoutForm.roomCondition !== 'good'" label="Mô tả hư hỏng">
            <a-textarea v-model:value="checkoutForm.damageDescription" :rows="3" />
          </a-form-item>

          <a-form-item label="Chụp ảnh hiện tại">
            <a-upload
              :file-list="checkoutForm.currentImages"
              list-type="picture-card"
              @change="handleCheckoutImagesUpload"
            >
              <div>
                <PlusOutlined />
                <div style="margin-top: 8px;">Upload</div>
              </div>
            </a-upload>
          </a-form-item>
        </a-form>

        <div style="text-align: right;">
          <a-button @click="checkoutModalVisible = false" style="margin-right: 8px;">Hủy</a-button>
          <a-button type="primary" @click="checkoutStep = 1">Tiếp theo</a-button>
        </div>
      </div>

      <!-- Step 2: Tính toán -->
      <div v-if="checkoutStep === 1">
        <a-descriptions bordered :column="1" size="small">
          <a-descriptions-item label="Tiền cọc ban đầu">
            {{ formatCurrency(selectedRecord?.depositAmount || 0) }} VNĐ
          </a-descriptions-item>
          <a-descriptions-item label="Chi phí bồi thường">
            <a-input-number 
              v-model:value="checkoutForm.compensationCost" 
              :min="0" 
              style="width: 100%;"
              :formatter="value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')"
            >
              <template #addonAfter>VNĐ</template>
            </a-input-number>
          </a-descriptions-item>
          <a-descriptions-item label="Tiền hoàn trả">
            <strong style="color: #52c41a; font-size: 18px;">
              {{ formatCurrency(calculateRefund()) }} VNĐ
            </strong>
          </a-descriptions-item>
        </a-descriptions>

        <div style="text-align: right; margin-top: 16px;">
          <a-button @click="checkoutStep = 0" style="margin-right: 8px;">Quay lại</a-button>
          <a-button type="primary" @click="checkoutStep = 2">Tiếp theo</a-button>
        </div>
      </div>

      <!-- Step 3: Hoàn tất -->
      <div v-if="checkoutStep === 2">
        <a-result
          status="success"
          title="Sẵn sàng hoàn tất check-out"
        >
          <template #extra>
            <a-space direction="vertical" style="width: 100%;">
              <a-checkbox v-model:checked="checkoutForm.keyReturned">
                Đã thu hồi chìa khóa/thẻ từ
              </a-checkbox>
              <a-checkbox v-model:checked="checkoutForm.depositRefunded">
                Đã hoàn trả tiền cọc: {{ formatCurrency(calculateRefund()) }} VNĐ
              </a-checkbox>
              <a-divider />
              <a-button 
                type="primary" 
                size="large" 
                block 
                @click="handleCheckout" 
                :loading="processing"
                :disabled="!checkoutForm.keyReturned || !checkoutForm.depositRefunded"
              >
                <CheckCircleOutlined /> Hoàn tất check-out
              </a-button>
              <a-button size="large" block @click="checkoutStep = 1">Quay lại</a-button>
            </a-space>
          </template>
        </a-result>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  CheckCircleOutlined, WarningOutlined, PlusOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import dayjs from 'dayjs'

const loading = ref(false)
const processing = ref(false)
const viewMode = ref('checkin')
const checkinModalVisible = ref(false)
const checkoutModalVisible = ref(false)
const checkinStep = ref(0)
const checkoutStep = ref(0)
const selectedRecord = ref(null)

// Mock data
const checkinList = ref([])
const checkoutList = ref([])

const checkinForm = ref({
  idCardImages: [],
  depositPaid: false,
  checklist: [],
  roomImages: [],
  notes: ''
})

const checkoutForm = ref({
  roomCondition: 'good',
  damageDescription: '',
  currentImages: [],
  compensationCost: 0,
  keyReturned: false,
  depositRefunded: false
})

const checkinColumns = [
  { title: 'Sinh viên', key: 'student', width: 200 },
  { title: 'Phòng', key: 'room', width: 150 },
  { title: 'Ngày nhận phòng', key: 'contractDate', width: 150 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150 }
]

const checkoutColumns = [
  { title: 'Sinh viên', key: 'student', width: 200 },
  { title: 'Phòng', key: 'room', width: 150 },
  { title: 'Ngày trả phòng', key: 'endDate', width: 150 },
  { title: 'Trạng thái', key: 'status', width: 120 },
  { title: 'Thao tác', key: 'actions', width: 150 }
]

const openCheckinModal = (record) => {
  selectedRecord.value = record
  checkinStep.value = 0
  checkinForm.value = {
    idCardImages: [],
    depositPaid: false,
    checklist: [],
    roomImages: [],
    notes: ''
  }
  checkinModalVisible.value = true
}

const openCheckoutModal = (record) => {
  selectedRecord.value = record
  checkoutStep.value = 0
  checkoutForm.value = {
    roomCondition: 'good',
    damageDescription: '',
    currentImages: [],
    compensationCost: 0,
    keyReturned: false,
    depositRefunded: false
  }
  checkoutModalVisible.value = true
}

const handleIdCardUpload = ({ fileList }) => {
  checkinForm.value.idCardImages = fileList
}

const handleRoomImagesUpload = ({ fileList }) => {
  checkinForm.value.roomImages = fileList
}

const handleCheckoutImagesUpload = ({ fileList }) => {
  checkoutForm.value.currentImages = fileList
}

const handleCheckin = async () => {
  processing.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1000))
    message.success('Check-in thành công!')
    checkinModalVisible.value = false
    // Reload data
  } catch (error) {
    message.error('Lỗi check-in')
  } finally {
    processing.value = false
  }
}

const handleCheckout = async () => {
  processing.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1000))
    message.success('Check-out thành công!')
    checkoutModalVisible.value = false
    // Reload data
  } catch (error) {
    message.error('Lỗi check-out')
  } finally {
    processing.value = false
  }
}

const calculateRefund = () => {
  const deposit = selectedRecord.value?.depositAmount || 0
  const compensation = checkoutForm.value.compensationCost || 0
  return Math.max(0, deposit - compensation)
}

const formatDate = (date) => {
  return date ? dayjs(date).format('DD/MM/YYYY') : ''
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}

const isOverdue = (date) => {
  return dayjs(date).isBefore(dayjs(), 'day')
}

onMounted(() => {
  // Load data
  message.info('Tính năng Check-in/Check-out đang được phát triển')
})
</script>
