import { billingApi as api } from './api';

export default {
  getAll() {
    return api.get('/payments');
  },
  getById(id) {
    return api.get(`/payments/${id}`);
  },
  create(data) {
    return api.post('/payments', data);
  },
  update(id, data) {
    return api.put(`/payments/${id}`, data);
  },
  delete(id) {
    return api.delete(`/payments/${id}`);
  }
};
