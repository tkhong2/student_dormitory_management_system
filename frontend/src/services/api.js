import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5119/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor
api.interceptors.request.use(
  (config) => {
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor
api.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const message = error.response?.data?.message || 'Lỗi kết nối máy chủ'
    return Promise.reject({ message, status: error.response?.status })
  }
)

export default api
