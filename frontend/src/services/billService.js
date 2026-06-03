import { billingApi } from './api';

export default {
  getAll() {
    return billingApi.get('/bills');
  },
  getById(id) {
    return billingApi.get(`/bills/${id}`);
  },
  create(data) {
    return billingApi.post('/bills', data);
  },
  update(id, data) {
    return billingApi.put(`/bills/${id}`, data);
  },
  delete(id) {
    return billingApi.delete(`/bills/${id}`);
  }
};
