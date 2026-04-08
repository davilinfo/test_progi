<template>
  <nav class="navbar">
    <div class="container navbar-inner">
      <RouterLink to="/" class="logo">
        <span class="logo-icon">🚗</span>
        <span class="logo-text">CarAuction</span>
      </RouterLink>

      <div class="nav-links">
        <RouterLink to="/" class="nav-link">Home</RouterLink>
        <RouterLink to="/calculator" class="nav-link">Calculator</RouterLink>
        <RouterLink to="/auctions" class="nav-link">Auctions</RouterLink>
      </div>

      <div class="nav-auth">
        <template v-if="auth.isAuthenticated">
          <span class="nav-user">{{ auth.user?.email }}</span>
          <span class="nav-badge" :class="roleBadgeClass">{{ auth.role }}</span>
          <button class="btn-logout" @click="handleLogout">Logout</button>
        </template>
        <template v-else>
          <RouterLink to="/login" class="btn-login">Login</RouterLink>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'

const auth = useAuthStore()
const router = useRouter()

const roleBadgeClass = computed(() => ({
  'badge-admin': auth.isAdmin,
  'badge-seller': auth.isSeller,
  'badge-buyer': auth.isBuyer
}))

function handleLogout() {
  auth.logout()
  router.push('/')
}
</script>

<style scoped>
.navbar {
  background: #1a1a2e;
  color: #fff;
  height: 64px;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
}

.navbar-inner {
  display: flex;
  align-items: center;
  height: 100%;
  gap: 2rem;
}

.logo {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.25rem;
  font-weight: 700;
  color: #fff;
}

.logo-icon { font-size: 1.5rem; }

.nav-links {
  display: flex;
  gap: 1.5rem;
  flex: 1;
}

.nav-link {
  color: rgba(255,255,255,0.75);
  font-size: 0.95rem;
  font-weight: 500;
  transition: color 0.2s;
}

.nav-link:hover, .nav-link.router-link-active { color: #fff; }

.nav-auth {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.nav-user {
  font-size: 0.875rem;
  color: rgba(255,255,255,0.75);
}

.nav-badge {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 0.2rem 0.6rem;
  border-radius: 9999px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.badge-admin { background: #f59e0b; color: #1a1a2e; }
.badge-seller { background: #10b981; color: #fff; }
.badge-buyer { background: #3b82f6; color: #fff; }

.btn-logout {
  background: transparent;
  border: 1px solid rgba(255,255,255,0.4);
  color: #fff;
  padding: 0.4rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
  transition: background 0.2s;
}

.btn-logout:hover { background: rgba(255,255,255,0.1); }

.btn-login {
  background: #3b82f6;
  color: #fff;
  padding: 0.4rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;
}

.btn-login:hover { background: #2563eb; }
</style>
