import { billingMaintenanceApi } from './api';

export default {
  // Invoice CRUD
  getAll() {
    return billingMaintenanceApi.get('/invoices');
  },
  getById(id) {
    return billingMaintenanceApi.get(`/invoices/${id}`);
  },
  getByStudentId(studentId) {
    return billingMaintenanceApi.get(`/invoices/student/${studentId}`);
  },
  getByContractId(contractId) {
    return billingMaintenanceApi.get(`/invoices/contract/${contractId}`);
  },
  getByInvoiceCode(invoiceCode) {
    return billingMaintenanceApi.get(`/invoices/code/${invoiceCode}`);
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
