<template>
  <div class="container">
    <div class="login-wrapper">
      <div class="login-card">
        <h1 class="login-title">Sign In</h1>
        <p class="login-subtitle">Access auctions and bid on vehicles</p>

        <form @submit.prevent="handleLogin" class="login-form">
          <div class="form-group">
            <label for="email">Email</label>
            <input
              id="email"
              type="email"
              v-model="form.email"
              placeholder="email@example.com"
              class="form-input"
              autocomplete="email"
              required
            />
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input
              id="password"
              type="password"
              v-model="form.password"
              placeholder="••••••••"
              class="form-input"
              autocomplete="current-password"
              required
            />
          </div>

          <div v-if="error" class="error-alert">{{ error }}</div>

          <button type="submit" class="btn-submit" :disabled="loading">
            <span v-if="loading" class="spinner-inline"></span>
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>

        <div class="demo-credentials">
          <p class="demo-title">Demo Credentials</p>
          <div class="cred-grid">
            <div v-for="cred in credentials" :key="cred.email" class="cred-item" @click="fillCredentials(cred)">
              <span class="cred-role" :class="`role-${cred.role.toLowerCase()}`">{{ cred.role }}</span>
              <span class="cred-email">{{ cred.email }}</span>
            </div>
          </div>
          <p class="cred-password">Password for all: <code>Password123!</code></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const loading = ref(false)
const error = ref('')
const form = reactive({ email: '', password: '' })

const credentials = [
  { email: 'admin@carauction.com', role: 'Admin' },
  { email: 'seller1@carauction.com', role: 'Seller' },
  { email: 'buyer1@carauction.com', role: 'Buyer' }
]

function fillCredentials(cred) {
  form.email = cred.email
  form.password = 'Password123!'
}

async function handleLogin() {
  loading.value = true
  error.value = ''
  try {
    await auth.login(form.email, form.password)
    const redirect = route.query.redirect || '/'
    router.push(redirect)
  } catch (err) {
    error.value = err.message || 'Login failed. Please check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding-top: 3rem;
}

.login-card {
  background: #fff;
  border-radius: 12px;
  padding: 2.5rem;
  box-shadow: 0 4px 24px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 420px;
}

.login-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1a1a2e;
  margin-bottom: 0.25rem;
}

.login-subtitle {
  color: #6b7280;
  font-size: 0.9rem;
  margin-bottom: 2rem;
}

.login-form { display: flex; flex-direction: column; gap: 1.25rem; }

.form-group { display: flex; flex-direction: column; gap: 0.4rem; }
label { font-size: 0.875rem; font-weight: 600; color: #374151; }

.form-input {
  padding: 0.65rem 0.9rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.form-input:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59,130,246,0.15);
}

.error-alert {
  background: #fef2f2;
  border: 1px solid #fecaca;
  color: #dc2626;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  font-size: 0.875rem;
}

.btn-submit {
  background: #3b82f6;
  color: #fff;
  padding: 0.75rem;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: background 0.2s;
}

.btn-submit:hover:not(:disabled) { background: #2563eb; }
.btn-submit:disabled { opacity: 0.7; cursor: not-allowed; }

.spinner-inline {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,0.4);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.demo-credentials {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.demo-title { font-size: 0.8rem; font-weight: 600; color: #9ca3af; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.75rem; }

.cred-grid { display: flex; flex-direction: column; gap: 0.4rem; margin-bottom: 0.75rem; }

.cred-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 0.75rem;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.15s;
  border: 1px solid #e5e7eb;
}

.cred-item:hover { background: #f9fafb; }

.cred-role {
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  min-width: 50px;
  text-align: center;
}

.role-admin { background: #fef3c7; color: #92400e; }
.role-seller { background: #d1fae5; color: #065f46; }
.role-buyer { background: #dbeafe; color: #1e40af; }
.cred-email { font-size: 0.85rem; color: #374151; }
.cred-password { font-size: 0.8rem; color: #6b7280; }
.cred-password code { background: #f3f4f6; padding: 0.15rem 0.4rem; border-radius: 4px; font-size: 0.85rem; }
</style>
