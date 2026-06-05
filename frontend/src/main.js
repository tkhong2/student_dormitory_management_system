import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import router from './router'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import './style.css'
import { useAuthStore } from './stores/auth'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.use(vuetify)
app.use(Antd)

// Initialize auth interceptor
const authStore = useAuthStore()
authStore.initializeInterceptor()

app.mount('#app')
