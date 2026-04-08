import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

const API_URL = import.meta.env.VITE_API_URL || ''

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || null)
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))

  const isAuthenticated = computed(() => !!token.value)
  const role = computed(() => user.value?.role || null)
  const isBuyer = computed(() => role.value === 'Buyer')
  const isSeller = computed(() => role.value === 'Seller')
  const isAdmin = computed(() => role.value === 'Admin')

  async function login(email, password) {
    const response = await fetch(`${API_URL}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    })

    if (!response.ok) {
      const err = await response.json()
      throw new Error(err.detail || 'Login failed')
    }

    const data = await response.json()
    token.value = data.data.token
    user.value = data.data.user

    localStorage.setItem('token', token.value)
    localStorage.setItem('user', JSON.stringify(user.value))
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  function authHeaders() {
    return token.value ? { Authorization: `Bearer ${token.value}` } : {}
  }

  return { token, user, isAuthenticated, role, isBuyer, isSeller, isAdmin, login, logout, authHeaders }
})
