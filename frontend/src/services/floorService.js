import api from "./api";

export const floorService = {
  /**
   * Lấy danh sách tất cả tầng
   * @returns {Promise<FloorDto[]>}
   */
  getAll() {
    return api.get("/floors");
  },

  /**
   * Lấy các tầng theo tòa nhà
   * @param {string|number} buildingId
   */
  getByBuildingId(buildingId) {
    return api.get(`/floors/building/${buildingId}`);
  },

  /**
   * Tạo tầng mới
   * @param {CreateFloorDto} data
   */
  create(data) {
    return api.post("/floors", data);
  },

  /**
   * Cập nhật tầng
   * @param {string|number} id
   * @param {CreateFloorDto} data
   */
  update(id, data) {
    return api.put(`/floors/${id}`, data);
  },

  /**
   * Xóa tầng
   * @param {string|number} id
   */
  delete(id) {
    return api.delete(`/floors/${id}`);
  },
};

/**
 * @typedef {Object} FloorDto
 * @property {number} id
 * @property {number} buildingId
 * @property {number} floorNumber
 * @property {string} label
 * @property {number} totalRooms
 * @property {string} floorPlanImageUrl
 * @property {boolean} isActive
 */

/**
 * @typedef {Object} CreateFloorDto
 * @property {number} buildingId
 * @property {number} floorNumber
 * @property {string} label
 * @property {number} totalRooms
 * @property {string} floorPlanImageUrl
 * @property {boolean} isActive
 */
