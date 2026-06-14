import jsPDF from 'jspdf'
import html2canvas from 'html2canvas'

// Load font for Vietnamese support (you can add custom font later)
// For now, we'll use default font with UTF-8 encoding

/**
 * Convert HTML element to PDF
 * @param {HTMLElement} element - DOM element to convert
 * @param {String} filename - PDF filename (without .pdf)
 * @param {Object} options - PDF options
 */
export async function htmlToPDF(element, filename = 'document', options = {}) {
  const {
    orientation = 'portrait', // 'portrait' or 'landscape'
    format = 'a4',
    margin = 10,
    scale = 2,
    quality = 0.95
  } = options

  try {
    // Capture element as canvas
    const canvas = await html2canvas(element, {
      scale: scale,
      useCORS: true,
      logging: false,
      backgroundColor: '#ffffff'
    })

    const imgData = canvas.toDataURL('image/png', quality)
    
    // Calculate dimensions
    const pdf = new jsPDF({
      orientation: orientation,
      unit: 'mm',
      format: format
    })

    const pageWidth = pdf.internal.pageSize.getWidth()
    const pageHeight = pdf.internal.pageSize.getHeight()
    
    const imgWidth = pageWidth - (margin * 2)
    const imgHeight = (canvas.height * imgWidth) / canvas.width
    
    let heightLeft = imgHeight
    let position = margin
    
    // Add first page
    pdf.addImage(imgData, 'PNG', margin, position, imgWidth, imgHeight)
    heightLeft -= (pageHeight - margin * 2)
    
    // Add more pages if content is longer
    while (heightLeft > 0) {
      position = heightLeft - imgHeight + margin
      pdf.addPage()
      pdf.addImage(imgData, 'PNG', margin, position, imgWidth, imgHeight)
      heightLeft -= (pageHeight - margin * 2)
    }
    
    // Save PDF
    pdf.save(`${filename}.pdf`)
    
    return true
  } catch (error) {
    console.error('Error generating PDF:', error)
    throw new Error('Không thể tạo file PDF')
  }
}

/**
 * Create invoice PDF with custom template
 * @param {Object} invoice - Invoice data
 * @param {String} filename - PDF filename
 */
export async function createInvoicePDF(invoice, filename = 'phieu-thu') {
  // Create temporary container
  const container = document.createElement('div')
  container.style.position = 'absolute'
  container.style.left = '-9999px'
  container.style.width = '210mm' // A4 width
  container.style.padding = '20mm'
  container.style.backgroundColor = '#ffffff'
  container.style.fontFamily = 'Arial, sans-serif'
  
  // Invoice HTML template
  container.innerHTML = `
    <div style="width: 100%; font-size: 14px; line-height: 1.6; color: #000;">
      <!-- Header -->
      <div style="text-align: center; margin-bottom: 30px; border-bottom: 3px solid #1890ff; padding-bottom: 20px;">
        <h1 style="margin: 0; font-size: 28px; color: #1890ff;">PHIẾU THU</h1>
        <p style="margin: 5px 0; font-size: 16px; color: #666;">Ký túc xá - Đại học ABC</p>
        <p style="margin: 5px 0; font-size: 12px; color: #999;">Địa chỉ: 123 Đường ABC, Quận XYZ, TP.HCM</p>
      </div>

      <!-- Invoice Info -->
      <div style="margin-bottom: 25px;">
        <table style="width: 100%; border-collapse: collapse;">
          <tr>
            <td style="padding: 8px 0; width: 50%;"><strong>Mã phiếu thu:</strong> ${invoice.invoiceCode || 'N/A'}</td>
            <td style="padding: 8px 0; text-align: right;"><strong>Ngày lập:</strong> ${formatDate(invoice.createdAt)}</td>
          </tr>
          <tr>
            <td colspan="2" style="padding: 8px 0;"><strong>Sinh viên:</strong> ${invoice.studentName} (${invoice.studentCode})</td>
          </tr>
          <tr>
            <td style="padding: 8px 0;"><strong>Phòng:</strong> ${invoice.roomNumber} - ${invoice.buildingName}</td>
            <td style="padding: 8px 0; text-align: right;"><strong>Kỳ thanh toán:</strong> ${invoice.billingMonth}/${invoice.billingYear}</td>
          </tr>
          <tr>
            <td colspan="2" style="padding: 8px 0;"><strong>Hạn thanh toán:</strong> ${formatDate(invoice.dueDate)}</td>
          </tr>
        </table>
      </div>

      <!-- Invoice Items -->
      <div style="margin-bottom: 25px;">
        <table style="width: 100%; border-collapse: collapse; border: 1px solid #ddd;">
          <thead>
            <tr style="background-color: #f5f5f5;">
              <th style="padding: 12px; text-align: left; border: 1px solid #ddd;">Khoản thu</th>
              <th style="padding: 12px; text-align: right; border: 1px solid #ddd; width: 120px;">Số tiền (VNĐ)</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td style="padding: 10px; border: 1px solid #ddd;">Tiền phòng</td>
              <td style="padding: 10px; text-align: right; border: 1px solid #ddd;">${formatCurrency(invoice.roomCharge || 0)}</td>
            </tr>
            <tr>
              <td style="padding: 10px; border: 1px solid #ddd;">Tiền điện (${invoice.electricityUsage || 0} kWh × ${formatCurrency(invoice.electricityUnitPrice || 0)})</td>
              <td style="padding: 10px; text-align: right; border: 1px solid #ddd;">${formatCurrency(invoice.electricityCharge || 0)}</td>
            </tr>
            <tr>
              <td style="padding: 10px; border: 1px solid #ddd;">Tiền nước (${invoice.waterUsage || 0} m³ × ${formatCurrency(invoice.waterUnitPrice || 0)})</td>
              <td style="padding: 10px; text-align: right; border: 1px solid #ddd;">${formatCurrency(invoice.waterCharge || 0)}</td>
            </tr>
            ${invoice.otherCharges ? `
            <tr>
              <td style="padding: 10px; border: 1px solid #ddd;">Phí khác</td>
              <td style="padding: 10px; text-align: right; border: 1px solid #ddd;">${formatCurrency(invoice.otherCharges)}</td>
            </tr>
            ` : ''}
            <tr style="background-color: #f5f5f5; font-weight: bold; font-size: 16px;">
              <td style="padding: 12px; border: 1px solid #ddd;">TỔNG CỘNG</td>
              <td style="padding: 12px; text-align: right; border: 1px solid #ddd; color: #1890ff;">${formatCurrency(invoice.totalAmount || 0)}</td>
            </tr>
            ${invoice.paidAmount > 0 ? `
            <tr>
              <td style="padding: 10px; border: 1px solid #ddd;">Đã thanh toán</td>
              <td style="padding: 10px; text-align: right; border: 1px solid #ddd; color: #52c41a;">-${formatCurrency(invoice.paidAmount)}</td>
            </tr>
            <tr style="font-weight: bold;">
              <td style="padding: 12px; border: 1px solid #ddd;">CÒN NỢ</td>
              <td style="padding: 12px; text-align: right; border: 1px solid #ddd; color: #ff4d4f;">${formatCurrency(invoice.debtAmount || 0)}</td>
            </tr>
            ` : ''}
          </tbody>
        </table>
      </div>

      <!-- Notes -->
      ${invoice.notes ? `
      <div style="margin-bottom: 25px; padding: 15px; background-color: #fffbe6; border-left: 4px solid #faad14;">
        <strong>Ghi chú:</strong> ${invoice.notes}
      </div>
      ` : ''}

      <!-- Footer -->
      <div style="margin-top: 50px;">
        <table style="width: 100%;">
          <tr>
            <td style="width: 50%; text-align: center;">
              <p style="margin-bottom: 80px;"><strong>Người nộp tiền</strong></p>
              <p style="margin: 0;">........................................</p>
              <p style="margin: 5px 0; font-size: 12px; color: #999;">(Ký và ghi rõ họ tên)</p>
            </td>
            <td style="width: 50%; text-align: center;">
              <p style="margin-bottom: 80px;"><strong>Người thu tiền</strong></p>
              <p style="margin: 0;">........................................</p>
              <p style="margin: 5px 0; font-size: 12px; color: #999;">(Ký và ghi rõ họ tên)</p>
            </td>
          </tr>
        </table>
      </div>

      <!-- Bottom note -->
      <div style="margin-top: 30px; text-align: center; font-size: 11px; color: #999; border-top: 1px solid #ddd; padding-top: 15px;">
        <p style="margin: 0;">Liên hệ: 0123.456.789 | Email: ktx@university.edu.vn</p>
        <p style="margin: 5px 0;">Phiếu thu này được tạo tự động từ hệ thống quản lý KTX</p>
      </div>
    </div>
  `
  
  document.body.appendChild(container)
  
  try {
    await htmlToPDF(container, filename, {
      orientation: 'portrait',
      format: 'a4',
      margin: 0,
      scale: 2
    })
    
    return true
  } finally {
    document.body.removeChild(container)
  }
}

/**
 * Create contract PDF with custom template
 * @param {Object} contract - Contract data
 * @param {String} filename - PDF filename
 */
export async function createContractPDF(contract, filename = 'hop-dong') {
  const container = document.createElement('div')
  container.style.position = 'absolute'
  container.style.left = '-9999px'
  container.style.width = '210mm'
  container.style.padding = '20mm'
  container.style.backgroundColor = '#ffffff'
  container.style.fontFamily = 'Arial, sans-serif'
  
  container.innerHTML = `
    <div style="width: 100%; font-size: 13px; line-height: 1.8; color: #000;">
      <!-- Header -->
      <div style="text-align: center; margin-bottom: 30px;">
        <h2 style="margin: 0; font-size: 24px; color: #1890ff; text-transform: uppercase;">Hợp đồng thuê phòng ký túc xá</h2>
        <p style="margin: 10px 0; font-size: 14px;">Mã hợp đồng: <strong>${contract.code || contract.contractCode}</strong></p>
        <p style="margin: 5px 0; font-size: 12px; color: #666;">Ngày lập: ${formatDate(contract.createdAt)}</p>
      </div>

      <!-- Parties -->
      <div style="margin-bottom: 25px;">
        <p style="margin-bottom: 15px; text-align: center; font-weight: bold; font-size: 14px;">CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</p>
        <p style="margin-bottom: 25px; text-align: center; font-weight: bold;">Độc lập - Tự do - Hạnh phúc</p>
        
        <h3 style="margin: 20px 0 15px 0; font-size: 16px;">BÊN CHO THUÊ (Bên A):</h3>
        <p style="margin: 5px 0; padding-left: 20px;">Tên: <strong>Ký túc xá - Đại học ABC</strong></p>
        <p style="margin: 5px 0; padding-left: 20px;">Địa chỉ: 123 Đường ABC, Quận XYZ, TP.HCM</p>
        <p style="margin: 5px 0; padding-left: 20px;">Điện thoại: 0123.456.789</p>

        <h3 style="margin: 20px 0 15px 0; font-size: 16px;">BÊN THUÊ (Bên B):</h3>
        <p style="margin: 5px 0; padding-left: 20px;">Họ tên: <strong>${contract.studentName}</strong></p>
        <p style="margin: 5px 0; padding-left: 20px;">Mã sinh viên: ${contract.studentCode}</p>
        <p style="margin: 5px 0; padding-left: 20px;">Phòng: <strong>${contract.roomNumber}</strong> - ${contract.buildingName}</p>
        <p style="margin: 5px 0; padding-left: 20px;">Loại phòng: ${contract.roomTypeName || 'N/A'}</p>
      </div>

      <!-- Terms -->
      <div style="margin-bottom: 25px;">
        <h3 style="margin: 20px 0 15px 0; font-size: 16px;">ĐIỀU KHOẢN HỢP ĐỒNG:</h3>
        
        <div style="margin-bottom: 15px;">
          <p style="margin: 0; font-weight: bold;">Điều 1: Thời hạn hợp đồng</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Ngày bắt đầu: <strong>${contract.startDate}</strong></p>
          <p style="margin: 5px 0; padding-left: 20px;">- Ngày kết thúc: <strong>${contract.endDate}</strong></p>
        </div>

        <div style="margin-bottom: 15px;">
          <p style="margin: 0; font-weight: bold;">Điều 2: Giá thuê và thanh toán</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Giá thuê: <strong style="color: #1890ff;">${formatCurrency(contract.price || contract.monthlyRent)}</strong> VNĐ/tháng</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Tiền cọc: <strong>${formatCurrency(contract.depositAmount || 0)}</strong> VNĐ</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Hình thức thanh toán: Chuyển khoản hoặc tiền mặt</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Hạn thanh toán: Trước ngày 25 hàng tháng</p>
        </div>

        <div style="margin-bottom: 15px;">
          <p style="margin: 0; font-weight: bold;">Điều 3: Quyền và nghĩa vụ của Bên A</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Cung cấp phòng ở đầy đủ tiện nghi theo quy định</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Bảo đảm an ninh, an toàn cho người thuê</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Sửa chữa, bảo trì cơ sở vật chất khi có yêu cầu</p>
        </div>

        <div style="margin-bottom: 15px;">
          <p style="margin: 0; font-weight: bold;">Điều 4: Quyền và nghĩa vụ của Bên B</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Thanh toán đầy đủ, đúng hạn các khoản phí</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Giữ gìn vệ sinh, cơ sở vật chất trong phòng</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Chấp hành nội quy ký túc xá</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Không được chuyển nhượng, cho thuê lại phòng</p>
        </div>

        <div style="margin-bottom: 15px;">
          <p style="margin: 0; font-weight: bold;">Điều 5: Điều khoản chấm dứt hợp đồng</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Hợp đồng hết hạn và không gia hạn</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Bên B vi phạm nội quy nghiêm trọng</p>
          <p style="margin: 5px 0; padding-left: 20px;">- Thỏa thuận chấm dứt giữa hai bên</p>
        </div>
      </div>

      <!-- Signatures -->
      <div style="margin-top: 50px;">
        <table style="width: 100%;">
          <tr>
            <td style="width: 50%; text-align: center;">
              <p style="margin-bottom: 10px; font-weight: bold;">ĐẠI DIỆN BÊN A</p>
              <p style="margin-bottom: 100px; font-size: 11px; font-style: italic;">(Ký, ghi rõ họ tên và đóng dấu)</p>
              <p style="margin: 0;">........................................</p>
            </td>
            <td style="width: 50%; text-align: center;">
              <p style="margin-bottom: 10px; font-weight: bold;">ĐẠI DIỆN BÊN B</p>
              <p style="margin-bottom: 100px; font-size: 11px; font-style: italic;">(Ký và ghi rõ họ tên)</p>
              <p style="margin: 0;">${contract.studentName}</p>
            </td>
          </tr>
        </table>
      </div>

      <!-- Footer -->
      <div style="margin-top: 30px; text-align: center; font-size: 11px; color: #999; border-top: 1px solid #ddd; padding-top: 15px;">
        <p style="margin: 0;">Hợp đồng có hiệu lực kể từ ngày ký. Hai bên cam kết thực hiện đúng các điều khoản đã thỏa thuận.</p>
      </div>
    </div>
  `
  
  document.body.appendChild(container)
  
  try {
    await htmlToPDF(container, filename, {
      orientation: 'portrait',
      format: 'a4',
      margin: 0,
      scale: 2
    })
    
    return true
  } finally {
    document.body.removeChild(container)
  }
}

// Helper functions
function formatDate(dateString) {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return dateString
  
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

function formatCurrency(amount) {
  if (typeof amount !== 'number') return '0'
  return amount.toLocaleString('vi-VN')
}
