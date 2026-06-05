<template>
  <div>
    <PageHeaderAnt title="Dashboard Nhân viên" subtitle="Tổng quan hoạt động hôm nay">
    </PageHeaderAnt>

    <!-- Stats -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col v-for="s in stats" :key="s.label" :xs="24" :sm="12" :lg="6">
        <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
          <div style="display: flex; justify-content: space-between; align-items: flex-start;">
            <div>
              <div style="font-size: 14px; color: #8c8c8c; margin-bottom: 8px">{{ s.label }}</div>
              <div style="font-size: 28px; font-weight: 700; color: #000">{{ s.value }}</div>
            </div>
            <div :style="{ fontSize: '32px', color: s.color, opacity: 0.2 }">
              <component :is="s.icon" />
            </div>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Quick Actions -->
    <a-row :gutter="[16, 16]" style="margin-bottom: 16px;">
      <a-col :xs="24" :lg="12">
        <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
            <h3 style="font-size: 16px; font-weight: 700; margin: 0">Hóa đơn mới</h3>
            <a-button type="primary" size="small">
              <template #icon><PlusOutlined /></template>
              Tạo
            </a-button>
          </div>
          <div style="display: grid; gap: 8px;">
            <div v-for="i in 3" :key="i" style="display: flex; justify-content: space-between; padding: 8px; background: #f5f5f5; border-radius: 6px;">
              <span>SV: Trần Văn A - Phòng 101</span>
              <span style="font-weight: 600;">500,000₫</span>
            </div>
          </div>
        </a-card>
      </a-col>

      <a-col :xs="24" :lg="12">
        <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
          <div style="margin-bottom: 16px;">
            <h3 style="font-size: 16px; font-weight: 700; margin: 0">Yêu cầu sửa chữa</h3>
          </div>
          <div style="display: grid; gap: 8px;">
            <a-button 
              v-for="i in 3" 
              :key="i"
              type="text" 
              block
              style="text-align: left; justify-content: flex-start; color: #000"
            >
              <div style="display: flex; justify-content: space-between; width: 100%;">
                <span>Phòng {{ 100 + i }} - Cửa bị hỏng</span>
                <a-tag color="orange">Chờ xử lý</a-tag>
              </div>
            </a-button>
          </div>
        </a-card>
      </a-col>
    </a-row>

    <!-- Recent Transactions -->
    <a-row :gutter="[16, 16]">
      <a-col :xs="24">
        <a-card :bordered="false" style="border-radius: 12px; box-shadow: 0 1px 2px rgba(0,0,0,0.05)">
          <div style="margin-bottom: 16px;">
            <h3 style="font-size: 16px; font-weight: 700; margin: 0">Thanh toán gần đây</h3>
          </div>
          <a-table 
            :columns="transactionColumns"
            :dataSource="recentPayments"
            :pagination="false"
            size="small"
          />
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import {
  DollarOutlined,
  FileTextOutlined,
  ClockCircleOutlined,
  ToolOutlined,
  PlusOutlined,
} from '@ant-design/icons-vue'
import PageHeaderAnt from '@/components/common/PageHeaderAnt.vue'

const stats = ref([
  { label: 'Hóa đơn tháng này', value: '12', color: '#1890ff', icon: FileTextOutlined },
  { label: 'Chưa thanh toán', value: '8', color: '#ff6b00', icon: ClockCircleOutlined },
  { label: 'Doanh thu', value: '15,2M', color: '#52c41a', icon: DollarOutlined },
  { label: 'Yêu cầu sửa chữa', value: '5', color: '#faad14', icon: ToolOutlined },
])

const transactionColumns = [
  { title: 'Sinh viên', dataIndex: 'student', key: 'student', width: 150 },
  { title: 'Số tiền', dataIndex: 'amount', key: 'amount', align: 'right', width: 120 },
  { title: 'Ngày', dataIndex: 'date', key: 'date', align: 'center', width: 120 },
  { title: 'Phương thức', dataIndex: 'method', key: 'method', width: 120 },
]

const recentPayments = ref([
  { key: 1, student: 'Trần Văn A', amount: '500,000₫', date: '05/06/2026', method: 'Chuyển khoản' },
  { key: 2, student: 'Lê Thị B', amount: '1,000,000₫', date: '04/06/2026', method: 'Tiền mặt' },
  { key: 3, student: 'Phạm Văn C', amount: '500,000₫', date: '03/06/2026', method: 'Chuyển khoản' },
])
</script>
