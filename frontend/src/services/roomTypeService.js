import api from './api'

export const roomTypeService = {
  /**
   * Lấy tất cả loại phòng
   * @returns {Promise<RoomTypeDto[]>}
   */
  getAll() {
    return api.get('/roomtypes')
  },

  /**
   * Lấy loại phòng theo ID
   * @param {string} id
   */
  getById(id) {
    return api.get(`/roomtypes/${id}`)
  },

  /**
   * Tạo loại phòng mới
   * @param {CreateRoomTypeDto} data
   */
  create(data) {
    return api.post('/roomtypes', data)
  },

  /**
   * Cập nhật loại phòng
   * @param {string} id
   * @param {CreateRoomTypeDto} data
   */
  update(id, data) {
    return api.put(`/roomtypes/${id}`, data)
  },

  /**
   * Xóa loại phòng
   * @param {string} id
   */
  delete(id) {
    return api.delete(`/roomtypes/${id}`)
  },
}

/**
 * @typedef {Object} RoomTypeDto
 * @property {number} id
 * @property {number} buildingId
 * @property {string} code
 * @property {string} name
 * @property {number} capacity
 * @property {number} pricePerMonth
 * @property {number} depositAmount
 * @property {number} electricityRate
 * @property {number} waterRate
 * @property {number} area
 * @property {string} bedType
 * @property {boolean} hasAirConditioner
 * @property {boolean} hasWaterHeater
 * @property {boolean} hasPrivateBathroom
 * @property {boolean} hasWindowView
 * @property {string} description
 * @property {string} thumbnailUrl
 * @property {boolean} isActive
 */

/**
 * @typedef {Object} CreateRoomTypeDto
 * @property {number} buildingId
 * @property {string} code
 * @property {string} name
 * @property {number} capacity
 * @property {number} pricePerMonth
 * @property {number} depositAmount
 * @property {number} electricityRate
 * @property {number} waterRate
 * @property {number} area
 * @property {string} bedType
 * @property {boolean} hasAirConditioner
 * @property {boolean} hasWaterHeater
 * @property {boolean} hasPrivateBathroom
 * @property {boolean} hasWindowView
 * @property {string} description
 * @property {string} thumbnailUrl
 * @property {boolean} isActive
 */
