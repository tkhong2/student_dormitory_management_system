import api from './api'

export const roomTypeAmenityService = {
  /**
   * Lấy tất cả tiện nghi của loại phòng
   * @returns {Promise<RoomTypeAmenityDto[]>}
   */
  getAll() {
    return api.get('/roomtypeamenities')
  },

  /**
   * Lấy tiện nghi theo loại phòng
   * @param {number} roomTypeId
   * @returns {Promise<RoomTypeAmenityDto[]>}
   */
  getByRoomTypeId(roomTypeId) {
    return api.get(`/roomtypeamenities/roomtype/${roomTypeId}`)
  },

  /**
   * Lấy tiện nghi theo ID
   * @param {number} id
   * @returns {Promise<RoomTypeAmenityDto>}
   */
  getById(id) {
    return api.get(`/roomtypeamenities/${id}`)
  },

  /**
   * Thêm tiện nghi cho loại phòng
   * @param {CreateRoomTypeAmenityDto} data
   * @returns {Promise<RoomTypeAmenityDto>}
   */
  create(data) {
    return api.post('/roomtypeamenities', data)
  },

  /**
   * Cập nhật tiện nghi của loại phòng
   * @param {number} id
   * @param {CreateRoomTypeAmenityDto} data
   */
  update(id, data) {
    return api.put(`/roomtypeamenities/${id}`, data)
  },

  /**
   * Xóa tiện nghi khỏi loại phòng
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/roomtypeamenities/${id}`)
  },
}

/**
 * @typedef {Object} RoomTypeAmenityDto
 * @property {number} id
 * @property {number} roomTypeId
 * @property {number} amenityId
 * @property {string} amenityName
 * @property {string} amenityCategory
 * @property {string} amenityIconUrl
 * @property {number} quantity
 * @property {string} note
 */

/**
 * @typedef {Object} CreateRoomTypeAmenityDto
 * @property {number} roomTypeId
 * @property {number} amenityId
 * @property {number} quantity
 * @property {string} note
 */
