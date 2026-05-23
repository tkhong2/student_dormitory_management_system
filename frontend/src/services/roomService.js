import api from './api'

export const roomService = {
  /**
   * Lấy tất cả phòng
   * @returns {Promise<RoomDto[]>}
   */
  getAll() {
    return api.get('/rooms')
  },

  /**
   * Lấy phòng theo ID
   * @param {string} id
   */
  getById(id) {
    return api.get(`/rooms/${id}`)
  },

  /**
   * Lấy phòng theo tòa nhà
   * @param {string} buildingId
   */
  getByBuildingId(buildingId) {
    return api.get(`/rooms/building/${buildingId}`)
  },

  /**
   * Tạo phòng mới
   * @param {CreateRoomDto} data
   */
  create(data) {
    return api.post('/rooms', data)
  },

  /**
   * Cập nhật phòng
   * @param {string} id
   * @param {CreateRoomDto} data
   */
  update(id, data) {
    return api.put(`/rooms/${id}`, data)
  },

  /**
   * Xóa phòng
   * @param {string} id
   */
  delete(id) {
    return api.delete(`/rooms/${id}`)
  },
}

/**
 * @typedef {Object} RoomDto
 * @property {string} id
 * @property {string} roomNumber
 * @property {number} floor
 * @property {string} buildingId
 * @property {string} roomTypeId
 * @property {string} status - 'Available' | 'Occupied' | 'Full' | 'Maintenance'
 * @property {number} currentOccupancy
 */

/**
 * @typedef {Object} CreateRoomDto
 * @property {string} roomNumber
 * @property {number} floor
 * @property {string} buildingId
 * @property {string} roomTypeId
 * @property {string} status
 * @property {number} currentOccupancy
 */
