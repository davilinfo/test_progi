<template>
  <div class="container">
    <div class="page-header">
      <h1 class="page-title">Fee Calculator</h1>
      <p class="page-subtitle">Enter a vehicle price and type to calculate the total purchase cost including all fees.</p>
    </div>

    <div class="calculator-layout">
      <FeeCalculator />

      <div class="fee-info-card">
        <h3 class="info-title">How Fees Are Calculated</h3>
        <div class="fee-info-section">
          <h4>Basic Buyer Fee (10%)</h4>
          <ul>
            <li>Common: min $10.00, max $50.00</li>
            <li>Luxury: min $25.00, max $200.00</li>
          </ul>
        </div>
        <div class="fee-info-section">
          <h4>Special Seller Fee</h4>
          <ul>
            <li>Common: 2% of vehicle price</li>
            <li>Luxury: 4% of vehicle price</li>
          </ul>
        </div>
        <div class="fee-info-section">
          <h4>Association Fee</h4>
          <ul>
            <li>$1 – $500: $5.00</li>
            <li>$500.01 – $1,000: $10.00</li>
            <li>$1,000.01 – $3,000: $15.00</li>
            <li>Over $3,000: $20.00</li>
          </ul>
        </div>
        <div class="fee-info-section">
          <h4>Storage Fee</h4>
          <ul>
            <li>Fixed: $100.00</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import FeeCalculator from '../components/FeeCalculator.vue'
</script>

<style scoped>
.page-header { margin-bottom: 2rem; }
.page-title { font-size: 2rem; font-weight: 700; color: #1a1a2e; margin-bottom: 0.5rem; }
.page-subtitle { color: #6b7280; font-size: 1rem; }

.calculator-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: start;
}

@media (max-width: 768px) {
  .calculator-layout { grid-template-columns: 1fr; }
}

.fee-info-card {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 16px rgba(0,0,0,0.08);
}

.info-title {
  font-size: 1rem;
  font-weight: 700;
  color: #1a1a2e;
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid #e5e7eb;
}

.fee-info-section { margin-bottom: 1.25rem; }

.fee-info-section h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.4rem;
}

.fee-info-section ul { list-style: none; }

.fee-info-section li {
  font-size: 0.825rem;
  color: #6b7280;
  padding: 0.2rem 0;
  padding-left: 1rem;
  position: relative;
}

.fee-info-section li::before {
  content: '·';
  position: absolute;
  left: 0;
  color: #9ca3af;
}
</style>
