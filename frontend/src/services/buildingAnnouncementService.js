import api from './api'

export const buildingAnnouncementService = {
  /**
   * Lấy tất cả thông báo
   * @returns {Promise<BuildingAnnouncementDto[]>}
   */
  getAll() {
    return api.get('/buildingannouncements')
  },

  /**
   * Lấy thông báo đã đăng
   * @returns {Promise<BuildingAnnouncementDto[]>}
   */
  getPublished() {
    return api.get('/buildingannouncements/published')
  },

  /**
   * Lấy thông báo theo tòa nhà
   * @param {number} buildingId
   * @returns {Promise<BuildingAnnouncementDto[]>}
   */
  getByBuildingId(buildingId) {
    return api.get(`/buildingannouncements/building/${buildingId}`)
  },

  /**
   * Lấy thông báo theo ID
   * @param {number} id
   * @returns {Promise<BuildingAnnouncementDto>}
   */
  getById(id) {
    return api.get(`/buildingannouncements/${id}`)
  },

  /**
   * Tạo thông báo mới
   * @param {CreateBuildingAnnouncementDto} data
   * @returns {Promise<BuildingAnnouncementDto>}
   */
  create(data) {
    return api.post('/buildingannouncements', data)
  },

  /**
   * Cập nhật thông báo
   * @param {number} id
   * @param {CreateBuildingAnnouncementDto} data
   */
  update(id, data) {
    return api.put(`/buildingannouncements/${id}`, data)
  },

  /**
   * Xóa thông báo
   * @param {number} id
   */
  delete(id) {
    return api.delete(`/buildingannouncements/${id}`)
  },
}

/**
 * @typedef {Object} BuildingAnnouncementDto
 * @property {number} id
 * @property {number} buildingId - nullable
 * @property {string} buildingName
 * @property {string} title
 * @property {string} content
 * @property {string} category
 * @property {string} priority
 * @property {boolean} isPinned
 * @property {string} publishedAt
 * @property {string} expiredAt
 * @property {number} createdByUserId
 * @property {string} createdByName
 * @property {string} imageUrl
 * @property {string} createdAt
 */

/**
 * @typedef {Object} CreateBuildingAnnouncementDto
 * @property {number} buildingId - nullable, null means announcement for all buildings
 * @property {string} title
 * @property {string} content
 * @property {string} category - Default: 'General'
 * @property {string} priority - Default: 'Normal'
 * @property {boolean} isPinned - Default: false
 * @property {string} publishedAt
 * @property {string} expiredAt
 * @property {number} createdByUserId
 * @property {string} createdByName
 * @property {string} imageUrl
 */
