import api from "./api";

export const studentService = {
  /**
   * Get all students
   */
  getAll() {
    return api.get("/students");
  },

  /**
   * Get by ID
   */
  getById(id) {
    return api.get(`/students/${id}`);
  },

  /**
   * Get by user ID
   */
  getByUserId(userId) {
    return api.get(`/students/user/${userId}`);
  },

  /**
   * Create new student
   */
  create(data) {
    return api.post("/students", data);
  },

  /**
   * Update student
   */
  update(id, data) {
    return api.put(`/students/${id}`, data);
  },

  /**
   * Delete student
   */
  delete(id) {
    return api.delete(`/students/${id}`);
  },
};
