import api from './api'

export const roomInspectionService = {
  /**
   * Lấy tất cả kiểm tra phòng
   * @returns {Promise<RoomInspectionDto[]>}
   */
  getAll() {
    return api.get('/roominspections')
  },

  /**
   * Lấy kiểm tra theo phòng
   * @param {number} roomId
   * @returns {Promise<RoomInspectionDto[]>}
   */
  getByRoomId(roomId) {
    return api.get(`/roominspections/room/${roomId}`)
  },

  /**
   * Lấy kiểm tra theo ID
   * @param {number} id
   * @returns {Promise<RoomInspectionDto>}
   */
  getById(id) {
    return api.get(`/roominspections/${id}`)
  },

  /**
   * Tạo kiểm tra mới
   * @param {CreateRoomInspectionDto} data
   * @returns {Promise<RoomInspectionDto>}
   */
  create(data) {
    return api.post('/roominspections', data)
  },

  /**
   * Cập nhật kiểm tra
   * @param {number} id
   * @param {CreateRoomInspectionDto} data
   */
  update(id, data) {
    return api.put(`/roominspections/${id}`, data)
  },

  /**
   * Xóa kiểm tra
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/roominspections/${id}`)
  },

  /**
   * Phê duyệt kiểm tra
   * @param {number} id
   * @param {Object} data
   * @param {number} data.approvedByUserId
   * @param {string} data.approvedByName
   * @param {string} data.approvalNote
   */
  approve(id, data) {
    return api.post(`/roominspections/${id}/approve`, data)
  },
}

/**
 * @typedef {Object} RoomInspectionDto
 * @property {number} id
 * @property {number} roomId
 * @property {string} roomNumber
 * @property {number} inspectorUserId
 * @property {string} inspectorName
 * @property {string} inspectionDate - DateOnly format (YYYY-MM-DD)
 * @property {string} inspectionType
 * @property {string} overallCondition
 * @property {string} electricalStatus
 * @property {string} plumbingStatus
 * @property {string} furnitureStatus
 * @property {string} wallStatus
 * @property {string} floorStatus
 * @property {number} electricityReading
 * @property {number} waterReading
 * @property {string} notes
 * @property {string} imageUrls
 * @property {string} nextInspectionDate - DateOnly format (YYYY-MM-DD)
 * @property {boolean} isApproved
 * @property {number} approvedByUserId
 * @property {string} approvedByName
 * @property {string} approvedAt
 * @property {string} approvalNote
 * @property {string} createdAt
 */

/**
 * @typedef {Object} CreateRoomInspectionDto
 * @property {number} roomId
 * @property {number} inspectorUserId
 * @property {string} inspectorName
 * @property {string} inspectionDate - DateOnly format (YYYY-MM-DD)
 * @property {string} inspectionType
 * @property {string} overallCondition
 * @property {string} electricalStatus
 * @property {string} plumbingStatus
 * @property {string} furnitureStatus
 * @property {string} wallStatus
 * @property {string} floorStatus
 * @property {number} electricityReading
 * @property {number} waterReading
 * @property {string} notes
 * @property {string} imageUrls
 * @property {string} nextInspectionDate - DateOnly format (YYYY-MM-DD)
 */
