import { contractStudentApi as api } from './api'

export const contractExtensionService = {
  getAll() {
    return api.get('/contractextensions')
  },

  getById(id) {
    return api.get(`/contractextensions/${id}`)
  },

  getByContractId(contractId) {
    return api.get(`/contractextensions/contract/${contractId}`)
  },

  create(data) {
    return api.post('/contractextensions', data)
  },

  update(id, data) {
    return api.put(`/contractextensions/${id}`, data)
  },

  delete(id) {
    return api.delete(`/contractextensions/${id}`)
  },
}
