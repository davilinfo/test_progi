<template>
  <div class="container">
    <section class="hero">
      <h1 class="hero-title">Car Auction Platform</h1>
      <p class="hero-subtitle">
        Browse active vehicle auctions and calculate your total purchase cost including all fees.
      </p>
      <div class="hero-actions">
        <RouterLink to="/calculator" class="btn-primary">Fee Calculator</RouterLink>
        <RouterLink to="/auctions" class="btn-secondary">View Auctions</RouterLink>
      </div>
    </section>

    <section class="features">
      <div class="feature-card">
        <div class="feature-icon">💰</div>
        <h3>Fee Calculator</h3>
        <p>Calculate the complete cost of any vehicle including buyer fee, seller fee, association and storage.</p>
        <RouterLink to="/calculator" class="feature-link">Try it →</RouterLink>
      </div>
      <div class="feature-card">
        <div class="feature-icon">🏷️</div>
        <h3>Live Auctions</h3>
        <p>Browse vehicles currently on auction and place your bids in real time.</p>
        <RouterLink to="/auctions" class="feature-link">Browse →</RouterLink>
      </div>
      <div class="feature-card">
        <div class="feature-icon">🔒</div>
        <h3>Secure Platform</h3>
        <p>JWT-authenticated access with role-based permissions for buyers and sellers.</p>
        <RouterLink to="/login" class="feature-link">Login →</RouterLink>
      </div>
    </section>
  </div>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>

<style scoped>
.hero {
  text-align: center;
  padding: 4rem 1rem;
}

.hero-title {
  font-size: 2.75rem;
  font-weight: 700;
  color: #1a1a2e;
  margin-bottom: 1rem;
  line-height: 1.2;
}

.hero-subtitle {
  font-size: 1.1rem;
  color: #6b7280;
  max-width: 520px;
  margin: 0 auto 2rem;
  line-height: 1.6;
}

.hero-actions { display: flex; gap: 1rem; justify-content: center; flex-wrap: wrap; }

.btn-primary {
  background: #3b82f6;
  color: #fff;
  padding: 0.8rem 2rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1rem;
  transition: background 0.2s;
}

.btn-primary:hover { background: #2563eb; }

.btn-secondary {
  background: #fff;
  color: #374151;
  padding: 0.8rem 2rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1rem;
  border: 1px solid #d1d5db;
  transition: background 0.2s;
}

.btn-secondary:hover { background: #f9fafb; }

.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 1.5rem;
  padding: 2rem 0 4rem;
}

.feature-card {
  background: #fff;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  border: 1px solid #e5e7eb;
}

.feature-icon { font-size: 2rem; margin-bottom: 1rem; }
.feature-card h3 { font-size: 1.1rem; font-weight: 600; margin-bottom: 0.5rem; color: #1a1a2e; }
.feature-card p { font-size: 0.9rem; color: #6b7280; line-height: 1.6; margin-bottom: 1rem; }
.feature-link { font-size: 0.875rem; color: #3b82f6; font-weight: 500; }
</style>
