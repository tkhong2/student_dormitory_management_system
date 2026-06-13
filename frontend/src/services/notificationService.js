import { billingMaintenanceApi } from './api';

export default {
  // Get all notifications
  getAll() {
    return billingMaintenanceApi.get('/notifications');
  },

  // Get notification by ID
  getById(id) {
    return billingMaintenanceApi.get(`/notifications/${id}`);
  },

  // Get notifications by user ID
  getByUserId(userId) {
    return billingMaintenanceApi.get(`/notifications/user/${userId}`);
  },

  // Get unread notifications by user ID
  getUnreadByUserId(userId) {
    return billingMaintenanceApi.get(`/notifications/user/${userId}/unread`);
  },

  // Create new notification
  create(data) {
    return billingMaintenanceApi.post('/notifications', data);
  },

  // Update notification
  update(id, data) {
    return billingMaintenanceApi.put(`/notifications/${id}`, data);
  },

  // Mark as read
  markAsRead(id) {
    return billingMaintenanceApi.put(`/notifications/${id}/read`);
  },

  // Mark all as read for a user
  markAllAsRead(userId) {
    return billingMaintenanceApi.put(`/notifications/user/${userId}/read-all`);
  },

  // Delete notification
  delete(id) {
    return billingMaintenanceApi.delete(`/notifications/${id}`);
  },

  // Send notification to multiple users
  sendBulk(data) {
    return billingMaintenanceApi.post('/notifications/bulk', data);
  }
};
