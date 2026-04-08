<template>
  <div class="container">
    <div class="page-header">
      <h1 class="page-title">Active Auctions</h1>
      <p class="page-subtitle">Browse vehicles currently available for bidding.</p>
    </div>

    <div v-if="auctionStore.loading" class="loading-state">
      <div class="spinner"></div>
      <span>Loading auctions...</span>
    </div>

    <div v-else-if="auctionStore.error" class="error-state">
      {{ auctionStore.error }}
    </div>

    <div v-else-if="auctionStore.auctions.length === 0" class="empty-state">
      <p>No active auctions at this time.</p>
    </div>

    <div v-else class="auctions-grid">
      <AuctionCard
        v-for="auction in auctionStore.auctions"
        :key="auction.id"
        :auction="auction"
        @click="goToDetail"
      />
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuctionStore } from '../stores/auctionStore.js'
import AuctionCard from '../components/AuctionCard.vue'

const auctionStore = useAuctionStore()
const router = useRouter()

onMounted(() => auctionStore.fetchActiveAuctions())

function goToDetail(auction) {
  router.push({ name: 'auction-detail', params: { id: auction.id } })
}
</script>

<style scoped>
.page-header { margin-bottom: 2rem; }
.page-title { font-size: 2rem; font-weight: 700; color: #1a1a2e; margin-bottom: 0.5rem; }
.page-subtitle { color: #6b7280; }

.loading-state {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 3rem;
  justify-content: center;
  color: #6b7280;
}

.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.error-state {
  color: #ef4444;
  padding: 2rem;
  background: #fef2f2;
  border-radius: 8px;
  text-align: center;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #9ca3af;
  background: #fff;
  border-radius: 12px;
}

.auctions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}
</style>
