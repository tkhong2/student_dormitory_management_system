import api from './api'

export const buildingService = {
  /**
   * Lấy danh sách tất cả tòa nhà
   * @returns {Promise<BuildingDto[]>}
   */
  getAll() {
    return api.get('/buildings')
  },

  /**
   * Lấy thông tin tòa nhà theo ID
   * @param {string} id
   * @returns {Promise<BuildingDto>}
   */
  getById(id) {
    return api.get(`/buildings/${id}`)
  },

  /**
   * Tạo tòa nhà mới
   * @param {CreateBuildingDto} data
   * @returns {Promise<BuildingDto>}
   */
  create(data) {
    return api.post('/buildings', data)
  },

  /**
   * Cập nhật tòa nhà
   * @param {string} id
   * @param {CreateBuildingDto} data
   */
  update(id, data) {
    return api.put(`/buildings/${id}`, data)
  },

  /**
   * Xóa tòa nhà
   * @param {string} id
   */
  delete(id) {
    return api.delete(`/buildings/${id}`)
  },
}

/**
 * @typedef {Object} BuildingDto
 * @property {number} id
 * @property {string} name
 * @property {string} gender - 'Male' | 'Female' | 'Mixed'
 * @property {number} totalFloors
 * @property {number} totalRooms
 * @property {string} address
 * @property {string} description
 * @property {string} managerName
 * @property {string} managerPhone
 * @property {string} constructionYear
 * @property {string} status - 'Active' | 'UnderMaintenance' | 'Closed'
 * @property {boolean} hasElevator
 * @property {boolean} hasParking
 * @property {boolean} hasLaundry
 * @property {string} thumbnailUrl
 */

/**
 * @typedef {Object} CreateBuildingDto
 * @property {string} name
 * @property {string} gender
 * @property {number} totalFloors
 * @property {number} totalRooms
 * @property {string} address
 * @property {string} description
 * @property {string} managerName
 * @property {string} managerPhone
 * @property {string} constructionYear
 * @property {string} status
 * @property {boolean} hasElevator
 * @property {boolean} hasParking
 * @property {boolean} hasLaundry
 * @property {boolean} isActive
 * @property {string} thumbnailUrl
 */
