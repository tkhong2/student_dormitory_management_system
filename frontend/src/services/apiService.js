// API Service - Centralized API URLs configuration
// Automatically switches between development (localhost) and production URLs

const getApiUrl = (serviceName) => {
  const urls = {
    gateway: import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5000',
    billing: import.meta.env.VITE_BILLING_MAINTENANCE_API_URL || 'http://localhost:5002/api',
    contracts: import.meta.env.VITE_CONTRACT_STUDENT_API_URL || 'http://localhost:5001/api',
    rooms: import.meta.env.VITE_ROOM_BUILDING_API_URL || 'http://localhost:5003/api'
  }
  
  return urls[serviceName] || urls.gateway
}

// Export API URLs
export const API_URLS = {
  // Gateway
  GATEWAY: getApiUrl('gateway'),
  
  // Billing & Maintenance Service
  BILLING: getApiUrl('billing'),
  INVOICES: `${getApiUrl('billing')}/invoices`,
  SEPAY: `${getApiUrl('billing')}/sepay`,
  MAINTENANCE_REQUESTS: `${getApiUrl('billing')}/maintenancerequests`,
  NOTIFICATIONS: `${getApiUrl('billing')}/notifications`,
  USERS: `${getApiUrl('billing')}/users`,
  
  // Contract & Student Service
  CONTRACTS: getApiUrl('contracts'),
  STUDENTS: `${getApiUrl('contracts')}/students`,
  CONTRACT_TEMPLATES: `${getApiUrl('contracts')}/contracttemplates`,
  ROOM_APPLICATIONS: `${getApiUrl('contracts')}/roomapplications`,
  
  // Room & Building Service
  ROOMS: getApiUrl('rooms'),
  ROOM_TYPES: `${getApiUrl('rooms')}/roomtypes`,
  BUILDINGS: `${getApiUrl('rooms')}/buildings`,
  FILES: `${getApiUrl('rooms')}/files`,
  PUBLIC_FACILITIES: `${getApiUrl('rooms')}/publicfacilities`
}

// Helper function to get authorization header
export const getAuthHeaders = () => ({
  'Authorization': `Bearer ${localStorage.getItem('token')}`
})

// Helper to build full URL
export const buildUrl = (baseUrl, path) => {
  // Remove '/api' from baseUrl if it exists and path starts with it
  const cleanBase = baseUrl.replace(/\/api$/, '')
  const cleanPath = path.startsWith('/') ? path : `/${path}`
  return `${cleanBase}${cleanPath}`
}

export default {
  API_URLS,
  getAuthHeaders,
  buildUrl
}
