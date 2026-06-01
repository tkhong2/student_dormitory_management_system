import api from "./api";

export const roomService = {
  /**
   * Lấy tất cả phòng
   * @returns {Promise<RoomDto[]>}
   */
  getAll() {
    return api.get("/rooms");
  },

  /**
   * Lấy phòng theo ID
   * @param {string} id
   */
  getById(id) {
    return api.get(`/rooms/${id}`);
  },

  /**
   * Lấy phòng theo tòa nhà
   * @param {string} buildingId
   */
  getByBuildingId(buildingId) {
    return api.get(`/rooms/building/${buildingId}`);
  },

  /**
   * Tạo phòng mới
   * @param {CreateRoomDto} data
   */
  create(data) {
    return api.post("/rooms", data);
  },

  /**
   * Cập nhật phòng
   * @param {string} id
   * @param {CreateRoomDto} data
   */
  update(id, data) {
    return api.put(`/rooms/${id}`, data);
  },

  /**
   * Xóa phòng
   * @param {string} id
   */
  delete(id) {
    return api.delete(`/rooms/${id}`);
  },
};

/**
 * @typedef {Object} RoomDto
 * @property {number} id
 * @property {string} roomNumber
 * @property {number} floorId
 * @property {number} roomTypeId
 * @property {number} buildingId
 * @property {number} floorNumber
 * @property {string} buildingName
 * @property {string} roomTypeName
 * @property {string} status - 'Available' | 'Full' | 'Maintenance' | 'Reserved' | 'Closed'
 * @property {number} currentOccupants
 * @property {number} maxOccupants
 * @property {string} orientation
 * @property {string} notes
 * @property {boolean} isLocked
 * @property {string} lockReason
 * @property {string} qrCode
 * @property {string} lastInspectedAt
 * @property {string} availableFrom
 */

/**
 * @typedef {Object} CreateRoomDto
 * @property {string} roomNumber
 * @property {number} floorId
 * @property {number} roomTypeId
 * @property {string} status
 * @property {number} currentOccupants
 * @property {number} maxOccupants
 * @property {string} orientation
 * @property {string} notes
 * @property {boolean} isLocked
 * @property {string} lockReason
 * @property {string} qrCode
 * @property {string} lastInspectedAt
 * @property {string} availableFrom
 */
