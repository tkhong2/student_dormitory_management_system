import axios from 'axios'

// API Gateway base URL already includes /api
const API_GATEWAY_BASE = import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5000/api'

export const fileService = {
  /**
   * Upload a file to the server
   * @param {File} file - The file to upload
   * @param {string} folder - The folder to upload to (default: 'documents')
   * @returns {Promise<{fileUrl: string, fileName: string, fileType: string, fileSizeBytes: number}>}
   */
  async uploadFile(file, folder = 'documents') {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('folder', folder)

    const token = localStorage.getItem('token')
    const uploadUrl = `${API_GATEWAY_BASE}/contract-files/upload`
    
    console.log('Uploading to:', uploadUrl)
    
    const response = await axios.post(uploadUrl, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
        'Authorization': token ? `Bearer ${token}` : ''
      }
    })

    return response.data
  },

  /**
   * Get file URL for viewing/downloading
   * @param {string} fileUrl - Relative file URL from database
   * @returns {string} - Full URL to access the file
   */
  getFileUrl(fileUrl) {
    if (!fileUrl) return ''
    if (fileUrl.startsWith('http')) return fileUrl
    // Convert /uploads/... to /api/contract-uploads/... for gateway routing
    const gatewayPath = fileUrl.replace('/uploads/', '/api/contract-uploads/')
    // Remove /api from base URL to avoid duplication, then add the path
    const baseWithoutApi = API_GATEWAY_BASE.replace('/api', '')
    const fullUrl = `${baseWithoutApi}${gatewayPath}`
    console.log('File URL:', fileUrl, '→', fullUrl)
    return fullUrl
  }
}
