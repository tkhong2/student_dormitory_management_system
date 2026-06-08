import api from "./api";

export const paymentService = {
  /**
   * Get all payments
   */
  getAll() {
    return api.get("/payments");
  },

  /**
   * Get by ID
   */
  getById(id) {
    return api.get(`/payments/${id}`);
  },

  /**
   * Get by invoice ID
   */
  getByInvoiceId(invoiceId) {
    return api.get(`/payments/invoice/${invoiceId}`);
  },

  /**
   * Create new payment (Record payment)
   */
  create(data) {
    return api.post("/payments", data);
  },

  /**
   * Update payment
   */
  update(id, data) {
    return api.put(`/payments/${id}`, data);
  },

  /**
   * Delete payment
   */
  delete(id) {
    return api.delete(`/payments/${id}`);
  },
};
