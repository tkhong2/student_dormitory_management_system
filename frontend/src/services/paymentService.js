import { billingApi } from './api';

export default {
  getAll() {
    return billingApi.get('/payments');
  },
  getById(id) {
    return billingApi.get(`/payments/${id}`);
  },
  create(data) {
    return billingApi.post('/payments', data);
  },
  update(id, data) {
    return billingApi.put(`/payments/${id}`, data);
  },
  delete(id) {
    return billingApi.delete(`/payments/${id}`);
  }
};
