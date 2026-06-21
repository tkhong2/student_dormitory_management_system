import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vuetify({ autoImport: true }),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  optimizeDeps: {
    exclude: ['core-js'],
  },
  server: {
    port: 5173,
    host: true,
    watch: {
      usePolling: false,
      ignored: ['**/node_modules/**', '**/.git/**']
    },
    fs: {
      strict: false
    }
  }
})
