import { contractStudentApi as api } from './api'

export const roomApplicationService = {
  getAll() {
    return api.get('/roomapplications')
  },

  getById(id) {
    return api.get(`/roomapplications/${id}`)
  },

  getByStudentId(studentId) {
    return api.get(`/roomapplications/student/${studentId}`)
  },

  getByUserId(userId) {
    return api.get(`/roomapplications/user/${userId}`)
  },

  getByStatus(status) {
    return api.get(`/roomapplications/status/${status}`)
  },

  create(data) {
    return api.post('/roomapplications', data)
  },

  update(id, data) {
    return api.put(`/roomapplications/${id}`, data)
  },

  approve(id, data) {
    return api.put(`/roomapplications/${id}/approve`, data)
  },

  reject(id, data) {
    return api.put(`/roomapplications/${id}/reject`, data)
  },

  accept(id, data) {
    return api.post(`/roomapplications/${id}/accept`, data)
  },

  delete(id) {
    return api.delete(`/roomapplications/${id}`)
  },
}
