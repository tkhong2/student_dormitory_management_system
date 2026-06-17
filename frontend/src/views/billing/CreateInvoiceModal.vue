<template>
  <a-modal
    :open="open"
    title="Tạo Phiếu Thu"
    width="1000px"
    @cancel="$emit('update:open', false)"
  >
    <a-form :model="form" layout="vertical">
      <a-row :gutter="16">
        <!-- Row 1: Basic Info -->
        <a-col :span="8">
          <a-form-item label="Mã Phiếu Thu">
            <a-input v-model:value="form.invoiceCode" disabled>
              <template #prefix><i class="fas fa-file-invoice" style="color: #52c41a"></i></template>
            </a-input>
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Loại Phiếu Thu" required>
            <a-select v-model:value="form.invoiceType" @change="$emit('invoiceTypeChange', form.invoiceType)">
              <a-select-option value="Monthly">Tiền phòng tháng</a-select-option>
              <a-select-option value="Deposit">Tiền cọc</a-select-option>
              <a-select-option value="DepositRefund">Hoàn cọc</a-select-option>
              <a-select-option value="Other">Khác</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Người Lập Phiếu">
            <a-input v-model:value="form.createdByName" disabled>
              <template #prefix><i class="fas fa-user-tie" style="color: #1890ff"></i></template>
            </a-input>
          </a-form-item>
        </a-col>
        
        <a-col :span="24">
          <a-form-item label="Chọn Hợp Đồng" required>
            <a-select 
              v-model:value="form.contractId" 
              placeholder="Chọn hợp đồng Active"
              show-search
              @change="$emit('contractChange', form.contractId)"
            >
              <a-select-option v-for="contract in contracts" :key="contract.id" :value="contract.id">
                <div style="display: flex; justify-content: space-between;">
                  <span><strong>{{ contract.contractCode }}</strong> - {{ contract.studentName }}</span>
                  <a-tag color="blue" size="small">{{ contract.roomNumber }}</a-tag>
                </div>
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        
        <!-- Auto-filled Info -->
        <a-col :span="24">
          <a-divider style="margin: 12px 0">
            <span style="color: #1890ff; font-weight: 500;">
              <i class="fas fa-folder-open" style="margin-right: 6px"></i>
              Thông tin hợp đồng
            </span>
          </a-divider>
        </a-col>
        
        <a-col :span="8">
          <a-form-item label="Sinh Viên">
            <a-input v-model:value="form.studentName" disabled />
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Mã SV">
            <a-input v-model:value="form.studentCode" disabled />
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Phòng - Tòa">
            <a-input :value="`${form.roomNumber} - ${form.buildingName}`" disabled />
          </a-form-item>
        </a-col>
        
        <!-- Period -->
        <a-col :span="24">
          <a-divider style="margin: 12px 0">
            <span style="color: #1890ff; font-weight: 500;">
              <i class="fas fa-calendar-alt" style="margin-right: 6px"></i>
              Kỳ thanh toán
            </span>
          </a-divider>
        </a-col>
        
        <a-col :span="8">
          <a-form-item label="Tháng" required>
            <a-input-number v-model:value="form.billingMonth" :min="1" :max="12" style="width: 100%" />
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Năm" required>
            <a-input-number v-model:value="form.billingYear" :min="2020" style="width: 100%" />
          </a-form-item>
        </a-col>
        <a-col :span="8">
          <a-form-item label="Hạn Thanh Toán" required>
            <a-date-picker v-model:value="form.dueDate" format="DD/MM/YYYY" style="width: 100%" />
          </a-form-item>
        </a-col>
        
        <!-- Billing Details -->
        <a-col :span="24">
          <a-divider style="margin: 12px 0">
            <span style="color: #1890ff; font-weight: 500;">
              <i class="fas fa-money-bill-wave" style="margin-right: 6px"></i>
              Chi tiết các khoản
            </span>
          </a-divider>
        </a-col>
        
        <!-- Monthly Invoice: Full details -->
        <template v-if="form.invoiceType === 'Monthly'">
          <a-col :span="24">
            <a-form-item label="Tiền Phòng">
              <a-input-number
                v-model:value="form.rentAmount"
                :min="0"
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
                addonAfter="đ"
              />
            </a-form-item>
          </a-col>
          
          <!-- Electricity -->
          <a-col :span="6">
            <a-form-item label="Điện (Đầu kỳ)">
              <a-input-number
                v-model:value="form.electricityStartIndex"
                :min="0"
                style="width: 100%"
                @change="calcElec"
                addonAfter="kWh"
              />
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Điện (Cuối kỳ)">
              <a-input-number
                v-model:value="form.electricityEndIndex"
                :min="0"
                style="width: 100%"
                @change="calcElec"
                addonAfter="kWh"
              />
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Đơn giá điện">
              <a-input-number
                v-model:value="form.electricityUnitPrice"
                :min="0"
                disabled
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
              >
                <template #addonAfter>đ/kWh</template>
              </a-input-number>
              <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
                <i class="fas fa-lock" style="margin-right: 4px"></i>Theo hợp đồng
              </div>
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Tiền Điện">
              <a-input-number
                v-model:value="form.electricityAmount"
                disabled
                style="width: 100%"
                :formatter="fmt"
                addonAfter="đ"
              />
            </a-form-item>
          </a-col>
          
          <!-- Water -->
          <a-col :span="6">
            <a-form-item label="Nước (Đầu kỳ)">
              <a-input-number v-model:value="form.waterStartIndex" :min="0" style="width: 100%" @change="calcWater" addonAfter="m³" />
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Nước (Cuối kỳ)">
              <a-input-number v-model:value="form.waterEndIndex" :min="0" style="width: 100%" @change="calcWater" addonAfter="m³" />
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Đơn giá nước">
              <a-input-number
                v-model:value="form.waterUnitPrice"
                :min="0"
                disabled
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
              >
                <template #addonAfter>đ/m³</template>
              </a-input-number>
              <div style="font-size: 12px; color: #8c8c8c; margin-top: 4px;">
                <i class="fas fa-lock" style="margin-right: 4px"></i>Theo hợp đồng
              </div>
            </a-form-item>
          </a-col>
          <a-col :span="6">
            <a-form-item label="Tiền Nước">
              <a-input-number v-model:value="form.waterAmount" disabled style="width: 100%" :formatter="fmt" addonAfter="đ" />
            </a-form-item>
          </a-col>
          
          <!-- Other -->
          <a-col :span="8">
            <a-form-item label="Phí Dịch Vụ">
              <a-input-number v-model:value="form.serviceAmount" :min="0" style="width: 100%" :formatter="fmt" :parser="parse" addonAfter="đ" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Nợ Kỳ Trước">
              <a-input-number v-model:value="form.previousDebt" :min="0" style="width: 100%" :formatter="fmt" :parser="parse" addonAfter="đ" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Giảm Giá">
              <a-input-number v-model:value="form.discount" :min="0" style="width: 100%" :formatter="fmt" :parser="parse" addonAfter="đ" />
            </a-form-item>
          </a-col>
          
          <a-col :span="12">
            <a-form-item label="Lý Do Giảm Giá">
              <a-input v-model:value="form.discountReason" placeholder="Ví dụ: Ưu đãi SV giỏi">
                <template #prefix><i class="fas fa-tag"></i></template>
              </a-input>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Phương Thức TT">
              <a-select v-model:value="form.paymentMethod">
                <a-select-option value="Cash">
                  <i class="fas fa-money-bill-alt"></i> Tiền mặt
                </a-select-option>
                <a-select-option value="BankTransfer">
                  <i class="fas fa-university"></i> Chuyển khoản
                </a-select-option>
                <a-select-option value="Momo">
                  <i class="fas fa-wallet"></i> Ví Momo
                </a-select-option>
                <a-select-option value="VNPay">
                  <i class="fas fa-credit-card"></i> VNPay
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
        
        <!-- Deposit Invoice: Simple amount -->
        <template v-else-if="form.invoiceType === 'Deposit'">
          <a-col :span="24">
            <a-alert 
              message="Phiếu thu tiền cọc" 
              description="Sinh viên cần đặt cọc trước khi nhận phòng. Tiền cọc sẽ được hoàn lại khi kết thúc hợp đồng." 
              type="info" 
              show-icon 
              style="margin-bottom: 16px"
            />
          </a-col>
          <a-col :span="24">
            <a-form-item label="Số Tiền Cọc" required>
              <a-input-number
                v-model:value="form.rentAmount"
                :min="0"
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
              >
                <template #addonBefore>
                  <i class="fas fa-hand-holding-usd" style="color: #52c41a"></i>
                </template>
                <template #addonAfter>VNĐ</template>
              </a-input-number>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Phương Thức TT">
              <a-select v-model:value="form.paymentMethod">
                <a-select-option value="Cash">
                  <i class="fas fa-money-bill-alt"></i> Tiền mặt
                </a-select-option>
                <a-select-option value="BankTransfer">
                  <i class="fas fa-university"></i> Chuyển khoản
                </a-select-option>
                <a-select-option value="Momo">
                  <i class="fas fa-wallet"></i> Ví Momo
                </a-select-option>
                <a-select-option value="VNPay">
                  <i class="fas fa-credit-card"></i> VNPay
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
        
        <!-- Deposit Refund: Amount with optional penalty -->
        <template v-else-if="form.invoiceType === 'DepositRefund'">
          <a-col :span="24">
            <a-alert 
              message="Phiếu hoàn trả tiền cọc" 
              description="Hoàn trả tiền cọc cho sinh viên khi kết thúc hợp đồng. Có thể trừ phí phạt nếu có vi phạm." 
              type="success" 
              show-icon 
              style="margin-bottom: 16px"
            />
          </a-col>
          <a-col :span="12">
            <a-form-item label="Số Tiền Cọc Ban Đầu" required>
              <a-input-number
                v-model:value="form.rentAmount"
                :min="0"
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
                addonAfter="đ"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Tiền Phạt (nếu có)">
              <a-input-number
                v-model:value="form.discount"
                :min="0"
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
              >
                <template #addonBefore>
                  <i class="fas fa-exclamation-triangle" style="color: #ff4d4f"></i>
                </template>
                <template #addonAfter>đ</template>
              </a-input-number>
            </a-form-item>
          </a-col>
          <a-col :span="24">
            <a-form-item label="Lý Do Trừ Tiền (nếu có)">
              <a-textarea 
                v-model:value="form.discountReason" 
                placeholder="Ví dụ: Hư hỏng thiết bị, mất chìa khóa, làm mất vệ sinh..."
                :rows="2"
              >
                <template #prefix><i class="fas fa-clipboard-list"></i></template>
              </a-textarea>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Phương Thức Hoàn">
              <a-select v-model:value="form.paymentMethod">
                <a-select-option value="Cash">
                  <i class="fas fa-money-bill-alt"></i> Tiền mặt
                </a-select-option>
                <a-select-option value="BankTransfer">
                  <i class="fas fa-university"></i> Chuyển khoản
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
        
        <!-- Other Invoice Type: Simple amount -->
        <template v-else>
          <a-col :span="24">
            <a-form-item label="Số Tiền">
              <a-input-number
                v-model:value="form.rentAmount"
                :min="0"
                style="width: 100%"
                :formatter="fmt"
                :parser="parse"
                addonAfter="đ"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Phương Thức TT">
              <a-select v-model:value="form.paymentMethod">
                <a-select-option value="Cash">
                  <i class="fas fa-money-bill-alt"></i> Tiền mặt
                </a-select-option>
                <a-select-option value="BankTransfer">
                  <i class="fas fa-university"></i> Chuyển khoản
                </a-select-option>
                <a-select-option value="Momo">
                  <i class="fas fa-wallet"></i> Ví Momo
                </a-select-option>
                <a-select-option value="VNPay">
                  <i class="fas fa-credit-card"></i> VNPay
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </template>
        
        <!-- Total -->
        <a-col :span="24">
          <a-form-item>
            <template #label>
              <span style="font-size: 16px; font-weight: 600; color: #1890ff">
                <i class="fas fa-calculator" style="margin-right: 6px"></i>
                <template v-if="form.invoiceType === 'Monthly'">TỔNG TIỀN</template>
                <template v-else-if="form.invoiceType === 'Deposit'">SỐ TIỀN CỌC</template>
                <template v-else-if="form.invoiceType === 'DepositRefund'">SỐ TIỀN HOÀN LẠI</template>
                <template v-else>TỔNG TIỀN</template>
              </span>
            </template>
            <a-input-number
              :value="total"
              disabled
              style="width: 100%; font-size: 20px; font-weight: bold; color: #1890ff"
              :formatter="fmt"
            >
              <template #addonBefore>
                <i class="fas fa-coins" style="color: #faad14"></i>
              </template>
              <template #addonAfter>
                <span style="font-weight: bold">VNĐ</span>
              </template>
            </a-input-number>
          </a-form-item>
        </a-col>
        
        <a-col :span="24">
          <a-form-item label="Ghi Chú">
            <a-textarea v-model:value="form.notes" :rows="2" placeholder="Ghi chú bổ sung (nếu có)">
              <template #prefix><i class="fas fa-sticky-note"></i></template>
            </a-textarea>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <template #footer>
      <a-button @click="$emit('update:open', false)">
        <i class="fas fa-times" style="margin-right: 4px"></i>
        Hủy
      </a-button>
      <a-button type="primary" @click="$emit('save')" :loading="saving">
        <i class="fas fa-save" style="margin-right: 4px"></i>
        Tạo Phiếu Thu
      </a-button>
    </template>
  </a-modal>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  open: Boolean,
  form: Object,
  contracts: Array,
  saving: Boolean
})

const emit = defineEmits(['update:open', 'save', 'contractChange'])

const total = computed(() => {
  const { invoiceType, rentAmount, electricityAmount, waterAmount, serviceAmount, previousDebt, discount } = props.form
  
  // For Monthly: Full calculation with all fees
  if (invoiceType === 'Monthly') {
    return (rentAmount || 0) + (electricityAmount || 0) + (waterAmount || 0) + 
           (serviceAmount || 0) + (previousDebt || 0) - (discount || 0)
  }
  
  // For Deposit: Just the deposit amount
  if (invoiceType === 'Deposit') {
    return (rentAmount || 0)
  }
  
  // For DepositRefund: Deposit amount minus penalty
  if (invoiceType === 'DepositRefund') {
    return (rentAmount || 0) - (discount || 0)
  }
  
  // For Other: Just the base amount
  return (rentAmount || 0)
})

const fmt = (value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
const parse = (value) => value.replace(/\$\s?|(,*)/g, '')

const calcElec = () => {
  const usage = (props.form.electricityEndIndex || 0) - (props.form.electricityStartIndex || 0)
  props.form.electricityAmount = usage > 0 ? usage * props.form.electricityUnitPrice : 0
}

const calcWater = () => {
  const usage = (props.form.waterEndIndex || 0) - (props.form.waterStartIndex || 0)
  props.form.waterAmount = usage > 0 ? usage * props.form.waterUnitPrice : 0
}
</script>
