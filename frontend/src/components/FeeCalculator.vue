<template>
  <div class="calculator-card">
    <div class="form-group">
      <label for="basePrice">Vehicle Base Price ($)</label>
      <input
        id="basePrice"
        type="number"
        v-model.number="basePrice"
        @input="onInputChange"
        placeholder="Enter vehicle price"
        min="1"
        step="0.01"
        class="form-input"
      />
    </div>

    <div class="form-group">
      <label for="vehicleType">Vehicle Type</label>
      <select id="vehicleType" v-model="vehicleType" @change="onInputChange" class="form-select">
        <option value="Common">Common</option>
        <option value="Luxury">Luxury</option>
      </select>
    </div>

    <div v-if="feeStore.loading" class="loading-state">
      <div class="spinner"></div>
      <span>Calculating fees...</span>
    </div>

    <div v-else-if="feeStore.error" class="error-state">
      {{ feeStore.error }}
    </div>

    <div v-else-if="feeStore.result" class="fee-breakdown">
      <h3 class="breakdown-title">Fee Breakdown</h3>

      <div class="fee-table">
        <div class="fee-row">
          <span class="fee-label">Vehicle Base Price</span>
          <span class="fee-value">{{ formatCurrency(feeStore.result.basePrice) }}</span>
        </div>
        <div class="fee-row">
          <span class="fee-label">
            Basic Buyer Fee
            <small>(10%, {{ vehicleType === 'Common' ? 'min $10 / max $50' : 'min $25 / max $200' }})</small>
          </span>
          <span class="fee-value">{{ formatCurrency(feeStore.result.basicBuyerFee) }}</span>
        </div>
        <div class="fee-row">
          <span class="fee-label">
            Special Seller Fee
            <small>({{ vehicleType === 'Common' ? '2%' : '4%' }})</small>
          </span>
          <span class="fee-value">{{ formatCurrency(feeStore.result.specialSellerFee) }}</span>
        </div>
        <div class="fee-row">
          <span class="fee-label">Association Fee</span>
          <span class="fee-value">{{ formatCurrency(feeStore.result.associationFee) }}</span>
        </div>
        <div class="fee-row">
          <span class="fee-label">Storage Fee (fixed)</span>
          <span class="fee-value">{{ formatCurrency(feeStore.result.storageFee) }}</span>
        </div>
      </div>

      <div class="total-row">
        <span class="total-label">Total Cost</span>
        <span class="total-value">{{ formatCurrency(feeStore.result.total) }}</span>
      </div>
    </div>

    <div v-else class="empty-state">
      Enter a vehicle price to see the fee breakdown.
    </div>
  </div>
</template>

<script setup>
import { ref, onUnmounted } from 'vue'
import { useFeeStore } from '../stores/feeStore.js'

const feeStore = useFeeStore()
const basePrice = ref('')
const vehicleType = ref('Common')
let debounceTimer = null

function onInputChange() {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    feeStore.calculateFees(basePrice.value, vehicleType.value)
  }, 400)
}

function formatCurrency(value) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value)
}

onUnmounted(() => {
  clearTimeout(debounceTimer)
  feeStore.reset()
})
</script>

<style scoped>
.calculator-card {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 16px rgba(0,0,0,0.08);
  max-width: 560px;
}

.form-group {
  margin-bottom: 1.25rem;
}

label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.4rem;
}

.form-input, .form-select {
  width: 100%;
  padding: 0.65rem 0.9rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  color: #1a1a2e;
  background: #fff;
  transition: border-color 0.2s, box-shadow 0.2s;
  outline: none;
}

.form-input:focus, .form-select:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59,130,246,0.15);
}

.loading-state {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem 0;
  color: #6b7280;
}

.spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.error-state {
  color: #ef4444;
  padding: 1rem;
  background: #fef2f2;
  border-radius: 8px;
  font-size: 0.875rem;
}

.empty-state {
  color: #9ca3af;
  font-size: 0.9rem;
  padding: 1.5rem 0;
  text-align: center;
}

.fee-breakdown { margin-top: 1.5rem; }

.breakdown-title {
  font-size: 1rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.fee-table { display: flex; flex-direction: column; gap: 0.5rem; }

.fee-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.fee-label {
  font-size: 0.9rem;
  color: #374151;
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
}

.fee-label small {
  font-size: 0.75rem;
  color: #9ca3af;
  font-weight: 400;
}

.fee-value {
  font-size: 0.9rem;
  font-weight: 500;
  color: #1a1a2e;
  white-space: nowrap;
}

.total-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1rem;
  padding: 1rem;
  background: #eff6ff;
  border-radius: 8px;
  border: 1px solid #bfdbfe;
}

.total-label {
  font-size: 1rem;
  font-weight: 700;
  color: #1e40af;
}

.total-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1e40af;
}
</style>
