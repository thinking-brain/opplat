import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const runningInAspire = env.OPPLAT_RUNNING_IN_ASPIRE === 'true'

  return {
    plugins: [react()],
    server: {
      host: env.HOST || '127.0.0.1',
      port: Number(env.PORT || env.VITE_PORT || '3200'),
      strictPort: runningInAspire,
      open: !runningInAspire
    }
  }
})
