import { billingMaintenanceApi } from './api';

export default {
  // Maintenance Request CRUD
  getAll() {
    return billingMaintenanceApi.get('/maintenancerequests');
  },
  getById(id) {
    return billingMaintenanceApi.get(`/maintenancerequests/${id}`);
  },
  getByRoomId(roomId) {
    return billingMaintenanceApi.get(`/maintenancerequests/room/${roomId}`);
  },
  getByStudentId(studentId) {
    return billingMaintenanceApi.get(`/maintenancerequests/student/${studentId}`);
  },
  create(data) {
    return billingMaintenanceApi.post('/maintenancerequests', data);
  },
  update(id, data) {
    return billingMaintenanceApi.put(`/maintenancerequests/${id}`, data);
  },
  delete(id) {
    return billingMaintenanceApi.delete(`/maintenancerequests/${id}`);
  },
  
  // Workflow actions
  assign(id, data) {
    return billingMaintenanceApi.post(`/maintenancerequests/${id}/assign`, data);
  },
  start(id) {
    return billingMaintenanceApi.post(`/maintenancerequests/${id}/start`);
  },
  resolve(id, data) {
    return billingMaintenanceApi.post(`/maintenancerequests/${id}/resolve`, data);
  },
  reject(id, data) {
    return billingMaintenanceApi.post(`/maintenancerequests/${id}/reject`, data);
  },
  rate(id, data) {
    return billingMaintenanceApi.post(`/maintenancerequests/${id}/rate`, data);
  }
};
