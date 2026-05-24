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

const api = createApi(
  import.meta.env.VITE_ROOM_BUILDING_API_URL || 'http://localhost:5119/api'
)

export const contractStudentApi = createApi(
  import.meta.env.VITE_CONTRACT_STUDENT_API_URL ||
    import.meta.env.VITE_CONTRACT_API_URL ||
    'http://localhost:5059/api'
)

export default api
