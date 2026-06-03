import axios from 'axios'

function createApi(baseURL) {
  const api = axios.create({
    baseURL,
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
    },
  })

  api.interceptors.request.use(
    (config) => config,
    (error) => Promise.reject(error)
  )

  api.interceptors.response.use(
    (response) => response.data,
    (error) => {
      const message = error.response?.data?.message || 'Lỗi kết nối máy chủ'
      return Promise.reject({ message, status: error.response?.status })
    }
  )

  return api
}

// Hardcoded API URLs for development (fallback if env vars not loaded)
const ROOM_BUILDING_API = 'http://localhost:5001/api'
const CONTRACT_STUDENT_API = 'http://localhost:5003/api'
const BILLING_MAINTENANCE_API = 'http://localhost:5002/api'

const api = createApi(
  import.meta.env.VITE_ROOM_BUILDING_API_URL || ROOM_BUILDING_API
)

export const contractStudentApi = createApi(
  import.meta.env.VITE_CONTRACT_STUDENT_API_URL ||
    import.meta.env.VITE_CONTRACT_API_URL ||
    CONTRACT_STUDENT_API
)

export const billingMaintenanceApi = createApi(
  import.meta.env.VITE_BILLING_MAINTENANCE_API_URL ||
    BILLING_MAINTENANCE_API
)

export default api
