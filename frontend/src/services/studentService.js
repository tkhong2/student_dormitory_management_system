import { contractStudentApi as api } from './api'

export const studentService = {
  getAll() {
    return api.get('/students')
  },

  getById(id) {
    return api.get(`/students/${id}`)
  },

  create(data) {
    return api.post('/students', data)
  },

  update(id, data) {
    return api.put(`/students/${id}`, data)
  },

  delete(id) {
    return api.delete(`/students/${id}`)
  },
}
