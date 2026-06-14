import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { exportToExcel as exportUtil } from '@/utils/excelExport'

/**
 * Composable để xuất Excel
 * Tái sử dụng cho nhiều trang
 */
export function useExcelExport() {
  const exporting = ref(false)

  const exportToExcel = async (data, columnMapping, filename, sheetName = 'Sheet1') => {
    if (!data || data.length === 0) {
      message.warning('Không có dữ liệu để xuất')
      return
    }

    exporting.value = true
    try {
      const now = new Date()
      const dateStr = `${now.getDate()}_${now.getMonth() + 1}_${now.getFullYear()}`
      const fullFilename = `${filename}_${dateStr}`

      await exportUtil(data, columnMapping, fullFilename, sheetName)
      message.success('Xuất Excel thành công')
    } catch (error) {
      console.error('Export error:', error)
      message.error(error.message || 'Lỗi xuất Excel')
    } finally {
      exporting.value = false
    }
  }

  return {
    exporting,
    exportToExcel
  }
}
