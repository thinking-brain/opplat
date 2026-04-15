import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const runningInAspire = env.OPPLAT_RUNNING_IN_ASPIRE === 'true'
  const proxyTarget = env.VITE_DEV_PROXY_TARGET
    || env.VITE_ADMIN_API_URL
    || 'http://localhost:8084'
  const proxyOptions = {
    target: proxyTarget,
    changeOrigin: false,
  }

  return {
    plugins: [react()],
    server: {
      host: env.HOST || '127.0.0.1',
      port: Number(env.PORT || env.VITE_PORT || '3200'),
      strictPort: runningInAspire,
      open: !runningInAspire,
      proxy: {
        '/admin': proxyOptions,
        '/public': proxyOptions,
      },
    },
  }
})
