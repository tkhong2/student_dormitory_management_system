// Copy và paste đoạn code này vào Console của trình duyệt (F12 -> Console)
const user = JSON.parse(localStorage.getItem('user'));
console.log('=== THÔNG TIN TÀI KHOẢN ===');
console.log('User ID:', user.id);
console.log('Username:', user.username);
console.log('Full Name:', user.fullName);
console.log('Email:', user.email);
console.log('Role:', user.role);
console.log('========================');
