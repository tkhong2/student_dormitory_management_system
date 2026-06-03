import { billingApi } from './api';

export default {
  getAll() {
    return billingApi.get('/maintenancerequests');
  },
  getById(id) {
    return billingApi.get(`/maintenancerequests/${id}`);
  },
  create(data) {
    return billingApi.post('/maintenancerequests', data);
  },
  update(id, data) {
    return billingApi.put(`/maintenancerequests/${id}`, data);
  },
  delete(id) {
    return billingApi.delete(`/maintenancerequests/${id}`);
  }
};
