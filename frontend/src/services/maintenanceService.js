import axios from 'axios'

const API_URL = 'http://localhost:5002/api/maintenancerequests'

const getAuthHeaders = () => ({
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('token')}`
  }
})

export default {
  async getAll() {
    const response = await axios.get(API_URL, getAuthHeaders())
    return response.data
  },

  async getById(id) {
    const response = await axios.get(`${API_URL}/${id}`, getAuthHeaders())
    return response.data
  },

  async getByRoomId(roomId) {
    const response = await axios.get(`${API_URL}/room/${roomId}`, getAuthHeaders())
    return response.data
  },

  async getByStudentId(studentId) {
    const response = await axios.get(`${API_URL}/student/${studentId}`, getAuthHeaders())
    return response.data
  },

  async create(data) {
    const response = await axios.post(API_URL, data, getAuthHeaders())
    return response.data
  },

  async update(id, data) {
    const response = await axios.put(`${API_URL}/${id}`, data, getAuthHeaders())
    return response.data
  },

  async assign(id, data) {
    const response = await axios.post(`${API_URL}/${id}/assign`, data, getAuthHeaders())
    return response.data
  },

  async start(id) {
    const response = await axios.post(`${API_URL}/${id}/start`, {}, getAuthHeaders())
    return response.data
  },

  async resolve(id, data) {
    const response = await axios.post(`${API_URL}/${id}/resolve`, data, getAuthHeaders())
    return response.data
  },

  async reject(id, data) {
    const response = await axios.post(`${API_URL}/${id}/reject`, data, getAuthHeaders())
    return response.data
  },

  async rate(id, data) {
    const response = await axios.post(`${API_URL}/${id}/rate`, data, getAuthHeaders())
    return response.data
  },

  async delete(id) {
    const response = await axios.delete(`${API_URL}/${id}`, getAuthHeaders())
    return response.data
  }
}
