import api from "./api";

export const roomApplicationService = {
  /**
   * Get all room applications
   */
  async getAll() {
    try {
      const response = await api.get("/roomapplications");
      return response;
    } catch (error) {
      console.warn('API error, returning mock data:', error);
      // Mock data fallback
      return [];
    }
  },

  /**
   * Get by ID
   */
  async getById(id) {
    try {
      const response = await api.get(`/roomapplications/${id}`);
      return response;
    } catch (error) {
      console.warn('API error:', error);
      return null;
    }
  },

  /**
   * Get by student ID
   */
  async getByStudentId(studentId) {
    try {
      const response = await api.get(`/roomapplications/student/${studentId}`);
      return response;
    } catch (error) {
      console.warn('API error:', error);
      return [];
    }
  },

  /**
   * Get by user ID
   */
  async getByUserId(userId) {
    try {
      const response = await api.get(`/roomapplications/user/${userId}`);
      return response;
    } catch (error) {
      console.warn('API error:', error);
      return [];
    }
  },

  /**
   * Get by status
   */
  async getByStatus(status) {
    try {
      const response = await api.get(`/roomapplications/status/${status}`);
      return response;
    } catch (error) {
      console.warn('API error:', error);
      return [];
    }
  },

  /**
   * Create new application
   */
  async create(data) {
    try {
      const response = await api.post("/roomapplications", data);
      return response;
    } catch (error) {
      console.error('Create failed:', error);
      throw error;
    }
  },

  /**
   * Update application
   */
  async update(id, data) {
    try {
      const response = await api.put(`/roomapplications/${id}`, data);
      return response;
    } catch (error) {
      console.error('Update failed:', error);
      throw error;
    }
  },

  /**
   * Approve application (Staff/Admin)
   */
  async approve(id, reviewData) {
    try {
      const response = await api.put(`/roomapplications/${id}/approve`, reviewData);
      return response;
    } catch (error) {
      console.error('Approve failed:', error);
      throw error;
    }
  },

  /**
   * Reject application (Staff/Admin)
   */
  async reject(id, reviewData) {
    try {
      const response = await api.put(`/roomapplications/${id}/reject`, reviewData);
      return response;
    } catch (error) {
      console.error('Reject failed:', error);
      throw error;
    }
  },

  /**
   * Delete application
   */
  async delete(id) {
    try {
      const response = await api.delete(`/roomapplications/${id}`);
      return response;
    } catch (error) {
      console.error('Delete failed:', error);
      throw error;
    }
  },
};
