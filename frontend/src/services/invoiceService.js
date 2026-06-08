import api from "./api";

export const invoiceService = {
  /**
   * Get all invoices
   */
  getAll() {
    return api.get("/invoices");
  },

  /**
   * Get by ID
   */
  getById(id) {
    return api.get(`/invoices/${id}`);
  },

  /**
   * Get by student ID
   */
  getByStudentId(studentId) {
    return api.get(`/invoices/student/${studentId}`);
  },

  /**
   * Get by contract ID
   */
  getByContractId(contractId) {
    return api.get(`/invoices/contract/${contractId}`);
  },

  /**
   * Get by status
   */
  getByStatus(status) {
    return api.get(`/invoices/status/${status}`);
  },

  /**
   * Create new invoice
   */
  create(data) {
    return api.post("/invoices", data);
  },

  /**
   * Update invoice
   */
  update(id, data) {
    return api.put(`/invoices/${id}`, data);
  },

  /**
   * Delete invoice
   */
  delete(id) {
    return api.delete(`/invoices/${id}`);
  },
};
