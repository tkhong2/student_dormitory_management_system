import api from './api'

const BASE_URL = '/checkin-checkout'

export const checkInCheckOutService = {
  // Get pending check-ins
  async getPendingCheckIns() {
    return await api.get(`${BASE_URL}/pending-checkins`)
  },

  // Get pending check-outs
  async getPendingCheckOuts() {
    return await api.get(`${BASE_URL}/pending-checkouts`)
  },

  // Get all check-ins
  async getAllCheckIns() {
    return await api.get(`${BASE_URL}/checkins`)
  },

  // Get all check-outs
  async getAllCheckOuts() {
    return await api.get(`${BASE_URL}/checkouts`)
  },

  // Get check-in by contract ID
  async getCheckInByContractId(contractId) {
    return await api.get(`${BASE_URL}/checkin/contract/${contractId}`)
  },

  // Get check-out by contract ID
  async getCheckOutByContractId(contractId) {
    return await api.get(`${BASE_URL}/checkout/contract/${contractId}`)
  },

  // Create check-in
  async createCheckIn(data) {
    return await api.post(`${BASE_URL}/checkin`, data)
  },

  // Create check-out
  async createCheckOut(data) {
    return await api.post(`${BASE_URL}/checkout`, data)
  }
}
