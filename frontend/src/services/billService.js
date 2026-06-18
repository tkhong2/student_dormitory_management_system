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
  },
  // Generate invoice from contract
  generateFromContract(contractId, data) {
    return billingMaintenanceApi.post(`/invoices/generate-from-contract/${contractId}`, data);
  },
  // Send reminder
  sendReminder(id) {
    return billingMaintenanceApi.post(`/invoices/${id}/send-reminder`);
  },
  // Update overdue status
  updateOverdueStatus() {
    return billingMaintenanceApi.post('/invoices/update-overdue-status');
  },
  // Get next invoice code
  getNextInvoiceCode(invoiceType, month, year) {
    return billingMaintenanceApi.get(`/invoices/next-code?invoiceType=${invoiceType}&month=${month}&year=${year}`);
  },
  // Auto-generate monthly invoices
  autoGenerateMonthly(month, year) {
    return billingMaintenanceApi.post(`/invoices/auto-generate-monthly?month=${month}&year=${year}`);
  }
};
