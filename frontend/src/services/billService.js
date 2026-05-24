import api from './api';

export default {
  getAll() {
    return api.get('/bills');
  },
  getById(id) {
    return api.get(`/bills/${id}`);
  },
  create(data) {
    return api.post('/bills', data);
  },
  update(id, data) {
    return api.put(`/bills/${id}`, data);
  },
  delete(id) {
    return api.delete(`/bills/${id}`);
  }
};
