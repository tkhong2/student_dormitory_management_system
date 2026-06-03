import { contractStudentApi as api } from './api'

export const studentDocumentService = {
  getAll() {
    return api.get('/studentdocuments')
  },

  getById(id) {
    return api.get(`/studentdocuments/${id}`)
  },

  getByStudentId(studentId) {
    return api.get(`/studentdocuments/student/${studentId}`)
  },

  create(data) {
    return api.post('/studentdocuments', data)
  },

  update(id, data) {
    return api.put(`/studentdocuments/${id}`, data)
  },

  verify(id, data) {
    return api.put(`/studentdocuments/${id}/verify`, data)
  },

  delete(id) {
    return api.delete(`/studentdocuments/${id}`)
  },
}
