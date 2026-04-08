import { defineStore } from 'pinia'
import { ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL || ''

export const useFeeStore = defineStore('fee', () => {
  const result = ref(null)
  const loading = ref(false)
  const error = ref(null)

  async function calculateFees(basePrice, vehicleType) {
    if (!basePrice || basePrice <= 0) {
      result.value = null
      return
    }

    loading.value = true
    error.value = null

    try {
      const params = new URLSearchParams({ basePrice, vehicleType })
      const response = await fetch(`${API_URL}/api/fees/calculate?${params}`)

      if (!response.ok) {
        const err = await response.json()
        throw new Error(err.detail || 'Failed to calculate fees')
      }

      const data = await response.json()
      result.value = data.data
    } catch (err) {
      error.value = err.message
      result.value = null
    } finally {
      loading.value = false
    }
  }

  function reset() {
    result.value = null
    error.value = null
  }

  return { result, loading, error, calculateFees, reset }
})
