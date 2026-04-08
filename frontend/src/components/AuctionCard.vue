<template>
  <div class="auction-card" @click="$emit('click', auction)">
    <div class="vehicle-info">
      <div class="vehicle-type-badge" :class="auction.vehicle.type === 'Common' ? 'badge-common' : 'badge-luxury'">
        {{ auction.vehicle.type }}
      </div>
      <h3 class="vehicle-name">{{ auction.vehicle.model }}</h3>
      <p class="vehicle-plate">{{ auction.vehicle.year }} · {{ auction.vehicle.plate }}</p>
    </div>

    <div class="auction-prices">
      <div class="price-item">
        <span class="price-label">Base Price</span>
        <span class="price-value">{{ formatCurrency(auction.basePrice) }}</span>
      </div>
      <div class="price-item">
        <span class="price-label">Current Bid</span>
        <span class="price-value highlight">{{ formatCurrency(auction.price) }}</span>
      </div>
    </div>

    <div class="auction-meta">
      <div class="status-badge" :class="auction.status === 'Active' ? 'status-active' : 'status-inactive'">
        {{ auction.status }}
      </div>
      <div class="dates">
        <span class="date-label">Ends:</span>
        <span class="date-value">{{ formatDate(auction.endDate) }}</span>
      </div>
    </div>

    <div class="card-footer">
      <span class="seller-info">Seller: {{ auction.sellerName }}</span>
      <span class="view-link">View Details →</span>
    </div>
  </div>
</template>

<script setup>
defineProps({ auction: { type: Object, required: true } })
defineEmits(['click'])

function formatCurrency(v) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(v)
}

function formatDate(d) {
  return new Intl.DateTimeFormat('en-US', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(d))
}
</script>

<style scoped>
.auction-card {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.07);
  cursor: pointer;
  border: 1px solid #e5e7eb;
  transition: box-shadow 0.2s, transform 0.2s;
}

.auction-card:hover {
  box-shadow: 0 8px 24px rgba(0,0,0,0.12);
  transform: translateY(-2px);
}

.vehicle-type-badge {
  display: inline-block;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 0.2rem 0.6rem;
  border-radius: 9999px;
  margin-bottom: 0.5rem;
}

.badge-common { background: #dbeafe; color: #1e40af; }
.badge-luxury { background: #fef3c7; color: #92400e; }

.vehicle-name { font-size: 1.1rem; font-weight: 600; color: #1a1a2e; margin-bottom: 0.25rem; }
.vehicle-plate { font-size: 0.8rem; color: #6b7280; margin-bottom: 1rem; }

.auction-prices {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 8px;
}

.price-item { display: flex; flex-direction: column; gap: 0.2rem; }
.price-label { font-size: 0.75rem; color: #9ca3af; font-weight: 500; }
.price-value { font-size: 1rem; font-weight: 600; color: #374151; }
.price-value.highlight { color: #1e40af; }

.auction-meta { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }

.status-badge {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 0.25rem 0.65rem;
  border-radius: 9999px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.status-active { background: #d1fae5; color: #065f46; }
.status-inactive { background: #f3f4f6; color: #6b7280; }

.dates { font-size: 0.8rem; color: #6b7280; }
.date-label { font-weight: 500; margin-right: 0.25rem; }

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 0.75rem;
  border-top: 1px solid #f3f4f6;
}

.seller-info { font-size: 0.8rem; color: #6b7280; }
.view-link { font-size: 0.8rem; color: #3b82f6; font-weight: 500; }
</style>
