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

// API Gateway base URL - all services should go through gateway
const API_GATEWAY_URL = 'http://localhost:5000/api'

const api = createApi(
  import.meta.env.VITE_API_GATEWAY_URL || API_GATEWAY_URL
)

export const contractStudentApi = createApi(
  import.meta.env.VITE_API_GATEWAY_URL || API_GATEWAY_URL
)

export const billingMaintenanceApi = createApi(
  import.meta.env.VITE_API_GATEWAY_URL || API_GATEWAY_URL
)

export default api
