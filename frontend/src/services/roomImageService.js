import api from './api'

export const roomImageService = {
  /**
   * Lấy ảnh theo phòng
   * @param {number} roomId
   * @returns {Promise<RoomImageDto[]>}
   */
  getByRoomId(roomId) {
    return api.get(`/roomimages/room/${roomId}`)
  },

  /**
   * Lấy ảnh theo ID
   * @param {number} id
   * @returns {Promise<RoomImageDto>}
   */
  getById(id) {
    return api.get(`/roomimages/${id}`)
  },

  /**
   * Thêm ảnh mới
   * @param {CreateRoomImageDto} data
   * @returns {Promise<RoomImageDto>}
   */
  create(data) {
    return api.post('/roomimages', data)
  },

  /**
   * Cập nhật ảnh
   * @param {number} id
   * @param {CreateRoomImageDto} data
   */
  update(id, data) {
    return api.put(`/roomimages/${id}`, data)
  },

  /**
   * Xóa ảnh
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/roomimages/${id}`)
  },
}

/**
 * @typedef {Object} RoomImageDto
 * @property {number} id
 * @property {number} roomId
 * @property {string} imageUrl
 * @property {string} caption
 * @property {boolean} isCoverImage
 * @property {number} sortOrder
 */

/**
 * @typedef {Object} CreateRoomImageDto
 * @property {number} roomId
 * @property {string} imageUrl
 * @property {string} caption
 * @property {boolean} isCoverImage
 * @property {number} sortOrder
 */
