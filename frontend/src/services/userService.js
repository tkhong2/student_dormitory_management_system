import { billingMaintenanceApi } from './api';

export default {
  // User CRUD
  getAll() {
    return billingMaintenanceApi.get('/users');
  },
  getById(id) {
    return billingMaintenanceApi.get(`/users/${id}`);
  },
  getByUsername(username) {
    return billingMaintenanceApi.get(`/users/username/${username}`);
  },
  getByEmail(email) {
    return billingMaintenanceApi.get(`/users/email/${email}`);
  },
  create(data) {
    return billingMaintenanceApi.post('/users', data);
  },
  update(id, data) {
    return billingMaintenanceApi.put(`/users/${id}`, data);
  },
  updatePassword(id, newPassword) {
    return billingMaintenanceApi.put(`/users/${id}/password`, newPassword, {
      headers: { 'Content-Type': 'application/json' }
    });
  },
  delete(id) {
    return billingMaintenanceApi.delete(`/users/${id}`);
  }
};
