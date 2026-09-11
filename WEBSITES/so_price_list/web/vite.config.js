import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// The IIS site is served from /SoPriceList, matching its siblings under
// V:\ESHWeb (SoStatus, CmrStatus, ...). Assets go to static/js and static/css
// so the deployed folder has the same shape those apps do.
export default defineConfig({
  plugins: [react()],
  base: '/SoPriceList/',
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    sourcemap: false,
    rollupOptions: {
      output: {
        entryFileNames: 'static/js/[name].[hash].js',
        chunkFileNames: 'static/js/[name].[hash].js',
        assetFileNames: ({ name }) =>
          /\.css$/.test(name ?? '')
            ? 'static/css/[name].[hash][extname]'
            : 'static/media/[name].[hash][extname]'
      }
    }
  },
  server: {
    port: 3000,
    proxy: {
      // dev only - in production the app calls the API host directly
      '/price_list': { target: 'http://172.16.3.2:3001', changeOrigin: true }
    }
  }
});
