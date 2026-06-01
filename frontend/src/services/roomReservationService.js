import api from './api'

export const roomReservationService = {
  /**
   * Lấy tất cả đặt phòng
   * @returns {Promise<RoomReservationDto[]>}
   */
  getAll() {
    return api.get('/roomreservations')
  },

  /**
   * Lấy đặt phòng đang hoạt động
   * @returns {Promise<RoomReservationDto[]>}
   */
  getActive() {
    return api.get('/roomreservations/active')
  },

  /**
   * Lấy đặt phòng theo phòng
   * @param {number} roomId
   * @returns {Promise<RoomReservationDto[]>}
   */
  getByRoomId(roomId) {
    return api.get(`/roomreservations/room/${roomId}`)
  },

  /**
   * Lấy đặt phòng theo sinh viên
   * @param {number} studentId
   * @returns {Promise<RoomReservationDto[]>}
   */
  getByStudentId(studentId) {
    return api.get(`/roomreservations/student/${studentId}`)
  },

  /**
   * Lấy đặt phòng theo ID
   * @param {number} id
   * @returns {Promise<RoomReservationDto>}
   */
  getById(id) {
    return api.get(`/roomreservations/${id}`)
  },

  /**
   * Tạo đặt phòng mới
   * @param {CreateRoomReservationDto} data
   * @returns {Promise<RoomReservationDto>}
   */
  create(data) {
    return api.post('/roomreservations', data)
  },

  /**
   * Cập nhật đặt phòng
   * @param {number} id
   * @param {CreateRoomReservationDto} data
   */
  update(id, data) {
    return api.put(`/roomreservations/${id}`, data)
  },

  /**
   * Xóa đặt phòng
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/roomreservations/${id}`)
  },

  /**
   * Hủy đặt phòng
   * @param {number} id
   * @param {string} reason
   */
  release(id, reason) {
    return api.post(`/roomreservations/${id}/release`, { reason })
  },
}

/**
 * @typedef {Object} RoomReservationDto
 * @property {number} id
 * @property {number} roomId
 * @property {string} roomNumber
 * @property {number} applicationId
 * @property {number} studentId
 * @property {string} studentName
 * @property {string} status
 * @property {string} expiresAt
 * @property {string} note
 * @property {number} createdByUserId
 * @property {string} createdByName
 * @property {string} releasedAt
 * @property {string} releaseReason
 * @property {string} createdAt
 */

/**
 * @typedef {Object} CreateRoomReservationDto
 * @property {number} roomId
 * @property {number} applicationId
 * @property {number} studentId
 * @property {string} studentName
 * @property {string} expiresAt
 * @property {string} note
 * @property {number} createdByUserId
 * @property {string} createdByName
 */
