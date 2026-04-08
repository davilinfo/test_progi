<template>
  <div class="container">
    <div v-if="auctionStore.loading" class="loading-state">
      <div class="spinner"></div>
      <span>Loading auction...</span>
    </div>

    <div v-else-if="auctionStore.error" class="error-state">{{ auctionStore.error }}</div>

    <template v-else-if="auction">
      <div class="back-link" @click="router.push('/auctions')">← Back to Auctions</div>

      <div class="detail-layout">
        <div class="detail-main">
          <div class="vehicle-header">
            <span class="type-badge" :class="auction.vehicle.type === 'Common' ? 'badge-common' : 'badge-luxury'">
              {{ auction.vehicle.type }}
            </span>
            <h1 class="vehicle-title">{{ auction.vehicle.model }}</h1>
            <p class="vehicle-sub">{{ auction.vehicle.year }} · Plate: {{ auction.vehicle.plate }}</p>
          </div>

          <div class="price-section">
            <div class="price-box">
              <span class="price-label">Base Price</span>
              <span class="price-val">{{ formatCurrency(auction.basePrice) }}</span>
            </div>
            <div class="price-box highlight">
              <span class="price-label">Current Bid</span>
              <span class="price-val">{{ formatCurrency(auction.price) }}</span>
            </div>
          </div>

          <div class="auction-info">
            <div class="info-row">
              <span class="info-label">Status</span>
              <span class="status-badge" :class="auction.status === 'Active' ? 'status-active' : 'status-inactive'">
                {{ auction.status }}
              </span>
            </div>
            <div class="info-row">
              <span class="info-label">Seller</span>
              <span class="info-value">{{ auction.sellerName }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Start Date</span>
              <span class="info-value">{{ formatDate(auction.startDate) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">End Date</span>
              <span class="info-value">{{ formatDate(auction.endDate) }}</span>
            </div>
          </div>

          <div v-if="auction.status === 'Active' && auth.isBuyer" class="bid-section">
            <h3 class="bid-title">Place Your Bid</h3>
            <div class="bid-form">
              <input
                type="number"
                v-model.number="bidAmount"
                :min="auction.price + 1"
                step="100"
                placeholder="Enter bid amount"
                class="bid-input"
              />
              <button class="btn-bid" @click="handleBid" :disabled="bidLoading || !bidAmount">
                <span v-if="bidLoading" class="spinner-sm"></span>
                {{ bidLoading ? 'Placing Bid...' : 'Place Bid' }}
              </button>
            </div>
            <p class="bid-hint">Minimum bid: {{ formatCurrency(auction.price + 1) }}</p>
            <div v-if="bidError" class="bid-error">{{ bidError }}</div>
            <div v-if="bidSuccess" class="bid-success">Bid placed successfully!</div>
          </div>

          <div v-else-if="auction.status === 'Active' && !auth.isAuthenticated" class="auth-prompt">
            <RouterLink to="/login" class="btn-login-prompt">Login as Buyer to Place a Bid</RouterLink>
          </div>
        </div>

        <div class="fee-panel">
          <h3 class="fee-panel-title">Estimated Total Cost</h3>
          <p class="fee-panel-sub">Based on current bid price</p>
          <FeeCalculatorDisplay :base-price="auction.price" :vehicle-type="auction.vehicle.type" />
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useAuctionStore } from '../stores/auctionStore.js'
import { useAuthStore } from '../stores/authStore.js'
import FeeCalculatorDisplay from '../components/FeeCalculatorDisplay.vue'

const route = useRoute()
const router = useRouter()
const auctionStore = useAuctionStore()
const auth = useAuthStore()

const bidAmount = ref(null)
const bidLoading = ref(false)
const bidError = ref('')
const bidSuccess = ref(false)

const auction = computed(() => auctionStore.currentAuction)

onMounted(() => auctionStore.fetchAuctionById(route.params.id))

async function handleBid() {
  if (!bidAmount.value) return
  bidLoading.value = true
  bidError.value = ''
  bidSuccess.value = false
  try {
    await auctionStore.placeBid(auction.value.id, bidAmount.value)
    bidSuccess.value = true
    bidAmount.value = null
  } catch (err) {
    bidError.value = err.message
  } finally {
    bidLoading.value = false
  }
}

function formatCurrency(v) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(v)
}

function formatDate(d) {
  return new Intl.DateTimeFormat('en-US', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(d))
}
</script>

<style scoped>
.loading-state {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 3rem;
  justify-content: center;
  color: #6b7280;
}

.spinner {
  width: 24px; height: 24px;
  border: 2px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.error-state { color: #ef4444; padding: 2rem; background: #fef2f2; border-radius: 8px; }

.back-link {
  display: inline-block;
  font-size: 0.9rem;
  color: #3b82f6;
  cursor: pointer;
  margin-bottom: 1.5rem;
  font-weight: 500;
}

.detail-layout {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 2rem;
  align-items: start;
}

@media (max-width: 900px) { .detail-layout { grid-template-columns: 1fr; } }

.vehicle-header {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.type-badge {
  display: inline-block;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 0.2rem 0.6rem;
  border-radius: 9999px;
  margin-bottom: 0.75rem;
}

.badge-common { background: #dbeafe; color: #1e40af; }
.badge-luxury { background: #fef3c7; color: #92400e; }

.vehicle-title { font-size: 1.75rem; font-weight: 700; color: #1a1a2e; margin-bottom: 0.25rem; }
.vehicle-sub { color: #6b7280; font-size: 0.9rem; }

.price-section {
  display: flex;
  gap: 1.5rem;
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.price-box { display: flex; flex-direction: column; gap: 0.25rem; flex: 1; }
.price-box.highlight { padding-left: 1.5rem; border-left: 2px solid #3b82f6; }
.price-label { font-size: 0.75rem; color: #9ca3af; font-weight: 500; text-transform: uppercase; letter-spacing: 0.05em; }
.price-val { font-size: 1.5rem; font-weight: 700; color: #1a1a2e; }
.price-box.highlight .price-val { color: #1e40af; }

.auction-info {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-row { display: flex; justify-content: space-between; align-items: center; }
.info-label { font-size: 0.875rem; color: #9ca3af; font-weight: 500; }
.info-value { font-size: 0.875rem; color: #374151; font-weight: 500; }

.status-badge {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 0.25rem 0.65rem;
  border-radius: 9999px;
  text-transform: uppercase;
}

.status-active { background: #d1fae5; color: #065f46; }
.status-inactive { background: #f3f4f6; color: #6b7280; }

.bid-section {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.bid-title { font-size: 1rem; font-weight: 600; color: #1a1a2e; margin-bottom: 1rem; }

.bid-form { display: flex; gap: 0.75rem; margin-bottom: 0.5rem; }

.bid-input {
  flex: 1;
  padding: 0.65rem 0.9rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  outline: none;
  transition: border-color 0.2s;
}

.bid-input:focus { border-color: #3b82f6; }

.btn-bid {
  background: #3b82f6;
  color: #fff;
  border: none;
  padding: 0.65rem 1.5rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: background 0.2s;
}

.btn-bid:hover:not(:disabled) { background: #2563eb; }
.btn-bid:disabled { opacity: 0.6; cursor: not-allowed; }

.spinner-sm {
  width: 14px; height: 14px;
  border: 2px solid rgba(255,255,255,0.4);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

.bid-hint { font-size: 0.8rem; color: #9ca3af; }

.bid-error {
  margin-top: 0.75rem;
  padding: 0.6rem 1rem;
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
  border-radius: 6px;
  font-size: 0.875rem;
}

.bid-success {
  margin-top: 0.75rem;
  padding: 0.6rem 1rem;
  background: #d1fae5;
  color: #065f46;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
}

.auth-prompt {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  text-align: center;
}

.btn-login-prompt {
  display: inline-block;
  background: #3b82f6;
  color: #fff;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 600;
  transition: background 0.2s;
}

.btn-login-prompt:hover { background: #2563eb; }

.fee-panel {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  position: sticky;
  top: 80px;
}

.fee-panel-title { font-size: 1rem; font-weight: 700; color: #1a1a2e; margin-bottom: 0.25rem; }
.fee-panel-sub { font-size: 0.8rem; color: #9ca3af; margin-bottom: 1.5rem; }
</style>
