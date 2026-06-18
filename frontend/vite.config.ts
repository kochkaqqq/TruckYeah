import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import svgr from 'vite-plugin-svgr'

export default defineConfig({
  plugins: [react(), svgr()],
  server: {
    proxy: {
      '/api/users': 'http://localhost:5001',
      '/api/countries': 'http://localhost:5001',
      '/api/companies': 'http://localhost:5001',
      '/api/comments': 'http://localhost:5001',
      '/Cargos': 'http://localhost:5002',
      '/Trucks': 'http://localhost:5002',
      '/Orders': 'http://localhost:5002',
      '/Chats': 'http://localhost:5003',
      '/Routes': 'http://localhost:5004',
    },
  },
  resolve: {
    alias: {
      '@': '/src',
    },
  },
})
