import api from './api'

export const fileService = {
  /**
   * Upload một file ảnh
   * @param {File} file - File object từ input
   * @returns {Promise<{url: string, fileName: string, originalFileName: string, size: number}>}
   */
  async upload(file) {
    const formData = new FormData()
    formData.append('file', file)
    
    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    
    return response
  },

  /**
   * Xóa file theo URL
   * @param {string} url - URL của file cần xóa
   */
  async delete(url) {
    return api.delete('/files/delete', { params: { url } })
  }
}
