import api from './api'

export const amenityService = {
  /**
   * Lấy tất cả tiện nghi
   * @returns {Promise<AmenityDto[]>}
   */
  getAll() {
    return api.get('/amenities')
  },

  /**
   * Lấy tiện nghi đang hoạt động
   * @returns {Promise<AmenityDto[]>}
   */
  getActives() {
    return api.get('/amenities/active')
  },

  /**
   * Lấy tiện nghi theo ID
   * @param {number} id
   * @returns {Promise<AmenityDto>}
   */
  getById(id) {
    return api.get(`/amenities/${id}`)
  },

  /**
   * Tạo tiện nghi mới
   * @param {CreateAmenityDto} data
   * @returns {Promise<AmenityDto>}
   */
  create(data) {
    return api.post('/amenities', data)
  },

  /**
   * Cập nhật tiện nghi
   * @param {number} id
   * @param {CreateAmenityDto} data
   */
  update(id, data) {
    return api.put(`/amenities/${id}`, data)
  },

  /**
   * Xóa tiện nghi
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/amenities/${id}`)
  },
}

/**
 * @typedef {Object} AmenityDto
 * @property {number} id
 * @property {string} name
 * @property {string} category
 * @property {string} iconUrl
 * @property {boolean} isActive
 */

/**
 * @typedef {Object} CreateAmenityDto
 * @property {string} name
 * @property {string} category
 * @property {string} iconUrl
 * @property {boolean} isActive
 */
