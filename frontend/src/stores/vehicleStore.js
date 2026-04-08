import { defineStore } from 'pinia'
import { ref } from 'vue'

const API_URL = import.meta.env.VITE_API_URL || ''

export const useVehicleStore = defineStore('vehicle', () => {
  const vehicles = ref([])
  const loading = ref(false)
  const error = ref(null)

  async function fetchVehicles() {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`${API_URL}/api/vehicles`)
      if (!response.ok) throw new Error('Failed to fetch vehicles')
      const data = await response.json()
      vehicles.value = data.data
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  return { vehicles, loading, error, fetchVehicles }
})
