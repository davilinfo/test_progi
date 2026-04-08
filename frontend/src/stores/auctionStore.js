import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from './authStore.js'

const API_URL = import.meta.env.VITE_API_URL || ''

export const useAuctionStore = defineStore('auction', () => {
  const auctions = ref([])
  const currentAuction = ref(null)
  const loading = ref(false)
  const error = ref(null)

  async function fetchActiveAuctions() {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`${API_URL}/api/auctions?status=Active`)
      if (!response.ok) throw new Error('Failed to fetch auctions')
      const data = await response.json()
      auctions.value = data.data
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function fetchAuctionById(id) {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`${API_URL}/api/auctions/${id}`)
      if (!response.ok) throw new Error('Auction not found')
      const data = await response.json()
      currentAuction.value = data.data
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function placeBid(auctionId, bidAmount) {
    const auth = useAuthStore()
    const response = await fetch(`${API_URL}/api/auctions/${auctionId}/bid`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...auth.authHeaders()
      },
      body: JSON.stringify({ bidAmount })
    })

    if (!response.ok) {
      const err = await response.json()
      throw new Error(err.detail || 'Failed to place bid')
    }

    const data = await response.json()
    currentAuction.value = data.data

    const idx = auctions.value.findIndex(a => a.id === auctionId)
    if (idx !== -1) auctions.value[idx] = data.data

    return data.data
  }

  return { auctions, currentAuction, loading, error, fetchActiveAuctions, fetchAuctionById, placeBid }
})
