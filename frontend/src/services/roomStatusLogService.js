import api from './api'

export const roomStatusLogService = {
  /**
   * Lấy tất cả lịch sử trạng thái phòng
   * @returns {Promise<RoomStatusLogDto[]>}
   */
  getAll() {
    return api.get('/roomstatuslogs')
  },

  /**
   * Lấy lịch sử theo phòng
   * @param {number} roomId
   * @returns {Promise<RoomStatusLogDto[]>}
   */
  getByRoomId(roomId) {
    return api.get(`/roomstatuslogs/room/${roomId}`)
  },

  /**
   * Lấy lịch sử theo ID
   * @param {number} id
   * @returns {Promise<RoomStatusLogDto>}
   */
  getById(id) {
    return api.get(`/roomstatuslogs/${id}`)
  },
}

/**
 * @typedef {Object} RoomStatusLogDto
 * @property {number} id
 * @property {number} roomId
 * @property {string} roomNumber
 * @property {string} oldStatus
 * @property {string} newStatus
 * @property {string} reason
 * @property {number} changedByUserId
 * @property {string} changedByName
 * @property {string} changedAt
 */
