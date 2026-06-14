<template>
  <div>
    <PageHeaderAnt title="Báo cáo & Thống kê" subtitle="Xem và xuất báo cáo công việc" />

    <!-- Quick Stats -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 24px;">
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="background: #f0f5ff;">
          <a-statistic
            title="Đơn chờ xử lý"
            :value="stats.pendingApplications"
            :valueStyle="{ color: '#1890ff' }"
          >
            <template #prefix><FileTextOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="background: #f6ffed;">
          <a-statistic
            title="Doanh thu tháng"
            :value="stats.monthlyRevenue"
            :valueStyle="{ color: '#52c41a' }"
            suffix="VNĐ"
          >
            <template #prefix><DollarOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="background: #fff7e6;">
          <a-statistic
            title="Bảo trì chưa xử lý"
            :value="stats.pendingMaintenance"
            :valueStyle="{ color: '#fa8c16' }"
          >
            <template #prefix><ToolOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
      <a-col :xs="12" :md="6">
        <a-card :bordered="false" style="background: #fff1f0;">
          <a-statistic
            title="Công nợ"
            :value="stats.totalDebt"
            :valueStyle="{ color: '#ff4d4f' }"
            suffix="VNĐ"
          >
            <template #prefix><WarningOutlined /></template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <!-- Report Cards -->
    <a-row :gutter="[16, 16]">
      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #e6f7ff; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <FileTextOutlined style="font-size: 24px; color: #1890ff;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Báo cáo công việc</div>
              <div style="font-size: 12px; color: #8c8c8c;">Đơn xử lý, thanh toán</div>
            </div>
          </div>
          <a-button type="primary" block @click="showWorkReportModal">
            Xem báo cáo
          </a-button>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #f6ffed; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <DollarOutlined style="font-size: 24px; color: #52c41a;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Báo cáo doanh thu</div>
              <div style="font-size: 12px; color: #8c8c8c;">Thu nhập theo thời gian</div>
            </div>
          </div>
          <a-button type="primary" block @click="showRevenueReportModal">
            Xem báo cáo
          </a-button>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #fff7e6; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <ToolOutlined style="font-size: 24px; color: #fa8c16;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Báo cáo bảo trì</div>
              <div style="font-size: 12px; color: #8c8c8c;">Yêu cầu và thời gian xử lý</div>
            </div>
          </div>
          <a-button type="primary" block @click="showMaintenanceReportModal">
            Xem báo cáo
          </a-button>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #fff1f0; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <WarningOutlined style="font-size: 24px; color: #ff4d4f;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Báo cáo công nợ</div>
              <div style="font-size: 12px; color: #8c8c8c;">Sinh viên nợ tiền</div>
            </div>
          </div>
          <a-button type="primary" block @click="showDebtReportModal">
            Xem báo cáo
          </a-button>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #f9f0ff; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <HomeOutlined style="font-size: 24px; color: #722ed1;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Báo cáo tỷ lệ lấp đầy</div>
              <div style="font-size: 12px; color: #8c8c8c;">Theo tòa nhà</div>
            </div>
          </div>
          <a-button type="primary" block @click="showOccupancyReportModal">
            Xem báo cáo
          </a-button>
        </a-card>
      </a-col>

      <a-col :xs="24" :md="12" :lg="8">
        <a-card :bordered="false" hoverable>
          <div style="display: flex; align-items: center; margin-bottom: 12px;">
            <div style="width: 48px; height: 48px; background: #e6fffb; border-radius: 8px; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
              <DownloadOutlined style="font-size: 24px; color: #13c2c2;" />
            </div>
            <div>
              <div style="font-size: 16px; font-weight: 600;">Xuất dữ liệu</div>
              <div style="font-size: 12px; color: #8c8c8c;">Sinh viên, hợp đồng, hóa đơn</div>
            </div>
          </div>
          <a-button type="primary" block @click="showExportModal">
            Xuất dữ liệu
          </a-button>
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

    <!-- Revenue Report Modal -->
    <a-modal
      v-model:open="revenueReportModalVisible"
      title="Báo cáo doanh thu"
      width="900px"
      :footer="null"
    >
      <a-form layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Từ ngày">
              <a-date-picker v-model:value="revenueReportForm.startDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Đến ngày">
              <a-date-picker v-model:value="revenueReportForm.endDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>

      <a-button type="primary" block size="large" @click="generateRevenueReport" :loading="generating">
        <DollarOutlined /> Tạo báo cáo
      </a-button>

      <a-divider />

      <div v-if="revenueReportData">
        <!-- Summary Stats -->
        <a-row :gutter="16" style="margin-bottom: 16px;">
          <a-col :span="8">
            <a-statistic 
              title="Tổng doanh thu"
              :value="revenueReportData.totalRevenue"
              suffix="VNĐ"
              :valueStyle="{ color: '#52c41a' }"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Số giao dịch"
              :value="revenueReportData.paymentCount"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Trung bình/GD"
              :value="Math.round(revenueReportData.averagePayment)"
              suffix="VNĐ"
            />
          </a-col>
        </a-row>

        <!-- By Payment Method -->
        <a-card title="Theo phương thức thanh toán" size="small" style="margin-bottom: 16px;">
          <a-descriptions bordered :column="2" size="small">
            <a-descriptions-item label="Tiền mặt">
              {{ formatCurrency(revenueReportData.byMethod.Cash) }} VNĐ
            </a-descriptions-item>
            <a-descriptions-item label="Chuyển khoản">
              {{ formatCurrency(revenueReportData.byMethod.BankTransfer) }} VNĐ
            </a-descriptions-item>
            <a-descriptions-item label="Thẻ">
              {{ formatCurrency(revenueReportData.byMethod.Card) }} VNĐ
            </a-descriptions-item>
            <a-descriptions-item label="Khác">
              {{ formatCurrency(revenueReportData.byMethod.Other) }} VNĐ
            </a-descriptions-item>
          </a-descriptions>
        </a-card>

        <!-- By Month -->
        <a-card title="Theo tháng" size="small" style="margin-bottom: 16px;">
          <a-table 
            :dataSource="revenueReportData.byMonth" 
            :columns="[
              { title: 'Tháng', dataIndex: 'month', key: 'month' },
              { title: 'Doanh thu', dataIndex: 'amount', key: 'amount', customRender: ({ text }) => formatCurrency(text) + ' VNĐ' }
            ]"
            :pagination="false"
            size="small"
          />
        </a-card>

        <div style="text-align: right;">
          <a-button type="primary" @click="exportRevenueReport">
            <DownloadOutlined /> Xuất Excel
          </a-button>
        </div>
      </div>
    </a-modal>

    <!-- Maintenance Report Modal -->
    <a-modal
      v-model:open="maintenanceReportModalVisible"
      title="Báo cáo bảo trì"
      width="900px"
      :footer="null"
    >
      <a-form layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Từ ngày">
              <a-date-picker v-model:value="maintenanceReportForm.startDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Đến ngày">
              <a-date-picker v-model:value="maintenanceReportForm.endDate" style="width: 100%;" format="DD/MM/YYYY" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>

      <a-button type="primary" block size="large" @click="generateMaintenanceReport" :loading="generating">
        <ToolOutlined /> Tạo báo cáo
      </a-button>

      <a-divider />

      <div v-if="maintenanceReportData">
        <!-- Summary Stats -->
        <a-row :gutter="16" style="margin-bottom: 16px;">
          <a-col :span="8">
            <a-statistic 
              title="Tổng yêu cầu"
              :value="maintenanceReportData.total"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Thời gian xử lý TB"
              :value="maintenanceReportData.avgResolutionDays"
              suffix="ngày"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Tổng chi phí"
              :value="maintenanceReportData.totalCost"
              suffix="VNĐ"
              :valueStyle="{ color: '#fa8c16' }"
            />
          </a-col>
        </a-row>

        <!-- By Status -->
        <a-card title="Theo trạng thái" size="small" style="margin-bottom: 16px;">
          <a-descriptions bordered :column="2" size="small">
            <a-descriptions-item label="Mới">
              {{ maintenanceReportData.byStatus.New }}
            </a-descriptions-item>
            <a-descriptions-item label="Đã phân công">
              {{ maintenanceReportData.byStatus.Assigned }}
            </a-descriptions-item>
            <a-descriptions-item label="Đang xử lý">
              {{ maintenanceReportData.byStatus.InProgress }}
            </a-descriptions-item>
            <a-descriptions-item label="Hoàn thành">
              {{ maintenanceReportData.byStatus.Done }}
            </a-descriptions-item>
            <a-descriptions-item label="Từ chối">
              {{ maintenanceReportData.byStatus.Rejected }}
            </a-descriptions-item>
          </a-descriptions>
        </a-card>

        <!-- By Priority -->
        <a-card title="Theo mức độ ưu tiên" size="small" style="margin-bottom: 16px;">
          <a-descriptions bordered :column="2" size="small">
            <a-descriptions-item label="Thấp">
              {{ maintenanceReportData.byPriority.Low }}
            </a-descriptions-item>
            <a-descriptions-item label="Trung bình">
              {{ maintenanceReportData.byPriority.Medium }}
            </a-descriptions-item>
            <a-descriptions-item label="Cao">
              {{ maintenanceReportData.byPriority.High }}
            </a-descriptions-item>
            <a-descriptions-item label="Khẩn cấp">
              {{ maintenanceReportData.byPriority.Urgent }}
            </a-descriptions-item>
          </a-descriptions>
        </a-card>

        <div style="text-align: right;">
          <a-button type="primary" @click="exportMaintenanceReport">
            <DownloadOutlined /> Xuất Excel
          </a-button>
        </div>
      </div>
    </a-modal>

    <!-- Debt Report Modal -->
    <a-modal
      v-model:open="debtReportModalVisible"
      title="Báo cáo công nợ"
      width="900px"
      :footer="null"
    >
      <div v-if="debtReportData">
        <!-- Summary Stats -->
        <a-row :gutter="16" style="margin-bottom: 16px;">
          <a-col :span="8">
            <a-statistic 
              title="Tổng công nợ"
              :value="debtReportData.totalDebt"
              suffix="VNĐ"
              :valueStyle="{ color: '#ff4d4f' }"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Số sinh viên nợ"
              :value="debtReportData.studentCount"
            />
          </a-col>
          <a-col :span="8">
            <a-statistic 
              title="Số hóa đơn quá hạn"
              :value="debtReportData.invoiceCount"
            />
          </a-col>
        </a-row>

        <a-divider />

        <!-- Debt List Table -->
        <a-table 
          :dataSource="debtReportData.debtList" 
          :columns="[
            { title: 'Mã SV', dataIndex: 'studentCode', key: 'studentCode', width: 100 },
            { title: 'Tên sinh viên', dataIndex: 'studentName', key: 'studentName' },
            { title: 'Phòng', dataIndex: 'roomNumber', key: 'roomNumber', width: 80 },
            { title: 'Số HĐ nợ', dataIndex: 'invoiceCount', key: 'invoiceCount', width: 90 },
            { title: 'Tổng nợ', dataIndex: 'totalDebt', key: 'totalDebt', width: 130,
              customRender: ({ text }) => formatCurrency(text) + ' VNĐ' },
            { title: 'Hạn cũ nhất', dataIndex: 'oldestDueDate', key: 'oldestDueDate', width: 110,
              customRender: ({ text }) => text ? dayjs(text).format('DD/MM/YYYY') : '' }
          ]"
          :pagination="{ pageSize: 10 }"
          size="small"
          :scroll="{ y: 400 }"
        />

        <div style="margin-top: 16px; text-align: right;">
          <a-button type="primary" @click="exportDebtReport">
            <DownloadOutlined /> Xuất Excel
          </a-button>
        </div>
      </div>
      <div v-else style="text-align: center; padding: 40px;">
        <a-spin size="large" />
      </div>
    </a-modal>

    <!-- Occupancy Report Modal -->
    <a-modal
      v-model:open="occupancyReportModalVisible"
      title="Báo cáo tỷ lệ lấp đầy"
      width="900px"
      :footer="null"
    >
      <div v-if="occupancyReportData">
        <!-- Overall Stats -->
        <a-row :gutter="16" style="margin-bottom: 16px;">
          <a-col :span="6">
            <a-statistic 
              title="Tỷ lệ lấp đầy"
              :value="occupancyReportData.overallRate"
              suffix="%"
              :valueStyle="{ color: '#722ed1' }"
            />
          </a-col>
          <a-col :span="6">
            <a-statistic 
              title="Tổng giường"
              :value="occupancyReportData.totalBeds"
            />
          </a-col>
          <a-col :span="6">
            <a-statistic 
              title="Đã có người"
              :value="occupancyReportData.totalOccupied"
              :valueStyle="{ color: '#52c41a' }"
            />
          </a-col>
          <a-col :span="6">
            <a-statistic 
              title="Còn trống"
              :value="occupancyReportData.totalEmpty"
              :valueStyle="{ color: '#1890ff' }"
            />
          </a-col>
        </a-row>

        <a-divider />

        <!-- By Building Table -->
        <a-table 
          :dataSource="occupancyReportData.byBuilding" 
          :columns="[
            { title: 'Tòa nhà', dataIndex: 'buildingName', key: 'buildingName' },
            { title: 'Số phòng', dataIndex: 'totalRooms', key: 'totalRooms', width: 100 },
            { title: 'Tổng giường', dataIndex: 'totalBeds', key: 'totalBeds', width: 110 },
            { title: 'Có người', dataIndex: 'occupiedBeds', key: 'occupiedBeds', width: 100 },
            { title: 'Trống', dataIndex: 'emptyBeds', key: 'emptyBeds', width: 80 },
            { title: 'Tỷ lệ (%)', dataIndex: 'occupancyRate', key: 'occupancyRate', width: 100,
              customRender: ({ text }) => text + '%' }
          ]"
          :pagination="false"
          size="small"
        />

        <div style="margin-top: 16px; text-align: right;">
          <a-button type="primary" @click="exportOccupancyReport">
            <DownloadOutlined /> Xuất Excel
          </a-button>
        </div>
      </div>
      <div v-else style="text-align: center; padding: 40px;">
        <a-spin size="large" />
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
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import {
  FileTextOutlined, DollarOutlined, ToolOutlined, WarningOutlined,
  HomeOutlined, DownloadOutlined
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'
import { roomApplicationService } from '@/services/roomApplicationService'
import { invoiceService } from '@/services/invoiceService'
import { paymentService } from '@/services/paymentService'
import maintenanceRequestService from '@/services/maintenanceRequestService'
import { contractService } from '@/services/contractService'
import { roomService } from '@/services/roomService'
import { buildingService } from '@/services/buildingService'
import { exportToExcel } from '@/utils/excelExport'
import axios from 'axios'
import dayjs from 'dayjs'

const generating = ref(false)
const exporting = ref(false)
const loading = ref(false)

const stats = ref({
  pendingApplications: 0,
  monthlyRevenue: 0,
  pendingMaintenance: 0,
  totalDebt: 0
})

const workReportModalVisible = ref(false)
const workReportData = ref(null)
const workReportForm = ref({
  startDate: dayjs().subtract(30, 'day'),
  endDate: dayjs()
})

const revenueReportModalVisible = ref(false)
const revenueReportData = ref(null)
const revenueReportForm = ref({
  startDate: dayjs().subtract(30, 'day'),
  endDate: dayjs()
})

const maintenanceReportModalVisible = ref(false)
const maintenanceReportData = ref(null)
const maintenanceReportForm = ref({
  startDate: dayjs().subtract(30, 'day'),
  endDate: dayjs()
})

const debtReportModalVisible = ref(false)
const debtReportData = ref(null)

const occupancyReportModalVisible = ref(false)
const occupancyReportData = ref(null)

const exportModalVisible = ref(false)
const exportForm = ref({
  type: undefined,
  dateRange: [dayjs().subtract(30, 'day'), dayjs()]
})

// Load stats on mount
const loadStats = async () => {
  loading.value = true
  try {
    const [applications, invoices, maintenance] = await Promise.all([
      roomApplicationService.getAll(),
      invoiceService.getAll(),
      maintenanceRequestService.getAll()
    ])

    // Pending applications
    stats.value.pendingApplications = applications.filter(a => a.status === 'Pending').length

    // Monthly revenue (current month)
    const now = new Date()
    const currentMonth = now.getMonth() + 1
    const currentYear = now.getFullYear()
    
    const monthlyInvoices = invoices.filter(inv => {
      return inv.billingMonth === currentMonth && 
             inv.billingYear === currentYear &&
             (inv.status === 'Paid' || inv.status === 'PartialPaid')
    })
    
    stats.value.monthlyRevenue = monthlyInvoices.reduce((sum, inv) => sum + (inv.paidAmount || 0), 0)

    // Pending maintenance
    stats.value.pendingMaintenance = maintenance.filter(m => 
      m.status === 'New' || m.status === 'Assigned' || m.status === 'InProgress'
    ).length

    // Total debt
    const overdueInvoices = invoices.filter(inv => 
      (inv.status === 'Unpaid' || inv.status === 'PartialPaid') &&
      new Date(inv.dueDate) < now
    )
    
    stats.value.totalDebt = overdueInvoices.reduce((sum, inv) => sum + (inv.debtAmount || 0), 0)
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    loading.value = false
  }
}

const showWorkReportModal = () => {
  workReportModalVisible.value = true
  workReportData.value = null
}

const showRevenueReportModal = () => {
  revenueReportModalVisible.value = true
  revenueReportData.value = null
}

const showMaintenanceReportModal = () => {
  maintenanceReportModalVisible.value = true
  maintenanceReportData.value = null
}

const showDebtReportModal = async () => {
  debtReportModalVisible.value = true
  await generateDebtReport()
}

const showOccupancyReportModal = async () => {
  occupancyReportModalVisible.value = true
  await generateOccupancyReport()
}

const showExportModal = () => {
  exportModalVisible.value = true
}

const generateWorkReport = async () => {
  if (!workReportForm.value.startDate || !workReportForm.value.endDate) {
    message.warning('Vui lòng chọn khoảng thời gian')
    return
  }

  generating.value = true
  try {
    const startDate = workReportForm.value.startDate.toDate()
    const endDate = workReportForm.value.endDate.toDate()

    const [applications, payments, maintenance] = await Promise.all([
      roomApplicationService.getAll(),
      paymentService.getAll(),
      maintenanceRequestService.getAll()
    ])

    // Filter by date range
    const applicationsInRange = applications.filter(a => {
      const createdDate = new Date(a.createdAt)
      return createdDate >= startDate && createdDate <= endDate
    })

    const paymentsInRange = payments.filter(p => {
      const paidDate = new Date(p.paidAt || p.paymentDate)
      return paidDate >= startDate && paidDate <= endDate
    })

    const maintenanceInRange = maintenance.filter(m => {
      const createdDate = new Date(m.createdAt)
      return createdDate >= startDate && createdDate <= endDate
    })

    workReportData.value = {
      approvedApplications: applicationsInRange.filter(a => a.status === 'Approved').length,
      rejectedApplications: applicationsInRange.filter(a => a.status === 'Rejected').length,
      totalPayments: paymentsInRange.reduce((sum, p) => sum + (p.amount || 0), 0),
      paymentCount: paymentsInRange.length,
      completedMaintenance: maintenanceInRange.filter(m => m.status === 'Done').length,
      pendingMaintenance: maintenanceInRange.filter(m => 
        m.status === 'New' || m.status === 'Assigned' || m.status === 'InProgress'
      ).length
    }
    
    message.success('Đã tạo báo cáo')
  } catch (error) {
    console.error('Error generating report:', error)
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportWorkReport = () => {
  if (!workReportData.value) return
  
  try {
    const data = [
      {
        category: 'Đơn đăng ký',
        detail: 'Đã duyệt',
        value: workReportData.value.approvedApplications
      },
      {
        category: 'Đơn đăng ký',
        detail: 'Từ chối',
        value: workReportData.value.rejectedApplications
      },
      {
        category: 'Thanh toán',
        detail: 'Tổng thu (VNĐ)',
        value: workReportData.value.totalPayments
      },
      {
        category: 'Thanh toán',
        detail: 'Số giao dịch',
        value: workReportData.value.paymentCount
      },
      {
        category: 'Bảo trì',
        detail: 'Đã xử lý',
        value: workReportData.value.completedMaintenance
      },
      {
        category: 'Bảo trì',
        detail: 'Đang xử lý',
        value: workReportData.value.pendingMaintenance
      }
    ]

    const columnMapping = {
      category: 'Danh mục',
      detail: 'Chi tiết',
      value: 'Giá trị'
    }

    const filename = `BaoCaoCongViec_${workReportForm.value.startDate.format('DDMMYYYY')}_${workReportForm.value.endDate.format('DDMMYYYY')}`
    
    exportToExcel(data, columnMapping, filename, 'Báo cáo công việc')
    message.success('Đã xuất báo cáo Excel')
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất báo cáo: ' + error.message)
  }
}

const generateRevenueReport = async () => {
  if (!revenueReportForm.value.startDate || !revenueReportForm.value.endDate) {
    message.warning('Vui lòng chọn khoảng thời gian')
    return
  }

  generating.value = true
  try {
    const startDate = revenueReportForm.value.startDate.toDate()
    const endDate = revenueReportForm.value.endDate.toDate()

    const [payments, invoices] = await Promise.all([
      paymentService.getAll(),
      invoiceService.getAll()
    ])

    // Filter payments by date range
    const paymentsInRange = payments.filter(p => {
      const paidDate = new Date(p.paidAt || p.paymentDate)
      return paidDate >= startDate && paidDate <= endDate
    })

    // Group by payment method
    const byMethod = {
      Cash: 0,
      BankTransfer: 0,
      Card: 0,
      Other: 0
    }

    paymentsInRange.forEach(p => {
      const method = p.paymentMethod || 'Other'
      byMethod[method] = (byMethod[method] || 0) + (p.amount || 0)
    })

    // Group by month
    const byMonth = {}
    paymentsInRange.forEach(p => {
      const date = new Date(p.paidAt || p.paymentDate)
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
      byMonth[monthKey] = (byMonth[monthKey] || 0) + (p.amount || 0)
    })

    revenueReportData.value = {
      totalRevenue: paymentsInRange.reduce((sum, p) => sum + (p.amount || 0), 0),
      paymentCount: paymentsInRange.length,
      averagePayment: paymentsInRange.length > 0 
        ? paymentsInRange.reduce((sum, p) => sum + (p.amount || 0), 0) / paymentsInRange.length 
        : 0,
      byMethod,
      byMonth: Object.keys(byMonth).sort().map(month => ({
        month,
        amount: byMonth[month]
      })),
      payments: paymentsInRange
    }
    
    message.success('Đã tạo báo cáo doanh thu')
  } catch (error) {
    console.error('Error generating revenue report:', error)
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportRevenueReport = () => {
  if (!revenueReportData.value) return
  
  try {
    const columnMapping = {
      paidAt: 'Ngày thanh toán',
      invoiceCode: 'Mã hóa đơn',
      amount: 'Số tiền',
      paymentMethod: 'Phương thức',
      note: 'Ghi chú'
    }

    const filename = `BaoCaoDoanhThu_${revenueReportForm.value.startDate.format('DDMMYYYY')}_${revenueReportForm.value.endDate.format('DDMMYYYY')}`
    
    exportToExcel(revenueReportData.value.payments, columnMapping, filename, 'Doanh thu')
    message.success('Đã xuất báo cáo Excel')
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất báo cáo: ' + error.message)
  }
}

const generateMaintenanceReport = async () => {
  if (!maintenanceReportForm.value.startDate || !maintenanceReportForm.value.endDate) {
    message.warning('Vui lòng chọn khoảng thời gian')
    return
  }

  generating.value = true
  try {
    const startDate = maintenanceReportForm.value.startDate.toDate()
    const endDate = maintenanceReportForm.value.endDate.toDate()

    const maintenance = await maintenanceRequestService.getAll()

    const maintenanceInRange = maintenance.filter(m => {
      const createdDate = new Date(m.createdAt)
      return createdDate >= startDate && createdDate <= endDate
    })

    // Stats by status
    const byStatus = {
      New: maintenanceInRange.filter(m => m.status === 'New').length,
      Assigned: maintenanceInRange.filter(m => m.status === 'Assigned').length,
      InProgress: maintenanceInRange.filter(m => m.status === 'InProgress').length,
      Done: maintenanceInRange.filter(m => m.status === 'Done').length,
      Rejected: maintenanceInRange.filter(m => m.status === 'Rejected').length
    }

    // Stats by category
    const byCategory = {}
    maintenanceInRange.forEach(m => {
      const cat = m.category || 'Khác'
      byCategory[cat] = (byCategory[cat] || 0) + 1
    })

    // Stats by priority
    const byPriority = {
      Low: maintenanceInRange.filter(m => m.priority === 'Low').length,
      Medium: maintenanceInRange.filter(m => m.priority === 'Medium').length,
      High: maintenanceInRange.filter(m => m.priority === 'High').length,
      Urgent: maintenanceInRange.filter(m => m.priority === 'Urgent').length
    }

    // Average resolution time (for completed)
    const completed = maintenanceInRange.filter(m => m.status === 'Done' && m.resolvedAt)
    const avgResolutionDays = completed.length > 0
      ? completed.reduce((sum, m) => {
          const created = new Date(m.createdAt)
          const resolved = new Date(m.resolvedAt)
          return sum + (resolved - created) / (1000 * 60 * 60 * 24)
        }, 0) / completed.length
      : 0

    maintenanceReportData.value = {
      total: maintenanceInRange.length,
      byStatus,
      byCategory,
      byPriority,
      avgResolutionDays: avgResolutionDays.toFixed(1),
      totalCost: completed.reduce((sum, m) => sum + (m.actualCost || 0), 0),
      requests: maintenanceInRange
    }
    
    message.success('Đã tạo báo cáo bảo trì')
  } catch (error) {
    console.error('Error generating maintenance report:', error)
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportMaintenanceReport = () => {
  if (!maintenanceReportData.value) return
  
  try {
    const columnMapping = {
      id: 'Mã yêu cầu',
      title: 'Tiêu đề',
      category: 'Danh mục',
      priority: 'Ưu tiên',
      status: 'Trạng thái',
      roomNumber: 'Phòng',
      createdAt: 'Ngày tạo',
      resolvedAt: 'Ngày hoàn thành',
      actualCost: 'Chi phí',
      rating: 'Đánh giá'
    }

    const filename = `BaoCaoBaoTri_${maintenanceReportForm.value.startDate.format('DDMMYYYY')}_${maintenanceReportForm.value.endDate.format('DDMMYYYY')}`
    
    exportToExcel(maintenanceReportData.value.requests, columnMapping, filename, 'Bảo trì')
    message.success('Đã xuất báo cáo Excel')
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất báo cáo: ' + error.message)
  }
}

const generateDebtReport = async () => {
  generating.value = true
  try {
    const invoices = await invoiceService.getAll()
    
    const now = new Date()
    const overdueInvoices = invoices.filter(inv => 
      (inv.status === 'Unpaid' || inv.status === 'PartialPaid') &&
      new Date(inv.dueDate) < now
    )

    // Group by student
    const debtByStudent = {}
    overdueInvoices.forEach(inv => {
      const key = inv.studentId
      if (!debtByStudent[key]) {
        debtByStudent[key] = {
          studentId: inv.studentId,
          studentName: inv.studentName || 'N/A',
          studentCode: inv.studentCode || 'N/A',
          roomNumber: inv.roomNumber || 'N/A',
          invoiceCount: 0,
          totalDebt: 0,
          oldestDueDate: inv.dueDate
        }
      }
      debtByStudent[key].invoiceCount++
      debtByStudent[key].totalDebt += (inv.debtAmount || 0)
      if (new Date(inv.dueDate) < new Date(debtByStudent[key].oldestDueDate)) {
        debtByStudent[key].oldestDueDate = inv.dueDate
      }
    })

    const debtList = Object.values(debtByStudent).sort((a, b) => b.totalDebt - a.totalDebt)

    debtReportData.value = {
      totalDebt: overdueInvoices.reduce((sum, inv) => sum + (inv.debtAmount || 0), 0),
      studentCount: debtList.length,
      invoiceCount: overdueInvoices.length,
      debtList
    }
    
    message.success('Đã tạo báo cáo công nợ')
  } catch (error) {
    console.error('Error generating debt report:', error)
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportDebtReport = () => {
  if (!debtReportData.value) return
  
  try {
    const columnMapping = {
      studentCode: 'Mã sinh viên',
      studentName: 'Tên sinh viên',
      roomNumber: 'Phòng',
      invoiceCount: 'Số hóa đơn nợ',
      totalDebt: 'Tổng nợ (VNĐ)',
      oldestDueDate: 'Hạn cũ nhất'
    }

    const filename = `BaoCaoCongNo_${dayjs().format('DDMMYYYY')}`
    
    exportToExcel(debtReportData.value.debtList, columnMapping, filename, 'Công nợ')
    message.success('Đã xuất báo cáo Excel')
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất báo cáo: ' + error.message)
  }
}

const generateOccupancyReport = async () => {
  generating.value = true
  try {
    const [rooms, buildings] = await Promise.all([
      roomService.getAll(),
      buildingService.getAll()
    ])

    const occupancyByBuilding = buildings.map(building => {
      const buildingRooms = rooms.filter(r => r.buildingId === building.id)
      const totalBeds = buildingRooms.reduce((sum, r) => sum + (r.maxOccupants || 0), 0)
      const occupiedBeds = buildingRooms.reduce((sum, r) => sum + (r.currentOccupants || 0), 0)
      const occupancyRate = totalBeds > 0 ? (occupiedBeds / totalBeds * 100).toFixed(1) : 0

      return {
        buildingId: building.id,
        buildingName: building.name,
        totalRooms: buildingRooms.length,
        totalBeds,
        occupiedBeds,
        emptyBeds: totalBeds - occupiedBeds,
        occupancyRate: parseFloat(occupancyRate)
      }
    })

    const totalBeds = occupancyByBuilding.reduce((sum, b) => sum + b.totalBeds, 0)
    const totalOccupied = occupancyByBuilding.reduce((sum, b) => sum + b.occupiedBeds, 0)

    occupancyReportData.value = {
      overallRate: totalBeds > 0 ? (totalOccupied / totalBeds * 100).toFixed(1) : 0,
      totalBeds,
      totalOccupied,
      totalEmpty: totalBeds - totalOccupied,
      byBuilding: occupancyByBuilding
    }
    
    message.success('Đã tạo báo cáo tỷ lệ lấp đầy')
  } catch (error) {
    console.error('Error generating occupancy report:', error)
    message.error('Lỗi tạo báo cáo')
  } finally {
    generating.value = false
  }
}

const exportOccupancyReport = () => {
  if (!occupancyReportData.value) return
  
  try {
    const columnMapping = {
      buildingName: 'Tòa nhà',
      totalRooms: 'Số phòng',
      totalBeds: 'Tổng giường',
      occupiedBeds: 'Giường có người',
      emptyBeds: 'Giường trống',
      occupancyRate: 'Tỷ lệ lấp đầy (%)'
    }

    const filename = `BaoCaoTyLeLapDay_${dayjs().format('DDMMYYYY')}`
    
    exportToExcel(occupancyReportData.value.byBuilding, columnMapping, filename, 'Tỷ lệ lấp đầy')
    message.success('Đã xuất báo cáo Excel')
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất báo cáo: ' + error.message)
  }
}

const handleExport = async () => {
  if (!exportForm.value.type) {
    message.warning('Vui lòng chọn loại dữ liệu')
    return
  }

  exporting.value = true
  try {
    let data = []
    let fileName = ''

    switch (exportForm.value.type) {
      case 'students':
        const response = await axios.get('http://localhost:5002/api/users', {
          headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        })
        data = response.data.filter(u => u.role === 'Student')
        fileName = 'DanhSachSinhVien'
        break
      
      case 'contracts':
        data = await contractService.getAll()
        fileName = 'DanhSachHopDong'
        break
      
      case 'invoices':
        data = await invoiceService.getAll()
        fileName = 'DanhSachHoaDon'
        break
      
      case 'payments':
        data = await paymentService.getAll()
        fileName = 'LichSuThanhToan'
        break
      
      case 'maintenance':
        data = await maintenanceRequestService.getAll()
        fileName = 'YeuCauBaoTri'
        break
    }

    // Simple CSV export
    const csv = convertToCSV(data)
    const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `${fileName}_${dayjs().format('YYYYMMDD')}.csv`
    link.click()
    URL.revokeObjectURL(url)

    message.success('Đã xuất dữ liệu ra file CSV')
    exportModalVisible.value = false
  } catch (error) {
    console.error('Export error:', error)
    message.error('Lỗi xuất dữ liệu')
  } finally {
    exporting.value = false
  }
}

const convertToCSV = (data) => {
  if (!data || data.length === 0) return ''
  
  const headers = Object.keys(data[0]).join(',')
  const rows = data.map(item => 
    Object.values(item).map(v => 
      typeof v === 'string' && v.includes(',') ? `"${v}"` : v
    ).join(',')
  )
  
  return [headers, ...rows].join('\n')
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN').format(value || 0)
}

onMounted(() => {
  loadStats()
})
</script>
