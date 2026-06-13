import { contractStudentApi as api } from './api'

export const roomTransferService = {
  getAll() {
    return api.get('/roomtransfers')
  },

  getById(id) {
    return api.get(`/roomtransfers/${id}`)
  },

  getByContractId(contractId) {
    return api.get(`/roomtransfers/contract/${contractId}`)
  },

  getByStudentId(studentId) {
    return api.get(`/roomtransfers/student/${studentId}`)
  },

  getByStatus(status) {
    return api.get(`/roomtransfers/status/${status}`)
  },

  create(data) {
    return api.post('/roomtransfers', data)
  },

  update(id, data) {
    return api.put(`/roomtransfers/${id}`, data)
  },

  approve(id, data) {
    return api.put(`/roomtransfers/${id}/approve`, data)
  },

  reject(id, data) {
    return api.put(`/roomtransfers/${id}/reject`, data)
  },

  delete(id) {
    return api.delete(`/roomtransfers/${id}`)
  },
}
