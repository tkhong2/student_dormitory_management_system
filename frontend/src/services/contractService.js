import { contractStudentApi as api } from './api'

export const contractService = {
  getAll() {
    return api.get('/contracts')
  },

  getById(id) {
    return api.get(`/contracts/${id}`)
  },

  create(data) {
    return api.post('/contracts', data)
  },

  update(id, data) {
    return api.put(`/contracts/${id}`, data)
  },

  delete(id) {
    return api.delete(`/contracts/${id}`)
  },
}
