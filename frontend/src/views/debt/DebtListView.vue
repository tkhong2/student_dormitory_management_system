<template>
  <div>
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Công nợ sinh viên</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Theo dõi các khoản nợ chưa thanh toán</p>
    </div>

    <a-card :bordered="false">
      <a-table 
        :columns="columns" 
        :data-source="debts" 
        :row-key="item => item.code"
        :pagination="{ pageSize: 10 }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'student'">
            <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0">
              <a-avatar size="32" style="background: #fff1f0; color: #ff4d4f">
                <UserOutlined />
              </a-avatar>
              <div>
                <div style="font-weight: 500">{{ record.student }}</div>
                <div style="font-size: 12px; color: #8c8c8c">{{ record.code }}</div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'total'">
            <span style="font-weight: 600; color: #ff4d4f">{{ fmt(record.total) }}</span>
          </template>
          <template v-else-if="column.key === 'months'">
            <a-tag color="error">{{ record.months }} tháng</a-tag>
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space>
              <a-button type="default" size="small">
                <template #icon><BellOutlined /></template>
                Nhắc
              </a-button>
              <a-button type="primary" size="small" style="background: #52c41a; border-color: #52c41a">
                <template #icon><DollarOutlined /></template>
                Thu
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>
    </a-card>
  </div>
</template>

<script setup>
import { UserOutlined, BellOutlined, DollarOutlined } from '@ant-design/icons-vue'

const columns = [
  { title: 'Sinh viên', key: 'student', width: 250 },
  { title: 'Phòng', dataIndex: 'room', key: 'room', align: 'center', width: 120 },
  { title: 'Tổng nợ', key: 'total', align: 'right', width: 150 },
  { title: 'Số tháng nợ', key: 'months', align: 'center', width: 120 },
  { title: 'Hạn cuối', dataIndex: 'deadline', key: 'deadline', align: 'center', width: 120 },
  { title: 'Thao tác', key: 'actions', align: 'center', width: 200 },
]

const debts = [
  { student: 'Lê Minh Cường', code: 'SV003', room: '103-A1', total: 1600000, months: 2, deadline: '20/04/2026' },
  { student: 'Ngô Thị Giang', code: 'SV007', room: '103-A2', total: 1500000, months: 1, deadline: '20/04/2026' },
  { student: 'Đặng Văn Hải', code: 'SV008', room: '—', total: 800000, months: 1, deadline: '20/05/2026' },
]

const fmt = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
</script>
