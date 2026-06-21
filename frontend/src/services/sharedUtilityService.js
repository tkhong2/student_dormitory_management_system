import axios from 'axios'

const API_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

// ==================== SharedUtilities ====================

export const getAllUtilities = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/sharedutilities`)
    return response.data
  } catch (error) {
    console.error('Error fetching utilities:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilityById = async (id) => {
  try {
    const response = await axios.get(`${API_URL}/api/sharedutilities/${id}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utility:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilitiesByBuilding = async (buildingId) => {
  try {
    const response = await axios.get(`${API_URL}/api/sharedutilities/building/${buildingId}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utilities by building:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilitiesByCategory = async (category) => {
  try {
    const response = await axios.get(`${API_URL}/api/sharedutilities/category/${category}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utilities by category:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilitiesByStatus = async (status) => {
  try {
    const response = await axios.get(`${API_URL}/api/sharedutilities/status/${status}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utilities by status:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const createUtility = async (data) => {
  try {
    const response = await axios.post(`${API_URL}/api/sharedutilities`, data)
    return response.data
  } catch (error) {
    console.error('Error creating utility:', error)
    throw { 
      message: error.response?.data?.message || 'Lỗi tạo tiện ích', 
      status: error.response?.status 
    }
  }
}

export const updateUtility = async (id, data) => {
  try {
    await axios.put(`${API_URL}/api/sharedutilities/${id}`, data)
  } catch (error) {
    console.error('Error updating utility:', error)
    throw { message: 'Lỗi cập nhật tiện ích', status: error.response?.status }
  }
}

export const deleteUtility = async (id) => {
  try {
    await axios.delete(`${API_URL}/api/sharedutilities/${id}`)
  } catch (error) {
    console.error('Error deleting utility:', error)
    throw { message: 'Lỗi xóa tiện ích', status: error.response?.status }
  }
}

export const recordUtilityUsage = async (id) => {
  try {
    await axios.post(`${API_URL}/api/sharedutilities/${id}/use`)
  } catch (error) {
    console.error('Error recording utility usage:', error)
    throw { message: 'Lỗi ghi nhận sử dụng', status: error.response?.status }
  }
}

export const completeUtilityUsage = async (id) => {
  try {
    await axios.post(`${API_URL}/api/sharedutilities/${id}/complete-use`)
  } catch (error) {
    console.error('Error completing utility usage:', error)
    throw { message: 'Lỗi kết thúc sử dụng', status: error.response?.status }
  }
}

// ==================== Utility Events ====================

export const getAllUtilityEvents = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityevents`)
    return response.data
  } catch (error) {
    console.error('Error fetching utility events:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilityEventById = async (id) => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityevents/${id}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utility event:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUtilityEventsByUtility = async (utilityId) => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityevents/utility/${utilityId}`)
    return response.data
  } catch (error) {
    console.error('Error fetching utility events:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUpcomingMaintenance = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityevents/upcoming-maintenance`)
    return response.data
  } catch (error) {
    console.error('Error fetching upcoming maintenance:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const createUtilityEvent = async (data) => {
  try {
    const response = await axios.post(`${API_URL}/api/utilityevents`, data)
    return response.data
  } catch (error) {
    console.error('Error creating utility event:', error)
    throw { message: 'Lỗi tạo sự kiện', status: error.response?.status }
  }
}

export const updateUtilityEvent = async (id, data) => {
  try {
    await axios.put(`${API_URL}/api/utilityevents/${id}`, data)
  } catch (error) {
    console.error('Error updating utility event:', error)
    throw { message: 'Lỗi cập nhật sự kiện', status: error.response?.status }
  }
}

export const deleteUtilityEvent = async (id) => {
  try {
    await axios.delete(`${API_URL}/api/utilityevents/${id}`)
  } catch (error) {
    console.error('Error deleting utility event:', error)
    throw { message: 'Lỗi xóa sự kiện', status: error.response?.status }
  }
}

// ==================== Utility Usage Logs ====================

export const getAllUsageLogs = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityusagelogs`)
    return response.data
  } catch (error) {
    console.error('Error fetching usage logs:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUsageLogById = async (id) => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityusagelogs/${id}`)
    return response.data
  } catch (error) {
    console.error('Error fetching usage log:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUsageLogsByUtility = async (utilityId) => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityusagelogs/utility/${utilityId}`)
    return response.data
  } catch (error) {
    console.error('Error fetching usage logs:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUsageLogsByStudent = async (studentId) => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityusagelogs/student/${studentId}`)
    // Enrich with utility category
    const logs = response.data
    const utilitiesResponse = await axios.get(`${API_URL}/api/sharedutilities`)
    const utilities = utilitiesResponse.data
    
    return logs.map(log => {
      const utility = utilities.find(u => u.id === log.sharedUtilityId)
      return {
        ...log,
        utilityCategory: utility?.category
      }
    })
  } catch (error) {
    console.error('Error fetching usage logs:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const getUnpaidUsageLogs = async () => {
  try {
    const response = await axios.get(`${API_URL}/api/utilityusagelogs/unpaid`)
    return response.data
  } catch (error) {
    console.error('Error fetching unpaid usage logs:', error)
    throw { message: 'Lỗi kết nối máy chủ', status: error.response?.status }
  }
}

export const createUsageLog = async (data) => {
  try {
    const response = await axios.post(`${API_URL}/api/utilityusagelogs`, data)
    return response.data
  } catch (error) {
    console.error('Error creating usage log:', error)
    throw { message: 'Lỗi tạo lịch sử sử dụng', status: error.response?.status }
  }
}

export const markUsageLogAsPaid = async (id, invoiceId = null) => {
  try {
    await axios.put(`${API_URL}/api/utilityusagelogs/${id}/mark-paid?invoiceId=${invoiceId || ''}`)
  } catch (error) {
    console.error('Error marking usage log as paid:', error)
    throw { message: 'Lỗi đánh dấu đã thanh toán', status: error.response?.status }
  }
}

export const endUsageLog = async (id, endTime) => {
  try {
    await axios.put(`${API_URL}/api/utilityusagelogs/${id}/end-usage`, endTime)
  } catch (error) {
    console.error('Error ending usage log:', error)
    throw { message: 'Lỗi kết thúc sử dụng', status: error.response?.status }
  }
}

export const deleteUsageLog = async (id) => {
  try {
    await axios.delete(`${API_URL}/api/utilityusagelogs/${id}`)
  } catch (error) {
    console.error('Error deleting usage log:', error)
    throw { message: 'Lỗi xóa lịch sử sử dụng', status: error.response?.status }
  }
}

// ==================== Utility Categories ====================

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

export const EVENT_TYPES = [
  { value: 'Maintenance', label: 'Bảo trì', color: 'blue' },
  { value: 'Repair', label: 'Sửa chữa', color: 'orange' },
  { value: 'Inspection', label: 'Kiểm tra', color: 'green' },
  { value: 'Replacement', label: 'Thay thế', color: 'purple' },
  { value: 'Cleaning', label: 'Vệ sinh', color: 'cyan' }
]

export const EVENT_STATUSES = [
  { value: 'Scheduled', label: 'Đã lên lịch', color: 'blue' },
  { value: 'InProgress', label: 'Đang thực hiện', color: 'orange' },
  { value: 'Completed', label: 'Hoàn thành', color: 'green' },
  { value: 'Cancelled', label: 'Đã hủy', color: 'red' }
]
