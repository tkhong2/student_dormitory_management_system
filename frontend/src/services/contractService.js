import { contractStudentApi as api } from './api'

export const contractService = {
  getAll() {
    return api.get('/contracts')
  },

  getById(id) {
    return api.get(`/contracts/${id}`)
  },

  getByStudentId(studentId) {
    return api.get(`/contracts/student/${studentId}`)
  },

  getByUserId(userId) {
    return api.get(`/contracts/user/${userId}`)
  },

  getByStatus(status) {
    return api.get(`/contracts/status/${status}`)
  },

  getByContractCode(contractCode) {
    return api.get(`/contracts/code/${contractCode}`)
  },

  create(data) {
    return api.post('/contracts', data)
  },

  update(id, data) {
    return api.put(`/contracts/${id}`, data)
  },

  activate(id) {
    return api.put(`/contracts/${id}/activate`)
  },

  acceptContract(id, data) {
    return api.post(`/contracts/${id}/accept`, data)
  },

  terminate(id, data) {
    return api.put(`/contracts/${id}/terminate`, data)
  },

  delete(id) {
    return api.delete(`/contracts/${id}`)
  },
}
