<template>
  <div>
    <PageHeaderAnt title="Báo cáo" subtitle="Xem và xuất báo cáo công việc" />

    <a-row :gutter="[16, 16]">
      <!-- Report Cards -->
      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 32px; text-align: center;">
              <FileTextOutlined style="font-size: 48px; color: white;" />
            </div>
          </template>
          <a-card-meta
            title="Báo cáo công việc"
            description="Báo cáo số lượng đơn đã xử lý, thanh toán đã thu trong khoảng thời gian"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showWorkReportModal">
              Xem báo cáo
            </a-button>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); padding: 32px; text-align: center;">
              <DollarOutlined style="font-size: 48px; color: white;" />
            </div>
          </template>
          <a-card-meta
            title="Báo cáo doanh thu"
            description="Thống kê doanh thu thu được theo ngày/tuần/tháng"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showRevenueReportModal">
              Xem báo cáo
            </a-button>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); padding: 32px; text-align: center;">
              <ToolOutlined style="font-size: 48px; color: white;" />
            </div>
          </template>
          <a-card-meta
            title="Báo cáo bảo trì"
            description="Thống kê yêu cầu bảo trì đã xử lý và thời gian xử lý"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showMaintenanceReportModal">
              Xem báo cáo
            </a-button>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); padding: 32px; text-align: center;">
              <WarningOutlined style="font-size: 48px; color: white;" />
            </div>
          </template>
          <a-card-meta
            title="Báo cáo công nợ"
            description="Danh sách sinh viên nợ tiền và tổng công nợ"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showDebtReportModal">
              Xem báo cáo
            </a-button>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #30cfd0 0%, #330867 100%); padding: 32px; text-align: center;">
              <HomeOutlined style="font-size: 48px; color: white;" />
            </div>
          </template>
          <a-card-meta
            title="Báo cáo tỷ lệ lấp đầy"
            description="Thống kê tỷ lệ lấp đầy phòng theo tòa nhà"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showOccupancyReportModal">
              Xem báo cáo
            </a-button>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card hoverable>
          <template #cover>
            <div style="background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%); padding: 32px; text-align: center;">
              <DownloadOutlined style="font-size: 48px; color: #333;" />
            </div>
          </template>
          <a-card-meta
            title="Xuất dữ liệu"
            description="Xuất danh sách sinh viên, hợp đồng, hóa đơn ra Excel"
          />
          <div style="margin-top: 16px;">
            <a-button type="primary" block @click="showExportModal">
              Xuất dữ liệu
            </a-button>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Work Report Modal -->
    <a-modal
      v-model:open="workReportModalVisible"
      title="Báo cáo công việc"
      width="800px"
      :footer="null"
    >
      <a-form layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Từ ngày">
              <a-date-picker v-model:value="workReportForm.startDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Đến ngày">
              <a-date-picker v-model:value="workReportForm.endDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>

      <a-button type="primary" block size="large" @click="generateWorkReport" :loading="generating">
        <FileTextOutlined /> Tạo báo cáo
      </a-button>

      <a-divider />

      <div v-if="workReportData">
        <a-descriptions bordered :column="2" size="small">
          <a-descriptions-item label="Đơn đã duyệt">
            {{ workReportData.approvedApplications }}
          </a-descriptions-item>
          <a-descriptions-item label="Đơn từ chối">
            {{ workReportData.rejectedApplications }}
          </a-descriptions-item>
          <a-descriptions-item label="Thanh toán đã thu">
            {{ formatCurrency(workReportData.totalPayments) }} VNĐ
          </a-descriptions-item>
          <a-descriptions-item label="Số giao dịch">
            {{ workReportData.paymentCount }}
          </a-descriptions-item>
          <a-descriptions-item label="Bảo trì đã xử lý">
            {{ workReportData.completedMaintenance }}
          </a-descriptions-item>
          <a-descriptions-item label="Bảo trì đang xử lý">
            {{ workReportData.pendingMaintenance }}
          </a-descriptions-item>
        </a-descriptions>

        <div style="margin-top: 16px; text-align: right;">
          <a-button type="primary" @click="exportWorkReport">
            <DownloadOutlined /> Xuất Excel
          </a-button>
        </div>
      </div>
    </a-modal>

    <!-- Export Modal -->
    <a-modal
      v-model:open="exportModalVisible"
      title="Xuất dữ liệu"
      @ok="handleExport"
      :confirmLoading="exporting"
    >
      <a-form layout="vertical">
        <a-form-item label="Loại dữ liệu">
          <a-select v-model:value="exportForm.type" placeholder="Chọn loại dữ liệu">
            <a-select-option value="students">Danh sách sinh viên</a-select-option>
            <a-select-option value="contracts">Danh sách hợp đồng</a-select-option>
            <a-select-option value="invoices">Danh sách hóa đơn</a-select-option>
            <a-select-option value="payments">Lịch sử thanh toán</a-select-option>
            <a-select-option value="maintenance">Yêu cầu bảo trì</a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="Khoảng thời gian">
          <a-range-picker v-model:value="exportForm.dateRange" style="width: 100%;" format="DD/MM/YYYY" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import {
  FileTextOutlined, DollarOutlined, ToolOutlined, WarningOutlined,
  HomeOutlined, DownloadOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'

const generating = ref(false)
const exporting = ref(false)

const workReportModalVisible = ref(false)
const workReportData = ref(null)
const workReportForm = ref({
  startDate: null,
  endDate: null
})

const exportModalVisible = ref(false)
const exportForm = ref({
  type: undefined,
  dateRange: []
})

const showWorkReportModal = () => {
  workReportModalVisible.value = true
}

const showRevenueReportModal = () => {
  message.info('Tính năng báo cáo doanh thu đang được phát triển')
}

const showMaintenanceReportModal = () => {
  message.info('Tính năng báo cáo bảo trì đang được phát triển')
}

const showDebtReportModal = () => {
  message.info('Tính năng báo cáo công nợ đang được phát triển')
}

const showOccupancyReportModal = () => {
  message.info('Tính năng báo cáo tỷ lệ lấp đầy đang được phát triển')
}

const showExportModal = () => {
  exportModalVisible.value = true
}

const generateWorkReport = async () => {
  generating.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Mock data
    workReportData.value = {
      approvedApplications: 45,
      rejectedApplications: 3,
      totalPayments: 125000000,
      paymentCount: 67,
      completedMaintenance: 23,
      pendingMaintenance: 5
    }
    
    message.success('Đã tạo báo cáo')
  } catch (error) {
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportWorkReport = () => {
  message.success('Đã xuất báo cáo ra file Excel')
  // TODO: Implement export
}

const handleExport = async () => {
  if (!exportForm.value.type) {
    message.warning('Vui lòng chọn loại dữ liệu')
    return
  }

  exporting.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1000))
    message.success('Đã xuất dữ liệu ra file Excel')
    exportModalVisible.value = false
  } catch (error) {
    message.error('Lỗi xuất dữ liệu')
  } finally {
    exporting.value = false
  }
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value)
}
</script>
