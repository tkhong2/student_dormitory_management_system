import api from './api'

const maintenanceService = {
  async getAll() {
    const response = await api.get('/maintenance-requests')
    return response.data
  },

  async getById(id) {
    const response = await api.get(`/maintenance-requests/${id}`)
    return response.data
  },

  async create(data) {
    const response = await api.post('/maintenance-requests', data)
    return response.data
  },

  async update(id, data) {
    const response = await api.put(`/maintenance-requests/${id}`, data)
    return response.data
  },

  async assign(id, data) {
    const response = await api.post(`/maintenance-requests/${id}/assign`, data)
    return response.data
  },

  async start(id, data) {
    const response = await api.post(`/maintenance-requests/${id}/start`, data)
    return response.data
  },

  async resolve(id, data) {
    const response = await api.post(`/maintenance-requests/${id}/resolve`, data)
    return response.data
  },

  async delete(id) {
    const response = await api.delete(`/maintenance-requests/${id}`)
    return response.data
  }
}

export default maintenanceService
