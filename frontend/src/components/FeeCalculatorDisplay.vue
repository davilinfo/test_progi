<template>
  <div>
    <div v-if="feeStore.loading" class="loading-inline">
      <div class="spinner-sm"></div> Calculating...
    </div>

    <div v-else-if="feeStore.result" class="fee-list">
      <div class="fee-row" v-for="item in feeItems" :key="item.label">
        <span class="fee-lbl">{{ item.label }}</span>
        <span class="fee-amt">{{ formatCurrency(item.value) }}</span>
      </div>
      <div class="total-row">
        <span>Total</span>
        <span>{{ formatCurrency(feeStore.result.total) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { watch, computed, onMounted, onUnmounted } from 'vue'
import { useFeeStore } from '../stores/feeStore.js'

const props = defineProps({
  basePrice: { type: Number, required: true },
  vehicleType: { type: String, required: true }
})

const feeStore = useFeeStore()

const feeItems = computed(() => feeStore.result ? [
  { label: 'Base Price', value: feeStore.result.basePrice },
  { label: 'Basic Buyer Fee', value: feeStore.result.basicBuyerFee },
  { label: 'Special Seller Fee', value: feeStore.result.specialSellerFee },
  { label: 'Association Fee', value: feeStore.result.associationFee },
  { label: 'Storage Fee', value: feeStore.result.storageFee }
] : [])

watch(() => [props.basePrice, props.vehicleType], ([price, type]) => {
  if (price > 0) feeStore.calculateFees(price, type)
}, { immediate: true })

onMounted(() => {
  if (props.basePrice > 0) feeStore.calculateFees(props.basePrice, props.vehicleType)
})

onUnmounted(() => feeStore.reset())

function formatCurrency(v) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(v)
}
</script>

<style scoped>
.loading-inline {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: #9ca3af;
}

.spinner-sm {
  width: 14px; height: 14px;
  border: 2px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.fee-list { display: flex; flex-direction: column; gap: 0.5rem; }

.fee-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.875rem;
  padding: 0.35rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.fee-lbl { color: #6b7280; }
.fee-amt { font-weight: 500; color: #374151; }

.total-row {
  display: flex;
  justify-content: space-between;
  font-size: 1rem;
  font-weight: 700;
  color: #1e40af;
  padding: 0.75rem;
  background: #eff6ff;
  border-radius: 6px;
  margin-top: 0.5rem;
}
</style>
