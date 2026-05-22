import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import '@mdi/font/css/materialdesignicons.css'

const lightTheme = {
  dark: false,
  colors: {
    background: '#ffffff',
    surface: '#ffffff',
    'surface-variant': '#f9fafb',
    primary: '#ff6b00',
    'primary-darken-1': '#e66000',
    secondary: '#1e3a8a',
    accent: '#f59e0b',
    error: '#ef4444',
    info: '#0ea5e9',
    success: '#22c55e',
    warning: '#f97316',
    'on-background': '#0f172a',
    'on-surface': '#0f172a',
  },
}

export default createVuetify({
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: { mdi },
  },
  theme: {
    defaultTheme: 'lightTheme',
    themes: { lightTheme },
  },
  defaults: {
    VCard: {
      rounded: 'lg',
      elevation: 0,
    },
    VBtn: {
      rounded: 'lg',
      elevation: 0,
      style: 'text-transform: none; font-weight: 600; letter-spacing: 0;',
    },
    VTextField: {
      variant: 'outlined',
      density: 'comfortable',
      color: 'primary',
    },
    VSelect: {
      variant: 'outlined',
      density: 'comfortable',
      color: 'primary',
    },
    VTextarea: {
      variant: 'outlined',
      color: 'primary',
    },
    VDataTable: {
      hover: true,
    },
  },
})
