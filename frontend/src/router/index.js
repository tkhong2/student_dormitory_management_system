import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  // ══ TRANG CÔNG KHAI (Mặc định) ══════════════
  {
    path: '/',
    component: () => import('../layouts/PublicLayout.vue'),
    children: [
      { path: '', name: 'home', meta: { title: 'Trang chủ' }, component: () => import('../views/public/HomePage.vue') },
      { path: 'about', name: 'about', meta: { title: 'Giới thiệu' }, component: () => import('../views/public/AboutPage.vue') },
      { path: 'rooms', name: 'room-registration', meta: { title: 'Đăng ký phòng' }, component: () => import('../views/public/RoomRegistrationPage.vue') },
      { path: 'news', name: 'news', meta: { title: 'Tin tức' }, component: () => import('../views/public/NewsPage.vue') },
      { path: 'rules', name: 'rules', meta: { title: 'Nội quy' }, component: () => import('../views/public/RulesPage.vue') },
      { path: 'contact', name: 'contact', meta: { title: 'Liên hệ' }, component: () => import('../views/public/ContactPage.vue') },
    ],
  },

  // ══ CỔNG SINH VIÊN (Sau đăng nhập) ══════════
  {
    path: '/student',
    component: () => import('../layouts/StudentLayout.vue'),
    meta: { requiresAuth: true, role: 'Student' },
    children: [
      { path: '', name: 'student-dashboard', meta: { title: 'Trang chủ' }, component: () => import('../views/student/StudentDashboard.vue') },
      { path: 'rooms', name: 'student-rooms', meta: { title: 'Đăng ký phòng' }, component: () => import('../views/student/StudentRoomListView.vue') },
      { path: 'room-registration', name: 'student-room-registration', meta: { title: 'Đăng ký phòng' }, component: () => import('../views/student/StudentRoomRegistrationView.vue') },
      { path: 'my-room', name: 'my-room', meta: { title: 'Phòng của tôi' }, component: () => import('../views/student/MyRoomView.vue') },
      { path: 'my-contract', name: 'my-contract', meta: { title: 'Hợp đồng' }, component: () => import('../views/student/MyContractView.vue') },
      { path: 'my-payments', name: 'my-payments', meta: { title: 'Thanh toán' }, component: () => import('../views/student/MyPaymentsView.vue') },
      { path: 'maintenance', name: 'student-maintenance', meta: { title: 'Yêu cầu sửa chữa' }, component: () => import('../views/student/StudentMaintenanceView.vue') },
      { path: 'notifications', name: 'student-notifications', meta: { title: 'Thông báo' }, component: () => import('../views/student/NotificationsView.vue') },
      { path: 'profile', name: 'student-profile', meta: { title: 'Hồ sơ cá nhân' }, component: () => import('../views/student/StudentProfileView.vue') },
    ],
  },

  // ══ CỔNG NHÂN VIÊN (Sau đăng nhập) ══════════
  {
    path: '/staff',
    component: () => import('../layouts/DefaultLayoutAnt.vue'),
    meta: { requiresAuth: true, role: 'Staff' },
    children: [
      { path: '', redirect: '/staff/dashboard' },
      
      // Dashboard
      { path: 'dashboard', name: 'staff-dashboard', meta: { title: 'Dashboard Nhân viên' }, component: () => import('../views/staff/StaffDashboard.vue') },
      
      // === VẬN HÀNH - NGHIỆP VỤ CHÍNH ===
      
      // Xử lý đơn đăng ký
      { path: 'room-applications', name: 'staff-room-applications', meta: { title: 'Đơn đăng ký phòng' }, component: () => import('../views/staff/StaffRoomApplicationsView.vue') },
      
      // Quản lý hợp đồng & Chuyển phòng
      { path: 'contracts', name: 'staff-contracts', meta: { title: 'Hợp đồng' }, component: () => import('../views/contracts/ContractListView.vue') },
      { path: 'contract-extensions', name: 'staff-contract-extensions', meta: { title: 'Gia hạn hợp đồng' }, component: () => import('../views/contracts/ContractExtensionsView.vue') },
      { path: 'room-transfers', name: 'staff-room-transfers', meta: { title: 'Chuyển phòng' }, component: () => import('../views/contracts/RoomTransfersView.vue') },
      
      // Check-in / Check-out
      { path: 'checkin-checkout', name: 'staff-checkin-checkout', meta: { title: 'Check-in/Check-out' }, component: () => import('../views/staff/StaffCheckinCheckoutView.vue') },
      { path: 'room-inspections', name: 'staff-room-inspections', meta: { title: 'Kiểm tra phòng' }, component: () => import('../views/room-inspections/RoomInspectionListView.vue') },
      
      // === TÀI CHÍNH ===
      
      // Ghi nhận thanh toán
      { path: 'payment-processing', name: 'staff-payment-processing', meta: { title: 'Ghi nhận thanh toán' }, component: () => import('../views/staff/StaffPaymentProcessingView.vue') },
      { path: 'invoices', name: 'staff-invoices', meta: { title: 'Hóa đơn' }, component: () => import('../views/billing/InvoiceList.vue') },
      { path: 'payments', name: 'staff-payments', meta: { title: 'Lịch sử thanh toán' }, component: () => import('../views/billing/PaymentList.vue') },
      { path: 'debt', name: 'staff-debt', meta: { title: 'Công nợ' }, component: () => import('../views/debt/DebtListView.vue') },
      
      // === BẢO TRÌ ===
      
      // Quản lý bảo trì
      { path: 'maintenance-requests', name: 'staff-maintenance-requests', meta: { title: 'Yêu cầu bảo trì' }, component: () => import('../views/staff/StaffMaintenanceView.vue') },
      
      // === SINH VIÊN ===
      
      { path: 'students', name: 'staff-students', meta: { title: 'Danh sách sinh viên' }, component: () => import('../views/students/StudentListView.vue') },
      
      // === BÁO CÁO ===
      
      { path: 'reports', name: 'staff-reports', meta: { title: 'Báo cáo' }, component: () => import('../views/staff/StaffReportsView.vue') },
    ],
  },

  // ══ TRANG QUẢN TRỊ (ADMIN) ═════════════════
  {
    path: '/admin',
    component: () => import('../layouts/DefaultLayoutAnt.vue'),
    meta: { requiresAuth: true, role: 'Admin' },
    children: [
      { path: '', name: 'dashboard', meta: { title: 'Dashboard' }, component: () => import('../views/dashboard/DashboardViewAnt.vue') },
      
      // === CẤU HÌNH HỆ THỐNG (Admin Only) ===
      { path: 'buildings', name: 'buildings', meta: { title: 'Quản lý tòa nhà' }, component: () => import('../views/buildings/BuildingListView.vue') },
      { path: 'room-types', name: 'room-types', meta: { title: 'Loại phòng' }, component: () => import('../views/room-types/RoomTypeListView.vue') },
      { path: 'rooms', name: 'rooms', meta: { title: 'Quản lý phòng' }, component: () => import('../views/rooms/RoomListView.vue') },
      { path: 'amenities', name: 'amenities', meta: { title: 'Tiện nghi' }, component: () => import('../views/amenities/AmenityListView.vue') },
      { path: 'users', name: 'users', meta: { title: 'Người dùng' }, component: () => import('../views/users/UserListView.vue') },
      
      // === VẬN HÀNH ===
      { path: 'floor-map', name: 'floor-map', meta: { title: 'Sơ đồ tầng' }, component: () => import('../views/floor-map/FloorMapView.vue') },
      { path: 'students', name: 'students', meta: { title: 'Sinh viên' }, component: () => import('../views/students/StudentListView.vue') },
      { path: 'student-documents', name: 'student-documents', meta: { title: 'Tài liệu sinh viên' }, component: () => import('../views/students/StudentDocumentsView.vue') },
      { path: 'registrations', name: 'registrations', meta: { title: 'Đăng ký phòng' }, component: () => import('../views/registrations/RegistrationListView.vue') },
      { path: 'room-applications', name: 'room-applications', meta: { title: 'Đơn đăng ký phòng' }, component: () => import('../views/registrations/RoomApplicationListView.vue') },
      { path: 'contracts', name: 'contracts', meta: { title: 'Hợp đồng' }, component: () => import('../views/contracts/ContractListView.vue') },
      { path: 'contract-extensions', name: 'contract-extensions', meta: { title: 'Gia hạn hợp đồng' }, component: () => import('../views/contracts/ContractExtensionsView.vue') },
      { path: 'room-transfers', name: 'room-transfers', meta: { title: 'Chuyển phòng' }, component: () => import('../views/contracts/RoomTransfersView.vue') },
      
      // === TÀI CHÍNH ===
      { path: 'billing', name: 'billing', meta: { title: 'Hóa đơn' }, component: () => import('../views/billing/BillingListView.vue') },
      { path: 'invoices', name: 'invoices', meta: { title: 'Phiếu thu' }, component: () => import('../views/billing/InvoiceList.vue') },
      { path: 'payments', name: 'payments', meta: { title: 'Thanh toán' }, component: () => import('../views/billing/PaymentList.vue') },
      { path: 'debt', name: 'debt', meta: { title: 'Công nợ' }, component: () => import('../views/debt/DebtListView.vue') },
      
      // === BẢO TRÌ ===
      { path: 'maintenance', name: 'maintenance', meta: { title: 'Bảo trì' }, component: () => import('../views/maintenance/MaintenanceListView.vue') },
      { path: 'maintenance-requests', name: 'maintenance-requests', meta: { title: 'Yêu cầu sửa chữa' }, component: () => import('../views/maintenance/MaintenanceRequestList.vue') },
      { path: 'room-inspections', name: 'room-inspections', meta: { title: 'Kiểm tra phòng' }, component: () => import('../views/room-inspections/RoomInspectionListView.vue') },
      
      // === THÔNG BÁO ===
      { path: 'announcements', name: 'announcements', meta: { title: 'Thông báo' }, component: () => import('../views/announcements/AnnouncementListView.vue') },
      { path: 'notifications', name: 'notifications', meta: { title: 'Gửi thông báo' }, component: () => import('../views/notifications/NotificationManagementView.vue') },
    ],
  },

  // ══ LOGIN ═══════════════════════════════════
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/auth/LoginView.vue'),
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() { return { top: 0 } },
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  const user = JSON.parse(localStorage.getItem('user') || 'null')

  // Prevent infinite loops - if we're already navigating to the target, just proceed
  if (from.path === to.path) {
    next()
    return
  }

  // Public routes that don't require authentication
  const publicPaths = ['/', '/about', '/rooms', '/news', '/rules', '/contact', '/login']
  const isPublicRoute = publicPaths.includes(to.path) || to.path.startsWith('/public')
  
  // If user is logged in and tries to access public HOME page (not login), redirect to dashboard
  if (token && user && to.path === '/') {
    if (user.role === 'Student') {
      return next('/student')
    } else if (user.role === 'Staff') {
      return next('/staff/dashboard')
    } else if (user.role === 'Admin') {
      return next('/admin')
    }
  }
  
  // Allow access to login page regardless of authentication status
  // This allows users to logout and login again
  if (to.path === '/login') {
    return next()
  }
  
  // Allow access to other public routes
  if (isPublicRoute) {
    return next()
  }

  // Check authentication for protected routes
  if (!token || !user) {
    return next('/login')
  }

  // Role-based access control for protected routes
  const userRole = user.role

  // Admin routes
  if (to.path.startsWith('/admin')) {
    if (userRole === 'Admin') {
      return next()
    } else if (userRole === 'Staff') {
      // Staff should use /staff routes instead
      return next('/staff/dashboard')
    } else {
      return next('/student')
    }
  }

  // Staff routes
  if (to.path.startsWith('/staff')) {
    if (userRole === 'Staff' || userRole === 'Admin') {
      return next()
    } else {
      return next('/student')
    }
  }

  // Student routes
  if (to.path.startsWith('/student')) {
    if (userRole === 'Student') {
      return next()
    } else if (userRole === 'Staff') {
      return next('/staff/dashboard')
    } else {
      return next('/admin')
    }
  }

  // Default: allow navigation
  next()
})

export default router
