import { billingMaintenanceApi } from './api';

export default {
  // Payment CRUD
  getAll() {
    return billingMaintenanceApi.get('/payments');
  },
  getById(id) {
    return billingMaintenanceApi.get(`/payments/${id}`);
  },
  getByInvoiceId(invoiceId) {
    return billingMaintenanceApi.get(`/payments/invoice/${invoiceId}`);
  },
  create(data) {
    return billingMaintenanceApi.post('/payments', data);
  },
  update(id, data) {
    return billingMaintenanceApi.put(`/payments/${id}`, data);
  },
  delete(id) {
    return billingMaintenanceApi.delete(`/payments/${id}`);
  }
};
