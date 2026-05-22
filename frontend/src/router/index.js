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
    children: [
      { path: '', name: 'student-dashboard', meta: { title: 'Trang chủ' }, component: () => import('../views/student/StudentDashboard.vue') },
      { path: 'my-room', name: 'my-room', meta: { title: 'Phòng của tôi' }, component: () => import('../views/student/MyRoomView.vue') },
      { path: 'my-contract', name: 'my-contract', meta: { title: 'Hợp đồng' }, component: () => import('../views/student/MyContractView.vue') },
      { path: 'my-payments', name: 'my-payments', meta: { title: 'Thanh toán' }, component: () => import('../views/student/MyPaymentsView.vue') },
      { path: 'maintenance', name: 'student-maintenance', meta: { title: 'Yêu cầu sửa chữa' }, component: () => import('../views/student/StudentMaintenanceView.vue') },
      { path: 'profile', name: 'student-profile', meta: { title: 'Hồ sơ cá nhân' }, component: () => import('../views/student/StudentProfileView.vue') },
    ],
  },

  // ══ TRANG QUẢN TRỊ ══════════════════════════
  {
    path: '/admin',
    component: () => import('../layouts/DefaultLayout.vue'),
    children: [
      { path: '', name: 'dashboard', meta: { title: 'Dashboard' }, component: () => import('../views/dashboard/DashboardView.vue') },
      { path: 'buildings', name: 'buildings', meta: { title: 'Tòa nhà' }, component: () => import('../views/buildings/BuildingListView.vue') },
      { path: 'room-types', name: 'room-types', meta: { title: 'Loại phòng' }, component: () => import('../views/room-types/RoomTypeListView.vue') },
      { path: 'rooms', name: 'rooms', meta: { title: 'Phòng ở' }, component: () => import('../views/rooms/RoomListView.vue') },
      { path: 'floor-map', name: 'floor-map', meta: { title: 'Sơ đồ tầng' }, component: () => import('../views/floor-map/FloorMapView.vue') },
      { path: 'students', name: 'students', meta: { title: 'Sinh viên' }, component: () => import('../views/students/StudentListView.vue') },
      { path: 'registrations', name: 'registrations', meta: { title: 'Đăng ký phòng' }, component: () => import('../views/registrations/RegistrationListView.vue') },
      { path: 'contracts', name: 'contracts', meta: { title: 'Hợp đồng' }, component: () => import('../views/contracts/ContractListView.vue') },
      { path: 'billing', name: 'billing', meta: { title: 'Hóa đơn' }, component: () => import('../views/billing/BillingListView.vue') },
      { path: 'debt', name: 'debt', meta: { title: 'Công nợ' }, component: () => import('../views/debt/DebtListView.vue') },
      { path: 'maintenance', name: 'maintenance', meta: { title: 'Bảo trì' }, component: () => import('../views/maintenance/MaintenanceListView.vue') },
      { path: 'users', name: 'users', meta: { title: 'Người dùng' }, component: () => import('../views/users/UserListView.vue') },
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

export default router
