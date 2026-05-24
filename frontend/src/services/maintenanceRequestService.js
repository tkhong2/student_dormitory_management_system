import api from './api';

export default {
  getAll() {
    return api.get('/maintenancerequests');
  },
  getById(id) {
    return api.get(`/maintenancerequests/${id}`);
  },
  create(data) {
    return api.post('/maintenancerequests', data);
  },
  update(id, data) {
    return api.put(`/maintenancerequests/${id}`, data);
  },
  delete(id) {
    return api.delete(`/maintenancerequests/${id}`);
  }
};
