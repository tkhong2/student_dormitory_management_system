import { billingMaintenanceApi } from './api';

export default {
  getAll() {
    return billingMaintenanceApi.get('/invoices');
  },
  getById(id) {
    return billingMaintenanceApi.get(`/invoices/${id}`);
  },
  create(data) {
    return billingMaintenanceApi.post('/invoices', data);
  },
  update(id, data) {
    return billingMaintenanceApi.put(`/invoices/${id}`, data);
  },
  delete(id) {
    return billingMaintenanceApi.delete(`/invoices/${id}`);
  }
};
