<template>
  <div>
    <!-- Page Header -->
    <div style="margin-bottom: 16px;">
      <h1 style="font-size: 20px; font-weight: 700; margin: 0;">Sơ đồ tầng</h1>
      <p style="font-size: 13px; color: #8c8c8c; margin: 4px 0 0 0;">Xem trực quan trạng thái phòng theo từng tầng</p>
    </div>

    <!-- Filters and Floor Map Card -->
    <a-card :bordered="false">
      <a-row :gutter="[16, 16]" align="middle" style="margin-bottom: 24px;">
        <a-col :xs="24" :sm="8">
          <a-form-item label="Tòa nhà" style="margin-bottom: 0;">
            <a-select v-model:value="building" placeholder="Chọn tòa nhà">
              <a-select-option value="Tòa A1">Tòa A1</a-select-option>
              <a-select-option value="Tòa A2">Tòa A2</a-select-option>
              <a-select-option value="Tòa B1">Tòa B1</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :xs="24" :sm="8">
          <a-form-item label="Tầng" style="margin-bottom: 0;">
            <a-radio-group v-model:value="floor" button-style="solid">
              <a-radio-button v-for="n in 5" :key="n" :value="n">T{{ n }}</a-radio-button>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :xs="24" :sm="8" style="display: flex; gap: 16px; justify-content: flex-end; flex-wrap: wrap;">
          <span v-for="l in legend" :key="l.label" style="display: flex; align-items: center; gap: 6px; font-size: 13px;">
            <span :style="{ width: '12px', height: '12px', borderRadius: '3px', background: l.color }" />
            {{ l.label }}
          </span>
        </a-col>
      </a-row>

      <div class="floor-grid">
        <div
          v-for="r in floorRooms"
          :key="r.id"
          class="floor-room"
          :style="{ background: getColor(r.status) + '18', borderColor: getColor(r.status) }"
          @click="openDetail(r)"
        >
          <div style="font-size: 16px; font-weight: 700;" :style="{ color: getColor(r.status) }">
            {{ r.number }}
          </div>
          <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
            {{ r.occ }}/{{ r.cap }}
          </div>
        </div>
      </div>
    </a-card>

    <!-- Detail Modal -->
    <a-modal
      v-model:open="detailDlg"
      :title="'Phòng ' + (selected?.number || '')"
      width="360px"
      @cancel="detailDlg = false"
      okText="Đóng"
      :cancelButtonProps="{ style: { display: 'none' } }"
    >
      <a-descriptions v-if="selected" :column="1" bordered size="small">
        <a-descriptions-item label="Tòa nhà">{{ building }}</a-descriptions-item>
        <a-descriptions-item label="Tầng">Tầng {{ floor }}</a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="getStatusTagColor(selected.status)">{{ selected.status }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Sức chứa">{{ selected.occ }}/{{ selected.cap }}</a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const building = ref('Tòa A1')
const floor = ref(1)
const detailDlg = ref(false)
const selected = ref(null)

const legend = [
  { label: 'Trống', color: '#16a34a' },
  { label: 'Đang ở', color: '#0ea5e9' },
  { label: 'Đã đầy', color: '#dc2626' },
  { label: 'Bảo trì', color: '#ea580c' },
]

const floorRooms = computed(() => {
  const statuses = ['Trống', 'Đang ở', 'Đã đầy', 'Bảo trì']
  return Array.from({ length: 10 }, (_, i) => {
    const n = `${floor.value}${String(i + 1).padStart(2, '0')}`
    const s = statuses[Math.floor(Math.random() * 3.2)]
    const cap = 4
    const occ = s === 'Trống' ? 0 : s === 'Đã đầy' ? cap : Math.floor(Math.random() * (cap - 1)) + 1
    return { id: i, number: n, status: s, occ, cap }
  })
})

const getColor = (s) => {
  return { 'Trống': '#16a34a', 'Đang ở': '#0ea5e9', 'Đã đầy': '#dc2626', 'Bảo trì': '#ea580c' }[s] || '#8c8c8c'
}

const getStatusTagColor = (s) => {
  return { 'Trống': 'green', 'Đang ở': 'blue', 'Đã đầy': 'red', 'Bảo trì': 'orange' }[s] || 'default'
}

function openDetail(room) {
  selected.value = room
  detailDlg.value = true
}
</script>

<style scoped>
.floor-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 12px;
}

.floor-room {
  aspect-ratio: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border: 2px solid;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.floor-room:hover {
  transform: scale(1.06);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.08);
}

:deep(.ant-form-item) {
  margin-bottom: 0;
}

:deep(.ant-form-item-label) {
  padding-bottom: 4px;
}
</style>
