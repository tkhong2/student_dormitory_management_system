<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold mb-1">Sơ đồ tầng</h1>
        <p class="text-body-2 text-medium-emphasis">Xem trực quan trạng thái phòng theo từng tầng</p>
      </div>
    </div>

    <v-card class="pa-6" style="border:1px solid #e5e7eb">
      <v-row align="center" class="mb-6">
        <v-col cols="12" sm="4">
          <v-select v-model="building" label="Tòa nhà" :items="['Tòa A1','Tòa A2','Tòa B1']" density="compact" hide-details />
        </v-col>
        <v-col cols="12" sm="4">
          <v-btn-toggle v-model="floor" color="primary" variant="outlined" divided mandatory density="compact">
            <v-btn v-for="n in 5" :key="n" :value="n" size="small">T{{ n }}</v-btn>
          </v-btn-toggle>
        </v-col>
        <v-col cols="12" sm="4" class="d-flex ga-4 justify-end flex-wrap">
          <span v-for="l in legend" :key="l.label" class="d-flex align-center ga-1 text-caption">
            <span :style="{width:'10px',height:'10px',borderRadius:'3px',background:l.color}" />{{ l.label }}
          </span>
        </v-col>
      </v-row>

      <div class="floor-grid">
        <div v-for="r in floorRooms" :key="r.id" class="floor-room" :style="{background:getColor(r.status)+'18',borderColor:getColor(r.status)}" @click="selected=r;detailDlg=true">
          <div class="text-subtitle-2 font-weight-bold" :style="{color:getColor(r.status)}">{{ r.number }}</div>
          <div class="text-caption text-medium-emphasis">{{ r.occ }}/{{ r.cap }}</div>
        </div>
      </div>
    </v-card>

    <v-dialog v-model="detailDlg" max-width="360">
      <v-card v-if="selected" class="pa-6">
        <h3 class="text-h6 font-weight-bold mb-4">Phòng {{ selected.number }}</h3>
        <v-list density="compact">
          <v-list-item title="Tòa" :subtitle="building" />
          <v-list-item title="Tầng" :subtitle="'Tầng '+floor" />
          <v-list-item title="Trạng thái" :subtitle="selected.status" />
          <v-list-item title="Sức chứa" :subtitle="selected.occ+'/'+selected.cap" />
        </v-list>
        <v-btn block color="primary" class="mt-4" @click="detailDlg=false">Đóng</v-btn>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
const building = ref('Tòa A1')
const floor = ref(1)
const detailDlg = ref(false)
const selected = ref(null)

const legend = [
  { label:'Trống', color:'#16a34a' },
  { label:'Đang ở', color:'#0ea5e9' },
  { label:'Đã đầy', color:'#dc2626' },
  { label:'Bảo trì', color:'#ea580c' },
]

const floorRooms = computed(() => {
  const statuses = ['Trống','Đang ở','Đã đầy','Bảo trì']
  return Array.from({length:10},(_,i) => {
    const n = `${floor.value}${String(i+1).padStart(2,'0')}`
    const s = statuses[Math.floor(Math.random()*3.2)]
    const cap = 4
    const occ = s==='Trống'?0:s==='Đã đầy'?cap:Math.floor(Math.random()*(cap-1))+1
    return { id:i, number:n, status:s, occ, cap }
  })
})

const getColor = s => ({ 'Trống':'#16a34a','Đang ở':'#0ea5e9','Đã đầy':'#dc2626','Bảo trì':'#ea580c' }[s])
</script>

<style scoped>
.floor-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(100px,1fr)); gap:12px; }
.floor-room {
  aspect-ratio:1; display:flex; flex-direction:column; align-items:center; justify-content:center;
  border:2px solid; border-radius:12px; cursor:pointer; transition:all .2s;
}
.floor-room:hover { transform:scale(1.06); box-shadow:0 6px 20px rgba(0,0,0,.08); }
</style>
