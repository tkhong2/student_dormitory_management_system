import api from './api'

const notificationService = {
  // Aggregate all notifications from different sources
  async getAllNotifications() {
    try {
      const notifications = []
      
      // 1. Pending room applications
      try {
        const response = await api.get('/roomapplications')
        const applications = response.data || []
        const pending = applications.filter(app => app.status === 'Pending')
        pending.forEach(app => {
          notifications.push({
            id: `app-${app.id}`,
            type: 'room_application',
            title: 'Đơn đăng ký phòng mới',
            description: `${app.studentName || 'Sinh viên'} đã đăng ký phòng`,
            time: new Date(app.submittedDate || Date.now()),
            link: '/admin/room-applications',
            unread: true,
            priority: 'normal'
          })
        })
      } catch (err) {
        console.error('Error fetching room applications:', err)
      }
      
      // 2. Pending maintenance requests
      try {
        const response = await api.get('/maintenancerequests')
        const requests = response.data || []
        const pending = requests.filter(req => req.status === 'Pending')
        pending.forEach(req => {
          notifications.push({
            id: `maint-${req.id}`,
            type: 'maintenance_request',
            title: 'Yêu cầu sửa chữa mới',
            description: `Phòng ${req.roomNumber || 'N/A'}: ${req.description || 'Yêu cầu sửa chữa'}`,
            time: new Date(req.requestDate || Date.now()),
            link: '/admin/maintenance-requests',
            unread: true,
            priority: req.priority === 'Urgent' ? 'high' : 'normal'
          })
        })
      } catch (err) {
        console.error('Error fetching maintenance requests:', err)
      }
      
      // 3. Approved contracts (recent)
      try {
        const response = await api.get('/contracts')
        const contracts = response.data || []
        const recent = contracts.filter(c => 
          c.status === 'Active' && 
          new Date(c.startDate) > new Date(Date.now() - 7 * 24 * 60 * 60 * 1000)
        )
        recent.forEach(contract => {
          notifications.push({
            id: `contract-${contract.id}`,
            type: 'contract_approved',
            title: 'Hợp đồng được chấp thuận',
            description: `Hợp đồng phòng ${contract.roomNumber || 'N/A'} đã được kích hoạt`,
            time: new Date(contract.startDate || Date.now()),
            link: '/admin/contracts',
            unread: false,
            priority: 'low'
          })
        })
      } catch (err) {
        console.error('Error fetching contracts:', err)
      }
      
      // 4. Expiring contracts (within 30 days)
      try {
        const response = await api.get('/contracts')
        const contracts = response.data || []
        const expiringSoon = contracts.filter(c => {
          if (c.status !== 'Active') return false
          const endDate = new Date(c.endDate)
          const now = new Date()
          const daysUntilExpiry = Math.floor((endDate - now) / (1000 * 60 * 60 * 24))
          return daysUntilExpiry > 0 && daysUntilExpiry <= 30
        })
        expiringSoon.forEach(contract => {
          const daysLeft = Math.floor((new Date(contract.endDate) - new Date()) / (1000 * 60 * 60 * 24))
          notifications.push({
            id: `expiring-${contract.id}`,
            type: 'contract_expiring',
            title: 'Hợp đồng sắp hết hạn',
            description: `Hợp đồng phòng ${contract.roomNumber || 'N/A'} còn ${daysLeft} ngày`,
            time: new Date(contract.endDate || Date.now()),
            link: '/admin/contracts',
            unread: true,
            priority: daysLeft <= 7 ? 'high' : 'normal'
          })
        })
      } catch (err) {
        console.error('Error fetching expiring contracts:', err)
      }
      
      // 5. Pending contract extensions
      try {
        const response = await api.get('/contractextensions')
        const extensions = response.data || []
        const pending = extensions.filter(ext => ext.status === 'Pending')
        pending.forEach(ext => {
          notifications.push({
            id: `ext-${ext.id}`,
            type: 'contract_extension',
            title: 'Yêu cầu gia hạn hợp đồng',
            description: `Yêu cầu gia hạn ${ext.extensionMonths || 0} tháng`,
            time: new Date(ext.requestDate || Date.now()),
            link: '/admin/contract-extensions',
            unread: true,
            priority: 'normal'
          })
        })
      } catch (err) {
        console.error('Error fetching contract extensions:', err)
      }
      
      // 6. Pending room transfers
      try {
        const response = await api.get('/roomtransfers')
        const transfers = response.data || []
        const pending = transfers.filter(t => t.status === 'Pending')
        pending.forEach(transfer => {
          notifications.push({
            id: `transfer-${transfer.id}`,
            type: 'room_transfer',
            title: 'Yêu cầu chuyển phòng',
            description: `Chuyển từ phòng ${transfer.fromRoomNumber || 'N/A'} sang ${transfer.toRoomNumber || 'N/A'}`,
            time: new Date(transfer.requestDate || Date.now()),
            link: '/admin/room-transfers',
            unread: true,
            priority: 'normal'
          })
        })
      } catch (err) {
        console.error('Error fetching room transfers:', err)
      }
      
      // Sort by time (newest first) and priority
      notifications.sort((a, b) => {
        // Priority order: high > normal > low
        const priorityOrder = { high: 3, normal: 2, low: 1 }
        if (priorityOrder[a.priority] !== priorityOrder[b.priority]) {
          return priorityOrder[b.priority] - priorityOrder[a.priority]
        }
        return b.time - a.time
      })
      
      return notifications
    } catch (error) {
      console.error('Error getting notifications:', error)
      return []
    }
  },
  
  // Get unread count
  async getUnreadCount() {
    try {
      const notifications = await this.getAllNotifications()
      return notifications.filter(n => n.unread).length
    } catch (error) {
      console.error('Error getting unread count:', error)
      return 0
    }
  }
}

export default notificationService
