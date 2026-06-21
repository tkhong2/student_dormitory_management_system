import axios from 'axios'

// Public API - KHÔNG cần authentication
const API_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

// Create axios instance WITHOUT auth interceptor
const publicApi = axios.create({
  baseURL: API_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Response interceptor để xử lý lỗi
publicApi.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const message = error.response?.data?.message || 'Lỗi kết nối máy chủ'
    return Promise.reject({ message, status: error.response?.status })
  }
)

// Get all utilities (public - no auth needed)
export const getAllUtilities = async () => {
  try {
    const response = await publicApi.get('/api/sharedutilities')
    return response
  } catch (error) {
    console.error('Error fetching utilities:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

// Get utilities by building (public)
export const getUtilitiesByBuilding = async (buildingId) => {
  try {
    const response = await publicApi.get(`/api/sharedutilities/building/${buildingId}`)
    return response
  } catch (error) {
    console.error('Error fetching utilities by building:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

// Get utilities by category (public)
export const getUtilitiesByCategory = async (category) => {
  try {
    const response = await publicApi.get(`/api/sharedutilities/category/${category}`)
    return response
  } catch (error) {
    console.error('Error fetching utilities by category:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

// Get all buildings (public)
export const getAllBuildings = async () => {
  try {
    const response = await publicApi.get('/api/buildings')
    return response
  } catch (error) {
    console.error('Error fetching buildings:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

// Utility Categories (same as sharedUtilityService)
export const UTILITY_CATEGORIES = [
  { value: 'WashingMachine', label: 'Máy giặt', icon: 'fa-tshirt', faIcon: 'fas fa-tshirt' },
  { value: 'Dryer', label: 'Máy sấy', icon: 'fa-wind', faIcon: 'fas fa-wind' },
  { value: 'Gym', label: 'Phòng Gym', icon: 'fa-dumbbell', faIcon: 'fas fa-dumbbell' },
  { value: 'StudyRoom', label: 'Phòng học', icon: 'fa-book-reader', faIcon: 'fas fa-book-reader' },
  { value: 'Kitchen', label: 'Bếp chung', icon: 'fa-utensils', faIcon: 'fas fa-utensils' },
  { value: 'LaundryRoom', label: 'Phòng giặt', icon: 'fa-soap', faIcon: 'fas fa-soap' },
  { value: 'RecreationRoom', label: 'Phòng giải trí', icon: 'fa-gamepad', faIcon: 'fas fa-gamepad' },
  { value: 'MeetingRoom', label: 'Phòng họp', icon: 'fa-users', faIcon: 'fas fa-users' },
  { value: 'Other', label: 'Khác', icon: 'fa-box', faIcon: 'fas fa-box' }
]

export const UTILITY_STATUSES = [
  { value: 'Available', label: 'Sẵn sàng', color: 'green' },
  { value: 'InUse', label: 'Đang sử dụng', color: 'blue' },
  { value: 'Broken', label: 'Hỏng', color: 'red' },
  { value: 'UnderMaintenance', label: 'Đang bảo trì', color: 'orange' },
  { value: 'Retired', label: 'Ngừng sử dụng', color: 'gray' }
]
