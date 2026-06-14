import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { createInvoicePDF, createContractPDF } from '@/utils/pdfExport'

/**
 * Composable để in PDF
 * Tái sử dụng cho nhiều trang
 */
export function usePrintPDF() {
  const printing = ref(false)

  /**
   * In phiếu thu PDF
   * @param {Object} invoice - Invoice data
   * @param {String} customFilename - Custom filename (optional)
   */
  const printInvoice = async (invoice, customFilename = null) => {
    if (!invoice) {
      message.warning('Không có dữ liệu phiếu thu')
      return false
    }

    printing.value = true
    try {
      const now = new Date()
      const dateStr = `${now.getDate()}_${now.getMonth() + 1}_${now.getFullYear()}`
      const filename = customFilename || `Phieu_thu_${invoice.invoiceCode || invoice.id}_${dateStr}`

      await createInvoicePDF(invoice, filename)
      message.success('In phiếu thu thành công')
      return true
    } catch (error) {
      console.error('Print invoice error:', error)
      message.error(error.message || 'Lỗi in phiếu thu')
      return false
    } finally {
      printing.value = false
    }
  }

  /**
   * In hợp đồng PDF
   * @param {Object} contract - Contract data
   * @param {String} customFilename - Custom filename (optional)
   */
  const printContract = async (contract, customFilename = null) => {
    if (!contract) {
      message.warning('Không có dữ liệu hợp đồng')
      return false
    }

    printing.value = true
    try {
      const now = new Date()
      const dateStr = `${now.getDate()}_${now.getMonth() + 1}_${now.getFullYear()}`
      const filename = customFilename || `Hop_dong_${contract.code || contract.contractCode || contract.id}_${dateStr}`

      await createContractPDF(contract, filename)
      message.success('In hợp đồng thành công')
      return true
    } catch (error) {
      console.error('Print contract error:', error)
      message.error(error.message || 'Lỗi in hợp đồng')
      return false
    } finally {
      printing.value = false
    }
  }

  return {
    printing,
    printInvoice,
    printContract
  }
}
