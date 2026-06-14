import * as XLSX from 'xlsx'

/**
 * Xuất dữ liệu ra file Excel
 * @param {Array} data - Mảng dữ liệu cần xuất
 * @param {Object} columnMapping - Mapping giữa key và tên cột tiếng Việt
 * @param {String} filename - Tên file (không cần .xlsx)
 * @param {String} sheetName - Tên sheet
 * 
 * @example
 * exportToExcel(
 *   invoices,
 *   {
 *     invoiceCode: 'Mã hóa đơn',
 *     studentName: 'Sinh viên',
 *     totalAmount: 'Tổng tiền'
 *   },
 *   'Danh_sach_hoa_don',
 *   'Hóa đơn'
 * )
 */
export function exportToExcel(data, columnMapping, filename, sheetName = 'Sheet1') {
  if (!data || data.length === 0) {
    throw new Error('Không có dữ liệu để xuất')
  }

  // Chuyển đổi data theo columnMapping
  const mappedData = data.map(row => {
    const newRow = {}
    Object.keys(columnMapping).forEach(key => {
      const vietnameseName = columnMapping[key]
      let value = row[key]
      
      // Format date
      if (value && typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}/)) {
        value = formatDate(value)
      }
      
      // Format number (nếu là số và là tiền)
      if (typeof value === 'number' && (key.includes('Amount') || key.includes('Price') || key.includes('Cost'))) {
        value = formatCurrency(value)
      }
      
      newRow[vietnameseName] = value ?? ''
    })
    return newRow
  })

  // Tạo worksheet
  const ws = XLSX.utils.json_to_sheet(mappedData)
  
  // Auto-size columns
  const colWidths = []
  Object.keys(columnMapping).forEach((key, index) => {
    const header = columnMapping[key]
    const maxLength = Math.max(
      header.length,
      ...data.map(row => String(row[key] ?? '').length)
    )
    colWidths.push({ wch: Math.min(maxLength + 2, 50) })
  })
  ws['!cols'] = colWidths

  // Tạo workbook
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, sheetName)

  // Xuất file
  XLSX.writeFile(wb, `${filename}.xlsx`)
}

/**
 * Xuất nhiều sheet trong 1 file Excel
 * @param {Array} sheets - Array of {data, columnMapping, sheetName}
 * @param {String} filename - Tên file
 * 
 * @example
 * exportMultipleSheets([
 *   { data: invoices, columnMapping: invoiceColumns, sheetName: 'Hóa đơn' },
 *   { data: payments, columnMapping: paymentColumns, sheetName: 'Thanh toán' }
 * ], 'Bao_cao_tai_chinh')
 */
export function exportMultipleSheets(sheets, filename) {
  const wb = XLSX.utils.book_new()

  sheets.forEach(({ data, columnMapping, sheetName }) => {
    if (!data || data.length === 0) return

    const mappedData = data.map(row => {
      const newRow = {}
      Object.keys(columnMapping).forEach(key => {
        const vietnameseName = columnMapping[key]
        let value = row[key]
        
        if (value && typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}/)) {
          value = formatDate(value)
        }
        
        if (typeof value === 'number' && (key.includes('Amount') || key.includes('Price') || key.includes('Cost'))) {
          value = formatCurrency(value)
        }
        
        newRow[vietnameseName] = value ?? ''
      })
      return newRow
    })

    const ws = XLSX.utils.json_to_sheet(mappedData)
    
    // Auto-size columns
    const colWidths = []
    Object.keys(columnMapping).forEach((key) => {
      const header = columnMapping[key]
      const maxLength = Math.max(
        header.length,
        ...data.map(row => String(row[key] ?? '').length)
      )
      colWidths.push({ wch: Math.min(maxLength + 2, 50) })
    })
    ws['!cols'] = colWidths

    XLSX.utils.book_append_sheet(wb, ws, sheetName)
  })

  XLSX.writeFile(wb, `${filename}.xlsx`)
}

/**
 * Format date sang định dạng dd/MM/yyyy
 */
function formatDate(dateString) {
  if (!dateString) return ''
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return dateString
  
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

/**
 * Format currency
 */
function formatCurrency(amount) {
  if (typeof amount !== 'number') return amount
  return amount.toLocaleString('vi-VN')
}

/**
 * Xuất với style (advanced - cần thư viện xlsx-style hoặc exceljs)
 * Tạm thời chưa implement, dùng basic export trước
 */
export function exportToExcelWithStyle(data, columnMapping, filename, sheetName = 'Sheet1') {
  // TODO: Implement với exceljs nếu cần style phức tạp
  exportToExcel(data, columnMapping, filename, sheetName)
}
